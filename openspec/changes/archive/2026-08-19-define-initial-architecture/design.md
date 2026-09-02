## Context

O repositório contém a visão do produto e regras mandatórias, mas ainda não contém a estrutura executável da solução. Consulte `proposal.md` para a motivação e `specs/architecture/solution-foundation/spec.md` para o contrato verificável. O desenho precisa acomodar uma plataforma SaaS para alimentação sem antecipar toda a complexidade funcional descrita na visão.

Restrições principais: .NET 10, PostgreSQL, EF Core para escrita, Dapper para leitura, Vue.js, Quasar, TypeScript e Docker; MediatR e AutoMapper são proibidos. Toda I/O será assíncrona com `CancellationToken`, o Domain permanecerá independente e mudanças arquiteturais posteriores exigirão ADR.

## Goals / Non-Goals

**Goals:**

- Definir limites de solução, módulos e dependências que possam ser protegidos por testes arquiteturais.
- Criar uma espinha dorsal CQRS pequena e explícita, resolvida pelo DI nativo e integrada ao FluentValidation.
- Tratar Multi-Tenancy, segurança, erros, auditoria e observabilidade como capacidades transversais desde a fundação.
- Permitir entregas verticais incrementais, executáveis e testáveis.

**Non-Goals:**

- Implementar nesta mudança catálogo, carrinho, pedidos, pagamentos, relatórios ou IA.
- Criar microservices, broker, Outbox, Redis, SignalR, SQLite ou integrações com fornecedores na fundação.
- Definir antecipadamente todos os aggregates e eventos de fases futuras.
- Criar repositories ou gateways genéricos sem ao menos dois casos reais equivalentes.

## Decisions

### 1. Monólito modular com projetos por responsabilidade e módulos por bounded context

A solução começa com poucos projetos físicos e módulos verticais internos. Isso evita uma explosão de assemblies por bounded context, mas preserva fronteiras que testes arquiteturais conseguem fiscalizar.

```text
OrderHub.sln
├── src/
│   ├── OrderHub.Domain/
│   │   ├── SharedKernel/
│   │   └── Modules/{Tenancy,Identity,Catalog,Orders,...}/
│   ├── OrderHub.Application/
│   │   ├── Abstractions/{Commands,Queries,Validation,Tenancy,Clock}/
│   │   └── Modules/<Module>/{Commands,Queries,Contracts}/
│   ├── OrderHub.Infrastructure/
│   │   ├── Persistence/{Write,Read,Migrations}/
│   │   ├── Identity/
│   │   ├── Observability/
│   │   └── Modules/<Module>/
│   ├── OrderHub.Contracts/
│   └── OrderHub.Api/
├── web/OrderHub.Web/
├── tests/
│   ├── OrderHub.Domain.Tests/
│   ├── OrderHub.Application.Tests/
│   ├── OrderHub.Integration.Tests/
│   └── OrderHub.Architecture.Tests/
├── docker/
│   ├── api/Dockerfile
│   └── web/Dockerfile
├── docs/
│   └── decisions/
├── scripts/
├── docker-compose.yml
└── README.md
```

Dependências permitidas:

```text
Domain                 → nenhuma camada externa
Application            → Domain
Infrastructure         → Application + Domain
Contracts              → sem dependência de Domain
API (composition root) → Application + Infrastructure + Contracts
Web                    → contratos HTTP publicados, nunca assemblies .NET
```

Alternativa considerada: um conjunto `Domain/Application/Infrastructure` por módulo. Foi adiada porque aumenta projetos e cerimônia antes de existirem módulos com necessidade de ciclo independente. A extração continua possível se métricas e acoplamento justificarem.

### 2. Bounded contexts incrementais

Primeira versão estrutural:

- **Tenancy**: Tenant, estabelecimento, configurações e tema; estabelece isolamento.
- **Identity**: usuários, credenciais, refresh tokens, roles, claims e policies.
- **Catalog**: categorias recursivas, produtos, variações, adicionais, combos, preço e disponibilidade.
- **Customers**: identidade do consumidor, contatos e endereços.
- **Orders**: carrinho confirmado, pedido, itens, atendimento, estado e transições.

Fases posteriores, criadas somente com specs próprias:

