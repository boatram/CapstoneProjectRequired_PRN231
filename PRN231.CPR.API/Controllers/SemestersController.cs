using BusinessObjects;
using BusinessObjects.BusinessObjects;
using BusinessObjects.DTOs.Request;
using BusinessObjects.DTOs.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository;
using Microsoft.AspNet.OData;
using ODataController = Microsoft.AspNetCore.OData.Routing.Controllers.ODataController;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PRN231.CPR.API.Controllers
{
    public class SemestersController : ODataController
    {
        private readonly ISemesterRepository semesterRepository;

        public SemestersController(ISemesterRepository repository)
        {
            semesterRepository = repository;
        }

        // GET: api/<SemesterController>
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<SemesterResponse>>> Get()
        {
            var ss = semesterRepository.GetSemesters();
            if (ss == null)
                return NotFound();
            return Ok(ss);
        }

        // GET api/<SemesterController>/5
        public async Task<ActionResult<Semester>> Get(int key)
        {
            if (key == 0) return NotFound();
            var s = semesterRepository.GetSemesterByID(key);
            return Ok(s);
        }

        public async Task<ActionResult> Post([FromBody] SemesterRequest value)
        {
            if (value != null)
            {
                semesterRepository.Create(value);
                return Ok();
            }
            return BadRequest();
        }

        // PUT api/<SemesterController>/5
        [HttpPut]
        public async Task<ActionResult> Put(int key)
        {
            if (key == 0) return NotFound();
            semesterRepository.Update(key);
            return Ok();
        }
        [AllowAnonymous]
        [HttpPost("student-in-semester")]
        public async Task<ActionResult<StudentInSemesterResponse>> CreateStudentInSemester(IFormFile file)
        {
            var rs = await semesterRepository.CreateStudentInSemester(file);
            return Ok(rs);
        }
    }
}
