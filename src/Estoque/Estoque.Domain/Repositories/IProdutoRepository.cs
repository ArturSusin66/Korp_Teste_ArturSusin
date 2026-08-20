using Korp.Estoque.Domain.Entities;

namespace Korp.Estoque.Domain.Repositories;

/// <summary>
/// Interface de repositório para Produto
/// Define contrato de acesso a dados
/// </summary>
public interface IProdutoRepository
{
    Task<Produto?> ObterPorCodigoAsync(string codigo);
    Task<List<Produto>> ListarTodosAsync();
    Task AdicionarAsync(Produto produto);
    Task AtualizarAsync(Produto produto);
    Task<bool> ExisteCodigoAsync(string codigo);
}