- **Coupons**, **Payments**, **Delivery**, **Notifications**, **Reporting** e **AI**.

Aggregates iniciais candidatos são `Tenant`, `User`, `CatalogProduct` e `Order`. Value Objects candidatos incluem `TenantId`, `Money`, `Email`, `PhoneNumber`, `Address`, `OrderNumber` e `TimeRange`. `Order` protege transições; políticas que dependam de múltiplos aggregates podem usar Domain Services. Domain Events serão criados apenas quando houver consumidor real ou necessidade de desacoplamento, por exemplo `OrderConfirmed` e `OrderCancelled`.

Alternativa considerada: modelar todos os contexts e eventos desde o início. Rejeitada por criar abstrações especulativas.

### 3. CQRS explícito com dispatchers próprios

```text
Write:
HTTP → Controller → CommandDispatcher → Validators → CommandHandler
     → Domain → Write Port → EF Core → PostgreSQL

Read:
HTTP → Controller → QueryDispatcher → Validators (quando aplicável)
     → QueryHandler → Read Port → Dapper → PostgreSQL → Read Model
```

Os dispatchers resolvem um único handler pelo DI nativo, executam todos os validators registrados antes do handler e propagam `CancellationToken`. Commands e Queries são records imutáveis quando isso melhorar clareza. Controllers apenas constroem contratos, despacham e traduzem o sucesso em HTTP; exceções seguem ao middleware global.

Alternativa considerada: MediatR. Rejeitada por regra explícita e porque a abstração necessária é pequena. Pipelines genéricos adicionais só serão criados após dois comportamentos reais compartilhados.

### 4. Persistência PostgreSQL compartilhada, modelos de acesso separados

EF Core implementa portas de escrita e migrations. Dapper implementa portas de leitura com SQL localizado em Infrastructure. Ambos usam PostgreSQL e a mesma transação lógica de dados; não haverá banco de leitura separado inicialmente.

Estratégia de schema: um banco, tabelas agrupadas por schema de módulo (`tenancy`, `identity`, `catalog`, `orders`) quando isso trouxer clareza. Chaves pertencentes a estabelecimentos incluem `tenant_id`. Índices e unicidades começam por:

- `(tenant_id, id)` nas entidades tenant-scoped;
- unicidade de slug/código externo dentro do Tenant;
- `(tenant_id, status, created_at)` em pedidos;
- `(tenant_id, category_id, is_available)` no catálogo;
- `(tenant_id, idempotency_key, operation)` para idempotência;
- refresh tokens por usuário, expiração e revogação.

O Tenant é resolvido de host/rota e identidade autenticada, reconciliado no servidor e disponibilizado por `ITenantContext`. Nenhuma autorização confia no TenantId do payload. Filtros globais do EF podem ser defesa adicional, nunca o único mecanismo; SQL Dapper deve conter filtro explícito e testes de isolamento.

SQLite foi rejeitado para a fundação: PostgreSQL já cobre desenvolvimento via Docker e SQLite criaria diferenças sem um caso offline ou cache local concreto.

### 5. Segurança, idempotência, auditoria e observabilidade transversais

JWT de curta duração, refresh token rotativo armazenado de forma segura, roles e policies protegem áreas autenticadas. A área pública resolve Tenant, mas não ganha privilégios administrativos. Senhas usam o hasher fornecido pelo ecossistema ASP.NET Core.

Um middleware global converte exceções conhecidas em ProblemDetails. Correlation ID entra ou é criado na borda e segue logs estruturados. Health checks cobrem processo e PostgreSQL. A instrumentação mantém pontos de extensão para OpenTelemetry sem exigir backend de telemetria na fase inicial.

Idempotência será uma capacidade de aplicação/persistência para operações críticas e terá escopo por Tenant, operação e chave. Auditoria registra ator, Tenant, correlation ID e mudanças relevantes; ela não substitui logs técnicos.

Alternativa considerada: Redis para idempotência. Adiada porque a garantia durável pode começar no PostgreSQL e evita novo serviço operacional.

### 6. Frontend Vue/Quasar modular e mobile first

