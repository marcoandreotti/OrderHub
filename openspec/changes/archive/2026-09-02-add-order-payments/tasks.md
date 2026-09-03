## 1. Domínio financeiro

- [x] 1.1 Implementar PaymentMethod tenant-scoped e verificar ativação, código e preservação histórica com testes de domínio
- [x] 1.2 Implementar Payment e transições financeiras e verificar valores, troco e estados com testes de domínio

## 2. Aplicação

- [x] 2.1 Criar Commands, Queries internas, handlers e validators de formas e pagamentos e verificar testes de Application
- [x] 2.2 Implementar confirmação idempotente com detecção de payload divergente e verificar repetição sem efeito duplicado
- [x] 2.3 Implementar cálculo de cobertura contra o total autoritativo do pedido e verificar independência do status operacional

## 3. Persistência

- [x] 3.1 Mapear formas, pagamentos e idempotência com EF Core e verificar constraints tenant-scoped
- [x] 3.2 Implementar projeções Dapper e migration de Payments e verificar upgrade, rollback e reapply
- [x] 3.3 Serializar confirmações concorrentes por pedido e verificar que o valor confirmado não excede o devido

## 4. Verificação

- [x] 4.1 Executar build e testes Domain/Application/Integration/Architecture e verificar zero erros e warnings relevantes
