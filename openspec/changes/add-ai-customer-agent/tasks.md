## 1. Domain and Contracts

- [ ] 1.1 Modelar ameaças, consentimento, limites e tools permitidas; verificar revisão de segurança.
- [ ] 1.2 Implementar sessão tenant-scoped e tools somente sobre contratos públicos; verificar autorização e validação de argumentos.

## 2. Application and Infrastructure

- [ ] 2.1 Integrar provedor com proteção contra prompt injection e limites; verificar catálogo malicioso e falhas do modelo.
- [ ] 2.2 Implementar resumo autoritativo, confirmação explícita e handoff; verificar abandono sem criar pedido.

## 3. Interfaces and Verification

- [ ] 3.1 Executar avaliação com casos de preço, indisponibilidade, modificadores, entrega e agendamento antes de habilitar mutações.
- [ ] 3.2 Executar build, testes unitários, arquiteturais e de integração relevantes; verificar zero erros, ausência de warnings novos e propagação de CancellationToken.
- [ ] 3.3 Revisar isolamento Multi-Tenant, ProblemDetails, contratos externos e documentação; verificar a Definition of Done do repositório.
