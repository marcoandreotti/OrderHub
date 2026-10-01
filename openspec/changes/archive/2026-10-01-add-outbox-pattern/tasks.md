## 1. Domain and Contracts

- [x] 1.1 Definir envelope versionado, idempotência e retenção; verificar testes de compatibilidade.
- [x] 1.2 Criar tabela, mapeamento e gravação na unidade de trabalho; verificar atomicidade com rollback.

## 2. Application and Infrastructure

- [x] 2.1 Implementar worker com leasing, lotes, backoff e falha terminal; verificar concorrência entre instâncias.
- [x] 2.2 Criar consumidor de referência idempotente e ferramentas de diagnóstico; verificar repetição após falha.

## 3. Interfaces and Verification

- [x] 3.1 Executar testes de integração de commit, rollback, retry e recuperação e documentar operação.
- [x] 3.2 Executar build, testes unitários, arquiteturais e de integração relevantes; verificar zero erros, ausência de warnings novos e propagação de CancellationToken.
- [x] 3.3 Revisar isolamento Multi-Tenant, ProblemDetails, contratos externos e documentação; verificar a Definition of Done do repositório.
