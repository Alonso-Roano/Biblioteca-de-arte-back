namespace Babel.Services.Service;
using Babel.Context;
using Babel.Models.DTOs;
using Babel.Models.Entities;
using Babel.Services.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

public class LikeService : ILikeService
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LikeService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<bool> ToggleLikeAsync(int libroId)
    {
        var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException("Debes estar autenticado para dar like.");
        }

        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.AspNetUserId == userId);
        if (usuario == null)
        {
            throw new UnauthorizedAccessException("Usuario no encontrado.");
        }

        var likeExistente = await _context.Likes
            .FirstOrDefaultAsync(l => l.LibroId == libroId && l.UsuarioId == usuario.Id);

        if (likeExistente != null)
        {
            _context.Likes.Remove(likeExistente);
            await _context.SaveChangesAsync();
            return false;
        }

        var nuevoLike = new Like
        {
            UsuarioId = usuario.Id,
            LibroId = libroId,
            FechaLike = DateTime.UtcNow
        };

        _context.Likes.Add(nuevoLike);
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<LikesDTO> GetLibroLikesInfoAsync(int libroId)
    {
        var likes = await _context.Likes
            .Where(l => l.LibroId == libroId)
            .Include(l => l.Usuario)
            .ToListAsync();

        return new LikesDTO
        {
            LibroId = libroId,
            TotalLikes = likes.Count,
            UsuariosQueDieronLike = likes.Select(l => l.Usuario.Nombre).ToList()
        };
    }
    public async Task<List<LibroLikesDTO>> GetAllLikesInfoAsync()
    {
        var likesGrouped = await _context.Likes
            .Include(l => l.Usuario)
            .Include(l => l.Libro)
            .GroupBy(l => l.LibroId)
            .Select(g => new LibroLikesDTO
            {
                LibroId = g.Key,
                TituloLibro = g.First().Libro.Titulo,
                TotalLikes = g.Count(),
                UsuariosQueDieronLike = g.Select(l => l.Usuario.Nombre).ToList()
            })
            .ToListAsync();

        return likesGrouped;
    }
    public async Task<bool> DeleteAllLikesAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null || !httpContext.User.IsInRole("Admin"))
        {
            throw new UnauthorizedAccessException("No tienes permisos para eliminar todos los likes.");
        }

        var allLikes = await _context.Likes.ToListAsync();
        if (!allLikes.Any()) return false;

        _context.Likes.RemoveRange(allLikes);
        await _context.SaveChangesAsync();

        return true;
    }

}
