using Korp.Faturamento.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Korp.Faturamento.Infrastructure.Data;

/// <summary>
/// DbContext para o serviço de Faturamento
/// Gerencia persistência de NotasFiscais e Itens
/// </summary>
public class FaturamentoDbContext : DbContext
{
    public FaturamentoDbContext(DbContextOptions<FaturamentoDbContext> options) : base(options) { }

    public DbSet<NotaFiscal> NotasFiscais { get; set; } = null!;
    public DbSet<ItemNotaFiscal> ItensNotaFiscal { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurar entidade NotaFiscal
        modelBuilder.Entity<NotaFiscal>(entity =>
        {
            entity.HasKey(n => n.Id);

            entity.Property(n => n.Numero)
                .IsRequired();

            entity.Property(n => n.Status)
                .IsRequired();

            entity.Property(n => n.DataEmissao)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(n => n.DataFechamento);

            entity.Property(n => n.Total)
                .IsRequired()
                .HasPrecision(18, 2);

            // Índice único no número
            entity.HasIndex(n => n.Numero)
                .IsUnique();

            // Relacionamento 1-N com ItemNotaFiscal
            entity.HasMany(n => n.Itens)
                .WithOne(i => i.NotaFiscal)
                .HasForeignKey(i => i.NotaFiscalId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configurar entidade ItemNotaFiscal
        modelBuilder.Entity<ItemNotaFiscal>(entity =>
        {
            entity.HasKey(i => i.Id);

            entity.Property(i => i.CodigoProduto)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(i => i.Quantidade)
                .IsRequired();

            entity.Property(i => i.Valor)
                .IsRequired()
                .HasPrecision(18, 2);

            entity.Property(i => i.CriadoEm)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
}
