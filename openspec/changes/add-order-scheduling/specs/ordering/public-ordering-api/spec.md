## ADDED Requirements

### Requirement: API pública consulta e reserva slots na confirmação
A API SHALL expor slots válidos e MUST revalidar atomicamente o slot escolhido ao confirmar o pedido.

#### Scenario: Duas confirmações disputam a última capacidade
- **WHEN** pedidos concorrentes usam o mesmo slot limitado
- **THEN** somente a capacidade disponível é aceita e os demais recebem conflito recuperável
