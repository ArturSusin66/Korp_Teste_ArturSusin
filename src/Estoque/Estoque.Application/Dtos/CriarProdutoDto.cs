namespace Korp.Estoque.Application.Dtos;

/// <summary>

/// </summary>
public class CriarProdutoDto
{
    public string Codigo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public int Saldo { get; set; }
}
