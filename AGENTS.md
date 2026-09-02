# AGENTS.md

# OrderHub — Codex Development Rules

Este arquivo contém regras obrigatórias para qualquer agente que altere este repositório.

Antes de criar, alterar ou remover código:

1. Leia este arquivo completamente.
2. Leia `openspec/project.md`.
3. Leia `openspec/architecture.md`.
4. Leia `openspec/conventions.md`.
5. Localize a spec correspondente em `openspec/specs/`.
6. Não implemente requisitos que não estejam na spec atual.
7. Não altere decisões arquiteturais sem registrar um ADR.

---

# 1. Arquitetura

A solução utiliza:

- .NET 10
- C#
- PostgreSQL
- Entity Framework Core para escrita
- Dapper para leitura
- Vue.js
- Quasar
- TypeScript
- Docker

Princípios arquiteturais:

- Hexagonal Architecture
- Clean Architecture
- DDD
- CQRS
- SOLID
- Clean Code
- Modular Monolith

---

# 2. Proibições

É proibido utilizar:

- MediatR
- AutoMapper

Não adicionar nenhuma dessas bibliotecas direta ou indiretamente.

Também é proibido:

- colocar regra de negócio em Controllers;
- acessar DbContext diretamente de Controllers;
- acessar Dapper diretamente de Controllers;
- retornar entidades de domínio pela API;
- colocar SQL no projeto Domain;
- referenciar EF Core no Domain;
- referenciar Dapper no Domain;
- referenciar Infrastructure no Domain;
- colocar regra de domínio em Repository;
- utilizar Service Locator;
- utilizar dependências estáticas para resolver serviços;
- criar abstrações sem necessidade real;
- criar microservices;
- introduzir mensageria sem uma spec específica.

---

# 3. Regra fundamental

Controllers devem ser finos.

Fluxo de escrita:

HTTP
→ Controller
→ CommandDispatcher
→ Validator
→ CommandHandler
→ Domain
→ Write Gateway / Repository
→ EF Core
→ PostgreSQL

Fluxo de leitura:

HTTP
→ Controller
→ QueryDispatcher
→ Validator quando necessário
→ QueryHandler
→ Read Gateway
→ Dapper
→ PostgreSQL
→ Read Model

Nunca misturar os dois fluxos.

---

# 4. CQRS

Commands alteram estado.

Queries nunca alteram estado.

Commands não devem existir para simplesmente consultar informações.

Queries não devem executar INSERT, UPDATE ou DELETE.

Utilizar:

- ICommand
- ICommand<TResult>
- ICommandHandler<TCommand>
- ICommandHandler<TCommand, TResult>
- ICommandDispatcher
- IQuery<TResult>
- IQueryHandler<TQuery, TResult>
- IQueryDispatcher

Os Dispatchers são implementados pelo próprio projeto.

Não utilizar MediatR.

---

# 5. Validation

Utilizar FluentValidation.

Validações de entrada devem ficar nos Validators.

Exemplos:

- campo obrigatório;
- tamanho;
- formato;
- range;
- formato de e-mail;
- IDs inválidos.

Regras de negócio pertencem ao domínio.

Exemplo:

ERRADO:

OrderCommandValidator:
    pedido não pode ser cancelado porque já saiu para entrega.

CORRETO:

Order.Cancel()

A entidade/agregado deve proteger essa regra.

---

# 6. Domain

O Domain deve permanecer independente.

Pode conter:

- Aggregates
- Entities
- Value Objects
- Domain Services
- Domain Events
- Domain Exceptions
- Specifications quando necessário

O Domain não pode conhecer:

- EF Core
- Dapper
- PostgreSQL
- HTTP
- Controllers
- Swagger
- Redis
- Docker
- Vue
- Quasar
- serviços externos

---

# 7. Persistência de escrita

Entity Framework Core é utilizado para escrita.

Repositories devem representar operações relevantes ao domínio.

Evitar interfaces genéricas como:

IRepository<TEntity>
{
    Add
    Update
    Delete
    GetAll
}

quando forem apenas uma cópia do DbSet.

Preferir:

IOrderRepository
{
    GetForUpdateAsync(...)
    AddAsync(...)
}

Abstrações genéricas são permitidas somente quando representarem comportamento realmente comum.

---

# 8. Persistência de leitura

