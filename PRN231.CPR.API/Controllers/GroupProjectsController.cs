using BusinessObjects.DTOs.Request;
using BusinessObjects.DTOs.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Repository;
using Repository.Helpers;
using static Repository.Helpers.Enum;
using ODataController = Microsoft.AspNetCore.OData.Routing.Controllers.ODataController;

namespace PRN231.CPR.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupProjectsController : ControllerBase
    {
        private readonly IGroupProjectRepository groupRepository;

        public GroupProjectsController(IGroupProjectRepository repository)
        {
            groupRepository = repository;
        }
        [HttpGet]
        public async Task<ActionResult<List<GroupProjectResponse>>> GetGroupProjects()
        {
            var rs = await groupRepository.GetGroupProjects();
            return Ok(rs);
        }
        [HttpGet("group-project-lecturer")]
        public async Task<ActionResult<List<GroupProjectResponse>>> GetGroupProjectsByLecturer(string email)
        {
            var rs = await groupRepository.GetGroupProjectsByLecturer(email);
            return Ok(rs);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<GroupProjectResponse>> GetGroupProjectById(int id)
        {
            var rs = await groupRepository.GetGroupProjectById(id);
            return Ok(rs);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GroupProjectResponse>> Put([FromBody] UpdateGroupProjectRequest userRequest, int id)
        {
            var rs = await groupRepository.UpdateGroupProject(id, userRequest);
            if (rs == null) return NotFound();
            return Ok(rs);
        }
        [HttpPost]
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
