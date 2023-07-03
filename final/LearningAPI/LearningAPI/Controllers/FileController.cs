using LearningAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearningAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private IWebHostEnvironment _environment;

        public FileController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        // POST: File/upload
        [HttpPost("upload"), Authorize]
        public IActionResult Upload(IFormFile file)
        {
            if (file.Length > 1024 * 1024)
            {
                return BadRequest("File size cannot exceed 1MB.");
            }

            var id = Nanoid.Nanoid.Generate(size:10);
            var filename = id + Path.GetExtension(file.FileName);
            var imagePath = Path.Combine(_environment.ContentRootPath, "wwwroot//uploads", filename);
            using var fileStream = new FileStream(imagePath, FileMode.Create);
            file.CopyTo(fileStream);

            return Ok(new { filename });
        }
    }
}
