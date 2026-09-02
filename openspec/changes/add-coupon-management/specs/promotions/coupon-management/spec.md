## ADDED Requirements

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
