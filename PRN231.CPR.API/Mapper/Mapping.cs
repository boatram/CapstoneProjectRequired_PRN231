
using AutoMapper;
using BusinessObjects;
using BusinessObjects.BusinessObjects;
using BusinessObjects.DTOs.Request;
using BusinessObjects.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace PRN231.CPR.API.Mapper
{
    public class Mapping : Profile
    {
        public Mapping() {

            CreateMap<Account, AccountResponse>().ReverseMap();
            CreateMap<UpdateAccountRequest, Account>().ReverseMap();
            CreateMap<AccountResponse, UpdateAccountRequest>().ReverseMap();
            CreateMap<AccountRequest, Account>().ReverseMap();
            CreateMap<AccountRequest, AccountResponse>().ReverseMap();
            CreateMap<GoogleAccountResponse, AccountResponse>().ReverseMap();

            CreateMap<TopicRequest, Topic>().ReverseMap();
            CreateMap<TopicResponse, Topic>().ReverseMap();

            CreateMap<SubjectRequest, Subject>().ReverseMap();
            CreateMap<SubjectResponse, Subject>().ReverseMap();

            CreateMap<Specialization, SpecializationResponse>().ReverseMap();

            CreateMap<StudentInSemesterRequest, StudentInSemester>().ReverseMap();
            CreateMap<StudentInSemesterResponse, StudentInSemester>().ReverseMap();

            CreateMap<StudentInGroupResponse, StudentInGroup>().ReverseMap();
            CreateMap<StudentInGroupRequest, StudentInGroup>().ReverseMap();

            CreateMap<GroupProjectRequest, GroupProject>().ReverseMap();
            CreateMap<GroupProjectResponse, GroupProject>().ReverseMap();

            CreateMap<TopicOfGroup, TopicOfGroupResponse>().ReverseMap();
        }

    }
}
