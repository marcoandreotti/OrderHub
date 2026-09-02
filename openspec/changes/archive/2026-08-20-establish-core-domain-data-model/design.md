## Context

A fundação atual já estabelece monólito modular, CQRS próprio, Domain independente, EF Core para escrita, Dapper para leitura e PostgreSQL compartilhado. O repositório ainda não possui modelo funcional persistido. O anexo propõe vinte estruturas relacionais e pede limites de aggregates, com destaque para Tenant, Categoria, Produto, Cupom e Pedido. Ver `proposal.md` e os deltas em `specs/`.

Há uma distinção necessária: `Tenant` representa o grupo contratante, enquanto `Establishment` representa a unidade operacional. Portanto, dados de operação pertencem simultaneamente ao Tenant e a uma unidade; tema, slug, catálogo e pedidos são unit-scoped, mesmo que os nomes de tabela sugeridos originalmente usassem apenas `tenant_id`.

## Goals / Non-Goals

**Goals:**

- Fixar aggregates, ownership, referências entre módulos e invariantes antes das migrations.
- Definir um schema PostgreSQL normalizado, seguro para dois níveis de escopo e adequado a EF Core/Dapper.
- Preservar histórico comercial em pedidos, cupons e pagamentos.
- Permitir implementação incremental com migrations reproduzíveis e testes por camada.

**Non-Goals:**

- Criar um aggregate único que carregue todo o grafo relacional.
- Implementar autenticação de cliente, adquirente externo, feriados, estoque, fiscal, entrega geográfica ou notificações.
- Adicionar read database, tabelas duplicadas, event sourcing, outbox ou mensageria.
- Prometer profundidade ilimitada de categoria no domínio; o domínio apenas não impõe limite artificial e protege contra ciclos.

## Decisions

### 1. Tenant é grupo; Establishment é a fronteira operacional

`Tenant` será aggregate root do cadastro do grupo e governará a associação de unidades. `Establishment` será aggregate root da unidade e de sua configuração visual. Entidades operacionais persistirão `tenant_id` e `establishment_id`; tabelas filhas poderão omitir esses campos somente quando a FK para o pai e as queries garantirem o mesmo escopo. Slug será propriedade da unidade, globalmente único, pois é ela que recebe acesso público.

Alternativas consideradas: fundir Tenant e unidade simplificaria o primeiro release, mas impediria múltiplas unidades; manter apenas `tenant_id` nos dados confundiria grupo e local de operação e permitiria associações indevidas entre unidades.

### 2. Limites de aggregates pequenos e referências por identidade

- `Tenant`: grupo e estado; não carrega todas as unidades em operações comuns.
- `Establishment`: dados da unidade e tema 1:1.
- `AdministrativeUser`: credencial/estado, associações de perfil e associações explícitas às unidades autorizadas; perfis conhecidos são dados de referência.
- `Category`: cada nó protege seu vínculo de pai; detecção de ciclo usa uma porta de consulta quando a árvore não estiver carregada.
- `Product`: produto, imagens, variações e vínculos ordenados a grupos de adicionais.
- `AdditionalGroup`: limites e itens adicionais; `Additional` é aggregate independente e reutilizável.
- `Customer`: cliente e endereços.
- `Table`, `Coupon`, `PaymentMethod` e `BusinessHours`: roots independentes.
- `Order`: itens, adicionais em snapshot, cupom aplicado e histórico de estados. Pagamentos são aggregates separados para admitir ciclo financeiro e integrações futuras, referenciando o pedido.

Alternativa considerada: um aggregate de catálogo completo ou Tenant contendo todos os dados produziria grafos grandes, contenção e transações desnecessárias. Relações entre aggregates são validadas no handler/domain service por portas explícitas e persistidas por identidade.

### 3. Vocabulário de valores e estados

IDs tipados, `Money`, `Email`, `PhoneNumber`, `Document`, `Address`, `Slug`, `OrderNumber`, `SelectionRange` e `TimeRange` encapsulam validação quando houver regra real. Dinheiro usa `numeric(12,2)` e quantidade usa `numeric(10,3)`; arredondamento monetário será explícito e consistente. Status de pedido, atendimento, desconto e pagamento são enums de domínio persistidos como `smallint`, com conversões explícitas.

As transições de pedido são uma máquina de estados do aggregate. O caminho feliz varia por atendimento: entrega admite `SaiuParaEntrega`/`Entregue`; mesa e retirada não passam por estados exclusivos de entrega. `Finalizado`, `Cancelado` e `Rejeitado` são terminais. Cada transição grava `OrderHistory` na mesma transação.

Alternativa considerada: strings no banco facilitariam inspeção, mas ampliariam armazenamento e risco de divergência; enums nativos PostgreSQL acoplariam migrations e deploys. `smallint` com check constraints equilibra integridade e evolução.

### 4. Snapshots fazem parte do Order

`OrderItem` copia nome do produto, nome da variação e preço unitário; `OrderItemAdditional` copia nome e preço; o cupom aplicado copia código e desconto. O endereço de entrega será snapshot owned pelo pedido, não FK mutável para `CustomerAddress`, ainda que o contrato inicial use um identificador de origem opcional. Alterações ou remoções no catálogo e cliente não mudam pedidos históricos.

Alternativa considerada: manter apenas FKs reduz duplicação, mas viola auditabilidade e torna o histórico dependente do estado atual.

### 5. Integridade e unicidade são aplicadas em mais de uma camada

