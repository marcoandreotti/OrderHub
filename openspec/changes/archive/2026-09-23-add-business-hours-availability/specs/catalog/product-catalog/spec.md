## ADDED Requirements

### Requirement: Oferta pode ficar temporariamente indisponível
O sistema SHALL permitir indisponibilizar produto ou opção por período ou até reativação, sem alterar seu cadastro nem seus históricos.

#### Scenario: Produto temporariamente indisponível
- **WHEN** o cardápio público é consultado durante a indisponibilidade
- **THEN** a oferta não pode ser adicionada a novo pedido e seu estado é comunicado conforme a política da unidade
