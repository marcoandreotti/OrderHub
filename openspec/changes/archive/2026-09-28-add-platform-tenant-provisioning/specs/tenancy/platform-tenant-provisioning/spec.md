## Purpose

Permite que operadores globais inicializem com segurança um novo cliente da plataforma, criando o Tenant, sua primeira unidade e o primeiro Owner em uma única jornada recuperável.

## ADDED Requirements

### Requirement: Provisionamento global exige identidade de plataforma plena
O sistema MUST permitir provisionamento de Tenant somente a `PlatformSuperUser` ativo, com segundo fator concluído e sem troca de senha pendente, e MUST NOT conceder essa operação a qualquer identidade tenant-scoped.

#### Scenario: Administrador de Tenant tenta provisionar
- **WHEN** Owner, Admin ou outro usuário tenant-scoped solicita o provisionamento de um Tenant
- **THEN** o sistema nega a operação sem criar recursos

#### Scenario: Superusuário com sessão restrita
- **WHEN** um `PlatformSuperUser` ainda sujeito à troca obrigatória de senha solicita o provisionamento
- **THEN** o sistema nega a operação até a autenticação global estar plenamente concluída

### Requirement: Primeiro provisionamento é completo e atômico
O sistema SHALL criar o Tenant, sua primeira unidade ativa, um primeiro Owner ativo e a associação ativa desse Owner com a unidade em uma única operação atômica. O resultado MUST fornecer os identificadores e códigos públicos necessários para continuar no onboarding e para o primeiro login do Owner, sem expor hashes ou identificadores usados como autoridade interna.

#### Scenario: Provisionamento concluído
- **WHEN** um superusuário autorizado informa dados válidos e únicos do Tenant, da unidade e do primeiro Owner
- **THEN** todos os recursos são persistidos, o Owner fica associado à unidade e a unidade pode iniciar o onboarding

#### Scenario: Falha em qualquer recurso
- **WHEN** a criação do Tenant, unidade, Owner ou associação falha
- **THEN** nenhum recurso parcial do provisionamento permanece persistido

### Requirement: Repetição do provisionamento é idempotente
Toda solicitação de provisionamento MUST possuir chave de intenção válida no escopo do ator global. A repetição com a mesma chave e o mesmo conteúdo SHALL retornar o resultado original, enquanto a reutilização da chave com conteúdo diferente MUST ser rejeitada por conflito.

#### Scenario: Resposta perdida e repetida
- **WHEN** uma solicitação concluída é repetida pelo mesmo ator com a mesma chave e o mesmo conteúdo
- **THEN** o sistema retorna os mesmos identificadores sem criar outro Tenant, unidade, Owner ou associação

#### Scenario: Chave reutilizada com conteúdo diferente
- **WHEN** o mesmo ator reutiliza uma chave de intenção com dados diferentes
- **THEN** o sistema rejeita por conflito sem alterar o provisionamento original

### Requirement: Unicidades são verificadas de forma concorrente
O provisionamento MUST preservar a unicidade do código público do Tenant, do slug global da unidade e do e-mail normalizado do Owner dentro do novo Tenant, inclusive sob solicitações concorrentes.

#### Scenario: Código público ou slug já utilizado
- **WHEN** o provisionamento tenta usar código público de Tenant ou slug de unidade já existente
- **THEN** o sistema retorna conflito e não persiste recursos parciais

#### Scenario: Solicitações concorrentes equivalentes
- **WHEN** solicitações concorrentes tentam criar os mesmos identificadores públicos com chaves distintas
- **THEN** no máximo um provisionamento é concluído e os demais recebem conflito

### Requirement: Plataforma adiciona unidade a Tenant existente
O sistema SHALL permitir que um superusuário plenamente autenticado adicione uma unidade a um Tenant existente e ativo, resolvendo o Tenant alvo no servidor e sem criar associação artificial para o ator global. A nova unidade SHALL iniciar com onboarding pendente e MUST possuir ao menos um Owner ativo do Tenant associado antes de concluir o onboarding.

#### Scenario: Nova unidade em Tenant ativo
- **WHEN** o superusuário seleciona um Tenant ativo, informa dados válidos da unidade e seleciona Owner ativo do mesmo Tenant
- **THEN** a unidade é criada nesse Tenant, o Owner é associado e o onboarding fica disponível

#### Scenario: Owner pertence a outro Tenant
- **WHEN** a solicitação referencia Owner que não pertence ao Tenant alvo
- **THEN** o sistema rejeita sem revelar dados do outro Tenant e sem criar a unidade

### Requirement: Provisionamentos podem ser consultados pela plataforma
O sistema SHALL permitir que superusuários plenamente autenticados listem Tenants de forma paginada e filtrável e consultem o resultado de uma intenção de provisionamento, incluindo unidades e estado de onboarding necessários para retomar a jornada.

#### Scenario: Plataforma pesquisa Tenant
- **WHEN** um superusuário autorizado pesquisa por nome, código público ou estado
- **THEN** o sistema retorna uma página ordenada e estável sem expor credenciais ou entidades de domínio

#### Scenario: Retomada após resposta desconhecida
- **WHEN** o cliente consulta uma chave de intenção anteriormente concluída pelo mesmo ator
- **THEN** o sistema retorna o resultado persistido e permite continuar na unidade provisionada

### Requirement: Ator global permanece separado do Tenant
O sistema MUST registrar a identidade global como autora do provisionamento e MUST NOT criar usuário administrativo, papel ou associação de unidade para representar o `PlatformSuperUser` dentro do Tenant provisionado.

#### Scenario: Inspeção dos acessos após provisionamento
- **WHEN** o provisionamento é concluído
- **THEN** somente o primeiro Owner possui associação administrativa inicial e o ator global permanece identificado separadamente
