﻿using LearningAPI.Models;
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

        public TutorialController(MyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll(string? search)
        {
            IQueryable<Tutorial> result = _context.Tutorials.Include(t => t.User);
            if (search != null)
            {
                result = result.Where(x => x.Title.Contains(search)
                    || x.Description.Contains(search));
            }
            var list = result.OrderByDescending(x => x.CreatedAt).ToList();
            var data = list.Select(t => new
            {
                t.Id,
                t.Title,
                t.Description,
                t.CreatedAt,
                t.UpdatedAt,
                t.UserId,
                User = new
                {
                    t.User?.Name
                }
            });
            return Ok(data);
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

        [HttpPost, Authorize]
        public IActionResult AddTutorial(Tutorial tutorial)
        {
            int userId = GetUserId();
            var now = DateTime.Now;
            var myTutorial = new Tutorial()
            {
                Title = tutorial.Title.Trim(),
                Description = tutorial.Description.Trim(),
                CreatedAt = now,
                UpdatedAt = now,
                UserId = userId
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