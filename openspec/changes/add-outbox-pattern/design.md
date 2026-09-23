## Context

A solução é monólito modular com PostgreSQL; confiabilidade é necessária sem broker prematuro. Consulte proposal.md para motivação e os delta specs para os contratos observáveis.

## Goals / Non-Goals

**Goals:** preservar CQRS, isolamento Multi-Tenant, contratos explícitos, I/O assíncrono e evolução incremental.

**Non-Goals:** introduzir microservices, MediatR, AutoMapper ou mensageria externa sem necessidade especificada.

## Decisions

Adicionar tabela outbox e registrar mensagens na unidade de trabalho da escrita. Worker com leasing processa lotes, marca sucesso/falha e usa backoff. Payloads são contratos versionados, não entidades. Consumidores devem ser idempotentes.

## Risks / Trade-offs

[Risco] Crescimento da tabela → retenção e índices. [Risco] Ordem global falsa → garantir apenas ordem quando a chave de agregado exigir.

## Migration Plan

Implantar tabela e worker inativo, habilitar produtor/consumidor por caso; rollback pausa processamento sem perder mensagens. Alterações de banco devem ser compatíveis com a versão anterior durante a implantação e possuir caminho de rollback.
