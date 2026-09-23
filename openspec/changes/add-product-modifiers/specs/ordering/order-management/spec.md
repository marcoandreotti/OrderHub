## ADDED Requirements

### Requirement: Pedido valida e preserva modificadores
O domínio MUST validar a composição vigente, calcular seu efeito monetário e preservar nomes, quantidades e preços de cada escolha no snapshot do item.

#### Scenario: Modificador muda após confirmação
- **WHEN** preço ou disponibilidade é alterado no catálogo
- **THEN** o pedido histórico permanece legível com a composição confirmada
