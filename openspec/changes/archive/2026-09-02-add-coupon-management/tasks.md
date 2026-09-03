## 1. Domínio de promoções

- [x] 1.1 Implementar Coupon, tipos de desconto, normalização e elegibilidade e verificar regras com testes de domínio
- [x] 1.2 Integrar snapshot e recálculo do desconto ao Order e verificar total não negativo nos testes de domínio

## 2. Aplicação

- [x] 2.1 Criar Commands, Queries internas, handlers e validators de cupons e verificar testes de Application
- [x] 2.2 Implementar aplicação, remoção e consumo no fluxo de confirmação e verificar revalidação após simulação

## 3. Persistência

- [x] 3.1 Mapear cupons e usos com EF Core e verificar unicidade de código por estabelecimento
- [x] 3.2 Implementar projeções Dapper e migration de Promotions e verificar upgrade, rollback e reapply
- [x] 3.3 Implementar controle concorrente do limite e verificar que somente um pedido consome o último uso

## 4. Verificação

- [x] 4.1 Testar atomicidade entre confirmação do pedido, snapshot e consumo do cupom em integração
- [x] 4.2 Executar build e testes relevantes e verificar zero erros e warnings relevantes
