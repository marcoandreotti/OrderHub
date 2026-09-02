## Why

O modelo de domínio do catálogo já protege categorias, produtos, imagens, variações e adicionais, mas ainda não existe um fluxo vertical persistente e acessível pela API. Concluir essa capacidade agora disponibiliza a administração do catálogo e a consulta pública do cardápio, estabelecendo a base necessária para pedidos.

## What Changes

- Adiciona casos de uso CQRS para administrar categorias, produtos, imagens, variações, adicionais e grupos de adicionais, com validação de entrada e propagação de `CancellationToken`.
- Persiste os aggregates do catálogo com EF Core e constraints PostgreSQL compatíveis com as invariantes estruturais.
- Adiciona projeções Dapper tenant/unit-scoped para administração e para o cardápio público hierárquico.
- Expõe contratos e endpoints administrativos finos, sem retornar entidades de domínio.
- Expõe consulta pública do cardápio que omite itens inativos e só apresenta ofertas vendáveis.
- Adiciona migration reproduzível do catálogo e testes de domínio, aplicação, API e integração, incluindo isolamento entre Tenants e unidades.

## Capabilities

### New Capabilities

Nenhuma.

### Modified Capabilities

- `catalog/product-catalog`: acrescenta os comportamentos observáveis de gerenciamento administrativo e consulta pública hierárquica do catálogo.

## Impact

- Afeta Domain, Application, Infrastructure, Contracts e API no módulo de catálogo.
- Adiciona tabelas, constraints, índices e migration do catálogo no PostgreSQL existente.
- Adiciona SQL de leitura exclusivamente em Infrastructure por meio de Dapper.
- Amplia os testes unitários, arquiteturais e de integração existentes, sem adicionar MediatR, AutoMapper, mensageria ou banco de leitura separado.
