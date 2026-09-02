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

