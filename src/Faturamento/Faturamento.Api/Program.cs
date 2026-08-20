using Korp.Faturamento.Application.Services;
using Korp.Faturamento.Domain.Repositories;
using Korp.Faturamento.Infrastructure.Data;
using Korp.Faturamento.Infrastructure.Repositories;
using Korp.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.AspNetCore.Builder;


var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog para logging
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .MinimumLevel.Information()
        .WriteTo.Console()
        .WriteTo.File("logs/faturamento-.txt", rollingInterval: RollingInterval.Day);
});

// Configurar DbContext
builder.Services.AddDbContext<FaturamentoDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("FaturamentoDatabase")
        ?? throw new InvalidOperationException("Connection string 'FaturamentoDatabase' not found.");
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

// Registrar serviços de aplicação
builder.Services.AddScoped<NotaFiscalApplicationService>();
builder.Services.AddScoped<INotaFiscalRepository, NotaFiscalRepository>();

// Registrar HttpClient para EstoqueService
var estoqueBaseUrl = builder.Configuration["Urls:EstoqueApi"]
    ?? throw new InvalidOperationException("Estoque API URL not configured.");

builder.Services
    .AddHttpClient<IEstoqueService, EstoqueHttpService>(client =>
    {
        client.BaseAddress = new Uri(estoqueBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(5);
    });

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Executar migrations automaticamente
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FaturamentoDbContext>();
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthorization();

// Middleware de tratamento de exceções
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
        context.Response.ContentType = "application/json";

        var response = new { mensagem = "Erro desconhecido" };

        if (exception is NegocioException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            response = new { mensagem = exception.Message };
        }
        else if (exception is ValidacaoException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            response = new { mensagem = exception.Message };
        }
        else if (exception is IntegracaoException)
        {
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            response = new { mensagem = exception.Message };
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            app.Logger.LogError(exception, "Erro não tratado");
        }

        await context.Response.WriteAsJsonAsync(response);
    });
});

app.MapControllers();

app.Run();