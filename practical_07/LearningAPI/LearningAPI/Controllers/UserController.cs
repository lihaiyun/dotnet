﻿using AutoMapper;
using LearningAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LearningAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly MyDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public UserController(MyDbContext context, IConfiguration configuration, IMapper mapper)
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterRequest request)
        {
            // Trim string values
            request.Name = request.Name.Trim();
            request.Email = request.Email.Trim().ToLower();
            request.Password = request.Password.Trim();

            // Check email
            var foundUser = _context.Users.Where(x => x.Email == request.Email).FirstOrDefault();
            if (foundUser != null)
            {
                string message = "Email already exists.";
                return BadRequest(new { message });
            }

            // Create user object
            var now = DateTime.Now;
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var user = new User()
            {
                Name = request.Name,
                Email = request.Email,
                Password = passwordHash,
                CreatedAt = now,
                UpdatedAt = now
            };

            // Add user
            _context.Users.Add(user);
            _context.SaveChanges();
            return Ok();
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        public IActionResult Login(LoginRequest request)
        {
            // Trim string values
            request.Email = request.Email.Trim().ToLower();
            request.Password = request.Password.Trim();

            // Check email and password
            string message = "Email or password is not correct.";
            var foundUser = _context.Users.Where(x => x.Email == request.Email).FirstOrDefault();
            if (foundUser == null)
            {
                return BadRequest(new { message });
            }
            bool verified = BCrypt.Net.BCrypt.Verify(request.Password, foundUser.Password);
            if (!verified)
            {
                return BadRequest(new { message });
            }

            // Return user info
            UserDTO userDTO = _mapper.Map<UserDTO>(foundUser);
            string accessToken = CreateToken(foundUser);
            LoginResponse response = new() { User = userDTO, AccessToken = accessToken };
            return Ok(response);
        }

        [HttpGet("auth"), Authorize]
        public IActionResult Auth()
        {
            var id = Convert.ToInt32(User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier)
                .Select(c => c.Value).SingleOrDefault());
            var name = User.Claims.Where(c => c.Type == ClaimTypes.Name)
                .Select(c => c.Value).SingleOrDefault();
            var email = User.Claims.Where(c => c.Type == ClaimTypes.Email)
                .Select(c => c.Value).SingleOrDefault();
            var user = new
            {
                id,
                email,
                name
            };
            return Ok(new { user });
        }

        private string CreateToken(User user)
        {
            string secret = _configuration.GetValue<string>("Authentication:Secret");
            int tokenExpiresDays = _configuration.GetValue<int>("Authentication:TokenExpiresDays");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddDays(tokenExpiresDays),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            string token = tokenHandler.WriteToken(securityToken);

            return token;
        }
    }
}