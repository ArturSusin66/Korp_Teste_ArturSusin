namespace Korp.Estoque.Application.Dtos;

/// <summary>
/// DTO para redução de saldo de produto
/// </summary>
public class ReducaoSaldoDto
{
    public int Quantidade { get; set; }
    public string MotivoOperacao { get; set; } = string.Empty;
}
