# Solution Foundation Specification Delta

## ADDED Requirements

### Requirement: Documentação OpenAPI no desenvolvimento local
A API SHALL publicar uma descrição OpenAPI e uma interface Swagger UI durante a execução em ambiente Development, sem expor essas interfaces automaticamente nos demais ambientes.

#### Scenario: Consulta da documentação em Development
- **WHEN** a API iniciar com o ambiente Development
- **THEN** o documento OpenAPI SHALL estar acessível por HTTP e a Swagger UI SHALL permitir explorar os endpoints documentados

#### Scenario: Execução fora de Development
- **WHEN** a API iniciar em um ambiente diferente de Development
- **THEN** o documento OpenAPI e a Swagger UI MUST NOT ser publicados

### Requirement: Abertura da aplicação web pelo projeto Docker Compose
O projeto Docker Compose do Visual Studio SHALL abrir a aplicação web no navegador quando a composição for iniciada pelo perfil de depuração, mantendo a documentação da API disponível em uma URL conhecida.

#### Scenario: Inicialização pelo Visual Studio
- **WHEN** um desenvolvedor iniciar o projeto `docker-compose` pelo perfil de depuração
- **THEN** o navegador SHALL abrir a aplicação web em `http://localhost:9000`
- **AND** a Swagger UI SHALL estar disponível em `http://localhost:8080/swagger` para acesso manual

#### Scenario: Inicialização pela CLI
- **WHEN** um desenvolvedor executar `docker compose up` pela linha de comando
- **THEN** os serviços SHALL iniciar sem depender da abertura de um navegador
