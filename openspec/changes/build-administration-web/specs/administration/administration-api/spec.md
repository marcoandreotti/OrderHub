## ADDED Requirements

### Requirement: API oferece gestão paginada de usuários
A API SHALL oferecer consulta paginada e filtrável de usuários administrativos e operações explícitas para cadastro, estado, papéis e associações, sempre limitadas ao Tenant autenticado.

#### Scenario: Pesquisa por unidade
- **WHEN** um administrador pesquisa usuários associados a uma unidade autorizada
- **THEN** a API retorna apenas usuários do mesmo Tenant que satisfazem o filtro

#### Scenario: Entrada estruturalmente inválida
- **WHEN** uma operação de usuário recebe campos inválidos
- **THEN** a API retorna ProblemDetails de validação sem executar alteração parcial
