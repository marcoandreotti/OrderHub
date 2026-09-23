## 1. Domain and Contracts

- [ ] 1.1 Definir contratos de template, finalidade, consentimento e idempotência; verificar cenários por canal.
- [ ] 1.2 Implementar módulo Communications e persistência tenant-scoped; verificar preferências e histórico.

## 2. Application and Infrastructure

- [ ] 2.1 Implementar primeiro adapter em sandbox e tradução de estados; verificar timeout, falha e segredo protegido.
- [ ] 2.2 Consumir solicitações via outbox com retry; verificar que transação de negócio não depende do fornecedor.

## 3. Interfaces and Verification

- [ ] 3.1 Criar administração mínima e executar envio ponta a ponta sem duplicidade para cada canal habilitado.
- [ ] 3.2 Executar build, testes unitários, arquiteturais e de integração relevantes; verificar zero erros, ausência de warnings novos e propagação de CancellationToken.
- [ ] 3.3 Revisar isolamento Multi-Tenant, ProblemDetails, contratos externos e documentação; verificar a Definition of Done do repositório.
