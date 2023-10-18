using BusinessObjects;
using BusinessObjects.BusinessObjects;
using BusinessObjects.DTOs.Request;
using BusinessObjects.DTOs.Response;
using Microsoft.AspNetCore.Mvc;
using Repository;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PRN231.CPR.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpecializationController : ControllerBase
    {

        private readonly ISpecializationRepository specializationRepository;

        public SpecializationController(ISpecializationRepository specializationRepository)
        {
            this.specializationRepository = specializationRepository;
        }

        // GET: api/<SpecializationController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SpecializationResponse>>> Get()
        {
            var ss = specializationRepository.GetSpecializations();
            if (ss == null)
                return NotFound();
            return Ok(ss);
        }

        // GET api/<SpecializationController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Specialization>> GetById(int id)
        {
            if (id == 0) return NotFound();
            var s = specializationRepository.GetSpecializationByID(id);
            return Ok(s);
        }

        // POST api/<SpecializationController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] SpecializationRequest value)
        {
            if (value != null)
            {
                specializationRepository.Create(value);
                return Ok();
            }
            return BadRequest();
        }

        // PUT api/<SpecializationController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id)
        {
            if (id == 0) return NotFound();
            specializationRepository.UpdateStatus(id);
            return Ok();
        }

    }
}
