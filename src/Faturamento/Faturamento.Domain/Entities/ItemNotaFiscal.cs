namespace Korp.Faturamento.Domain.Entities;

/// <summary>
/// Entidade que representa um item dentro de uma Nota Fiscal
/// </summary>
public class ItemNotaFiscal
{
    public int Id { get; private set; }
    public int NotaFiscalId { get; private set; }
    public string CodigoProduto { get; private set; } = string.Empty;
    public int Quantidade { get; private set; }
    public decimal Valor { get; private set; }
    public DateTime CriadoEm { get; private set; }

    // Referência para navegação
    public virtual NotaFiscal? NotaFiscal { get; set; }

    // Construtor privado para EF Core
    private ItemNotaFiscal() { }

    public ItemNotaFiscal(string codigoProduto, int quantidade, decimal valor)
    {
        if (string.IsNullOrWhiteSpace(codigoProduto))
            throw new ArgumentException("Código do produto é obrigatório.", nameof(codigoProduto));

        if (quantidade <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero.", nameof(quantidade));

        if (valor <= 0)
            throw new ArgumentException("Valor deve ser maior que zero.", nameof(valor));

        CodigoProduto = codigoProduto.Trim();
        Quantidade = quantidade;
        Valor = valor;
        CriadoEm = DateTime.UtcNow;
    }
}
