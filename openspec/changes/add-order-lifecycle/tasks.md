## 1. Aggregate de pedido

- [ ] 1.1 Implementar Order, itens, adicionais e tipos de atendimento e verificar composição e escopo com testes de domínio
- [ ] 1.2 Implementar snapshots e cálculo autoritativo de totais e verificar arredondamento e total não negativo
- [ ] 1.3 Implementar máquina de estados e histórico imutável e verificar todas as transições válidas e inválidas

## 2. Aplicação

- [ ] 2.1 Criar portas para resolver ofertas, clientes, mesas e sequência e verificar isolamento em testes de handlers
- [ ] 2.2 Criar Commands, handlers e validators de composição, confirmação e transições e verificar CancellationToken e atomicidade
- [ ] 2.3 Criar Queries/read models internos de pedido e verificar snapshots históricos nas projeções

## 3. Persistência

- [ ] 3.1 Mapear aggregate e histórico com EF Core e verificar constraints tenant-scoped nos testes de modelo
- [ ] 3.2 Implementar contador monotônico concorrente por estabelecimento e verificar números distintos em teste de integração
- [ ] 3.3 Implementar gateways Dapper e migration de Ordering e verificar upgrade, rollback e reapply

## 4. Verificação

- [ ] 4.1 Executar build e suítes Domain/Application/Integration/Architecture e verificar zero erros e warnings relevantes
