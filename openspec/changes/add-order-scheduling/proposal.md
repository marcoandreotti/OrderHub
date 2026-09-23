## Why

Clientes só podem pedir para atendimento imediato, embora retirada e entrega frequentemente sejam planejadas. Agendamento amplia a operação sem criar um fluxo de pedido separado.

## What Changes

- Permitir escolha de data e horário futuros em modalidades habilitadas.
- Validar janela mínima, horizonte, horários e capacidade configurados.
- Preservar o agendamento no pedido e ordenar a operação pelo momento prometido.
- Revalidar as condições autoritativas durante a confirmação.

## Capabilities

### New Capabilities

- `ordering/order-scheduling`: regras de janela, capacidade e compromisso temporal de pedidos futuros.

### Modified Capabilities

- `ordering/order-management`: associar e preservar agendamento no ciclo de vida.
- `ordering/public-ordering-api`: consultar slots e confirmar o slot escolhido.
- `ordering/public-ordering-web`: permitir seleção e comunicação de horários disponíveis.
- `operations/order-operations-dashboard`: separar e ordenar pedidos imediatos e agendados.

## Impact

Ordering Domain, persistência, consultas, APIs pública e operacional e aplicações web. Depende conceitualmente da disponibilidade definida por `add-business-hours-availability`.
