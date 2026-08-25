using Microsoft.EntityFrameworkCore;
using Serilog;

using Korp.Faturamento.Infrastructure.Data;
using Korp.Faturamento.Domain.Repositories;
using Korp.Faturamento.Infrastructure.Repositories;
using Korp.Faturamento.Application.Services;
using Korp.Shared.Exceptions;

var builder = WebApplication.CreateBuilder(args);


builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .MinimumLevel.Information()
        .WriteTo.Console()
        .WriteTo.File("logs/faturamento-.txt", rollingInterval: RollingInterval.Day);
});

// Configurar DbContext 
var connectionString = builder.Configuration.GetConnectionString("FaturamentoDatabase")
    ?? throw new InvalidOperationException("Connection string 'FaturamentoDatabase' não encontrada.");

builder.Services.AddDbContext<FaturamentoDbContext>(options =>
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 0, 30)),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null
        )
    ));

// Injeção de Dependências
builder.Services.AddScoped<INotaFiscalRepository, NotaFiscalRepository>();
builder.Services.AddScoped<NotaFiscalApplicationService>();

// Configuração do Client HTTP para comunicação síncrona com o Estoque
var estoqueBaseUrl = builder.Configuration["Urls:EstoqueApi"] ?? "http://localhost:5001";

builder.Services
    .AddHttpClient<IEstoqueService, EstoqueHttpService>(client =>
    {
        client.BaseAddress = new Uri(estoqueBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(5);
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Registro do serviço de aplicação
builder.Services.AddScoped<NotaFiscalApplicationService>();

// Configuração do HttpClient para comunicação síncrona com Estoque com timeout curto
builder.Services.AddHttpClient("EstoqueClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Urls:EstoqueApi"] ?? "http://localhost:5001");
    client.Timeout = TimeSpan.FromSeconds(5); // Timeout curto explícito conforme edital
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Aplicação de Migrations no Startup
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<FaturamentoDbContext>();
        dbContext.Database.Migrate();
        logger.LogInformation("Conexão com MySQL de Faturamento estabelecida com sucesso.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Erro ao aplicar migrations ou conectar ao MySQL no Faturamento.");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthorization();

// Middleware Global de Erros de Negócio e Integração
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
        context.Response.ContentType = "application/json";

        var response = new { mensagem = "Erro interno no servidor." };

        if (exception is NegocioException || exception is ValidacaoException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            response = new { mensagem = exception.Message };
        }
        else if (exception is IntegracaoException)
        {
            context.Response.StatusCode = StatusCodes.Status502BadGateway;
            response = new { mensagem = "Falha de integração com o serviço de Estoque. A nota permanece Aberta." };
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            app.Logger.LogError(exception, "Erro não tratado no Faturamento API");
        }

        await context.Response.WriteAsJsonAsync(response);
    });
});



app.MapControllers();

app.Run();