## ADDED Requirements

### Requirement: Provisionamento estabelece o primeiro Owner
O provisionamento de um novo Tenant MUST criar exatamente um usuário administrativo ativo com papel Owner e associação ativa à primeira unidade. O e-mail MUST ser único no novo Tenant, a senha temporária MUST ser armazenada somente como hash e a operação MUST preservar a invariante de ao menos um Owner ativo.

#### Scenario: Primeiro Owner válido
- **WHEN** um superusuário provisiona Tenant com dados válidos do responsável inicial
- **THEN** o responsável é criado como Owner ativo e associado à primeira unidade na mesma operação

#### Scenario: Falha ao criar ou associar Owner
- **WHEN** o primeiro Owner não pode ser criado ou associado de forma válida
- **THEN** o Tenant e a unidade não permanecem parcialmente provisionados

### Requirement: Primeiro Owner troca a senha temporária
O primeiro Owner criado pelo provisionamento MUST concluir o segundo fator e trocar a senha temporária antes de receber capacidades tenant-scoped. Enquanto a troca estiver pendente, sua sessão SHALL permitir somente consultar o contexto, trocar a senha e encerrar a sessão; a troca MUST revogar sessões anteriores e exigir nova autenticação.

#### Scenario: Primeiro acesso com credencial temporária
- **WHEN** o Owner provisionado conclui senha e segundo fator usando a credencial temporária
- **THEN** o sistema emite sessão restrita sem acesso ao onboarding ou a dados operacionais

#### Scenario: Troca concluída
- **WHEN** o Owner informa a senha temporária vigente e uma nova senha válida
- **THEN** o sistema remove a restrição, revoga sessões anteriores e exige novo login

### Requirement: Unidade adicional recebe administração inicial
Ao adicionar unidade a Tenant existente, o sistema MUST exigir a associação de ao menos um Owner ativo daquele Tenant, sem conceder ou remover papéis durante a operação.

#### Scenario: Owner elegível selecionado
- **WHEN** o superusuário adiciona unidade e seleciona Owner ativo do Tenant alvo
- **THEN** o sistema cria a associação ativa necessária à administração da nova unidade

#### Scenario: Usuário sem papel Owner
- **WHEN** a operação referencia usuário ativo do Tenant que não possui papel Owner
- **THEN** o sistema rejeita a criação da unidade sem alterações parciais
