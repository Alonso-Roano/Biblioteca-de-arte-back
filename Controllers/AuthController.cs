namespace Babel.Controllers;

using Babel.Context;
using Babel.Models.DTOs.Auth;
using Babel.Models.Entities;
using Babel.Services.IService;
using Babel.Services.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Babel.Services.Service.TokenService;

[Route("api/Auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ITokenService _tokenService;

    private static Dictionary<string, string> refreshTokens = new();

    public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        var response = await _tokenService.RegisterAsync(request);
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var response = await _tokenService.LoginAsync(request);
        return Ok(response);
    }


    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        var user = await _tokenService.GetUserByRefreshToken(request.RefreshToken);
        if (user == null)
            return Unauthorized("Refresh Token inválido");

        var roles = await _userManager.GetRolesAsync(user);
        var newAccessToken = await _tokenService.GenerateJwtToken(user, roles[0]);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        await _tokenService.SaveRefreshToken(user, newRefreshToken);

        return Ok(new { AccessToken = newAccessToken, RefreshToken = newRefreshToken });
    }
    [HttpGet("metrics")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<DashboardMetricsDTO>> GetDashboardMetrics()
    {
        try
        {
            var metrics = await _tokenService.GetDashboardMetricsAsync();

            if (metrics == null)
            {
                return NotFound("No se pudieron obtener las métricas.");
            }

            return Ok(metrics);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }
    [HttpPut("update-profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileDTO request, string token)
    {
        try
        {

            await _tokenService.UpdateUserProfileAsync(token, request);
            return Ok(new { message = "Perfil actualizado correctamente.", request });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}