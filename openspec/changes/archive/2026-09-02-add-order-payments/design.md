## Context

O módulo Ordering preservará o total devido, enquanto Payments precisa registrar eventos financeiros históricos sem acoplar o pedido a um provedor externo.

## Goals / Non-Goals

**Goals:**

- Separar ciclo financeiro do ciclo operacional.
- Suportar divisão, troco aplicável e confirmação idempotente.
- Preservar histórico mesmo quando a forma for desativada.

**Non-Goals:**

- Integrar adquirentes, Pix, webhooks ou conciliação externa.
- Expor endpoints HTTP nesta etapa.

## Decisions

### Pagamento como aggregate próprio tenant-scoped

O pagamento terá ciclo e idempotência próprios e referenciará o pedido sem pertencer à coleção controlada por ele. Incorporá-lo ao aggregate do pedido aumentaria contenção e misturaria ciclos distintos.

### Cobertura calculada por projeção autoritativa

A soma de pagamentos confirmados será consultada no mesmo estabelecimento e comparada ao total do pedido. Um campo pago mutável no pedido poderia divergir do histórico financeiro.

### Idempotência persistida

Chave, hash da operação e resultado serão persistidos com constraint tenant-scoped. Cache em memória foi rejeitado por não sobreviver a reinícios.

## Risks / Trade-offs

- [Confirmações concorrentes excedem o devido] → Serializar a decisão financeira por pedido na transação.
- [Pagamento externo futuro exige novos estados] → Manter enumeração e transições explícitas extensíveis por mudança de spec.

## Migration Plan

Criar tabelas de formas, pagamentos e idempotência; validar upgrade e rollback; somente depois habilitar handlers de confirmação.
