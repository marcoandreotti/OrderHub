## 1. Domain and Contracts

- [x] 1.1 Definir DTOs versionáveis e porta de publicação; verificar testes de serialização e ausência de entidades.
- [x] 1.2 Implementar hub SignalR, autenticação e grupos por unidade; verificar testes de isolamento Multi-Tenant.

## 2. Application and Infrastructure

- [x] 2.1 Publicar sinais somente após persistência confirmada; verificar testes de sucesso e rollback.
- [x] 2.2 Integrar painel com conexão, reconciliação e fallback; verificar testes de reconexão e polling sem sobreposição.

## 3. Interfaces and Verification

- [x] 3.1 Configurar implantação e telemetria da conexão; verificar cenário ponta a ponta com dois usuários e duas unidades.
- [x] 3.2 Executar build, testes unitários, arquiteturais e de integração relevantes; verificar zero erros, ausência de warnings novos e propagação de CancellationToken.
- [x] 3.3 Revisar isolamento Multi-Tenant, ProblemDetails, contratos externos e documentação; verificar a Definition of Done do repositório.
