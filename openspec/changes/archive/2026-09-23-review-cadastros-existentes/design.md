## Context

A web administrativa já possui páginas, contratos e APIs de cadastro; a falha deve ser localizada sem ampliar o domínio. Consulte proposal.md para motivação e os delta specs para os contratos observáveis.

## Goals / Non-Goals

**Goals:** preservar CQRS, isolamento Multi-Tenant, contratos explícitos, I/O assíncrono e evolução incremental.

**Non-Goals:** introduzir microservices, MediatR, AutoMapper ou mensageria externa sem necessidade especificada.

## Decisions

Mapear cada formulário como máquina de estados simples: inicial, inválido, pronto, enviando, sucesso e erro. Centralizar interpretação de ProblemDetails e manter o servidor como autoridade. Testes de componente cobrem habilitação e testes de integração cobrem o contrato real.

## Risks / Trade-offs

[Risco] Correção mascarar incompatibilidade de API → comparar requests com os contratos e corrigir a origem. [Risco] Regressão em edição → cobrir criação e edição separadamente.

## Migration Plan

Liberar por fluxo de cadastro, mantendo rollback apenas da camada web quando não houver ajuste de contrato. Alterações de banco devem ser compatíveis com a versão anterior durante a implantação e possuir caminho de rollback.
