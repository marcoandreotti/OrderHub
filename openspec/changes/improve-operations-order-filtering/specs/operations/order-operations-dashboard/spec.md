## ADDED Requirements

### Requirement: Painel filtra pedidos por período
O painel SHALL oferecer data inicial e final inclusivas para filtrar os pedidos retornados pela consulta da fila. Na primeira abertura sem datas na URL, SHALL selecionar hoje e o dia anterior. Os limites enviados à API SHALL representar o início local da data inicial e o início do dia seguinte à data final, em intervalo semiaberto. O período SHALL permanecer nos parâmetros da URL.

#### Scenario: Primeira abertura sem período
- **WHEN** o operador abre o painel sem datas nos parâmetros da URL
- **THEN** a fila consulta os dois últimos dias corridos, incluindo hoje

#### Scenario: Aplicação do período selecionado
- **WHEN** o operador altera as datas para um intervalo válido
- **THEN** o painel consulta novamente a API com o período e demais filtros selecionados

#### Scenario: Intervalo inválido
- **WHEN** a data inicial é posterior à data final
- **THEN** o painel não envia o intervalo inválido à API e indica que a aplicação não está disponível

### Requirement: Estado não é repetido nos cartões da fila
O painel SHALL comunicar o estado do pedido no cabeçalho da coluna e SHALL omitir o rótulo visual de estado repetido em cada cartão dessa coluna.

#### Scenario: Cartão dentro de uma coluna de estado
- **WHEN** um pedido é exibido na fila
- **THEN** seu estado é comunicado pela coluna e não é repetido como etiqueta no cartão

### Requirement: Estado da conexão e sincronização é apresentado em uma linha
O painel SHALL combinar a descrição do modo de atualização, o estado da conexão e a última sincronização em uma única linha junto ao título, mantendo alertas e ações de recuperação visíveis quando necessários.

#### Scenario: Atualização em tempo real conectada
- **WHEN** a conexão está ativa e a fila já foi sincronizada
- **THEN** uma única linha informa a conexão, reconciliação e horário da última sincronização

#### Scenario: Atualização por fallback
- **WHEN** o canal em tempo real está indisponível
- **THEN** uma única linha informa a atualização periódica e a última sincronização, mantendo visível eventual alerta de conexão

### Requirement: Cartões distinguem atendimento atual do histórico
Os cartões de pedidos em atendimento SHALL exibir o tempo decorrido em minutos. Cartões em estados históricos (`Completed`, `Cancelled` ou `Rejected`) SHALL exibir data e hora de criação.

#### Scenario: Pedido em atendimento
- **WHEN** um pedido está em uma etapa operacional ativa
- **THEN** o cartão mostra há quantos minutos ele foi criado

#### Scenario: Pedido em estado histórico
- **WHEN** um pedido está concluído, cancelado ou rejeitado
- **THEN** o cartão mostra a data e a hora em que foi criado

#### Scenario: Coluna histórica sem subtítulo de atendimento
- **WHEN** a coluna concluídos, cancelados ou rejeitados contém pedidos imediatos
- **THEN** os cartões são exibidos sem o subtítulo “Imediatos”
