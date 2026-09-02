## Purpose

Define clientes e endereços por unidade com baixa fricção de compra e preservação do isolamento de dados.

## ADDED Requirements

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

