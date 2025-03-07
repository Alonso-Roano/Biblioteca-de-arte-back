using Babel.Models.DTOs;
using Babel.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
    public async Task<IActionResult> GetLibros([FromQuery] int skip = 0, [FromQuery] int take = 20)
    {
        var libros = await _libroService.GetAllAsync(skip, take);
        return Ok(libros);
    }

    [HttpGet("buscar")]
    public async Task<ActionResult<IEnumerable<LibroQueryDTO>>> Buscar(
        [FromQuery] string? titulo,
        [FromQuery] string? autor,
        [FromQuery] string? etiqueta,
        [FromQuery] bool populares = false,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20)
    {
        IEnumerable<LibroQueryDTO> libros = new List<LibroQueryDTO>();

        if (!string.IsNullOrEmpty(titulo))
        {
            libros = await _libroService.GetByTituloAsync(titulo, skip, take);
        }
        else if (!string.IsNullOrEmpty(autor))
        {
            libros = await _libroService.GetByAutorNombreAsync(autor, skip, take);
        }
        else if (!string.IsNullOrEmpty(etiqueta))
        {
            libros = await _libroService.GetByEtiquetaNombreAsync(etiqueta, skip, take);
        }
        else if (populares)
        {
            libros = await _libroService.GetLibrosMasGustadosAsync();
        }
        else
        {
            return BadRequest("Debes especificar un criterio de búsqueda.");
        }

        return Ok(libros);
    }
    [Authorize]
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
    [Authorize]
    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var libro = await _libroService.GetBySlugAsync(slug);
        if (libro == null) return NotFound(new { message = "Libro no encontrado." });

        return Ok(libro);
    }

    [Authorize]
    [HttpGet("autor")]
    public async Task<ActionResult<IEnumerable<LibroDTO>>> GetLibrosByAutor(string token)
    {
        var libros = await _libroService.GetLibrosByAutorAsync(token);
        if (libros == null || !libros.Any()) return NotFound("No se encontraron libros para este autor.");
        return Ok(libros);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<LibroDTO>> PostLibro(LibroDTO libroDto)
    {
        var createdLibro = await _libroService.CreateAsync(libroDto);
        return CreatedAtAction(nameof(GetLibro), new { id = createdLibro.Id }, createdLibro);
    }

    [Authorize]
    [HttpPost("user")]
    public async Task<ActionResult<LibroCreateDTO>> PostLibroUser(LibroCreateDTO libroDto)
    {
        var createdLibro = await _libroService.CreateLibroAspAsync(libroDto);
        return Ok(createdLibro); 
    }


    [Authorize]
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

    [Authorize]
    [Authorize(Roles = "Admin")]
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
            return Ok(new { message = "Libro eliminado correctamente", libroID = id });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("No tienes permisos para eliminar este libro.");
        }
    }
}
