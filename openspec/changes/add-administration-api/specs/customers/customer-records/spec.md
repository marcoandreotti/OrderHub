## ADDED Requirements

### Requirement: Administração pesquisa clientes da unidade autorizada
A API SHALL permitir pesquisa paginada por nome, telefone ou e-mail normalizados somente no estabelecimento autorizado e SHALL permitir manutenção de seus endereços.

#### Scenario: Pesquisa por telefone
- **WHEN** um ator autorizado pesquisar telefone na unidade selecionada
- **THEN** somente clientes daquela unidade SHALL compor o resultado
