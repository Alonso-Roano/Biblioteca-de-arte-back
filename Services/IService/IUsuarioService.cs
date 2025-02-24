namespace Babel.Services.IService;
using Babel.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IUsuarioService
{
    Task<IEnumerable<UsuarioDTO>> GetAllAsync();
    Task<UsuarioDTO> GetByIdAsync(int id);
    Task<UsuarioDTO> GetByAspNetUserIdAsync(string aspNetUserId);
    Task<UsuarioDTO> CreateAsync(UsuarioDTO usuarioDto);
    Task<UsuarioDTO> UpdateAsync(int id, UsuarioDTO usuarioDto);
    Task<bool> DeleteAsync(int id);
}
