## 1. Fundação do modelo e migrations

- [x] 1.1 Inventariar projetos, módulos, tipos de identidade, abstrações de tempo/tenant e persistência já existentes; registrar no PR o reuso escolhido e verificar que nenhuma abstração equivalente foi duplicada.
- [x] 1.2 Adicionar somente os value objects e enums compartilhados comprovadamente usados por pelo menos duas capacidades (`Money`, quantidades e IDs/estados aplicáveis), com testes de precisão, igualdade e valores inválidos passando.
- [x] 1.3 Criar o projeto dedicado de migrations dentro de Infrastructure, configurar design time sem referências de Domain/Application para ele e verificar a solution e os testes arquiteturais.
- [x] 1.4 Definir convenções de schemas, nomes, timestamps UTC, precisões, enums `smallint`, FKs e delete behaviors; verificar mappings com testes de metadata do EF Core.
- [x] 1.5 Documentar comandos de criar, aplicar e reverter migrations e verificar o procedimento em PostgreSQL vazio descartável.

## 2. Tenancy e unidades

- [x] 2.1 Implementar os aggregates `Tenant` e `Establishment`, incluindo slug normalizado, ativação e associação de unidade, e verificar invariantes e unicidade em testes de domínio/aplicação.
- [x] 2.2 Implementar tema 1:1 da unidade com tokens validados e fallback padrão, e verificar tema parcial e unidade inativa em testes.
- [x] 2.3 Estender o contexto autenticado e criar serviço scoped de seleção de unidade que valide Tenant, usuário e associação ativa explícita sem confiar em IDs do payload; verificar negação de unidade cruzada, unidade não associada e associação revogada com dois Tenants e duas unidades do mesmo Tenant.
- [x] 2.4 Implementar mappings, portas e adapters de escrita/leitura de Tenancy com filtros explícitos, e verificar isolamento EF Core e Dapper em testes de integração PostgreSQL.
- [x] 2.5 Criar a migration de Tenancy com slug global, chaves de escopo e constraints, e verificar aplicação e rollback em banco descartável.

## 3. Identidade e configuração operacional

- [x] 3.1 Implementar usuário administrativo, e-mail normalizado, hash de senha e perfis conhecidos com políticas, e verificar e-mail tenant-scoped, usuário inativo e autorização em testes.
- [x] 3.2 Persistir usuários, perfis e associações N:N usuário–estabelecimento ativas/revogáveis, com seed determinístico apenas dos perfis de referência; verificar constraints contra associação cross-Tenant e que permissões não atravessam Tenants ou unidades.
- [x] 3.3 Implementar mesa com código unit-scoped e token público opaco, revogável e globalmente único, e verificar resolução válida e combinações slug/token cruzadas.
- [x] 3.4 Implementar horários regulares dentro do mesmo dia e consulta de disponibilidade, e verificar intervalos inválidos, dias fechados e múltiplos intervalos.
- [x] 3.5 Criar a migration de Identity/Operations e verificar constraints, índices, aplicação em banco vazio e testes de integração.

## 4. Catálogo

- [x] 4.1 Implementar `Category` com pai opcional e porta de ancestralidade, e verificar autorreferência, ciclo indireto, pai de outra unidade e ordenação.
- [x] 4.2 Implementar `Product` com categoria, código, preço base, imagens e variações, e verificar preço não negativo, uma imagem principal e referências unit-scoped.
- [x] 4.3 Implementar `Additional` e `AdditionalGroup` com limites de seleção, itens reutilizáveis e vínculos ordenados a produtos, e verificar mínimo/máximo e referências cruzadas.
- [ ] 4.4 Implementar comandos, validators, handlers e repositories relevantes do catálogo com `CancellationToken`, e verificar testes de Application sem regra de domínio nos validators.
- [ ] 4.5 Implementar projeções Dapper do catálogo hierárquico e oferta pública, sempre tenant/unit-scoped, e verificar árvore acíclica, itens inativos omitidos e isolamento em integração.
- [ ] 4.6 Criar a migration de catálogo com FKs, checks, índices e delete behaviors restritivos, e verificar aplicação/rollback e rejeição de estados estruturais inválidos.

