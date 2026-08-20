using Korp.Faturamento.Application.Dtos;
using Korp.Shared.Exceptions;

namespace Korp.Faturamento.Application.Services;

/// <summary>
/// Interface para comunicação com serviço de Estoque
/// Implementa o padrão de integração entre microsserviços
/// </summary>
public interface IEstoqueService
{
    Task<ProdutoEstoqueDto> ObterProdutoAsync(string codigo);
    Task<ProdutoEstoqueDto> ReducirSaldoAsync(string codigo, ReducaoSaldoEstoqueDto dto);
}
