## Why

Pedidos são o núcleo transacional do OrderHub e dependem do catálogo e dos registros de clientes já disponíveis. A solução precisa materializar composição, snapshots comerciais, totais, numeração por estabelecimento e transições auditáveis antes de expor qualquer fluxo HTTP de pedidos.

## What Changes

- Implementar o aggregate de pedido para mesa, retirada e entrega.
- Preservar snapshots de produtos, variações, adicionais, cliente e endereço aplicáveis.
- Calcular subtotal, descontos, taxas e total exclusivamente no domínio.
- Implementar numeração monotônica por estabelecimento, persistência transacional e histórico imutável de status.
- Implementar Commands, Queries internas, validators, gateways e testes do ciclo de vida.

## Capabilities

### New Capabilities

Nenhuma.

### Modified Capabilities

- `ordering/order-management`: detalhar criação, composição, confirmação, transições, cancelamento, histórico e consultas do ciclo de vida do pedido.

## Impact

Afeta Domain, Application, Infrastructure, migrations e testes do módulo Ordering, além de portas de leitura para Catalog, Customers e Operations. Não adiciona endpoints HTTP nesta change.
