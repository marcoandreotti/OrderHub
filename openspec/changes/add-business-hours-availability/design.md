## Context

A grade semanal existe, mas decisões de venda estão dispersas entre operação, catálogo e ordering. Consulte proposal.md para motivação e os delta specs para os contratos observáveis.

## Goals / Non-Goals

**Goals:** preservar CQRS, isolamento Multi-Tenant, contratos explícitos, I/O assíncrono e evolução incremental.

**Non-Goals:** introduzir microservices, MediatR, AutoMapper ou mensageria externa sem necessidade especificada.

## Decisions

Introduzir um serviço de domínio/aplicação de disponibilidade com precedência explícita: inatividade, exceção, pausa e grade. Persistir exceções e indisponibilidades tenant-scoped. Revalidar no command de confirmação; clients apenas exibem a decisão.

## Risks / Trade-offs

[Risco] Fuso e virada do dia → usar fuso configurado da unidade e instantes UTC. [Risco] Corrida no checkout → revalidação autoritativa.

## Migration Plan

Migrar mantendo grades atuais como padrão, adicionar administração e só então aplicar bloqueio na confirmação. Alterações de banco devem ser compatíveis com a versão anterior durante a implantação e possuir caminho de rollback.
