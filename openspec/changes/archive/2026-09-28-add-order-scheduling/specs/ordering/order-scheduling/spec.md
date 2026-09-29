## Purpose

Permite comprometer pedidos para horários futuros válidos, respeitando disponibilidade, antecedência, horizonte e capacidade da unidade.

## ADDED Requirements

### Requirement: Slots são calculados autoritativamente
O sistema MUST oferecer slots somente para modalidades habilitadas dentro de antecedência, horizonte, disponibilidade e capacidade configurados.

#### Scenario: Capacidade esgotada
- **WHEN** um slot atinge seu limite antes da confirmação
- **THEN** o sistema rejeita o agendamento e retorna alternativas ainda válidas

### Requirement: Agendamento confirmado é preservado
O pedido confirmado SHALL registrar instante agendado e fuso da unidade e mudanças posteriores MUST ser explícitas e históricas.

#### Scenario: Horário da unidade muda depois da confirmação
- **WHEN** a grade é alterada
- **THEN** o compromisso já confirmado permanece identificável para tratamento operacional
