# Administration Web Specification

## Purpose

Oferece aos usuários autorizados uma interface web consistente para administrar pessoas, catálogo, clientes, promoções e pagamentos de cada unidade.

## Requirements

### Requirement: Área administrativa exige sessão válida
A aplicação SHALL exigir autenticação administrativa concluída antes de exibir dados protegidos e MUST redirecionar sessões ausentes ou expiradas para o login sem preservar dados sensíveis na interface.

#### Scenario: Sessão expirada
- **WHEN** uma requisição administrativa retorna que a sessão não é mais válida
- **THEN** a aplicação limpa o estado protegido e conduz o usuário ao login

### Requirement: Unidade selecionada pertence ao usuário
A aplicação SHALL permitir selecionar somente unidades retornadas pelo contexto autenticado e MUST reconstruir consultas ao trocar de unidade.

#### Scenario: Troca de unidade
- **WHEN** o usuário seleciona outra unidade autorizada
- **THEN** a tela descarta dados da unidade anterior e carrega dados da nova unidade

### Requirement: Navegação respeita capacidades
A aplicação SHALL apresentar módulos e ações compatíveis com as capacidades do usuário, sem substituir a autorização obrigatória da API.

#### Scenario: Cozinha acessa administração
- **WHEN** um usuário sem capacidade de gestão tenta navegar diretamente para uma tela administrativa
- **THEN** a aplicação mostra acesso negado e a API não entrega os dados

#### Scenario: Administração visual do papel Owner
- **WHEN** um usuário gerencia papéis de usuários do Tenant
- **THEN** a interface disponibiliza concessão ou remoção de Owner somente a um Owner atuando sobre outro usuário, sem substituir a validação obrigatória da API

#### Scenario: Administração visual do estado de Owner
- **WHEN** um usuário visualiza ações de ativação ou desativação de um usuário com papel Owner
- **THEN** a interface disponibiliza essas ações somente a outro Owner ativo e apresenta conflitos de proteção do último Owner retornados pela API, sem substituir a validação do servidor

### Requirement: Recursos administrativos possuem experiência completa
A aplicação SHALL permitir pesquisar, paginar, criar e editar usuários, catálogo, clientes, cupons e formas de pagamento conforme as operações autorizadas disponíveis.

#### Scenario: Edição válida
- **WHEN** um gerente envia alterações válidas em um recurso permitido
- **THEN** a aplicação persiste pela API e atualiza a visão sem duplicar o recurso

#### Scenario: Manutenção de adicional ou grupo recém-criado
- **WHEN** um usuário autorizado cadastra adicional ou grupo sem vínculos e recarrega a administração do catálogo
- **THEN** a interface permite pesquisar, localizar, editar e selecionar esse recurso pelas consultas independentes, inclusive em páginas posteriores, sem exigir vínculo prévio com grupo ou produto

#### Scenario: Manutenção de recurso inativo
- **WHEN** um usuário autorizado pesquisa adicionais ou grupos inativos
- **THEN** a interface permite localizá-los e editar seus dados e vínculos sem remover silenciosamente itens inativos já associados

### Requirement: Erros e estados são compreensíveis
A aplicação MUST representar carregamento, ausência de dados, falha de rede, validação, conflito, proibição e recurso não encontrado com mensagens e ações adequadas.

#### Scenario: Validação rejeitada
- **WHEN** a API retorna ProblemDetails de validação
- **THEN** a aplicação associa os erros aos campos quando possível e preserva os dados editados

### Requirement: Operações sensíveis exigem confirmação
A aplicação SHALL solicitar confirmação explícita para desativar acesso, revogar associação ou executar outra ação com impacto operacional relevante.

#### Scenario: Usuário cancela confirmação
- **WHEN** o operador cancela a confirmação de uma ação sensível
- **THEN** nenhuma requisição mutável é enviada

### Requirement: Interface é responsiva e acessível
A aplicação SHALL oferecer navegação por teclado, indicação de foco, rótulos e contraste adequados e SHALL funcionar nos tamanhos suportados de desktop e tablet.

#### Scenario: Operação por teclado
- **WHEN** um usuário percorre formulário e ações sem dispositivo apontador
- **THEN** controles permanecem alcançáveis, identificáveis e acionáveis

### Requirement: Plataforma possui área própria de provisionamento
A aplicação SHALL apresentar ao `PlatformSuperUser` plenamente autenticado uma área global separada da navegação tenant-scoped, com listagem de Tenants e ações para provisionar Tenant ou adicionar unidade. Usuários tenant-scoped MUST NOT visualizar nem acessar essa área.

#### Scenario: Plataforma sem unidades existentes
- **WHEN** o primeiro superusuário acessa a aplicação e nenhum Tenant ou unidade existe
- **THEN** a interface apresenta estado vazio com ação para provisionar o primeiro Tenant, em vez de solicitar associação de unidade

#### Scenario: Usuário tenant tenta rota de plataforma
- **WHEN** um usuário tenant-scoped navega diretamente para a área global
- **THEN** a interface apresenta acesso negado e a API não entrega dados globais

### Requirement: Jornada coleta o conjunto completo de dados
O formulário de provisionamento SHALL coletar e validar dados do Tenant, primeira unidade e primeiro Owner, gerar uma chave de intenção por tentativa lógica e preservar os dados editados quando a API retornar validação ou conflito.

#### Scenario: Envio válido
- **WHEN** o superusuário confirma dados válidos e a API conclui o provisionamento
- **THEN** a interface apresenta o resultado, seleciona a unidade criada e oferece continuar para seu onboarding

#### Scenario: Resposta de rede desconhecida
- **WHEN** a conexão falha após o envio e o resultado é desconhecido
- **THEN** a interface reutiliza a mesma chave de intenção para consultar ou repetir a operação sem duplicar recursos

### Requirement: Contextos global e operacional são distintos
A aplicação MUST limpar ou reconstruir estado tenant-scoped ao alternar entre a área global e uma unidade, e SHALL permitir que a identidade de plataforma abra o onboarding de unidade existente sem simular associação administrativa.

#### Scenario: Entrada no onboarding após provisionamento
- **WHEN** o superusuário escolhe continuar na unidade recém-criada
- **THEN** a aplicação seleciona a unidade retornada, descarta dados anteriores e carrega o onboarding daquela unidade

#### Scenario: Retorno à área global
- **WHEN** o superusuário retorna da administração de uma unidade para a área de plataforma
- **THEN** dados operacionais da unidade deixam de ser exibidos sem encerrar a sessão global
