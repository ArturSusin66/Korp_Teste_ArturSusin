using Korp.Estoque.Application.Services;
using Korp.Estoque.Domain.Repositories;
using Korp.Estoque.Infrastructure.Data;
using Korp.Estoque.Infrastructure.Repositories; 
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar Conexão com MySQL
var connectionString = builder.Configuration.GetConnectionString("EstoqueDatabase");

builder.Services.AddDbContext<EstoqueDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// 2. Injeção de Dependências das Camadas da Aplicação
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<ProdutoApplicationService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 3. Middlewares e Rotas
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();