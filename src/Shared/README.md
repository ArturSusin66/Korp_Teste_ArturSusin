# Shared - Código Compartilhado

## ⚠️ Política de Uso

Este projeto contém código compartilhado entre microsserviços. **Adicione código aqui APENAS se:**

1. ✅ Duplicação causa problema real
2. ✅ Mudança em uma exceção afeta **AMBOS** os serviços
3. ✅ Impossível deixar separado sem quebrar contrato
4. ❌ **NÃO:** DTOs (cada serviço tem seu contrato)
5. ❌ **NÃO:** Entidades (cada serviço tem seu modelo)
6. ❌ **NÃO:** Lógica de negócio (nunca!)

## 📦 Conteúdo Atual

### Exceptions/

**NegocioException.cs** - Exceção base para erros de negócio
- Usar para validações e regras que geram 400 Bad Request
- Exemplo: saldo insuficiente, produto duplicado

**ValidacaoException.cs** - Exceção para validação de dados
- Usar para validação de DTOs
- Retorna 400 Bad Request

**IntegracaoException.cs** - Exceção para falha de integração entre serviços
- Usar quando comunicação entre APIs falha
- Retorna 503 Service Unavailable

## 🚀 Como Usar

### No Estoque.Api

```csharp
using Korp.Shared.Exceptions;

try
{
    var produto = await _repository.ObterAsync(codigo);
    if (produto == null)
        throw new NegocioException($"Produto {codigo} não encontrado");
}
catch (NegocioException ex)
{
    // Mapear para 400 Bad Request
}
```

### No Faturamento.Api

```csharp
using Korp.Shared.Exceptions;

try
{
    await _estoqueService.ReducirSaldoAsync(codigo, quantidade);
}
catch (IntegracaoException ex)
{
    // Mapear para 503 Service Unavailable
    // Não persiste nota como fechada
}
```

## 📝 Versionamento

- **Versão 1.0:** Exceções base
- Futuro: Adicionar DTOs comuns se necessário (com cuidado)

## 🔄 Renovação

A cada Sprint, revisar se há código aqui que poderia ser desacoplado dos serviços sem prejudicar a qualidade.
