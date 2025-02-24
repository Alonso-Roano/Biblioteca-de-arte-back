namespace Babel.Services.Service
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using System.Text;
    using Babel.Services.IService;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.EntityFrameworkCore;

    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<IdentityUser> _userManager;
        public TokenService(IConfiguration config, UserManager<IdentityUser> userManager)
        {
            _config = config;
            _userManager = userManager;
        }

        public string GenerateJwtToken(IdentityUser user, string role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_config["JwtSettings:ExpirationMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task SaveRefreshToken(IdentityUser user, string refreshToken)
        {
            var existingToken = await _userManager.GetAuthenticationTokenAsync(user, "MyApp", "RefreshToken");

            if (existingToken != null)
            {
                await _userManager.RemoveAuthenticationTokenAsync(user, "MyApp", "RefreshToken");
            }

            await _userManager.SetAuthenticationTokenAsync(user, "MyApp", "RefreshToken", refreshToken);
        }

        public async Task<IdentityUser?> GetUserByRefreshToken(string refreshToken)
        {
            var users = await _userManager.Users.ToListAsync();
            foreach (var user in users)
            {
                var storedToken = await _userManager.GetAuthenticationTokenAsync(user, "MyApp", "RefreshToken");
                if (storedToken == refreshToken)
                {
                    return user;
                }
            }
            return null;
        }
    }
}
