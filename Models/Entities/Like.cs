namespace Babel.Models.Entities;
using System;
public class Like
{
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; }
    public int LibroId { get; set; }
    public Libro Libro { get; set; }
    public DateTime FechaLike { get; set; } = DateTime.Now;
}