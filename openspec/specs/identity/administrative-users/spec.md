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

### Requirement: Usuários administrativos podem ser gerenciados por capacidade
O sistema SHALL permitir que um usuário autorizado consulte, convide ou cadastre, altere estado, atribua papéis e mantenha associações de unidade de usuários do mesmo Tenant.

#### Scenario: Proprietário associa gerente
- **WHEN** um proprietário autorizado atribui papel e unidade válidos a outro usuário de seu Tenant
- **THEN** o sistema persiste a associação e ela passa a valer nas próximas autorizações ou renovações

#### Scenario: Tentativa de gestão cruzada
- **WHEN** uma operação referencia usuário, papel ou unidade de outro Tenant
- **THEN** o sistema rejeita sem revelar dados do outro Tenant

### Requirement: Papel Owner é gerenciado exclusivamente por outro Owner
Na gestão de usuários do Tenant, o sistema MUST permitir conceder ou remover o papel Owner somente quando o autor autenticado for Owner e o destinatário for outro usuário do mesmo Tenant. A restrição SHALL abranger cadastro e qualquer operação que altere papéis, mantendo a proteção do último administrador. Esta regra não redefine os poderes globais de PlatformSuperUser.

#### Scenario: Owner altera o papel de outro usuário
- **WHEN** um Owner concede ou remove Owner de outro usuário do mesmo Tenant e a proteção do último administrador é satisfeita
- **THEN** o sistema permite a alteração

#### Scenario: Admin tenta elevar seus próprios privilégios
- **WHEN** um Admin tenta atribuir Owner a si próprio
- **THEN** o sistema nega a operação sem alterar papéis ou outros dados

#### Scenario: Admin tenta gerenciar Owner de terceiro
- **WHEN** um Admin tenta conceder ou remover Owner de outro usuário, inclusive durante cadastro
- **THEN** o sistema nega a operação sem alterações parciais

#### Scenario: Owner tenta alterar seu próprio papel Owner
- **WHEN** um Owner tenta conceder ou remover seu próprio papel Owner
- **THEN** o sistema nega a operação e exige que outro Owner autorizado faça a alteração

### Requirement: Estado de Owner é gerenciado exclusivamente por outro Owner
O sistema MUST permitir ativação ou desativação de usuário com papel Owner somente por outro Owner ativo do mesmo Tenant. O papel do destinatário SHALL ser considerado mesmo quando ele estiver inativo. Admin MUST NOT ativar ou desativar Owner.

#### Scenario: Outro Owner altera estado
- **WHEN** um Owner ativo ativa ou desativa outro usuário com papel Owner do mesmo Tenant e preserva ao menos um Owner ativo
- **THEN** o sistema permite a alteração

#### Scenario: Admin tenta alterar estado de Owner
- **WHEN** um Admin tenta ativar ou desativar um usuário com papel Owner
- **THEN** o sistema nega a operação sem alterações parciais

#### Scenario: Owner tenta desativar a si próprio
- **WHEN** um Owner tenta desativar seu próprio usuário
- **THEN** o sistema nega a operação mesmo que exista outro Owner ativo

### Requirement: Tenant preserva ao menos um Owner ativo
O sistema MUST impedir alterações de papéis ou estado que deixem o Tenant sem Owner ativo, mesmo quando restarem Admins ativos. A verificação e a escrita SHALL preservar essa invariante atomicamente, inclusive sob concorrência e sem dispensá-la por autorização global.

#### Scenario: Operação autorizada removeria o último Owner ativo
- **WHEN** uma operação autorizada de estado ou papéis deixaria o Tenant sem Owner ativo
- **THEN** o sistema rejeita com conflito sem persistir alterações parciais

#### Scenario: Operações concorrentes afetam os últimos Owners
- **WHEN** operações concorrentes tentam remover o papel Owner ou desativar os últimos Owners ativos do mesmo Tenant
- **THEN** o sistema revalida autorização e estado de forma coordenada e impede a conclusão de operações que eliminariam o último Owner ativo

### Requirement: Último administrador não perde acesso acidentalmente
O sistema MUST impedir uma alteração que deixe o Tenant sem nenhum usuário ativo capaz de administrar usuários e associações.

#### Scenario: Desativação do último administrador
- **WHEN** o único administrador elegível tenta desativar a si próprio ou remover sua capacidade essencial
- **THEN** o sistema rejeita a operação com conflito explicativo
