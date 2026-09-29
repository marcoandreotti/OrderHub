## Why

A operação de pedidos ainda depende de polling e atualização manual, criando atraso e risco de pedidos não percebidos. Atualizações em tempo real tornam o painel operacional adequado ao trabalho contínuo sem eliminar a recuperação por consulta autoritativa.

## What Changes

- Adicionar canal SignalR autenticado e tenant-scoped para eventos de pedidos.
- Publicar notificações após mudanças persistidas, sem transportar entidades de domínio.
- Atualizar o painel imediatamente e reconciliar dados após reconexão.
- Manter polling controlado como fallback de degradação.

## Capabilities

### New Capabilities

- `operations/realtime-order-updates`: conexão, autorização, entrega e recuperação de atualizações operacionais em tempo real.

### Modified Capabilities

- `operations/order-operations-dashboard`: substituir o polling como mecanismo primário por sincronização em tempo real com fallback explícito.

## Impact

API, Application, Infrastructure, painel operacional, autenticação de conexões e configuração de implantação. Introduz SignalR, mas não mensageria externa.
