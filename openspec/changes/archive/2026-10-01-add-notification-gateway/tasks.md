## 1. Domain and Contracts

- [x] 1.1 Definir contratos de template, finalidade, consentimento e idempotência; verificar cenários por canal.
- [x] 1.2 Implementar módulo Communications e persistência tenant-scoped; verificar preferências e histórico.

## 2. Application and Infrastructure

- [x] 2.1 Implementar adapters SMTP sandbox e Meta WhatsApp Cloud API; traduzir estados e tratar timeout sem expor credenciais.
- [x] 2.2 Consumir solicitações via outbox com retry; verificar que transação de negócio não depende do fornecedor.

## 3. Interfaces and Verification

- [x] 3.1 Criar administração mínima e executar envio ponta a ponta sem duplicidade para cada canal habilitado.
- [x] 3.2 Executar build, typecheck e testes unitários, arquiteturais e de integração relevantes; verificar zero erros, ausência de warnings novos e propagação de CancellationToken.
- [x] 3.3 Revisar isolamento Multi-Tenant, ProblemDetails, contratos externos e documentação; verificar a Definition of Done do repositório.
