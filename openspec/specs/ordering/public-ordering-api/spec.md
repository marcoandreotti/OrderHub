# Public Ordering API Specification

## Purpose

Define a borda HTTP anônima que permite ao visitante compor, confirmar, acompanhar e cancelar pedidos sem expor identificadores internos ou confiar em cálculos do cliente.

## Requirements

### Requirement: API pública resolve o estabelecimento no servidor
Toda operação pública SHALL resolver Tenant e estabelecimento por slug ativo ou token público opaco e MUST NOT aceitar TenantId como autoridade enviado pelo cliente.

#### Scenario: Slug de unidade inativa
- **WHEN** uma requisição usar slug de Tenant ou estabelecimento inativo
- **THEN** a API MUST NOT revelar dados nem permitir criação de pedido

### Requirement: Criação pública de pedido é idempotente
A confirmação pública MUST exigir chave idempotente, recalcular composição e totais no servidor e retornar referência pública opaca.

#### Scenario: Cliente repete confirmação
- **WHEN** a mesma confirmação for reenviada com a mesma chave e conteúdo
- **THEN** a API SHALL retornar o pedido originalmente criado sem duplicação

### Requirement: Acompanhamento público limita dados expostos
A consulta por referência pública SHALL retornar somente informações necessárias ao cliente, incluindo composição, totais e histórico apresentável, sem dados administrativos ou internos.

#### Scenario: Referência inválida
- **WHEN** uma referência pública inexistente ou alterada for consultada
- **THEN** a API MUST NOT revelar se pedidos próximos existem

### Requirement: Erros públicos seguem ProblemDetails
Falhas de validação, conflito, indisponibilidade e regra de domínio SHALL produzir ProblemDetails consistente sem stack trace ou detalhes de infraestrutura.

#### Scenario: Composição inválida
- **WHEN** o visitante confirmar seleção que viola limites de adicionais
- **THEN** a API SHALL rejeitar a operação com resposta padronizada e sem persistência parcial

### Requirement: API pública decide disponibilidade de forma autoritativa
A API MUST avaliar unidade, modalidade, instante e composição ao consultar ofertas e novamente ao confirmar o pedido, sem confiar em disponibilidade calculada pelo cliente.

#### Scenario: Disponibilidade muda antes da confirmação
- **WHEN** a modalidade ou oferta se torna indisponível após entrar no carrinho
- **THEN** a confirmação é rejeitada sem persistir pedido parcial e informa os itens afetados

### Requirement: Fluxo público cota e confirma entrega autoritativamente
A API SHALL cotar endereço e MUST recalcular sua elegibilidade e taxa ao confirmar, rejeitando cota expirada ou incompatível.

#### Scenario: Taxa muda entre cotação e confirmação
- **WHEN** a política vigente produz valor diferente
- **THEN** a API não confirma silenciosamente e retorna o total autoritativo atualizado

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
