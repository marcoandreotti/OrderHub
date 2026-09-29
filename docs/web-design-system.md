# Design System web do OrderHub

## Propósito

Este documento é a fonte oficial das decisões visuais e dos padrões de componentes da aplicação web. Ele orienta implementação e revisão, sem substituir as specs funcionais em `openspec/specs/`.

O objetivo é permitir que qualquer pessoa ou agente descubra como o OrderHub já resolve uma necessidade antes de criar uma solução nova.

## Fontes oficiais

As fontes devem ser consultadas em conjunto:

- Este documento define propósito, critérios de uso e padrões visuais.
- `web/OrderHub.Web/src/themes/_tokens.scss` contém os tokens implementados.
- `web/OrderHub.Web/src/themes/tenant-theme.ts` e `web/OrderHub.Web/src/modules/public-ordering/theme.ts` contêm a aplicação programática de tema.
- `web/OrderHub.Web/src/css/app.scss` contém estilos globais ainda vigentes.
- `web/OrderHub.Web/src/components/` contém componentes compartilhados implementados.
- `web/OrderHub.Web/src/layouts/` contém os shells oficiais das superfícies.
- Componentes reutilizáveis de feature permanecem em `web/OrderHub.Web/src/modules/<module>/`.
- Specs de frontend em `openspec/specs/` definem comportamento, acessibilidade e resultados obrigatórios.

Se documentação e implementação divergirem, não escolher silenciosamente. Confirmar se o código representa dívida, se o documento está desatualizado ou se a mudança exige decisão explícita.

## Princípios

1. Pesquisar antes de criar.
2. Reutilizar por semântica, não apenas por semelhança visual.
3. Usar Quasar como matéria-prima e não como identidade visual final.
4. Criar abstração compartilhada somente para um padrão estável e comprovado.
5. Priorizar tokens semânticos em vez de valores arbitrários.
6. Tornar estados, ações e hierarquia compreensíveis antes de decorar a interface.
7. Preservar acessibilidade e responsividade como requisitos, não como acabamento.
8. Evoluir incrementalmente sem exigir refatoração visual fora do escopo da feature.

## Superfícies oficiais

### Public

Atende o cliente do estabelecimento. Seu propósito é transformar descoberta em pedido válido com pouco atrito.

Prioridades:

- mobile-first;
- identidade visual do Tenant;
- fotografia e reconhecimento do produto;
- busca, categorias e descoberta;
- preço, disponibilidade e prazo evidentes;
- carrinho persistentemente acessível;
- composição de produtos complexos sem esconder requisitos;
- checkout direto e acompanhamento compreensível.

O tema público pode variar por Tenant, mas deve manter contraste, foco e legibilidade. Tokens de marca pública devem ter escopo da superfície e não devem alterar silenciosamente Administration, Operations, KDS ou Platform.

### Administration

Atende proprietários, gerentes e pessoas autorizadas a configurar o estabelecimento. Seu propósito é reduzir esforço cognitivo e operacional na manutenção do negócio.

Prioridades:

- navegação consistente;
- cabeçalhos e ações de página previsíveis;
- formulários claros;
- tabelas e filtros eficientes;
- densidade adequada ao trabalho;
- estados e confirmações inequívocos;
- indicadores somente quando apoiam uma decisão.

A marca do Tenant pode aparecer como identificação ou accent discreto. Ela não controla a linguagem visual administrativa.

### Operations

Atende equipes que acompanham e avançam pedidos. Seu propósito é reduzir o tempo entre perceber uma mudança e executar a ação correta.

Prioridades:

- estados e próxima ação evidentes;
- tempo decorrido e horário prometido visíveis;
- alto contraste e leitura rápida;
- poucos cliques;
- conexão, sincronização e defasagem perceptíveis;
- uso por teclado, monitor, tablet e touch;
- separação entre trabalho imediato e demanda futura.

### KDS

Atende a produção da cozinha. Seu propósito é preservar sequência, prioridade e composição em ambiente de pressão.

Prioridades:

- leitura à distância;
- quantidades, modificadores e observações em destaque;
- atraso comunicado por mais de um sinal;
- ações grandes e inequívocas;
- mínimo de navegação e distração;
- funcionamento em tela cheia e tablet quando aplicável.

### Platform

