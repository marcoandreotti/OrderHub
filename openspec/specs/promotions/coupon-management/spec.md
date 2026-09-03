# Coupon Management Specification

## Purpose

Define cupons por unidade, suas janelas e limites de uso e o desconto histórico aplicado ao pedido.

## Requirements

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

### Requirement: Consumo de cupom é concorrente e consistente
A verificação do limite e o registro de uso do cupom MUST ocorrer como um único efeito transacional associado ao pedido.

#### Scenario: Último uso disputado
- **WHEN** duas confirmações concorrerem pelo último uso disponível do cupom
- **THEN** no máximo uma SHALL consumir o benefício e a outra MUST ser rejeitada

### Requirement: Código de cupom é normalizado antes da comparação
O sistema SHALL ignorar diferenças não significativas definidas pela normalização e MUST avaliar o código somente no estabelecimento do pedido.

#### Scenario: Código com caixa diferente
- **WHEN** o cliente informar um código equivalente com diferença de maiúsculas e minúsculas
- **THEN** o sistema SHALL localizar o mesmo cupom normalizado no estabelecimento

### Requirement: Validação pública de cupom não reserva uso
A API SHALL informar o desconto atualmente elegível para uma composição, mas MUST revalidar e consumir o cupom somente na confirmação do pedido.

#### Scenario: Limite esgota após validação
- **WHEN** o limite do cupom for consumido entre a validação e a confirmação
- **THEN** a confirmação MUST rejeitar o cupom sem usar o resultado anterior como autorização

### Requirement: Gestão administra cupons da unidade autorizada
A API SHALL permitir criar, consultar, alterar, ativar e desativar cupons somente a atores com capacidade de gestão no estabelecimento.

#### Scenario: Código duplicado
- **WHEN** a gestão cadastrar código normalizado já existente na unidade
- **THEN** a API SHALL retornar conflito padronizado sem alterar o cupom existente
