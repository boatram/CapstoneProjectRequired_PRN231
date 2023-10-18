using BusinessObjects.Commons;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using Repository.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Helpers
{
    public static class ImportExcelReader
    {
        public static List<T> ReadExcelToList<T>(this ExcelWorksheet worksheet) where T : new()
        {           
            ExcelTable table = null;

            if (worksheet.Tables.Any())
            {
                table = worksheet.Tables.FirstOrDefault();
            }
            else
            {
                table = worksheet.Tables.Add(worksheet.Dimension, "tbl" + typeof(T).Name);

                ExcelAddressBase newaddy = new ExcelAddressBase(table.Address.Start.Row, table.Address.Start.Column, table.Address.End.Row + 1, table.Address.End.Column);

                //Edit the raw XML by searching for all references to the old address
                table.TableXml.InnerXml = table.TableXml.InnerXml.Replace(table.Address.ToString(), newaddy.ToString());
            }

            //Get the cells based on the table address
            List<IGrouping<int, ExcelRangeBase>> groups = table.WorkSheet.Cells[table.Address.Start.Row, table.Address.Start.Column, table.Address.End.Row, table.Address.End.Column]
                .GroupBy(cell => cell.Start.Row)
                .ToList();

            //Assume the second row represents column data types (big assumption!)
            List<Type> types = groups.Skip(1).FirstOrDefault().Select(rcell => rcell.Value.GetType()).ToList();

            //Get the properties of T
            List<PropertyInfo> modelProperties = new T().GetType().GetProperties().ToList();

            //Assume first row has the column names
            var colnames = groups.FirstOrDefault()
                .Select((hcell, idx) => new
                {
                    Name = hcell.Value.ToString(),
                    index = idx
                })
                .Where(o => modelProperties.Select(p => p.Name).Contains(o.Name))
                .ToList();
            //Everything after the header is data
            List<List<object>> rowvalues = groups
                .Skip(1) //Exclude header
                .Select(cg => cg.Select(c => c.Value).ToList()).ToList();

            //Create the collection container
            List<T> collection = new List<T>();
            foreach (List<object> row in rowvalues)
            {
                T tnew = new T();
                foreach (var colname in colnames)
                {
                    //This is the real wrinkle to using reflection - Excel stores all numbers as double including int
                    object val = row[colname.index];
                    Type type = types[colname.index];
                    PropertyInfo prop = modelProperties.FirstOrDefault(p => p.Name == colname.Name);
                        var unboxedVal = val;

                    //FAR FROM A COMPLETE LIST!!!
                    if (prop.CustomAttributes.Any(
                                (Func<CustomAttributeData, bool>)(a => a.AttributeType == typeof(IntAttribute))))
                    {
                        prop.SetValue(tnew, (int)unboxedVal);
                    }
                    else if (prop.PropertyType == typeof(double))
                    {
                        prop.SetValue(tnew, unboxedVal);
                    }
                    else if (prop.CustomAttributes.Any(
                                (Func<CustomAttributeData, bool>)(a => a.AttributeType == typeof(DateRangeAttribute))))
                        {
                        //double d = double.Parse(unboxedVal.ToString());
                        IFormatProvider provider = new CultureInfo("fr-FR");
                        DateTimeStyles styles = DateTimeStyles.AssumeUniversal;
                        DateTime parsedDate = DateTime.Parse(unboxedVal.ToString(), provider, styles);
                        prop.SetValue(tnew, parsedDate);
                    }
                    else if (prop.CustomAttributes.Any(
                                (Func<CustomAttributeData, bool>)(a => a.AttributeType == typeof(StringAttribute))))
                     {
                        prop.SetValue(tnew, val.ToString().Trim());
                    }
                    else if (prop.CustomAttributes.Any(
                                (Func<CustomAttributeData, bool>)(a => a.AttributeType == typeof(BooleanAttribute))))
                    {
                        if (val.Equals("yes") || val.Equals("Yes") || val.Equals("1")|| val.Equals("Pass") || val.Equals("pass")) val = true;
                        else val = false;
                        prop.SetValue(tnew, val);
                    }
                    else throw new CrudException(HttpStatusCode.BadRequest, string.Format("Type '{0}' not implemented yet!", prop.PropertyType.Name), "");               
                }
                collection.Add(tnew);
            }

            return collection;
        }
    }
}

