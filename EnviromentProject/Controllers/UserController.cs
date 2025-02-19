using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using EnviromentProject.Model;
using EnviromentProject.Data;

namespace EnviromentProject.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserRepository _userRepository;

        public UserController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // 🔹 GET: api/users
        [HttpGet]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            var users = _userRepository.GetAllUsers();
            return Ok(users);
        }

        // 🔹 GET: api/users/{username}
        [HttpGet("{username}")]
        public ActionResult<User> GetUser(string username)
        {
            var user = _userRepository.GetUserByUsername(username);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        // 🔹 POST: api/users
        [HttpPost]
        public ActionResult CreateUser([FromBody] User newUser)
        {
            var existingUser = _userRepository.GetUserByUsername(newUser.Username);
            if (existingUser != null)
            {
                return Conflict("Gebruiker bestaat al.");
            }

            _userRepository.InsertUser(newUser);
            return CreatedAtAction(nameof(GetUser), new { username = newUser.Username }, newUser);
        }

        // 🔹 POST: api/users/login
        [HttpPost("login")]
        public ActionResult<string> Login([FromBody] User loginUser)
        {
            var user = _userRepository.GetUserByUsername(loginUser.Username);
            if (user == null || user.Password != loginUser.Password)
            {
                return Unauthorized("Ongeldige gebruikersnaam of wachtwoord.");
            }
            return Ok("Inloggen succesvol.");
        }

        // 🔹 DELETE: api/users/{username}
        [HttpDelete("{username}")]
        public IActionResult DeleteUser(string username)
        {
            var user = _userRepository.GetUserByUsername(username);
            if (user == null)
            {
                return NotFound();
            }

            _userRepository.DeleteUser(username);
            return NoContent();
        }
    }
}
