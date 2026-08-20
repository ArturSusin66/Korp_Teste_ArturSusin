# Frontend - KORP Sistema de Notas Fiscais

Frontend Angular para o sistema de emissão de notas fiscais.

## Instalação

```bash
cd src/frontend
npm install
```

## Desenvolvimento

```bash
ng serve
# ou
npm start
```

Acesse `http://localhost:4200`

## Build

```bash
ng build --configuration production
```

## Arquitetura

### Estrutura de Componentes

```
src/app/
├── app.component.ts          # Root component
├── app.config.ts             # Configuração da aplicação
├── app.routes.ts             # Rotas
├── shared/
│   ├── models/               # Interfaces e tipos
│   │   ├── produto.model.ts
│   │   └── nota-fiscal.model.ts
│   └── services/             # Serviços compartilhados
│       ├── estoque.service.ts
│       ├── faturamento.service.ts
│       └── error.service.ts
└── features/
    ├── produtos/
    │   └── produtos.component.ts
    └── notas-fiscais/
        └── notas-fiscais.component.ts
```

## Tecnologias

- **Angular 18+** - Framework
- **RxJS** - Programação reativa
- **Reactive Forms** - Gerenciamento de formulários
- **TypeScript** - Linguagem tipada

## Recursos

✅ Componentes standalone (sem NgModule)
✅ Rotas lazy-loaded
✅ Gerenciamento de estado com RxJS
✅ Tratamento de erros centralizado
✅ Comunicação HTTP com APIs
✅ Ciclos de vida bem implementados (OnInit, OnDestroy)
