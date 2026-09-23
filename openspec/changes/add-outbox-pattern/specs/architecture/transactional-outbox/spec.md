## Purpose

Garante publicação recuperável de eventos derivados de gravações locais sem exigir um broker distribuído para a primeira implementação.

## ADDED Requirements

### Requirement: Mensagem e estado de negócio são atômicos
Uma mensagem de outbox MUST ser persistida na mesma transação da mudança que a originou e MUST NOT existir quando a transação de negócio falhar.

#### Scenario: Commit é revertido
- **WHEN** a gravação do aggregate não conclui
- **THEN** nenhuma mensagem correspondente fica disponível para processamento

### Requirement: Processamento tolera repetição
O processador SHALL usar retry limitado, registrar falhas e exigir consumidores idempotentes, pois uma mensagem MAY ser entregue mais de uma vez.

#### Scenario: Processo cai após entrega
- **WHEN** o efeito ocorre antes da marcação de conclusão
- **THEN** a nova tentativa não produz efeito duplicado observável
