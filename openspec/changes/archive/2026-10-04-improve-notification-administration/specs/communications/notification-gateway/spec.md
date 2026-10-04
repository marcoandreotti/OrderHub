## ADDED Requirements

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
