# Design: OpenAPI and Docker Compose launch URLs

## Context

A API ASP.NET Core ainda não registra serviços OpenAPI nem middleware de Swagger. O projeto `docker-compose.dcproj` referencia um override de desenvolvimento, mas esse arquivo ainda não está presente. A composição publica a API na porta 8080 e a web na porta 9000 por padrão.

## Goals / Non-Goals

### Goals

- Disponibilizar OpenAPI e Swagger UI em Development.
- Abrir a web automaticamente ao depurar o projeto Compose no Visual Studio e manter uma URL estável para acesso manual ao Swagger.
- Manter `docker compose up` independente de recursos de desktop.

### Non-Goals

- Publicar Swagger em produção.
- Adicionar autenticação específica à documentação.
- Alterar endpoints, contratos HTTP ou o frontend.

## Decisions

### OpenAPI no composition root

O registro e o pipeline OpenAPI serão configurados em `OrderHub.Api`, que já é o composition root HTTP. Será usada a integração suportada pelo ecossistema ASP.NET Core e uma UI Swagger compatível com o documento gerado.

### Exposição condicionada ao ambiente

O mapeamento do documento e da UI será condicionado a `app.Environment.IsDevelopment()`. Isso evita ampliar a superfície pública dos demais ambientes por padrão.

### Launch do Visual Studio via projeto Compose

O `docker-compose.dcproj` usará as propriedades oficiais `DockerLaunchAction`, `DockerServiceName` e `DockerServiceUrl` para abrir a raiz do serviço `web`. O tooling do Visual Studio admite apenas um serviço e uma URL de launch por perfil, portanto a Swagger UI não será aberta automaticamente e permanecerá disponível em `http://localhost:8080/swagger`.

A URL de launch usará a porta publicada do serviço `web`, cuja porta padrão declarada no Compose é 9000. Personalizações por variáveis de ambiente continuam válidas para a execução dos serviços, e o token de porta do tooling acompanha a porta efetivamente publicada.

## Risks / Trade-offs

- O Visual Studio abrirá somente a Web; abrir também o Swagger exigiria outro perfil ou um launcher externo, deliberadamente fora do escopo.
- Mudanças futuras no tooling Docker do Visual Studio podem exigir ajuste das propriedades de launch.

## Verification

- Build completo da solução.
- Testes da API validando disponibilidade em Development e ausência fora dele.
- Validação da configuração combinada com `docker compose config`.
