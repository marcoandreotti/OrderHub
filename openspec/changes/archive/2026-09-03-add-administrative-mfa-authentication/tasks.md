## 1. Modelo e contratos de segurança

- [x] 1.1 Criar ADR para identidade global, código público de contexto, bootstrap e autorização cross-Tenant; verificar aprovação e coerência com a arquitetura antes de escrever código.
- [x] 1.2 Inventariar autenticação, usuários, hashing, options, startup e testes existentes; registrar o reuso escolhido e verificar que nenhuma abstração equivalente foi duplicada.
- [x] 1.3 Implementar código público único e normalizado do Tenant com migration e resolução segura; verificar duplicidade, normalização e dois Tenants em testes.
- [x] 1.4 Definir contratos de início/conclusão do login, renovação, troca de senha e logout sem expor TenantId, claims internas ou segredos; verificar serialização e OpenAPI em testes.
- [x] 1.5 Implementar identidade de plataforma separada, desafios e sessões com expiração, uso único, tentativas, rotação e troca obrigatória; verificar invariantes em testes de domínio/aplicação.

## 2. Casos de uso e entrega do código

- [x] 2.1 Implementar Commands/Queries, validators e handlers das duas etapas para contextos Tenant e plataforma com respostas não enumeráveis e CancellationToken; verificar código de contexto, credenciais, estado e MFA em testes de Application.
- [x] 2.2 Definir porta de entrega e adapter de e-mail configurável, com fake de testes e sem registrar conteúdo sensível; verificar envio, falha e cancelamento por testes.
- [x] 2.3 Implementar limites por identidade/origem e reenvio controlado sem bloqueio permanente; verificar janelas, concorrência e mensagens uniformes.
- [x] 2.4 Implementar troca obrigatória da senha temporária e gestão de superusuários exclusiva por pares, preservando o último ativo; verificar sessão restrita, revogação e negação a papéis tenant-scoped.

## 3. Persistência e sessão

- [x] 3.1 Mapear código do Tenant, identidades globais, desafios, sessões e famílias de renovação com EF Core, armazenando somente hashes de segredos; verificar metadata, constraints e ausência de texto aberto.
- [x] 3.2 Criar migration incremental de autenticação e verificar upgrade, rollback e reapply em PostgreSQL descartável.
- [x] 3.3 Implementar bootstrap idempotente do primeiro superusuário a partir de options/secrets validados; verificar primeira publicação, reinício, concorrência e configuração ausente.
- [x] 3.4 Implementar access token curto, refresh rotativo, sessão restrita, revogação e detecção de reutilização; verificar logout, troca de senha, usuário desativado e associação revogada em integração.

## 4. API e endurecimento

- [x] 4.1 Mapear endpoints finos de autenticação, troca de senha e superusuários via dispatchers e cookies seguros/CSRF conforme topologia; verificar status, cookies, policies e ProblemDetails por testes HTTP.
- [x] 4.2 Construir principals tenant-scoped ou globais somente de sessões persistidas, restringir o esquema existente a testes e verificar rejeição de Tenant, papel e escopo forjados em produção.
- [x] 4.3 Adicionar auditoria sem segredos e configuração validada na inicialização; verificar que senha, código e tokens não aparecem em logs de testes.

## 5. Verificação final

- [x] 5.1 Executar testes Domain, Application, Integration, Architecture e API, incluindo bootstrap concorrente, superusuário, sessão restrita e dois Tenants; verificar isolamento e zero falhas.
- [x] 5.2 Executar formatação e build da solution com zero erros/warnings relevantes e revisar o diff contra specs, design, AGENTS.md e proibições arquiteturais.
