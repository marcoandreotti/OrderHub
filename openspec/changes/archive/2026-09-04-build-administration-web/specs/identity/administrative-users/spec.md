## ADDED Requirements

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
