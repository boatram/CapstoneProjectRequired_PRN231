using AutoMapper;
using BusinessObjects.BusinessObjects;
using BusinessObjects.DTOs;
using BusinessObjects.DTOs.Request;
using BusinessObjects.DTOs.Response;
using DataAccess;
using Hangfire;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

        public async Task<List<TopicResponse>> Create(IFormFile file, int id)
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
                                var lecturer = accountRepository.GetAccountById(id).Result;
                                if(lecturer == null)
                                    throw new CrudException(HttpStatusCode.NotFound, "Lecturer not found !!!", "");
                                if(lecturer.RoleId !=2)
                                    throw new CrudException(HttpStatusCode.BadRequest, "Import is not allowed !!!", "");
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
                                    if(spec.Status==false) throw new CrudException(HttpStatusCode.BadRequest, "Specialiazation is not available !!!", "");
                                    var semester=semesterRepository.GetSemesters().Result.Where(a=>a.Code.Equals(topic.SemesterCode)).SingleOrDefault();
                                    if(semester == null) throw new CrudException(HttpStatusCode.NotFound, "Semester not found !!!", "");
                                    if(semester.Status != null && semester.Status ==false) throw new CrudException(HttpStatusCode.BadRequest, $"Semester {semester.Code} has finished, please enter the current semester !!!", "");
                                    if (TopicDAO.Instance.GetTopics().Where(a=>a.Name.Equals(topic.Name)).SingleOrDefault()!=null)
                                        throw new CrudException(HttpStatusCode.BadRequest, $"Topic {topic.Name} has already created !!!", "");
                                    sub.Status = true;
                                    sub.SpecializationId = spec.Id;
                                    sub.SemesterId = semester.Id;
                                    List<TopicOfLecturer> l = new List<TopicOfLecturer>();
                                    TopicOfLecturer t=new TopicOfLecturer();
                                    t.IsSuperLecturer = true;
                                    t.LecturerId = id;
                                    t.Status = true;
                                    l.Add(t);
                                    sub.TopicOfLecturers = l;
                                    TopicDAO.Instance.CreateTopic(sub);

                                    var datespan = (semester.EndDate.Value.Date - DateTime.Now.Date).TotalHours;
                                    BackgroundJob.Schedule(() =>
                                        AutoUpdateStatusTopic(semester.Code),
                                                TimeSpan.FromHours(datespan));
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
                throw new CrudException(HttpStatusCode.BadRequest, "Create Topic Error!!!", ex?.Message);
            }
        }
        public async Task AutoUpdateStatusTopic(string code)
        {
            var topic = TopicDAO.Instance.GetTopics();
            if (topic != null)
            {
                foreach (var s in topic)
                {
                    if (s.Semester.Code ==code)
                    {
                        s.Status = false;
                        TopicDAO.Instance.UpdateStatus(s.Id);
                    }
                }

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
        public Task<TopicResponse> Update(int topicid, int groupId)
        {
            try
            {
                if (topicid == null || groupId == null)
                    throw new CrudException(HttpStatusCode.BadRequest, "Invalid Information !!!!!", "");
                var topicGroup= TopicDAO.Instance.GetTopics().Where(a=>a.Id==topicid).SingleOrDefault();
                if(topicGroup == null)
                    throw new CrudException(HttpStatusCode.NotFound, "Topic not found !!!", "");
                var topic=TopicOfGroupDAO.Instance.GetTopicOfGroupByID(topicid,groupId);
                TopicOfGroupDAO.Instance.Update(topic, topicid);
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
                }).SingleOrDefault(x=>x.Id==topicid));
            }
            catch (CrudException ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Update topic status error!!!!!", ex.Message);
            }
           
        }

        public void UpdateStatus(int Id) => TopicDAO.Instance.UpdateStatus(Id);
    }
}
