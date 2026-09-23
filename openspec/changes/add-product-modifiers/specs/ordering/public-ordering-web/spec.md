## ADDED Requirements

### Requirement: Interface conduz composição válida
A aplicação SHALL renderizar grupos, limites, dependências e efeitos de preço retornados pela API e MUST impedir avanço enquanto regras conhecidas não forem satisfeitas.

#### Scenario: Grupo obrigatório incompleto
- **WHEN** o visitante tenta adicionar o produto sem escolhas mínimas
- **THEN** a aplicação destaca o grupo e mantém o produto fora do carrinho
