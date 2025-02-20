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
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: env.Jwt_Issuer,
                audience: env.Jwt_Audience,
                claims: authClaims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(env.Jwt_Key)),
                    SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> GenerateRefreshTokenAsync(long userId)
        {
            var refreshToken = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString());

            var user = await _context.Users.FindAsync(userId);
            if (user == null) 
                return string.Empty;

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // Refresh token valid for 7 days
            await _context.SaveChangesAsync();

            return refreshToken;
        }

        public async Task<string> RefreshAccessTokenAsync(string refreshToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
            if (user == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
                return "Invalid refresh token";

            return GenerateJwtToken(user);
        }
        public async Task<bool> LogoutAsync(long userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _context.SaveChangesAsync();

            return true;
        }

    }
}
