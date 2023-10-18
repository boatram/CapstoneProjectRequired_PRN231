using BusinessObjects.BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class StudentInSemesterDAO
    {
          private static StudentInSemesterDAO instance = null;
        public static readonly object instanceLock = new object();
        public static StudentInSemesterDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new StudentInSemesterDAO();
                    }
                    return instance;
                }
            }
        }
        public IEnumerable<StudentInSemester> GetStudentInSemesters()
        {
            var accounts = new List<StudentInSemester>();
            try
            {
                using var context = new CPRContext();
                accounts = context.StudentInSemesters.Include(c => c.Semester).Include(c=>c.Subject).Include(c=>c.Student).ToList();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return accounts;
        }
        public StudentInSemester GetStudentInSemesterByID(int? cusID)
        {
            StudentInSemester account = null;
            try
            {
                using var context = new CPRContext();
                account = context.StudentInSemesters.Include(c => c.Semester).Include(c => c.Subject).Include(c => c.Student).SingleOrDefault(m => m.Id == cusID);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return account;
        }
        public StudentInSemester AddNew(StudentInSemester account)
        {
            try
            {
                StudentInSemester _account = GetStudentInSemesterByID(account.Id);
                if (_account == null)
                {
                    using var context = new CPRContext();
                    context.StudentInSemesters.Add(account);
                    context.SaveChanges();
                    return _account;
                }
                else
                {
                    throw new Exception("The account is already exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public StudentInSemester Update(StudentInSemester account, int id)
        {
            try
            {
                StudentInSemester _account = GetStudentInSemesterByID(id);
                if (_account != null)
                {
                    using var context = new CPRContext();
                    context.StudentInSemesters.Update(account);
                    context.SaveChanges();
                    return account;
                }
                else
                {
                    throw new Exception("The account does not already exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
