namespace Babel.Models.Entities;
using System;
using System.Collections.Generic;
public class Libro
{
    public int Id { get; set; }
    public string Titulo { get; set; }
    public string Descripcion { get; set; }
    public string Slug { get; set; }
    public DateTime? FechaPublicacion { get; set; }
    public DateTime? FechaUltimaEdicion { get; set; }
    public int AutorId { get; set; }
    public string Color { get; set; }
    public Usuario Autor { get; set; }
    public ICollection<LibroEtiqueta> LibrosEtiquetas { get; set; }
    public ICollection<Like> Likes { get; set; }
}
