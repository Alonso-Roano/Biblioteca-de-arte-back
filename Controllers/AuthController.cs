namespace Babel.Controllers;

using Babel.Context;
using Babel.Models.DTOs.Auth;
using Babel.Models.Entities;
using Babel.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/Auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly ApplicationDbContext _context;

    private static Dictionary<string, string> refreshTokens = new();

    public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ITokenService tokenService, ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        var user = new IdentityUser { UserName = request.Username, Email = request.Email };
        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        await _userManager.AddToRoleAsync(user, "User");

        var usuario = new Usuario
        {
            Nombre = request.Username,
            Email = request.Email,
            Alias = request.Alias ?? "Anonimo",
            AspNetUserId = user.Id
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _tokenService.GenerateJwtToken(user, roles.FirstOrDefault() ?? "User");
        var refreshToken = _tokenService.GenerateRefreshToken();

        await _tokenService.SaveRefreshToken(user, refreshToken);

        return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var user = await _userManager.FindByNameAsync(request.Username)
                   ?? await _userManager.FindByEmailAsync(request.Username);

        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            return Unauthorized("Usuario o contraseña incorrectos");

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _tokenService.GenerateJwtToken(user, roles.FirstOrDefault() ?? "User");
        var refreshToken = _tokenService.GenerateRefreshToken();

        await _tokenService.SaveRefreshToken(user, refreshToken);

        return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
    }


    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        var user = await _tokenService.GetUserByRefreshToken(request.RefreshToken);
        if (user == null)
            return Unauthorized("Refresh Token inválido");

        var roles = await _userManager.GetRolesAsync(user);
        var newAccessToken = _tokenService.GenerateJwtToken(user, roles[0]);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        await _tokenService.SaveRefreshToken(user, newRefreshToken);

        return Ok(new { AccessToken = newAccessToken, RefreshToken = newRefreshToken });
    }
}