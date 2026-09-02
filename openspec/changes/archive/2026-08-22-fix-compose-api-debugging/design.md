## Context

O Compose aponta o serviço `api` para `docker/api/Dockerfile`, que publica a aplicação em Release e não possui o estágio `base` esperado pelo Fast Mode. O repositório já contém `src/OrderHub.Api/Dockerfile`, gerado para o Container Tools, mas ele não é usado pela composição. O perfil atual do `.dcproj` define somente o launch da Web e não declara ações por serviço.

## Goals / Non-Goals

**Goals:**

- Tornar explícita a associação entre o serviço `api` e o projeto `OrderHub.Api`.
- Executar a API com Fast Mode e debugger anexado quando o Compose iniciar em Debug pelo Visual Studio.
- Continuar produzindo uma imagem autocontida para execução por Docker Compose CLI.

**Non-Goals:**

- Anexar debugger ao PostgreSQL ou ao Nginx da aplicação web.
- Introduzir hot reload dentro do container.
- Alterar portas, URLs públicas ou comportamento funcional da API.

## Decisions

### Usar o Dockerfile do projeto da API

O serviço `api` referenciará `src/OrderHub.Api/Dockerfile`. Esse arquivo possui os estágios `base`, `build`, `publish` e `final` esperados pelo Container Tools: o estágio `base` suporta Fast Mode em Debug e o estágio `final` preserva a imagem completa usada fora do Visual Studio.

A alternativa de duplicar esses estágios em `docker/api/Dockerfile` foi rejeitada porque manteria dois Dockerfiles semanticamente equivalentes e sujeitos a divergência.

### Declarar ações por serviço no perfil Compose

O projeto Compose terá `Properties/launchSettings.json` com `api` em `StartDebugging` e `postgres`/`web` em `StartWithoutDebugging`. A Web continuará sendo o serviço de launch do navegador. Isso torna a intenção explícita e permite ao Visual Studio gerar os overrides de debugger e montar os binários Debug da API.

A alternativa de depender apenas das propriedades globais do `.dcproj` foi rejeitada porque elas escolhem o serviço/URL do navegador, mas não expressam qual serviço deve receber o debugger.

### Recriar a composição após a correção

A verificação removerá apenas os containers do projeto Compose de desenvolvimento e os recriará com build. O volume PostgreSQL será preservado. Isso evita que a porta 8080 continue servindo a imagem antiga.

## Risks / Trade-offs

- [O cache do Visual Studio pode preservar configuração gerada anteriormente] → Encerrar a sessão de debug e recriar a composição; limpar `obj/Docker` somente se o tooling não regenerar os arquivos.
- [Dois Dockerfiles da API continuam presentes inicialmente] → Após confirmar que nenhuma automação referencia `docker/api/Dockerfile`, remover o duplicado como parte da implementação para manter uma única fonte.
- [Fast Mode depende do Visual Studio Container Tools] → A execução por CLI continuará usando o estágio final e será validada separadamente.

## Migration Plan

1. Atualizar a referência do Dockerfile e o perfil Compose.
2. Validar a configuração combinada e o build da solução.
3. Encerrar a composição antiga sem remover volumes.
4. Iniciar novamente em Debug pelo Visual Studio e validar breakpoint, Swagger e Web.
5. Em rollback, restaurar a referência anterior do Dockerfile e o perfil de launch; os dados PostgreSQL permanecem no volume.
