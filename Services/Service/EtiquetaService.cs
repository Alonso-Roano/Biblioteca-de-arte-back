namespace Babel.Services
{
    using Babel.Context;
    using Babel.Models.DTOs;
    using Babel.Models.Entities;
    using Babel.Services.IService;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class EtiquetaService : IEtiquetaService
    {
        private readonly ApplicationDbContext _context;

        public EtiquetaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EtiquetaDTO>> GetAllAsync()
        {
            return await _context.Etiquetas
                .Select(e => new EtiquetaDTO
                {
                    Id = e.Id,
                    Nombre = e.Nombre
                })
                .ToListAsync();
        }

        public async Task<EtiquetaDTO> GetByIdAsync(int id)
        {
            var etiqueta = await _context.Etiquetas
                .Where(e => e.Id == id)
                .Select(e => new EtiquetaDTO
                {
                    Id = e.Id,
                    Nombre = e.Nombre
                })
                .FirstOrDefaultAsync();
            return etiqueta;
        }

        public async Task<EtiquetaDTO> CreateAsync(EtiquetaDTO etiquetaDto)
        {
            var etiqueta = new Etiqueta
            {
                Id = etiquetaDto.Id,
                Nombre = etiquetaDto.Nombre
            };

            _context.Etiquetas.Add(etiqueta);
            await _context.SaveChangesAsync();

            var etiquetaResponseDto = new EtiquetaDTO
            {
                Id = etiqueta.Id,
                Nombre = etiqueta.Nombre
            };

            return etiquetaResponseDto;
        }


        public async Task<EtiquetaDTO> UpdateAsync(int id, EtiquetaDTO etiquetaDto)
        {
            var existingEtiqueta = await _context.Etiquetas.FindAsync(id);
            if (existingEtiqueta == null)
                return null;

            existingEtiqueta.Nombre = etiquetaDto.Nombre;

            _context.Etiquetas.Update(existingEtiqueta);
            await _context.SaveChangesAsync();

            var updatedEtiquetaDto = new EtiquetaDTO
            {
                Nombre = existingEtiqueta.Nombre
            };

            return updatedEtiquetaDto;
        }



        public async Task<bool> DeleteAsync(int id)
        {
            var etiqueta = await _context.Etiquetas.FindAsync(id);
            if (etiqueta == null)
                return false;

            _context.Etiquetas.Remove(etiqueta);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
