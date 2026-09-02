## Context

O pedido fornecerá subtotal e confirmação transacional; a promoção precisa avaliar elegibilidade e registrar consumo sem assumir responsabilidade pelo ciclo completo do pedido.

## Goals / Non-Goals

**Goals:**

- Encapsular regras e janela de elegibilidade no módulo Promotions.
- Integrar snapshot e desconto ao aggregate de pedido.
- Impedir estouro de limite sob concorrência.

**Non-Goals:**

- Campanhas automáticas, combinações de cupons ou segmentação avançada.
- Endpoints HTTP nesta etapa.

## Decisions

### Elegibilidade calculada no domínio com dados autoritativos

O pedido fornece o valor elegível e o cupom decide o desconto. Aceitar desconto calculado pelo cliente foi rejeitado por segurança.

### Consumo confirmado na transação do pedido

O uso somente será contabilizado quando o pedido for confirmado. Reservas antecipadas exigiriam expiração e coordenação sem requisito atual.

### Controle otimista e constraint de uso

O cupom terá versão de concorrência e o consumo será persistido junto do snapshot. Isso mantém o monólito simples sem locks distribuídos.

## Risks / Trade-offs

- [Concorrência excede o limite] → Detectar conflito de versão e reavaliar antes de confirmar.
- [Alteração de cupom após simulação] → Revalidar integralmente na confirmação.

## Migration Plan

Criar tabelas e índices de cupons e usos, aplicar migration e validar concorrência e rollback antes de integrar a confirmação do pedido.
