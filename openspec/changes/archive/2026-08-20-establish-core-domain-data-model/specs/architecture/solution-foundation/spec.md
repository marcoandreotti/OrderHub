## MODIFIED Requirements

### Requirement: Isolamento Multi-Tenant
O sistema MUST representar Tenant como grupo proprietário de uma ou mais unidades operacionais. Toda informação operacional MUST ser associada ao Tenant autenticado e à unidade aplicável, e todas as operações MUST impedir leitura ou alteração cruzada entre Tenants e entre unidades sem associação ativa do usuário no mesmo Tenant.

#### Scenario: Tenant informado pelo cliente
- **WHEN** o cliente enviar um TenantId diferente daquele resolvido pelo contexto autenticado
- **THEN** o sistema MUST ignorar esse valor para autorização e MUST impedir acesso aos dados de outro Tenant

#### Scenario: Unidade informada pelo cliente
- **WHEN** o cliente enviar uma unidade que não pertença ao Tenant resolvido ou à qual o ator não tenha acesso
- **THEN** o sistema MUST negar a operação sem revelar dados da unidade

#### Scenario: Seleção autenticada da unidade
- **WHEN** a rota ou claim indicar uma unidade e o usuário não possuir associação ativa correspondente no Tenant autenticado
- **THEN** o sistema MUST negar a criação do contexto operacional independentemente do identificador enviado no payload

#### Scenario: Consulta de dados do estabelecimento
- **WHEN** uma consulta for executada em contexto de Tenant e unidade
- **THEN** apenas dados pertencentes a ambos os escopos autorizados SHALL compor o resultado

## ADDED Requirements

### Requirement: Migrations isoladas das camadas internas
As migrations PostgreSQL SHALL residir em projeto dedicado da camada Infrastructure, usado para design time e deployment, e Domain e Application MUST NOT referenciar esse projeto nem bibliotecas de persistência.

#### Scenario: Validação de referências
- **WHEN** testes arquiteturais analisarem o projeto de migrations
- **THEN** referências das camadas Domain ou Application ao projeto de migrations SHALL ser rejeitadas
