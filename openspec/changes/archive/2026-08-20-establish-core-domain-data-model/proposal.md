## Why

O OrderHub possui a fundação técnica, mas ainda não tem contratos verificáveis para o núcleo de negócio nem um modelo persistente capaz de sustentar a operação inicial de um estabelecimento. Definir agora os limites de domínio, relações, snapshots históricos e isolamento por Tenant evita que migrations e casos de uso cristalizem um modelo inconsistente.

## What Changes

- Define `Tenant` como grupo proprietário de uma ou mais unidades (`Establishment`) e estabelece o escopo de dados que pertencem à unidade.
- Define identidade administrativa com usuários, perfis operacionais e associações explícitas às unidades autorizadas dentro do Tenant.
- Define o catálogo hierárquico com categorias recursivas, produtos, imagens, variações, grupos de adicionais e adicionais reutilizáveis.
- Define clientes sem autenticação obrigatória, seus endereços e mesas identificadas por token público opaco.
- Define o aggregate `Order`, seus itens e adicionais como snapshots, tipos de atendimento, totais, transições de status e histórico operacional.
- Define cupons, formas de pagamento, pagamentos múltiplos por pedido e horários regulares de funcionamento.
- Define invariantes, relacionamentos, ownership entre aggregates, unicidades, integridade referencial e isolamento Multi-Tenant/Establishment.
- Planeja a persistência de escrita em EF Core e leitura em Dapper sobre o mesmo PostgreSQL, sem tabelas ou banco de leitura separados.
- Planeja um projeto exclusivo para migrations PostgreSQL dentro de Infrastructure, usado em design time/deployment e sem ser referenciado pelas camadas internas.
- Não inclui endpoints completos, telas, integração com adquirente, feriados/exceções de horário, banco de leitura separado, outbox ou mensageria.

## Capabilities

### New Capabilities

- `tenancy/establishment-management`: Tenant, unidades, identidade visual e regras de escopo e isolamento.
- `identity/administrative-users`: usuários administrativos, perfis operacionais e associações explícitas de acesso por Tenant e unidade.
- `catalog/product-catalog`: categorias hierárquicas, produtos, imagens, variações e adicionais.
- `customers/customer-records`: clientes sem conta obrigatória e endereços de entrega.
- `operations/service-configuration`: mesas com token público e horários regulares de funcionamento.
- `ordering/order-management`: aggregate de pedido, itens em snapshot, totais, estados e histórico.
- `promotions/coupon-management`: definição, elegibilidade, utilização e snapshot de cupons aplicados.
- `payments/order-payments`: formas de pagamento e pagamentos parciais ou múltiplos vinculados ao pedido.
- `persistence/core-postgresql-schema`: modelo relacional PostgreSQL, separação EF Core/Dapper e ciclo isolado de migrations.

### Modified Capabilities

- `architecture/solution-foundation`: concretiza o requisito transversal de isolamento Multi-Tenant para distinguir grupo Tenant de unidade operacional e formaliza o projeto isolado de migrations.

## Impact

- Afeta os módulos de Domain, Application, Infrastructure, API/Contracts e seus testes, além da composição da solution e do ambiente PostgreSQL.
- Introduz aggregates, entities, value objects, enums, portas de persistência, mapeamentos EF Core, SQL Dapper, constraints, índices e migrations iniciais.
- Exige reconciliação do Tenant autenticado, da unidade selecionada e da associação ativa do usuário à unidade; identificadores recebidos do cliente não concedem autorização.
- Preserva as restrições existentes: Domain independente, Controllers finos, CQRS próprio, EF Core somente para escrita, Dapper somente para leitura, sem MediatR, AutoMapper, mensageria ou microservices.
