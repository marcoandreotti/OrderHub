## Why

Pedidos confirmados já possuem ciclo de vida e políticas por papel, porém a equipe do estabelecimento não tem uma visão operacional dedicada. O painel permitirá que atendimento, cozinha e entrega atuem com rapidez e dentro das transições autorizadas.

## What Changes

- Criar painel autenticado de pedidos organizado por estado e adequado às funções de atendimento, cozinha e entrega.
- Exibir detalhes operacionais, tipo de atendimento, itens, observações, pagamento e histórico necessários à execução.
- Permitir somente as transições oferecidas ao papel autenticado e tratar conflitos causados por alterações concorrentes.
- Atualizar a visão por polling com intervalo controlado, pausa quando a página estiver oculta e atualização manual.
- Destacar pedidos novos ou atrasados sem introduzir mensageria ou infraestrutura de tempo real nesta etapa.
- Adicionar filtros operacionais, feedback acessível e testes dos fluxos por papel.

## Capabilities

### New Capabilities

- `operations/order-operations-dashboard`: experiência operacional autenticada para visualizar e conduzir o ciclo dos pedidos.

### Modified Capabilities

Nenhuma. O painel utiliza as consultas, transições e políticas administrativas já especificadas.

## Impact

Afeta o frontend Quasar, estado e atualização periódica da visão operacional, integração com endpoints de pedidos e testes de interface/API. Depende de `add-administrative-mfa-authentication`; tempo real permanece fora do escopo até necessidade comprovada.
