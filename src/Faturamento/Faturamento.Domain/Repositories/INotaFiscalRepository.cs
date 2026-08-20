using Korp.Faturamento.Domain.Entities;

namespace Korp.Faturamento.Domain.Repositories;

/// <summary>
/// Interface de repositório para NotaFiscal
/// </summary>
public interface INotaFiscalRepository
{
    Task<NotaFiscal?> ObterPorNumeroAsync(int numero);
    Task<List<NotaFiscal>> ListarTodosAsync();
    Task AdicionarAsync(NotaFiscal notaFiscal);
    Task AtualizarAsync(NotaFiscal notaFiscal);
    Task<int> ObterProximoNumeroAsync();
}
