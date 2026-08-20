# Decisões Técnicas - Sistema KORP

## 📋 Índice
1. [Comunicação entre Microsserviços](#comunicação-entre-microsserviços)
2. [Arquitetura de Camadas](#arquitetura-de-camadas)
3. [Tratamento de Erros](#tratamento-de-erros)
4. [Persistência de Dados](#persistência-de-dados)
5. [Frontend Angular](#frontend-angular)
6. [Testes](#testes)
7. [Segurança](#segurança)

---

## Comunicação entre Microsserviços

### Decisão Adotada: HTTP com HttpClient

**Justificativa:**
- ✅ Simples de implementar e explicar
- ✅ Fácil de simular falhas (mock de timeout)
- ✅ Sem dependência de infraestrutura adicional
- ✅ Padrão RESTful clássico
- ✅ Apropriado para teste técnico

### Contrato de APIs

#### Estoque Service (Porta 5000)

**GET /api/produtos/{codigo}**
- Obter produto e saldo
- Response 200: DTO com código, descrição, saldo
- Response 404: Produto não encontrado

**POST /api/produtos/{codigo}/reduzir-saldo**
- Reduzir saldo de um produto
- Request: { quantidade, motivoOperacao }
- Response 200: novo saldo
- Response 400: Saldo insuficiente
- Response 404: Produto não encontrado

#### Faturamento Service (Porta 5001)

**POST /api/notas-fiscais**
- Criar nota fiscal vazia (status Aberta)
- Response 201: Número gerado

**POST /api/notas-fiscais/{numero}/itens**
- Adicionar item à nota
- Request: { codigoProduto, quantidade, valor }
- Response 200: Item adicionado

**POST /api/notas-fiscais/{numero}/imprimir**
- Imprimir/fechar nota fiscal
- Response 200: Nota fechada (estoque atualizado)
- Response 400: Nota não aberta
- Response 503: Serviço Estoque indisponível

### Padrão de Resiliência

**Retry Policy (Polly):**
- 1ª tentativa falha → espera 2 segundos
- 2ª tentativa falha → espera 2 segundos
- 3ª tentativa falha → lança exceção
- Timeout: 5 segundos por requisição

**Tratamento de Falhas:**
```csharp
try
{
    var resultado = await _estoqueService.ReducirSaldoAsync(...);
}
catch (HttpRequestException ex)
{
    // Log da falha
    // Retorna 503 ao frontend
    // Não persiste mudanças no BD
}
```

---

## Arquitetura de Camadas

### Clean Architecture: 4 Camadas

```
Api Layer
  ↓
Application Layer (Services, DTOs, Validation)
  ↓
Domain Layer (Entities, ValueObjects, Business Rules)
  ↓
Infrastructure Layer (DbContext, Repositories, EF Core)
```

### Responsabilidades

**Domain** (Regras de Negócio Puras)
- Entidades: Produto, NotaFiscal, ItemNotaFiscal
- ValueObjects: StatusNotaFiscal (Enum)
- Exceções de negócio
- Métodos de validação
- Zero dependências externas

**Application** (Orquestração)
- Services: ProdutoApplicationService, NotaFiscalApplicationService
- DTOs: CriarProdutoDto, NotaFiscalResponseDto, etc.
- Validators: FluentValidation
- Mappers: AutoMapper (se necessário)
- Integrações com outros serviços (EstoqueHttpService)

**Infrastructure** (Persistência)
- DbContext: EstoqueDbContext, FaturamentoDbContext
- Repositories: ProdutoRepository, NotaFiscalRepository
- Migrations: EF Core
- Configuração de BD (indices, constraints)

**Api** (Exposição HTTP)
- Controllers: ProdutosController, NotasFiscaisController
- Middleware: ExceptionHandling, Logging
- Configuração de DI
- CORS habilitado

---

## Tratamento de Erros

### Exceções Customizadas

**Shared/Exceptions:**
- `NegocioException` - Erro de regra de negócio (4xx)
- `ValidacaoException` - Validação (400)
- `IntegracaoException` - Falha de comunicação (503)

**Domain/Exceptions:**
- `ProdutoNaoEncontradoException`
- `SaldoInsuficienteException`
- `NotaFiscalFechadaException`
- `NotaFiscalVaziaException`

### Mapeamento HTTP

```csharp
try { ... }
catch (NegocioException ex) => return BadRequest(new { mensagem = ex.Message });
catch (IntegracaoException ex) => return StatusCode(503, new { mensagem = ex.Message });
catch (Exception ex) => return InternalServerError();
```

### Logging

- **Info:** Operações bem-sucedidas (criar produto, emitir nota)
- **Warning:** Tentativas falhadas (saldo insuficiente)
- **Error:** Falhas de integração, exceções inesperadas
- **Debug:** Valores de entrada/saída (development apenas)

---

## Persistência de Dados

### Entity Framework Core

**Duas instâncias de DbContext:**
- `EstoqueDbContext` - gerencia Produtos
- `FaturamentoDbContext` - gerencia NotasFiscal e ItemNotaFiscal

**Migrations:**
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

**Configurações:**
- Connection string via variável de ambiente
- Lazy loading desabilitado (evita problemas de N+1)
- Índices em colunas de busca (Codigo em Produto)
- Constraints: NOT NULL, UNIQUE, FOREIGN KEY

**Transações:**
```csharp
using (var transaction = await _context.Database.BeginTransactionAsync())
{
    try
    {
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
}
```

### LINQ

**Uso apropriado:**
- `Where()` para filtros
- `FirstOrDefault()` / `SingleOrDefault()` para buscas únicas
- `Select()` para projeções (DTO mapping)
- `Include()` para eager loading
- `AsNoTracking()` para read-only

**Exemplo:**
```csharp
var produto = await _context.Produtos
    .AsNoTracking()
    .FirstOrDefaultAsync(p => p.Codigo == codigo);
```

---

## Frontend Angular

### Ciclos de Vida

**OnInit:**
- Carregar dados da API
- Inicializar formulários
- Configurar subscriptions

**OnDestroy:**
- Desinscrever-se de Observables (previne memory leaks)
- Limpar recursos

**ChangeDetectionStrategy:**
- OnPush: atualizar apenas quando @Input muda ou evento dispara
- Default: atualizar a cada mudança (mais simples, menos otimizado)

### RxJS

**Observables para requisições HTTP:**
```typescript
this.faturamentoService.criarNota().pipe(
  tap(nota => console.log('Nota criada', nota)),
  catchError(error => {
    this.errorService.mostrarErro(error);
    return throwError(error);
  })
).subscribe(nota => this.nota = nota);
```

**Subject para comunicação entre componentes:**
```typescript
private notaCriadaSubject = new Subject<NotaFiscal>();
notaCriada$ = this.notaCriadaSubject.asObservable();
```

### Componentes

**Pequenos e focados:**
- ProdutoFormComponent: apenas formulário
- ProdutoListComponent: apenas listagem
- Lógica em serviços (injetáveis)

**Tratamento de Loading e Erros:**
```html
<div *ngIf="loading$ | async; else loaded">
  <app-loading></app-loading>
</div>
<ng-template #loaded>
  <div *ngIf="erro$ | async as erro" class="alert alert-danger">
    {{ erro }}
  </div>
  <form *ngIf="!(erro$ | async)" [formGroup]="form">
    <!-- formulário -->
  </form>
</ng-template>
```

---

## Testes

### Estrutura

```
Estoque.Tests/
├── Unit/
│   ├── Domain/
│   │   └── ProdutoTests.cs
│   └── Application/
│       └── ProdutoServiceTests.cs
├── Integration/
│   └── ProdutoApiTests.cs
└── Setup/
    └── TestDatabaseFixture.cs
```

### Framework & Ferramentas
- **xUnit:** Framework
- **Moq:** Mock de dependências
- **FluentAssertions:** Assertions legíveis
- **TestContainers:** BD isolado para testes (opcional)

### Casos Prioritários

**Domain Tests (Unit):**
- Produto.ReducirSaldo() - reduz corretamente
- Produto.ReducirSaldo() - lança exceção se saldo < 0
- NotaFiscal.Imprimir() - muda status
- NotaFiscal.Imprimir() - lança exceção se fechada

**Application Tests (Unit com Mock):**
- Service cria produto e salva
- Service chama EstoqueService ao imprimir
- Service trata exceção de EstoqueService

**Integration Tests:**
- API retorna 201 ao criar
- API retorna 400 ao erro de validação
- API retorna 503 ao falhar Estoque
- Fluxo completo: criar produto → nota → imprimir → saldo atualizado

---

## Segurança

### Variáveis de Ambiente

**Nunca hardcode:**
- Connection strings
- API keys
- Passwords
- URLs de terceiros

**Usar arquivo `.env`:**
```
ASPNETCORE_ENVIRONMENT=Development
ESTOQUE_DATABASE_CONNECTION=Server=localhost;...
```

**.gitignore:**
```
.env
appsettings.Development.json
.env.local
```

### CORS

**Frontend autorizado:**
```csharp
services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
    {
        builder.WithOrigins("http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});
```

### Validação

**Input validation:**
- Quantidade > 0
- Código/Descrição não vazios
- Valores numéricos válidos

**DTOs com DataAnnotations ou FluentValidation:**
```csharp
public class CriarProdutoDto
{
    [Required]
    [StringLength(50)]
    public string Codigo { get; set; }
}
```

---

## Assincronismo

### Regra: Sempre async/await

**Banco de dados:**
```csharp
var produto = await _context.Produtos.FirstOrDefaultAsync(p => p.Codigo == codigo);
```

**HTTP:**
```csharp
var response = await _httpClient.GetAsync($"{_baseUrl}/api/produtos/{codigo}");
```

**Controllers:**
```csharp
[HttpPost]
public async Task<ActionResult<ProdutoResponseDto>> Criar([FromBody] CriarProdutoDto dto)
{
    var resultado = await _service.CriarAsync(dto);
    return CreatedAtAction(nameof(Obter), new { codigo = resultado.Codigo }, resultado);
}
```

**Nunca usar:**
- `.Result` (causa deadlock)
- `.Wait()` (causa deadlock)
- `.Synchronously` (anti-pattern)

---

## Conclusão

Estas decisões técnicas foram adotadas para:
1. ✅ Atender aos requisitos da KORP
2. ✅ Demonstrar conhecimento de padrões modernos .NET
3. ✅ Manter simplicidade apropriada para teste técnico
4. ✅ Facilitar explicação no vídeo de apresentação
5. ✅ Evitar overengineering

Todas as escolhas priorizam **clareza, manutenibilidade e demonstração de conhecimento técnico**.