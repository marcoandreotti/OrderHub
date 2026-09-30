## ADDED Requirements

### Requirement: KDS prioriza leitura à distância
O KDS SHALL apresentar número do pedido, tipo de atendimento, tempo, quantidades, itens, modificadores e observações com hierarquia legível nos monitores e tablets suportados. Metadados secundários MUST NOT competir com informações de produção.

#### Scenario: Cozinheiro consulta monitor de produção
- **WHEN** o KDS é utilizado em um monitor na distância operacional prevista
- **THEN** o cozinheiro identifica pedido, tempo, quantidades, modificadores, observações e etapa sem abrir uma visualização complementar

### Requirement: Prioridade e atraso usam sinais redundantes
O KDS SHALL comunicar prioridade, proximidade do horário prometido e atraso por texto ou ícone além da cor, mantendo ordenação coerente com o trabalho executável.

#### Scenario: Ticket torna-se atrasado
- **WHEN** um ticket ultrapassa o limite de tempo aplicável
- **THEN** o KDS altera sua indicação de urgência com rótulo ou ícone perceptível e mantém o ticket na posição operacional adequada

### Requirement: Ações de produção suportam touch e teclado
O KDS SHALL permitir iniciar, pausar quando suportado e concluir trabalho pelos dispositivos de entrada previstos, com alvos adequados, foco visível e confirmação perceptível da transição.

#### Scenario: Cozinheiro conclui ticket em tablet
- **WHEN** o cozinheiro aciona a conclusão por touch em um tablet suportado
- **THEN** a ação pode ser executada sem hover, apresenta resultado perceptível e o ticket é reconciliado com o estado vigente

#### Scenario: Operador utiliza teclado
- **WHEN** o operador percorre tickets e ações por teclado
- **THEN** seleção, foco, ação disponível e resultado permanecem identificáveis
