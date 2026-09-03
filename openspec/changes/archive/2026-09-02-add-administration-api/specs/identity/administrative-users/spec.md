## ADDED Requirements

### Requirement: Políticas administrativas mapeiam capacidades operacionais
As políticas SHALL conceder gestão a proprietário, administrador e gerente; atendimento a papéis configurados para pedidos; cozinha às transições de preparo; e entrega às transições de entrega, sempre limitadas às unidades associadas.

#### Scenario: Entregador finaliza entrega associada
- **WHEN** um entregador associado solicitar transição de entrega permitida
- **THEN** a API SHALL autorizar a ação sem conceder acesso às configurações administrativas
