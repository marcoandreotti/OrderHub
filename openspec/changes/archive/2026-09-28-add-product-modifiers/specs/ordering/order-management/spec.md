## ADDED Requirements

### Requirement: Pedido valida e preserva modificadores
O domínio MUST validar a composição vigente e calcular seu efeito monetário conforme as estratégias configuradas no catálogo. O preço da variação selecionada, ou o preço-base do produto, SHALL ser mantido quando não houver grupo de composição; com um grupo `HighestPrice` ou `Proportional`, seu resultado SHALL substituí-lo. Os valores de grupos `Additive` serão somados ao preço-base ou composto e grupos `NoPriceChange` não alterarão o preço. O domínio MUST preservar no snapshot do item os identificadores e nomes do produto, grupos e opções, preço-base considerado, frações exatas quando aplicáveis, quantidades, preços vigentes das opções, estratégia e valor calculado por grupo e preço unitário final.

O cálculo SHALL usar os preços recuperados pelo servidor e pertencentes ao tenant/estabelecimento do pedido. A quantidade do item do pedido multiplica o preço unitário final. A composição confirmada e seus valores SHALL permanecer imutáveis diante de mudanças posteriores no catálogo.

#### Scenario: Modificador muda após confirmação
- **WHEN** preço ou disponibilidade é alterado no catálogo
- **THEN** o pedido histórico permanece legível com a composição confirmada

#### Scenario: HighestPrice em dois sabores
- **WHEN** uma unidade é composta por 1/2 Calabresa a R$ 40,00 e 1/2 Portuguesa a R$ 50,00 usando `HighestPrice`
- **THEN** o grupo vale R$ 50,00

#### Scenario: Proportional em dois sabores
- **WHEN** uma unidade é composta por 1/2 Calabresa a R$ 40,00 e 1/2 Portuguesa a R$ 50,00 usando `Proportional`
- **THEN** o grupo vale R$ 45,00

#### Scenario: Proportional em três sabores
- **WHEN** uma unidade é composta por 1/3 Calabresa a R$ 42,00, 1/3 Portuguesa a R$ 48,00 e 1/3 Camarão a R$ 60,00 usando `Proportional`
- **THEN** o grupo vale R$ 50,00

#### Scenario: Composição, borda e adicional
- **WHEN** um grupo de sabores `HighestPrice` vale R$ 50,00, uma borda `Additive` custa R$ 8,00 e um adicional `Additive` custa R$ 5,00
- **THEN** o preço unitário é R$ 63,00

#### Scenario: Remoção sem efeito monetário
- **WHEN** uma opção de remoção usa `NoPriceChange`
- **THEN** a composição é preservada no snapshot e não altera o preço unitário
