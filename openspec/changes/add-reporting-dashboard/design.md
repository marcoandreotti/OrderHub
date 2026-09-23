## Context

Relatórios são leitura e devem permanecer fora dos aggregates e do fluxo EF de escrita. Consulte proposal.md para motivação e os delta specs para os contratos observáveis.

## Goals / Non-Goals

**Goals:** preservar CQRS, isolamento Multi-Tenant, contratos explícitos, I/O assíncrono e evolução incremental.

**Non-Goals:** introduzir microservices, MediatR, AutoMapper ou mensageria externa sem necessidade especificada.

## Decisions

Criar read gateways Dapper com definições versionadas dos indicadores, filtros tenant-scoped e consultas agregadas. A web consome contratos específicos. Exportação reutiliza a mesma definição/filtros e aplica limites.

## Risks / Trade-offs

[Risco] Consultas caras → índices, limites de período e medição. [Risco] Ambiguidade financeira → documentar inclusão por estado e data.

## Migration Plan

Entregar indicadores básicos, medir planos de execução e adicionar comparações/exportação gradualmente. Alterações de banco devem ser compatíveis com a versão anterior durante a implantação e possuir caminho de rollback.