O Domain protege invariantes de negócio; validators protegem formato; PostgreSQL protege integridade estrutural. Índices únicos incluem slug global, `(tenant_id, normalized_email)` para usuário, `(establishment_id, code)` para categoria/produto/cupom/forma de pagamento/mesa conforme aplicável, `(establishment_id, number)` para pedido e token público de mesa global. Checks cobrem valores não negativos, limites de seleção, períodos e enums válidos.

FKs não usarão cascade indiscriminado. Filhos internos do aggregate podem ser removidos em cascade; referências históricas usam `RESTRICT` ou permanecem opcionais com snapshot. Desativação lógica é preferida para recursos já utilizados.

### 6. Concorrência explícita em numeração, cupom e pagamento

Número de pedido será alocado atomicamente por unidade, por sequência/contador transacional dedicado, e não por `MAX(numero)+1`. Consumo de cupom com limite usa atualização condicional ou controle de concorrência. Confirmação financeira usa idempotência tenant-scoped e constraint para identificador externo quando presente. Totais e histórico são persistidos na mesma transação do pedido.

Alternativa considerada: locks em memória não funcionam com múltiplas instâncias e não protegem concorrência direta no banco.

### 7. Mesmo PostgreSQL, modelos de acesso separados

Os mappings EF Core e repositories de escrita ficam nos adapters Infrastructure dos módulos. Gateways Dapper mantêm SQL e projeções próprios, sempre com filtros explícitos de Tenant e unidade. Não haverá entidades compartilhadas entre os caminhos, read schema duplicado nem abstração genérica de repository.

### 8. Projeto dedicado para migrations dentro de Infrastructure

Será criado um projeto executável/design-time dedicado, por exemplo `OrderHub.Infrastructure.Migrations`, que referencia apenas os adapters de persistência e pacotes EF necessários para localizar os `DbContext`s. API, Application e Domain não o referenciam; deployment executa a ferramenta explicitamente antes da aplicação. O projeto não constitui nova camada arquitetural e não justifica ADR porque concretiza a arquitetura já aprovada; caso a implementação exija inversão dessa regra, um ADR será necessário antes do código.

Migrations serão organizadas por contexto/schema de módulo quando a solution existente suportar múltiplos contexts, mas aplicadas em ordem documentada. Uma migration inicial não mistura seed de dados operacionais; somente perfis de referência estritamente necessários podem usar seed determinístico.

### 9. Implementação será fatiada por dependência

Primeiro entram tipos compartilhados, Tenancy e projeto de migrations; depois Identity e configuração operacional; em seguida catálogo e clientes; então pedido/cupom; por fim pagamentos e gateways de leitura. Cada fatia inclui testes de domínio, aplicação, persistência com dois Tenants/unidades e migration em banco vazio antes da próxima.

### 10. Seleção de unidade exige associação explícita do usuário

Usuários administrativos pertencem ao Tenant e terão uma relação N:N persistida com `Establishment`. A seleção da unidade pode chegar por claim emitida após seleção autenticada ou por segmento de rota, mas nunca será aceita como prova de autorização. Um serviço de Application recebe Tenant, usuário e unidade do contexto autenticado, consulta a associação ativa por uma porta e somente então produz o contexto operacional usado pelos handlers.

A associação possui estado ativo para permitir revogação sem apagar auditoria. A constraint relacional impede que usuário e unidade de Tenants diferentes sejam associados. Perfis continuam definindo o que o usuário pode fazer; a associação de unidade define onde pode fazer. Proprietário e administrador não recebem acesso implícito a todas as unidades nesta versão: suas associações devem ser provisionadas explicitamente.

Alternativas consideradas: acesso automático a todas as unidades do Tenant reduziria cadastros, mas impediria restrição por filial; confiar diretamente em `establishment_id` de claim/rota misturaria seleção com autorização e permitiria contexto obsoleto após revogação.

## Risks / Trade-offs

- [Duplicar `tenant_id` e `establishment_id` aumenta colunas e risco de inconsistência] → FKs compostas/constraints quando úteis, preenchimento server-side e testes com dois grupos e duas unidades.
- [Muitas capacidades em uma única mudança aumentam o tamanho da implementação] → tarefas fatiadas em marcos compiláveis, sem exigir endpoint completo para todas as tabelas de uma vez.
- [Categoria recursiva pode gerar ciclo e consultas caras] → validação de ancestralidade, índice no pai e projeção Dapper dedicada.
- [Contadores e cupons sofrem corrida] → primitivas atômicas do PostgreSQL e testes concorrentes.
- [Cascade pode apagar histórico] → cascade apenas dentro do aggregate e restrição/desativação para recursos referenciados.
- [Projeto de migrations depender dos adapters pode crescer como composition root paralelo] → limitar responsabilidade a design time/aplicação de schema e validar referências arquiteturais.
- [Horário cruzando meia-noite não cabe no modelo inicial] → exigir intervalos dentro do dia; uma futura spec poderá modelar virada de dia e exceções.
- [Validar associação em toda operação aumenta consultas] → resolver uma vez por request em serviço scoped, sem cache além da request para que revogações sejam observadas rapidamente.

## Migration Plan

1. Confirmar os schemas/módulos e criar o projeto dedicado de migrations com comandos documentados.
2. Criar migrations incrementais na mesma ordem das fatias, aplicando-as primeiro em PostgreSQL vazio e depois em banco com dados de teste.
3. Executar testes de integridade, concorrência e isolamento com pelo menos dois Tenants e duas unidades no mesmo Tenant.
4. Aplicar migrations como etapa explícita anterior ao deploy da API e verificar health/readiness.
5. Enquanto não houver dados de produção, rollback pode voltar à migration anterior. Após dados reais, toda migration destrutiva exigirá estratégia de expansão/contração, backup verificado e plano específico; operações irreversíveis devem falhar de modo explícito.
