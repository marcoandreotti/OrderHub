## Purpose

Define garantias observáveis do armazenamento PostgreSQL para o núcleo, incluindo integridade, isolamento e evolução reproduzível.

## ADDED Requirements

### Requirement: Modelo relacional preserva invariantes estruturais
O armazenamento MUST aplicar chaves primárias, estrangeiras, nulabilidade, precisão, limites, unicidades e checks necessários para impedir estados estruturalmente inválidos mesmo fora do fluxo HTTP.

#### Scenario: Relação inválida
- **WHEN** uma gravação tentar persistir referência inexistente ou violar unicidade tenant-scoped
- **THEN** o PostgreSQL MUST rejeitar a alteração atomicamente

### Requirement: Escrita e leitura usam o mesmo banco com caminhos separados
O sistema SHALL usar EF Core no fluxo de escrita e Dapper no fluxo de leitura sobre o mesmo PostgreSQL, sem tabelas `_read`/`_write` ou banco de leitura separado nesta capacidade.

#### Scenario: Consulta de projeção
- **WHEN** uma consulta precisar combinar dados de várias tabelas
- **THEN** ela SHALL usar uma projeção de leitura sem carregar aggregates para produzir o resultado

### Requirement: Consultas são isoladas por Tenant e unidade
Toda operação persistente MUST aplicar o escopo de Tenant e, quando aplicável, de unidade, incluindo relações que não carreguem o identificador diretamente.

#### Scenario: Teste com dois Tenants
- **WHEN** dados equivalentes existirem em dois Tenants e uma consulta for executada para um deles
- **THEN** somente os dados do Tenant e unidade autorizados SHALL ser retornados ou alterados

### Requirement: Migrations possuem ciclo reproduzível e isolado
As migrations PostgreSQL SHALL ser geradas e executadas por projeto dedicado de Infrastructure, sem referência das camadas Domain ou Application e com comandos documentados para criação, aplicação e rollback seguro.

#### Scenario: Banco vazio
- **WHEN** todas as migrations forem aplicadas em PostgreSQL vazio suportado
- **THEN** o schema completo SHALL ser criado em ordem determinística e a aplicação SHALL iniciar sem alterações manuais

#### Scenario: Rollback de migration
- **WHEN** uma migration reversível for removida em ambiente controlado sem dados incompatíveis
- **THEN** o comando documentado SHALL restaurar o schema anterior ou SHALL falhar de modo explícito quando reversão segura não existir

### Requirement: Operações relacionais críticas são atômicas
Alterações que envolvam aggregate, histórico, uso de cupom ou confirmação financeira MUST ser persistidas na mesma transação quando compuserem um único efeito de negócio.

#### Scenario: Falha parcial
- **WHEN** qualquer gravação de um efeito atômico falhar
- **THEN** nenhuma das alterações desse efeito SHALL permanecer persistida

