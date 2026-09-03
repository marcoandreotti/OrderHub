# Order Payments Specification

## Purpose

Define formas e registros de pagamento por unidade, admitindo divisão de valores e futura conciliação externa.

## Requirements

### Requirement: Forma de pagamento é configurável por unidade
Cada forma de pagamento SHALL pertencer a uma unidade, possuir código único nessa unidade, indicar se é online e somente estar disponível para novas cobranças quando ativa.

#### Scenario: Forma inativa
- **WHEN** uma nova cobrança escolher forma inativa
- **THEN** o sistema MUST rejeitar a cobrança sem afetar pagamentos históricos

### Requirement: Pedido admite múltiplos pagamentos
Um pedido MAY possuir múltiplos pagamentos não negativos, inclusive em formas distintas, e cada pagamento MUST pertencer à mesma unidade do pedido.

#### Scenario: Pagamento dividido
- **WHEN** dois pagamentos válidos totalizarem o valor devido
- **THEN** o sistema SHALL preservar ambos e considerar o valor integralmente coberto

#### Scenario: Pagamento excedente
- **WHEN** a confirmação de um pagamento fizer a soma confirmada exceder o valor devido sem regra de troco aplicável
- **THEN** o sistema MUST rejeitar a confirmação

### Requirement: Estado financeiro é explícito e histórico
Cada pagamento SHALL manter status, valor, forma, instantes relevantes, troco quando aplicável e identificador externo opcional, sem depender do estado atual da forma de pagamento.

#### Scenario: Confirmação repetida
- **WHEN** a mesma operação idempotente de confirmação for reapresentada
- **THEN** o sistema SHALL preservar um único efeito financeiro

### Requirement: Confirmação financeira exige chave idempotente
Toda confirmação de pagamento MUST receber uma chave idempotente válida no escopo do estabelecimento e da operação financeira.

#### Scenario: Repetição com os mesmos dados
- **WHEN** a mesma chave e o mesmo conteúdo forem reapresentados
- **THEN** o sistema SHALL retornar o resultado original sem duplicar o pagamento confirmado

#### Scenario: Repetição com conteúdo diferente
- **WHEN** a mesma chave for reapresentada com valor ou pagamento diferente
- **THEN** o sistema MUST rejeitar a operação por conflito

### Requirement: Cobertura financeira deriva de pagamentos confirmados
Somente pagamentos confirmados SHALL compor o valor pago; registros pendentes, falhos ou cancelados MUST permanecer históricos sem cobrir o pedido.

#### Scenario: Pagamento pendente completa o valor nominal
- **WHEN** a soma nominal incluir pagamento ainda pendente
- **THEN** o pedido MUST continuar financeiramente descoberto pelo valor pendente

### Requirement: API pública oferece somente formas ativas
A composição pública SHALL listar somente formas de pagamento ativas do estabelecimento e MUST validar novamente a forma ao confirmar o pedido.

#### Scenario: Forma desativada antes da confirmação
- **WHEN** uma forma selecionada for desativada antes da confirmação
- **THEN** a API MUST rejeitar a seleção e solicitar uma forma disponível

### Requirement: Gestão mantém formas e consulta pagamentos
A API SHALL permitir manutenção de formas de pagamento por gestão e consulta dos pagamentos do pedido por papéis operacionais autorizados.

#### Scenario: Forma desativada com histórico
- **WHEN** uma forma for desativada administrativamente
- **THEN** ela MUST deixar de aparecer em novas cobranças e SHALL permanecer nos pagamentos históricos
