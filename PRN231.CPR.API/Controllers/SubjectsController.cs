using BusinessObjects;
using BusinessObjects.DTOs.Request;
using BusinessObjects.DTOs.Response;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository;
using static Repository.Helpers.Enum;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PRN231.CPR.API.Controllers
{

    public class SubjectsController : ODataController
    {
        private readonly ISubjectRepository subjectRepository;

        public SubjectsController(ISubjectRepository repository)
        {
            subjectRepository = repository;
        }


        // GET: api/<SubjectController>/5
        [Route("api/Subjects/Prerequisite/{id}")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubjectResponse>>> GetSubjectIsPrerequisite(int id)
        {
            var ss = subjectRepository.GetSubjectIsPrerequisite((int)id);
            if (ss == null)
                return NotFound();
            return Ok(ss);
        }


        // GET api/<SubjectController>/5
        [Route("api/Subjects/Specialization/{id}")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubjectResponse>>> GetSubjects(int id)
        {
            var ss = subjectRepository.GetSubjectBySpecializationId(id);
            if (ss == null)
                return NotFound();
            return Ok(ss);
        }
        [HttpPost()]
        public async Task<ActionResult<SubjectResponse>>Post(IFormFile file)
        {
            var rs = await subjectRepository.Create(file);
            return Ok(rs);
        }

        // PUT api/<SubjectController>/5
        [HttpPut]
        public async Task<ActionResult> Put(int key)
        {
            if (key == 0) return NotFound();
            subjectRepository.UpdateStatus(key);
            return Ok();
        }

    }
}
