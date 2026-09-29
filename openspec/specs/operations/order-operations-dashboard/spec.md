# Order Operations Dashboard Specification

## Purpose

Fornece às equipes de atendimento, cozinha e entrega uma visão operacional autenticada para acompanhar e avançar pedidos com segurança.

## Requirements

### Requirement: Painel mostra somente pedidos da unidade autorizada
O painel MUST exigir sessão válida e SHALL consultar pedidos exclusivamente para a unidade selecionada entre as associações ativas do usuário.

#### Scenario: Troca de unidade
- **WHEN** o operador troca para outra unidade autorizada
- **THEN** pedidos da unidade anterior são removidos antes de carregar a nova visão

### Requirement: Pedidos são organizados para execução
O painel SHALL organizar pedidos por estado e apresentar número, tempo, atendimento, itens, observações e situação financeira necessários ao papel do operador.

#### Scenario: Novo pedido confirmado
- **WHEN** a atualização encontra pedido confirmado ainda não exibido
- **THEN** o painel o destaca na etapa adequada sem alterar seu estado automaticamente

### Requirement: Ações respeitam papel e estado vigente
O painel SHALL oferecer somente transições compatíveis com o papel e o estado conhecidos, e a API MUST revalidar ambos no momento da ação.

#### Scenario: Estado mudou em outro terminal
- **WHEN** o operador tenta uma transição baseada em estado desatualizado
- **THEN** o painel apresenta o conflito e recarrega o pedido sem ocultar a decisão do servidor

### Requirement: Atualização periódica é controlada
O painel SHALL atualizar pedidos por polling configurável, pausar chamadas quando a página não estiver visível, impedir ciclos sobrepostos e permitir atualização manual.

#### Scenario: Requisição demora além do intervalo
- **WHEN** uma atualização ainda está em andamento no próximo intervalo
- **THEN** o painel não inicia uma segunda atualização concorrente

### Requirement: Falhas não interrompem silenciosamente a operação
O painel SHALL indicar perda de atualização, manter visível o instante da última sincronização bem-sucedida e tentar novamente com intervalo progressivo limitado.

#### Scenario: API temporariamente indisponível
- **WHEN** atualizações consecutivas falham
- **THEN** dados existentes são marcados como possivelmente desatualizados e o operador recebe ação de nova tentativa

### Requirement: Operação é acessível em desktop e tablet
O painel SHALL manter ações, estados e alertas distinguíveis por texto/ícone além de cor e navegáveis por teclado nos dispositivos suportados.

#### Scenario: Alteração de estado por teclado
- **WHEN** o operador seleciona e confirma uma transição usando teclado
- **THEN** foco, confirmação e resultado permanecem perceptíveis

### Requirement: Painel prioriza atualização em tempo real com fallback
O painel SHALL aplicar notificações em tempo real, indicar o estado da conexão e SHALL usar reconciliação por consulta ou polling controlado enquanto o canal estiver indisponível.

#### Scenario: Canal em tempo real indisponível
- **WHEN** a conexão falha ou é interrompida
- **THEN** o painel mantém dados visíveis, sinaliza possível defasagem e inicia o fallback sem ciclos concorrentes

### Requirement: Painel separa demanda futura da demanda executável
O painel SHALL identificar pedidos agendados, ordenar pelo momento prometido e evitar que pedidos distantes ocultem trabalho imediato.

#### Scenario: Pedido aproxima-se da janela de produção
- **WHEN** o horário operacional configurado é alcançado
- **THEN** o pedido passa a receber destaque na fila sem mudança automática de status
