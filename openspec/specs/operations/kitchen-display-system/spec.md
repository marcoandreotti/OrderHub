# Kitchen Display System Specification

## Purpose

Organiza o trabalho da cozinha em uma fila tenant-scoped, orientada por preparo, prioridade e tempo prometido dos pedidos.

## Requirements

### Requirement: Fila contém somente trabalho autorizado e preparável
O KDS MUST mostrar somente pedidos da unidade autorizada em estados compatíveis, com itens, quantidades, modificadores e observações necessários à produção.

#### Scenario: Operador troca de unidade
- **WHEN** o operador seleciona outra unidade autorizada
- **THEN** o trabalho anterior é removido antes de carregar a nova fila

### Requirement: Ações respeitam estado vigente
O KDS SHALL permitir iniciar e concluir preparo somente conforme papel e transições válidas, e MUST tratar conflitos recarregando o estado do servidor.

#### Scenario: Outro terminal avança o pedido
- **WHEN** uma ação usa estado desatualizado
- **THEN** o sistema rejeita a transição e apresenta o estado atual sem duplicar histórico

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
