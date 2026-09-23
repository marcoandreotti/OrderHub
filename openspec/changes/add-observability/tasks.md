## 1. Domain and Contracts

- [ ] 1.1 Definir convenções de correlação, campos permitidos e cardinalidade; verificar testes contra PII.
- [ ] 1.2 Configurar logs estruturados e propagação de contexto nas bordas; verificar correlação em teste de integração.

## 2. Application and Infrastructure

- [ ] 2.1 Adicionar health checks de liveness/readiness; verificar comportamento com PostgreSQL indisponível.
- [ ] 2.2 Instrumentar métricas e traces de HTTP, handlers e banco; verificar exportação local e ausência de dependência no Domain.

## 3. Interfaces and Verification

- [ ] 3.1 Documentar dashboards/alertas mínimos e validar que falha do exportador não afeta a aplicação.
- [ ] 3.2 Executar build, testes unitários, arquiteturais e de integração relevantes; verificar zero erros, ausência de warnings novos e propagação de CancellationToken.
- [ ] 3.3 Revisar isolamento Multi-Tenant, ProblemDetails, contratos externos e documentação; verificar a Definition of Done do repositório.
