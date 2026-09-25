## ADDED Requirements

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
