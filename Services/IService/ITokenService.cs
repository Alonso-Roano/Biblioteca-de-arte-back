using Babel.Models.DTOs.Auth;
using Babel.Services.Service;
using Microsoft.AspNetCore.Identity;
using static Babel.Services.Service.TokenService;

namespace Babel.Services.IService
{
    public interface ITokenService
    {
        Task<AuthResponseDTO> RegisterAsync(RegisterRequestDto request);
        Task<AuthResponseDTO> LoginAsync(LoginRequestDto request);
        public Task<string> GenerateJwtToken(IdentityUser user, string role);
        public string  GenerateRefreshToken();
        Task<DashboardMetricsDTO> GetDashboardMetricsAsync();
        public Task SaveRefreshToken(IdentityUser user, string refreshToken);
        public Task<IdentityUser?> GetUserByRefreshToken(string refreshToken);
        Task UpdateUserProfileAsync(string token, UpdateUserProfileDTO request);
    }
}
