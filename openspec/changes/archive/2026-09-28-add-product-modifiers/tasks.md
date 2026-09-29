## 1. Domain and Contracts

- [x] 1.1 Mapear adicionais atuais e definir migração compatível para `Additive`; verificar que dados e cálculo preço-base + adicionais permanecem iguais antes e depois.
- [x] 1.2 Evoluir domínio de grupos, opções, compatibilidades (`Requires` direcional e `Excludes` mútuo), estratégia de preço e fração exata; testar quatro estratégias, limites, regras entre grupos, composição integral e exclusividade de um grupo composto por produto.

## 2. Application and Infrastructure

- [x] 2.1 Persistir o modelo e snapshot explicável no pedido (grupo, estratégia, opções, preços, frações, valor por grupo e preço unitário); verificar atomicidade, preço e histórico imutável.
- [x] 2.2 Atualizar APIs administrativa e pública para estratégia e frações; verificar preço cliente não autoritativo, isolamento tenant e rejeição de opção/grupo inválidos.

## 3. Interfaces and Verification

- [x] 3.1 Atualizar manutenção do catálogo e compositor público; verificar ponta a ponta `HighestPrice` (R$ 50), `Proportional` (R$ 45 e R$ 50), borda/adicional (R$ 58 e R$ 63), remoção sem preço, composição inválida e apresentação conduzida por metadados.
- [x] 3.2 Executar build, testes unitários, arquiteturais e de integração relevantes; verificar zero erros, ausência de warnings novos e propagação de CancellationToken.
- [x] 3.3 Revisar isolamento Multi-Tenant, ProblemDetails, contratos externos e documentação; verificar a Definition of Done do repositório.
