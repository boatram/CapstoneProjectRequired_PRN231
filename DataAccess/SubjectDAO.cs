using BusinessObjects;
using BusinessObjects.BusinessObjects;
using BusinessObjects.DTOs.Request;
using BusinessObjects.DTOs.Response;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class SubjectDAO
    {
        private static SubjectDAO instance = null;
        public static readonly object instanceLock = new object();
        public static SubjectDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new SubjectDAO();
                    }
                    return instance;
                }
            }
        }
        public IEnumerable<Subject> GetSubjects()
        {
            var subjects = new List<Subject>();
            try
            {
                using var context = new CPRContext();
                subjects = context.Subjects.Include(c=>c.Specialization).ToList();
               
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return subjects;
        }
        public IEnumerable<SubjectResponse> GetSubjectIsPrerequisite(int id)
        {
            var subjects = new List<SubjectResponse>();
            try
            {
                using var context = new CPRContext();
                subjects = context.Subjects.Where(s => s.SpecializationId == id).
                    Where(s => s.IsPrerequisite.Equals(true)).Select( a => new SubjectResponse
                {
                        Id = a.Id,
                        Code = a.Code,
                        Name = a.Name,
                        IsPrerequisite = a.IsPrerequisite,
                        SpecializationId = a.SpecializationId,
                        Status = a.Status,
                }).ToList();
                //subjects = context.Subjects.Where(c => c.SpecializationId == id).Where(c => c.IsPrerequisite.Equals(true)).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return subjects;
        }

        public IEnumerable<SubjectResponse> GetSubjectBySpecializationId(int id)
        {
            var subjects = new List<SubjectResponse>();
            try
            {
                using var context = new CPRContext();
                subjects = context.Subjects.Where(c => c.SpecializationId == id).Select(s => new SubjectResponse
                {
                    Id= s.Id,
                    Code = s.Code,
                    Name = s.Name,
                    IsPrerequisite = s.IsPrerequisite,
                    SpecializationId = s.SpecializationId,
                    Status = s.Status,
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return subjects;
        }

        public Subject GetSubjectByID(int? Id)
        {
            Subject subject = null;
            try
            {
                using var context = new CPRContext();
                subject = context.Subjects.SingleOrDefault(m => m.Id == Id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return subject;
        }

        public Subject GetSubjectByCode(string? code)
        {
            Subject subject = null;
            try
            {
                using var context = new CPRContext();
                subject = context.Subjects.SingleOrDefault(m => m.Code.Equals(code));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return subject;
        }

        public void Create(Subject s)
        {
            try
            {
                using var context = new CPRContext();
                context.Subjects.Add(s);  
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void UpdateStatus(int Id)
        {
            try
            {
                Subject _s = GetSubjectByID(Id);
                if (_s != null)
                {
                    using var context = new CPRContext();
                    if (_s.IsPrerequisite.Equals(true)) _s.IsPrerequisite = false;
                    else _s.IsPrerequisite = true;
                    context.Subjects.Update(_s);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("The Subject does not already exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
