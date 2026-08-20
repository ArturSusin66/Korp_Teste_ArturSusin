namespace Korp.Faturamento.Application.Dtos;

/// <summary>
/// DTO para adicionar item a nota fiscal
/// </summary>
public class AdicionarItemNotaFiscalDto
{
    public string CodigoProduto { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal Valor { get; set; }
}
