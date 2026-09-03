## Purpose

Oferece aos usuários autorizados uma interface web consistente para administrar pessoas, catálogo, clientes, promoções e pagamentos de cada unidade.

## ADDED Requirements

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

### Requirement: Recursos administrativos possuem experiência completa
A aplicação SHALL permitir pesquisar, paginar, criar e editar usuários, catálogo, clientes, cupons e formas de pagamento conforme as operações autorizadas disponíveis.

#### Scenario: Edição válida
- **WHEN** um gerente envia alterações válidas em um recurso permitido
- **THEN** a aplicação persiste pela API e atualiza a visão sem duplicar o recurso

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
