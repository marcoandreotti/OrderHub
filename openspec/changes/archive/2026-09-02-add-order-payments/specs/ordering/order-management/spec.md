## ADDED Requirements

### Requirement: Estado operacional e cobertura financeira são independentes
O pedido SHALL expor seu status operacional e seu valor financeiramente coberto como informações distintas, sem inferir automaticamente uma transição operacional a partir de pagamento.

#### Scenario: Pagamento integral antes do preparo
- **WHEN** o pedido for integralmente pago enquanto ainda aguarda confirmação operacional
- **THEN** sua cobertura financeira SHALL ser integral e seu status operacional SHALL permanecer inalterado

### Requirement: Valor devido é autoritativo para pagamentos
Operações financeiras MUST usar o total atual preservado pelo pedido e MUST NOT aceitar um valor devido informado pelo cliente como fonte de decisão.

#### Scenario: Valor informado diverge do pedido
- **WHEN** uma confirmação de pagamento usar valor devido diferente do total autoritativo
- **THEN** o sistema MUST avaliar excesso e cobertura a partir do pedido persistido
