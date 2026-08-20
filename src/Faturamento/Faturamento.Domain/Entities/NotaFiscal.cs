using Korp.Faturamento.Domain.Enums;
using Korp.Shared.Exceptions;

namespace Korp.Faturamento.Domain.Entities;

/// <summary>
/// Entidade de domínio que representa uma Nota Fiscal
/// </summary>
public class NotaFiscal
{
    public int Id { get; private set; }
    public int Numero { get; private set; }
    public StatusNotaFiscal Status { get; private set; } = StatusNotaFiscal.Aberta;
    public DateTime DataEmissao { get; private set; }
    public DateTime? DataFechamento { get; private set; }
    public decimal Total { get; private set; }

    // Coleção de itens
    private readonly List<ItemNotaFiscal> _itens = new();
    public virtual IReadOnlyList<ItemNotaFiscal> Itens => _itens.AsReadOnly();

    // Construtor privado para EF Core
    private NotaFiscal() { }

    public NotaFiscal(int numero)
    {
        if (numero <= 0)
            throw new NegocioException("Número da nota fiscal deve ser maior que zero.");

        Numero = numero;
        DataEmissao = DateTime.UtcNow;
        Status = StatusNotaFiscal.Aberta;
    }

    /// <summary>
    /// Adicionar item à nota fiscal
    /// Apenas notas abertas podem receber itens
    /// </summary>
    public void AdicionarItem(string codigoProduto, int quantidade, decimal valor)
    {
        if (Status != StatusNotaFiscal.Aberta)
            throw new NegocioException("Não é possível adicionar itens a uma nota fechada.");

        var item = new ItemNotaFiscal(codigoProduto, quantidade, valor);
        _itens.Add(item);
        CalcularTotal();
    }

    /// <summary>
    /// Fechar a nota fiscal
    /// Apenas notas abertas podem ser fechadas
    /// Nota deve ter pelo menos um item
    /// </summary>
    public void Fechar()
    {
        if (Status != StatusNotaFiscal.Aberta)
            throw new NegocioException("Nota fiscal já está fechada.");

        if (_itens.Count == 0)
            throw new NegocioException("Não é possível fechar uma nota sem itens.");

        Status = StatusNotaFiscal.Fechada;
        DataFechamento = DateTime.UtcNow;
    }

    /// <summary>
    /// Validar se nota está aberta para impressão
    /// </summary>
    public bool PodeSerImpresa()
    {
        return Status == StatusNotaFiscal.Aberta && _itens.Count > 0;
    }

    private void CalcularTotal()
    {
        Total = _itens.Sum(item => item.Valor * item.Quantidade);
    }
}
