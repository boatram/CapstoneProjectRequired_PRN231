using BusinessObjects;
using BusinessObjects.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Repository;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PRN231.CPR.API.Controllers
{
    public class TopicViewController : ODataController
    {
        private readonly ITopicViewRepository topicViewRepository;

        public TopicViewController(ITopicViewRepository repository)
        {
            topicViewRepository = repository;
        }

        // GET: api/<TopicViewController>
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<TopicView>>> Get()
        {
            var ss = topicViewRepository.GetTopicViews();
            if (ss == null)
                return NotFound();
            return Ok(ss);
        }

        // PUT api/<TopicViewController>/5
        public async Task<ActionResult> Put(int key)
        {
            if (key == 0) return NotFound();
            topicViewRepository.UpdateStatus(key);
            return Ok();
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
