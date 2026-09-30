## ADDED Requirements

### Requirement: Administração apresenta contexto e ações de forma consistente
A aplicação SHALL manter unidade ativa, navegação autorizada, título da página, descrição ou contexto necessário e ação principal em posições e hierarquias previsíveis entre os módulos administrativos.

#### Scenario: Gestor alterna entre módulos
- **WHEN** um gestor navega do catálogo para clientes, cupons ou pagamentos
- **THEN** a aplicação preserva a identificação da unidade e apresenta navegação, cabeçalho, filtros, feedback e ação principal com linguagem consistente

#### Scenario: Módulo sem ação de criação autorizada
- **WHEN** o usuário acessa um módulo no qual não possui capacidade de criação
- **THEN** o cabeçalho e a navegação permanecem consistentes sem apresentar uma ação indisponível como se fosse executável

### Requirement: Catálogo oferece visões visual e compacta equivalentes
A aplicação SHALL permitir alternar produtos do catálogo entre uma visão visual orientada por imagem e uma visão compacta orientada por comparação, preservando filtros, paginação, seleção e conjunto de resultados.

#### Scenario: Gestor alterna para visão visual
- **WHEN** o gestor pesquisa produtos e seleciona a visão visual
- **THEN** a aplicação apresenta imagem, nome, categoria, preço inicial, estado e resumo de variações ou grupos para os mesmos resultados filtrados

#### Scenario: Gestor alterna para visão compacta
- **WHEN** o gestor retorna à visão compacta
- **THEN** pesquisa, filtros, página atual e recursos ativos ou inativos permanecem aplicados sem nova interpretação do conjunto de dados

### Requirement: Personalização pública não altera a linguagem administrativa
A aplicação MUST manter contraste, tipografia, densidade, estados e ações administrativas consistentes independentemente do tema público da unidade selecionada. A identificação do Tenant MAY aparecer somente nos elementos previstos pela identidade administrativa.

#### Scenario: Gestor seleciona unidade com tema público expressivo
- **WHEN** o gestor seleciona uma unidade cujo cardápio usa cores, fonte ou raio personalizados
- **THEN** a administração mantém sua linguagem visual oficial e exibe apenas a identificação de marca permitida
