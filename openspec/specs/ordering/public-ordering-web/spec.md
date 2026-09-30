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

### Requirement: Interface comunica indisponibilidade acionável
A aplicação SHALL distinguir unidade fechada, modalidade pausada e oferta indisponível, informar próxima abertura quando conhecida e impedir envio sabidamente inválido.

#### Scenario: Unidade fechada
- **WHEN** o visitante acessa o cardápio fora da disponibilidade
- **THEN** a navegação pode permanecer visível, mas a confirmação imediata é bloqueada com motivo claro

### Requirement: Checkout apresenta cobertura e custo da entrega
A aplicação SHALL coletar endereço suficiente, apresentar taxa e estimativa retornadas pelo servidor e exigir aceite de qualquer alteração antes da confirmação.

#### Scenario: Endereço não atendido
- **WHEN** a cotação indicar ausência de cobertura
- **THEN** a aplicação impede entrega e oferece somente modalidades disponíveis

### Requirement: Interface conduz composição válida
A aplicação SHALL renderizar grupos, limites, dependências, estratégia, preços e disponibilidade retornados pela API. Em grupos fracionários, SHALL permitir a composição integral suportada pelo catálogo e enviar numerador/denominador exatos. A interface poderá apresentar a prévia, mas não será autoridade de preço; simulação e confirmação usarão o cálculo da API. A aplicação MUST impedir avanço quando regras conhecidas não forem satisfeitas.

#### Scenario: Grupo obrigatório incompleto
- **WHEN** o visitante tenta adicionar o produto sem escolhas mínimas
- **THEN** a aplicação destaca o grupo e mantém o produto fora do carrinho

#### Scenario: Composição de múltiplos sabores
- **WHEN** o catálogo expõe um grupo fracionário com 2, 3 ou 4 opções por unidade
- **THEN** a aplicação coleta respectivamente metades, terços ou quartos e envia as frações exatas para validação e cálculo pela API

#### Scenario: Estratégia de preço apresentada
- **WHEN** a pessoa seleciona opções em grupos com estratégias diferentes
- **THEN** a interface apresenta os efeitos de preço descritos pela API sem implementar fórmulas específicas de sabor, borda ou remoção

### Requirement: Cliente escolhe somente horários retornados pelo servidor
A aplicação SHALL apresentar opção imediata ou agendada conforme modalidade, unidade e slots vigentes e SHALL comunicar fuso e prazo esperado.

#### Scenario: Slot expira no checkout
- **WHEN** a confirmação rejeita o slot selecionado
- **THEN** a aplicação preserva o carrinho e solicita nova escolha

### Requirement: Cardápio favorece descoberta sem perder contexto
A aplicação SHALL permitir localizar produtos por busca e navegar por categorias preservando a identificação, disponibilidade e modalidade da unidade. Em telas móveis, o visitante MUST conseguir alternar categorias e continuar a jornada sem retornar ao início da página.

#### Scenario: Visitante busca produto no celular
- **WHEN** o visitante pesquisa um termo enquanto percorre o cardápio em viewport móvel
- **THEN** a aplicação apresenta somente resultados compatíveis e mantém unidade, disponibilidade, categorias e acesso ao carrinho compreensíveis

#### Scenario: Visitante troca de categoria após rolagem
- **WHEN** o visitante já percorreu produtos e seleciona outra categoria
- **THEN** a aplicação conduz ao conteúdo correspondente sem perder o contexto da unidade ou os itens do carrinho

### Requirement: Carrinho permanece acessível durante a compra
A aplicação SHALL manter um acesso perceptível ao carrinho enquanto houver itens, apresentando ao menos quantidade e total conhecido sem ocultar conteúdo ou ações essenciais.

#### Scenario: Carrinho com itens durante navegação móvel
- **WHEN** o visitante adiciona um produto e continua navegando pelo cardápio no celular
- **THEN** quantidade, total e ação para revisar o carrinho permanecem acessíveis sem exigir retorno ao topo

### Requirement: Composição extensa permanece operável em qualquer viewport
A aplicação SHALL apresentar produtos com variações, frações e adicionais em uma superfície adequada ao espaço disponível, mantendo requisitos, progresso da seleção, preço conhecido, quantidade e ação de inclusão perceptíveis durante a composição.

#### Scenario: Pizza com múltiplos grupos no celular
- **WHEN** o visitante configura uma pizza com sabores, borda e adicionais em viewport móvel
- **THEN** a aplicação mantém o grupo atual, o progresso da composição e a ação de adicionar legíveis e acionáveis durante todo o fluxo

#### Scenario: Composição inválida ao tentar adicionar
- **WHEN** o visitante aciona a inclusão sem completar um grupo obrigatório
- **THEN** a aplicação mantém a composição aberta, identifica o grupo incompleto e direciona a atenção para a correção necessária
