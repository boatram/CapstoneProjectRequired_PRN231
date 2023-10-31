using BusinessObjects;
using BusinessObjects.DTOs.Request;
using BusinessObjects.DTOs.Response;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository;
using static Repository.Helpers.Enum;
using Microsoft.AspNetCore.OData.Query;

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
        
        [Authorize]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<SubjectResponse>>> GetSubjects()
        {
            var ss = await subjectRepository.GetSubjects();
            if (ss == null)
                return NotFound();
            return Ok(ss);
        }
        [HttpPost()]
        [Authorize(Roles = "Admin")]
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
