## ADDED Requirements

### Requirement: Principal autenticado deriva do cadastro vigente
O sistema MUST construir o principal administrativo a partir do usuário, Tenant, papéis e associações ativas persistidos no servidor no momento da autenticação ou renovação.

#### Scenario: Associação revogada após login
- **WHEN** uma associação de unidade é revogada antes da renovação da sessão
- **THEN** a credencial renovada não concede acesso à unidade revogada

#### Scenario: Usuário desativado
- **WHEN** um usuário é desativado
- **THEN** novas autenticações e renovações são rejeitadas e suas sessões ativas deixam de autorizar operações

### Requirement: Identidade de plataforma é separada do Tenant
O sistema MUST representar superusuários como identidades globais sem TenantId, papéis tenant-scoped ou associações de estabelecimento, e MUST distinguir explicitamente suas autorizações das concedidas a usuários administrativos.

#### Scenario: Superusuário atua em um Tenant
- **WHEN** um superusuário plenamente autenticado executa uma operação global permitida sobre um Tenant existente
- **THEN** o sistema registra sua identidade de plataforma como ator sem criar associação artificial ao Tenant

### Requirement: Superusuário administra somente seus pares
Somente um superusuário ativo com senha definitiva e segundo fator concluído SHALL poder criar, ativar ou desativar outro superusuário, e o sistema MUST preservar ao menos um superusuário ativo.

#### Scenario: Administrador de Tenant tenta nomear superusuário
- **WHEN** Owner, Admin ou qualquer papel tenant-scoped tenta conceder acesso global
- **THEN** o sistema rejeita a operação sem criar ou alterar identidade de plataforma

#### Scenario: Último superusuário seria desativado
- **WHEN** uma operação deixaria a plataforma sem superusuário ativo
- **THEN** o sistema rejeita a alteração com conflito
