using LearningAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearningAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly MyDbContext _context;

        public UserController(MyDbContext context)
        {
            _context = context;
        }

        // POST: api/User/register
        [HttpPost("register")]
        public IActionResult Register(RegisterRequest request)
        {
            var foundUser = _context.Users.Where(x => x.Email == request.Email).FirstOrDefault();
            if (foundUser != null)
            {
                return BadRequest("Email already exists.");
            }

            var now = DateTime.Now;
            var user = new User() {
                Name = request.Name,
                Email = request.Email,
                Password = request.Password,
                CreatedAt = now,
                UpdatedAt = now
            };

            _context.Users.Add(user);
            _context.SaveChanges();
            return Ok(user);
        }
    }
}
