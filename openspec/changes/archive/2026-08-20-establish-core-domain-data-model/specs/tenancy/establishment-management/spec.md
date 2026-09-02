## Purpose

Define o grupo Tenant, suas unidades operacionais e identidade visual com isolamento seguro e resolução pública estável.

## ADDED Requirements

### Requirement: Tenant possui unidades operacionais
O sistema SHALL representar o Tenant como grupo proprietário e cada estabelecimento como uma unidade pertencente a exatamente um Tenant, permitindo múltiplas unidades por grupo.

#### Scenario: Nova unidade no grupo
- **WHEN** uma unidade válida for cadastrada por um ator autorizado
- **THEN** ela SHALL pertencer ao Tenant do ator e MUST NOT ser associada a outro Tenant por identificador recebido do cliente

### Requirement: Slug público identifica a unidade
Cada unidade publicamente acessível MUST possuir slug normalizado e globalmente único, e unidades ou Tenants inativos MUST NOT disponibilizar seu conteúdo público.

#### Scenario: Resolução por slug
- **WHEN** um visitante acessar um slug ativo existente
- **THEN** o sistema SHALL resolver a unidade e seu Tenant sem conceder privilégios administrativos

#### Scenario: Slug indisponível
- **WHEN** o slug estiver ausente, duplicado ou associado a unidade inativa
- **THEN** o sistema SHALL rejeitar o cadastro ou não revelar dados da unidade, conforme a operação

### Requirement: Identidade visual por unidade
O sistema SHALL manter uma configuração visual por unidade, com cores, fonte, logotipo e favicon validados e valores padrão quando a personalização estiver incompleta.

#### Scenario: Tema parcial
- **WHEN** uma unidade não possuir todos os tokens visuais personalizados
- **THEN** o sistema SHALL completar a identidade com os tokens padrão sem impedir o acesso público

### Requirement: Escopo de unidade é validado no servidor
Dados operacionais SHALL ser associados ao Tenant e à unidade a que pertencem, e toda operação autenticada MUST validar no servidor que a unidade pertence ao Tenant resolvido e possui associação ativa com o usuário.

#### Scenario: Unidade de outro Tenant
- **WHEN** uma operação usar identificador de unidade que não pertence ao Tenant resolvido
- **THEN** o sistema MUST negar a operação sem revelar a existência de dados cruzados

#### Scenario: Unidade do Tenant sem autorização do usuário
- **WHEN** uma operação autenticada usar unidade do Tenant resolvido sem associação ativa com o usuário
- **THEN** o sistema MUST negar a operação sem revelar dados da unidade
