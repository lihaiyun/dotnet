using LearningAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace LearningAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TutorialController : ControllerBase
    {
        private static readonly List<Tutorial> list = new();

        private readonly MyDbContext _context;

        public TutorialController(MyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(list);
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
