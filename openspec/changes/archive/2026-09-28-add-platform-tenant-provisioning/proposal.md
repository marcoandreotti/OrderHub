## Why

O primeiro `PlatformSuperUser` consegue autenticar, mas não existe um fluxo utilizável para criar o primeiro Tenant, sua primeira unidade e o primeiro Owner. Sem esse provisionamento, o contexto administrativo não contém unidades e todos os demais cadastros ficam inacessíveis, embora o onboarding e as operações tenant-scoped já estejam implementados.

## What Changes

- Adicionar um fluxo global, exclusivo de `PlatformSuperUser` plenamente autenticado, para provisionar um Tenant com sua primeira unidade e seu primeiro Owner.
- Tornar a operação atômica e repetível com segurança por chave de intenção, sem aceitar `TenantId`, papéis globais ou associações como autoridade enviada pelo cliente.
- Permitir cadastro posterior de novas unidades em Tenant existente pelo escopo global, preservando slug único e isolamento entre Tenants.
- Criar contratos e endpoints explícitos para listar Tenants, consultar o resultado do provisionamento e executar o provisionamento inicial.
- Adicionar uma experiência web específica para identidades de plataforma, inclusive estado vazio, formulário, feedback de conflito e transição para o onboarding da unidade criada.
- Criar e associar o primeiro Owner ativo à primeira unidade, com senha temporária e troca obrigatória no primeiro acesso.
- Registrar o ator global e os identificadores provisionados para futura integração com a trilha de auditoria, sem criar associação artificial entre o superusuário e o Tenant.

## Capabilities

### New Capabilities

- `tenancy/platform-tenant-provisioning`: provisionamento global, atômico e idempotente de Tenant, primeira unidade e primeiro Owner, além da adição controlada de unidades a Tenants existentes.

### Modified Capabilities

- `tenancy/establishment-management`: distinguir o cadastro tenant-scoped de unidade do provisionamento global e permitir que um ator de plataforma crie unidade em Tenant explicitamente resolvido no servidor.
- `identity/administrative-users`: definir criação segura do primeiro Owner durante o provisionamento, associação inicial e troca obrigatória da senha temporária.
- `administration/administration-api`: expor operações globais explícitas de consulta e provisionamento sem enfraquecer as rotas tenant-scoped existentes.
- `administration/administration-web`: oferecer área e jornada próprias para o `PlatformSuperUser`, com estado vazio e entrada no onboarding após o provisionamento.

## Impact

Afeta Tenancy, Identity, Application, persistência EF Core/Dapper, migrations se necessárias, contratos HTTP, autorização global, frontend Vue/Quasar e testes de domínio, aplicação, integração e interface. Reutiliza os dispatchers próprios, `ProblemDetails`, autenticação administrativa, `PlatformSuperUser`, `Tenant`, `Establishment`, `AdministrativeUser` e o onboarding existentes. Não introduz MediatR, AutoMapper, mensageria, cobrança, assinatura ou provisionamento de infraestrutura externa.
