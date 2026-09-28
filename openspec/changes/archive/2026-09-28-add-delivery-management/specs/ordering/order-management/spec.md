## ADDED Requirements

### Requirement: Pedido preserva snapshot da entrega contratada
Pedido de entrega confirmado MUST preservar endereço normalizado, região ou regra aplicada, taxa e estimativa usadas no total, independentemente de alterações posteriores.

#### Scenario: Política muda após confirmação
- **WHEN** taxa ou cobertura é alterada
- **THEN** o pedido histórico mantém os valores e dados de entrega confirmados
