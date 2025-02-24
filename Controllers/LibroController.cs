using Babel.Models.DTOs;
using Babel.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class LibroController : ControllerBase
{
    private readonly ILibroService _libroService;

    public LibroController(ILibroService libroService)
    {
        _libroService = libroService;
    }

    [HttpGet]
    public async Task<IActionResult> GetLibros()
    {
        var libros = await _libroService.GetAllAsync();
        return Ok(libros);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLibro(int id)
    {
        var libro = await _libroService.GetByIdAsync(id);
        if (libro == null)
        {
            return NotFound();
        }
        return Ok(libro);
    }
    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var libro = await _libroService.GetBySlugAsync(slug);
        if (libro == null) return NotFound(new { message = "Libro no encontrado." });

        return Ok(libro);
    }
    [HttpGet("autor/{autorId}")]
    public async Task<ActionResult<IEnumerable<LibroDTO>>> GetLibrosByAutor(int autorId)
    {
        var libros = await _libroService.GetByAutorIdAsync(autorId);
        if (libros == null || !libros.Any()) return NotFound("No se encontraron libros para este autor.");
        return Ok(libros);
    }

    [HttpPost]
    public async Task<ActionResult<LibroDTO>> PostLibro(LibroDTO libroDto)
    {
        var createdLibro = await _libroService.CreateAsync(libroDto);
        return CreatedAtAction(nameof(GetLibro), new { id = createdLibro.Id }, createdLibro);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutLibro(int id, LibroDTO libroDto)
    {
        try
        {
            var updatedLibro = await _libroService.UpdateAsync(id, libroDto);
            if (updatedLibro == null)
            {
                return NotFound();
            }
            return Ok(updatedLibro);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("No tienes permisos para modificar este libro.");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLibro(int id)
    {
        try
        {
            var deleted = await _libroService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("No tienes permisos para eliminar este libro.");
        }
    }
}
