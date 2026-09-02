## Purpose

Define mesas e horários regulares que configuram os modos de atendimento presenciais e a disponibilidade da unidade.

## ADDED Requirements

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

