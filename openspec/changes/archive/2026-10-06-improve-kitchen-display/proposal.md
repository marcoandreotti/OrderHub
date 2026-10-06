# Melhorar leitura da fila da cozinha

## Problema

A fila da cozinha repete a etapa nos tickets, deixa tipo de atendimento em texto secundário e apresenta os estados em ordem que dificulta a leitura operacional.

## Mudanças

- Exibir identificação do atendimento junto ao número com cores semânticas para entrega, retirada e mesa.
- Remover o status repetido dos tickets, manter etapas em cabeçalhos de faixa colorida e centralizada, e ordenar “Aguardando preparo” antes de “Em preparo”.
- Combinar prioridade da fila, conexão e última sincronização em uma linha abaixo do título.
- Apresentar “COZINHA” na cor de marca junto a “Fila de produção”.

## Impacto

Mudança restrita à interface KDS existente; fluxo de autorização, leitura e transição da fila permanece inalterado.
