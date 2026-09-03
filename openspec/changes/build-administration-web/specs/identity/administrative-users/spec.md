## ADDED Requirements

### Requirement: Usuários administrativos podem ser gerenciados por capacidade
O sistema SHALL permitir que um usuário autorizado consulte, convide ou cadastre, altere estado, atribua papéis e mantenha associações de unidade de usuários do mesmo Tenant.

#### Scenario: Proprietário associa gerente
- **WHEN** um proprietário autorizado atribui papel e unidade válidos a outro usuário de seu Tenant
- **THEN** o sistema persiste a associação e ela passa a valer nas próximas autorizações ou renovações

#### Scenario: Tentativa de gestão cruzada
- **WHEN** uma operação referencia usuário, papel ou unidade de outro Tenant
- **THEN** o sistema rejeita sem revelar dados do outro Tenant

### Requirement: Último administrador não perde acesso acidentalmente
O sistema MUST impedir uma alteração que deixe o Tenant sem nenhum usuário ativo capaz de administrar usuários e associações.

#### Scenario: Desativação do último administrador
- **WHEN** o único administrador elegível tenta desativar a si próprio ou remover sua capacidade essencial
- **THEN** o sistema rejeita a operação com conflito explicativo
