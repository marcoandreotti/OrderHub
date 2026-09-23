## ADDED Requirements

### Requirement: Operações administrativas relevantes geram auditoria consistente
A API SHALL acionar auditoria fora dos Controllers e MUST correlacionar resultado com a operação, inclusive para falhas previstas, sem registrar payload sensível integral.

#### Scenario: Alteração concluída
- **WHEN** uma operação auditável é persistida
- **THEN** o evento identifica o alvo e o resultado confirmado da alteração
