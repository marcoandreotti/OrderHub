## Context

O projeto já possui o esquema `customers` e convenções de isolamento por Tenant e estabelecimento, mas ainda não possui aggregates, casos de uso ou persistência funcional para clientes. Consulte `proposal.md` para a motivação.

## Goals / Non-Goals

**Goals:**

- Manter clientes e endereços como modelo independente do ciclo de pedidos.
- Garantir normalização, isolamento e troca atômica do endereço principal.
- Preparar portas reutilizáveis pelas APIs pública e administrativa posteriores.

**Non-Goals:**

- Criar autenticação de cliente, fidelidade ou endpoints HTTP.
- Compartilhar automaticamente clientes entre estabelecimentos.

## Decisions

### Cliente como aggregate root de seus endereços

Endereços serão controlados pelo cliente para que a regra de um único principal permaneça consistente. A alternativa de repositórios independentes permitiria alterações parciais e exigiria coordenação desnecessária.

### Normalização no domínio e constraints no banco

O domínio normalizará os contatos e a persistência repetirá as garantias estruturais com índices tenant-scoped. Confiar somente na aplicação não protegeria concorrência; confiar somente no banco produziria erros tardios pouco expressivos.

### EF Core para escrita e Dapper para leitura

O aggregate será persistido pelo fluxo de escrita existente; buscas operacionais usarão projeções próprias sem carregar o aggregate.

## Risks / Trade-offs

- [Telefones internacionais possuem formatos variados] → Armazenar valor informado e forma normalizada com validação conservadora.
- [Duas requisições tentam criar o mesmo contato] → Usar constraint por estabelecimento e converter a violação em conflito padronizado.

## Migration Plan

Adicionar as tabelas e índices do módulo Customers, aplicar a migration em banco vazio e atualizado e validar rollback antes de habilitar os casos de uso.
