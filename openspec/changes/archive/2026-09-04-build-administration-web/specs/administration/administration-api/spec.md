## ADDED Requirements

### Requirement: API oferece gestão paginada de usuários
A API SHALL oferecer consulta paginada e filtrável de usuários administrativos e operações explícitas para cadastro, estado, papéis e associações, sempre limitadas ao Tenant autenticado.

#### Scenario: Pesquisa por unidade
- **WHEN** um administrador pesquisa usuários associados a uma unidade autorizada
- **THEN** a API retorna apenas usuários do mesmo Tenant que satisfazem o filtro

#### Scenario: Entrada estruturalmente inválida
- **WHEN** uma operação de usuário recebe campos inválidos
- **THEN** a API retorna ProblemDetails de validação sem executar alteração parcial

#### Scenario: Chamada direta tenta elevar Admin a Owner
- **WHEN** um Admin chama diretamente uma operação de cadastro ou alteração de papéis tentando conceder ou remover Owner, para si ou para terceiros
- **THEN** a API retorna HTTP 403 com ProblemDetails e não persiste alterações parciais

#### Scenario: Alteração de estado de Owner sem autorização
- **WHEN** um Admin tenta ativar ou desativar Owner, ou um Owner tenta desativar a si próprio por chamada direta
- **THEN** a API retorna HTTP 403 com ProblemDetails sem alterar dados

#### Scenario: Operação autorizada eliminaria o último Owner ativo
- **WHEN** uma operação autorizada de estado ou papéis deixaria o Tenant sem Owner ativo
- **THEN** a API retorna HTTP 409 com ProblemDetails sem alterações parciais

### Requirement: Administração consulta adicionais e grupos independentemente dos vínculos
A API SHALL oferecer consultas independentes, paginadas e filtráveis por nome e atividade para adicionais e grupos da unidade autorizada. As consultas MUST exigir capacidade de gestão, derivar o Tenant no servidor e validar a unidade antes de retornar dados. Sem filtro de atividade, SHALL incluir ativos e inativos, com ou sem vínculos. Os contratos MUST fornecer os dados necessários à edição, incluindo limites e itens ordenados dos grupos, sem expor entidades de domínio.

#### Scenario: Adicional ainda não associado a grupo
- **WHEN** um gerente autorizado consulta adicionais de uma unidade que contém um adicional sem vínculo com grupo
- **THEN** a API inclui esse adicional quando ele satisfaz os filtros, sem exigir associação prévia

#### Scenario: Grupo ainda não associado a produto
- **WHEN** um gerente autorizado consulta grupos de uma unidade que contém um grupo sem vínculo com produto
- **THEN** a API inclui o grupo com seus limites e itens ordenados, inclusive itens inativos, quando ele satisfaz os filtros

#### Scenario: Paginação e filtro de atividade
- **WHEN** uma consulta válida informa página, tamanho e filtro de atividade
- **THEN** a API retorna a página solicitada e o total filtrado em ordem estável por nome e ID, sem omitir recursos por ausência de vínculos

#### Scenario: Paginação inválida
- **WHEN** o cliente informa página menor que um ou tamanho fora do intervalo de um a cem
- **THEN** a API rejeita com ProblemDetails de validação

#### Scenario: Consulta sem autorização para a unidade
- **WHEN** o ator tenta consultar adicionais ou grupos sem capacidade de gestão ou sem acesso à unidade selecionada
- **THEN** a API nega o acesso sem revelar os recursos

### Requirement: Correção administrativa preserva o cardápio público
A adição das consultas administrativas de adicionais e grupos MUST NOT alterar o contrato público nem seus filtros de vendabilidade e MUST NOT publicar recursos sem vínculos ou inativos por causa dessas consultas.

#### Scenario: Consultas administrativas e públicas sobre a mesma unidade
- **WHEN** uma unidade contém adicionais ou grupos inativos ou ainda sem vínculos
- **THEN** eles permanecem consultáveis pela administração autorizada, enquanto o cardápio público mantém somente a composição vendável anteriormente permitida
