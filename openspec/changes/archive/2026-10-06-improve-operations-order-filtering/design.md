# Design

Os campos `De` e `Até` representam dias locais inclusivos. Na consulta, `from` é o início local do dia inicial e `to` é o início local do dia seguinte ao final selecionado, mantendo o intervalo semiaberto já suportado pelo backend. Os valores date-only ficam na query string para permitir recarga e compartilhamento da mesma visão.

O padrão inicial inclui hoje e ontem. Datas inválidas ou intervalo invertido não serão enviados; os controles continuam editáveis e os demais filtros existentes permanecem utilizáveis.

O status continua expresso pelo título e pela cor semântica da coluna. O cartão mantém número, tempo, atendimento, cliente quando disponível, itens, total, sinais operacionais e próxima ação.
