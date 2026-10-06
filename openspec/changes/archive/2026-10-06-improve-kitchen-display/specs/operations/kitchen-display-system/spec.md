## ADDED Requirements

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
