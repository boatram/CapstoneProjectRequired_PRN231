using BusinessObjects.DTOs.Request;
using BusinessObjects.DTOs.Response;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Repository;
using Repository.Helpers;
using static Repository.Helpers.Enum;
using ODataController = Microsoft.AspNetCore.OData.Routing.Controllers.ODataController;

namespace PRN231.CPR.API.Controllers
{
    public class GroupProjectsController : ODataController
    {
        private readonly IGroupProjectRepository groupRepository;

        public GroupProjectsController(IGroupProjectRepository repository)
        {
            groupRepository = repository;
        }
        [EnableQuery]
        public async Task<ActionResult<List<GroupProjectResponse>>> Get()
        {
            var rs = await groupRepository.GetGroupProjects();
            return Ok(rs);
        }
        [EnableQuery]
        public async Task<ActionResult<GroupProjectResponse>> Get(int key)
        {
            var rs = await groupRepository.GetGroupProjectById(key);
            return Ok(rs);
        }

        [HttpPut]
        public async Task<ActionResult<GroupProjectResponse>> Put([FromBody] UpdateGroupProjectRequest userRequest, int key)
        {
            var rs = await groupRepository.UpdateGroupProject(key, userRequest);
            if (rs == null) return NotFound();
            return Ok(rs);
        }

        public async Task<ActionResult<GroupProjectResponse>>Post([FromBody]GroupProjectRequest request)
        {
            var rs = await groupRepository.CreateGroupProject(request);
            return Ok(rs);
        }

        [HttpPut("leader-changing")]
        public async Task<ActionResult<GroupProjectResponse>> ChangeLeaderInGroup(int groupId, string code)
        {
            var rs = await groupRepository.ChangeLeaderInGroup(groupId,code);
            if (rs == null) return NotFound();
            return Ok(rs);
        }
        [HttpPut("group-leaving")]
        public async Task<ActionResult<GroupProjectResponse>> LeaveGroupProject(int id, string code)
        {
            var rs = await groupRepository.LeaveGroup(id,code);
            if (rs == null) return NotFound();
            return Ok(rs);
        }
    }
}
