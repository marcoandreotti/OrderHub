## 1. Domain e contratos

- [x] 1.1 Revisar e reutilizar `Tenant`, `Establishment`, `AdministrativeUser`, papéis, associações, autenticação e onboarding existentes; documentar no PR os pontos de extensão e verificar que nenhuma abstração equivalente foi duplicada.
- [x] 1.2 Estender o modelo de usuário administrativo para representar troca obrigatória de senha temporária e verificar em testes de domínio criação provisionada, troca válida e preservação dos usuários existentes.
- [x] 1.3 Modelar a intenção de provisionamento com ator global, chave, hash canônico, estado e resultado, e verificar invariantes de conclusão, repetição equivalente e reutilização incompatível.
- [x] 1.4 Definir contratos externos explícitos para provisionar Tenant, adicionar unidade, listar/consultar Tenants e recuperar intenção; verificar serialização, ausência de entidades/hashes e nenhuma autoridade derivada de `TenantId` enviado no provisionamento inicial.

## 2. Application e autorização

- [x] 2.1 Criar Commands, Queries e validators globais para provisionamento, unidade adicional e consultas; verificar campos obrigatórios, formatos, paginação, chave de intenção e propagação de `CancellationToken`.
- [x] 2.2 Implementar autorização de plataforma plena por identidade persistida, negando usuário tenant-scoped e sessão com troca pendente; verificar os três contextos em testes de Application.
- [x] 2.3 Implementar o handler atômico de Tenant, primeira unidade, primeiro Owner e associação, preservando unicidades e invariantes; verificar sucesso, rollback em cada falha e ausência de associação artificial do ator global.
- [x] 2.4 Implementar repetição idempotente por ator, chave e hash do request; verificar retorno dos mesmos IDs, conflito por payload diferente e concorrência com chaves iguais ou distintas.
- [x] 2.5 Implementar adição de unidade a Tenant existente com seleção de Owner ativo do mesmo Tenant; verificar Tenant inativo/inexistente, usuário sem papel, Owner cross-Tenant e onboarding pendente.
- [x] 2.6 Estender autenticação e troca de senha para Owner provisionado, mantendo o fluxo atual de plataforma; verificar sessão restrita, rotas permitidas, revogação e novo login tenant-scoped.

## 3. Persistência e migrations

- [x] 3.1 Implementar portas e adapter EF Core de transação/unidade de trabalho para persistir o provisionamento completo; verificar commit único e rollback integral em teste de integração PostgreSQL.
- [x] 3.2 Mapear intenção de provisionamento e marca de troca obrigatória com índices e constraints concorrentes; verificar unicidade por ator/chave, hash protegido e compatibilidade de usuários existentes.
- [x] 3.3 Criar migration incremental e verificar upgrade, rollback e reapply em banco vazio e em schema com Tenants, unidades e usuários existentes.
- [x] 3.4 Implementar read gateways Dapper para listagem paginada de Tenants, detalhes e recuperação de intenção; verificar ordenação estável, filtros, limites e ausência de credenciais nas projeções.
- [x] 3.5 Traduzir violações concorrentes de código público, slug, e-mail e intenção para conflitos padronizados; verificar que nenhuma exceção de persistência vaza pela API.

## 4. API global

- [x] 4.1 Criar política e grupo de endpoints globais de plataforma separados de `/api/admin`, usando dispatchers e contratos explícitos; verificar HTTP 401/403 para sessão ausente, tenant-scoped ou restrita.
- [x] 4.2 Expor provisionamento, unidade adicional, listagem/detalhe e consulta de intenção com `ProblemDetails`; verificar HTTP de sucesso, validação, conflito, recurso indisponível e retry idempotente em testes de API.
- [x] 4.3 Verificar OpenAPI e logs das rotas globais para garantir que senha temporária, hashes, tokens e dados sensíveis não sejam retornados nem registrados.

## 5. Interface de plataforma

- [x] 5.1 Adicionar rota, guard, layout e navegação próprios da plataforma; verificar estado vazio para banco sem Tenant e acesso negado para usuário tenant-scoped.
- [x] 5.2 Implementar client e store globais para listagem, provisionamento, unidade adicional e recuperação por chave de intenção; verificar paginação, retry após resposta desconhecida e interpretação de `ProblemDetails`.
- [x] 5.3 Implementar formulário acessível de Tenant, primeira unidade e primeiro Owner com validação e preservação dos dados; verificar válido, inválido, conflito, envio único e responsividade em testes de componente.
- [x] 5.4 Integrar sucesso com seleção da unidade e onboarding, e retorno à área global com limpeza de estado tenant-scoped; verificar que dados da unidade anterior não permanecem visíveis.

## 6. Verificação final

- [x] 6.1 Cobrir o percurso ponta a ponta em PostgreSQL real: primeiro login `PLATFORM`, provisionamento, login restrito do Owner, troca de senha e entrada no onboarding; verificar que os cadastros administrativos ficam disponíveis após selecionar a unidade.
- [x] 6.2 Cobrir dois Tenants, unidade adicional, Owner cross-Tenant, retries e concorrência; verificar isolamento, atomicidade e preservação de ao menos um Owner ativo.
- [x] 6.3 Executar builds backend/web, typecheck, testes Domain, Application, Integration, Architecture e frontend; verificar zero falhas, ausência de warnings relevantes, proibição de MediatR/AutoMapper e propagação de `CancellationToken`.
- [x] 6.4 Atualizar documentação operacional do primeiro acesso e provisionamento, incluindo entrega segura da senha temporária e rollback; revisar a implementação contra specs, design, ADR-002 e Definition of Done.
