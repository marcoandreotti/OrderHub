## ADDED Requirements

### Requirement: Gestão mantém formas e consulta pagamentos
A API SHALL permitir manutenção de formas de pagamento por gestão e consulta dos pagamentos do pedido por papéis operacionais autorizados.

#### Scenario: Forma desativada com histórico
- **WHEN** uma forma for desativada administrativamente
- **THEN** ela MUST deixar de aparecer em novas cobranças e SHALL permanecer nos pagamentos históricos
