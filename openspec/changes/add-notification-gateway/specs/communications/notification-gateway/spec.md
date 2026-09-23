## Purpose

Centraliza comunicações transacionais por canais substituíveis, respeitando consentimento, segurança, idempotência e rastreabilidade por Tenant.

## ADDED Requirements

### Requirement: Notificação usa template e canal autorizados
O sistema MUST resolver template, idioma, canal, destino e consentimento no contexto do Tenant antes de solicitar envio ao fornecedor.

#### Scenario: Canal sem consentimento
- **WHEN** a finalidade exige consentimento ausente
- **THEN** o envio não é tentado e o motivo é registrado

### Requirement: Tentativas são rastreáveis e idempotentes
Cada solicitação SHALL possuir chave idempotente, estado e histórico de tentativas, sem duplicar envio confirmado para a mesma finalidade.

#### Scenario: Resposta do fornecedor é incerta
- **WHEN** ocorre timeout após a solicitação
- **THEN** a recuperação consulta ou repete conforme capacidade do adapter sem criar duplicação evitável
