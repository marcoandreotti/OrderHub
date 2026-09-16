## ADDED Requirements

### Requirement: API oferece configuração administrativa da unidade
A API SHALL oferecer consultas e comandos explícitos para progresso do onboarding, dados/tema, horários, mesas, tokens e associações, usando dispatchers e políticas específicas.

#### Scenario: Falha em etapa
- **WHEN** uma alteração válida estruturalmente viola regra de domínio ou conflito persistente
- **THEN** a API retorna ProblemDetails apropriado e não persiste estado parcial
