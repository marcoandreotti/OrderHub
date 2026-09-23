## ADDED Requirements

### Requirement: API pública decide disponibilidade de forma autoritativa
A API MUST avaliar unidade, modalidade, instante e composição ao consultar ofertas e novamente ao confirmar o pedido, sem confiar em disponibilidade calculada pelo cliente.

#### Scenario: Disponibilidade muda antes da confirmação
- **WHEN** a modalidade ou oferta se torna indisponível após entrar no carrinho
- **THEN** a confirmação é rejeitada sem persistir pedido parcial e informa os itens afetados
