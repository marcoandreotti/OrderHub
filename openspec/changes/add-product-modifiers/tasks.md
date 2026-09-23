## 1. Domain and Contracts

- [ ] 1.1 Mapear adicionais atuais e definir migração compatível; verificar dados existentes antes e depois.
- [ ] 1.2 Evoluir domínio de grupos, opções e compatibilidades; verificar limites, sabores, bordas e remoções.

## 2. Application and Infrastructure

- [ ] 2.1 Persistir o modelo e snapshot no pedido; verificar atomicidade, preço e histórico.
- [ ] 2.2 Atualizar APIs administrativa e pública; verificar que preço enviado pelo cliente é ignorado.

## 3. Interfaces and Verification

- [ ] 3.1 Atualizar manutenção do catálogo e compositor público; verificar combinações válidas e inválidas ponta a ponta.
- [ ] 3.2 Executar build, testes unitários, arquiteturais e de integração relevantes; verificar zero erros, ausência de warnings novos e propagação de CancellationToken.
- [ ] 3.3 Revisar isolamento Multi-Tenant, ProblemDetails, contratos externos e documentação; verificar a Definition of Done do repositório.
