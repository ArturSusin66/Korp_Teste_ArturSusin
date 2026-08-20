namespace Korp.Faturamento.Application.Dtos;

/// <summary>
/// DTO para consultar produto no serviço de Estoque
/// </summary>
public class ProdutoEstoqueDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public int Saldo { get; set; }
}
