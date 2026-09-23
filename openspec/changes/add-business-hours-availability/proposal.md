## Why

Horários semanais simples não representam pausas, feriados, modalidades temporariamente fechadas ou produtos indisponíveis. A operação precisa impedir pedidos que o estabelecimento não consegue atender.

## What Changes

- Adicionar exceções de calendário e pausas manuais por modalidade.
- Calcular disponibilidade autoritativa no servidor e expor motivos e próxima abertura.
- Permitir indisponibilidade temporária de ofertas sem apagar cadastro.
- Revalidar disponibilidade na confirmação do pedido.

## Capabilities

### New Capabilities

Nenhuma.

### Modified Capabilities

- `operations/service-configuration`: ampliar horários regulares com exceções, pausas e disponibilidade por modalidade.
- `catalog/product-catalog`: representar indisponibilidade temporária separadamente da ativação administrativa.
- `ordering/public-ordering-api`: aplicar e expor disponibilidade autoritativa durante consulta e confirmação.
- `ordering/public-ordering-web`: comunicar estados de indisponibilidade e bloquear novas seleções inválidas.

## Impact

Domínio operacional e de catálogo, persistência EF Core, projeções Dapper, APIs pública e administrativa e aplicações web.
