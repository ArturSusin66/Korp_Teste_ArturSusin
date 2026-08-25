using Korp.Estoque.Application.Services;
using Korp.Estoque.Domain.Repositories;
using Korp.Estoque.Infrastructure.Data;
using Korp.Estoque.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configurar Conexão com MySQL
var connectionString = builder.Configuration.GetConnectionString("EstoqueDatabase");

builder.Services.AddDbContext<EstoqueDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null
        )
    ));

// Injeção de Dependências
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<ProdutoApplicationService>();

//  Configurar CORS (Liberando o Angular)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 4. Configuração do Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// UseRouting/UseCors deve ficar antes dos endpoints/controllers
app.UseRouting();

// Aplica a política de CORS
app.UseCors("AllowAngular");

app.UseAuthorization();

app.MapControllers();

// Aplicação Segura de Migrations no Startup
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<EstoqueDbContext>();
        dbContext.Database.Migrate();
        logger.LogInformation("Conexão com MySQL estabelecida e Migrations aplicadas.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Erro ao conectar no MySQL ou aplicar migrations.");
    }
}

app.Run();