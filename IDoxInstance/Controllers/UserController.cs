using IDoxInstance.Context;
using IDoxInstance.Entities;
using IDoxInstance.Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;

namespace IDoxInstance.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IJwtmiddleware _jwt;

        public UserController(IJwtmiddleware jwt, AppDbContext context)
        {
            _jwt = jwt;
            _context = context;
        }

        [HttpPost("RegisterUser")]
        public async Task<IActionResult> RegisterUser(UserDTO dTO)
        {
            if (!ModelState.IsValid)
                return BadRequest();// <--
            var user = new User
            {
                FullName = dTO.FullName,
                Email = dTO.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dTO.Password)
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return Ok();// <-- message
        }
        [HttpGet("GetUserbyId")]
        public async Task<IActionResult> GetUserById(int Id)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var user = await _context.Users.FindAsync(Id);
            return Ok(user);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x=>x.Email == dto.Email);
            if (user == null)
                return BadRequest("User does not exists!");
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            {
                return BadRequest("Invalid credentials");
            }
            var jwt = await _jwt.JSONToken(user);
            return Ok(jwt);
        }

        [HttpGet("CurrentUser")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> CurrentUser()
        {
            var claim = User.Identity as ClaimsIdentity;
            var currentUser = await _context.Users.FirstOrDefaultAsync(x=>x.Email == claim.Name);
            return Ok(currentUser);
        }
        
    }
}
