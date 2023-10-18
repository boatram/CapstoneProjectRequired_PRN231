using BusinessObjects.DTOs.Request;
using BusinessObjects.DTOs.Response;
using Microsoft.AspNetCore.Http;
using Repository.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Repository.Helpers.Enum;

namespace Repository
{
    public interface IGroupProjectRepository
    {
        Task<IEnumerable<GroupProjectResponse>> GetGroupProjects();
        Task<GroupProjectResponse> CreateGroupProject(GroupProjectRequest group);
        Task<GroupProjectResponse> GetGroupProjectById(int id);
       Task<GroupProjectResponse> UpdateGroupProject(int groupId, UpdateGroupProjectRequest request);
        Task<GroupProjectResponse>CreatTopicOfGroup(int groupId, int topicId);
        Task<GroupProjectResponse> ChangeLeaderInGroup(int groupId, string code);
        Task<GroupProjectResponse> LeaveGroup(int groupId, string code);
    }
}
