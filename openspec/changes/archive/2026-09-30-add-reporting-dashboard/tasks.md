## 1. Domain and Contracts

- [x] 1.1 Definir formalmente indicadores, estados e datas de competência; verificar exemplos de cálculo.
- [x] 1.2 Implementar queries Dapper e índices necessários; verificar isolamento e planos de execução.

## 2. Application and Infrastructure

- [x] 2.1 Expor contratos paginados/agregados e exportação limitada; verificar autorização e equivalência de filtros.
- [x] 2.2 Construir dashboard com filtros, séries, comparações e estados vazios; verificar responsividade e acessibilidade.
- [x] 2.3 Integrar a composição visual do dashboard à experiência administrativa remodelada após os contratos de 1.1 e 2.1 estarem disponíveis, consumindo os indicadores retornados sem duplicar cálculos financeiros no frontend.

## 3. Interfaces and Verification

- [x] 3.1 Validar resultados contra conjunto conhecido de pedidos, pagamentos e cancelamentos.
- [x] 3.2 Executar build, testes unitários, arquiteturais e de integração relevantes; verificar zero erros, ausência de warnings novos e propagação de CancellationToken.
- [x] 3.3 Revisar isolamento Multi-Tenant, ProblemDetails, contratos externos e documentação; verificar a Definition of Done do repositório.
