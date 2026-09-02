## ADDED Requirements

### Requirement: Operação administrativa consulta e transiciona pedidos
A API SHALL fornecer listagem e detalhe por período, status, número e tipo de atendimento e SHALL expor somente transições autorizadas pelo papel e pelo domínio.

#### Scenario: Cozinha inicia preparação
- **WHEN** um usuário autorizado da cozinha solicitar preparação de pedido em estado compatível
- **THEN** o sistema SHALL aplicar a transição e registrar o ator no histórico
