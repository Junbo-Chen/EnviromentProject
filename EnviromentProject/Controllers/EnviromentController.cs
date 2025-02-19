using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using EnviromentProject.Model;
using EnviromentProject.Data;
using Environment = EnviromentProject.Model.Environment;

namespace EnviromentProject.Controllers
{
    [Route("api/environments")]
    [ApiController]
    public class EnvironmentController : ControllerBase
    {
        private readonly EnvironmentRepository _environmentRepository;

        public EnvironmentController(EnvironmentRepository environmentRepository)
        {
            _environmentRepository = environmentRepository;
        }

        // 🔹 GET: api/environments
        [HttpGet]
        public ActionResult<IEnumerable<Environment>> Get()
        {
            var environments = _environmentRepository.GetAllEnvironments();
            return Ok(environments);
        }

        // 🔹 GET: api/environments/{id}
        [HttpGet("{id}")]
        public ActionResult<Environment> Get(Guid id)
        {
            var environment = _environmentRepository.GetEnvironmentById(id);
            if (environment == null)
            {
                return NotFound();
            }
            return Ok(environment);
        }

        // 🔹 POST: api/environments
        [HttpPost]
        public ActionResult Post([FromBody] Environment environment)
        {
            if (environment == null)
            {
                return BadRequest("Invalid environment data.");
            }

            _environmentRepository.InsertEnvironment(environment);
            return CreatedAtAction(nameof(Get), new { id = environment.Id }, environment);
        }

        // 🔹 PUT: api/environments/{id}
        [HttpPut("{id}")]
        public ActionResult Put(Guid id, [FromBody] Environment updatedEnvironment)
        {
            var existingEnvironment = _environmentRepository.GetEnvironmentById(id);
            if (existingEnvironment == null)
            {
                return NotFound();
            }

            existingEnvironment.Name = updatedEnvironment.Name;
            existingEnvironment.MaxHeight = updatedEnvironment.MaxHeight;
            existingEnvironment.MaxLength = updatedEnvironment.MaxLength;
            existingEnvironment.UserId = updatedEnvironment.Id;

            _environmentRepository.UpdateEnvironment(existingEnvironment);
            return NoContent();
        }

        // 🔹 DELETE: api/environments/{id}
        [HttpDelete("{id}")]
        public ActionResult Delete(Guid id)
        {
            var environment = _environmentRepository.GetEnvironmentById(id);
            if (environment == null)
            {
                return NotFound();
            }

            _environmentRepository.DeleteEnvironment(id);
            return NoContent();
        }
    }
}
