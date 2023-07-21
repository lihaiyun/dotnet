using LearningAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace LearningAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly MyDbContext _context;

        public UserController(MyDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterRequest request)
        {
            // Trim string values
            request.Name = request.Name.Trim();
            request.Email = request.Email.Trim().ToLower();
            request.Password = request.Password.Trim();

            // Check email
            var foundUser = _context.Users.Where(x => x.Email == request.Email).FirstOrDefault();
            if (foundUser != null)
            {
                string message = "Email already exists.";
                return BadRequest(new { message });
            }

            // Create user object
            var now = DateTime.Now;
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var user = new User()
            {
                Name = request.Name,
                Email = request.Email,
                Password = passwordHash,
                CreatedAt = now,
                UpdatedAt = now
            };

            // Add user
            _context.Users.Add(user);
            _context.SaveChanges();
            return Ok();
        }

        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            // Trim string values
            request.Email = request.Email.Trim().ToLower();
            request.Password = request.Password.Trim();

            // Check email and password
            string message = "Email or password is not correct.";
            var foundUser = _context.Users.Where(x => x.Email == request.Email).FirstOrDefault();
            if (foundUser == null)
            {
                return BadRequest(new { message });
            }
            bool verified = BCrypt.Net.BCrypt.Verify(request.Password, foundUser.Password);
            if (!verified)
            {
                return BadRequest(new { message });
            }

            // Return user info
            var user = new
            {
                foundUser.Id,
                foundUser.Email,
                foundUser.Name
            };
            return Ok(new { user });
        }
    }
}