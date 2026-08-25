namespace Korp.Estoque.Application.Dtos;

/// <summary>

/// </summary>
public class ProdutoResponseDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public int Saldo { get; set; }
    public DateTime CriadoEm { get; set; }
}
