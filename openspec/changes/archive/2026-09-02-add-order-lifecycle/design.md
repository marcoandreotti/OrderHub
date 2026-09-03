## Context

Catálogo, operações e clientes fornecem os dados necessários, mas o modelo atual não materializa pedidos. A mudança introduz o principal aggregate transacional e seu histórico.

## Goals / Non-Goals

**Goals:**

- Proteger composição, snapshots, totais, numeração e transições no domínio.
- Persistir confirmação e histórico atomicamente.
- Oferecer portas de leitura adequadas às APIs futuras.

**Non-Goals:**

- Aplicar cupons, confirmar pagamentos ou expor endpoints HTTP.
- Introduzir mensageria ou processamento distribuído.

## Decisions

### Pedido como aggregate root

Itens, adicionais, snapshot de entrega e histórico imediato serão controlados pelo pedido. Dividi-los em aggregates independentes enfraqueceria invariantes transacionais sem benefício no monólito modular atual.

### Snapshots comerciais explícitos

O pedido copiará nomes e preços relevantes no momento da confirmação. Referenciar apenas o catálogo atual reduziria armazenamento, mas quebraria o histórico após edições.

### Sequência por estabelecimento no PostgreSQL

Uma estrutura de contador tenant-scoped será atualizada atomicamente na mesma transação de confirmação. Calcular `MAX + 1` foi rejeitado por condição de corrida.

### Máquina de estados no domínio

Métodos de transição explícitos protegerão caminhos válidos e criarão entradas históricas. Um setter genérico de status permitiria combinações inválidas.

## Risks / Trade-offs

- [Aggregate cresce com itens e histórico] → Carregar aggregate somente no fluxo de escrita e usar Dapper nas consultas.
- [Concorrência em pedidos do mesmo estabelecimento] → Usar token de concorrência e contador atualizado atomicamente.

## Migration Plan

Criar tabelas de pedidos, itens, adicionais, histórico e contadores; validar upgrade, rollback e reapply antes de habilitar handlers.
