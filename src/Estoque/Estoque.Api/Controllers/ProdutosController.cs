using Korp.Estoque.Application.Dtos;
using Korp.Estoque.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Korp.Estoque.Api.Controllers;

/// <summary>
/// Controller para gerenciamento de produtos
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly ProdutoApplicationService _service;
    private readonly ILogger<ProdutosController> _logger;

    public ProdutosController(ProdutoApplicationService service, ILogger<ProdutosController> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Criar novo produto
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProdutoResponseDto>> Criar([FromBody] CriarProdutoDto dto)
    {
        _logger.LogInformation("Criando novo produto com código: {Codigo}", dto.Codigo);
        var resultado = await _service.CriarAsync(dto);
        return CreatedAtAction(nameof(Obter), new { codigo = resultado.Codigo }, resultado);
    }

    /// <summary>
    /// Obter produto por código
    /// </summary>
    [HttpGet("{codigo}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProdutoResponseDto>> Obter(string codigo)
    {
        _logger.LogInformation("Obtendo produto com código: {Codigo}", codigo);
        var resultado = await _service.ObterPorCodigoAsync(codigo);
        return Ok(resultado);
    }

    /// <summary>
    /// Listar todos os produtos
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ProdutoResponseDto>>> Listar()
    {
        _logger.LogInformation("Listando todos os produtos");
        var resultado = await _service.ListarTodosAsync();
        return Ok(resultado);
    }

    /// <summary>
    /// Reduzir saldo de um produto
    /// </summary>
    [HttpPost("{codigo}/reduzir-saldo")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProdutoResponseDto>> ReducirSaldo(string codigo, [FromBody] ReducaoSaldoDto dto)
    {
        _logger.LogInformation("Reduzindo saldo do produto {Codigo} em {Quantidade} unidades", codigo, dto.Quantidade);
        var resultado = await _service.ReducirSaldoAsync(codigo, dto);
        return Ok(resultado);
    }
}
