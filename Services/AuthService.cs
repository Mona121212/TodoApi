using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;
using System.Data;

namespace TodoApi.Services
{
    public class AuthService: IAuthService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }
        // takes and email and password, returns a token string.
        public async Task<string?> Register(string email, string password)
        {
            // check the email exiting
            if (await _db.Users.AnyAsync(u => u.Email == email))
                return null;
            var user = new User
            {
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            // generate a JWT token and return it  to the frontend.
            return GenerateToken(user);
        }

        public async Task<string?> Login(string email, string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

            if(user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            return GenerateToken(user);
        }

        private string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DataSetDateTime.UtcNow.AddHours(
                    double.Parse(_config["Jwt:ExpiresInHours"]!
                )),
                signingCredentials: credentials);

        }
    }
}
