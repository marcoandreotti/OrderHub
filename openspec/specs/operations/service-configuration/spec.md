# Service Configuration Specification

## Purpose

Define mesas e horários regulares que configuram os modos de atendimento presenciais e a disponibilidade da unidade.

## Requirements

### Requirement: Mesa possui código e token público opaco
Cada mesa SHALL pertencer a uma unidade, possuir código único nessa unidade e token público opaco único, não previsível e revogável.

#### Scenario: Acesso por QR Code válido
- **WHEN** um visitante acessar slug e token de uma mesa ativa da mesma unidade
- **THEN** o sistema SHALL identificar a unidade e a mesa para o pedido sem expor identificadores internos

#### Scenario: Token de outra unidade
- **WHEN** slug e token pertencerem a unidades diferentes
- **THEN** o sistema MUST rejeitar a associação

### Requirement: Horários regulares são consistentes
A unidade MAY configurar intervalos ativos de abertura por dia da semana, e cada intervalo MUST possuir abertura anterior ao fechamento dentro do mesmo dia nesta versão inicial.

#### Scenario: Intervalo inválido
- **WHEN** o fechamento não for posterior à abertura
- **THEN** o sistema SHALL rejeitar o horário

#### Scenario: Consulta fora do horário
- **WHEN** a disponibilidade for consultada fora de todos os intervalos ativos do dia
- **THEN** a unidade SHALL ser apresentada como fechada para novos pedidos

### Requirement: Horários podem ser administrados por unidade
O sistema SHALL permitir consultar e substituir a configuração semanal de horários de uma unidade autorizada em operação atômica.

#### Scenario: Um intervalo é inválido
- **WHEN** uma grade contém qualquer intervalo inconsistente
- **THEN** o sistema rejeita toda a alteração e preserva a grade anterior

### Requirement: Mesas e tokens possuem ciclo administrativo
O sistema SHALL permitir listar, criar, renomear, ativar e desativar mesas e SHALL permitir renovar seu token público opaco invalidando imediatamente o anterior.

#### Scenario: Token renovado
- **WHEN** um administrador autorizado renova o token de uma mesa
- **THEN** a URL anterior deixa de resolver a mesa e uma nova URL pública fica disponível

### Requirement: QR Code não incorpora dados internos
O sistema SHALL gerar ou fornecer os dados para um QR Code contendo somente a URL pública com slug e token opaco, sem TenantId, EstablishmentId ou identificadores internos da mesa.

#### Scenario: Conteúdo do QR Code
- **WHEN** um QR Code é obtido para uma mesa ativa
- **THEN** seu conteúdo pode ser compartilhado publicamente sem revelar identificadores internos
