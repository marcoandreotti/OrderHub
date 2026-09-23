## Why

O sistema já possui fluxos suficientes para que falhas e degradações sejam difíceis de investigar apenas pelo comportamento visível. É necessário observar saúde, desempenho e correlação ponta a ponta antes da expansão operacional.

## What Changes

- Adicionar logs estruturados com correlação e contexto seguro de Tenant.
- Instrumentar métricas e tracing para HTTP, handlers, banco e integrações.
- Expor verificações separadas de liveness e readiness.
- Definir proteção de dados, cardinalidade e comportamento de degradação da telemetria.

## Capabilities

### New Capabilities

- `architecture/observability`: requisitos verificáveis de logs, métricas, tracing e health checks.

### Modified Capabilities

Nenhuma.

## Impact

Configuração transversal da API e Infrastructure, implantação Docker e testes operacionais. Pode adicionar bibliotecas de instrumentação, sem acoplar Domain a observabilidade.
