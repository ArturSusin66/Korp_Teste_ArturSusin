namespace Korp.Faturamento.Application.Dtos;

/// <summary>
/// DTO para resposta de nota fiscal nas APIs
/// </summary>
public class NotaFiscalResponseDto
{
    public int Id { get; set; }
    public int Numero { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime DataEmissao { get; set; }
    public DateTime? DataFechamento { get; set; }
    public decimal Total { get; set; }
    public List<ItemNotaFiscalResponseDto> Itens { get; set; } = new();
}
