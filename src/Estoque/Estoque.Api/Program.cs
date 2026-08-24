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

// 2. Injeção de Dependências
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<ProdutoApplicationService>();

// 3. Configurar CORS para o Angular
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

// --- CRIAÇÃO DO APP (BUILD) ---
var app = builder.Build();

// 4. Middlewares do Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting(); // Habilita o roteamento do ASP.NET Core
app.UseCors("AllowAngular");
app.UseAuthorization();
app.MapControllers();
app.Run();