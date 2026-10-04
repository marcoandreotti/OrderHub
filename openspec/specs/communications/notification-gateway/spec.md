# Notification Gateway Specification

## Purpose

Centraliza comunicações transacionais por canais substituíveis, respeitando consentimento, segurança, idempotência e rastreabilidade por Tenant.

## Requirements

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

### Requirement: Administração explica o fluxo de notificações
A interface administrativa SHALL explicar como modelos, consentimentos e solicitações de envio se relacionam e SHALL distinguir aceitação do provedor da entrega final.

#### Scenario: Configurar um modelo
- **WHEN** uma pessoa administra modelos de notificação
- **THEN** a interface explica finalidade, canal, idioma e campos de conteúdo
- **AND** apresenta as finalidades e campos de pedido público suportados

#### Scenario: Registrar consentimento
- **WHEN** uma pessoa registra ou revoga consentimento
- **THEN** a interface explica destino, canal, finalidade e origem da autorização
- **AND** comunica que o histórico anterior é preservado

#### Scenario: Solicitar envio
- **WHEN** uma pessoa solicita uma notificação
- **THEN** a interface informa que a solicitação será validada e colocada na fila
- **AND** não afirma que a mensagem foi entregue ao destinatário

### Requirement: Fluxo administrativo de notificações é responsivo e compreensível
A interface SHALL apresentar modelos da unidade, consentimentos e histórico de envios em abas identificáveis. Edição e criação de modelos, ajuda contextual e solicitação de envio SHALL ser apresentadas em modais focados. A explicação geral do fluxo SHALL ser acessível por um ícone discreto junto ao texto introdutório.

#### Scenario: Revisar modelos
- **WHEN** a pessoa abre a aba de modelos
- **THEN** vê finalidade, canal, idioma, exigência de consentimento e estado de cada modelo
- **AND** pode criar ou editar um modelo em um modal

#### Scenario: Revisar consentimentos
- **WHEN** há vários eventos para o mesmo destino, canal e finalidade
- **THEN** a lista principal mostra somente o estado mais recente
- **AND** permite consultar os eventos anteriores e registrar revogação
- **AND** não permite excluir eventos de auditoria

#### Scenario: Consultar histórico de envios
- **WHEN** a pessoa informa um período
- **THEN** a API filtra as solicitações por data inicial e final
- **AND** a pessoa pode abrir as tentativas registradas
- **AND** a interface não oferece reenvio ou exclusão de solicitações

#### Scenario: Tela estreita
- **WHEN** a tela é usada em smartphone ou em uma janela estreita
- **THEN** os formulários e histórico se reorganizam sem exigir rolagem horizontal da página para encontrar ações
