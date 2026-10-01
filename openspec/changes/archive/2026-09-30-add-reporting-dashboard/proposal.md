## Why

Pedidos e pagamentos já geram dados úteis, mas gestores não possuem uma visão consolidada do desempenho. Relatórios transformam esses dados em decisões operacionais sem carregar aggregates para leitura.

## What Changes

- Fornecer indicadores de faturamento, pedidos, ticket médio e produtos.
- Permitir filtros por unidade e período com definições financeiras explícitas.
- Exibir comparações, séries temporais e estados vazios ou incompletos.
- Exportar dados tabulares dentro do escopo autorizado.

## Capabilities

### New Capabilities

- `reporting/business-dashboard`: projeções e visualizações gerenciais tenant-scoped.

### Modified Capabilities

Nenhuma.

## Impact

Read gateways Dapper, endpoints administrativos, nova área web e índices de consulta. A primeira versão usa leitura direta do PostgreSQL, sem data warehouse.
