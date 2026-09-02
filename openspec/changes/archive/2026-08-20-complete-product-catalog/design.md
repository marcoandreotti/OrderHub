## Context

O domínio já contém `Category`, `Product`, `ProductImage`, `ProductVariation`, `Additional`, `AdditionalGroup` e entidades de vínculo, além de testes das invariantes centrais. A aplicação já possui dispatchers próprios, FluentValidation, contexto autenticado e `EstablishmentScopeResolver`; Infrastructure já separa EF Core para escrita e Dapper para leitura no mesmo PostgreSQL. A API ainda não expõe o catálogo e o `OrderHubDbContext` ainda não o mapeia. Consulte `proposal.md` para a motivação e `specs/catalog/product-catalog/spec.md` para os novos contratos observáveis.

## Goals / Non-Goals

**Goals:**

- Completar um fluxo vertical do catálogo preservando a separação CQRS e as dependências da arquitetura hexagonal.
- Tornar explícito o escopo autenticado de Tenant/unidade nas escritas e leituras administrativas.
- Resolver o escopo público exclusivamente pelo slug ativo da unidade.
- Persistir aggregates e vínculos com constraints equivalentes às invariantes estruturais.
- Produzir projeções administrativas completas e projeções públicas enxutas sem carregar aggregates.

**Non-Goals:**

- Criar pedidos, validar seleção de catálogo durante um pedido ou gerar snapshots de pedido.
- Armazenar arquivos de imagem; o catálogo apenas mantém URLs validadas.
- Adicionar paginação, busca textual avançada, cache, Redis, eventos ou banco de leitura separado.
- Alterar autenticação, perfis existentes ou a arquitetura definida; nenhuma decisão exige ADR.

## Decisions

### Casos de uso organizados por aggregate e intenção

Commands e handlers serão criados para operações administrativas de categoria, produto e suas coleções, adicional e grupo de adicionais. Queries distintas fornecerão a visão administrativa e o cardápio público. Validators tratarão somente formato, obrigatoriedade e ranges de entrada; referências de mesmo Tenant/unidade, ciclos e demais invariantes continuarão protegidas pelo domínio e pelos handlers.

Alternativa considerada: um único command para substituir todo o catálogo. Foi rejeitada porque amplia contenção, dificulta conflitos e mistura aggregates independentes em uma transação artificial.

### Escopo administrativo resolvido antes do acesso ao catálogo

Endpoints administrativos exigirão a política `management` e enviarão apenas o identificador da unidade necessário à seleção operacional. O handler usará `ITenantContext` e `EstablishmentScopeResolver` para confirmar Tenant, usuário e associação ativa antes de carregar ou alterar dados. Repositories sempre receberão ou derivarão o escopo validado e aplicarão Tenant e unidade em todas as buscas, inclusive por ID.

Alternativa considerada: aceitar `TenantId` no contrato e filtrá-lo no repository. Foi rejeitada porque transforma um valor controlado pelo cliente em entrada de autorização.

### Escopo público resolvido por slug em gateway de leitura

A query pública receberá somente o slug. Um gateway Dapper resolverá Tenant e unidade ativos e projetará o cardápio em uma consulta tenant/unit-scoped. O resultado omitirá todos os componentes inativos e categorias sem oferta vendável, mantendo a ordenação persistida. Ausência ou inatividade produzirá a mesma resposta de não encontrado, evitando enumeração de estado interno.

Alternativa considerada: reutilizar `ITenantContext` na consulta pública. Foi rejeitada porque visitantes não possuem contexto autenticado e o slug já é a identidade pública especificada da unidade.

### EF Core para aggregates e Dapper para duas projeções

O `OrderHubDbContext` receberá conjuntos e configurações para categorias, produtos e suas coleções, adicionais, grupos e tabelas de associação. Repositories EF carregarão aggregates necessários à alteração e salvarão cada efeito de negócio atomicamente. Dois gateways Dapper separados representarão as necessidades administrativas e públicas, retornando read models da Application e mantendo SQL em Infrastructure.

Alternativa considerada: montar consultas por EF Core ou reutilizar entidades do domínio como resposta. Foi rejeitada pela separação CQRS e pela proibição de expor entidades pela API.

### Schema relacional com escopo e integridade redundantes

As tabelas de catálogo usarão o schema PostgreSQL já convencionado e chaves UUID. Entidades principais carregarão `tenant_id` e `establishment_id`; unicidades relevantes serão compostas pelo escopo, incluindo código normalizado de produto. FKs compostas ou constraints equivalentes impedirão referências entre unidades. Checks protegerão preços não negativos, ordens não negativas e limites coerentes; índices atenderão filtros de escopo, slug-resolução indireta, hierarquia e ordenação. Delete behaviors serão restritivos para preservar referências e evitar cascatas acidentais.

Alternativa considerada: depender somente da validação da aplicação. Foi rejeitada porque importações, concorrência e futuras rotas de escrita também precisam preservar integridade.

### Contratos e endpoints separados por audiência

Contracts administrativos incluirão entradas explícitas e read models completos, inclusive estado ativo. Endpoints serão finos: recebem contratos, despacham command/query e traduzem o resultado HTTP; erros conhecidos continuarão no middleware global como `ProblemDetails`. A rota pública será separada das rotas administrativas e não aceitará Tenant ou unidade fora do slug.

Alternativa considerada: compartilhar o mesmo DTO entre administração e público. Foi rejeitada porque a visão administrativa inclui itens inativos e metadados que não devem compor o cardápio público.

## Risks / Trade-offs

- [Consultas hierárquicas com várias coleções podem multiplicar linhas] → Projetar conjuntos achatados em poucas consultas Dapper e montar a árvore em memória por IDs, com teste para duplicações e ordenação.
- [Entidades existentes podem precisar de operações explícitas de edição, ativação e remoção de vínculos] → Adicionar somente comportamentos exigidos pelos casos de uso, mantendo setters privados e testes de invariantes.
- [FKs compostas aumentam o número de índices e colunas em associações] → Aceitar o custo para garantir isolamento no banco e validar o modelo EF com testes de metadata e PostgreSQL real.
- [Migration com tabelas novas é reversível, mas rollback destrói dados do catálogo] → Documentar que o downgrade é restrito a ambiente controlado e validar o rollback em banco descartável.

## Migration Plan

1. Implementar e testar domínio/aplicação, mappings e adapters sem expor as rotas.
2. Gerar uma única migration incremental de catálogo no projeto dedicado de migrations.
3. Validar upgrade desde banco vazio e desde o schema atual, constraints e rollback em PostgreSQL descartável.
4. Publicar a versão que registra handlers, gateways e endpoints junto da migration.
5. Em rollback controlado, remover a versão da aplicação antes de reverter a migration; ambientes com dados reais exigem backup/exportação prévia, pois a reversão remove tabelas do catálogo.
