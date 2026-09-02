# Customer Records Specification

## Purpose

Define clientes e endereços por unidade com baixa fricção de compra e preservação do isolamento de dados.

## Requirements

### Requirement: Cliente pode comprar sem conta autenticada
O sistema SHALL permitir registrar cliente da unidade com nome e telefone, sem exigir senha ou confirmação de e-mail para iniciar um pedido.

#### Scenario: Cliente sem e-mail
- **WHEN** um visitante fornecer nome e telefone válidos sem e-mail
- **THEN** o sistema SHALL permitir seu uso no fluxo de pedido

### Requirement: Dados do cliente são isolados por unidade
Clientes MUST pertencer a uma unidade e MUST NOT ser pesquisados, vinculados ou alterados por outra unidade apenas por coincidência de telefone, e-mail ou documento.

#### Scenario: Telefones iguais em unidades distintas
- **WHEN** unidades diferentes registrarem o mesmo telefone
- **THEN** o sistema SHALL manter registros independentes e isolados

### Requirement: Cliente mantém endereços rotulados
Um cliente MAY possuir múltiplos endereços completos e SHALL possuir no máximo um endereço principal por vez.

#### Scenario: Novo endereço principal
- **WHEN** outro endereço do cliente for definido como principal
- **THEN** o sistema SHALL remover a marca principal do endereço anterior de forma consistente

### Requirement: Contatos de cliente são normalizados por estabelecimento
O sistema SHALL normalizar nome, telefone e e-mail opcional antes da persistência e MUST aplicar qualquer critério de unicidade somente no escopo do mesmo estabelecimento.

#### Scenario: Mesmo telefone no mesmo estabelecimento
- **WHEN** um cliente já registrado for identificado pelo telefone normalizado dentro do mesmo estabelecimento
- **THEN** o sistema SHALL atualizar ou reutilizar esse registro conforme a operação solicitada sem criar duplicação acidental

### Requirement: Endereços pertencem ao cliente e ao mesmo estabelecimento
Cada endereço MUST pertencer ao mesmo Tenant e estabelecimento do cliente, SHALL conter os dados necessários para entrega e MUST impedir associação cruzada.

#### Scenario: Endereço de cliente de outra unidade
- **WHEN** uma operação tentar associar ao cliente um endereço pertencente a outra unidade
- **THEN** o sistema MUST rejeitar a operação sem alterar os registros existentes

### Requirement: Alterações de cliente e endereço são consistentes
Uma operação que alterar cliente, endereços e endereço principal como um único efeito SHALL ser atômica.

#### Scenario: Falha ao definir endereço principal
- **WHEN** a persistência falhar durante a troca do endereço principal
- **THEN** o sistema SHALL preservar o cliente e a configuração de endereços anteriores integralmente
