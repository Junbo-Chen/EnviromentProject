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
            // Haal UserId op uit JWT-token
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated.");
            }

            var environments = _environmentRepository.GetEnvironmentsByUserId(userId);
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
            // Haal UserId op uit JWT-token
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated.");
            }

            // Check if user already has 5 environments
            var userEnvironments = _environmentRepository.GetEnvironmentsByUserId(userId);
            if (userEnvironments.Count() >= 5)
            {
                return BadRequest("You have reached the maximum limit of 5 environments.");
            }

            environment.UserId = userId; // Koppel de gebruiker aan het Environment-object
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

            // Check if user owns this environment
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated.");
            }

            if (existingEnvironment.UserId != userId)
            {
                return Forbid("You can only modify your own environments.");
            }

            existingEnvironment.Name = updatedEnvironment.Name;
            existingEnvironment.MaxHeight = updatedEnvironment.MaxHeight;
            existingEnvironment.MaxLength = updatedEnvironment.MaxLength;
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

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated.");
            }

            if (environment.UserId != userId)
            {
                return Forbid("You can only delete your own environments.");
            }

            _environmentRepository.DeleteEnvironment(id);
            return NoContent();
        }
    }
}
