namespace Korp.Estoque.Application.Dtos;

/// <summary>

/// </summary>
public class ReducaoSaldoDto
{
    public int Quantidade { get; set; }
    public string MotivoOperacao { get; set; } = string.Empty;
}
