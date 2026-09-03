## Why

O backend público já permite consultar ofertas, simular, confirmar e acompanhar pedidos, mas o cliente final ainda não possui uma jornada web. Esta mudança transforma essas capacidades em um canal de venda acessível por URL ou QR Code.

## What Changes

- Criar cardápio público por slug com identidade visual da unidade, categorias e ofertas vendáveis.
- Criar seleção de variações e adicionais com respeito aos limites apresentados pelo servidor.
- Criar carrinho persistido localmente e recalculado pela simulação autoritativa antes da confirmação.
- Criar identificação do cliente, seleção/cadastro de endereço, atendimento em mesa, retirada ou entrega, cupom e forma de pagamento.
- Confirmar o pedido com chave idempotente e apresentar recibo, referência pública e acompanhamento de status.
- Tratar indisponibilidade, validação, conflito, falhas de rede e repetição segura sem expor dados internos ou TenantId.
- Adicionar experiência responsiva e acessível, além de testes dos principais percursos públicos.

## Capabilities

### New Capabilities

- `ordering/public-ordering-web`: jornada web pública completa desde o acesso ao cardápio até o acompanhamento do pedido.

### Modified Capabilities

Nenhuma. A interface consome os comportamentos já definidos pelas capacidades públicas existentes.

## Impact

Afeta principalmente o frontend Quasar, roteamento público, estado do carrinho, cliente HTTP e testes de interface. Reutiliza as APIs públicas de catálogo e ordering e não requer autenticação administrativa.
