# Administration API Specification

## Purpose

Define a borda HTTP autenticada para gestão e operação diária dos estabelecimentos, aplicando políticas por papel, isolamento Multi-Tenant, contratos explícitos e consultas paginadas.

## Requirements

### Requirement: Toda rota administrativa exige contexto autorizado
A API administrativa MUST autenticar o ator, derivar o Tenant de seu contexto e validar associação ativa com o estabelecimento antes de consultar ou alterar dados.

#### Scenario: Usuário associado a outra unidade
- **WHEN** um usuário autenticado solicitar dados de unidade sem associação ativa
- **THEN** a API MUST negar a operação sem revelar a existência dos dados

### Requirement: Capacidades administrativas usam políticas específicas
Cada operação SHALL exigir política compatível com gestão, atendimento, cozinha ou entrega, sem depender apenas de autenticação genérica.

#### Scenario: Cozinha tenta editar cupom
- **WHEN** um usuário com papel exclusivo de cozinha solicitar alteração de cupom
- **THEN** a API MUST negar a operação

### Requirement: Listagens administrativas são paginadas e filtráveis
Consultas de coleções SHALL aceitar paginação limitada e filtros documentados e MUST ordenar resultados de forma estável.

#### Scenario: Página acima do limite
- **WHEN** o cliente solicitar tamanho de página superior ao permitido
- **THEN** a API SHALL rejeitar ou limitar o valor conforme contrato documentado

### Requirement: Contratos administrativos não expõem entidades
Respostas SHALL usar read models e contratos explícitos e MUST NOT serializar entidades de domínio, modelos EF ou detalhes SQL.

#### Scenario: Consulta de pedido
- **WHEN** um pedido for consultado administrativamente
- **THEN** a resposta SHALL conter os dados operacionais autorizados sem metadados internos de persistência

### Requirement: Produção aceita somente autenticação administrativa real
A API MUST rejeitar em ambientes não destinados a testes qualquer principal administrativo que não tenha sido emitido após a conclusão dos fatores exigidos.

#### Scenario: Cabeçalhos de identidade forjados
- **WHEN** um cliente envia diretamente identificadores, papéis ou associações por cabeçalhos em produção
- **THEN** a API ignora esses valores e retorna acesso não autorizado

#### Scenario: Autenticação de testes
- **WHEN** testes automatizados executam no ambiente de testes explicitamente configurado
- **THEN** o mecanismo substituto pode criar principals sem ficar disponível nos demais ambientes

### Requirement: Escopo global é explícito e auditável
A API MUST reconhecer acesso global somente por uma identidade de plataforma persistida e por sessão plenamente autenticada, sem permitir que TenantId, código de Tenant, papel ou cabeçalho enviado pelo cliente conceda esse escopo.

#### Scenario: Papel global forjado
- **WHEN** uma requisição tenta declarar diretamente papel ou escopo de superusuário
- **THEN** a API ignora a declaração e rejeita o acesso

#### Scenario: Superusuário seleciona Tenant
- **WHEN** uma identidade global válida executa operação permitida em Tenant específico
- **THEN** a API resolve o alvo explicitamente, aplica isolamento aos dados acessados e registra o ator global

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
