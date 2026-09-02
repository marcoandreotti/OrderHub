## Purpose

Define o catálogo vendável de cada unidade, incluindo hierarquia, variações, imagens e adicionais reutilizáveis.

## ADDED Requirements

### Requirement: Categorias formam uma hierarquia acíclica
Uma categoria SHALL pertencer a uma unidade, MAY possuir uma categoria pai da mesma unidade e MUST rejeitar vínculos que criem ciclo ou autorreferência.

#### Scenario: Categoria descendente usada como pai
- **WHEN** uma categoria for movida para baixo de uma de suas descendentes
- **THEN** o domínio MUST rejeitar a alteração e preservar a hierarquia anterior

#### Scenario: Categoria de outra unidade
- **WHEN** uma categoria pai pertencer a unidade diferente
- **THEN** o sistema MUST rejeitar o vínculo

### Requirement: Produto possui oferta base e variações opcionais
Cada produto SHALL pertencer a uma categoria da mesma unidade, possuir código único na unidade, preço base não negativo e MAY oferecer variações ativas com preço próprio não negativo.

#### Scenario: Variação selecionada
- **WHEN** um produto com variações for incluído em uma oferta
- **THEN** nome e preço da variação selecionada SHALL compor a oferta sem alterar o preço base cadastrado

### Requirement: Produto admite galeria ordenada
Um produto MAY possuir múltiplas imagens ordenadas e MUST possuir no máximo uma imagem principal.

#### Scenario: Segunda imagem principal
- **WHEN** uma imagem for marcada como principal e já existir outra principal
- **THEN** o sistema SHALL manter apenas uma imagem principal de forma consistente

### Requirement: Adicionais são reutilizáveis por unidade
Adicionais SHALL pertencer à unidade e MAY compor grupos vinculados a vários produtos; cada grupo MUST manter mínimo e máximo coerentes e somente referenciar adicionais da mesma unidade.

#### Scenario: Seleção fora dos limites
- **WHEN** a seleção de um grupo obrigatório não atender ao mínimo ou exceder o máximo
- **THEN** o sistema MUST rejeitar a composição do item

#### Scenario: Adicional cruzado
- **WHEN** um grupo tentar referenciar adicional de outra unidade
- **THEN** o sistema MUST rejeitar o vínculo

### Requirement: Itens inativos não são vendáveis
Categorias, produtos, variações, grupos ou adicionais inativos MUST NOT ser oferecidos para novas seleções públicas.

#### Scenario: Produto desativado após pedido anterior
- **WHEN** um produto for desativado
- **THEN** novos pedidos MUST NOT selecioná-lo e pedidos históricos SHALL permanecer legíveis
