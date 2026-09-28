## Why

Pedidos de entrega preservam endereço, mas ainda não existe política completa de cobertura, região, taxa e execução. Sem isso, o estabelecimento não controla onde entrega nem o custo aplicado.

## What Changes

- Configurar regiões atendidas, critérios de cobertura, taxas e estimativas por unidade.
- Validar e normalizar o endereço informado antes da confirmação.
- Preservar no pedido o snapshot da política e taxa aplicadas.
- Acompanhar despacho, responsável e conclusão da entrega quando utilizados.

## Capabilities

### New Capabilities

- `delivery/delivery-management`: cobertura, precificação e acompanhamento operacional da entrega.

### Modified Capabilities

- `ordering/order-management`: preservar elegibilidade e snapshot comercial da entrega no pedido.
- `ordering/public-ordering-api`: cotar e validar entrega nos fluxos públicos.
- `ordering/public-ordering-web`: coletar endereço, apresentar taxa e comunicar indisponibilidade.

## Impact

Novo módulo de Delivery, integrações com pedidos e clientes, persistência, endpoints administrativos e públicos e interfaces web. Geocodificação externa não é obrigatória nesta primeira versão.
