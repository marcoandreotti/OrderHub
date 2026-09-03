## ADDED Requirements

### Requirement: Visitante acompanha e cancela somente por referência pública
Operações anônimas sobre pedido SHALL exigir referência opaca válida e o cancelamento MUST respeitar o estado atual e a política de cancelamento do domínio.

#### Scenario: Cancelamento após início do preparo
- **WHEN** o visitante solicitar cancelamento em estado que não admite cancelamento público
- **THEN** o sistema MUST rejeitar a transição e preservar o pedido
