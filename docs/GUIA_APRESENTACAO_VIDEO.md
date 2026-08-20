# Guia de Apresentação em Vídeo - KORP

## 🎬 Checklist de Apresentação

### Duração Total Recomendada: 15-20 minutos

---

## PARTE 1: Demonstração de Telas e Funcionalidades (5-7 minutos)

### Tela 1: Cadastro de Produtos
- [ ] Abrir formulário de novo produto
- [ ] Preencher: Código (P001), Descrição (Notebook), Saldo (10)
- [ ] Clicar em "Salvar"
- [ ] Mostrar mensagem de sucesso
- [ ] Exibir produto na listagem com saldo atualizado

**Narração:**
> "Aqui estou criando um novo produto no sistema. O produto recebe um código único, uma descrição e um saldo inicial no estoque. Após salvar, o produto aparece na listagem."

### Tela 2: Listagem de Produtos
- [ ] Mostrar tabela com produtos e saldos
- [ ] Demonstrar que saldo é atualizado em tempo real
- [ ] (Opcional) Mostrar que não é possível criar produto com código duplicado

**Narração:**
> "A listagem exibe todos os produtos cadastrados com seus saldos atualizados. Cada produto tem um código único que será referenciado nas notas fiscais."

### Tela 3: Criar Nota Fiscal
- [ ] Clicar em "Nova Nota Fiscal"
- [ ] Sistema gera número sequencial automaticamente (ex: NF-000001)
- [ ] Status aparece como "Aberta"
- [ ] Exibir formulário para adicionar itens

**Narração:**
> "Ao criar uma nova nota fiscal, o sistema gera automaticamente um número sequencial. A nota inicia com status 'Aberta', permitindo adicionar itens."

### Tela 4: Adicionar Itens à Nota
- [ ] Selecionar produto (dropdown com produtos e saldos)
- [ ] Digitar quantidade (ex: 2 unidades)
- [ ] Validar que quantidade não pode exceder saldo
- [ ] Clicar em "Adicionar Item"
- [ ] Mostrar item adicionado em tabela
- [ ] Adicionar mais um item (para demonstrar múltiplos itens)
- [ ] Exibir total da nota

**Narração:**
> "Adicionamos itens à nota fiscal. O sistema valida se a quantidade solicitada está disponível em estoque. Podemos adicionar múltiplos produtos em uma única nota."

### Tela 5: Botão Imprimir com Loading
- [ ] Clicar em "Imprimir Nota Fiscal"
- [ ] **Mostrar spinner/indicador de processamento**
- [ ] Aguardar conclusão
- [ ] Status da nota muda para "Fechada"
- [ ] Mostrar mensagem de sucesso
- [ ] Botão imprimir desabilitado

**Narração:**
> "Ao clicar em imprimir, o sistema processa a nota fiscal. Você pode ver o indicador de processamento. Após a conclusão, o status muda para 'Fechada' e o estoque é automaticamente atualizado."

### Tela 6: Validação de Saldo Atualizado
- [ ] Ir para listagem de produtos
- [ ] Mostrar que saldo do produto foi reduzido (ex: 10 - 2 = 8)
- [ ] Narrar a atualização

**Narração:**
> "Veja que o saldo do produto foi reduzido de 10 para 8 unidades, conforme a quantidade usada na nota fiscal. O controle de estoque está sincronizado."

### Tela 7: Bloqueio de Impressão (Nota Fechada)
- [ ] Tentar imprimir nota que já está fechada
- [ ] Sistema exibe erro: "Nota não pode ser impressa. Status inválido."
- [ ] Mostrar que botão estava desabilitado (bom UX)

**Narração:**
> "O sistema não permite imprimir uma nota que já foi fechada. Isso garante integridade de dados e evita processamento duplicado."

### Tela 8: Simulação de Falha (Serviço de Estoque Offline) - OPCIONAL
- [ ] Parar o serviço de Estoque (se apresentador tiver acesso ao console)
- [ ] OU documentar como fazer isso (para referência)
- [ ] Tentar criar nova nota e imprimir
- [ ] Sistema exibe: "Serviço de Estoque indisponível. Tente novamente."
- [ ] Nota NÃO é marcada como fechada
- [ ] Reiniciar serviço e tentar novamente (sucesso)

**Narração:**
> "Uma funcionalidade importante é o tratamento de falhas entre microsserviços. Se o serviço de Estoque está indisponível, o sistema fornece feedback claro ao usuário e não persiste dados inconsistentes. Quando o serviço recupera, a operação pode ser retentada com sucesso."

---

## PARTE 2: Detalhamento Técnico (7-10 minutos)

### Seção 1: Arquitetura Geral (1-2 minutos)

**Mostrar diagrama ou descrever:**

```
Frontend (Angular) → Faturamento API (5001) → Estoque API (5000)
                              ↓
                          MySQL BD
```

