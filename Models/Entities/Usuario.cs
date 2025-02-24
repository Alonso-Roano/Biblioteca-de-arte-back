namespace Babel.Models.Entities;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Alias { get; set; } = "Anonimo";
    public string Email { get; set; }
    public string AspNetUserId { get; set; }
    public IdentityUser AspNetUser { get; set; } 
    public ICollection<Libro> Libros { get; set; }
    public ICollection<Like> Likes { get; set; }
}
