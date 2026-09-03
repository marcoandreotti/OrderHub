# Administrative Users Specification

## Purpose

Define identidades administrativas e seus perfis operacionais para acesso autorizado às unidades de um Tenant.

## Requirements

### Requirement: Usuário administrativo pertence a um Tenant
Cada usuário administrativo MUST pertencer a exatamente um Tenant, possuir e-mail normalizado único nesse Tenant e somente acessar unidades do mesmo grupo para as quais possua associação ativa explícita.

#### Scenario: E-mail repetido no Tenant
- **WHEN** for cadastrado um usuário com e-mail já utilizado no mesmo Tenant
- **THEN** o sistema SHALL rejeitar o cadastro por conflito

#### Scenario: Mesmo e-mail em outro Tenant
- **WHEN** o mesmo e-mail for cadastrado em Tenant distinto
- **THEN** o sistema SHALL permitir a identidade separada sem compartilhar permissões ou dados

#### Scenario: Unidade sem associação
- **WHEN** um usuário autenticado selecionar unidade do próprio Tenant sem possuir associação ativa a ela
- **THEN** o sistema MUST negar o acesso sem revelar dados da unidade

#### Scenario: Unidade associada
- **WHEN** um usuário autenticado selecionar unidade ativa do seu Tenant para a qual possua associação ativa
- **THEN** o sistema SHALL estabelecer o contexto operacional dessa unidade

### Requirement: Perfis concedem capacidades explícitas
O sistema SHALL associar usuários a um ou mais perfis conhecidos, incluindo proprietário, administrador, gerente, atendente, cozinha e entrega, e SHALL autorizar ações por políticas derivadas desses perfis.

#### Scenario: Usuário sem perfil exigido
- **WHEN** um usuário autenticado tentar operação cujo perfil não possui a capacidade exigida
- **THEN** o sistema MUST negar a operação sem alterar estado

### Requirement: Associação de unidade é explícita e revogável
O sistema SHALL manter associações explícitas entre usuário e estabelecimento, MUST impedir associações entre Tenants distintos e MUST interromper novos acessos quando a associação for desativada ou removida.

#### Scenario: Associação cruzada
- **WHEN** uma operação tentar associar usuário e estabelecimento pertencentes a Tenants diferentes
- **THEN** o sistema MUST rejeitar a associação

#### Scenario: Associação revogada
- **WHEN** a associação usada por uma seleção de unidade tiver sido revogada
- **THEN** o sistema MUST negar a criação de novo contexto operacional para essa unidade

### Requirement: Credenciais e estado de acesso são protegidos
Senhas MUST ser armazenadas apenas como hash adequado, usuários inativos MUST NOT autenticar e o último acesso bem-sucedido SHALL poder ser registrado.

#### Scenario: Usuário inativo
- **WHEN** um usuário inativo apresentar credenciais válidas
- **THEN** a autenticação MUST ser negada

### Requirement: Políticas administrativas mapeiam capacidades operacionais
As políticas SHALL conceder gestão a proprietário, administrador e gerente; atendimento a papéis configurados para pedidos; cozinha às transições de preparo; e entrega às transições de entrega, sempre limitadas às unidades associadas.

#### Scenario: Entregador finaliza entrega associada
- **WHEN** um entregador associado solicitar transição de entrega permitida
- **THEN** a API SHALL autorizar a ação sem conceder acesso às configurações administrativas
