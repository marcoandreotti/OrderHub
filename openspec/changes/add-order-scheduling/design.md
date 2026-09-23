## Context

Agendamento combina disponibilidade, capacidade e compromisso temporal, sem justificar um aggregate de pedido separado. Consulte proposal.md para motivação e os delta specs para os contratos observáveis.

## Goals / Non-Goals

**Goals:** preservar CQRS, isolamento Multi-Tenant, contratos explícitos, I/O assíncrono e evolução incremental.

**Non-Goals:** introduzir microservices, MediatR, AutoMapper ou mensageria externa sem necessidade especificada.

## Decisions

Modelar política e slots no módulo Ordering. Consultas calculam disponibilidade; confirmação reserva capacidade na mesma transação do pedido. Armazenar UTC e fuso da unidade. O dashboard usa janela de produção, não muda status automaticamente.

## Risks / Trade-offs

[Risco] Concorrência no último slot → restrição transacional. [Risco] Alteração de horário → pedidos confirmados permanecem e geram destaque operacional.

## Migration Plan

Aplicar após disponibilidade, iniciar com capacidade opcional ilimitada e habilitar por modalidade/unidade. Alterações de banco devem ser compatíveis com a versão anterior durante a implantação e possuir caminho de rollback.
