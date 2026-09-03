## ADDED Requirements

### Requirement: Confirmação financeira exige chave idempotente
Toda confirmação de pagamento MUST receber uma chave idempotente válida no escopo do estabelecimento e da operação financeira.

#### Scenario: Repetição com os mesmos dados
- **WHEN** a mesma chave e o mesmo conteúdo forem reapresentados
- **THEN** o sistema SHALL retornar o resultado original sem duplicar o pagamento confirmado

#### Scenario: Repetição com conteúdo diferente
- **WHEN** a mesma chave for reapresentada com valor ou pagamento diferente
- **THEN** o sistema MUST rejeitar a operação por conflito

### Requirement: Cobertura financeira deriva de pagamentos confirmados
Somente pagamentos confirmados SHALL compor o valor pago; registros pendentes, falhos ou cancelados MUST permanecer históricos sem cobrir o pedido.

#### Scenario: Pagamento pendente completa o valor nominal
- **WHEN** a soma nominal incluir pagamento ainda pendente
- **THEN** o pedido MUST continuar financeiramente descoberto pelo valor pendente
