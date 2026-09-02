## ADDED Requirements

### Requirement: Validação pública de cupom não reserva uso
A API SHALL informar o desconto atualmente elegível para uma composição, mas MUST revalidar e consumir o cupom somente na confirmação do pedido.

#### Scenario: Limite esgota após validação
- **WHEN** o limite do cupom for consumido entre a validação e a confirmação
- **THEN** a confirmação MUST rejeitar o cupom sem usar o resultado anterior como autorização
