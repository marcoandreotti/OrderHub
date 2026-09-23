## Context

Observabilidade é transversal, mas Domain deve permanecer independente e dados multi-tenant não podem vazar. Consulte proposal.md para motivação e os delta specs para os contratos observáveis.

## Goals / Non-Goals

**Goals:** preservar CQRS, isolamento Multi-Tenant, contratos explícitos, I/O assíncrono e evolução incremental.

**Non-Goals:** introduzir microservices, MediatR, AutoMapper ou mensageria externa sem necessidade especificada.

## Decisions

Instrumentar nas bordas API/Infrastructure com OpenTelemetry e logs estruturados. Propagar correlation id por contexto explícito. Usar métricas de baixa cardinalidade e endpoints separados de health. Exportadores são configuração de implantação.

## Risks / Trade-offs

[Risco] Custo e cardinalidade → limites e amostragem configuráveis. [Risco] PII em logs → allowlist de campos e testes.

## Migration Plan

Ativar primeiro logs/correlação, depois health, métricas e traces; falha do exportador não derruba a aplicação. Alterações de banco devem ser compatíveis com a versão anterior durante a implantação e possuir caminho de rollback.