Atende identidades globais de provisionamento. Seu propósito é tornar inequívoco que o usuário está fora do contexto operacional de um Tenant.

Prioridades:

- identidade institucional consistente;
- separação visual entre contexto global e unidade;
- ações de provisionamento cuidadosamente confirmadas;
- estados vazios orientados ao próximo passo.

## Layouts oficiais

- `PublicLayout.vue`: shell do cardápio, checkout e acompanhamento público.
- `AdministrationLayout.vue`: shell tenant-scoped de gestão.
- `OperationsLayout.vue`: shell de pedidos, entrega e cozinha.
- `PlatformLayout.vue`: shell global de provisionamento.

Não criar um layout genérico para unificar superfícies com propósitos diferentes. Extrações compartilhadas são aceitáveis quando preservam identidade, navegação e densidade próprias.

## Tokens

### Tokens implementados

`_tokens.scss` define atualmente:

- cores primary, secondary, accent, background, surface e text;
- raio-base;
- família tipográfica.

Esses tokens são oficiais enquanto existirem no código, mas formam apenas a fundação inicial.

### Critérios para novos tokens

Antes de adicionar cor, espaçamento, raio, sombra, tipografia, breakpoint ou z-index:

1. verificar se já existe token com a mesma semântica;
2. verificar se o valor representa decisão reutilizável;
3. escolher nome pelo papel, não pela aparência ou por uma página específica;
4. documentar o token quando seu uso não for autoexplicativo;
5. validar contraste e impacto nas superfícies aplicáveis.

Evitar tokens como `--catalog-card-margin-17`. Preferir papéis estáveis, por exemplo superfície elevada, texto secundário, espaço médio ou raio de card, quando esses conceitos forem realmente adotados.

Valores locais continuam aceitáveis para ajustes intrínsecos e não reutilizáveis. Não criar token apenas para eliminar todo número do CSS.

### Evolução prevista

As categorias abaixo são candidatas, não tokens já implementados:

- cores semânticas de success, warning, danger, info e estados operacionais;
- escala de spacing;
- escala tipográfica;
- raios por papel;
- bordas e elevações;
- foco;
- motion e duração;
- camadas e z-index;
- densidades public, administration e operations.

Uma feature não deve introduzir toda a escala antecipadamente. Adicionar somente decisões exigidas por casos concretos.

## Classificação de componentes

### Global

Representa padrão estável usado por múltiplas features ou superfícies. Deve permanecer em `src/components/` ou em uma futura pasta oficial do Design System, caso essa organização seja aprovada.

Requisitos:

- propósito e limites claros;
- semântica estável;
- mais de um consumidor concreto ou padrão transversal evidente;
- uso de tokens oficiais;
- API pequena e coerente;
- documentação neste catálogo;
- testes quando houver comportamento próprio.

### Domain ou Feature

Representa conceito reutilizável dentro de um domínio, como produto, pedido ou cliente. Deve permanecer próximo ao módulo consumidor.

Exemplos conceituais: `ProductCard`, `OrderCard`, `OrderTimeline`, `ModifierSelector` e `CustomerAddressCard`. Esses nomes não indicam que os componentes já existam ou estejam aprovados globalmente.

### Page-specific

Organiza apenas uma página e não representa um padrão reutilizável. Deve permanecer ao lado da página ou dentro dela quando a complexidade for pequena.

Não promover componentes automaticamente. Também não aumentar indefinidamente props e slots de um componente existente para acomodar responsabilidades diferentes.

## Catálogo atual

### Componentes compartilhados implementados

#### ProblemBanner

- Caminho: `web/OrderHub.Web/src/components/ProblemBanner.vue`.
- Propósito: apresentar erros HTTP e ProblemDetails de forma consistente, mantendo espaço para uma ação de recuperação.
- Usar quando: uma página, painel ou operação precisa comunicar falha recuperável ou erro conhecido da API.
- Não usar quando: o erro pertence exclusivamente a um campo de formulário ou uma mensagem transitória de sucesso é suficiente.

Não existem atualmente `AppButton`, `AppCard`, `AppDialog`, `AppStatus`, `AppPageHeader` ou equivalentes oficiais. Antes de criar qualquer um deles, comprovar o padrão, pesquisar consumidores e documentar a decisão aqui.

### Componentes de feature identificados

