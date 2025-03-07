namespace Babel.Controllers;
using Babel.Models.DTOs;
using Babel.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class UsuarioController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuarioController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var usuarios = await _usuarioService.GetAllAsync();
        return Ok(usuarios);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(int id)
    {
        var usuario = await _usuarioService.GetByIdAsync(id);
        if (usuario == null) return NotFound();

        return Ok(usuario);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] UsuarioDTO usuarioDto)
    {
        var usuario = await _usuarioService.CreateAsync(usuarioDto);
        return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, usuario);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UsuarioCrear usuarioDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var usuario = await _usuarioService.GetByIdAsync(id);
        if (usuario == null) return NotFound();

        var updatedUsuario = await _usuarioService.UpdateAsync(id, usuarioDto);
        if (updatedUsuario == null) return BadRequest();

        return Ok(updatedUsuario);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _usuarioService.DeleteAsync(id);
        if (!success) return NotFound();

        return Ok(new { message = "Usuario eliminado correctamente", userId = id });
    }
}
