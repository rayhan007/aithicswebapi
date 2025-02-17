using aithics.data.Models;
using aithics.service.Interfaces;
using Microsoft.EntityFrameworkCore;
using aithics.data;
using aithics.data.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace aithics.service.Services
{
    public class AuthService : IAuthService
    {
        private readonly AithicsDbContext _context;

        public AuthService(AithicsDbContext context)
        {
            _context = context;
        }

        public async Task<string> RegisterUserAsync(User user, string password)
        {
            // Hash password using BCrypt before storing
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return "User registered successfully.";
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            try
            {
                var user = await _context.Users.SingleAsync(u => u.Email == email);      


                if (user == null || !VerifyPassword(password, user.PasswordHash))
                    return "Invalid login attempt";

                return GenerateJwtToken(user);
            }

            catch(Exception ex) 
            
            {
                return "Invalid login attempt";
            }
           
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }

        private string GenerateJwtToken(User user)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: "https://portal.aithics.net",
                audience: "https://api.aithics.net",
                claims: authClaims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(env.Jwt_Key)),
                    SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
