## Why

Notificações e futuras integrações não podem depender de uma gravação no banco seguida de publicação não atômica. A outbox permite evolução confiável sem introduzir um broker antes de existir necessidade comprovada.

## What Changes

- Persistir mensagens de integração na mesma transação da mudança de negócio.
- Processar mensagens com retry, idempotência e rastreabilidade.
- Definir retenção, falha terminal e recuperação operacional.
- Manter o transporte desacoplado e inicialmente executável no próprio monólito.

## Capabilities

### New Capabilities

- `architecture/transactional-outbox`: entrega confiável de eventos derivados de transações locais.

### Modified Capabilities

Nenhuma.

## Impact

Persistência PostgreSQL, interceptação explícita no fluxo de escrita, worker hospedado e observabilidade. Não adiciona RabbitMQ nem altera o Domain para conhecer infraestrutura.
