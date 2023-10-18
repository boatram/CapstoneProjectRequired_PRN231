

using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Stream = System.IO.Stream;
using Task = System.Threading.Tasks.Task;
using static System.Formats.Asn1.AsnWriter;
using Microsoft.AspNetCore.Http;

namespace Repository
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IConfiguration _config;
        private String ApiKey;
        private static string Bucket;
        private static string AuthEmail;
        private static string AuthPassword;
        public FileStorageService(IConfiguration config)
        {
            _config = config;
            ApiKey = _config["Firebase:ApiKey"];
            Bucket = _config["Firebase:Bucket"];
            AuthEmail = _config["EmailUserName"];
            AuthPassword = _config["EmailPassword"];
        }

        public async Task<string> UploadFileToDefaultAsync(Stream fileStream, string fileName)
        {
            var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);

            var cancellation = new CancellationTokenSource();
            var task = new FirebaseStorage(Bucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    ThrowOnCancel = true
                }
                ).Child("images").Child(fileName).PutAsync(fileStream, cancellation.Token);
            try
            {
                string link = await task;
                return link;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Task<FileStreamResult> ExportStudentToExcel()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
               AppDomain.CurrentDomain.RelativeSearchPath ?? "");
            var resourcePath = Path.Combine(path, @"Resources");
            var filePath = Path.Combine(resourcePath, @"GeneralReportTemplate.xlsx");
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
            string fileName = "AddAccountTemplate_" + ".xlsx";

            using (ExcelPackage package = new ExcelPackage(fileStream))
            {
                int sheetIndex = 0;
                ExcelWorksheet ws = package.Workbook.Worksheets[sheetIndex];
                char startHeaderChar = 'A';
                int startHeaderNumber = 5;
                int driverHeaderReport = 6;

                var nfi = new NumberFormatInfo { NumberGroupSeparator = "." };

                char startHeaderCharStore = 'A';
                int startHeaderNumberStore = 1;
                ws.Columns.Style.WrapText = true;
                ws.Columns.Width = 15;
                ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["" + (startHeaderCharStore) + (startHeaderNumberStore)].Style.Font.Bold = true;
                ws.Cells["" + (startHeaderCharStore) + (startHeaderNumberStore)].Style.Fill.PatternType =
                    ExcelFillStyle.DarkDown;
                ws.Cells["" + (startHeaderCharStore) + (startHeaderNumberStore)].Style.Fill.BackgroundColor
                    .SetColor(Color.Lime);
                ws.Cells["" + (startHeaderCharStore++) + (startHeaderNumberStore)].Value = "Code";
                ws.Cells["" + (startHeaderCharStore) + (startHeaderNumberStore)].Style.Font.Bold = true;
                ws.Cells["" + (startHeaderCharStore) + (startHeaderNumberStore)].Style.Fill.PatternType =
                    ExcelFillStyle.DarkDown;
                ws.Cells["" + (startHeaderCharStore) + (startHeaderNumberStore)].Style.Fill.BackgroundColor
                    .SetColor(Color.Lime);
                ws.Cells["" + (startHeaderCharStore++) + (startHeaderNumberStore)].Value = "Name";
                ws.Cells["" + (startHeaderCharStore) + (startHeaderNumberStore)].Style.Font.Bold = true;
                ws.Cells["" + (startHeaderCharStore) + (startHeaderNumberStore)].Style.Fill.PatternType =
                    ExcelFillStyle.DarkDown;
                ws.Cells["" + (startHeaderCharStore) + (startHeaderNumberStore)].Style.Fill.BackgroundColor
                    .SetColor(Color.Lime);
                ws.Cells["" + (startHeaderCharStore++) + (startHeaderNumberStore)].Value = "Email";
                ws.Cells["" + (startHeaderCharStore) + (startHeaderNumberStore)].Style.Font.Bold = true;
                ws.Cells["" + (startHeaderCharStore) + (startHeaderNumberStore)].Style.Fill.PatternType =
                    ExcelFillStyle.DarkDown;
                ws.Cells["" + (startHeaderCharStore) + (startHeaderNumberStore)].Style.Fill.BackgroundColor
                    .SetColor(Color.Green);

                ws.Cells["" + (startHeaderCharStore) + (startHeaderNumberStore)].Style.Fill.BackgroundColor
                    .SetColor(Color.Lime);
                ws.Cells["" + (startHeaderCharStore++) + (startHeaderNumberStore)].Value = "Gender";
                ws.Cells["" + (startHeaderCharStore) + (startHeaderNumberStore)].Style.Font.Bold = true;
                ws.Cells["" + (startHeaderCharStore) + (startHeaderNumberStore)].Style.Fill.PatternType =
                    ExcelFillStyle.DarkDown;

                ws.Cells["" + (startHeaderCharStore) + (startHeaderNumberStore)].Style.Fill.BackgroundColor
                    .SetColor(Color.Lime);
                ws.Cells["" + (startHeaderCharStore++) + (startHeaderNumberStore)].Value =
                    "Date Of Birth (dd/mm/yyyy)";
                ws.Cells["" + (startHeaderCharStore) + (startHeaderNumberStore)].Style.Font.Bold = true;
                ws.Cells["" + (startHeaderCharStore) + (startHeaderNumberStore)].Style.Fill.PatternType =
                    ExcelFillStyle.DarkDown;
                ws.Cells["" + (startHeaderCharStore) + (startHeaderNumberStore)].Style.Fill.BackgroundColor
                    .SetColor(Color.Lime);
                ws.Cells["" + (startHeaderCharStore++) + (startHeaderNumberStore)].Value =
                    "Phone";
                MemoryStream ms = new MemoryStream();
                package.SaveAs(ms);
                ms.Seek(0, SeekOrigin.Begin);
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                fileStream.Close();
                return Task.FromResult( new  FileStreamResult(ms, contentType)
                {
                    FileDownloadName = fileName
                });
            }
        }


    }
}

