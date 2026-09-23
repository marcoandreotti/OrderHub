## ADDED Requirements

### Requirement: Produto define grupos de modificadores configuráveis
O catálogo SHALL representar grupos tipados de escolhas, incluindo sabores, bordas, adicionais e remoções, com limites, quantidades, compatibilidades e regras de preço tenant-scoped.

#### Scenario: Combinação incompatível
- **WHEN** uma composição viola limite ou compatibilidade configurada
- **THEN** o sistema rejeita a seleção sem alterar o produto cadastrado
