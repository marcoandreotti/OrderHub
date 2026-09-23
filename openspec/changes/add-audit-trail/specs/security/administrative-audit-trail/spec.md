## Purpose

Preserva evidências tenant-scoped de ações administrativas relevantes para investigação, segurança e responsabilização operacional.

## ADDED Requirements

### Requirement: Evento de auditoria é imutável e contextualizado
Cada ação selecionada MUST registrar ator, Tenant, unidade quando aplicável, operação, alvo, instante, resultado e correlação, sem permitir alteração administrativa posterior.

#### Scenario: Operação negada
- **WHEN** uma ação sensível é recusada por autorização
- **THEN** a tentativa é auditada sem revelar o recurso ao solicitante

### Requirement: Consulta de auditoria é restrita
O sistema SHALL oferecer consulta paginada somente a papéis autorizados e MUST aplicar filtros tenant-scoped no servidor.

#### Scenario: Consulta cruzada
- **WHEN** um usuário tenta filtrar outro Tenant
- **THEN** nenhum evento externo é retornado nem sua existência revelada
