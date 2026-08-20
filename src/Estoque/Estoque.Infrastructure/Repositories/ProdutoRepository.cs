using Korp.Estoque.Domain.Entities;
using Korp.Estoque.Domain.Repositories;
using Korp.Estoque.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Korp.Estoque.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório de Produto usando EF Core
/// </summary>
public class ProdutoRepository : IProdutoRepository
{
    private readonly EstoqueDbContext _context;

    public ProdutoRepository(EstoqueDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Produto?> ObterPorCodigoAsync(string codigo)
    {
        return await _context.Produtos
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Codigo == codigo);
    }

    public async Task<List<Produto>> ListarTodosAsync()
    {
        return await _context.Produtos
            .AsNoTracking()
            .OrderBy(p => p.Codigo)
            .ToListAsync();
    }

    public async Task AdicionarAsync(Produto produto)
    {
        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Produto produto)
    {
        _context.Produtos.Update(produto);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExisteCodigoAsync(string codigo)
    {
        return await _context.Produtos
            .AnyAsync(p => p.Codigo == codigo);
    }
}
