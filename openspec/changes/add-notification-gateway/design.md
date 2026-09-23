## Context

Fornecedores e canais variam; casos de uso não devem conhecer SDKs externos. Consulte proposal.md para motivação e os delta specs para os contratos observáveis.

## Goals / Non-Goals

**Goals:** preservar CQRS, isolamento Multi-Tenant, contratos explícitos, I/O assíncrono e evolução incremental.

**Non-Goals:** introduzir microservices, MediatR, AutoMapper ou mensageria externa sem necessidade especificada.

## Decisions

Definir porta de envio na Application, orquestração no módulo Communications e adapters na Infrastructure. Templates e preferências são tenant-scoped. Processamento assíncrono consome outbox; cada adapter traduz estados externos.

## Risks / Trade-offs

[Risco] Duplicidade → chave idempotente por finalidade. [Risco] Lock-in → contratos internos neutros e adapter por fornecedor.

## Migration Plan

Aplicar após outbox, iniciar com um canal/fornecedor em sandbox e ativar templates gradualmente. Alterações de banco devem ser compatíveis com a versão anterior durante a implantação e possuir caminho de rollback.
