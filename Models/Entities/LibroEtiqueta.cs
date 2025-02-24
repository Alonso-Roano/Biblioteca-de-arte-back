namespace Babel.Models.Entities;
public class LibroEtiqueta
{
    public int LibroId { get; set; }
    public Libro Libro { get; set; }

    public int EtiquetaId { get; set; }
    public Etiqueta Etiqueta { get; set; }
}
