namespace Babel.Services.IService
{
    using Babel.Models.DTOs;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ILibroService
    {
        Task<IEnumerable<LibroDTO>> GetAllAsync();
        Task<LibroDTO> GetByIdAsync(int id);
        Task<IEnumerable<LibroDTO>> GetByAutorIdAsync(int autorId);
        Task<LibroDTO> CreateAsync(LibroDTO libroDto);
        Task<LibroDTO> UpdateAsync(int id, LibroDTO libroDto);
        Task<bool> DeleteAsync(int id);
        Task<LibroDTO> GetBySlugAsync(string slug);
    }
}