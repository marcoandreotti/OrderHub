## ADDED Requirements

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
