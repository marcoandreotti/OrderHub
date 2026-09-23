## Purpose

Oferece indicadores gerenciais reproduzíveis sobre vendas e operação, usando projeções de leitura isoladas por Tenant e unidade.

## ADDED Requirements

### Requirement: Indicadores possuem definições consistentes
O sistema SHALL calcular faturamento, pedidos, ticket médio e produtos por período e unidade usando estados financeiros e operacionais explicitamente definidos.

#### Scenario: Período contém pedido cancelado
- **WHEN** o indicador é calculado
- **THEN** o cancelamento é tratado conforme a definição exibida e não contado silenciosamente como venda concluída

### Requirement: Consulta e exportação respeitam autorização
Dashboard e exportações MUST derivar Tenant do usuário e limitar unidades às associações autorizadas.

#### Scenario: Exportação solicitada
- **WHEN** um gestor exporta uma visão filtrada
- **THEN** o arquivo contém os mesmos limites, período e escopo da consulta autorizada
