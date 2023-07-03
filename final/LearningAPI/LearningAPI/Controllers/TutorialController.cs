using LearningAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LearningAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TutorialController : ControllerBase
    {
        private readonly MyDbContext _context;

        public TutorialController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/Tutorial
        [HttpGet]
        public IActionResult GetTutorials(string? search)
        {
            IQueryable<Tutorial> result = _context.Tutorials.Include(t => t.User);
            if (search != null)
            {
                result = result.Where(x => x.Title.Contains(search) || x.Description.Contains(search));
            }
            var list = result.OrderByDescending(x => x.CreatedAt).ToList();

            var response = list
                .Select(t => new
                {
                    t.Id,
                    t.Title,
                    t.Description,
                    t.ImageFile,
                    t.CreatedAt,
                    t.UpdatedAt,
                    t.UserId,
                    User = new
                    {
                        Name = t.User != null ? t.User.Name : string.Empty
                    }
                });
            return Ok(response);
        }

        // GET: api/Tutorial/5
        [HttpGet("{id}")]
        public IActionResult GetTutorial(int id)
        {
            Tutorial? t = _context.Tutorials.Find(id);
            if (t == null)
            {
                return NotFound();
            }

            User? user = _context.Users.Find(t.UserId);
            t.User = user;

            var response = new
            {
                t.Id,
                t.Title,
                t.Description,
                t.ImageFile,
                t.CreatedAt,
                t.UpdatedAt,
                t.UserId,
                User = new
                {
                    Name = t.User != null ? t.User.Name : string.Empty
                }
            };
            return Ok(response);
        }

        // POST: api/Tutorial
        [HttpPost, Authorize]
        public IActionResult PostTutorial(Tutorial tutorial)
        {
            var now = DateTime.Now;
            int userId = GetUserId();
            var myTutorial = new Tutorial()
            {
                Title = tutorial.Title.Trim(),
                Description = tutorial.Description.Trim(),
                ImageFile = tutorial.ImageFile,
                CreatedAt = now,
                UpdatedAt = now,
                UserId = userId
            };

            _context.Tutorials.Add(myTutorial);
            _context.SaveChanges();
            return Ok(myTutorial);
        }

        // PUT: api/Tutorial/5
        [HttpPut("{id}"), Authorize]
        public IActionResult PutTutorial(int id, Tutorial tutorial)
        {
            var myTutorial = _context.Tutorials.Find(id);
            if (myTutorial == null)
            {
                return NotFound();
            }

            int userId = GetUserId();
            if (myTutorial.UserId != userId)
            {
                return Forbid();
            }

            myTutorial.Title = tutorial.Title.Trim();
            myTutorial.Description = tutorial.Description.Trim();
            myTutorial.ImageFile = tutorial.ImageFile;
            myTutorial.UpdatedAt = DateTime.Now;

            _context.Tutorials.Update(myTutorial);
            try
            {
                _context.SaveChanges();
            }
            catch (Exception)
            {
                return BadRequest("Technical error");
            }
            return Ok();
        }

        // DELETE: api/Tutorial/5
        [HttpDelete("{id}")]
        public IActionResult DeleteTutorial(int id)
        {
            var myTutorial = _context.Tutorials.Find(id);
            if (myTutorial == null)
            {
                return NotFound();
            }

            int userId = GetUserId();
            if (myTutorial.UserId != userId)
            {
                return Forbid();
            }

            _context.Tutorials.Remove(myTutorial);
            _context.SaveChanges();
            return Ok();
        }

        private int GetUserId()
        {
            return Convert.ToInt32(User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier)
                .Select(c => c.Value).SingleOrDefault());
        }
    }
}
