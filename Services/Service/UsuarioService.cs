namespace Babel.Services.Service;
using Babel.Context;
using Babel.Models.DTOs;
using Babel.Models.Entities;
using Babel.Services.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

public class UsuarioService : IUsuarioService
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<IdentityUser> _userManager;

    public UsuarioService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
    }

    public async Task<IEnumerable<UsuarioDTO>> GetAllAsync()
    {
        return await _context.Usuarios
            .Select(u => new UsuarioDTO
            {
                Id = u.Id,
                Nombre = u.Nombre,
                Alias = u.Alias,
                Email = u.Email
            })
            .ToListAsync();
    }

    public async Task<UsuarioDTO> GetByIdAsync(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null) return null;

        return new UsuarioDTO
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Alias = usuario.Alias,
            Email = usuario.Email
        };
    }

    public async Task<UsuarioDTO> GetByAspNetUserIdAsync(string aspNetUserId)
    {
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.AspNetUserId == aspNetUserId);

        if (usuario == null) return null;

        return new UsuarioDTO
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Alias = usuario.Alias,
            Email = usuario.Email
        };
    }

    public async Task<UsuarioDTO> CreateAsync(UsuarioDTO usuarioDto)
    {
        var usuario = new Usuario
        {
            Nombre = usuarioDto.Nombre,
            Alias = usuarioDto.Alias ?? "Anonimo",
            Email = usuarioDto.Email
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return new UsuarioDTO
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Alias = usuario.Alias,
            Email = usuario.Email
        };
    }

    public async Task<UsuarioCrear> UpdateAsync(int id, UsuarioCrear usuarioDto)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null) return null;

        if (!string.IsNullOrEmpty(usuarioDto.Nombre))
        {
            usuario.Nombre = usuarioDto.Nombre;
        }

        if (!string.IsNullOrEmpty(usuarioDto.Alias))
        {
            usuario.Alias = usuarioDto.Alias;
        }

        if (!string.IsNullOrEmpty(usuarioDto.Password))
        {
            var user = await _userManager.FindByIdAsync(usuario.AspNetUserId);
            if (user != null)
            {
                var result = await _userManager.RemovePasswordAsync(user); 
                if (!result.Succeeded)
                {
                    throw new Exception("Error al eliminar la contraseña actual.");
                }

                result = await _userManager.AddPasswordAsync(user, usuarioDto.Password); 
                if (!result.Succeeded)
                {
                    throw new Exception("Error al agregar la nueva contraseña.");
                }
            }
        }

        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();

        return new UsuarioCrear
        {
            Nombre = usuario.Nombre,
            Alias = usuario.Alias,
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null) return false;

        _context.Usuarios.Remove(usuario);
        await _context.SaveChangesAsync();

        return true;
    }
}

