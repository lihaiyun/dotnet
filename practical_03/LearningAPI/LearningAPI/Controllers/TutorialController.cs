using LearningAPI.Models;
using Microsoft.AspNetCore.Mvc;

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
    }
}
