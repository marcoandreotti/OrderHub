                    ┌──────────────────┐
                    │   Vue / Quasar   │
                    └────────┬─────────┘
                             │
                           HTTP
                             │
                    ┌────────▼─────────┐
                    │       API        │
                    │   Controllers    │
                    └────────┬─────────┘
                             │
             ┌───────────────┴───────────────┐
             │                               │
      CommandDispatcher               QueryDispatcher
             │                               │
       FluentValidation                FluentValidation
             │                               │
      CommandHandler                   QueryHandler
             │                               │
         Domain                         Read Gateway
             │                               │
       Write Gateway                       Dapper
             │                               │
        EF Core                         PostgreSQL
             │
        PostgreSQL


regra de dependência:

      Domain
      ↑
      Application
      ↑
      Infrastructure
      ↑
      API

observação importante:

      Domain não conhece ninguém.

      Application conhece Domain.

      Infrastructure implementa portas definidas pelas camadas internas.

      API é composition root e conecta tudo.

