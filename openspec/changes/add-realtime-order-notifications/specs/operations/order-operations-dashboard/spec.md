## ADDED Requirements

### Requirement: Painel prioriza atualização em tempo real com fallback
O painel SHALL aplicar notificações em tempo real, indicar o estado da conexão e SHALL usar reconciliação por consulta ou polling controlado enquanto o canal estiver indisponível.

#### Scenario: Canal em tempo real indisponível
- **WHEN** a conexão falha ou é interrompida
- **THEN** o painel mantém dados visíveis, sinaliza possível defasagem e inicia o fallback sem ciclos concorrentes
