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
