using Babel.Context;
using Babel.Models.Entities;
using Babel.Models.DTOs;
using Babel.Services.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Drawing;

public class LibroService : ILibroService
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LibroService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<IEnumerable<LibroDTO>> GetAllAsync()
    {
        return await _context.Libros
            .Include(l => l.Autor)
            .Include(l => l.LibrosEtiquetas)
                .ThenInclude(le => le.Etiqueta)
            .Select(l => new LibroDTO
            {
                Id = l.Id,
                Titulo = l.Titulo,
                Slug = l.Slug,
                Descripcion = l.Descripcion,
                FechaPublicacion = l.FechaPublicacion,
                FechaUltimaEdicion = l.FechaUltimaEdicion,
                AutorId = l.AutorId,
                Color = l.Color,
                AutorNombre = l.Autor.Nombre,
                EtiquetaIds = l.LibrosEtiquetas.Select(le => le.EtiquetaId).ToList()
            })
            .ToListAsync();
    }
    public async Task<LibroDTO> GetByIdAsync(int id)
    {
        var libro = await _context.Libros
            .Include(l => l.Autor)
            .Include(l => l.LibrosEtiquetas)
                .ThenInclude(le => le.Etiqueta)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (libro == null) return null;

        return new LibroDTO
        {
            Id = libro.Id,
            Titulo = libro.Titulo,
            Slug = libro.Slug,
            Descripcion = libro.Descripcion,
            FechaPublicacion = libro.FechaPublicacion,
            FechaUltimaEdicion = libro.FechaUltimaEdicion,
            AutorId = libro.AutorId,
            AutorNombre = libro.Autor.Nombre,
            Color = libro.Color,
            EtiquetaIds = libro.LibrosEtiquetas.Select(le => le.EtiquetaId).ToList()
        };
    }
    public async Task<LibroDTO> GetBySlugAsync(string slug)
    {
        var libro = await _context.Libros
            .Include(l => l.Autor)
            .Include(l => l.LibrosEtiquetas)
                .ThenInclude(le => le.Etiqueta)
            .FirstOrDefaultAsync(l => l.Slug == slug);

        if (libro == null) return null;

        return new LibroDTO
        {
            Id = libro.Id,
            Titulo = libro.Titulo,
            Slug = libro.Slug,
            Descripcion = libro.Descripcion,
            FechaPublicacion = libro.FechaPublicacion,
            FechaUltimaEdicion = libro.FechaUltimaEdicion,
            AutorId = libro.AutorId,
            AutorNombre = libro.Autor.Nombre,
            Color = libro.Color,
            EtiquetaIds = libro.LibrosEtiquetas.Select(le => le.EtiquetaId).ToList()
        };
    }
    public async Task<IEnumerable<LibroDTO>> GetByAutorIdAsync(int autorId)
    {
        return await _context.Libros
            .Where(l => l.AutorId == autorId)
            .Include(l => l.Autor)
            .Include(l => l.LibrosEtiquetas)
                .ThenInclude(le => le.Etiqueta)
            .Select(l => new LibroDTO
            {
                Id = l.Id,
                Titulo = l.Titulo,
                Slug = l.Slug,
                Descripcion = l.Descripcion,
                FechaPublicacion = l.FechaPublicacion,
                FechaUltimaEdicion = l.FechaUltimaEdicion,
                AutorId = l.AutorId,
                AutorNombre = l.Autor.Nombre,
                Color = l.Color,
                EtiquetaIds = l.LibrosEtiquetas.Select(le => le.EtiquetaId).ToList()
            })
            .ToListAsync();
    }
    public async Task<LibroDTO> CreateAsync(LibroDTO libroDto)
    {
        var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.AspNetUserId == userId);

        if (usuario == null)
        {
            throw new InvalidOperationException("Usuario no encontrado.");
        }

        var libro = new Libro
        {
            Titulo = libroDto.Titulo,
            Descripcion = libroDto.Descripcion,
            FechaPublicacion = libroDto.FechaPublicacion,
            FechaUltimaEdicion = libroDto.FechaUltimaEdicion,
            AutorId = usuario.Id,
            Color = libroDto.Color,
            Slug = await GenerateUniqueSlug(libroDto.Titulo)
        };

        if (libroDto.EtiquetaIds != null && libroDto.EtiquetaIds.Any())
        {
            libro.LibrosEtiquetas = libroDto.EtiquetaIds.Select(ei => new LibroEtiqueta { EtiquetaId = ei }).ToList();
        }

        _context.Libros.Add(libro);
        await _context.SaveChangesAsync();

        return new LibroDTO
        {
            Id = libro.Id,
            Titulo = libro.Titulo,
            Slug = libro.Slug,
            Descripcion = libro.Descripcion,
            FechaPublicacion = libro.FechaPublicacion,
            FechaUltimaEdicion = libro.FechaUltimaEdicion,
            AutorId = usuario.Id,
            Color = libroDto.Color,
            EtiquetaIds = libro.LibrosEtiquetas.Select(le => le.EtiquetaId).ToList()
        };
    }


    private async Task<string> GenerateUniqueSlug(string titulo)
    {
        string baseSlug = NormalizeSlug(titulo);
        string slug = baseSlug;
        int count = 1;

        while (await _context.Libros.AnyAsync(l => l.Slug == slug))
        {
            slug = $"{baseSlug}_{count}";
            count++;
        }

        return slug;
    }
    private string NormalizeSlug(string titulo)
    {
        return titulo
            .ToLower()
            .Replace(" ", "_")
            .Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u")
            .Replace("ñ", "n");
    }


    public async Task<LibroDTO> UpdateAsync(int id, LibroDTO libroDto)
    {
        var libro = await _context.Libros.Include(l => l.Autor)
                                         .Include(l => l.LibrosEtiquetas)
                                         .FirstOrDefaultAsync(l => l.Id == id);

        if (libro == null) return null;

        if (!UsuarioPuedeModificarOEliminar(libro))
        {
            throw new UnauthorizedAccessException("No tienes permisos para modificar este libro.");
        }

        if (libro.Titulo != libroDto.Titulo)
        {
            libro.Slug = await GenerateUniqueSlug(libroDto.Titulo);
        }

        libro.Titulo = libroDto.Titulo;
        libro.Descripcion = libroDto.Descripcion;
        libro.FechaPublicacion = libroDto.FechaPublicacion;
        libro.FechaUltimaEdicion = DateTime.UtcNow;
        libro.Color = libroDto.Color;

        if (libroDto.EtiquetaIds != null)
        {
            _context.LibrosEtiquetas.RemoveRange(libro.LibrosEtiquetas);

            libro.LibrosEtiquetas = libroDto.EtiquetaIds.Select(ei => new LibroEtiqueta { EtiquetaId = ei }).ToList();
        }

        _context.Libros.Update(libro);
        await _context.SaveChangesAsync();

        return new LibroDTO
        {
            Id = libro.Id,
            Titulo = libro.Titulo,
            Slug = libro.Slug,
            Descripcion = libro.Descripcion,
            FechaPublicacion = libro.FechaPublicacion,
            FechaUltimaEdicion = libro.FechaUltimaEdicion,
            AutorId = libro.AutorId,
            Color = libro.Color,
            EtiquetaIds = libro.LibrosEtiquetas.Select(le => le.EtiquetaId).ToList()
        };
    }



    public async Task<bool> DeleteAsync(int id)
    {
        var libro = await _context.Libros.Include(l => l.Autor).FirstOrDefaultAsync(l => l.Id == id);
        if (libro == null) return false;

        if (!UsuarioPuedeModificarOEliminar(libro))
        {
            throw new UnauthorizedAccessException("No tienes permisos para eliminar este libro.");
        }

        _context.Libros.Remove(libro);
        await _context.SaveChangesAsync();
        return true;
    }

    private bool UsuarioPuedeModificarOEliminar(Libro libro)
    {
        var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        bool esAdmin = _httpContextAccessor.HttpContext.User.IsInRole("Admin");

        return esAdmin || libro.Autor.AspNetUserId == userId;
    }
}
