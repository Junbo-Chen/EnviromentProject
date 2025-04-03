using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using EnviromentProject.Model;
using EnviromentProject.Data;
using Object = EnviromentProject.Model.Object;
using EnviromentProject.Interface;

namespace EnviromentProject.Controllers
{
    [Route("api/objects")]
    [ApiController]
    public class ObjectController : ControllerBase
    {
        private readonly IObjectRepository _objectRepository;

        public ObjectController(IObjectRepository objectRepository)
        {
            _objectRepository = objectRepository;
        }

        // 🔹 GET: api/objects/{environmentId}
        [HttpGet("environment/{environmentId}")]
        public ActionResult<IEnumerable<Object>> GetObjectsByEnvironmentId(Guid environmentId)
        {
            var objects = _objectRepository.GetObjectsByEnvironmentId(environmentId);
            if (objects == null)
            {
                return NotFound();
            }
            return Ok(objects);
        }

        // 🔹 GET: api/objects/{id}
        [HttpGet("{id}")]
        public ActionResult<Object> GetObject(Guid id)
        {
            var obj = _objectRepository.GetObjectById(id);
            if (obj == null)
            {
                return NotFound();
            }
            return Ok(obj);
        }

        // 🔹 POST: api/objects
        [HttpPost]
        public ActionResult CreateObject2D([FromBody] Object obj)
        {
            if (obj == null)
            {
                return BadRequest("Object2D is null or missing required fields.");
            }

            try
            {
                // Authenticate user
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated.");
                }

                // Verify that the environment belongs to the user
                var environment = _environmentRepository.GetEnvironmentById(obj.EnvironmentId);
                if (environment == null)
                {
                    return BadRequest("The specified environment does not exist.");
                }

                if (environment.UserId != userId)
                {
                    return Forbid("You can only add objects to your own environments.");
                }

                // Set the creator ID
                obj.CreatedBy = userId;

                _objectRepository.InsertObject(obj);
                return CreatedAtAction(nameof(GetObject), new { id = obj.Id }, obj);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error while creating object: {ex.Message}");
            }
        }

        // 🔹 PUT: api/objects/{id}
        [HttpPut("{id}")]
        public ActionResult Put(Guid id, [FromBody] Object updatedObject)
        {
            var existingObject = _objectRepository.GetObjectById(id);
            if (existingObject == null)
            {
                return NotFound();
            }

            // Authenticate user
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated.");
            }

            // Verify that the user is the creator of the object
            if (existingObject.CreatedBy != userId)
            {
                return Forbid("You can only modify objects you created.");
            }

            existingObject.PrefabId = updatedObject.PrefabId;
            existingObject.PositionX = updatedObject.PositionX;
            existingObject.PositionY = updatedObject.PositionY;
            existingObject.ScaleX = updatedObject.ScaleX;
            existingObject.ScaleY = updatedObject.ScaleY;
            existingObject.RotationZ = updatedObject.RotationZ;
            existingObject.SortingLayer = updatedObject.SortingLayer;
            _objectRepository.UpdateObject(existingObject);
            return NoContent();
        }

        // 🔹 DELETE: api/objects/{id}
        [HttpDelete("{id}")]
        public ActionResult Delete(Guid id)
        {
            var obj = _objectRepository.GetObjectById(id);
            if (obj == null)
            {
                return NotFound();
            }

            // Authenticate user
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated.");
            }

            // Verify that the user is the creator of the object
            if (obj.CreatedBy != userId)
            {
                return Forbid("You can only delete objects you created.");
            }

            _objectRepository.DeleteObject(id);
            return NoContent();
        }
    }
}
