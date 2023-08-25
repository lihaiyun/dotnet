using AutoMapper;
using LearningAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LearningAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TutorialController : ControllerBase
    {
        private readonly MyDbContext _context;
        private readonly IMapper _mapper;

        public TutorialController(MyDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TutorialDTO>), StatusCodes.Status200OK)]
        public IActionResult GetAll(string? search)
        {
            IQueryable<Tutorial> result = _context.Tutorials.Include(t => t.User);
            if (search != null)
            {
                result = result.Where(x => x.Title.Contains(search)
                    || x.Description.Contains(search));
            }
            var list = result.OrderByDescending(x => x.CreatedAt).ToList();
            IEnumerable<TutorialDTO> data = list.Select(t => _mapper.Map<TutorialDTO>(t));
            return Ok(data);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TutorialDTO), StatusCodes.Status200OK)]
        public IActionResult GetTutorial(int id)
        {
            Tutorial? tutorial = _context.Tutorials.Include(t => t.User)
                .FirstOrDefault(t => t.Id == id);
            if (tutorial == null)
            {
                return NotFound();
            }
            TutorialDTO data = _mapper.Map<TutorialDTO>(tutorial);
            return Ok(data);
        }

        [HttpPost, Authorize]
        [ProducesResponseType(typeof(TutorialDTO), StatusCodes.Status200OK)]
        public IActionResult AddTutorial(AddTutorialRequest tutorial)
        {
            int userId = GetUserId();
            var now = DateTime.Now;
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

            Tutorial? newTutorial = _context.Tutorials.Include(t => t.User)
                .FirstOrDefault(t => t.Id == myTutorial.Id);
            TutorialDTO tutorialDTO = _mapper.Map<TutorialDTO>(newTutorial);
            return Ok(tutorialDTO);
        }

        [HttpPut("{id}"), Authorize]
        public IActionResult UpdateTutorial(int id, UpdateTutorialRequest tutorial)
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

            if (tutorial.Title != null)
            {
                myTutorial.Title = tutorial.Title.Trim();
            }
            if (tutorial.Description != null)
            {
                myTutorial.Description = tutorial.Description.Trim();
            }
            if (tutorial.ImageFile != null)
            {
                myTutorial.ImageFile = tutorial.ImageFile;
            }
            myTutorial.UpdatedAt = DateTime.Now;

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

        [HttpDelete("{id}"), Authorize]
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
            return Convert.ToInt32(User.Claims
                .Where(c => c.Type == ClaimTypes.NameIdentifier)
                .Select(c => c.Value).SingleOrDefault());
        }
    }
}
