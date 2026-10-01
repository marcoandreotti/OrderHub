## Purpose

Oferece indicadores gerenciais reproduzíveis sobre vendas e operação, usando projeções de leitura isoladas por Tenant e unidade.

## ADDED Requirements

### Requirement: Indicadores possuem definições consistentes
O sistema SHALL calcular faturamento, pedidos, ticket médio e produtos por período e unidade usando estados financeiros e operacionais explicitamente definidos.

As datas inicial e final informadas pelo usuário SHALL ser inclusivas e interpretadas no fuso horário configurado para a unidade; a consulta SHALL convertê-las em um intervalo UTC semiaberto. O período SHALL conter de 1 a 366 dias. Séries SHALL agrupar os valores pelo dia local da unidade.

Para este relatório, os indicadores SHALL ter estas definições:

- **Pedidos recebidos:** quantidade distinta de pedidos que entrou em `Confirmed` no período, conforme o histórico de status. Rascunhos não contam.
- **Vendas concluídas (faturamento):** soma do total final persistido em pedidos que entrou em `Completed` no período. O total já inclui descontos e taxas conforme o snapshot comercial do pedido. Pedidos cancelados ou rejeitados não compõem esse indicador, mesmo se tiverem pagamento confirmado.
- **Ticket médio:** faturamento de vendas concluídas dividido pela quantidade de pedidos concluídos no mesmo período; é zero quando não há pedidos concluídos.
- **Produtos vendidos:** soma das quantidades dos itens snapshot dos pedidos concluídos no período, agrupada pelo nome snapshot do produto.
- **Pagamentos confirmados:** soma dos valores de pagamentos em estado `Confirmed` cuja data `confirmed_at` está no período. Este indicador financeiro é separado de faturamento e não altera o estado operacional do pedido.
- **Pedidos cancelados/rejeitados:** quantidade distinta de pedidos que entrou em `Cancelled` ou `Rejected` no período, pela data da transição no histórico. Esse número é apresentado separadamente e nunca é subtraído ou contado como venda concluída.

Quando houver comparação, o período anterior SHALL ter a mesma quantidade de dias e terminar imediatamente antes da data inicial do período selecionado. Exportação e dashboard SHALL aplicar a mesma unidade, período, fuso, filtros e definições.

#### Scenario: Período contém pedido cancelado
- **WHEN** o indicador é calculado
- **THEN** o pedido não compõe faturamento, ticket médio ou produtos vendidos e sua transição é contabilizada somente em pedidos cancelados/rejeitados

#### Scenario: Pagamento confirmado pertence a pedido cancelado
- **WHEN** um pedido cancelado ou rejeitado possui pagamento confirmado no período
- **THEN** o valor aparece somente em pagamentos confirmados, sem ser reconhecido como venda concluída

#### Scenario: Unidade possui fuso diferente de UTC
- **WHEN** o usuário seleciona uma data de relatório
- **THEN** início, fim e agrupamento diário respeitam o fuso da unidade, inclusive em mudanças de offset

#### Scenario: Período anterior para comparação
- **WHEN** o período selecionado contém N dias
- **THEN** a comparação cobre exatamente N dias consecutivos imediatamente anteriores e não sobrepostos

### Requirement: Consulta e exportação respeitam autorização
Dashboard e exportações MUST derivar Tenant do usuário e limitar unidades às associações autorizadas.

A área administrativa SHALL disponibilizar `GET /api/admin/establishments/{establishmentId}/reports/dashboard` com datas inicial/final, modalidade opcional e paginação dos produtos. A API SHALL exigir capacidade de gestão, validar o escopo autenticado antes de consultar, rejeitar períodos fora de 1–366 dias e limitar a página de produtos a 100 itens (10 por padrão). A resposta SHALL conter os indicadores atuais e anteriores, série diária e metadados da página.

`GET /api/admin/establishments/{establishmentId}/reports/dashboard/export` SHALL produzir CSV com os mesmos filtros, definições, unidade e fuso. O arquivo SHALL limitar-se ao período máximo especificado, às duas séries agregadas e a no máximo 100 produtos, sem exportar dados de outra unidade.

#### Scenario: Exportação solicitada
- **WHEN** um gestor exporta uma visão filtrada
- **THEN** o arquivo contém os mesmos limites, período e escopo da consulta autorizada

#### Scenario: Leitura fora da unidade autorizada
- **WHEN** um usuário consulta o relatório para unidade fora de suas associações ativas
- **THEN** a API nega a operação sem retornar indicadores ou produtos

#### Scenario: Período excede o limite
- **WHEN** um usuário solicita mais de 366 dias
- **THEN** a API retorna ProblemDetails de validação antes de executar as agregações

#### Scenario: Paginação do ranking de produtos
- **WHEN** uma página válida do ranking é solicitada
- **THEN** a resposta preserva a contagem total, ordena por quantidade vendida decrescente e limita o tamanho máximo a 100
