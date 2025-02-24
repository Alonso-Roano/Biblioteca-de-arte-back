using Microsoft.AspNetCore.Identity;

namespace Babel.Services.IService
{
    public interface ITokenService
    {
        public string GenerateJwtToken(IdentityUser user, string role);
        public string GenerateRefreshToken();
        public Task SaveRefreshToken(IdentityUser user, string refreshToken);
        public Task<IdentityUser?> GetUserByRefreshToken(string refreshToken);
    }
}
