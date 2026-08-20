using Korp.Faturamento.Application.Dtos;
using Korp.Shared.Exceptions;
using Polly;
using System.Net.Http.Json;

namespace Korp.Faturamento.Application.Services;

/// <summary>
/// Implementação de IEstoqueService via HTTP
/// Comunica com Estoque.Api usando HttpClient com retry policy
/// </summary>
public class EstoqueHttpService : IEstoqueService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<EstoqueHttpService> _logger;
    private readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy;

    public EstoqueHttpService(HttpClient httpClient, ILogger<EstoqueHttpService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClient.Timeout = TimeSpan.FromSeconds(5);

        // Configurar política de retry com Polly
        _retryPolicy = Policy
            .Handle<HttpRequestException>()
            .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(2000),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    _logger.LogWarning(
                        "Tentativa {RetryCount} de chamar Estoque após {DelayMs}ms",
                        retryCount, timespan.TotalMilliseconds);
                }
            );
    }

    public async Task<ProdutoEstoqueDto> ObterProdutoAsync(string codigo)
    {
        try
        {
            _logger.LogInformation("Obtendo produto {Codigo} do serviço Estoque", codigo);

            var response = await _retryPolicy.ExecuteAsync(
                () => _httpClient.GetAsync($"/api/produtos/{codigo}")
            );

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "Erro ao obter produto {Codigo}. Status: {StatusCode}",
                    codigo, response.StatusCode);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new NegocioException($"Produto {codigo} não encontrado no estoque.");

                throw new IntegracaoException(
                    $"Falha ao consultar estoque. Status: {response.StatusCode}");
            }

            var produto = await response.Content.ReadAsAsync<ProdutoEstoqueDto>();
            _logger.LogInformation("Produto {Codigo} obtido com sucesso. Saldo: {Saldo}", codigo, produto.Saldo);
            return produto;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Erro de comunicação ao obter produto {Codigo}", codigo);
            throw new IntegracaoException(
                "Serviço de Estoque indisponível. Tente novamente.", ex);
        }
    }

    public async Task<ProdutoEstoqueDto> ReducirSaldoAsync(string codigo, ReducaoSaldoEstoqueDto dto)
    {
        try
        {
            _logger.LogInformation(
                "Reduzindo saldo do produto {Codigo} em {Quantidade} unidades",
                codigo, dto.Quantidade);

            var response = await _retryPolicy.ExecuteAsync(
                () => _httpClient.PostAsJsonAsync(
                    $"/api/produtos/{codigo}/reduzir-saldo",
                    dto)
            );

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    "Erro ao reduzir saldo. Status: {StatusCode}, Content: {Content}",
                    response.StatusCode, content);

                // Possíveis erros:
                // 400: Saldo insuficiente ou produto não encontrado
                // 404: Produto não encontrado
                // 503: Serviço indisponível

                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    throw new NegocioException($"Saldo insuficiente para produto {codigo}.");

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new NegocioException($"Produto {codigo} não encontrado.");

                throw new IntegracaoException(
                    "Serviço de Estoque indisponível. Tente novamente.");
            }

            var produto = await response.Content.ReadAsAsync<ProdutoEstoqueDto>();
            _logger.LogInformation(
                "Saldo reduzido com sucesso. Novo saldo: {NovoSaldo}",
                produto.Saldo);
            return produto;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Erro de comunicação ao reduzir saldo do produto {Codigo}", codigo);
            throw new IntegracaoException(
                "Serviço de Estoque indisponível. Tente novamente.", ex);
        }
    }
}
