using Korp.Faturamento.Application.Dtos;
using Korp.Faturamento.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Korp.Faturamento.Api.Controllers;

/// <summary>
/// Controller para gerenciamento de notas fiscais
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class NotasFiscaisController : ControllerBase
{
    private readonly NotaFiscalApplicationService _service;
    private readonly ILogger<NotasFiscaisController> _logger;

    public NotasFiscaisController(NotaFiscalApplicationService service, ILogger<NotasFiscaisController> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Criar nova nota fiscal
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<NotaFiscalResponseDto>> Criar([FromBody] CriarNotaFiscalDto dto)
    {
        _logger.LogInformation("Criando nova nota fiscal");
        var resultado = await _service.CriarAsync(dto);
        return CreatedAtAction(nameof(Obter), new { numero = resultado.Numero }, resultado);
    }

    /// <summary>
    /// Obter nota fiscal por número
    /// </summary>
    [HttpGet("{numero}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NotaFiscalResponseDto>> Obter(int numero)
    {
        _logger.LogInformation("Obtendo nota fiscal número {Numero}", numero);
        var resultado = await _service.ObterPorNumeroAsync(numero);
        return Ok(resultado);
    }

    /// <summary>
    /// Listar todas as notas fiscais
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<NotaFiscalResponseDto>>> Listar()
    {
        _logger.LogInformation("Listando todas as notas fiscais");
        var resultado = await _service.ListarTodosAsync();
        return Ok(resultado);
    }

    /// <summary>
    /// Adicionar item à nota fiscal
    /// </summary>
    [HttpPost("{numero}/itens")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NotaFiscalResponseDto>> AdicionarItem(int numero, [FromBody] AdicionarItemNotaFiscalDto dto)
    {
        _logger.LogInformation(
            "Adicionando item {CodigoProduto} à nota {Numero}",
            dto.CodigoProduto, numero);
        var resultado = await _service.AdicionarItemAsync(numero, dto);
        return Ok(resultado);
    }

    /// <summary>
    /// Imprimir (fechar) nota fiscal
    /// Atualiza saldo dos produtos no estoque
    /// </summary>
    [HttpPost("{numero}/imprimir")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<NotaFiscalResponseDto>> Imprimir(int numero)
    {
        _logger.LogInformation("Imprimindo nota fiscal número {Numero}", numero);
        var resultado = await _service.ImprimirAsync(numero);
        return Ok(resultado);
    }
}
