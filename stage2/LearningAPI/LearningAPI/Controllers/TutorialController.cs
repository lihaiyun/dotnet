﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LearningAPI;
using LearningAPI.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace LearningAPI.Controllers
{
    [Route("api/[controller]")]
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
        public async Task<ActionResult<IEnumerable<Tutorial>>> GetTutorials(string? search)
        {
            if (_context.Tutorials == null)
            {
                return NotFound();
            }

            IQueryable<Tutorial> result = _context.Tutorials.Include(x => x.User);
            if (search != null)
            {
                result = result.Where(x => x.Title.Contains(search) || x.Description.Contains(search));
            }
            return await result.OrderByDescending(x => x.CreatedAt).ToListAsync();
        }

        // GET: api/Tutorial/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tutorial>> GetTutorial(int id)
        {
            if (_context.Tutorials == null)
            {
                return NotFound();
            }
            var tutorial = await _context.Tutorials.FindAsync(id);

            if (tutorial == null)
            {
                return NotFound();
            }

            return tutorial;
        }

        // PUT: api/Tutorial/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTutorial(int id, Tutorial tutorial)
        {
            if (id != tutorial.Id)
            {
                return BadRequest();
            }

            var myTutorial = await _context.Tutorials.FindAsync(id);
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
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TutorialExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Tutorial
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost, Authorize]
        public async Task<ActionResult<Tutorial>> PostTutorial(Tutorial tutorial)
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            int userId = Convert.ToInt32(User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier)
                .Select(c => c.Value).SingleOrDefault());

            if (_context.Tutorials == null)
            {
                return Problem("Entity set 'MyDbContext.Tutorials'  is null.");
            }

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
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTutorial", new { id = myTutorial.Id }, myTutorial);
        }

        // DELETE: api/Tutorial/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTutorial(int id)
        {
            if (_context.Tutorials == null)
            {
                return NotFound();
            }
            var tutorial = await _context.Tutorials.FindAsync(id);
            if (tutorial == null)
            {
                return NotFound();
            }

            _context.Tutorials.Remove(tutorial);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TutorialExists(int id)
        {
            return (_context.Tutorials?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
