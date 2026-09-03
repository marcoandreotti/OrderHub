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
