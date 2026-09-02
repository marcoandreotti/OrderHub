## Purpose

Define cupons por unidade, suas janelas e limites de uso e o desconto histórico aplicado ao pedido.

## ADDED Requirements

### Requirement: Cupom possui código único e validade
Cada cupom SHALL pertencer a uma unidade, possuir código normalizado único nessa unidade, tipo percentual ou valor fixo, valor válido e período com início anterior ao fim.

#### Scenario: Código repetido
- **WHEN** outro cupom usar o mesmo código normalizado na unidade
- **THEN** o sistema SHALL rejeitar o cadastro por conflito

### Requirement: Aplicação respeita elegibilidade
Um cupom MUST estar ativo, dentro da janela, abaixo do limite de usos quando definido e atender ao valor mínimo do pedido; seu desconto MUST NOT tornar o total negativo.

#### Scenario: Limite alcançado
- **WHEN** a quantidade máxima já tiver sido utilizada
- **THEN** o sistema MUST rejeitar uma nova aplicação

### Requirement: Pedido preserva snapshot do cupom
Ao aplicar um cupom, o pedido SHALL preservar código e valor de desconto efetivamente concedido independentemente de alterações posteriores no cupom.

#### Scenario: Cupom editado posteriormente
- **WHEN** descrição, valor ou estado do cupom mudar após aplicação
- **THEN** o pedido histórico SHALL manter código e desconto originalmente aplicados

