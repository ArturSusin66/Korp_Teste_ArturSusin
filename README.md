# Korp - Sistema de Emissão de Notas Fiscais

## 📋 Objetivo
Desenvolver uma aplicação completa de emissão de notas fiscais com arquitetura de microsserviços, demonstrando boas práticas em C#/.NET, Angular e integração entre serviços.

## 🏗️ Arquitetura
- **Backend:** ASP.NET Core (C#) - Microsserviços
  - Serviço de Estoque (porta 5000)
  - Serviço de Faturamento (porta 5001)
- **Frontend:** Angular (porta 4200)
- **Banco de Dados:** MySQL
- **Padrão:** Clean Architecture com separação de camadas

## 📁 Estrutura do Projeto

```
Korp_Teste_ArturSusin/
├── README.md
├── .gitignore
├── docs/
│   ├── KORP_Decisoes_Tecnicas.md
│   └── GUIA_APRESENTACAO_VIDEO.md
├── src/
│   ├── Shared/
│   │   ├── Exceptions/
│   │   └── README.md
│   ├── Estoque/
│   │   ├── Estoque.Api/
│   │   ├── Estoque.Application/
│   │   ├── Estoque.Domain/
│   │   ├── Estoque.Infrastructure/
│   │   └── Estoque.Tests/
│   ├── Faturamento/
│   │   ├── Faturamento.Api/
│   │   ├── Faturamento.Application/
│   │   ├── Faturamento.Domain/
│   │   ├── Faturamento.Infrastructure/
│   │   └── Faturamento.Tests/
│   └── frontend/
│       ├── src/
│       ├── e2e/
│       └── package.json
└── docker-compose.yml
```

## 🚀 Como Executar

### Pré-requisitos
- .NET 8 SDK ou superior
- Node.js 18+ e npm
- MySQL 8+
- Visual Studio Code ou Visual Studio 2022

### Variáveis de Ambiente

Crie um arquivo `.env` na raiz do projeto ou configure em `appsettings.Development.json`:

```
ASPNETCORE_ENVIRONMENT=Development
ESTOQUE_DATABASE_CONNECTION=Server=localhost;Database=EstoqueDb;User=root;Password=sua_senha;
FATURAMENTO_DATABASE_CONNECTION=Server=localhost;Database=FaturamentoDb;User=root;Password=sua_senha;
ESTOQUE_BASE_URL=http://localhost:5000
FATURAMENTO_BASE_URL=http://localhost:5001
```

### Iniciar Serviços

**Terminal 1 - Serviço de Estoque:**
```bash
cd src/Estoque/Estoque.Api
dotnet restore
dotnet run
# Disponível em http://localhost:5000
```

**Terminal 2 - Serviço de Faturamento:**
```bash
cd src/Faturamento/Faturamento.Api
dotnet restore
dotnet run
# Disponível em http://localhost:5001
```

**Terminal 3 - Frontend Angular:**
```bash
cd src/frontend
npm install
ng serve
# Disponível em http://localhost:4200
```

## 🧪 Executar Testes

```bash
# Testes do Estoque
cd src/Estoque/Estoque.Tests
dotnet test

# Testes de Faturamento
cd src/Faturamento/Faturamento.Tests
dotnet test
```

## 📡 APIs

### Serviço de Estoque (Porta 5000)

**Criar Produto:**
```
POST /api/produtos
Content-Type: application/json

{
  "codigo": "P001",
  "descricao": "Notebook Dell",
  "saldo": 10
}
```

**Obter Produto:**
```
GET /api/produtos/{codigo}
```

**Reduzir Saldo:**
```
POST /api/produtos/{codigo}/reduzir-saldo
Content-Type: application/json

{
  "quantidade": 2,
  "motivoOperacao": "Emissão Nota Fiscal NF-000001"
}
```

### Serviço de Faturamento (Porta 5001)

**Criar Nota Fiscal:**
```
POST /api/notas-fiscais
```

**Adicionar Item à Nota:**
```
POST /api/notas-fiscais/{numero}/itens
Content-Type: application/json

{
  "codigoProduto": "P001",
  "quantidade": 2,
  "valor": 3000.00
}
```

**Imprimir Nota Fiscal:**
```
POST /api/notas-fiscais/{numero}/imprimir
```

## 🔧 Tecnologias Utilizadas

### Backend
- **ASP.NET Core 8** - Framework web
- **Entity Framework Core** - ORM
- **Polly** - Tratamento de resiliência
- **xUnit** - Framework de testes
- **Moq** - Mock de dependências
- **FluentAssertions** - Assertions legíveis

### Frontend
- **Angular 18+** - Framework web
- **RxJS** - Programação reativa
- **TypeScript** - Linguagem tipada
- **Angular Material** - Componentes visuais (opcional)

### Banco de Dados
- **MySQL 8+** - Persistência de dados

## 📚 Documentação

- **[Decisões Técnicas](docs/KORP_Decisoes_Tecnicas.md)** - Detalhamento das escolhas arquiteturais
- **[Guia de Apresentação](docs/GUIA_APRESENTACAO_VIDEO.md)** - Checklist para vídeo de apresentação

## 🎯 Requisitos Funcionais

✅ Cadastro de Produtos (código, descrição, saldo)
✅ Cadastro de Notas Fiscais com numeração sequencial
✅ Status de Nota (Aberta/Fechada)
✅ Múltiplos itens em uma nota
✅ Impressão de nota com atualização de estoque
✅ Bloqueio de impressão para notas não abertas
✅ Tratamento de falhas entre microsserviços
✅ Persistência real em banco de dados
✅ Recuperação e feedback adequado ao usuário

## ⚙️ Requisitos Não-Funcionais

✅ Microsserviços independentes
✅ Clean Architecture
✅ Tratamento robusto de erros
✅ Logging adequado
✅ Testes unitários e integração
✅ Segurança (sem hardcoding de secrets)
✅ Assincronismo (async/await)

## 📝 Sprint Atual

**Sprint 0.1:** Estrutura Base e Configuração
- Repositório criado
- Estrutura de pastas
- Configuração de ambientes
- Documentação inicial

## 📧 Autor

**Artur Susin** - Candidato ao Teste Técnico KORP

## 📄 Licença

Este projeto é fornecido para fins de avaliação técnica.