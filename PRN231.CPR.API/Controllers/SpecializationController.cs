using BusinessObjects;
using BusinessObjects.BusinessObjects;
using BusinessObjects.DTOs.Request;
using BusinessObjects.DTOs.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Repository;
using System.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PRN231.CPR.API.Controllers
{
    public class SpecializationController : ODataController
    {

        private readonly ISpecializationRepository specializationRepository;

        public SpecializationController(ISpecializationRepository specializationRepository)
        {
            this.specializationRepository = specializationRepository;
        }

        // GET: api/<SpecializationController>
        [Authorize]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<SpecializationResponse>>> GetSpecialization()
        {
            var ss = specializationRepository.GetSpecializations();
            if (ss == null)
                return NotFound();
            return Ok(ss);
        }

        public async Task<ActionResult<Specialization>> GetSpecializationById(int key)
        {
            if (key == 0) return NotFound();
            var s = specializationRepository.GetSpecializationByID(key);
            return Ok(s);
        }

        // POST api/<SpecializationController>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Post([FromBody] SpecializationRequest value)
        {
            if (ModelState.IsValid)
            {
                var rs = await specializationRepository.CreateSpecialization(value);
                return Ok(rs);
            }
            else return new BadRequestObjectResult(ModelState);
            return Ok();
        }
        public async Task<ActionResult> Put(int key)
        {
            if (key == 0) return NotFound();
            specializationRepository.UpdateStatus(key);
            return Ok();
        }

    }
}
