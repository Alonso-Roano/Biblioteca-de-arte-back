namespace Babel.Context;

using Babel.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public DbSet<Libro> Libros { get; set; }
    public DbSet<Etiqueta> Etiquetas { get; set; }
    public DbSet<LibroEtiqueta> LibrosEtiquetas { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Usuario>()
            .HasOne(u => u.AspNetUser)
            .WithMany()
            .HasForeignKey(u => u.AspNetUserId)
            .OnDelete(DeleteBehavior.Restrict); 

        builder.Entity<LibroEtiqueta>()
            .HasKey(le => new { le.LibroId, le.EtiquetaId });

        builder.Entity<LibroEtiqueta>()
            .HasOne(le => le.Libro)
            .WithMany(l => l.LibrosEtiquetas)
            .HasForeignKey(le => le.LibroId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<LibroEtiqueta>()
            .HasOne(le => le.Etiqueta)
            .WithMany(e => e.LibrosEtiquetas)
            .HasForeignKey(le => le.EtiquetaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Like>()
            .HasKey(l => new { l.UsuarioId, l.LibroId });

        builder.Entity<Like>()
            .HasOne(l => l.Usuario)
            .WithMany(u => u.Likes)
            .HasForeignKey(l => l.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict); 

        builder.Entity<Like>()
            .HasOne(l => l.Libro)
            .WithMany(b => b.Likes)
            .HasForeignKey(l => l.LibroId)
            .OnDelete(DeleteBehavior.Restrict); 

        builder.Entity<Like>()
            .HasIndex(l => new { l.UsuarioId, l.LibroId })
            .IsUnique();
    }
}
