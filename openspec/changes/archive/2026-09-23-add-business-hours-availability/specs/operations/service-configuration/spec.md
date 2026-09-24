## ADDED Requirements

### Requirement: Disponibilidade combina agenda, exceções e pausas
A unidade SHALL configurar exceções por data e pausas temporárias por modalidade, e a decisão vigente MUST prevalecer sobre a grade semanal de forma determinística.

#### Scenario: Pausa durante horário regular
- **WHEN** uma modalidade é pausada manualmente durante intervalo aberto
- **THEN** novos pedidos dessa modalidade são recusados até o fim ou cancelamento da pausa
