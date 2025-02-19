using Microsoft.AspNetCore.Mvc;
using EnviromentProject.Model;
using System.Collections.Generic;
using Object = EnviromentProject.Model.Object;

namespace EnviromentProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ObjectController : ControllerBase
    {
        private static List<Object> _objects = new List<Object>();

        // GET: api/Object
        [HttpGet]
        public ActionResult<IEnumerable<Object>> GetObjects()
        {
            return _objects;
        }

        // GET: api/Object/5
        [HttpGet("{id}")]
        public ActionResult<Object> GetObject(Guid id)
        {
            var obj = _objects.Find(o => o.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            return obj;
        }

        // POST: api/Object
        [HttpPost]
        public ActionResult<Object> PostObject([FromBody] Object obj)
        {
            _objects.Add(obj);
            return CreatedAtAction(nameof(GetObject), new { id = obj.Id }, obj);
        }

        // PUT: api/Object/5
        [HttpPut("{id}")]
        public IActionResult PutObject(Guid id, [FromBody] Object obj)
        {
            var existingObj = _objects.Find(o => o.Id == id);
            if (existingObj == null)
            {
                return NotFound();
            }
            existingObj.PrefabId = obj.PrefabId;
            existingObj.PositionX = obj.PositionX;
            existingObj.PositionY = obj.PositionY;
            existingObj.ScaleX = obj.ScaleX;
            existingObj.ScaleY = obj.ScaleY;
            existingObj.RotationZ = obj.RotationZ;
            existingObj.SortingLayer = obj.SortingLayer;
            return NoContent();
        }

        // DELETE: api/Object/5
        [HttpDelete("{id}")]
        public IActionResult DeleteObject(Guid id)
        {
            var obj = _objects.Find(o => o.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _objects.Remove(obj);
            return NoContent();
        }
    }
}
