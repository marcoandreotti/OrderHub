## Why

O frontend já cobre as jornadas essenciais, mas ainda apresenta uma linguagem visual genérica e inconsistente entre compra, gestão e operação. A maturidade funcional alcançada permite agora reduzir atrito, acelerar decisões operacionais e dar identidade própria a cada público sem trocar Vue, Quasar ou a arquitetura existente.

## What Changes

- Remodelar o cardápio público como uma experiência mobile-first orientada a descoberta, fotografia, composição clara e carrinho persistentemente acessível.
- Consolidar a administração como uma superfície estável e produtiva, com shell, cabeçalhos, filtros, formulários, feedback e densidade consistentes.
- Evoluir o catálogo administrativo para oferecer reconhecimento visual e manutenção eficiente, incluindo visões em cards e lista sem perder pesquisa, paginação ou recursos inativos.
- Refinar o painel de pedidos como cockpit operacional, priorizando tempo, estado, próxima ação, conexão e separação entre demanda imediata e agendada.
- Refinar o KDS para leitura à distância e uso por monitor, tablet ou touch, destacando prioridade, atraso, quantidades, modificadores e observações.
- Ampliar os tokens e componentes compartilhados somente a partir de padrões comprovados, mantendo componentes de domínio próximos às respectivas features.
- Isolar a personalização pública do Tenant das identidades de Administration, Operations, KDS e Platform.
- Padronizar responsividade, acessibilidade e estados loading, success, empty, error, disabled e unauthorized nas superfícies afetadas.
- Migrar as telas incrementalmente, preservando contratos de API, regras de domínio, autorização e isolamento Multi-Tenant.

## Capabilities

### New Capabilities

Nenhuma. O Design System é uma decisão transversal de implementação e documentação; o comportamento observável permanece nas capabilities web existentes.

### Modified Capabilities

- `ordering/public-ordering-web`: tornar descoberta, navegação por categorias, carrinho persistente e composição responsiva requisitos explícitos da jornada pública.
- `administration/administration-web`: estabelecer consistência de shell e estados, além de visões visual e compacta do catálogo sem perda funcional.
- `operations/order-operations-dashboard`: explicitar hierarquia temporal, próxima ação, conexão e adaptação para monitor, tablet e touch.
- `operations/kitchen-display-system`: adicionar requisitos de legibilidade à distância, prioridade, atraso, modificadores, observações e interação adequada ao ambiente de produção.

## Impact

- Frontend Vue/Quasar em `web/OrderHub.Web`, principalmente temas, estilos, layouts, componentes compartilhados e módulos público, administrativo e operacional.
- Testes Vitest e verificações de navegador das jornadas remodeladas.
- Documentação do Design System e catálogo de componentes.
- Uma extensão aditiva do contrato de resumo operacional incluirá somente a projeção mínima de itens necessária à leitura da fila; não altera comandos, regras de domínio, autenticação, autorização ou dependências externas.
- A dashboard administrativa deverá consumir a capability `reporting/business-dashboard` da change `add-reporting-dashboard` quando disponível, sem duplicar suas projeções ou definições financeiras.
