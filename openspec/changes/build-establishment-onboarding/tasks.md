## 1. Contratos e prontidão

- [ ] 1.1 Confirmar autenticação e shell administrativo disponíveis e inventariar Tenancy, Identity e Operations existentes; verificar reuso e dependências antes da implementação.
- [ ] 1.2 Definir read model de progresso e critérios calculados de prontidão sem duplicar invariantes; verificar unidades novas, existentes, completas e regressão de prontidão em testes.
- [ ] 1.3 Definir contratos explícitos das etapas e chaves de intenção quando necessárias; verificar que nenhum payload usa TenantId para autorização.

## 2. Configuração da unidade

- [ ] 2.1 Implementar Commands/Queries, validators e handlers para dados e tema da unidade; verificar slug duplicado, fallback e isolamento em testes.
- [ ] 2.2 Implementar substituição atômica da grade de horários; verificar intervalos inválidos, dias fechados e rollback integral.
- [ ] 2.3 Implementar gestão de mesas e rotação de tokens opacos; verificar unicidade, revogação imediata e combinação slug/token.
- [ ] 2.4 Integrar associações iniciais de usuários preservando administrador elegível; verificar papéis, unidade cruzada e último administrador.

## 3. Persistência e API

- [ ] 3.1 Implementar adapters EF/Dapper e eventual persistência aditiva de progresso/idempotência; verificar constraints e isolamento com dois Tenants.
- [ ] 3.2 Criar migration incremental quando necessária e verificar upgrade, rollback e reapply sobre banco vazio e schema existente.
- [ ] 3.3 Mapear endpoints finos por capacidade e consulta composta de progresso; verificar políticas, ProblemDetails e ausência de alterações parciais em testes HTTP.
- [ ] 3.4 Fornecer URL pública segura para QR Code sem IDs internos; verificar conteúdo, renovação e invalidação do token anterior.

## 4. Wizard administrativo

- [ ] 4.1 Criar rotas e estado retomável por etapa dentro da administração; verificar recarga, avanço, retorno e unidade parcialmente configurada.
- [ ] 4.2 Implementar telas de dados/tema, horários, mesas/QR e acessos; verificar validação, responsividade e acessibilidade.
- [ ] 4.3 Implementar revisão e conclusão com revalidação da prontidão; verificar bloqueio de conclusão e sucesso sem duplicar recursos.

## 5. Verificação final

- [ ] 5.1 Cobrir percurso completo em testes frontend, Application, integração e API, incluindo retry idempotente e retomada.
- [ ] 5.2 Executar typecheck, builds web/backend, migrations e todas as suítes relevantes; verificar zero erros/warnings relevantes e revisar contra specs, design e AGENTS.md.
