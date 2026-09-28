## ADDED Requirements

### Requirement: Fluxo público cota e confirma entrega autoritativamente
A API SHALL cotar endereço e MUST recalcular sua elegibilidade e taxa ao confirmar, rejeitando cota expirada ou incompatível.

#### Scenario: Taxa muda entre cotação e confirmação
- **WHEN** a política vigente produz valor diferente
- **THEN** a API não confirma silenciosamente e retorna o total autoritativo atualizado
