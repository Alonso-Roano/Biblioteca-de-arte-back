using Babel.Context;
using Babel.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Babel.Controllers;

[ApiController]
[Route("api/")]
public class SeederController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ApplicationDbContext _context;

    public SeederController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    [HttpGet("Seeder")]
    public async Task<IActionResult> SeedUsers()
    {

        await CreateUsers();
        await CreateBooks();
        await CreateTags();
        await AssignTagsToBooks();

        return Ok("Datos creados correctamente.");
    }
    private async Task CreateRoles()
    {
        var roles = new List<string> { "User", "Admin" };

        foreach (var role in roles)
        {
            var roleExist = await _context.Roles.AnyAsync(r => r.Name == role);
            if (!roleExist)
            {
                var identityRole = new IdentityRole(role)
                {
                    NormalizedName = role.ToUpper()
                };
                await _context.Roles.AddAsync(identityRole);
            }
        }
        await _context.SaveChangesAsync();
    }

    private async Task CreateUsers()
    {
        await CreateRoles();

        var users = new List<(string Username, string Email, string Password, string Role, string ProfilePicture)>
    {
        ("usuario1", "usuario1@example.com", "Password123!", "User", "icono_1.png"),
        ("usuario2", "usuario2@example.com", "Password123!", "User", "icono_1.png"),
        ("admin", "admin@example.com", "AdminPassword123!", "Admin", "icono_1.png")
    };

        foreach (var (username, email, password, role, profilePicture) in users)
        {
            var userExists = await _userManager.FindByNameAsync(username);
            if (userExists == null)
            {
                var user = new IdentityUser { UserName = username, Email = email };
                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    var usuario = new Usuario
                    {
                        Nombre = username,
                        Alias = username,
                        Email = email,
                        profileImage= profilePicture,
                        AspNetUserId = user.Id
                    };

                    await _context.Usuarios.AddAsync(usuario);
                    await _context.SaveChangesAsync();

                    await _userManager.AddToRoleAsync(user, role);
                }
                else
                {
                    throw new Exception($"Error creando usuario {username}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                var usuarioExistente = await _context.Usuarios
                                                      .FirstOrDefaultAsync(u => u.Email == email);
                if (usuarioExistente == null)
                {
                    var usuario = new Usuario
                    {
                        Nombre = username,
                        Alias = username,
                        Email = email,
                        AspNetUserId = userExists.Id
                    };

                    await _context.Usuarios.AddAsync(usuario);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
    private async Task CreateBooks()
    {
        var usuarios = await _context.Usuarios.ToListAsync();

        var libros = new List<Libro>
    {
        new Libro { Titulo = "Libro 1", Descripcion = "Descripción libro 1", FechaPublicacion = DateTime.Now, AutorId = usuarios[0].Id, Slug = "Libro_1", Color = "#fff"  },
        new Libro { Titulo = "Libro 2", Descripcion = "Descripción libro 2", FechaPublicacion = DateTime.Now, AutorId = usuarios[1].Id, Slug = "Libro_2", Color = "#000"  },
        new Libro { Titulo = "Libro 3", Descripcion = "Descripción libro 3", FechaPublicacion = DateTime.Now, AutorId = usuarios[2].Id, Slug = "Libro_3", Color = "#f00"  },
        new Libro { Titulo = "Libro 4", Descripcion = "Descripción libro 4", FechaPublicacion = DateTime.Now, AutorId = usuarios[0].Id, Slug = "Libro_4", Color = "#00f"  },
        new Libro { Titulo = "Libro 5", Descripcion = "Descripción libro 5", FechaPublicacion = DateTime.Now, AutorId = usuarios[1].Id, Slug = "Libro_5", Color = "#0f0"  }
    };

        await _context.AddRangeAsync(libros);
        await _context.SaveChangesAsync();
    }


    private async Task CreateTags()
    {
        var etiquetas = new List<Etiqueta>
        {
            new Etiqueta { Nombre = "Ficción" },
            new Etiqueta { Nombre = "Aventura" }
        };

        await _context.Etiquetas.AddRangeAsync(etiquetas);
        await _context.SaveChangesAsync();
    }

    private async Task AssignTagsToBooks()
    {
        var libros = await _context.Libros.ToListAsync();
        var etiquetas = await _context.Etiquetas.ToListAsync();

        var librosEtiquetas = new List<LibroEtiqueta>();
        foreach (var libro in libros)
        {
            foreach (var etiqueta in etiquetas)
            {
                librosEtiquetas.Add(new LibroEtiqueta { LibroId = libro.Id, EtiquetaId = etiqueta.Id });
            }
        }

        await _context.LibrosEtiquetas.AddRangeAsync(librosEtiquetas);
        await _context.SaveChangesAsync();
    }

}