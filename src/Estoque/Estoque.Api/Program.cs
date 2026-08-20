using Microsoft.AspNetCore.Builder;
using Korp.Estoque.Application.Services;
using Korp.Estoque.Domain.Repositories;
using Korp.Estoque.Infrastructure.Data;
using Korp.Estoque.Infrastructure.Repositories;
using Korp.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplicationBuilder.CreateBuilder(args);

// Configurar Serilog para logging
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .MinimumLevel.Information()
        .WriteTo.Console()
        .WriteTo.File("logs/estoque-.txt", rollingInterval: RollingInterval.Day);
});

// Configurar serviços
builder.Services.AddDbContext<EstoqueDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("EstoqueDatabase")
        ?? throw new InvalidOperationException("Connection string 'EstoqueDatabase' not found.");
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

// Registrar serviços de aplicação
builder.Services.AddScoped<ProdutoApplicationService>();

// Registrar repositórios
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();

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
    var dbContext = scope.ServiceProvider.GetRequiredService<EstoqueDbContext>();
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
