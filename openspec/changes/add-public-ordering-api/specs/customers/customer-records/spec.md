## ADDED Requirements

### Requirement: Fluxo público mantém cliente no estabelecimento resolvido
A API pública SHALL localizar ou registrar cliente e endereço somente dentro do estabelecimento resolvido e MUST NOT permitir consulta geral de clientes por contato.

#### Scenario: Telefone existe em outra unidade
- **WHEN** um visitante informar telefone registrado apenas em outro estabelecimento
- **THEN** a API MUST tratar os dados de forma independente sem revelar o registro externo
