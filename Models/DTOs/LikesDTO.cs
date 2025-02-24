namespace Babel.Models.DTOs;
public class LikesDTO
{
    public int LibroId { get; set; }
    public int TotalLikes { get; set; }
    public List<string> UsuariosQueDieronLike { get; set; }
}

