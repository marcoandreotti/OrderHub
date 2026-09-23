## Context

Endereços existem e pedidos aceitam entrega, mas falta um módulo responsável por cobertura e execução. Consulte proposal.md para motivação e os delta specs para os contratos observáveis.

## Goals / Non-Goals

**Goals:** preservar CQRS, isolamento Multi-Tenant, contratos explícitos, I/O assíncrono e evolução incremental.

**Non-Goals:** introduzir microservices, MediatR, AutoMapper ou mensageria externa sem necessidade especificada.

## Decisions

Criar bounded context Delivery com regiões e políticas próprias. A cotação retorna token/referência curta, mas a confirmação recalcula dados críticos. O pedido armazena snapshot da entrega; acompanhamento referencia o pedido sem duplicar seu ciclo de vida.

## Risks / Trade-offs

[Risco] Endereço ambíguo → normalização e confirmação pelo cliente. [Risco] Polígonos complexos → primeira versão suporta critérios simples e extensíveis, sem fornecedor obrigatório.

## Migration Plan

Cadastrar políticas inativas, validar em modo administrativo, ativar cotação e por último exigir cobertura. Alterações de banco devem ser compatíveis com a versão anterior durante a implantação e possuir caminho de rollback.
