using LearningAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace LearningAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TutorialController : ControllerBase
    {
        private static readonly List<Tutorial> list = [];

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(list);
        }

        [HttpPost]
        public IActionResult AddTutorial(Tutorial tutorial)
        {
            list.Add(tutorial);
            return Ok(tutorial);
        }
    }
}
