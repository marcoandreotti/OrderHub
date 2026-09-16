## ADDED Requirements

### Requirement: Unidade expõe configuração e prontidão administrativas
O sistema SHALL permitir que usuários autorizados consultem e alterem dados e tema da unidade e consultem sua prontidão calculada, sempre dentro do Tenant autenticado.

#### Scenario: Tema parcial válido
- **WHEN** o administrador salva somente tokens de tema permitidos
- **THEN** o sistema persiste os valores informados e mantém fallback para os demais

#### Scenario: Slug duplicado
- **WHEN** o administrador escolhe slug público já utilizado
- **THEN** o sistema rejeita com conflito sem alterar a unidade
