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

### Requirement: Canais iniciais possuem adapters substituíveis
O sistema MUST oferecer adapters de e-mail por SMTP e WhatsApp pela Cloud API da Meta. O contrato interno MUST permitir adicionar canais futuros sem alterar templates, consentimentos e casos de uso existentes. Credenciais dos fornecedores MUST vir de configuração protegida de implantação e MUST NOT ser gravadas nas tabelas de comunicação ou expostas em logs.

#### Scenario: Fornecedor não configurado
- **WHEN** uma solicitação seleciona um canal sem credencial ou endereço configurado
- **THEN** a tentativa é marcada como falha recuperável sem expor o segredo e permanece rastreável

#### Scenario: Solicitação WhatsApp aceita pela Meta
- **WHEN** a Cloud API aceita uma mensagem de template
- **THEN** o identificador externo e o estado aceito pelo provedor são registrados sem afirmar entrega final

### Requirement: Preferências e templates são isolados por Tenant
Templates, consentimentos e histórico MUST ser associados ao Tenant autenticado e à unidade autorizada; uma consulta ou alteração não pode acessar dados de outro Tenant.

#### Scenario: Tenant acessa dado de outro Tenant
- **WHEN** uma solicitação de administração usa um identificador de unidade sem acesso autenticado
- **THEN** a operação é negada e nenhum dado de comunicação é retornado ou alterado

#### Scenario: Consentimento é revogado
- **WHEN** a pessoa destinatária revoga uma permissão registrada anteriormente
- **THEN** o histórico de consentimento é preservado e a decisão mais recente bloqueia novos envios que exigem consentimento
