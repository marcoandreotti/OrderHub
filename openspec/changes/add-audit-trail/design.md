## Context

Auditoria cruza módulos administrativos e não deve depender de lógica nos Controllers. Consulte proposal.md para motivação e os delta specs para os contratos observáveis.

## Goals / Non-Goals

**Goals:** preservar CQRS, isolamento Multi-Tenant, contratos explícitos, I/O assíncrono e evolução incremental.

**Non-Goals:** introduzir microservices, MediatR, AutoMapper ou mensageria externa sem necessidade especificada.

## Decisions

Definir porta de auditoria na Application e adapter append-only na Infrastructure. Handlers ou pipeline explícito fornecem operação e alvo; middleware captura negações na borda. Consultas usam Dapper e autorização dedicada.

## Risks / Trade-offs

[Risco] Auditoria e negócio divergirem → eventos de sucesso participam da transação ou outbox quando aplicável. [Risco] Dados sensíveis → metadados por allowlist.

## Migration Plan

Criar armazenamento e leitura, instrumentar ações por lotes e validar cobertura antes de exigir auditoria obrigatória. Alterações de banco devem ser compatíveis com a versão anterior durante a implantação e possuir caminho de rollback.
