using IDoxInstance.Context;
using IDoxInstance.Entities;
using IDoxInstance.Entities.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IDoxInstance
{
    public class JWTmiddleware : IJwtmiddleware
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public JWTmiddleware(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task<string> JSONToken(User user)
        {
            var userExists = await _context.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
            
            //var userRoles = userDepartment.Id;
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };
            //authClaims.Add(new Claim(ClaimTypes., userExists.RoleId.ToString()));
            var userIdentity = new ClaimsIdentity(authClaims, ClaimTypes.Name);
            var authSigninKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidateAudience"],
                expires: DateTime.Now.AddHours(8),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha256));
            var Token = new JwtSecurityTokenHandler().WriteToken(token);
            return Token;
        }
    }
}
