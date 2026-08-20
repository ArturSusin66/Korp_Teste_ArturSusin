namespace Korp.Estoque.Application.Dtos;

/// <summary>
/// DTO para criação de novo produto
/// </summary>
public class CriarProdutoDto
{
    public string Codigo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public int Saldo { get; set; }
}