Dapper é utilizado para leitura.

Queries podem:

- usar joins;
- utilizar CTE;
- fazer projections;
- retornar DTOs específicos;
- utilizar agregações;
- aplicar filtros;
- aplicar paginação.

Não carregar Aggregate Roots apenas para montar consultas.

---

# 9. Multi-Tenant

Toda funcionalidade pertencente a um estabelecimento deve considerar TenantId.

Nunca confiar em TenantId enviado pelo cliente para autorização.

O Tenant deve ser obtido de contexto autenticado ou mecanismo equivalente.

Toda consulta deve garantir isolamento entre Tenants.

Nunca permitir acesso cruzado de dados.

---

# 10. API

Controllers:

- recebem requests;
- executam Dispatcher;
- retornam resultado HTTP.

Controllers NÃO:

- executam regra de negócio;
- acessam banco;
- criam queries SQL;
- executam EF diretamente;
- fazem mapeamentos complexos.

---

# 11. Exceptions

Não espalhar try/catch pelos Controllers.

Exceptions são tratadas pelo middleware global.

Utilizar ProblemDetails.

Tipos esperados:

- ValidationException
- DomainException
- NotFoundException
- ConflictException
- ForbiddenException
- UnauthorizedException

---

# 12. Mapping

AutoMapper é proibido.

Mapeamentos simples devem ser explícitos.

Preferir:

ProductResponse.From(product)

ou

new ProductResponse(...)

Mapeamentos devem permanecer legíveis.

---

# 13. Generics

Antes de criar uma abstração genérica, pergunte:

"Existem pelo menos dois casos reais com exatamente o mesmo comportamento?"

Se a resposta for não, não criar generic prematuramente.

Não criar abstrações especulativas.

---

# 14. Async

Toda operação de I/O deve utilizar async/await.

Propagar CancellationToken.

Exemplo:

Task<Order?> GetAsync(
    Guid id,
    CancellationToken cancellationToken);

Não utilizar:

.Result
.Wait()

---

# 15. Código moderno

Utilizar recursos modernos do C# quando aumentarem clareza.

Preferir:

- records para contracts imutáveis;
- nullable reference types;
- required quando adequado;
- primary constructors quando melhorarem legibilidade;
- pattern matching;
- file-scoped namespaces.

Não utilizar recursos modernos apenas por estética.

---

# 16. Testes

Para cada regra relevante adicionar testes.

Domain:
- regras;
- invariantes;
- transições de estado.

Application:
- handlers;
- validators.

Infrastructure:
- integration tests quando necessário.

API:
- integration tests para fluxos importantes.

Não escrever testes que apenas reproduzam a implementação.

---

# 17. Feature Workflow

Ao implementar uma feature:

1. localizar a spec;
2. identificar bounded context;
3. identificar Command ou Query;
4. criar contrato;
5. criar Validator;
6. criar Handler;
7. criar abstrações necessárias;
8. implementar Adapter;
9. criar endpoint;
10. criar testes;
11. executar testes;
12. executar build;
13. atualizar documentação quando necessário.

---

# 18. Antes de criar código novo

Pesquise primeiro no projeto.

Nunca criar uma classe, interface, helper ou abstração sem verificar se já existe equivalente.

Reutilizar código existente quando semanticamente apropriado.

Não criar duplicações com nomes diferentes.

---

# 19. Mudanças arquiteturais

Se uma tarefa exigir mudança arquitetural:

NÃO alterar silenciosamente.

Criar primeiro:

openspec/decisions/ADR-XXX-<descricao>.md

Explicar:

- Contexto
- Problema
- Opções
- Decisão
- Consequências

Somente então implementar.

---

# 20. Definition of Done

Uma feature só está pronta quando:

- [ ] Compila sem erros
- [ ] Testes existentes continuam passando
- [ ] Novos testes foram adicionados quando necessários
- [ ] Não viola dependências arquiteturais
- [ ] Não introduz warnings relevantes
- [ ] CancellationToken foi propagado
- [ ] Validação foi implementada
- [ ] Tenant isolation foi considerada
- [ ] Tratamento de erro está padronizado
- [ ] API não retorna entidades
- [ ] Código duplicado relevante não foi introduzido
- [ ] Spec foi atendida completamente

"Funciona" não significa "pronto".