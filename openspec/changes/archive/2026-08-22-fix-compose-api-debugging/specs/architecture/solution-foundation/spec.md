## MODIFIED Requirements

### Requirement: Abertura da aplicação web pelo projeto Docker Compose
O projeto Docker Compose do Visual Studio SHALL iniciar todos os serviços locais, anexar o debugger à API no perfil Debug e abrir a aplicação web no navegador, mantendo a documentação da API disponível em uma URL conhecida.

#### Scenario: Inicialização pelo Visual Studio
- **WHEN** um desenvolvedor iniciar o projeto `docker-compose` pelo perfil Debug
- **THEN** PostgreSQL, API e aplicação web SHALL iniciar
- **AND** o debugger SHALL ser anexado ao processo da API executando os binários Debug atuais
- **AND** o navegador SHALL abrir a aplicação web em `http://localhost:9000`
- **AND** a Swagger UI SHALL estar disponível em `http://localhost:8080/swagger` para acesso manual

#### Scenario: Alteração da API durante o desenvolvimento
- **WHEN** o código da API for alterado e o perfil Debug do projeto Compose for reiniciado
- **THEN** o container da API SHALL executar a versão atualizada dos binários Debug sem depender de uma imagem Release publicada anteriormente

#### Scenario: Inicialização pela CLI
- **WHEN** um desenvolvedor executar `docker compose up --build` pela linha de comando
- **THEN** os serviços SHALL iniciar com imagens atualizadas sem depender do Visual Studio ou da abertura de um navegador

