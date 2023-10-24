using AutoMapper;
using BusinessObjects.BusinessObjects;
using BusinessObjects.DTOs.Request;
using BusinessObjects.DTOs.Response;
using DataAccess;
using Hangfire;
using OfficeOpenXml.Style;
using Repository.Exceptions;
using Repository.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Google.Apis.Requests.BatchRequest;

namespace Repository
{
    public class GroupPojectRepository : IGroupProjectRepository
    {
        private readonly IMapper mapper;
        private readonly ISubjectRepository subjectRepository;
        private readonly IAccountRepository accountRepository;
        private readonly ITopicRepository topicRepository;
        private readonly ISemesterRepository semesterRepository;
        public GroupPojectRepository(IMapper mapper,ISubjectRepository subjectRepository, IAccountRepository accountRepository, ITopicRepository topicRepository, ISemesterRepository semesterRepository)
        {
            this.mapper = mapper;
            this.subjectRepository = subjectRepository;
            this.accountRepository = accountRepository;
            this.topicRepository = topicRepository;
            this.semesterRepository = semesterRepository;
        }

        public async Task<GroupProjectResponse> ChangeLeaderInGroup(int groupId, string code)
        {
            try
            {
                var group = GroupDAO.Instance.GetGroupProjectByID(groupId);
                if (group == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, $"Not found group with id{groupId.ToString()}", "");
                }
                var rs = group.StudentInGroups.Where(a => a.Status == true).SingleOrDefault();
                rs.Status = false;
                foreach (var stu in group.StudentInGroups)
                {
                    var studentInGroup = accountRepository.GetAccounts().Result.Where(a => a.Code == code && a.RoleId==1).SingleOrDefault();
                    if (studentInGroup == null)
                    {
                        throw new CrudException(HttpStatusCode.NotFound, $"Not found student with code {code}", "");
                    }

                    if (stu.StudentId==studentInGroup.Id) stu.Status = true;
                }
                GroupDAO.Instance.Update(group, groupId);
                var groupResult = GroupDAO.Instance.GetGroupProjects().Select(x => new GroupProjectResponse()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Status = x.Status,
                    StudentInGroups = mapper.Map<List<StudentInGroupResponse>>(group.StudentInGroups),
                    TopicOfGroups = mapper.Map<List<TopicOfGroupResponse>>(group.TopicOfGroups)
                }).FirstOrDefault();
                return groupResult;
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Change leader error!!!!!", ex.Message);
            }
        }
        private bool CheckStudentHasGroup(int id)
        {
           var s= semesterRepository.GetSemesters().Where(a => a.StartDate <= DateTime.UtcNow && a.EndDate >= DateTime.UtcNow).SingleOrDefault();
            var list = GetGroupProjectBySemester(s.Code).Result .Select(a=>a.StudentInGroups);
            foreach(var project in list)
            {
                if (project.Where(a => a.StudentId == id).Count() > 0) return true;
            }
            return false;
        }

        public async Task<GroupProjectResponse> CreateGroupProject(GroupProjectRequest group)
        {
            try
            {
                GroupProject groupProject = new GroupProject();
                groupProject.Name = group.Name;
                List<StudentInGroup> students = new List<StudentInGroup>();
                List<StudentInGroupResponse>studentInGroups = new List<StudentInGroupResponse>();
                if (CheckStudentHasGroup((int)group.StudentInGroup.StudentId)==true)
                    throw new CrudException(HttpStatusCode.BadRequest, "Students has already create group ", "");
                var acc = accountRepository.GetAccountById((int)group.StudentInGroup.StudentId).Result;
                if (acc.RoleId != 1) throw new CrudException(HttpStatusCode.BadRequest, "Must add members whose role is student ", "");
                var subjects = subjectRepository.GetSubjectIsPrerequisite((int)acc.SpecializationId).Where(a=>a.Code != "SEP490").ToList();
                var account = accountRepository.GetAccountsInSemester().Result.Where(a => a.StudentId == group.StudentInGroup.StudentId && a.Subject.IsPrerequisite == true && a.Status == true && a.Subject.SpecializationId == acc.SpecializationId).ToList();
                if(account != null && account.Count() > 0) 
                {
                    if (account.Count() == subjects.Count())
                    {
                        StudentInGroup student = new StudentInGroup();
                        student.Status = true;
                        student.Role = group.StudentInGroup.Role;
                        student.Description = group.StudentInGroup.Description;
                        student.JoinDate = DateTime.UtcNow;
                        student.StudentId = acc.Id;
                        var rs = mapper.Map<StudentInGroupResponse>(student);
                        rs.StudentCode = acc.Code;
                        rs.StudentName = acc.Name;
                        rs.StudentEmail = acc.Email;
                        students.Add(student);
                        studentInGroups.Add(rs);
                    }
                    }
                else
                    throw new CrudException(HttpStatusCode.BadRequest, "Students are not eligible to create a group ", "");
                groupProject.StudentInGroups=students;
                GroupDAO.Instance.AddNew(groupProject);
                var groupResult = GroupDAO.Instance.GetGroupProjects().Select(x => new GroupProjectResponse()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Status = x.Status,
                    StudentInGroups = studentInGroups,
                }).FirstOrDefault();
                return groupResult;
                }
               
              
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Create group Error!!!", ex?.Message);
            }
        }
        public async Task AutoUpdateStatusTopic(int topicId,int semesterId)
        {
            var group = TopicOfGroupDAO.Instance.GetTopicOfGroups().Where(a => a.TopicId == topicId && a.Topic.SemesterId == semesterId);
            if (group != null)
            {
                if(group.FirstOrDefault().Status==null)
                TopicOfGroupDAO.Instance.Update(group.FirstOrDefault(), group.FirstOrDefault().TopicId);

            }              
        }

        public async Task<GroupProjectResponse> CreatTopicOfGroup(int groupId, int topicId)
        {
            try
            {
                var group = GroupDAO.Instance.GetGroupProjectByID(groupId);

                if (group == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, $"Not found group with id{groupId.ToString()}", "");
                }
                if (3 > group.StudentInGroups.Count() || group.StudentInGroups.Count() > 5)
                    throw new CrudException(HttpStatusCode.BadRequest, "Not enough members to create a group", "");

                var s = semesterRepository.GetSemesters().Where(a => a.StartDate <= DateTime.UtcNow && a.EndDate >= DateTime.UtcNow).SingleOrDefault();
                List<TopicOfGroupResponse> topicOfGroups = new List<TopicOfGroupResponse>();
                var topic = topicRepository.GetTopics().Result.Where(a => a.Id == topicId && a.SemesterId==s.Id && a.Status==true).SingleOrDefault();
                if( topic ==null )
                    throw new CrudException(HttpStatusCode.NotFound, $"Not found topic with id{topicId.ToString()} in this semester", "");
                if (TopicOfGroupDAO.Instance.GetTopicOfGroups().Where(a=>a.TopicId==topicId && a.Topic.SemesterId == topic.SemesterId && a.Status == true).SingleOrDefault() != null)
                    throw new CrudException(HttpStatusCode.BadRequest, "Topc has been already registered for this semester", "");
                if (TopicOfGroupDAO.Instance.GetTopicOfGroups().Where(a => a.GroupProjectId == groupId && a.Topic.SemesterId==topic.SemesterId && a.Status==true || a.GroupProjectId == groupId && a.Topic.SemesterId == topic.SemesterId && a.Status==null).SingleOrDefault() !=null)
                    throw new CrudException(HttpStatusCode.BadRequest, "You have already registered for the topic for this semester", "");
                TopicOfGroup topicOfGroup = new TopicOfGroup();
                topicOfGroup.GroupProjectId = groupId;
                topicOfGroup.TopicId =topic.Id ;
                topicOfGroup.Status = null;
                group.TopicOfGroups.Add(topicOfGroup);
                GroupDAO.Instance.AddTopicOfGroup(topicOfGroup);
                BackgroundJob.Schedule(() =>
                    AutoUpdateStatusTopic(topicId,topic.SemesterId),
                            TimeSpan.FromMinutes(2));
                var topicResponse = mapper.Map<TopicOfGroupResponse>(topicOfGroup);
                topicResponse.SemesterCode = topic.SemesterCode;
                topicResponse.TopicName = topic.Name;
                topicResponse.SpecializationName=topic.SpecializationName;
                topicOfGroups.Add(topicResponse);
                var groupResult = GroupDAO.Instance.GetGroupProjects().Select(x => new GroupProjectResponse()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Status = x.Status,
                    StudentInGroups = mapper.Map<List<StudentInGroupResponse>>(group.StudentInGroups),
                    TopicOfGroups= topicOfGroups
                }).FirstOrDefault();
                return groupResult;
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Create topic of group error!!!!!", ex.Message);
            }
        }

        public async Task<GroupProjectResponse> GetGroupProjectById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Id Group Invalid", "");
                }
                var group = GroupDAO.Instance.GetGroupProjects().Select(x => new GroupProjectResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Status = x.Status,
                    StudentInGroups = new List<StudentInGroupResponse>
                   (x.StudentInGroups.Select(a => new StudentInGroupResponse
                   {
                       Id = a.Id,
                       Status = a.Status,
                       StudentId = a.StudentId,
                       Description = a.Description,
                       GroupId = a.GroupId,
                       JoinDate = a.JoinDate,
                       Role = a.Role,
                       StudentCode = accountRepository.GetAccounts().Result.Where(s => s.Id == a.StudentId).SingleOrDefault().Code,
                       StudentEmail = accountRepository.GetAccounts().Result.Where(s => s.Id == a.StudentId).SingleOrDefault().Email,
                       StudentName = accountRepository.GetAccounts().Result.Where(s => s.Id == a.StudentId).SingleOrDefault().Name,
                   })),
                    TopicOfGroups=new List<TopicOfGroupResponse>
                    (x.TopicOfGroups.Select(a=>new TopicOfGroupResponse
                    {
                        GroupProjectId = a.GroupProjectId,
                        Status=a.Status,
                        TopicId=a.TopicId,
                        TopicName=topicRepository.GetTopics().Result.Where(x=>x.Id==a.TopicId).SingleOrDefault().Name,
                        SemesterCode=topicRepository.GetTopics().Result.Where(x=>x.Id==a.TopicId).SingleOrDefault().SemesterCode,
                        SpecializationName= topicRepository.GetTopics().Result.Where(x => x.Id == a.TopicId).SingleOrDefault().SpecializationName
                    }))
                }).SingleOrDefault(x=>x.Id==id);
                if (group == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, $"Not found group with id {id.ToString()}", "");
                }
                return group;
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get Group By ID Error!!!", ex.InnerException?.Message);
            }
        }

        public async Task<IEnumerable<GroupProjectResponse>> GetGroupProjects()
        {
            try
            {
                var groups = GroupDAO.Instance.GetGroupProjects().Select(x => new GroupProjectResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Status = x.Status,
                    StudentInGroups = new List<StudentInGroupResponse>
                    (x.StudentInGroups.Select(a => new StudentInGroupResponse
                    {
                        Id = a.Id,
                        Status = a.Status,
                        StudentId = a.StudentId,
                        Description = a.Description,
                        GroupId = a.GroupId,
                        JoinDate = a.JoinDate,
                        Role = a.Role,
                        StudentCode = accountRepository.GetAccounts().Result.Where(s => s.Id == a.StudentId).SingleOrDefault().Code,
                        StudentEmail = accountRepository.GetAccounts().Result.Where(s => s.Id == a.StudentId).SingleOrDefault().Email,
                        StudentName = accountRepository.GetAccounts().Result.Where(s => s.Id == a.StudentId).SingleOrDefault().Name,
                    })),
                    TopicOfGroups = new List<TopicOfGroupResponse>
                     (x.TopicOfGroups.Select(a => new TopicOfGroupResponse
                     {
                         GroupProjectId = a.GroupProjectId,
                         Status = a.Status,
                         TopicId = a.TopicId,
                         TopicName = topicRepository.GetTopics().Result.Where(x => x.Id == a.TopicId).SingleOrDefault().Name,
                         SemesterCode = topicRepository.GetTopics().Result.Where(x => x.Id == a.TopicId).SingleOrDefault().SemesterCode,
                         SpecializationName = topicRepository.GetTopics().Result.Where(x => x.Id == a.TopicId).SingleOrDefault().SpecializationName
                     }))
                }).ToList();
                return groups.AsEnumerable();
            }
            catch (CrudException ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get group list error!!!!!", ex.Message);
            }
        }

        public async Task<GroupProjectResponse> LeaveGroup(int groupId, string code)
        {
            try
            {
                var group = GroupDAO.Instance.GetGroupProjectByID(groupId);

                if (group == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, $"Not found group with id{groupId.ToString()}", "");
                }
               foreach(var stu in group.StudentInGroups)
                {
                    var studentInGroup = accountRepository.GetAccounts().Result.Where(a => a.Code == code && a.RoleId==1).SingleOrDefault();
                    if (studentInGroup == null)
                    {
                        throw new CrudException(HttpStatusCode.NotFound, $"Not found student with id{groupId.ToString()}", "");
                    }
                    var s = semesterRepository.GetSemesters().Where(a => a.StartDate <= DateTime.UtcNow && a.EndDate >= DateTime.UtcNow).SingleOrDefault();
                    if (TopicOfGroupDAO.Instance.GetTopicOfGroups().Where(a => a.GroupProjectId == groupId && a.Status == true && a.Topic.SemesterId==s.Id).SingleOrDefault()!=null)
                        throw new CrudException(HttpStatusCode.NotFound, "Once you have registered for a topic, you are not allowed to leave the group", "");
                    if(stu.StudentId == studentInGroup.Id && stu.Status==true) throw new CrudException(HttpStatusCode.NotFound, "You must change the leader position to another person in the group before leaving", "");
                    if (stu.StudentId==studentInGroup.Id)
                        GroupDAO.Instance.RemoveStudentInGroup(stu,groupId);
                }
                var groupResult = GroupDAO.Instance.GetGroupProjects().Select(x => new GroupProjectResponse()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Status = x.Status,
                    StudentInGroups = mapper.Map<List<StudentInGroupResponse>>(group.StudentInGroups),
                    TopicOfGroups = mapper.Map<List<TopicOfGroupResponse>>(group.TopicOfGroups)
                }).FirstOrDefault();
                return groupResult;
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Leave group error!!!!!", ex.Message);
            }
        }

        public async Task<GroupProjectResponse> UpdateGroupProject(int groupId, UpdateGroupProjectRequest request)
        {
            try
            {
                var group=GroupDAO.Instance.GetGroupProjectByID(groupId);

                if (group == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, $"Not found group with id{groupId.ToString()}", "");
                }

                
                List<StudentInGroup> students = new List<StudentInGroup>();
                List<StudentInGroupResponse> studentInGroups = new List<StudentInGroupResponse>();
                if (request.StudentInGroups != null)
                {
                    if (request.StudentInGroups.Count > 0)
                    {
                        if (group.StudentInGroups.Count() == 5)
                            throw new CrudException(HttpStatusCode.BadRequest, "The group has enough members ", "");
                        if (5 - group.StudentInGroups.Count() < request.StudentInGroups.Count())
                            throw new CrudException(HttpStatusCode.BadRequest, "Group membership has exceeded 4 to 5 people ", "");

                        foreach (var stu in request.StudentInGroups)
                        {
                            if (CheckStudentHasGroup((int)stu.StudentId) == true)
                                throw new CrudException(HttpStatusCode.BadRequest, "Students has already create group ", "");
                            var acc = accountRepository.GetAccountById((int)stu.StudentId).Result;
                            if (acc.RoleId != 1) throw new CrudException(HttpStatusCode.BadRequest, "Must add members whose role is student ", "");
                            var subjects = subjectRepository.GetSubjectIsPrerequisite((int)acc.SpecializationId).Where(a => a.Code != "SEP490").ToList();
                            var account = accountRepository.GetAccountsInSemester().Result.Where(a => a.StudentId == stu.StudentId && a.Subject.IsPrerequisite == true && a.Status == true && a.Subject.SpecializationId == acc.SpecializationId).ToList();
                            if (account != null && account.Count() > 0)
                            {
                                if (account.Count() == subjects.Count())
                                {
                                    StudentInGroup student = new StudentInGroup();
                                    student.Status = false;
                                    student.Role = stu.Role;
                                    student.Description = stu.Description;
                                    student.JoinDate = DateTime.UtcNow;
                                    student.StudentId = acc.Id;
                                    var rs = mapper.Map<StudentInGroupResponse>(student);
                                    rs.StudentCode = acc.Code;
                                    rs.StudentName = acc.Name;
                                    rs.StudentEmail = acc.Email;
                                    students.Add(student);
                                    studentInGroups.Add(rs);
                                }

                            }
                            else
                                throw new CrudException(HttpStatusCode.BadRequest, $"Student {stu.StudentId} are not eligible to create a group ", "");
                        }
                        group.StudentInGroups = students;
                    }
                }
                GroupDAO.Instance.Update(group, groupId);
                if (request.TopicId != null )
                {

                    return await CreatTopicOfGroup(groupId, (int)request.TopicId);
                }
                var groupResult = GroupDAO.Instance.GetGroupProjects().Select(x => new GroupProjectResponse()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Status = x.Status,
                    StudentInGroups = studentInGroups,
                }).FirstOrDefault();
                return groupResult;
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Update group error!!!!!", ex.Message);
            }
        }
        public async Task<List<GroupProjectResponse>> GetGroupProjectBySemester(string code)
        {
            try
            {
                List<GroupProjectResponse> groups = new List<GroupProjectResponse>();
                var list = GetGroupProjects().Result;
                foreach(var t in list)
                {
                    if(t.TopicOfGroups!= null && t.TopicOfGroups.Count() > 0)
                    {
                        var group = t.TopicOfGroups.Where(a => a.SemesterCode == code && a.Status != null).ToList();
                        if (group != null && group.Count() > 0)
                        {
                            var id = group.Where(a => a.Status == true).SingleOrDefault().GroupProjectId;
                            if (id != null)
                                groups.Add(GetGroupProjectById((int)id).Result);
                        }
                    }                
                }              
                if(groups.Count == 0) groups = list.Where(a => a.TopicOfGroups ==null).ToList();
                return mapper.Map<List<GroupProjectResponse>>(groups);
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get group by semester error!!!!!", ex.Message);
            }
        }
    }
}
