## 1. Compose e associação da API

- [x] 1.1 Alterar o serviço `api` para usar `src/OrderHub.Api/Dockerfile` e verificar com `docker compose config` que o caminho e o contexto resolvidos estão corretos.
- [x] 1.2 Remover o Dockerfile duplicado `docker/api/Dockerfile` após confirmar que nenhuma configuração restante o referencia.
- [x] 1.3 Associar `OrderHub.Api.csproj` ao `docker-compose.dcproj` e verificar que o MSBuild resolve ambos os projetos sem erros.

## 2. Perfil de depuração do Visual Studio

- [x] 2.1 Criar o perfil Compose com `api` em `StartDebugging`, `postgres` e `web` em `StartWithoutDebugging`, preservando a Web como URL de launch, e validar o JSON e os nomes dos serviços.
- [x] 2.2 Gerar/iniciar a configuração Debug do projeto Compose e verificar que o container da API recebe o comando, volumes e metadados de depuração do Visual Studio.

## 3. Verificação do ambiente

- [x] 3.1 Encerrar somente a composição antiga sem remover volumes e recriar os serviços com build, verificando que todos ficam healthy.
- [x] 3.2 Verificar respostas HTTP 200 para a Web, `/openapi/v1.json` e `/swagger/index.html` na composição recriada.
- [x] 3.3 Executar build e testes da solução, verificando zero erros, zero warnings relevantes e todos os testes aprovados.
