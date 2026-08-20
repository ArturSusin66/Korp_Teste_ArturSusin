namespace Korp.Faturamento.Application.Dtos;

/// <summary>
/// DTO para reduzir saldo no serviço de Estoque
/// </summary>
public class ReducaoSaldoEstoqueDto
{
    public int Quantidade { get; set; }
    public string MotivoOperacao { get; set; } = string.Empty;
}
