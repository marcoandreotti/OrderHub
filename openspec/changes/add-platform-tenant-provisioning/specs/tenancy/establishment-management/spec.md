## MODIFIED Requirements

### Requirement: Tenant possui unidades operacionais
O sistema SHALL representar o Tenant como grupo proprietário e cada estabelecimento como uma unidade pertencente a exatamente um Tenant, permitindo múltiplas unidades por grupo. Uma identidade tenant-scoped autorizada SHALL criar unidade somente no próprio Tenant; um `PlatformSuperUser` plenamente autenticado MAY criar a primeira unidade durante o provisionamento ou adicionar unidade a Tenant explicitamente selecionado e resolvido no servidor.

#### Scenario: Nova unidade no grupo
- **WHEN** uma unidade válida for cadastrada por um ator tenant-scoped autorizado
- **THEN** ela SHALL pertencer ao Tenant do ator e MUST NOT ser associada a outro Tenant por identificador recebido do cliente

#### Scenario: Nova unidade por ator global
- **WHEN** um `PlatformSuperUser` plenamente autenticado cadastra unidade em Tenant existente e ativo
- **THEN** o sistema resolve o Tenant alvo no servidor, registra o ator global e não cria associação artificial para ele
