using Korp.Faturamento.Domain.Entities;
using Korp.Faturamento.Domain.Repositories;
using Korp.Faturamento.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Korp.Faturamento.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório de NotaFiscal usando EF Core
/// </summary>
public class NotaFiscalRepository : INotaFiscalRepository
{
    private readonly FaturamentoDbContext _context;

    public NotaFiscalRepository(FaturamentoDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<NotaFiscal?> ObterPorNumeroAsync(int numero)
    {
        return await _context.NotasFiscais
            .Include(n => n.Itens)
            .FirstOrDefaultAsync(n => n.Numero == numero);
    }

    public async Task<List<NotaFiscal>> ListarTodosAsync()
    {
        return await _context.NotasFiscais
            .Include(n => n.Itens)
            .OrderByDescending(n => n.DataEmissao)
            .ToListAsync();
    }

    public async Task AdicionarAsync(NotaFiscal notaFiscal)
    {
        _context.NotasFiscais.Add(notaFiscal);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(NotaFiscal notaFiscal)
    {
        _context.NotasFiscais.Update(notaFiscal);
        await _context.SaveChangesAsync();
    }

    public async Task<int> ObterProximoNumeroAsync()
    {
        var ultimaNota = await _context.NotasFiscais
            .OrderByDescending(n => n.Numero)
            .FirstOrDefaultAsync();

        return (ultimaNota?.Numero ?? 0) + 1;
    }
}
