namespace Babel.Services.IService;
using System.Threading.Tasks;
using Babel.Models.DTOs;

public interface ILikeService
{
    Task<bool> ToggleLikeAsync(int libroId);
    Task<LikesDTO> GetLibroLikesInfoAsync(int libroId);
    Task<List<LibroLikesDTO>> GetAllLikesInfoAsync();
    Task<bool> DeleteAllLikesAsync();


}
