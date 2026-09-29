## Purpose

Mantém clientes operacionais autorizados sincronizados com mudanças de pedidos da unidade, com recuperação segura após interrupções.

## ADDED Requirements

### Requirement: Conexão em tempo real respeita autorização da unidade
O sistema MUST autenticar a conexão, derivar Tenant e unidades autorizadas no servidor e MUST NOT enviar eventos de outra unidade.

#### Scenario: Associação revogada
- **WHEN** a associação do operador deixa de ser válida
- **THEN** novas inscrições ou entregas para a unidade são negadas

### Requirement: Eventos permitem reconciliação autoritativa
Cada evento SHALL identificar pedido, tipo de mudança, unidade e versão ou instante suficientes para o cliente decidir quando recarregar a projeção autoritativa.

#### Scenario: Cliente reconecta após interrupção
- **WHEN** a conexão é restabelecida sem garantia de continuidade
- **THEN** o cliente recarrega os pedidos antes de considerar a visão sincronizada
