## Why

O fluxo de pedidos precisa identificar clientes e endereços sem exigir conta autenticada, mantendo os registros rigorosamente isolados por estabelecimento. Esta é a primeira dependência ainda ausente para compor pedidos de entrega e preservar dados reutilizáveis do cliente.

## What Changes

- Implementar o cadastro e a atualização de clientes por estabelecimento com nome, telefone e e-mail opcional.
- Permitir múltiplos endereços completos e garantir no máximo um endereço principal por cliente.
- Adicionar escrita com EF Core, leitura com Dapper e constraints PostgreSQL para isolamento e consistência.
- Cobrir regras de domínio, handlers, validators e persistência com testes.

## Capabilities

### New Capabilities

Nenhuma.

### Modified Capabilities

- `customers/customer-records`: detalhar o comportamento implementável para manutenção de clientes, normalização de contato, endereços e isolamento por estabelecimento.

## Impact

Afeta Domain, Application, Infrastructure, migrations e testes dos módulos Customers e Persistence. Não adiciona endpoints HTTP públicos ou administrativos; esses serão entregues pelas changes de API posteriores.
