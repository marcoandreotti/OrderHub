## 1. Domain and Contracts

- [ ] 1.1 Modelar exceções, pausas e precedência no domínio; verificar fuso, sobreposição e virada de data em testes.
- [ ] 1.2 Persistir configurações e indisponibilidade tenant-scoped; verificar migração e integração PostgreSQL.

## 2. Application and Infrastructure

- [ ] 2.1 Implementar decisão autoritativa e revalidação na confirmação; verificar corrida com carrinho desatualizado.
- [ ] 2.2 Expor administração e projeções públicas; verificar contratos, autorização e motivos de indisponibilidade.

## 3. Interfaces and Verification

- [ ] 3.1 Atualizar as duas aplicações web e verificar horários, pausas, próxima abertura e produto indisponível ponta a ponta.
- [ ] 3.2 Executar build, testes unitários, arquiteturais e de integração relevantes; verificar zero erros, ausência de warnings novos e propagação de CancellationToken.
- [ ] 3.3 Revisar isolamento Multi-Tenant, ProblemDetails, contratos externos e documentação; verificar a Definition of Done do repositório.
