namespace Babel.Models.DTOs
{
    public class LibroDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Slug { get; set; }
        public DateTime? FechaPublicacion { get; set; }
        public DateTime? FechaUltimaEdicion { get; set; }
        public int AutorId { get; set; }
        public string Color { get; set; }
        public string AutorNombre { get; set; }
        public List<int> EtiquetaIds { get; set; }
    }
}
