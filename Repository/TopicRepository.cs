using AutoMapper;
using BusinessObjects.BusinessObjects;
using BusinessObjects.DTOs;
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

namespace Repository
{
    public class TopicRepository : ITopicRepository
    {
        private readonly IMapper _mapper;
        private readonly ISemesterRepository semesterRepository;
        private readonly ISpecializationRepository specializationRepository;
        private readonly IAccountRepository accountRepository;
        public TopicRepository(IMapper mapper, ISemesterRepository semesterRepository, ISpecializationRepository specializationRepository,IAccountRepository accountRepository)
        {
            _mapper = mapper;
            this.semesterRepository = semesterRepository;
            this.specializationRepository = specializationRepository;
            this.accountRepository = accountRepository;
        }

        public async Task<List<TopicResponse>> Create(IFormFile file)
        {
            try
            {
                List<TopicRequest> list = new List<TopicRequest>();
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
                                list = worksheet.ReadExcelToList<TopicRequest>();
                                var topics = TopicDAO.Instance.GetTopics().ToList();
                                foreach (var t in topics)
                                {
                                     t.Status = false;
                                    TopicDAO.Instance.Update(t, t.Id);
                                }
                                foreach (TopicRequest topic in list)
                                {
                                    var sub = _mapper.Map<TopicRequest, Topic>(topic);
                                    var spec = specializationRepository.GetSpecializations().Where(a => a.Name.Equals(topic.SpecializationName)).SingleOrDefault();
                                    if (spec == null)
                                        throw new CrudException(HttpStatusCode.NotFound, "Specialiazation not found !!!", "");
                                    var semester=semesterRepository.GetSemesters().Where(a=>a.Code.Equals(topic.SemesterCode)).SingleOrDefault();
                                    if(semester == null) throw new CrudException(HttpStatusCode.NotFound, "Semester not found !!!", "");
                                    var topic1 = topics.Where(a => a.Name.Equals(topic.Name)).SingleOrDefault();
                                    if (topic1 != null)
                                    {
                                        topic1.Status = true;
                                        topic1.SemesterId = semester.Id;
                                    }
                                    else
                                        sub.Status = true;
                                    sub.SpecializationId = spec.Id;
                                    sub.SemesterId = semester.Id;
                                    TopicDAO.Instance.CreateTopic(sub);
                                }
                                return TopicDAO.Instance.GetTopics().Select(x => new TopicResponse
                                {
                                    Id = x.Id,
                                    Description=x.Description,
                                    Name = x.Name,
                                    Status = x.Status,
                                    SpecializationName =x.Specialization.Name ,
                                    SemesterCode=x.Semester.Code,
                                    SemesterId=x.Semester.Id
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
                throw new CrudException(HttpStatusCode.BadRequest, "Create Subject Error!!!", ex?.Message);
            }
        }
        public Task<IEnumerable<TopicResponse>> GetTopics()
        {
            try
            {
                return Task.FromResult(TopicDAO.Instance.GetTopics().Select(x => new TopicResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    SemesterId = (int)x.SemesterId,
                    SemesterCode = x.Semester.Code,
                    Status = x.Status,
                    SpecializationName = x.Specialization.Name,
                    TopicOfLecturers = new List<TopicOfLecturerResponse>
                    (x.TopicOfLecturers.Select(x => new TopicOfLecturerResponse
                    {
                        LecturerId = (int)x.LecturerId,
                        LecturerEmail = accountRepository.GetAccountById((int)x.LecturerId).Result.Email
                    }))
                }).AsEnumerable());
            }
            catch (CrudException ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get topic list error!!!!!", ex.Message);
            }
        }
        public void UpdateStatus(int Id) => TopicDAO.Instance.UpdateStatus(Id);
    }
}
