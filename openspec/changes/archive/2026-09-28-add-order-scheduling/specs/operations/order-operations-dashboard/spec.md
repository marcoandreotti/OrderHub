## ADDED Requirements

### Requirement: Painel separa demanda futura da demanda executável
O painel SHALL identificar pedidos agendados, ordenar pelo momento prometido e evitar que pedidos distantes ocultem trabalho imediato.

#### Scenario: Pedido aproxima-se da janela de produção
- **WHEN** o horário operacional configurado é alcançado
- **THEN** o pedido passa a receber destaque na fila sem mudança automática de status
