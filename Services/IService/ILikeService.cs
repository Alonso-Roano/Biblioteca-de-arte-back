namespace Babel.Services.IService;
using System.Threading.Tasks;
using Babel.Models.DTOs;

public interface ILikeService
{
    Task<bool> ToggleLikeAsync(int libroId, int usuarioId);
    Task<LikesDTO> GetLibroLikesInfoAsync(int libroId);
    Task<List<LibroLikesDTO>> GetAllLikesInfoAsync();
    Task<bool> DeleteAllLikesAsync();
    Task<List<LibroDTO>> GetLikesByUserAsync(string token);
    Task<bool> ToggleLikeByUserAsync(int libroId, string token);


}
