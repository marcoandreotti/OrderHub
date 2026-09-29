## ADDED Requirements

### Requirement: API expõe administração global de Tenants
A API SHALL oferecer rotas globais explícitas para listar e consultar Tenants, provisionar Tenant com primeira unidade e primeiro Owner e adicionar unidade a Tenant existente. Essas rotas MUST exigir identidade de plataforma plenamente autenticada, contratos externos explícitos e resolução server-side do alvo, e MUST permanecer separadas das rotas administrativas tenant-scoped.

#### Scenario: Provisionamento global válido
- **WHEN** um `PlatformSuperUser` envia contrato válido e chave de intenção inédita
- **THEN** a API retorna o resultado do provisionamento com identificadores, código público do Tenant, unidade criada e próximo passo de onboarding

#### Scenario: Usuário tenant chama rota global
- **WHEN** uma identidade tenant-scoped chama qualquer rota global de Tenants
- **THEN** a API retorna HTTP 403 com `ProblemDetails` e não executa a operação

#### Scenario: Entrada estruturalmente inválida
- **WHEN** a requisição contém nome, código público, slug, fuso, e-mail, senha temporária ou chave de intenção inválidos
- **THEN** a API retorna `ProblemDetails` de validação sem iniciar transação de provisionamento

### Requirement: API distingue conflitos e recursos ausentes
A API MUST retornar respostas padronizadas para unicidade, reutilização incompatível de chave de intenção, Tenant inativo ou inexistente e Owner inelegível, sem revelar dados de outro Tenant.

#### Scenario: Identificador público duplicado
- **WHEN** o código público do Tenant, slug da unidade ou outra unicidade protegida conflita
- **THEN** a API retorna HTTP 409 com `ProblemDetails` e sem persistência parcial

#### Scenario: Tenant alvo indisponível
- **WHEN** uma unidade adicional é solicitada para Tenant inexistente ou inativo
- **THEN** a API retorna resposta de recurso indisponível sem criar a unidade

### Requirement: Consulta global é paginada e retomável
A API SHALL oferecer consulta paginada e filtrável de Tenants e consulta do resultado por chave de intenção pertencente ao ator global, com ordenação estável e limites documentados.

#### Scenario: Página válida de Tenants
- **WHEN** um superusuário informa filtros e paginação válidos
- **THEN** a API retorna contratos resumidos em ordem estável com total filtrado

#### Scenario: Consulta de intenção de outro ator
- **WHEN** um superusuário tenta consultar chave de intenção registrada por outro ator global
- **THEN** a API não retorna o resultado daquela intenção
