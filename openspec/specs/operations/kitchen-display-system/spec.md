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

### Requirement: Ticket identifica tipo de atendimento sem repetir etapa
O KDS SHALL apresentar o tipo de atendimento junto ao número do pedido, com rótulo textual distinguível para entrega, retirada e mesa. O ticket SHALL NOT repetir em seu conteúdo o estado já comunicado pelo cabeçalho da etapa.

#### Scenario: Ticket na fila de produção
- **WHEN** um ticket aparece em uma etapa da cozinha
- **THEN** o número e tipo de atendimento são identificáveis, e o estado aparece somente no cabeçalho da coluna

### Requirement: Etapas da cozinha têm ordem e cabeçalhos operacionais claros
O KDS SHALL apresentar “Aguardando preparo” antes de “Em preparo”, cada etapa em uma faixa com cor distinta, texto centralizado e contagem perceptível.

#### Scenario: Fila com as duas etapas
- **WHEN** a fila é exibida em desktop ou tablet
- **THEN** “Aguardando preparo” aparece primeiro e cada faixa identifica sua etapa e quantidade

### Requirement: Título e sincronização formam uma hierarquia concisa
O KDS SHALL exibir “COZINHA - Fila de produção” como título, preservando a cor de marca para “COZINHA”, e combinar prioridade da fila, conexão e última sincronização em uma única linha abaixo dele.

#### Scenario: Canal em tempo real indisponível
- **WHEN** a fila opera por atualização periódica
- **THEN** a linha informa prioridade, modo de atualização e última sincronização, mantendo alertas e recuperação disponíveis
