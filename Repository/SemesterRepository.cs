using AutoMapper;
using BusinessObjects;
using BusinessObjects.BusinessObjects;
using BusinessObjects.DTOs.Request;
using BusinessObjects.DTOs.Response;
using DataAccess;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using Repository.Exceptions;
using Repository.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Repository.Helpers.Enum;

namespace Repository
{
    public class SemesterRepository : ISemesterRepository
    {
        private readonly IMapper _mapper;
        private readonly IAccountRepository accountRepository;
        private readonly ISubjectRepository subjectRepository;
        public SemesterRepository(IMapper mapper,IAccountRepository accountRepository,ISubjectRepository subjectRepository)
        {
            _mapper = mapper;
            this.accountRepository = accountRepository;
            this.subjectRepository = subjectRepository;
        }

        public void Create(SemesterRequest semester) => SemesterDAO.Instance.Create(semester);

        public Semester GetSemesterByID(int? Id) => SemesterDAO.Instance.GetSemesterByID(Id);

        public void Update(int Id) => SemesterDAO.Instance.Update(Id);
     

        IEnumerable<SemesterResponse> ISemesterRepository.GetSemesters() => SemesterDAO.Instance.GetSemesters();
        public async Task<List<StudentInSemesterResponse>> CreateStudentInSemester(IFormFile file)
        {
            try
            {
                List<StudentInSemesterRequest> list = new List<StudentInSemesterRequest>();
                List<StudentInSemesterResponse> result = new List<StudentInSemesterResponse>();
                if (file != null)
                {
                    using (ExcelPackage package = new ExcelPackage(file.OpenReadStream()))
                    {
                        ExcelWorkbook workbook = package.Workbook;
                        if (workbook != null)
                        {
                            ExcelWorksheet worksheet = workbook.Worksheets.FirstOrDefault();
                            if (worksheet != null)
                            {
                                list = worksheet.ReadExcelToList<StudentInSemesterRequest>();
                                foreach (StudentInSemesterRequest account in list)
                                {
                                    var customer = _mapper.Map<StudentInSemesterRequest,StudentInSemester>(account);
                                    
                                    var s = accountRepository.GetAccounts().Result.Where(s => s.Code == account.StudentCode && s.RoleId==1).SingleOrDefault();
                                    if (s == null)
                                    {
                                        throw new CrudException(HttpStatusCode.NotFound, "Student not found !!!", "");
                                    }
                                    var semester = SemesterDAO.Instance.GetSemesters().Where(s => s.Code == account.SemesterCode).SingleOrDefault();
                                    if (semester == null)
                                    {
                                        throw new CrudException(HttpStatusCode.NotFound, "Semester not found !!!", "");
                                    }
                                    var subject = subjectRepository.GetSubjects().Result.Where(s => s.Code == account.SubjectCode).SingleOrDefault();
                                    if (subject == null)
                                    {
                                        throw new CrudException(HttpStatusCode.NotFound, "Subject not found !!!", "");
                                    }
                                    if(subject.Specialization.Id != s.Specialization.Id)
                                    {
                                        throw new CrudException(HttpStatusCode.BadRequest, "Specialization does not have this subject !!!", "");
                                    }
                                    customer.SubjectId = subject.Id;
                                    customer.SemesterId = semester.Id;
                                    customer.StudentId = s.Id;
                                    var rs = StudentInSemesterDAO.Instance.GetStudentInSemesters().ToList();
                                    if (rs != null && rs.Count() > 0)
                                    {
                                        var stu = rs.Where(a => a.Semester.Code == account.SemesterCode && a.Subject.Code == account.SubjectCode && a.Student.Code == account.StudentCode).SingleOrDefault();
                                        if(stu !=null)
                                        {
                                            if (stu.Status == account.Status) continue;
                                            stu.SemesterId = semester.Id;
                                            stu.Status = account.Status;
                                            StudentInSemesterDAO.Instance.Update(stu, stu.Id);
                                        }
                                        else StudentInSemesterDAO.Instance.AddNew(customer);
                                    }
                                    else StudentInSemesterDAO.Instance.AddNew(customer);

                                }
                                return result = StudentInSemesterDAO.Instance.GetStudentInSemesters().Select(a => new StudentInSemesterResponse
                                {
                                    Id = a.Id,
                                    SemesterId = a.SemesterId,
                                    Status = a.Status,
                                    StudentId = a.StudentId,
                                    SubjectId = a.SubjectId,
                                    SemesterCode = a.Semester.Code,
                                    StudentCode = a.Student.Code,
                                    Subject = _mapper.Map<SubjectResponse>(a.Subject)
                                }).ToList();
                            }
                        }
                    }
                }
                return null;
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Progress Error!!!", ex?.Message);
            }
        }
    }
}