## 5. Clientes

- [ ] 5.1 Implementar `Customer` e endereços sem credencial obrigatória, incluindo no máximo um principal, e verificar cadastro somente com nome/telefone e troca atômica do endereço principal.
- [ ] 5.2 Implementar portas, handlers, validators e adapters EF Core/Dapper de clientes, e verificar telefones iguais isolados entre unidades e ausência de acesso cruzado.
- [ ] 5.3 Criar a migration de clientes/endereço com constraints e índices adequados, e verificar aplicação e integridade em PostgreSQL.

## 6. Pedidos e cupons

- [ ] 6.1 Implementar `Order` com número unit-scoped, tipos de atendimento e requisitos de mesa/endereço, e verificar combinações válidas e inválidas em testes de domínio.
- [ ] 6.2 Implementar itens, adicionais e endereço de entrega como snapshots do pedido, e verificar que alterações posteriores de catálogo/cliente não modificam o histórico.
- [ ] 6.3 Implementar cálculo autoritativo de subtotal, descontos, taxas e total com precisão monetária, e verificar arredondamento, payload divergente e impossibilidade de total negativo.
- [ ] 6.4 Implementar máquina de estados por tipo de atendimento e histórico imutável na mesma transação, e verificar todas as transições aceitas/rejeitadas e estados terminais.
- [ ] 6.5 Implementar `Coupon`, elegibilidade, limite concorrente e snapshot aplicado ao pedido, e verificar janela, mínimo, percentual/fixo, limite esgotado e edição posterior.
- [ ] 6.6 Implementar alocação atômica do número do pedido e proteção idempotente de criação, e verificar concorrência paralela sem números ou efeitos duplicados.
- [ ] 6.7 Implementar handlers, validators, portas e repositories de pedido/cupom com transação única, e verificar rollback completo em falha induzida.
- [ ] 6.8 Criar projeções Dapper para detalhe e acompanhamento histórico do pedido, e verificar snapshots, timeline e filtros de Tenant/unidade.
- [ ] 6.9 Criar a migration de pedidos/cupons com checks, contador, índices e referências históricas restritivas, e verificar banco vazio, rollback e concorrência PostgreSQL.

## 7. Pagamentos

- [ ] 7.1 Implementar `PaymentMethod` e `Payment` com estados, troco e identificador externo opcional, e verificar forma inativa, valores inválidos e referências unit-scoped.
- [ ] 7.2 Implementar cobertura do pedido por múltiplos pagamentos e regra de excedente/troco, e verificar pagamento dividido, insuficiente e excedente em testes de domínio/aplicação.
- [ ] 7.3 Implementar confirmação idempotente e concorrente de pagamento em transação, e verificar que repetição ou corrida produz um único efeito financeiro.
- [ ] 7.4 Implementar adapters EF Core e projeções Dapper financeiras com isolamento explícito, e verificar leitura/escrita com dois Tenants e duas unidades.
- [ ] 7.5 Criar a migration de formas/pagamentos com constraints e índices, e verificar aplicação, rollback e preservação de histórico após desativação da forma.

## 8. Integração, segurança e conclusão

- [ ] 8.1 Registrar módulos, dispatchers, DbContexts, gateways e options no composition root sem acesso direto dos Controllers à persistência, e verificar resolução do container e testes arquiteturais.
- [ ] 8.2 Adicionar contratos e endpoints mínimos necessários para exercitar cada capacidade via CQRS, sem retornar entidades de domínio, e verificar respostas e ProblemDetails em testes de API.
- [ ] 8.3 Executar a cadeia completa de migrations em PostgreSQL vazio, popular dados de teste de dois Tenants/duas unidades e verificar isolamento, snapshots e transações nos fluxos críticos.
- [ ] 8.4 Executar formatação, build da solution, testes unitários, arquiteturais e de integração; corrigir erros e warnings relevantes até todos os comandos concluírem com sucesso.
- [ ] 8.5 Revisar documentação e diagramas do modelo contra todas as specs desta mudança, verificando que não foram introduzidos MediatR, AutoMapper, mensageria, read database ou requisitos fora do escopo.
