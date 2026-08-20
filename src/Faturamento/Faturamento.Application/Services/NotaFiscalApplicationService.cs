using Korp.Faturamento.Application.Dtos;
using Korp.Faturamento.Domain.Entities;
using Korp.Faturamento.Domain.Repositories;
using Korp.Shared.Exceptions;

namespace Korp.Faturamento.Application.Services;

/// <summary>
/// Service de aplicação para operações com notas fiscais
/// Orquestra lógica de negócio com repositório e integração com Estoque
/// </summary>
public class NotaFiscalApplicationService
{
    private readonly INotaFiscalRepository _repository;
    private readonly IEstoqueService _estoqueService;
    private readonly ILogger<NotaFiscalApplicationService> _logger;

    public NotaFiscalApplicationService(
        INotaFiscalRepository repository,
        IEstoqueService estoqueService,
        ILogger<NotaFiscalApplicationService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _estoqueService = estoqueService ?? throw new ArgumentNullException(nameof(estoqueService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Criar nova nota fiscal com número sequencial
    /// </summary>
    public async Task<NotaFiscalResponseDto> CriarAsync(CriarNotaFiscalDto dto)
    {
        _logger.LogInformation("Criando nova nota fiscal");

        var proximoNumero = await _repository.ObterProximoNumeroAsync();
        var notaFiscal = new NotaFiscal(proximoNumero);

        await _repository.AdicionarAsync(notaFiscal);

        return MapearParaDto(notaFiscal);
    }

    /// <summary>
    /// Obter nota fiscal por número
    /// </summary>
    public async Task<NotaFiscalResponseDto> ObterPorNumeroAsync(int numero)
    {
        _logger.LogInformation("Obtendo nota fiscal número {Numero}", numero);

        var notaFiscal = await _repository.ObterPorNumeroAsync(numero);
        if (notaFiscal == null)
            throw new NegocioException($"Nota fiscal número {numero} não encontrada.");

        return MapearParaDto(notaFiscal);
    }

    /// <summary>
    /// Listar todas as notas fiscais
    /// </summary>
    public async Task<List<NotaFiscalResponseDto>> ListarTodosAsync()
    {
        _logger.LogInformation("Listando todas as notas fiscais");
        var notas = await _repository.ListarTodosAsync();
        return notas.Select(MapearParaDto).ToList();
    }

    /// <summary>
    /// Adicionar item à nota fiscal
    /// Valida saldo do produto antes de adicionar
    /// </summary>
    public async Task<NotaFiscalResponseDto> AdicionarItemAsync(int numero, AdicionarItemNotaFiscalDto dto)
    {
        _logger.LogInformation(
            "Adicionando item {CodigoProduto} (quantidade {Quantidade}) à nota {Numero}",
            dto.CodigoProduto, dto.Quantidade, numero);

        var notaFiscal = await _repository.ObterPorNumeroAsync(numero);
        if (notaFiscal == null)
            throw new NegocioException($"Nota fiscal número {numero} não encontrada.");

        // Validar saldo do produto no serviço de Estoque
        var produto = await _estoqueService.ObterProdutoAsync(dto.CodigoProduto);
        if (produto.Saldo < dto.Quantidade)
            throw new NegocioException(
                $"Saldo insuficiente. Disponível: {produto.Saldo}, Solicitado: {dto.Quantidade}");

        // Adicionar item à nota
        notaFiscal.AdicionarItem(dto.CodigoProduto, dto.Quantidade, dto.Valor);

        // Persistir
        await _repository.AtualizarAsync(notaFiscal);

        return MapearParaDto(notaFiscal);
    }

    /// <summary>
    /// Imprimir (fechar) nota fiscal
    /// Atualiza saldo dos produtos no serviço de Estoque
    /// Se falhar na atualização de estoque, desfaz a operação
    /// </summary>
    public async Task<NotaFiscalResponseDto> ImprimirAsync(int numero)
    {
        _logger.LogInformation("Iniciando impressão da nota fiscal {Numero}", numero);

        var notaFiscal = await _repository.ObterPorNumeroAsync(numero);
        if (notaFiscal == null)
            throw new NegocioException($"Nota fiscal número {numero} não encontrada.");

        // Validar se pode ser impressa
        if (!notaFiscal.PodeSerImpresa())
            throw new NegocioException(
                "Nota fiscal não pode ser impressa. Verifique se está aberta e tem itens.");

        // Atualizar saldo de cada item no Estoque
        try
        {
            foreach (var item in notaFiscal.Itens)
            {
                _logger.LogInformation(
                    "Reduzindo estoque: {CodigoProduto} - {Quantidade} unidades",
                    item.CodigoProduto, item.Quantidade);

                var reduziDto = new ReducaoSaldoEstoqueDto
                {
                    Quantidade = item.Quantidade,
                    MotivoOperacao = $"Emissão Nota Fiscal NF-{numero:D6}"
                };

                await _estoqueService.ReducirSaldoAsync(item.CodigoProduto, reduziDto);
            }

            // Todos os itens foram processados com sucesso
            // Agora pode fechar a nota
            notaFiscal.Fechar();
            await _repository.AtualizarAsync(notaFiscal);

            _logger.LogInformation("Nota fiscal {Numero} impressa com sucesso", numero);
            return MapearParaDto(notaFiscal);
        }
        catch (IntegracaoException ex)
        {
            _logger.LogError(ex, "Falha ao atualizar estoque para nota {Numero}", numero);
            // NÃO fecha a nota se falhar na integração
            throw;
        }
    }

    private static NotaFiscalResponseDto MapearParaDto(NotaFiscal notaFiscal)
    {
        return new NotaFiscalResponseDto
        {
            Id = notaFiscal.Id,
            Numero = notaFiscal.Numero,
            Status = notaFiscal.Status.ToString(),
            DataEmissao = notaFiscal.DataEmissao,
            DataFechamento = notaFiscal.DataFechamento,
            Total = notaFiscal.Total,
            Itens = notaFiscal.Itens.Select(item => new ItemNotaFiscalResponseDto
            {
                Id = item.Id,
                CodigoProduto = item.CodigoProduto,
                Quantidade = item.Quantidade,
                Valor = item.Valor
            }).ToList()
        };
    }
}
