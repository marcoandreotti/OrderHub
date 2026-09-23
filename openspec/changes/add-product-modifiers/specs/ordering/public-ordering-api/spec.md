## ADDED Requirements

### Requirement: API expõe regras e não aceita preço de modificador do cliente
A API SHALL fornecer os grupos necessários à composição e MUST recalcular escolhas e preços no servidor ao alterar ou confirmar o carrinho.

#### Scenario: Cliente envia preço adulterado
- **WHEN** o valor informado diverge da regra vigente
- **THEN** o sistema ignora o valor do cliente e usa o cálculo autoritativo
