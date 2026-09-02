## Why

Após o ciclo básico de pedidos, o sistema precisa aplicar promoções de forma autoritativa e historicamente estável. Cupons devem respeitar estabelecimento, validade, limites e valor mínimo sem permitir descontos inconsistentes ou reutilização acima do permitido.

## What Changes

- Implementar cupons percentuais e de valor fixo, com código normalizado por estabelecimento.
- Validar janela de vigência, ativação, pedido mínimo e limite de usos.
- Aplicar o desconto ao pedido sem permitir total negativo e preservar snapshot do benefício concedido.
- Persistir aplicação e consumo de forma transacional e protegida contra concorrência.
- Adicionar Commands, Queries internas, validators, gateways e testes.

## Capabilities

### New Capabilities

Nenhuma.

### Modified Capabilities

- `promotions/coupon-management`: detalhar cadastro, elegibilidade, consumo consistente e integração do snapshot do cupom ao pedido.
- `ordering/order-management`: incorporar o desconto de cupom ao cálculo autoritativo e ao histórico comercial do pedido.

## Impact

Afeta Domain, Application, Infrastructure, migrations e testes dos módulos Promotions e Ordering. Depende de `add-order-lifecycle` e não adiciona endpoints HTTP.
