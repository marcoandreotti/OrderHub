## Why

Comunicações com clientes não devem ficar acopladas a fornecedores ou casos de uso individuais. Um gateway central permite enviar mensagens transacionais com consistência e rastreabilidade por diferentes canais.

## What Changes

- Definir gateway para WhatsApp, e-mail e SMS com adapters substituíveis.
- Gerenciar templates, consentimento, preferências e dados de destino.
- Registrar tentativas, resultados, idempotência e políticas de retry.
- Disparar comunicações a partir de eventos confirmados, sem bloquear a transação principal.

## Capabilities

### New Capabilities

- `communications/notification-gateway`: composição, roteamento e acompanhamento de notificações transacionais.

### Modified Capabilities

Nenhuma.

## Impact

Novo módulo de Communications, configurações por Tenant, adapters externos e telas administrativas. Para entrega assíncrona confiável, a implementação deve suceder `add-outbox-pattern`.
