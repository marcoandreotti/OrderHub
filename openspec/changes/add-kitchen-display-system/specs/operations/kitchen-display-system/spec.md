## Purpose

Organiza o trabalho da cozinha em uma fila tenant-scoped, orientada por preparo, prioridade e tempo prometido dos pedidos.

## ADDED Requirements

### Requirement: Fila contém somente trabalho autorizado e preparável
O KDS MUST mostrar somente pedidos da unidade autorizada em estados compatíveis, com itens, quantidades, modificadores e observações necessários à produção.

#### Scenario: Operador troca de unidade
- **WHEN** o operador seleciona outra unidade autorizada
- **THEN** o trabalho anterior é removido antes de carregar a nova fila

### Requirement: Ações respeitam estado vigente
O KDS SHALL permitir iniciar e concluir preparo somente conforme papel e transições válidas, e MUST tratar conflitos recarregando o estado do servidor.

#### Scenario: Outro terminal avança o pedido
- **WHEN** uma ação usa estado desatualizado
- **THEN** o sistema rejeita a transição e apresenta o estado atual sem duplicar histórico
