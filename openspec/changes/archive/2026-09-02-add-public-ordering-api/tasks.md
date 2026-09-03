## 1. Contratos públicos

- [x] 1.1 Criar requests e responses explícitos para contexto, cliente, simulação, confirmação e acompanhamento e verificar que não expõem entidades ou TenantId
- [x] 1.2 Criar validators para composição, referências e idempotência e verificar ProblemDetails nos testes de API

## 2. Casos de uso públicos

- [x] 2.1 Implementar resolução pública por slug e token de mesa e verificar indisponibilidade de unidades inativas
- [x] 2.2 Implementar simulação e validação não reservante de cupom e verificar recálculo autoritativo
- [x] 2.3 Implementar confirmação pública idempotente e verificar reenvio sem duplicação do pedido
- [x] 2.4 Implementar acompanhamento e cancelamento por referência opaca e verificar limitação dos dados expostos

## 3. Endpoints e segurança

- [x] 3.1 Mapear endpoints públicos finos via dispatchers e verificar rotas e contratos no OpenAPI
- [x] 3.2 Testar isolamento, referências inválidas, preços manipulados e falhas atômicas em integração HTTP

## 4. Verificação

- [x] 4.1 Executar build e todas as suítes relevantes e verificar zero erros e warnings relevantes
