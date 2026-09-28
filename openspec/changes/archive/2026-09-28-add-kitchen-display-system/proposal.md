## Why

O painel operacional geral acompanha pedidos, mas não organiza o trabalho específico da cozinha. Um KDS reduz perda de contexto, atrasos e dependência de comandas impressas.

## What Changes

- Criar visão de cozinha por fila, prioridade, tempo e etapa de produção.
- Exibir itens, quantidades, observações e modificadores necessários ao preparo.
- Permitir ações autorizadas de início e conclusão de preparo.
- Destacar atrasos e preservar consistência diante de concorrência.

## Capabilities

### New Capabilities

- `operations/kitchen-display-system`: experiência operacional dedicada à produção e expedição de pedidos.

### Modified Capabilities

Nenhuma. O KDS utiliza transições já controladas por `ordering/order-management`.

## Impact

Nova área Vue/Quasar, novas queries Dapper e endpoints operacionais. Pode consumir `operations/realtime-order-updates` quando essa change estiver aplicada, mantendo atualização compatível por polling até então.
