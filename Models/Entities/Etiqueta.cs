namespace Babel.Models.Entities;
using System.Collections.Generic;

public class Etiqueta
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public ICollection<LibroEtiqueta> LibrosEtiquetas { get; set; }
}
