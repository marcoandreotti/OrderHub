## Why

Ao iniciar `docker-compose` como projeto principal no Visual Studio, os serviços sobem, mas o debugger não é anexado à API e o container pode continuar executando uma imagem publicada anteriormente. Isso impede breakpoints e fez a API ativa responder 404 para o Swagger recém-adicionado.

## What Changes

- Associar o serviço Compose `api` ao projeto `OrderHub.Api` e ao Dockerfile compatível com o Fast Mode do Visual Studio.
- Garantir que o perfil Debug do projeto Compose inicie a API com o debugger anexado e binários Debug atuais.
- Preservar a execução reproduzível por `docker compose` fora do Visual Studio.
- Verificar que, após recriação da composição, o Swagger da API atual responde em `http://localhost:8080/swagger`.

## Capabilities

### New Capabilities

Nenhuma.

### Modified Capabilities

- `architecture/solution-foundation`: exigir que o perfil Debug do Docker Compose permita depurar a API atual no Visual Studio, além de iniciar os serviços e abrir a aplicação web.

## Impact

- Arquivos afetados: `docker-compose.yml`, `docker-compose.dcproj`, configuração do projeto `OrderHub.Api` e Dockerfile da API.
- Ferramentas afetadas: Visual Studio Container Tools e Docker Compose.
- Não altera contratos HTTP, regras de domínio, persistência, CQRS ou isolamento Multi-Tenant.
