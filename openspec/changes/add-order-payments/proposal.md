## Why

Pedidos confirmados precisam registrar como o valor devido será ou foi coberto, inclusive com divisão entre formas de pagamento. O estado financeiro deve permanecer explícito, histórico e idempotente antes que as APIs exponham operações de pagamento.

## What Changes

- Implementar formas de pagamento configuráveis por estabelecimento.
- Implementar múltiplos pagamentos por pedido, com status, valor, troco e referência externa opcional.
- Derivar cobertura financeira a partir de pagamentos confirmados e impedir confirmação excedente sem regra de troco.
- Proteger confirmações críticas com idempotência e persistência transacional.
- Adicionar Commands, Queries internas, validators, gateways e testes.

## Capabilities

### New Capabilities

Nenhuma.

### Modified Capabilities

- `payments/order-payments`: detalhar configuração de formas, registro, confirmação idempotente e cálculo da cobertura financeira.
- `ordering/order-management`: integrar o valor devido e o estado de cobertura financeira sem misturar o ciclo operacional do pedido com o ciclo do pagamento.

## Impact

Afeta Domain, Application, Infrastructure, migrations e testes dos módulos Payments e Ordering. Depende de `add-order-lifecycle` e não introduz integração com adquirentes externos.
