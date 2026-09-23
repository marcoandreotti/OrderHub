## Context

O agente é uma nova interface sobre APIs públicas existentes e não pode substituir regras determinísticas. Consulte proposal.md para motivação e os delta specs para os contratos observáveis.

## Goals / Non-Goals

**Goals:** preservar CQRS, isolamento Multi-Tenant, contratos explícitos, I/O assíncrono e evolução incremental.

**Non-Goals:** introduzir microservices, MediatR, AutoMapper ou mensageria externa sem necessidade especificada.

## Decisions

Executar orquestração em módulo isolado com conjunto mínimo de tools tipadas. Toda mutação passa pelos commands públicos e confirmação humana. Sessões possuem Tenant resolvido por canal, limites, consentimento e trilha correlacionada.

## Risks / Trade-offs

[Risco] Alucinação → respostas factuais derivadas de tools e resumo autoritativo. [Risco] Prompt injection → não tratar conteúdo do catálogo/cliente como instrução. [Risco] custo → limites por sessão.

## Migration Plan

Começar somente leitura em ambiente controlado, habilitar carrinho e depois confirmação mediante métricas e revisão de segurança. Alterações de banco devem ser compatíveis com a versão anterior durante a implantação e possuir caminho de rollback.
