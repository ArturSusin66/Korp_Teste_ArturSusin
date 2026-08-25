# Korp_Teste_ArturSusin

## Sobre o Projeto

O objetivo do projeto é demonstrar conhecimentos em desenvolvimento de software, arquitetura, boas práticas de programação, integração entre serviços e construção de interfaces, conforme os requisitos definidos no teste.

## Funcionalidades

- Cadastro e gerenciamento de dados
- Integração entre camadas da aplicação
- Validação de regras de negócio
- Tratamento de erros e exceções
- Persistência de dados
- Interface para interação com o usuário

## Tecnologias Utilizadas

> Ajuste esta seção conforme as tecnologias efetivamente utilizadas no projeto.

- C# / .NET
- ASP.NET Core
- Entity Framework Core
- Angular
- TypeScript
- SQL Server / SQLite
- Docker

## Estrutura do Projeto

```text
Korp_Teste_ArturSusin/
├── frontend/
├── backend/
├── services/
├── database/
├── docker-compose.yml
└── README.md
```

## Como Executar o Projeto

### Pré-requisitos

- Git
- Docker e Docker Compose

ou

- .NET SDK
- Node.js
- Banco de dados configurado

### Clonando o Repositório

```bash
git clone https://github.com/ArturSusin66/Korp_Teste_ArturSusin.git
cd Korp_Teste_ArturSusin
```

### Executando com Docker

```bash
docker-compose up --build
```

### Executando Manualmente

Backend:

```bash
cd backend
dotnet restore
dotnet run
```

Frontend:

```bash
cd frontend
npm install
npm start
```

## Arquitetura

A aplicação foi desenvolvida seguindo princípios de separação de responsabilidades, permitindo maior manutenibilidade, escalabilidade e facilidade de testes.

Principais componentes:

- Frontend responsável pela experiência do usuário
- API responsável pelas regras de negócio
- Camada de persistência de dados
- Serviços auxiliares e integrações

## Melhorias Futuras

- Testes automatizados
- Pipeline CI/CD
- Observabilidade e monitoramento
- Autenticação e autorização
- Cobertura ampliada de cenários de negócio

## Autor

**Artur Susin**

GitHub: https://github.com/ArturSusin66

---


