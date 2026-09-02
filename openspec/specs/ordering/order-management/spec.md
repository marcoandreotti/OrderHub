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