**Narração:**
> "A aplicação segue uma arquitetura de microsserviços. Temos dois serviços independentes:
> 
> 1. **Serviço de Estoque** (porta 5000): Responsável por gerenciar produtos e saldos
> 2. **Serviço de Faturamento** (porta 5001): Responsável por criar e gerenciar notas fiscais
> 
> O frontend Angular comunica principalmente com o Faturamento, que por sua vez se comunica com Estoque via HTTP.
> 
> A comunicação entre serviços usa HTTP com timeout de 5 segundos e retry automático de até 3 tentativas."

### Seção 2: Ciclos de Vida do Angular (1-2 minutos)

**Mostrar arquivo TypeScript (ex: produto-list.component.ts):**

```typescript
export class ProdutoListComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  ngOnInit(): void {
    this.carregarProdutos();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  carregarProdutos(): void {
    this.produtoService.listar()
      .pipe(takeUntil(this.destroy$))
      .subscribe(produtos => this.produtos = produtos);
  }
}
```

**Narração:**
> "Aqui você vê os ciclos de vida do Angular em ação:
> 
> - **ngOnInit()**: Disparado quando o componente é inicializado. Aqui carregamos os dados da API.
> 
> - **ngOnDestroy()**: Disparado quando o componente é destruído. É importante desinscrever-se dos Observables para evitar vazamento de memória.
> 
> Usamos `takeUntil()` com um Subject para garantir que quando o componente for destruído, todas as subscriptions sejam canceladas automaticamente."

### Seção 3: RxJS e Observables (1-2 minutos)

**Mostrar arquivo TypeScript (ex: faturamento.service.ts):**

```typescript
imprimirNota(numero: number): Observable<NotaFiscal> {
  return this.http.post<NotaFiscal>(
    `${this.apiUrl}/notas-fiscais/${numero}/imprimir`,
    {}
  ).pipe(
    tap(nota => console.log('Nota impressa', nota)),
    catchError(error => {
      this.logger.error('Erro ao imprimir', error);
      return throwError(() => new Error('Falha na impressão'));
    }),
    retry({ count: 3, delay: 2000 })
  );
}
```

**Narração:**
> "Aqui usamos RxJS para gerenciar requisições assíncronas:
> 
> - **tap()**: Intercepta o resultado sem modificá-lo (útil para logging)
> 
> - **catchError()**: Captura erros e trata-os
> 
> - **retry()**: Retenta automaticamente 3 vezes com delay de 2 segundos entre tentativas
> 
> Esse padrão garante que operações transitórias (network glitches) sejam automaticamente recuperadas."

### Seção 4: EF Core e LINQ (1-2 minutos)

**Mostrar arquivo C# (ex: ProdutoRepository.cs):**

```csharp
public async Task<Produto?> ObterPorCodigoAsync(string codigo)
{
    return await _context.Produtos
        .AsNoTracking()
        .FirstOrDefaultAsync(p => p.Codigo == codigo);
}

public async Task ReducirSaldoAsync(string codigo, int quantidade)
{
    var produto = await _context.Produtos
        .FirstOrDefaultAsync(p => p.Codigo == codigo);
    
    if (produto == null)
        throw new ProdutoNaoEncontradoException(codigo);
    
    if (produto.Saldo < quantidade)
        throw new SaldoInsuficienteException(produto.Saldo, quantidade);
    
    produto.ReducirSaldo(quantidade);
    await _context.SaveChangesAsync();
}
```

**Narração:**
> "Aqui você vê Entity Framework Core em ação com LINQ:
> 
> - **FirstOrDefaultAsync()**: Busca o primeiro item que corresponde ao filtro (ou null)
> 
> - **AsNoTracking()**: Otimiza leitura (não rastreia mudanças)
> 
> - **SaveChangesAsync()**: Persiste as mudanças no banco de dados de forma assíncrona
> 
> A validação ocorre tanto no domain (Produto.ReducirSaldo()) quanto na persistência, garantindo integridade."

### Seção 5: Tratamento de Erros e Exceções (1-2 minutos)

**Mostrar arquivo C# (ex: ExceptionHandlingMiddleware.cs):**

```csharp
try
{
    await _next(context);
}
catch (NegocioException ex)
{
    context.Response.StatusCode = StatusCodes.Status400BadRequest;
    await context.Response.WriteAsJsonAsync(new { mensagem = ex.Message });
}
catch (IntegracaoException ex)
{
    context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
    await context.Response.WriteAsJsonAsync(new { mensagem = ex.Message });
}
catch (Exception ex)
{
    _logger.LogError(ex, "Erro não tratado");
    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
}
```

**Narração:**
> "O tratamento de erros é centralizado em um middleware. Diferentes tipos de exceção mapeiam para diferentes status HTTP:
> 
> - **NegocioException** → 400 Bad Request (validação de regra de negócio)
> - **IntegracaoException** → 503 Service Unavailable (falha de comunicação)
> - **Exception genérica** → 500 Internal Server Error
> 
> Todos os erros são loggados para debug posterior. O frontend recebe mensagens amigáveis em vez de stack traces."

