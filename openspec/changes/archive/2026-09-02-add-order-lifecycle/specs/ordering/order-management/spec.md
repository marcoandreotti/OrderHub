## ADDED Requirements

### Requirement: Pedido é composto antes da confirmação
O sistema SHALL permitir construir um pedido em estado inicial e MUST revalidar escopo, disponibilidade e composição no momento da confirmação.

#### Scenario: Produto desativado antes da confirmação
- **WHEN** um produto selecionado deixar de ser vendável antes da confirmação
- **THEN** o sistema MUST rejeitar a confirmação sem produzir pedido confirmado parcial

### Requirement: Número do pedido é atribuído atomicamente
O número monotônico do pedido SHALL ser reservado no escopo do estabelecimento durante a confirmação e MUST permanecer único sob concorrência.

#### Scenario: Confirmações concorrentes
- **WHEN** dois pedidos do mesmo estabelecimento forem confirmados simultaneamente
- **THEN** ambos SHALL receber números distintos e crescentes sem depender de numeração global

### Requirement: Referência pública do pedido é opaca
Cada pedido confirmado SHALL possuir referência pública não previsível distinta de seus identificadores internos.

#### Scenario: Consulta por sequência aproximada
- **WHEN** um visitante tentar inferir outro pedido alterando a referência pública
- **THEN** o sistema MUST NOT revelar a existência nem os dados do outro pedido

### Requirement: Transições preservam consistência operacional
Cada transição SHALL validar o estado atual, o tipo de atendimento e o ator aplicável antes de alterar estado e registrar histórico no mesmo efeito atômico.

#### Scenario: Falha ao gravar histórico
- **WHEN** o histórico da transição não puder ser persistido
- **THEN** o status do pedido MUST permanecer inalterado
