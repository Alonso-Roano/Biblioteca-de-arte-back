namespace Babel.Services.IService
{
    using Babel.Models.DTOs;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using static LibroService;

    public interface ILibroService
    {
        Task<IEnumerable<LibroDTO>> GetAllAsync(int skip, int take);
        Task<LibroDTO> GetByIdAsync(int id);
        Task<LibroDTO> GetBySlugAsync(string slug);
        Task<IEnumerable<LibroDTO>> GetLibrosByAutorAsync(string token);
        Task<LibroDTO> CreateAsync(LibroDTO libroDto);
        Task<LibroDTO> UpdateAsync(int id, LibroDTO libroDto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<LibroQueryDTO>> GetByTituloAsync(string titulo, int skip, int take);
        Task<IEnumerable<LibroQueryDTO>> GetByEtiquetaNombreAsync(string etiqueta, int skip, int take);
        Task<IEnumerable<LibroQueryDTO>> GetByAutorNombreAsync(string autor, int skip, int take);
        Task<IEnumerable<LibroQueryDTO>> GetLibrosMasGustadosAsync();
        Task<LibroCreateDTO> CreateLibroAspAsync(LibroCreateDTO libroDto);
    }
}