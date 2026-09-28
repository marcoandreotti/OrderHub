# Kitchen Display System

O KDS está disponível em `/operations/kitchen` para usuários com a capacidade
`order-kitchen`. A unidade ativa sempre vem da sessão autenticada; a troca de
unidade limpa a fila anterior antes de carregar o novo snapshot.

## Contratos HTTP

- `GET /api/admin/establishments/{establishmentId}/kitchen` retorna somente
  pedidos `Confirmed` e `Preparing` da unidade autorizada.
- `POST /api/admin/establishments/{establishmentId}/orders/{orderId}/prepare`
  inicia o preparo.
- `POST /api/admin/establishments/{establishmentId}/orders/{orderId}/ready`
  conclui o preparo e remove o pedido da fila da cozinha.

Cada ticket contém a ação autorizada pelo servidor, itens, quantidades,
variações, modificadores, observações, instante de confirmação e, quando
aplicável, início do preparo. A API não expõe entidades de domínio nem aceita
`TenantId` do cliente.

## Ordenação e atualização

Pedidos em preparo têm precedência sobre pedidos aguardando preparo. Dentro de
cada etapa, a fila usa o instante de confirmação e o número do pedido como
ordenação estável. O painel destaca como atrasados os tickets confirmados há
15 minutos ou mais; esse destaque é visual e não altera o estado do pedido.

O painel prefere notificações realtime e mantém polling como fallback. O
polling pausa quando a página não está visível, não sobrepõe requisições e usa
backoff limitado após falhas. Uma atualização manual permanece disponível.

## Concorrência e erros

As transições são revalidadas no domínio e persistidas com controle otimista.
Conflitos retornam `application/problem+json` com HTTP 409. Ao receber esse
status, o frontend descarta qualquer expectativa local e recarrega a fila do
servidor.
