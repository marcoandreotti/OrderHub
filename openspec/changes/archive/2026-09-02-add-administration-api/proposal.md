## Why

Os módulos de negócio precisam de uma borda administrativa única para operação e manutenção por usuários autenticados. A API deve aplicar políticas por papel, resolver Tenant e estabelecimento pelo contexto autenticado e separar endpoints de gestão das rotas públicas.

## What Changes

- Expor endpoints administrativos para clientes, pedidos, cupons, formas de pagamento e pagamentos.
- Expor consultas paginadas e filtradas por estabelecimento para operação diária.
- Expor transições operacionais de pedido conforme políticas de atendimento, cozinha, entrega e gestão.
- Expor manutenção de cupons e formas de pagamento apenas a papéis autorizados.
- Padronizar contratos, paginação, validação, autorização e ProblemDetails em todos os endpoints.

## Capabilities

### New Capabilities

- `administration/administration-api`: definir a borda HTTP autenticada para gestão e operação dos módulos implementados.

### Modified Capabilities

- `customers/customer-records`: definir consultas e manutenção administrativas de clientes e endereços.
- `ordering/order-management`: definir consultas operacionais e transições administrativas autorizadas.
- `promotions/coupon-management`: definir manutenção administrativa de cupons.
- `payments/order-payments`: definir manutenção de formas e operações administrativas de pagamentos.
- `identity/administrative-users`: detalhar as políticas aplicadas às capacidades administrativas e operacionais expostas.

## Impact

Afeta Contracts, Application e API, incluindo autorização por políticas, projeções Dapper e testes de integração HTTP. Depende das changes de domínio e da API pública planejadas anteriormente; o catálogo administrativo existente será preservado e alinhado às mesmas convenções.