- `CatalogPicker.vue`: seleção paginada de vínculos dentro do catálogo administrativo.
- `AccessStep.vue`: etapa de acesso no onboarding administrativo.

Eles permanecem próximos aos módulos porque suas responsabilidades são específicas. Reuso fora desses domínios exige nova avaliação, não simples movimentação de pasta.

### Layouts

Os quatro layouts listados na seção Layouts oficiais fazem parte do catálogo e não devem ser substituídos por um `MainLayout` único.

## Uso do Quasar

Antes de usar diretamente `QBtn`, `QDialog`, `QCard`, `QInput`, `QTable`, `QBanner` ou outro componente:

1. procurar componente oficial com a semântica necessária;
2. verificar padrões já usados na mesma superfície;
3. usar o componente compartilhado quando houver linguagem ou comportamento oficial;
4. usar Quasar diretamente quando a estrutura for local e não houver padrão global;
5. não criar wrapper que apenas replique props sem acrescentar semântica, comportamento ou consistência.

## Padrões de experiência

### Estados de dados

Toda experiência assíncrona deve avaliar:

- loading: informar atividade e preservar geometria com skeleton quando isso reduzir salto visual;
- success: confirmar resultado na intensidade adequada;
- empty: explicar a ausência e oferecer próxima ação quando existir;
- error: explicar, preservar dados úteis e oferecer recuperação;
- disabled: tornar o motivo compreensível quando necessário;
- unauthorized ou forbidden: não exibir dados protegidos e orientar sem sugerir acesso inexistente.

### Formulários

- associar erros aos campos quando possível;
- preservar valores após validação, conflito ou falha recuperável;
- distinguir ação primária, secundária e destrutiva;
- confirmar ações sensíveis;
- não depender de placeholder como rótulo;
- organizar formulários longos em seções coerentes sem esconder dependências importantes.

### Navegação

- apresentar somente módulos compatíveis com as capacidades conhecidas, sem substituir autorização da API;
- manter contexto de unidade visível em Administration e Operations;
- reconstruir dados ao trocar unidade;
- manter foco previsível após navegação e fechamento de dialogs;
- usar breadcrumbs apenas quando comunicarem hierarquia real.

### Feedback

- usar mensagens próximas da ação que as originou;
- reservar dialogs para decisões, confirmações ou tarefas focadas;
- não usar notificação transitória como única forma de comunicar erro que exige ação;
- comunicar estado de sincronização em experiências em tempo real.

## Responsividade

Toda alteração deve ser avaliada em smartphone, tablet e desktop quando essas dimensões forem suportadas pela superfície.

- Public é mobile-first.
- Administration prioriza desktop e tablet, sem quebrar fluxos essenciais em larguras menores previstas pela spec.
- Operations e KDS devem considerar monitor, tablet e touch.
- Preferir CSS responsivo, grid, flex e recursos do Quasar a condicionais JavaScript de layout.
- Não considerar concluída uma tela validada apenas em desktop.

## Acessibilidade

Toda alteração deve preservar:

- navegação completa por teclado;
- foco visível e ordem de foco coerente;
- rótulos acessíveis;
- HTML semântico antes de ARIA;
- contraste adequado;
- estado comunicado por texto ou ícone além de cor;
- áreas de toque apropriadas;
- mensagens de erro perceptíveis;
- preferência por redução de movimento quando animações forem introduzidas.

## Fluxo para criar ou alterar uma tela

1. Ler a spec da capability.
2. Identificar a superfície e o layout oficial.
3. Pesquisar páginas e componentes semanticamente semelhantes.
4. Consultar tokens e padrões de estado.
5. Classificar novos componentes.
6. Implementar a menor extensão coerente.
7. Testar comportamento, estados e permissões.
8. Verificar responsividade e acessibilidade.
9. Atualizar este catálogo se um padrão global for criado ou alterado.

## Evolução do Design System

Não introduzir Storybook, biblioteca visual adicional ou gerador de documentação apenas para criar um catálogo. Este Markdown é suficiente enquanto permitir descoberta e revisão.

Quando o volume de componentes e variantes tornar a manutenção manual insuficiente, propor a ferramenta, seus consumidores, custo e estratégia de adoção antes de adicionar dependências.

Uma mudança global de tokens, temas, layouts ou APIs de componentes exige avaliação de impacto. Se alterar decisão arquitetural, registrar ADR antes da implementação.
