## ADDED Requirements

### Requirement: Painel torna urgência e próxima ação imediatamente perceptíveis
O painel SHALL apresentar em cada pedido executável o estado, tempo relevante, horário prometido quando existir, resumo dos itens e próxima ação autorizada com hierarquia suficiente para decisão sem abrir detalhes nos casos comuns. A consulta da fila SHALL fornecer a projeção mínima dos itens no próprio resumo, preservando isolamento por Tenant e sem consultas de detalhe por cartão. Urgência MUST NOT depender somente de cor.

#### Scenario: Pedido novo aguarda aceite
- **WHEN** um pedido confirmado entra na etapa inicial
- **THEN** número, tipo de atendimento, tempo de espera, itens essenciais e ação de aceite ficam perceptíveis no próprio cartão

#### Scenario: Fila contém pedidos com múltiplos itens
- **WHEN** o operador consulta uma página da fila
- **THEN** cada resumo apresenta quantidade, produto e variação dos itens projetados pela leitura da fila sem carregar o agregado nem emitir uma consulta de detalhe por pedido

#### Scenario: Pedido ultrapassa o tempo esperado
- **WHEN** o tempo relevante do pedido alcança a faixa de atraso definida pela experiência
- **THEN** o painel diferencia o pedido por texto ou ícone além da cor e mantém disponível a ação compatível com o estado

### Requirement: Painel adapta densidade ao dispositivo operacional
O painel SHALL preservar leitura, seleção, filtros, estados, detalhes e transições em desktop, monitor, tablet e interação touch suportados, reorganizando conteúdo sem ocultar a fila executável.

#### Scenario: Operação em tablet touch
- **WHEN** o operador utiliza o painel em tablet suportado
- **THEN** cartões, filtros e ações permanecem legíveis, não se sobrepõem e oferecem áreas acionáveis adequadas sem depender de hover

#### Scenario: Operação em monitor amplo
- **WHEN** o painel é exibido em monitor amplo
- **THEN** as etapas simultâneas utilizam o espaço disponível sem dispersar tempo, estado e ações essenciais

### Requirement: Estado de atualização permanece associado à fila
O painel SHALL manter conexão, última sincronização e possível defasagem perceptíveis junto ao contexto operacional, sem remover os pedidos já carregados durante uma falha transitória.

#### Scenario: Canal em tempo real entra em fallback
- **WHEN** a conexão em tempo real é interrompida e o fallback passa a reconciliar a fila
- **THEN** o operador percebe o modo de atualização, a última sincronização e se os dados podem estar defasados enquanto continua consultando os pedidos existentes
