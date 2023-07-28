using LearningAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LearningAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TutorialController : ControllerBase
    {
        private readonly MyDbContext _context;

        public TutorialController(MyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll(string? search)
        {
            IQueryable<Tutorial> result = _context.Tutorials;
            if (search != null)
            {
                result = result.Where(x => x.Title.Contains(search)
                    || x.Description.Contains(search));
            }
            var list = result.OrderByDescending(x => x.CreatedAt).ToList();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public IActionResult GetTutorial(int id)
        {
            Tutorial? tutorial = _context.Tutorials.Find(id);
            if (tutorial == null)
            {
                return NotFound();
            }
            return Ok(tutorial);
        }

        [HttpPost]
        public IActionResult AddTutorial(Tutorial tutorial)
        {
            var now = DateTime.Now;
            var myTutorial = new Tutorial()
            {
                Title = tutorial.Title.Trim(),
                Description = tutorial.Description.Trim(),
                CreatedAt = now,
                UpdatedAt = now
            };

            _context.Tutorials.Add(myTutorial);
            _context.SaveChanges();
            return Ok(myTutorial);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateTutorial(int id, Tutorial tutorial)
        {
            var myTutorial = _context.Tutorials.Find(id);
            if (myTutorial == null)
            {
                return NotFound();
            }

            myTutorial.Title = tutorial.Title.Trim();
            myTutorial.Description = tutorial.Description.Trim();
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

        [HttpDelete("{id}")]
        public IActionResult DeleteTutorial(int id)
        {
            var myTutorial = _context.Tutorials.Find(id);
            if (myTutorial == null)
            {
                return NotFound();
            }

            _context.Tutorials.Remove(myTutorial);
            _context.SaveChanges();
            return Ok();
        }

        private int GetUserId()
        {
            return Convert.ToInt32(User.Claims
                .Where(c => c.Type == ClaimTypes.NameIdentifier)
                .Select(c => c.Value).SingleOrDefault());
        }
    }
}
