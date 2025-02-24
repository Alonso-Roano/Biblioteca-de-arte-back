namespace Babel.Controllers
{
    using Babel.Models.DTOs;
    using Babel.Services.IService;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;

    [Route("api/[controller]")]
    [ApiController]
    public class EtiquetaController : ControllerBase
    {
        private readonly IEtiquetaService _etiquetaService;

        public EtiquetaController(IEtiquetaService etiquetaService)
        {
            _etiquetaService = etiquetaService;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EtiquetaDTO>>> GetEtiquetas()
        {
            var etiquetas = await _etiquetaService.GetAllAsync();
            return Ok(etiquetas);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<EtiquetaDTO>> GetEtiqueta(int id)
        {
            var etiqueta = await _etiquetaService.GetByIdAsync(id);
            if (etiqueta == null)
                return NotFound();

            return Ok(etiqueta);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<EtiquetaDTO>> PostEtiqueta(EtiquetaDTO etiqueta)
        {
            var createdEtiqueta = await _etiquetaService.CreateAsync(etiqueta);
            return CreatedAtAction(nameof(GetEtiqueta), new { id = createdEtiqueta.Id }, createdEtiqueta);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<EtiquetaDTO>> PutEtiqueta(int id, EtiquetaDTO etiqueta)
        {
            var updatedEtiqueta = await _etiquetaService.UpdateAsync(id, etiqueta);
            if (updatedEtiqueta == null)
                return NotFound();

            return Ok(updatedEtiqueta);
        }

        [Authorize(Roles ="Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteEtiqueta(int id)
        {
            var success = await _etiquetaService.DeleteAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
