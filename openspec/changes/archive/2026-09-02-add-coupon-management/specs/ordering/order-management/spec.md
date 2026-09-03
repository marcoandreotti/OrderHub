## ADDED Requirements

### Requirement: Desconto de cupom integra o total autoritativo
O pedido SHALL calcular o desconto elegível sobre seu subtotal aplicável, preservar código e valor concedido e MUST impedir que o total final seja negativo.

#### Scenario: Cupom fixo maior que o valor elegível
- **WHEN** o desconto fixo superar o valor do pedido ao qual pode ser aplicado
- **THEN** o desconto SHALL ser limitado ao valor elegível e o total final SHALL ser zero

### Requirement: Remoção de cupom recalcula o pedido não confirmado
Um cupom MAY ser removido antes da confirmação e o pedido SHALL recalcular seus totais sem preservar consumo.

#### Scenario: Cupom removido durante composição
- **WHEN** o cliente remover o cupom antes de confirmar o pedido
- **THEN** o sistema SHALL restaurar o total sem desconto e MUST NOT contabilizar uso
