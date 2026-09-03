# Order Management Specification

## Purpose

Define pedidos como registros transacionais e históricos, protegendo composição, totais, atendimento e ciclo de vida.

## Requirements

### Requirement: Pedido pertence a uma unidade e possui número estável
Cada pedido MUST pertencer a uma unidade, possuir número único e monotônico no escopo da unidade e indicar atendimento em mesa, retirada ou entrega.

#### Scenario: Referência cruzada
- **WHEN** cliente, mesa, produto, cupom ou forma relacionada não pertencer à unidade do pedido
- **THEN** o sistema MUST rejeitar a operação

### Requirement: Tipo de atendimento exige dados compatíveis
Pedido em mesa MUST referenciar mesa ativa; pedido para entrega MUST preservar o endereço de entrega aplicável; retirada MUST NOT exigir mesa ou endereço de entrega.

#### Scenario: Entrega sem endereço
- **WHEN** um pedido de entrega for submetido sem endereço válido
- **THEN** o sistema MUST rejeitar sua confirmação

### Requirement: Itens preservam snapshot comercial
Cada item MUST registrar produto, variação quando aplicável, nomes, quantidades e preços usados na compra; adicionais aplicados MUST registrar seus próprios nomes, quantidades e preços.

#### Scenario: Catálogo alterado após confirmação
- **WHEN** nome ou preço do catálogo mudar depois da criação do pedido
- **THEN** a visualização e os cálculos históricos SHALL continuar usando o snapshot do pedido

### Requirement: Totais são calculados pelo domínio
Subtotal, descontos, taxas e total MUST ser derivados dos itens e políticas aplicáveis, usar precisão monetária definida e MUST NOT resultar em total negativo.

#### Scenario: Total informado diverge
- **WHEN** um cliente informar total diferente do calculado pelo sistema
- **THEN** o sistema SHALL ignorar o valor para decisão de negócio e usar o cálculo autoritativo

### Requirement: Ciclo de vida aceita apenas transições válidas
O pedido SHALL percorrer estados conhecidos de criação, confirmação, preparação, prontidão, entrega, finalização, cancelamento ou rejeição e MUST rejeitar transições incompatíveis com o estado e tipo de atendimento atuais.

#### Scenario: Entrega antes de preparação
- **WHEN** for solicitada entrega de pedido que ainda não alcançou estado compatível
- **THEN** o domínio MUST rejeitar a transição

#### Scenario: Cancelamento terminal
- **WHEN** um pedido já cancelado, rejeitado ou finalizado receber transição operacional incompatível
- **THEN** o domínio MUST rejeitar a alteração

### Requirement: Toda mudança de status gera histórico
Cada transição aceita MUST produzir entrada histórica imutável com status anterior, novo status, instante, observação opcional e ator quando autenticado.

#### Scenario: Transição por processo público
- **WHEN** uma transição aceita não possuir usuário administrativo
- **THEN** o histórico SHALL registrar o ator como ausente e preservar os demais dados

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

### Requirement: Desconto de cupom integra o total autoritativo
O pedido SHALL calcular o desconto elegível sobre seu subtotal aplicável, preservar código e valor concedido e MUST impedir que o total final seja negativo.

#### Scenario: Cupom fixo maior que o valor elegível
- **WHEN** o desconto fixo superar o valor do pedido ao qual pode ser aplicado
- **THEN** o desconto SHALL ser limitado ao valor elegível e o total final SHALL ser zero

### Requirement: Remoção de cupom recalcula o pedido não confirmado
Um cupom MAY ser removido antes da confirmação e o pedido SHALL recalcular seus totais sem preservar consumo.

#### Scenario: Cupom removido durante composição
- **WHEN** o cliente remover o cupom antes de confirmar o pedido
- **THEN** o sistema SHALL restaurar o total sem desconto e MUST NOT contabilizar uso

### Requirement: Estado operacional e cobertura financeira são independentes
O pedido SHALL expor seu status operacional e seu valor financeiramente coberto como informações distintas, sem inferir automaticamente uma transição operacional a partir de pagamento.

#### Scenario: Pagamento integral antes do preparo
- **WHEN** o pedido for integralmente pago enquanto ainda aguarda confirmação operacional
- **THEN** sua cobertura financeira SHALL ser integral e seu status operacional SHALL permanecer inalterado

### Requirement: Valor devido é autoritativo para pagamentos
Operações financeiras MUST usar o total atual preservado pelo pedido e MUST NOT aceitar um valor devido informado pelo cliente como fonte de decisão.

#### Scenario: Valor informado diverge do pedido
- **WHEN** uma confirmação de pagamento usar valor devido diferente do total autoritativo
- **THEN** o sistema MUST avaliar excesso e cobertura a partir do pedido persistido

### Requirement: Visitante acompanha e cancela somente por referência pública
Operações anônimas sobre pedido SHALL exigir referência opaca válida e o cancelamento MUST respeitar o estado atual e a política de cancelamento do domínio.

#### Scenario: Cancelamento após início do preparo
- **WHEN** o visitante solicitar cancelamento em estado que não admite cancelamento público
- **THEN** o sistema MUST rejeitar a transição e preservar o pedido

### Requirement: Operação administrativa consulta e transiciona pedidos
A API SHALL fornecer listagem e detalhe por período, status, número e tipo de atendimento e SHALL expor somente transições autorizadas pelo papel e pelo domínio.

#### Scenario: Cozinha inicia preparação
- **WHEN** um usuário autorizado da cozinha solicitar preparação de pedido em estado compatível
- **THEN** o sistema SHALL aplicar a transição e registrar o ator no histórico
