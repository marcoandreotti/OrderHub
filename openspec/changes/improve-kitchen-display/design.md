# Design

Os cabeçalhos de etapa são faixas horizontais com texto centralizado e contagem alinhada à direita. “Aguardando preparo” usa o token de estado confirmado; “Em preparo” usa o token de preparo. Os tickets deixam de repetir a etapa.

Cada ticket mantém o número como informação principal e ganha uma etiqueta curta para o tipo de atendimento: entrega em azul informativo, retirada em âmbar e mesa em verde. As etiquetas incluem texto, portanto a cor não é o único sinal.

O título usa “COZINHA” em primary, seguido de “- Fila de produção”. A descrição da prioridade e o resumo dinâmico da sincronização formam uma linha só; alertas e tentativa de recuperação continuam separados.
