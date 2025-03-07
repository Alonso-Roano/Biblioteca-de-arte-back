namespace Babel.Models.DTOs
{
    public class LibroCreateDTO
    {
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public DateTime FechaUltimaEdicion { get; set; }
        public string AspNetUserId { get; set; }
        public string Color { get; set; }
        public List<int> EtiquetaIds { get; set; } = new List<int>();
    }
}
