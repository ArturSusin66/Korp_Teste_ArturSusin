namespace Korp.Faturamento.Application.Dtos;

/// <summary>
/// DTO para resposta de item de nota fiscal
/// </summary>
public class ItemNotaFiscalResponseDto
{
    public int Id { get; set; }
    public string CodigoProduto { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal Valor { get; set; }
}
