## Why

Variações e adicionais básicos não cobrem composições comuns em pizzarias, como múltiplos sabores, bordas, remoções e dependências entre escolhas. O catálogo precisa expressar essas combinações sem lógica específica na interface.

## What Changes

- Evoluir grupos de opções com tipos, quantidades, limites, regras de compatibilidade e estratégias de preço `Additive`, `HighestPrice`, `Proportional` e `NoPriceChange`.
- Suportar sabores, bordas, adicionais, remoções e escolhas compostas por configuração.
- Representar frações de escolhas compostas exatamente por numerador e denominador; validar composição integral no servidor.
- Calcular preços de modificadores no servidor e preservar snapshots que expliquem o preço no pedido.
- Orientar a interface pública a partir dos metadados do catálogo.

## Capabilities

### New Capabilities

Nenhuma.

### Modified Capabilities

- `catalog/product-catalog`: ampliar o modelo de variações e adicionais para modificadores compostos e regras de seleção.
- `ordering/order-management`: validar composição, calcular preço e preservar snapshot dos modificadores.
- `ordering/public-ordering-api`: expor opções e validar a composição enviada.
- `ordering/public-ordering-web`: construir produtos configuráveis conforme as regras do catálogo.

## Impact

Catálogo, pedidos, persistência, contratos públicos e administrativos e interfaces de manutenção e compra. Exige migração compatível dos adicionais já existentes.
