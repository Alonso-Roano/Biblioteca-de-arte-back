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
using System.Globalization;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

public class LibroService : ILibroService
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LibroService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IEnumerable<LibroDTO>> GetAllAsync(int skip, int take)
    {
        return await _context.Libros
            .Include(l => l.Autor)
            .Include(l => l.LibrosEtiquetas)
                .ThenInclude(le => le.Etiqueta)
            .Skip(skip)
            .Take(take)
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

    public async Task<IEnumerable<LibroQueryDTO>> GetByTituloAsync(string titulo, int skip, int take)
    {
        string normalizedTitulo = NormalizeString(titulo);

        var libros = await _context.Libros
            .Include(l => l.Autor)
            .Include(l => l.LibrosEtiquetas).ThenInclude(le => le.Etiqueta)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        var resultado = libros
            .Where(l => NormalizeString(l.Titulo).Contains(normalizedTitulo))
            .Select(l => new LibroQueryDTO
            {
                Id = l.Id,
                Titulo = l.Titulo,
                Slug = l.Slug,
                Descripcion = l.Descripcion,
                FechaPublicacion = l.FechaPublicacion,
                FechaUltimaEdicion = l.FechaUltimaEdicion,
                AutorNombre = l.Autor.Nombre,
                Color = l.Color,
                Etiquetas = l.LibrosEtiquetas.Select(le => le.Etiqueta.Nombre).ToList()
            });

        return resultado;
    }

    public async Task<IEnumerable<LibroQueryDTO>> GetByEtiquetaNombreAsync(string etiqueta, int skip, int take)
    {
        string normalizedEtiqueta = NormalizeString(etiqueta); 

        var libros = await _context.Libros
            .Include(l => l.Autor)
            .Include(l => l.LibrosEtiquetas).ThenInclude(le => le.Etiqueta)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        var resultado = libros
            .Where(l => l.LibrosEtiquetas
                        .Any(le => NormalizeString(le.Etiqueta.Nombre).Contains(normalizedEtiqueta))) 
            .Select(l => new LibroQueryDTO
            {
                Id = l.Id,
                Titulo = l.Titulo,
                Slug = l.Slug,
                Descripcion = l.Descripcion,
                FechaPublicacion = l.FechaPublicacion,
                FechaUltimaEdicion = l.FechaUltimaEdicion,
                AutorNombre = l.Autor.Nombre,
                Color = l.Color,
                Etiquetas = l.LibrosEtiquetas.Select(le => le.Etiqueta.Nombre).ToList()
            });

        return resultado;
    }

    public async Task<IEnumerable<LibroQueryDTO>> GetByAutorNombreAsync(string autor, int skip, int take)
    {
        string normalizedAutor = NormalizeString(autor);

        var libros = await _context.Libros
            .Include(l => l.Autor)
            .Include(l => l.LibrosEtiquetas).ThenInclude(le => le.Etiqueta)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        var resultado = libros
            .Where(l => NormalizeString(l.Autor.Nombre).Contains(normalizedAutor))
            .Select(l => new LibroQueryDTO
            {
                Id = l.Id,
                Titulo = l.Titulo,
                Slug = l.Slug,
                Descripcion = l.Descripcion,
                FechaPublicacion = l.FechaPublicacion,
                FechaUltimaEdicion = l.FechaUltimaEdicion,
                AutorNombre = l.Autor.Nombre,
                Color = l.Color,
                Etiquetas = l.LibrosEtiquetas.Select(le => le.Etiqueta.Nombre).ToList()
            });

        return resultado;
    }


    public async Task<IEnumerable<LibroQueryDTO>> GetLibrosMasGustadosAsync()
    {
        return await _context.Libros
            .OrderByDescending(l => l.Likes.Count)
            .Include(l => l.Autor)
            .Include(l => l.LibrosEtiquetas).ThenInclude(le => le.Etiqueta)
            .Select(l => new LibroQueryDTO
            {
                Id = l.Id,
                Titulo = l.Titulo,
                Slug = l.Slug,
                Descripcion = l.Descripcion,
                FechaPublicacion = l.FechaPublicacion,
                FechaUltimaEdicion = l.FechaUltimaEdicion,
                AutorNombre = l.Autor.Nombre,
                Color = l.Color,
                Etiquetas = l.LibrosEtiquetas.Select(le => le.Etiqueta.Nombre).ToList()
            })
            .Take(4)
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

    public async Task<IEnumerable<LibroDTO>> GetLibrosByAutorAsync(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var aspNetUserId = jwtToken.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        if (string.IsNullOrEmpty(aspNetUserId))
        {
            throw new UnauthorizedAccessException("Token inválido o no contiene el ID de usuario.");
        }

        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.AspNetUserId == aspNetUserId);
        if (usuario == null)
        {
            throw new KeyNotFoundException("Usuario no encontrado.");
        }

        return await _context.Libros
            .Where(l => l.AutorId == usuario.Id)
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
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == libroDto.AutorId);

        if (usuario == null)
        {
            throw new InvalidOperationException("Autor no encontrado.");
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

    public async Task<LibroCreateDTO> CreateLibroAspAsync(LibroCreateDTO libroDto)
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.AspNetUserId == libroDto.AspNetUserId);

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

        return new LibroCreateDTO
        {
            Titulo = libro.Titulo,
            Descripcion = libro.Descripcion,
            AspNetUserId = libroDto.AspNetUserId,
            Color = libroDto.Color,
            EtiquetaIds = libro.LibrosEtiquetas.Select(le => le.EtiquetaId).ToList()
        };
    }
    


    private async Task<string> GenerateUniqueSlug(string titulo)
    {
        string baseSlug = NormalizeString(titulo);
        string slug = baseSlug;
        int count = 1;

        while (await _context.Libros.AnyAsync(l => l.Slug == slug))
        {
            slug = $"{baseSlug}_{count}";
            count++;
        }

        return slug;
    }

    private string NormalizeString(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        string normalized = input.Normalize(NormalizationForm.FormD);
        normalized = new string(normalized.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray());
        normalized = normalized.Normalize(NormalizationForm.FormC);

        normalized = normalized.Replace(" ", "").ToLower();
        normalized = normalized.Replace("+", "").ToLower();

        normalized = normalized.Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u")
                               .Replace("ñ", "n").Replace("Á", "a").Replace("É", "e").Replace("Í", "i").Replace("Ó", "o")
                               .Replace("Ú", "u").Replace("Ñ", "n");

        return normalized;
    }

    public async Task<LibroDTO> UpdateAsync(int id, LibroDTO libroDto)
    {
        var libro = await _context.Libros.Include(l => l.Autor)
                                         .Include(l => l.LibrosEtiquetas)
                                         .FirstOrDefaultAsync(l => l.Id == id);

        if (libro == null) return null;

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


        _context.Libros.Remove(libro);
        await _context.SaveChangesAsync();
        return true;
    }


   
}
