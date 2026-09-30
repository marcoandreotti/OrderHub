## 1. Fundação visual e isolamento de tema

- [x] 1.1 Aplicar um identificador de superfície aos quatro layouts oficiais e limitar o tema do Tenant ao subtree público, verificando por teste que navegar entre Public e Administration não preserva cores ou tipografia públicas.
- [x] 1.2 Introduzir somente os tokens semânticos exigidos pelas superfícies iniciais para borda, texto secundário, foco, success, warning, danger e urgência, verificando contraste e ausência de novos valores duplicados nos arquivos migrados.
- [x] 1.3 Manter aliases compatíveis para os estilos ainda não migrados e registrar consumidores restantes, verificando por busca que nenhum seletor alterado ficou sem token resolvido.
- [x] 1.4 Atualizar `docs/web-design-system.md` com tokens e componentes efetivamente implementados, verificando que o catálogo não declare candidatos como componentes existentes.

## 2. Jornada pública mobile-first

- [x] 2.1 Separar no módulo público os componentes de contexto da unidade, busca, categorias e apresentação de produto, verificando com Vitest que busca e troca de categoria preservam contexto e carrinho.
- [x] 2.2 Implementar a apresentação mobile-first do cardápio e sua adaptação desktop, verificando em navegador que 320px, tablet e desktop não apresentam sobreposição ou overflow essencial.
- [x] 2.3 Implementar o acesso persistente ao carrinho com quantidade e total conhecido, verificando que ele permanece acionável após rolagem e não encobre conteúdo, foco ou mensagens de indisponibilidade.
- [x] 2.4 Remodelar o compositor como superfície responsiva com progresso, requisitos, quantidade, preço e ação persistente, verificando sabores fracionários, grupos obrigatórios e foco no primeiro grupo inválido.
- [x] 2.5 Cobrir loading, empty, error, fechado, pausado, esgotado e agendamento no cardápio remodelado, verificando texto, ação de recuperação e comunicação que não dependa somente de cor.

## 3. Operação de pedidos

- [x] 3.1 Estender de forma aditiva a projeção Dapper e o contrato de resumo operacional com quantidade, produto e variação dos itens, sem N+1 e com isolamento por Tenant; extrair o cartão de pedido como componente da feature exibindo estado, tempo, atendimento, itens essenciais e próxima ação; verificar por testes o mapeamento da projeção e a matriz de ações autorizadas.
- [x] 3.2 Aplicar sinais redundantes de urgência e atraso e reorganizar o board para monitor e tablet touch, verificando rótulo ou ícone além da cor, áreas de toque e operação por teclado.
- [x] 3.3 Integrar conexão, última sincronização, fallback e possível defasagem à hierarquia do cockpit, verificando que falhas mantêm a fila existente e expõem ação de nova tentativa.
- [x] 3.4 Preservar a separação entre demanda imediata e agendada na nova composição, verificando ordenação e promoção visual quando a janela de produção for alcançada.

## 4. Kitchen Display System

- [x] 4.1 Extrair ticket e coluna como componentes da feature KDS sem reutilizar o cartão de operações, verificando que número, tempo, quantidades, modificadores e observações permanecem disponíveis.
- [x] 4.2 Implementar o modo KDS de baixa distração para monitor e tablet, verificando leitura sem abertura de detalhe, ausência de overflow e manutenção da unidade autorizada.
- [x] 4.3 Implementar prioridade e atraso com texto ou ícone além da cor e ações adequadas a touch e teclado, verificando início, pausa quando suportada, conclusão e conflito de estado.

## 5. Administração e catálogo

- [x] 5.1 Normalizar o shell administrativo, unidade ativa, navegação e cabeçalho de página sem criar um layout genérico compartilhado com Operations, verificando capacidades, troca de unidade e restauração de foco.
- [x] 5.2 Definir tratamentos consistentes para filtros, estado sem unidade, loading, empty, error e confirmações em pelo menos dois módulos antes de avaliar extração global, verificando os fluxos existentes de recuperação e preservação de formulário.
- [x] 5.3 Implementar no catálogo um estado único de busca, filtros, paginação e seleção consumido pelas visões visual e compacta, verificando que alternar a visão não refaz a interpretação nem perde estado.
- [x] 5.4 Implementar a visão visual de produtos com imagem, nome, categoria, preço inicial, estado e resumo de variações ou grupos, verificando produtos sem imagem, inativos e com muitas associações.
- [x] 5.5 Manter a visão compacta eficiente para comparação e manutenção, verificando pesquisa independente e edição de categorias, produtos, adicionais e grupos ativos ou inativos.
- [x] 5.6 Confirmar que temas públicos expressivos não alteram tipografia, contraste, densidade ou estados administrativos, verificando a troca entre unidades com temas diferentes.

## 6. Dashboard e migração administrativa

- [x] 6.1 Transferida para `add-reporting-dashboard`, tarefa 2.3; a implementação permanece pendente até que os contratos de indicadores estejam disponíveis.
- [x] 6.2 Migrar páginas administrativas restantes apenas para padrões comprovados em consumidores anteriores, verificando que qualquer novo componente global possui ao menos dois consumidores, documentação e testes de comportamento.

## 7. Verificação e limpeza

- [x] 7.1 Executar `npm test`, `npm run typecheck` e `npm run build` em `web/OrderHub.Web` e corrigir regressões sem reduzir cobertura comportamental.
- [x] 7.2 Executar verificações de navegador para Public mobile, Administration tablet/desktop, Operations tablet/monitor e KDS tablet/monitor, verificando teclado, foco, contraste, touch, overflow e estados sem cor exclusiva.
- [x] 7.3 Pesquisar e remover estilos e aliases de compatibilidade sem consumidores, verificando que nenhum valor visual global ou tema público continua vazando entre superfícies.
- [x] 7.4 Revisar o mapa final de componentes, atualizar `docs/web-design-system.md` e validar a change com `openspec validate remodel-web-experiences --strict`.
