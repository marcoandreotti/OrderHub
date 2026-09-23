## ADDED Requirements

### Requirement: Pedido distingue atendimento imediato e agendado
O pedido SHALL indicar se é imediato ou agendado e, no segundo caso, MUST preservar o compromisso temporal validado na confirmação.

#### Scenario: Agendamento ausente
- **WHEN** um pedido declarado agendado não possui slot válido
- **THEN** a confirmação é rejeitada sem reservar número de pedido
