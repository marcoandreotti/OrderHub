## ADDED Requirements

### Requirement: Checkout apresenta cobertura e custo da entrega
A aplicação SHALL coletar endereço suficiente, apresentar taxa e estimativa retornadas pelo servidor e exigir aceite de qualquer alteração antes da confirmação.

#### Scenario: Endereço não atendido
- **WHEN** a cotação indicar ausência de cobertura
- **THEN** a aplicação impede entrega e oferece somente modalidades disponíveis
