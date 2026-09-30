## ADDED Requirements

### Requirement: Cardápio favorece descoberta sem perder contexto
A aplicação SHALL permitir localizar produtos por busca e navegar por categorias preservando a identificação, disponibilidade e modalidade da unidade. Em telas móveis, o visitante MUST conseguir alternar categorias e continuar a jornada sem retornar ao início da página.

#### Scenario: Visitante busca produto no celular
- **WHEN** o visitante pesquisa um termo enquanto percorre o cardápio em viewport móvel
- **THEN** a aplicação apresenta somente resultados compatíveis e mantém unidade, disponibilidade, categorias e acesso ao carrinho compreensíveis

#### Scenario: Visitante troca de categoria após rolagem
- **WHEN** o visitante já percorreu produtos e seleciona outra categoria
- **THEN** a aplicação conduz ao conteúdo correspondente sem perder o contexto da unidade ou os itens do carrinho

### Requirement: Carrinho permanece acessível durante a compra
A aplicação SHALL manter um acesso perceptível ao carrinho enquanto houver itens, apresentando ao menos quantidade e total conhecido sem ocultar conteúdo ou ações essenciais.

#### Scenario: Carrinho com itens durante navegação móvel
- **WHEN** o visitante adiciona um produto e continua navegando pelo cardápio no celular
- **THEN** quantidade, total e ação para revisar o carrinho permanecem acessíveis sem exigir retorno ao topo

### Requirement: Composição extensa permanece operável em qualquer viewport
A aplicação SHALL apresentar produtos com variações, frações e adicionais em uma superfície adequada ao espaço disponível, mantendo requisitos, progresso da seleção, preço conhecido, quantidade e ação de inclusão perceptíveis durante a composição.

#### Scenario: Pizza com múltiplos grupos no celular
- **WHEN** o visitante configura uma pizza com sabores, borda e adicionais em viewport móvel
- **THEN** a aplicação mantém o grupo atual, o progresso da composição e a ação de adicionar legíveis e acionáveis durante todo o fluxo

#### Scenario: Composição inválida ao tentar adicionar
- **WHEN** o visitante aciona a inclusão sem completar um grupo obrigatório
- **THEN** a aplicação mantém a composição aberta, identifica o grupo incompleto e direciona a atenção para a correção necessária
