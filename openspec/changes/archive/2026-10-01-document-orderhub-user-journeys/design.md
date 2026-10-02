# Design

## Context

See `proposal.md` for motivation. O repositório já mantém inventários de capacidades e documentação de módulos, mas ainda não possui um guia introdutório único, orientado por tarefas. Os fluxos e rótulos devem ser confirmados diretamente nas rotas e páginas existentes antes de escrever o material.

## Goals / Non-Goals

**Goals:**
- Dar ao leitor o menor caminho confiável para concluir objetivos comuns.
- Distinguir a experiência do consumidor dos caminhos administrativos/operacionais do estabelecimento.
- Criar conteúdo fácil de ampliar como parte de um manual de estabelecimento.

**Non-Goals:**
- Documentar cada campo/configuração ou substituir documentação técnica e OpenSpec.
- Descrever pagamentos online, WhatsApp automatizado ou outros recursos ainda não disponíveis.
- Criar capturas de tela que possam ficar desatualizadas ou exigir manutenção contínua.

## Decisions

- Criar um guia de entrada em `docs/` com índice por objetivo, e manter detalhes futuros em páginas separadas por perfil. Isso mantém o primeiro documento curto e permite sua incorporação futura a um manual maior.
- Para cada caminho, usar o padrão: objetivo, onde começar, passos numerados, resultado esperado e para que serve aquela etapa. Incluir pré-requisitos e limitações somente quando necessários para completar o fluxo.
- Confirmar textos de menu, URLs e passos lendo as telas atuais e cruzando com specs/inventários. Em caso de divergência, documentar o comportamento que existe e apontar a inconsistência para revisão, sem inventar uma rota idealizada.
- Usar linguagem simples em português brasileiro, explicar termos do produto na primeira ocorrência e limitar cada jornada aos passos essenciais.

## Risks / Trade-offs

- [Risco] Rotas e rótulos mudam com frequência → manter o guia ligado a páginas específicas e incluir a verificação dessas referências na Definition of Done.
- [Risco] O leitor pode confundir recursos previstos com recursos disponíveis → escrever apenas sobre capacidades observadas e sinalizar pré-requisitos/indisponibilidades claramente.
- [Risco] Um guia curto não cobre todos os casos → oferecer links para documentação existente e permitir a expansão por perfil em etapas futuras.
