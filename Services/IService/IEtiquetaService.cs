namespace Babel.Services.IService
{
    using Babel.Models.DTOs;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IEtiquetaService
    {
        Task<IEnumerable<EtiquetaDTO>> GetAllAsync();
        Task<EtiquetaDTO> GetByIdAsync(int id);
        Task<EtiquetaDTO> CreateAsync(EtiquetaDTO etiqueta);
        Task<EtiquetaDTO> UpdateAsync(int id, EtiquetaDTO etiqueta);
        Task<bool> DeleteAsync(int id);
    }
}