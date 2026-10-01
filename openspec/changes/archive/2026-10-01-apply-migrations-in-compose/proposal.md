# Proposal

## Why

O ambiente Docker Compose pode iniciar a API com um PostgreSQL recém-criado ou atualizado sem aplicar as migrations pendentes. Isso causa falhas durante o desenvolvimento e exige que cada pessoa execute manualmente o projeto de migrations.

## What Changes

- Executar o projeto dedicado `OrderHub.Infrastructure.Migrations` como etapa única de inicialização do Compose.
- Fazer a API aguardar a conclusão bem-sucedida das migrations e manter explícita a dependência do PostgreSQL saudável.
- Preservar o fluxo atual de desenvolvimento do Visual Studio e da CLI, incluindo a API em Debug.
- Encerrar a inicialização com erro visível se uma migration falhar, em vez de iniciar a API contra um schema incompatível.

## Capabilities

### New Capabilities

- Nenhuma.

### Modified Capabilities

- `architecture/solution-foundation`: o ambiente local em Compose aplica migrations antes de iniciar a API.

## Impact

- `docker-compose.yml` e possivelmente Dockerfiles/build context para produzir o executável de migrations.
- Projeto `src/OrderHub.Infrastructure.Migrations` existente; sem novo mecanismo de persistência ou execução de migrations na API.
- Dependência de inicialização da API e documentação de execução local, se necessário.
