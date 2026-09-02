## 1. Domínio e contratos internos

- [x] 1.1 Inventariar os tipos, dispatchers, exceções, contexto operacional e padrões de persistence já existentes antes de criar abstrações do catálogo, e verificar por busca no repositório que nenhuma classe ou porta equivalente foi duplicada.
- [x] 1.2 Completar comportamentos explícitos de edição, ativação, desativação, ordenação e mudança de pai de `Category`, e verificar em testes de domínio autorreferência, ciclo indireto, pai cross-unit e transições de estado.
- [x] 1.3 Completar comportamentos de edição e ativação de `Product`, imagens, variações e vínculos de grupos, e verificar em testes de domínio preço, URL, imagem principal única, ordenação e referências cross-unit.
- [x] 1.4 Completar comportamentos de edição e ativação de `Additional` e `AdditionalGroup`, incluindo manutenção de itens e limites, e verificar em testes de domínio ranges, duplicidade, ordenação e referências cross-unit.

## 2. Casos de uso CQRS do catálogo

- [x] 2.1 Definir portas de escrita específicas por aggregate, read models e gateways das projeções administrativa e pública, todos assíncronos e com `CancellationToken`, e verificar que Application não referencia EF Core, Dapper ou Infrastructure.
- [x] 2.2 Implementar commands, validators e handlers de criação e alteração de categorias usando `EstablishmentScopeResolver`, e verificar entradas inválidas, autorização de unidade, ciclos e sucesso em testes de Application.
- [x] 2.3 Implementar commands, validators e handlers de criação e alteração de produtos, imagens, variações e vínculos de grupos, e verificar escopo, conflito de código e atomicidade esperada em testes de Application.
- [x] 2.4 Implementar commands, validators e handlers de criação e alteração de adicionais e grupos, e verificar limites, vínculos, escopo e entradas inválidas em testes de Application.
- [x] 2.5 Implementar queries e handlers para catálogo administrativo por unidade e cardápio público por slug, e verificar que a primeira exige escopo operacional enquanto a segunda não confia em contexto ou IDs fornecidos pelo visitante.
- [x] 2.6 Registrar handlers e validators do catálogo nos dispatchers existentes, e verificar a resolução de todos os casos de uso em teste do container de Application.

## 3. Persistência de escrita com EF Core

- [x] 3.1 Adicionar `DbSet` e mappings de `Category`, `Product`, imagens, variações e vínculos de grupos com precisão monetária, checks, FKs, índices, unicidade unit-scoped e delete behaviors restritivos, e verificar o modelo em testes de metadata do EF Core.
- [x] 3.2 Adicionar mappings de `Additional`, `AdditionalGroup` e itens de grupo com checks, FKs compostas ou proteção equivalente contra vínculo cross-unit e índices de ordenação, e verificar o modelo em testes de metadata do EF Core.
- [x] 3.3 Implementar repositories EF Core com buscas sempre filtradas por Tenant/unidade e gravação atômica por efeito de negócio, e verificar carga de coleções, inexistência cross-tenant e rollback em testes de integração PostgreSQL.
- [x] 3.4 Traduzir violações esperadas de unicidade e integridade para erros padronizados sem espalhar `try/catch` pelos endpoints, e verificar código duplicado e referência inválida em integração.

## 4. Projeções de leitura com Dapper

- [x] 4.1 Implementar gateway Dapper da visão administrativa incluindo ativos e inativos, hierarquia e todas as coleções ordenadas, e verificar composição sem duplicações e isolamento com dois Tenants e duas unidades.
- [x] 4.2 Implementar gateway Dapper do cardápio público resolvido por slug, omitindo Tenant/unidade ou componentes inativos e categorias sem oferta vendável, e verificar slug inexistente, escopo correto, preços e ordenação em integração.
- [x] 4.3 Verificar que todo SQL do catálogo permanece em Infrastructure e que queries não alteram estado, por testes arquiteturais e inspeção dos gateways.

## 5. Migration do catálogo

- [x] 5.1 Gerar a migration incremental `ProductCatalog` no projeto dedicado, revisar o código gerado e verificar que cria tabelas, constraints, índices e relações previstos sem alterações não relacionadas.
- [x] 5.2 Aplicar a cadeia completa de migrations em PostgreSQL vazio e também sobre o schema anterior, e verificar inicialização determinística e leitura/escrita do catálogo sem ajustes manuais.
- [x] 5.3 Verificar em PostgreSQL a rejeição de código duplicado na mesma unidade, referências cross-unit e estados estruturais inválidos, além de permitir código igual em unidades distintas.
- [x] 5.4 Reverter a migration em banco descartável e reaplicá-la, documentando o comando e a perda de dados esperada no downgrade, e verificar retorno ao schema anterior sem objetos residuais.

## 6. Contratos e endpoints HTTP

- [x] 6.1 Criar contratos externos explícitos para commands administrativos, projeção administrativa e cardápio público sem expor entidades de domínio ou `TenantId` como entrada de autorização, e verificar referências e serialização em testes.
- [x] 6.2 Adicionar endpoints administrativos finos protegidos pela política `management`, despachando exclusivamente commands e queries, e verificar sucesso, validação, conflito, ausência e negação cross-unit por testes de API com `ProblemDetails`.
- [x] 6.3 Adicionar endpoint público de cardápio por slug sem privilégios administrativos, e verificar unidade ativa, itens inativos omitidos e resposta indistinguível para slug inexistente ou unidade inativa em testes de API.
- [x] 6.4 Registrar gateways, repositories e módulos no composition root, e verificar que o container resolve todos os endpoints sem acesso direto da API a EF Core ou Dapper.

## 7. Verificação da entrega

- [x] 7.1 Consolidar testes de domínio, Application, arquitetura, API e integração para todos os cenários do delta spec, e verificar que cada requisito possui ao menos um teste rastreável.
- [x] 7.2 Executar formatação, build da solution e toda a suíte de testes; corrigir erros e warnings relevantes até todos os comandos concluírem com sucesso.
- [x] 7.3 Revisar o diff final contra proposal, design, delta spec, AGENTS.md e specs principais, e verificar ausência de MediatR, AutoMapper, regra de negócio em endpoints, acesso cruzado de Tenant e requisitos fora do escopo.
