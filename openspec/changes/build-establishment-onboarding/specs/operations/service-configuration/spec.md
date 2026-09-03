## ADDED Requirements

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
