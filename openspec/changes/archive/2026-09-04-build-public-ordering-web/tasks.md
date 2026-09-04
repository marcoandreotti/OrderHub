## 1. Contexto e catálogo

- [x] 1.1 Estruturar módulo e rotas públicas por slug/token reutilizando layout e tema; verificar contexto válido, unidade indisponível e QR de mesa em testes.
- [x] 1.2 Implementar cliente público tipado e tratamento de ProblemDetails sem transportar TenantId; verificar contratos e falhas HTTP.
- [x] 1.3 Criar catálogo responsivo com categorias, produtos, variações e adicionais; verificar ordenação, itens inativos ausentes e limites de seleção.

## 2. Carrinho e simulação

- [x] 2.1 Implementar store versionada por unidade para itens, escolhas e observações; verificar restauração, troca de slug e migração/limpeza de estado inválido.
- [x] 2.2 Integrar simulação autoritativa de totais, cupom e forma de pagamento; verificar alteração de preço, cupom inelegível e produto indisponível.
- [x] 2.3 Criar carrinho e revisão com estados de carregamento/erro/vazio acessíveis; verificar mobile e navegação por teclado.

## 3. Checkout e confirmação

- [x] 3.1 Implementar identificação e endereço do cliente conforme tipo de atendimento; verificar mesa, retirada e entrega com combinações válidas/inválidas.
- [x] 3.2 Implementar seleção de pagamento e confirmação com máquina de estados e chave idempotente por intenção; verificar duplo clique, retry e edição material.
- [x] 3.3 Criar recibo de confirmação sem dados internos e preservar referência pública localmente; verificar retomada após recarregar a página.

## 4. Acompanhamento e qualidade

- [x] 4.1 Implementar acompanhamento por polling visível e cancelamento quando permitido; verificar estado terminal, backoff e cancelamento rejeitado.
- [x] 4.2 Cobrir percurso completo com testes de componentes e integração web/API, incluindo falha de rede e resposta perdida; verificar ausência de pedido duplicado.
- [x] 4.3 Executar typecheck, build web, testes relevantes e auditoria de acessibilidade; verificar zero erros/warnings relevantes e conformidade com specs/design.
