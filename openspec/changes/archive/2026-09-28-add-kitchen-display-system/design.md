## Context

As transições de pedido já pertencem ao domínio; o KDS é uma projeção e experiência operacional especializada. Consulte proposal.md para motivação e os delta specs para os contratos observáveis.

## Goals / Non-Goals

**Goals:** preservar CQRS, isolamento Multi-Tenant, contratos explícitos, I/O assíncrono e evolução incremental.

**Non-Goals:** introduzir microservices, MediatR, AutoMapper ou mensageria externa sem necessidade especificada.

## Decisions

Criar queries Dapper próprias da fila, commands existentes ou específicos para transições válidas e UI responsiva por cartões/etapas. Nenhuma regra de status fica no frontend. Atualização começa por polling e pode consumir realtime quando disponível.

## Risks / Trade-offs

[Risco] Fila extensa → paginação/janelas e índices. [Risco] Ação concorrente → controle otimista e recarga do item.

## Migration Plan

Entregar leitura primeiro, depois ações e por fim alertas; rollback remove a rota sem afetar pedidos. Alterações de banco devem ser compatíveis com a versão anterior durante a implantação e possuir caminho de rollback.
