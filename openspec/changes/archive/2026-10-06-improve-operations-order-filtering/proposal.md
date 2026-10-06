# Melhorar filtro e leitura do painel de operações

## Problema

O painel operacional não permite limitar a fila por período. Além disso, os cards repetem o estado que já está identificado pela coluna, ocupando espaço sem acrescentar informação.

## Mudanças

- Adicionar filtro de data inicial e final com padrão dos dois últimos dias corridos.
- Aplicar o intervalo à consulta autoritativa da API e preservar os filtros na URL.
- Simplificar os cards removendo o rótulo visual de estado duplicado.

## Impacto

Alteração restrita ao painel de pedidos em Operations. A API de pedidos já recebe os limites `from` e `to`; nenhum contrato de backend precisa mudar.