### Seção 6: Testes (1-2 minutos)

**Mostrar diretório de testes e um exemplo:**

```csharp
[Fact]
public async Task Imprimir_ComSaldoSuficiente_AtualizaEstoque()
{
    // Arrange
    var produto = new Produto("P001", "Notebook", 10);
    var nota = new NotaFiscal();
    nota.AdicionarItem("P001", 2, 3000);
    
    // Act
    var resultado = await _service.ImprimirAsync(nota.Numero);
    
    // Assert
    resultado.Status.Should().Be(StatusNotaFiscal.Fechada);
    var produtoAtualizado = await _repository.ObterPorCodigoAsync("P001");
    produtoAtualizado.Saldo.Should().Be(8);
}
```

**Narração:**
> "Temos testes unitários que validam o comportamento crítico do sistema:
> 
> - Produto com saldo insuficiente não permite impressão
> - Saldo é corretamente atualizado
> - Nota só pode ser impressa se status for Aberta
> - Falha de Estoque não marca nota como fechada
> 
> Isso garante que mudanças futuras não quebrem funcionalidades essenciais."

### Seção 7: Comunicação entre Microsserviços e Resiliência (1 minuto)

**Mostrar arquivo C# (ex: EstoqueHttpService.cs):**

```csharp
public class EstoqueHttpService : IEstoqueService
{
    private readonly HttpClient _httpClient;
    private readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy;

    public EstoqueHttpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.Timeout = TimeSpan.FromSeconds(5);
        
        _retryPolicy = Policy
            .Handle<HttpRequestException>()
            .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: _ => TimeSpan.FromMilliseconds(2000)
            );
    }

    public async Task<ProdutoDto> ReducirSaldoAsync(string codigo, int quantidade)
    {
        var response = await _retryPolicy.ExecuteAsync(
            () => _httpClient.PostAsync(
                $"{_baseUrl}/api/produtos/{codigo}/reduzir-saldo",
                ...)
        );
        
        if (!response.IsSuccessStatusCode)
            throw new IntegracaoException("Falha ao atualizar estoque");
        
        return await response.Content.ReadAsAsync<ProdutoDto>();
    }
}
```

**Narração:**
> "A comunicação entre serviços usa HttpClient com:
> 
> - **Timeout de 5 segundos**: Evita que a requisição fique pendente indefinidamente
> 
> - **Retry Policy (Polly)**: Retenta automaticamente 3 vezes com 2 segundos de delay
> 
> - **Tratamento de erro**: Se mesmo após 3 tentativas falhar, lança IntegracaoException
> 
> Isso torna o sistema robusto contra falhas transitórias de rede."

---

## PARTE 3: Resumo Executivo (1 minuto)

**Narração Final:**
> "Este sistema demonstra boas práticas modernas de desenvolvimento:
> 
> ✅ **Arquitetura:** Microsserviços desacoplados com comunicação clara
> ✅ **Backend:** Clean Architecture, SOLID, async/await
> ✅ **Frontend:** Componentes pequenos, RxJS, ciclos de vida bem utilizados
> ✅ **Resiliência:** Retry, timeout, tratamento de falhas
> ✅ **Testes:** Cobertura de cenários críticos
> ✅ **Dados:** Persistência real em MySQL com EF Core
> 
> O sistema está pronto para produção em escala pequena e pode ser facilmente expandido para adicionar features como autenticação, autorização e mais domínios de negócio."

---

## 📋 Checklist Técnico Final

Antes de gravar o vídeo:

- [ ] APIs compilam sem erros
- [ ] Frontend compila sem warnings
- [ ] Banco de dados migrations aplicadas
- [ ] Todas as telas funcionam conforme esperado
- [ ] Testes passam
- [ ] Nenhuma senha ou secret visível no código
- [ ] Arquivos de configuração corretos
- [ ] URLs e portas estão corretas
- [ ] Narração clara e técnica (evitar termos vagos)
- [ ] Vídeo com boa iluminação e áudio claro
- [ ] Duração entre 15-20 minutos

---

## 🎥 Dicas de Gravação

1. **Gire o vídeo em 1080p ou maior**
2. **Aumente o tamanho da fonte no IDE** (mínimo 14pt)
3. **Use fundo simples** para melhor foco
4. **Fale devagar e claramente**
5. **Pause entre seções** para respirar
6. **Mostre o código atual** quando mencionar implementação
7. **Demonstre antes, explique depois** (técnica storytelling)
8. **Tenha um script**, mas não leia como robô
9. **Teste a gravação antes** (áudio, resolução, iluminação)
10. **Hospede em Google Drive ou YouTube** com acesso público

---

## 📧 Entrega Final

Email para `rh@korp.com.br` deve conter:

1. **Link do repositório GitHub** (público)
2. **Link do vídeo de apresentação**
3. **Resumo das tecnologias utilizadas**
4. **Instruções de como executar o projeto localmente**
5. **Confirmar que todos os requisitos foram atendidos**

---

**Boa sorte com sua apresentação!** 🚀