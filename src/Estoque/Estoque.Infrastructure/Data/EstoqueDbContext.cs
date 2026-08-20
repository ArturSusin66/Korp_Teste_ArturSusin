using Korp.Estoque.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Korp.Estoque.Infrastructure.Data;

/// <summary>
/// DbContext para o serviço de Estoque
/// Gerencia persistência de Produtos
/// </summary>
public class EstoqueDbContext : DbContext
{
    public EstoqueDbContext(DbContextOptions<EstoqueDbContext> options) : base(options) { }

    public DbSet<Produto> Produtos { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurar entidade Produto
        modelBuilder.Entity<Produto>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Codigo)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(p => p.Descricao)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(p => p.Saldo)
                .IsRequired();

            entity.Property(p => p.CriadoEm)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(p => p.AtualizadoEm);

            // Índice único no código
            entity.HasIndex(p => p.Codigo)
                .IsUnique();
        });
    }
}
