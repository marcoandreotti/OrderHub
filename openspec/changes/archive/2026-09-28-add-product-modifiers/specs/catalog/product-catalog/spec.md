## ADDED Requirements

### Requirement: Produto define grupos de modificadores configuráveis
O catálogo SHALL representar grupos tipados e tenant-scoped de escolhas, incluindo sabores, bordas, adicionais e remoções, com limites de seleção, quantidades, compatibilidades e uma estratégia de preço. A estratégia pertence ao grupo e SHALL ser uma das seguintes: `Additive`, `HighestPrice`, `Proportional` ou `NoPriceChange`. Grupos existentes SHALL ser migrados como `Additive`, sem mudar o comportamento de preço atual.

Cada associação de opção a grupo MAY definir regras para uma associação alvo (grupo + opção) do mesmo estabelecimento. A regra `Requires` SHALL ser direcional: selecionar a origem exige selecionar o alvo. A regra `Excludes` SHALL impedir a seleção conjunta das duas opções, independentemente da direção do cadastro. Produtos SHALL associar os grupos necessários para satisfazer todas as referências de suas regras. Referências inexistentes, cruzadas entre tenants/estabelecimentos ou incompatíveis com os grupos do produto SHALL ser rejeitadas.

`Additive` SHALL calcular o valor do grupo pela soma do preço vigente de cada opção multiplicado por sua quantidade selecionada. `HighestPrice` SHALL usar o maior preço vigente entre as opções selecionadas, sem ponderação por fração. `Proportional` SHALL somar o preço vigente de cada opção multiplicado por sua fração exata da unidade composta. `NoPriceChange` SHALL produzir valor zero. Valores proporcionais SHALL ser calculados com frações exatas e convertidos para moeda arredondando uma única vez no total do grupo, conforme a regra de `Money`.

Grupos `HighestPrice` e `Proportional` SHALL representar um único grupo de composição integral. Opções selecionadas nesses grupos SHALL informar frações positivas como numerador/denominador, sem aproximação decimal, e uma quantidade por opção. Quando configurada como integral, a soma exata das frações SHALL ser igual a 1. O catálogo SHALL permitir inicialmente composições 1/1, 1/2 + 1/2, três opções de 1/3 e quatro opções de 1/4. Grupos `Additive` SHALL usar quantidades e não frações. Um produto SHALL associar no máximo um grupo `HighestPrice` ou `Proportional`; grupos `Additive` e `NoPriceChange` não concorrem para definir o preço composto.

#### Scenario: Combinação incompatível
- **WHEN** uma composição viola limite ou compatibilidade configurada
- **THEN** o sistema rejeita a seleção sem alterar o produto cadastrado

#### Scenario: Grupo de composição substitui preço-base
- **WHEN** um produto possui um grupo `HighestPrice` ou `Proportional` associado e uma composição válida é selecionada
- **THEN** o resultado do grupo define o preço-base composto da unidade; sem grupo de composição, permanece o preço da variação selecionada ou o preço-base do produto

#### Scenario: Grupo existente mantém preço
- **WHEN** um grupo de adicionais existente é migrado
- **THEN** sua estratégia passa a `Additive` e o preço do produto/variação continua sendo somado aos adicionais selecionados

#### Scenario: Opção exige escolha em outro grupo
- **WHEN** uma opção selecionada possui regra `Requires` para opção de outro grupo e a opção-alvo não foi selecionada
- **THEN** o sistema rejeita a composição

#### Scenario: Opções mutuamente incompatíveis
- **WHEN** duas opções ligadas por `Excludes` são selecionadas juntas
- **THEN** o sistema rejeita a composição, mesmo que a regra tenha sido cadastrada em apenas uma das opções
