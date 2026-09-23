## Context

O painel usa polling e a API é o ponto autorizado para pedidos. A conexão deve preservar autenticação e isolamento por unidade. Consulte proposal.md para motivação e os delta specs para os contratos observáveis.

## Goals / Non-Goals

**Goals:** preservar CQRS, isolamento Multi-Tenant, contratos explícitos, I/O assíncrono e evolução incremental.

**Non-Goals:** introduzir microservices, MediatR, AutoMapper ou mensageria externa sem necessidade especificada.

## Decisions

Usar SignalR na borda da API, grupos por unidade validados no servidor e mensagens DTO versionáveis. Emitir somente após persistência confirmada. O evento invalida ou atualiza projeções; reconexão sempre reconcilia por query. Polling permanece fallback.

## Risks / Trade-offs

[Risco] Perda ou duplicação de eventos → eventos são sinais e a query é autoritativa. [Risco] Escala futura → manter abstração de publicação interna sem adicionar broker agora.

## Migration Plan

Implantar hub e telemetria, habilitar cliente com fallback e permitir rollback para polling. Alterações de banco devem ser compatíveis com a versão anterior durante a implantação e possuir caminho de rollback.
