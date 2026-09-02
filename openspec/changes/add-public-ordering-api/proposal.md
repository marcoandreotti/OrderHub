## Why

Com catálogo, clientes, pedidos, cupons e pagamentos implementados, falta uma borda HTTP pública segura para que visitantes componham, criem e acompanhem pedidos. Essa API deve resolver o estabelecimento no servidor e nunca confiar em identificadores ou totais fornecidos pelo cliente.

## What Changes

- Expor contexto público do estabelecimento e configurações necessárias ao pedido por slug ou token opaco de mesa.
- Expor criação ou atualização de cliente e endereço dentro do fluxo público.
- Expor simulação autoritativa, validação de cupom, criação idempotente e consulta de pedido por referência pública opaca.
- Expor cancelamento público somente quando permitido pelo domínio.
- Retornar contratos explícitos e ProblemDetails sem revelar TenantId, entidades ou dados internos.

## Capabilities

### New Capabilities

- `ordering/public-ordering-api`: definir os fluxos HTTP anônimos de composição, criação idempotente, acompanhamento e cancelamento de pedidos.

### Modified Capabilities

- `customers/customer-records`: definir o uso público de clientes e endereços dentro do estabelecimento resolvido.
- `ordering/order-management`: definir referências públicas opacas e operações permitidas ao visitante.
- `promotions/coupon-management`: definir validação pública de cupom sem expor regras ou dados internos.
- `payments/order-payments`: definir as formas ativas e os dados financeiros aceitos na criação pública do pedido.

## Impact

Afeta Contracts, Application e API, com adapters de leitura quando necessários e testes de integração HTTP. Depende das quatro changes de domínio anteriores e reutiliza o catálogo público já concluído.
