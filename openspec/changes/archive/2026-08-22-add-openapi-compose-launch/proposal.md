# Change: Add OpenAPI and Docker Compose launch URLs

## Why

O ambiente local já inicia PostgreSQL, API e aplicação web pelo Docker Compose, mas a API não publica documentação OpenAPI e o perfil do projeto Compose não abre automaticamente as interfaces úteis ao desenvolvedor.

## What Changes

- Publicar o documento OpenAPI e uma interface Swagger UI para a API em ambiente Development.
- Configurar o projeto `docker-compose` do Visual Studio para abrir a aplicação web quando a composição for iniciada, mantendo a Swagger UI disponível em uma URL conhecida.
- Preservar as portas configuráveis do Compose, usando 8080 para a API e 9000 para a web como valores padrão no perfil de desenvolvimento.
- Verificar por testes que o documento OpenAPI está disponível somente no ambiente esperado e validar a configuração de launch da web.

## Impact

- Capability afetada: `architecture/solution-foundation`.
- Projetos afetados: `OrderHub.Api` e `docker-compose`.
- Não altera regras de domínio, CQRS, persistência ou isolamento Multi-Tenant.
