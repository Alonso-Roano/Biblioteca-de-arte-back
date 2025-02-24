namespace Babel.Services.Service;
using Babel.Context;
using Babel.Models.DTOs;
using Babel.Models.Entities;
using Babel.Services.IService;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

public class UsuarioService : IUsuarioService
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UsuarioService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
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

    public async Task<UsuarioDTO> UpdateAsync(int id, UsuarioDTO usuarioDto)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null) return null;

        if (!UsuarioPuedeModificarOEliminar(usuario))
        {
            throw new UnauthorizedAccessException("No tienes permisos para modificar este usuario.");
        }

        usuario.Nombre = usuarioDto.Nombre;
        usuario.Alias = usuarioDto.Alias ?? usuario.Alias;
        usuario.Email = usuarioDto.Email;

        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();

        return new UsuarioDTO
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Alias = usuario.Alias,
            Email = usuario.Email
        };
    }

    private bool UsuarioPuedeModificarOEliminar(Usuario usuario)
    {
        var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        bool esAdmin = _httpContextAccessor.HttpContext.User.IsInRole("Admin");

        return esAdmin || usuario.AspNetUserId == userId;
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

