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



        [Authorize]
        [EnableQuery]
        public async Task<ActionResult<List<SemesterResponse>>> GetSemesters()
        {
            var ss = await semesterRepository.GetSemesters();
            if (ss == null)
                return NotFound();
            return Ok(ss);
        }

        // GET api/<SemesterController>/5
        public async Task<ActionResult<Semester>> GetSemesterById(int key)
        {
            if (key == 0) return NotFound();
            var s = semesterRepository.GetSemesterByID(key);
            return Ok(s);
        }
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Post([FromBody] SemesterRequest value)
        {
            if (ModelState.IsValid)
            {
                var rs = await semesterRepository.CreateSemester(value);
                return Ok(rs);
            }
            else return new BadRequestObjectResult(ModelState);
            return Ok();
        }
        [AllowAnonymous]
        [HttpPost("student-in-semester")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<StudentInSemesterResponse>> CreateStudentInSemester(IFormFile file)
        {
            var rs = await semesterRepository.CreateStudentInSemester(file);
            return Ok(rs);
        }
    }
}
