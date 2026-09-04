## Context

As APIs públicas já resolvem contexto, catálogo, cliente, simulação, confirmação, acompanhamento e cancelamento. O frontend possui um layout público e suporte a tema, mas nenhuma jornada. O navegador não pode ser autoridade para preço, elegibilidade ou Tenant.

## Goals / Non-Goals

**Goals:**

- Entregar um percurso mobile-first completo, resiliente a repetição e mudanças de oferta.
- Preservar a intenção do carrinho sem transformar estado local em verdade comercial.
- Manter módulos e contratos públicos separados da área autenticada.

**Non-Goals:**

- Conta autenticada de consumidor, marketplace entre unidades, pagamento online integrado ou aplicação nativa.
- Cache offline/PWA nesta primeira entrega.

## Decisions

1. **Rota pública por slug e token opcional.** O router extrai apenas identificadores públicos e consulta o contexto; nenhum TenantId é mantido como autoridade.
2. **Store de carrinho versionada por unidade.** Persistir localmente somente IDs, quantidades, escolhas e observações. Limpar/migrar quando a versão for incompatível ou o slug mudar.
3. **Simulação antes da confirmação.** Debounce apenas quando útil; o checkout sempre executa simulação final. Divergências são mostradas antes do envio.
4. **Máquina de estados da interface.** Modelar carregamento do contexto, composição, identificação, revisão, confirmação e acompanhamento para impedir duplo envio e navegação incoerente.
5. **Idempotência ligada à intenção.** Gerar chave no início de uma tentativa de confirmação e reutilizá-la nos retries. Qualquer edição material posterior cria nova intenção/chave.
6. **Tema por CSS custom properties validadas.** Aplicar somente tokens retornados pelo contexto e manter fallback legível; HTML/CSS arbitrário da unidade não é aceito.
7. **Polling moderado no acompanhamento.** Atualizar enquanto a página está visível, com backoff em falha. Tempo real não é necessário para o primeiro fluxo público.

## Risks / Trade-offs

- [Carrinho obsoleto] → Simulação autoritativa e mensagens claras para itens alterados/inativos.
- [Perda da referência após fechar a página] → Persistir apenas a referência pública e oferecer retomada local.
- [Múltiplos cliques] → Estado de envio único e chave idempotente estável.
- [Tema prejudica acessibilidade] → Validar tokens, limitar superfícies tematizáveis e testar contraste/fallback.

## Migration Plan

1. Adicionar rotas e shell público mantendo a página atual como fallback.
2. Entregar catálogo/composição, depois checkout e acompanhamento.
3. Validar contra API real com unidade e mesa de teste.
4. Alterar a rota pública principal somente após testes de percurso; rollback restaura a página de fundação sem afetar pedidos/API.
