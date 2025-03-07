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
    using Babel.Models.DTOs.Auth;
    using Babel.Models.Entities;
    using Babel.Context;

    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;
        public TokenService(IConfiguration config, UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _config = config;
            _userManager = userManager;
            _context = context;
        }
        public async Task<AuthResponseDTO> RegisterAsync(RegisterRequestDto request)
        {
            try
            {
                var existingUserByEmail = await _userManager.FindByEmailAsync(request.Email);

                if (existingUserByEmail != null)
                {
                    throw new Exception("El correo electrónico o nombre de usuario ya están en uso.");
                }

                var user = new IdentityUser { UserName = $"user_{Guid.NewGuid().ToString("N").Substring(0, 8)}", Email = request.Email };
                var result = await _userManager.CreateAsync(user, request.Password);

                if (!result.Succeeded)
                {
                    var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Error al registrar usuario: {errorMessages}");
                }

                await _userManager.AddToRoleAsync(user, "User");

                var usuario = new Usuario
                {
                    Nombre = request.Username,
                    Email = request.Email,
                    Alias = request.Alias ?? "Anonimo",
                    profileImage = "icono_1.png",
                    AspNetUserId = user.Id
                };

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                var roles = await _userManager.GetRolesAsync(user);
                var accessToken = await GenerateJwtToken(user, roles.FirstOrDefault() ?? "User");
                var refreshToken = GenerateRefreshToken();
                await SaveRefreshToken(user, refreshToken);

                return new AuthResponseDTO
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    User = new UserDTO
                    {
                        Id = usuario.Id.ToString(),
                        AspNetUserId = user.Id,
                        Nombre = usuario.Nombre,
                        Email = usuario.Email,
                        Alias = usuario.Alias,
                        UserName = user.UserName
                    }
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en el registro: {ex.Message}");
            }
        }

        public async Task<AuthResponseDTO> LoginAsync(LoginRequestDto request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Username);

                if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
                    throw new UnauthorizedAccessException("Usuario o contraseña incorrectos");

                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.AspNetUserId == user.Id);
                if (usuario == null)
                    throw new Exception("El usuario no tiene datos asociados en la base de datos");

                var roles = await _userManager.GetRolesAsync(user);
                var accessToken = await GenerateJwtToken(user, roles.FirstOrDefault() ?? "User");
                var refreshToken = GenerateRefreshToken();

                await SaveRefreshToken(user, refreshToken);

                return new AuthResponseDTO
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    User = new UserDTO
                    {
                        Id = usuario.Id.ToString(),
                        AspNetUserId = user.Id,
                        Nombre = usuario.Nombre,
                        Email = usuario.Email,
                        Alias = usuario.Alias,
                        UserName = user.UserName
                    }
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en el inicio de sesión: {ex.Message}");
            }
        }



        public async Task<string> GenerateJwtToken(IdentityUser user, string role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.AspNetUserId == user.Id);

            var claims = new List<Claim>
    {
        new Claim("Id", user.Id),
        new Claim(ClaimTypes.Role, role),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            if (usuario != null)
            {
                claims.Add(new Claim("Nombre", usuario.Nombre));
                claims.Add(new Claim("Email", usuario.Email));
                claims.Add(new Claim("Alias", usuario.Alias));
                claims.Add(new Claim("Image", usuario.profileImage));
            }

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

        public async Task<DashboardMetricsDTO> GetDashboardMetricsAsync()
        {
            var totalLibros = await _context.Libros.CountAsync();
            var totalLikes = await _context.Likes.CountAsync();
            var totalUsuarios = await _context.Usuarios.CountAsync();
            var totalEtiquetas = await _context.Etiquetas.CountAsync();

            return new DashboardMetricsDTO
            {
                TotalLibros = totalLibros,
                TotalLikes = totalLikes,
                TotalUsuarios = totalUsuarios,
                TotalEtiquetas = totalEtiquetas
            };
        }
        public async Task UpdateUserProfileAsync(string token, UpdateUserProfileDTO request)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var aspNetUserId = jwtToken.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

            if (string.IsNullOrEmpty(aspNetUserId))
                throw new UnauthorizedAccessException("No se pudo identificar al usuario.");

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.AspNetUserId == aspNetUserId);
            if (usuario == null)
                throw new KeyNotFoundException("Usuario no encontrado.");

            usuario.Nombre = request.Nombre ?? usuario.Nombre;
            usuario.Alias = request.Alias ?? usuario.Alias;
            usuario.profileImage = request.Image ?? usuario.profileImage;


            if (!string.IsNullOrWhiteSpace(request.NewPassword))
            {
                var user = await _userManager.FindByIdAsync(aspNetUserId);
                if (user == null)
                    throw new KeyNotFoundException("Usuario de autenticación no encontrado.");

                var tokenChange = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, tokenChange, request.NewPassword);

                if (!result.Succeeded)
                    throw new Exception($"Error al cambiar la contraseña: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            await _context.SaveChangesAsync();
        }
        public class UpdateUserProfileDTO
        {
            public string? Nombre { get; set; }
            public string? Alias { get; set; }
            public string? Image { get; set; }
            public string? NewPassword { get; set; }
        }

    }

    public class DashboardMetricsDTO
    {
        public int TotalLibros { get; set; }
        public int TotalLikes { get; set; }
        public int TotalUsuarios { get; set; }
        public int TotalEtiquetas { get; set; }
    }
}
