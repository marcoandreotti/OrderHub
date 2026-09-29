## ADDED Requirements

### Requirement: API expõe regras e não aceita preço de modificador do cliente
A API SHALL fornecer grupos, limites, compatibilidades, estratégia de preço, preços de catálogo e disponibilidade necessários à composição. Ao simular ou confirmar o carrinho, receberá identificadores de produto, grupo e opção, quantidade para opções aditivas e numerador/denominador para escolhas fracionárias; não aceitará do cliente preço-base, preço de opção, preço de grupo, desconto ou preço final como fonte autoritativa. O servidor SHALL resolver preços vigentes com isolamento de tenant, validar associação entre produto/grupo/opção, limites, compatibilidades e soma exata das frações, e calcular o valor autoritativo.

#### Scenario: Cliente envia preço adulterado
- **WHEN** o valor informado diverge da regra vigente
- **THEN** o sistema ignora o valor do cliente e usa o cálculo autoritativo

#### Scenario: Frações de composição não totalizam uma unidade
- **WHEN** um grupo configurado como composição integral recebe frações cuja soma exata difere de 1
- **THEN** a API rejeita a composição sem criar ou confirmar o pedido

#### Scenario: Opção não pertence ao grupo do produto
- **WHEN** o cliente envia uma opção que não pertence ao grupo associado ao produto no estabelecimento autenticado
- **THEN** a API rejeita a composição sem revelar dados de outro tenant

#### Scenario: Dependência entre grupos não satisfeita
- **WHEN** uma opção selecionada exige outra opção de um grupo associado ao produto e ela está ausente
- **THEN** a API rejeita a simulação ou confirmação sem persistência parcial

#### Scenario: Opções excluídas selecionadas juntas
- **WHEN** a composição inclui duas opções vinculadas por `Excludes`
- **THEN** a API rejeita a simulação ou confirmação sem persistência parcial

#### Scenario: Preço-base e preço composto
- **WHEN** a API calcula um produto sem grupo de composição ou com grupo `HighestPrice`/`Proportional`
- **THEN** usa respectivamente o preço da variação/produto ou o resultado do grupo como base, somando em ambos os casos os grupos `Additive`
