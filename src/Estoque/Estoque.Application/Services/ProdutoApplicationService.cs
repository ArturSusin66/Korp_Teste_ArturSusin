using Korp.Estoque.Application.Dtos;
using Korp.Estoque.Domain.Entities;
using Korp.Estoque.Domain.Repositories;
using Korp.Shared.Exceptions;

namespace Korp.Estoque.Application.Services;

/// <summary>
/// Service de aplicação para operações com produtos
/// Orquestra lógica de negócio com repositório
/// </summary>
public class ProdutoApplicationService
{
    private readonly IProdutoRepository _repository;

    public ProdutoApplicationService(IProdutoRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <summary>
    /// Criar novo produto
    /// </summary>
    public async Task<ProdutoResponseDto> CriarAsync(CriarProdutoDto dto)
    {
        // Validar se já existe produto com este código
        var existe = await _repository.ExisteCodigoAsync(dto.Codigo);
        if (existe)
            throw new NegocioException($"Já existe produto com o código '{dto.Codigo}'.");

        // Criar entidade de domínio
        var produto = new Produto(dto.Codigo, dto.Descricao, dto.Saldo);

        // Persistir
        await _repository.AdicionarAsync(produto);

        // Retornar DTO
        return MapearParaDto(produto);
    }

    /// <summary>
    /// Obter produto por código
    /// </summary>
    public async Task<ProdutoResponseDto> ObterPorCodigoAsync(string codigo)
    {
        var produto = await _repository.ObterPorCodigoAsync(codigo);
        if (produto == null)
            throw new NegocioException($"Produto com código '{codigo}' não encontrado.");

        return MapearParaDto(produto);
    }

    /// <summary>
    /// Listar todos os produtos
    /// </summary>
    public async Task<List<ProdutoResponseDto>> ListarTodosAsync()
    {
        var produtos = await _repository.ListarTodosAsync();
        return produtos.Select(MapearParaDto).ToList();
    }

    /// <summary>
    /// Reduzir saldo de produto
    /// </summary>
    public async Task<ProdutoResponseDto> ReducirSaldoAsync(string codigo, ReducaoSaldoDto dto)
    {
        var produto = await _repository.ObterPorCodigoAsync(codigo);
        if (produto == null)
            throw new NegocioException($"Produto com código '{codigo}' não encontrado.");

        // Chamar método de domínio que valida regra de negócio
        produto.ReducirSaldo(dto.Quantidade);

        // Persistir mudanças
        await _repository.AtualizarAsync(produto);

        return MapearParaDto(produto);
    }

    private static ProdutoResponseDto MapearParaDto(Produto produto)
    {
        return new ProdutoResponseDto
        {
            Id = produto.Id,
            Codigo = produto.Codigo,
            Descricao = produto.Descricao,
            Saldo = produto.Saldo,
            CriadoEm = produto.CriadoEm
        };
    }
}
