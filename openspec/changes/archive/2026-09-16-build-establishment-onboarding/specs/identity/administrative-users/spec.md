## ADDED Requirements

### Requirement: Onboarding mantém acesso inicial da unidade
O sistema SHALL permitir que um administrador autorizado associe usuários do mesmo Tenant à unidade e MUST preservar ao menos um administrador ativo capaz de manter acessos.

#### Scenario: Associação inicial válida
- **WHEN** o responsável associa usuário ativo e papel permitido à nova unidade
- **THEN** a associação passa a compor a prontidão e as futuras autorizações da unidade
