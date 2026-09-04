# Public Ordering Web Specification

## Purpose

Permite que visitantes realizem pedidos por uma interface web responsiva, segura e tematizada, desde o cardápio público até o acompanhamento.

## Requirements

### Requirement: Acesso público resolve o contexto da unidade
A aplicação SHALL carregar unidade, tema, disponibilidade, mesa quando informada e ofertas públicas a partir do slug e do token opaco presentes na URL, sem aceitar TenantId do visitante.

#### Scenario: QR Code de mesa válido
- **WHEN** o visitante abre uma URL com slug e token de mesa compatíveis
- **THEN** a aplicação apresenta a unidade e inicia o pedido associado à mesa resolvida pelo servidor

#### Scenario: Unidade indisponível
- **WHEN** o contexto público não existe, está inativo ou indisponível
- **THEN** a aplicação não exibe dados internos e informa que pedidos não podem ser realizados

### Requirement: Cardápio apresenta somente composição vendável
A aplicação SHALL exibir categorias, produtos, variações e adicionais retornados pela API pública, preservando preços e ordenação fornecidos.

#### Scenario: Produto com opções obrigatórias
- **WHEN** o visitante seleciona produto que exige adicionais mínimos
- **THEN** a aplicação impede inclusão até que uma seleção válida seja feita

### Requirement: Carrinho preserva intenção sem ser autoridade de preço
A aplicação SHALL manter localmente itens e escolhas para a mesma unidade, mas MUST usar a simulação do servidor como fonte autoritativa de disponibilidade, descontos e totais.

#### Scenario: Preço mudou após inclusão
- **WHEN** a simulação retorna valor diferente do exibido anteriormente
- **THEN** a aplicação atualiza o resumo e exige que o visitante veja o total atual antes de confirmar

### Requirement: Checkout coleta dados compatíveis com o atendimento
A aplicação SHALL solicitar somente os dados necessários para mesa, retirada ou entrega e SHALL permitir identificação e endereço conforme os contratos públicos.

#### Scenario: Entrega sem endereço
- **WHEN** o visitante escolhe entrega e tenta prosseguir sem endereço válido
- **THEN** a aplicação informa os campos necessários e não confirma o pedido

### Requirement: Cupom e pagamento são validados pelo servidor
A aplicação SHALL permitir informar cupom e selecionar apenas formas de pagamento públicas ativas, exibindo o resultado autoritativo da simulação.

#### Scenario: Cupom esgota após simulação
- **WHEN** o cupom deixa de ser elegível antes da confirmação
- **THEN** a aplicação apresenta o conflito retornado e oferece recalcular o pedido

### Requirement: Confirmação é idempotente
A aplicação MUST reutilizar a mesma chave idempotente enquanto repetir a confirmação da mesma intenção e SHALL criar nova chave somente após mudança material ou conclusão definitiva.

#### Scenario: Resposta perdida
- **WHEN** a confirmação foi processada, mas a resposta não chegou ao navegador
- **THEN** uma repetição segura conduz ao mesmo pedido sem duplicação

### Requirement: Visitante acompanha o pedido por referência pública
Após confirmar, a aplicação SHALL preservar a referência pública, exibir o estado e histórico permitidos e oferecer cancelamento somente enquanto aceito pelo servidor.

#### Scenario: Cancelamento não permitido
- **WHEN** o visitante solicita cancelamento após o estado permitido
- **THEN** a aplicação mantém o acompanhamento e explica que o pedido não pode mais ser cancelado

### Requirement: Jornada pública é responsiva e acessível
A aplicação SHALL operar em dispositivos móveis suportados, com foco visível, controles rotulados, contraste e comunicação de erros que não dependa somente de cor.

#### Scenario: Compra em tela móvel
- **WHEN** o visitante percorre cardápio, carrinho e checkout em viewport móvel
- **THEN** todas as informações e ações essenciais permanecem legíveis e acionáveis
