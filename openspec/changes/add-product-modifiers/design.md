## Context

O catálogo já possui variações e adicionais; a evolução deve preservar dados e pedidos históricos. Consulte proposal.md para motivação e os delta specs para os contratos observáveis.

## Goals / Non-Goals

**Goals:** preservar CQRS, isolamento Multi-Tenant, contratos explícitos, I/O assíncrono e evolução incremental.

**Non-Goals:** introduzir microservices, MediatR, AutoMapper ou mensageria externa sem necessidade especificada.

## Decisions

Generalizar grupos existentes de forma compatível, usando tipos e regras configuráveis em vez de subclasses por pizzaria. Preço e validação ficam no domínio. O pedido recebe snapshot detalhado e a web é dirigida por metadados.

## Risks / Trade-offs

[Risco] Modelo excessivamente genérico → limitar tipos aos casos aprovados. [Risco] Migração de adicionais → mapear dados atuais para o tipo padrão sem alterar comportamento.

## Migration Plan

Adicionar campos compatíveis e migrar grupos atuais, atualizar leitura/escrita, depois habilitar novos tipos. Alterações de banco devem ser compatíveis com a versão anterior durante a implantação e possuir caminho de rollback.
