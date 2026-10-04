## Why

A tela administrativa de notificações reúne configuração de modelos, registro de consentimento, solicitação de envio e histórico, mas não explica como essas partes se relacionam. A densidade atual dificulta entender quando uma mensagem será enviada e o que o estado retornado pelo provedor significa.

## What Changes

- Explicar o fluxo de modelo, consentimento e solicitação de envio em linguagem operacional.
- Reorganizar a tela em seções responsivas, mantendo as operações existentes e seus contratos.
- Explicar finalidades e campos aceitos por modelos de pedidos públicos sem exigir que o usuário descubra os identificadores técnicos por tentativa e erro.
- Distinguir consentimento concedido e revogado, e esclarecer que solicitação enfileirada ou aceita pelo provedor não confirma entrega final.

## Capabilities

### New Capabilities

Nenhuma.

### Modified Capabilities

- `communications/notification-gateway`: tornar configuração, consentimento e envio compreensíveis na administração.

## Impact

Somente a experiência web de Administration em `web/OrderHub.Web/src/modules/administration/communications/`. Não altera persistência, autorização, contratos da API, regras de consentimento ou integração com provedores.
