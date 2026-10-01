# Proposal

## Why

O gateway de notificações e a outbox já oferecem envio rastreável, mas hoje o cliente depende de abrir o acompanhamento público para perceber confirmação e andamento do pedido. Ligar os eventos do ciclo de vida do pedido a esse gateway fecha a jornada iniciada no checkout sem tornar a entrega de e-mail parte da transação do pedido.

## What Changes

- Solicitar notificações transacionais por e-mail após a confirmação pública e em transições operacionais relevantes do pedido.
- Reutilizar templates, consentimentos, idempotência, histórico, SMTP e processamento existente da outbox.
- Garantir que pedido e evento de notificação sejam registrados atomicamente e que falhas de envio não revertam nem bloqueiem a operação do pedido.
- Incluir referência segura para acompanhamento público, sem expor identificadores internos.
- Manter WhatsApp fora da automação inicial; o gateway continuará aberto a outros canais.

## Capabilities

### New Capabilities

- `communications/customer-order-notifications`: define eventos do pedido que geram comunicações transacionais ao cliente e as garantias dessa integração.

### Modified Capabilities

- Nenhuma. A capacidade existente `communications/notification-gateway` já define transporte, consentimento, idempotência e histórico reutilizados por esta nova integração.

## Impact

- Application/Domain: transições do pedido e confirmação pública, usando os contratos de outbox existentes.
- Infrastructure: criação de notificações a partir dos eventos de pedido e resolução de dados necessários ao template.
- Configuração administrativa existente de templates e consentimentos; entrega inicial por SMTP, testável com Mailpit.
- Sem novo provedor, serviço externo ou fluxo de pagamento.
