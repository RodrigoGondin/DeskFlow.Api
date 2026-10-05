using DeskFlow.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeskFlow.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Chamado> Chamados { get; set; }
    public DbSet<Interacao> Interacoes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        
        modelBuilder.Entity<Chamado>()
            .Property(c => c.Prioridade)
            .HasConversion<string>();

        modelBuilder.Entity<Chamado>()
            .Property(c => c.Status)
            .HasConversion<string>();

        modelBuilder.Entity<Chamado>()
            .HasOne(c => c.Categoria)
            .WithMany(cat => cat.Chamados)
            .HasForeignKey(c => c.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
