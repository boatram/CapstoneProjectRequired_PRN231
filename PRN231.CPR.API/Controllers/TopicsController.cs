using BusinessObjects;
using BusinessObjects.DTOs;
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
    public class TopicsController : ODataController
    {
        private readonly ITopicRepository topicRepository;

        public TopicsController(ITopicRepository repository)
        {
            topicRepository = repository;
        }
       [Authorize]
        [EnableQuery]
        public async Task<ActionResult<List<TopicResponse>>> Get()
        {
            var rs = await topicRepository.GetTopics();
            return Ok(rs);
        }
       // [Authorize(Roles ="Lecturer")]
        [HttpPost("topic-of-lecturer")]
        public async Task<ActionResult<TopicResponse>> Post(IFormFile file, int id)
        {
            var rs = await topicRepository.Create(file,id);
            return Ok(rs);
        }

        [HttpPut("accept-topic")]
        public async Task<ActionResult> Put(int topicId, int groupId)
        {
           var rs= topicRepository.Update(topicId,groupId);
            return Ok(rs);
        }
        /*
        // GET api/<TopicViewController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // DELETE api/<TopicViewController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        */
    }
}
