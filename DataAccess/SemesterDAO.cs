using BusinessObjects;
using BusinessObjects.BusinessObjects;
using BusinessObjects.DTOs.Request;
using BusinessObjects.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class SemesterDAO
    {
        private static SemesterDAO instance = null;
        public static readonly object instanceLock = new object();
        public static SemesterDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new SemesterDAO();
                    }
                    return instance;
                }
            }
        }
        
        public IEnumerable<SemesterResponse> GetSemesters()
        {
            var semesters = new List<SemesterResponse>();
            try
            {
                using var context = new CPRContext();
                semesters = context.Semesters.Select(a => new SemesterResponse
                {
                    Id = a.Id,
                    Code = a.Code,
                    StartDate = a.StartDate,
                    EndDate = a.EndDate,
                    Status = true,
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return semesters;
        }

        public Semester GetSemesterByID(int? Id)
        {
            Semester semester = null;
            try
            {
                using var context = new CPRContext();
                semester = context.Semesters.SingleOrDefault(m => m.Id == Id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return semester;
        }

        public int GetSemesterIdByCode(string? code)
        {
            int id;
            Semester semester = null;
            try
            {
                using var context = new CPRContext();
                semester = context.Semesters.SingleOrDefault(m => m.Code.Equals(code));
                if (semester != null)
                {
                    id = semester.Id;
                }
                else id = 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return id;
        }

        public void Create(SemesterRequest semester)
        {
            try
            {
                Semester _semester = new Semester();
                _semester.Id = GetSemesterIdByCode(semester.Code);
                using var context = new CPRContext();
                if (_semester.Id == 0)
                {
                    _semester.Code = semester.Code;
                    _semester.StartDate = semester.StartDate;
                    _semester.EndDate = semester.EndDate;
                    _semester.Status = true;
                    context.Semesters.Add(_semester);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("The semester is already exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
       
        public void Update(int Id)
        {
            try
            {
                Semester _semester = GetSemesterByID(Id);
                if (_semester != null)
                {
                    using var context = new CPRContext();
                    _semester.Status = false;
                    context.Semesters.Update(_semester);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("The semester does not already exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
