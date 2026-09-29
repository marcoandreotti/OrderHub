## ADDED Requirements

### Requirement: Interface conduz composição válida
A aplicação SHALL renderizar grupos, limites, dependências, estratégia, preços e disponibilidade retornados pela API. Em grupos fracionários, SHALL permitir a composição integral suportada pelo catálogo e enviar numerador/denominador exatos. A interface poderá apresentar a prévia, mas não será autoridade de preço; simulação e confirmação usarão o cálculo da API. A aplicação MUST impedir avanço quando regras conhecidas não forem satisfeitas.

#### Scenario: Grupo obrigatório incompleto
- **WHEN** o visitante tenta adicionar o produto sem escolhas mínimas
- **THEN** a aplicação destaca o grupo e mantém o produto fora do carrinho

#### Scenario: Composição de múltiplos sabores
- **WHEN** o catálogo expõe um grupo fracionário com 2, 3 ou 4 opções por unidade
- **THEN** a aplicação coleta respectivamente metades, terços ou quartos e envia as frações exatas para validação e cálculo pela API

#### Scenario: Estratégia de preço apresentada
- **WHEN** a pessoa seleciona opções em grupos com estratégias diferentes
- **THEN** a interface apresenta os efeitos de preço descritos pela API sem implementar fórmulas específicas de sabor, borda ou remoção