```text
web/OrderHub.Web/src/
├── boot/                 # HTTP, autenticação e inicialização
├── layouts/              # Public, Operations, Administration
├── router/               # rotas e guards
├── modules/
│   ├── public-menu/
│   ├── auth/
│   ├── operations/
│   └── administration/
├── components/
│   ├── base/             # AppButton, AppInput, AppDialog...
│   └── domain/           # ProductCard, CategoryTree, OrderTimeline...
├── composables/
├── services/             # cliente HTTP e adapters
├── stores/               # Pinia apenas para estado global real
├── models/
└── themes/               # tokens e TenantTheme
```

O cliente HTTP centraliza base URL, bearer token, correlation ID e tratamento consistente de ProblemDetails. Stores não replicam todo estado do servidor. Componentes recursivos são usados para hierarquias, com proteção contra ciclos vindos de dados inválidos. Tokens CSS derivados de `TenantTheme` aplicam branding; componentes não carregam cores de Tenant diretamente.

PWA será apenas uma possibilidade arquitetural; service worker, offline e notificações exigirão specs futuras.

### 7. Topologia Docker mínima

```text
Browser
  │
  ▼
Vue/Quasar (web)
  │ HTTP
  ▼
ASP.NET Core API
  │
  ▼
PostgreSQL (volume persistente)
```

`docker-compose.yml` fornece rede interna, volume nomeado, health checks e configuração via variáveis de ambiente. O frontend não acessa PostgreSQL. Segredos reais ficam fora do repositório; `.env.example` documenta apenas nomes e valores seguros de desenvolvimento.

### 8. Roadmap vertical e verificável

1. **Fase 0 — Fundação:** solution, dispatchers, validation, ProblemDetails, observabilidade mínima, testes arquiteturais, web shell e Docker.
2. **Fase 1 — Tenancy + Identity:** resolução de Tenant, autenticação, refresh token, policies, usuários e tema básico.
3. **Fase 2 — Catálogo:** categorias recursivas, produtos, preços, adicionais e cardápio público.
4. **Fase 3 — Carrinho:** composição e validação do carrinho no frontend, ainda sem pedido persistido.
5. **Fase 4 — Pedidos:** criação idempotente, aggregate e acompanhamento básico.
6. **Fase 5 — Operação:** confirmação, rejeição, produção, entrega/retirada e histórico.
7. **Fase 6 — Administração:** configurações do estabelecimento e gestão ampliada.
8. **Fase 7 — Cupons:** regras promocionais e auditoria de uso.
9. **Fase 8 — Relatórios:** read models e consultas agregadas.
10. **Fase 9 — IA:** ferramentas controladas, confirmação humana e autorização por contexto.
11. **Fase 10 — Integrações:** pagamentos, WhatsApp, impressão, delivery e notificações, cada qual com porta e spec próprias.

Cada fase exige build, testes de domínio/aplicação pertinentes, integração onde houver I/O e uma aplicação inicializável. Uma fase não cria infraestrutura das fases futuras apenas para “preparar”.

## Risks / Trade-offs

- [Um único banco permite acoplamento acidental entre módulos] → schemas, ownership explícito, portas e testes arquiteturais; consultas cruzadas somente em read models deliberados.
- [Filtro Multi-Tenant esquecido em SQL Dapper] → Tenant obrigatório na assinatura das portas, revisão de SQL e testes de integração com dois Tenants.
- [Dispatchers próprios crescerem como um framework interno] → manter apenas resolução, validação e cancelamento; novos behaviors exigem casos reais e decisão explícita.
- [Categorias recursivas gerarem ciclos ou consultas caras] → invariantes contra ciclos, índices e estratégia de leitura escolhida na spec de catálogo.
- [Escopo funcional amplo atrasar valor] → roadmap vertical, critérios de conclusão por fase e specs independentes.
- [JWT/refresh token ampliar superfície de segurança] → rotação, revogação, expiração curta, armazenamento seguro e testes de autorização.

## Migration Plan

Não há sistema legado nem dados a migrar. A aplicação da mudança começa pela Fase 0 e só avança após build, testes e execução Docker bem-sucedidos. Se a fundação não inicializar, o rollback consiste em reverter os artefatos criados nessa fase; migrations posteriores deverão possuir estratégia de rollback compatível com o ambiente antes de implantação.
