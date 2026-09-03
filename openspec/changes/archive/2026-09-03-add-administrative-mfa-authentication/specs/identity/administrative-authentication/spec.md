## Purpose

Protege o acesso administrativo e operacional por autenticação em duas etapas, com sessão criada somente após senha e código de uso único válidos.

## ADDED Requirements

### Requirement: Login exige contexto, senha e segundo fator
O sistema MUST exigir código público de Tenant para usuários administrativos ou código público de plataforma para superusuários, validar e-mail e senha antes de criar um desafio e SHALL emitir uma sessão autenticada somente após a validação de um código de uso único enviado ao e-mail do usuário.

#### Scenario: Credenciais e código válidos
- **WHEN** um usuário ativo informa credenciais válidas e conclui o desafio dentro da validade
- **THEN** o sistema cria uma sessão contendo somente identidades e permissões obtidas no servidor

#### Scenario: Senha válida sem código
- **WHEN** a senha é válida, mas o segundo fator não foi concluído
- **THEN** o sistema não concede acesso às rotas protegidas

#### Scenario: Código de Tenant não corresponde ao usuário
- **WHEN** credenciais são apresentadas com código de Tenant ao qual o usuário não pertence
- **THEN** o sistema rejeita com a mesma resposta pública usada para credenciais inválidas

#### Scenario: Superusuário usa código de plataforma
- **WHEN** um superusuário ativo apresenta o código público de plataforma e credenciais válidas
- **THEN** o sistema inicia o mesmo segundo fator sem exigir associação a Tenant

### Requirement: Respostas não enumeram usuários
O sistema MUST produzir resposta pública indistinguível para e-mail inexistente, senha incorreta, usuário inativo ou usuário sem acesso permitido.

#### Scenario: Identidade desconhecida
- **WHEN** uma tentativa utiliza e-mail que não corresponde a usuário elegível
- **THEN** a resposta não revela se o e-mail existe

### Requirement: Código é limitado e de uso único
O desafio MUST expirar, possuir limite de tentativas e intervalo de reenvio, e SHALL ser invalidado após uso bem-sucedido ou emissão de substituto.

#### Scenario: Reutilização do código
- **WHEN** um código já consumido é informado novamente
- **THEN** o sistema rejeita a tentativa sem criar outra sessão

#### Scenario: Limite de tentativas atingido
- **WHEN** o número máximo de tentativas inválidas é alcançado
- **THEN** o desafio é invalidado e um novo fluxo deve ser iniciado

### Requirement: Segredos de autenticação não são armazenados em texto aberto
O sistema MUST armazenar senhas e códigos somente em representação protegida e MUST impedir que códigos, senhas ou tokens de sessão sejam registrados em logs ou retornados por APIs.

#### Scenario: Inspeção de persistência e logs
- **WHEN** um desafio é criado e utilizado
- **THEN** o valor original da senha e do código não aparece na persistência nem nos logs da aplicação

### Requirement: Sessão possui renovação e revogação seguras
O sistema SHALL emitir credencial de acesso curta e renovação rotativa, e MUST invalidar a cadeia de renovação no logout, na desativação do usuário ou ao detectar reutilização.

#### Scenario: Token de renovação reutilizado
- **WHEN** um token de renovação já rotacionado é apresentado novamente
- **THEN** o sistema revoga a cadeia associada e exige nova autenticação em duas etapas

### Requirement: Tentativas abusivas são limitadas
O sistema MUST limitar tentativas e reenvios por identidade e origem sem usar bloqueio permanente que permita negar serviço a um usuário legítimo.

#### Scenario: Solicitações em excesso
- **WHEN** o limite configurado é ultrapassado
- **THEN** o sistema rejeita temporariamente novas tentativas com resposta padronizada

### Requirement: Primeiro superusuário nasce por bootstrap seguro
Na inicialização da API, o sistema SHALL criar idempotentemente o primeiro superusuário quando não existir nenhum, usando e-mail e senha temporária obtidos exclusivamente de secrets de implantação, e MUST falhar de forma segura se a configuração obrigatória estiver ausente ou inválida.

#### Scenario: Primeira publicação
- **WHEN** a API inicia sem superusuário persistido e possui secrets válidos de bootstrap
- **THEN** exatamente uma identidade global ativa é criada com senha temporária protegida

#### Scenario: Reinício após bootstrap
- **WHEN** a API reinicia e já existe ao menos um superusuário
- **THEN** nenhum novo superusuário é criado e os secrets não sobrescrevem credenciais existentes

### Requirement: Primeiro acesso obriga troca da senha
Uma identidade criada por bootstrap MUST concluir o segundo fator e receber sessão restrita à troca de senha; o sistema SHALL liberar acesso global somente após definir uma nova senha que satisfaça a política e invalidar a senha temporária e sessões anteriores.

#### Scenario: Acesso antes da troca
- **WHEN** o superusuário autenticado com senha temporária tenta acessar qualquer operação diferente de consultar sessão, trocar senha ou encerrar sessão
- **THEN** o sistema rejeita a operação e informa que a troca de senha é obrigatória

#### Scenario: Troca concluída
- **WHEN** o superusuário informa a senha temporária vigente e uma nova senha válida
- **THEN** o sistema substitui o hash, remove a obrigação de troca, revoga sessões anteriores e exige autenticação normal para acesso global
