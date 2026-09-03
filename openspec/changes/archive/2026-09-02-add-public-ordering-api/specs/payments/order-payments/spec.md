## ADDED Requirements

### Requirement: API pública oferece somente formas ativas
A composição pública SHALL listar somente formas de pagamento ativas do estabelecimento e MUST validar novamente a forma ao confirmar o pedido.

#### Scenario: Forma desativada antes da confirmação
- **WHEN** uma forma selecionada for desativada antes da confirmação
- **THEN** a API MUST rejeitar a seleção e solicitar uma forma disponível
