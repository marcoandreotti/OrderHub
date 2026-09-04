## Why

As capacidades administrativas existem no backend, mas o estabelecimento ainda não possui uma interface utilizável para operá-las. Uma aplicação administrativa consolida essas APIs em fluxos seguros, responsivos e compreensíveis para proprietários e gerentes.

## What Changes

- Criar a área web autenticada de administração usando Vue, Quasar, TypeScript e os tokens de tema existentes.
- Adicionar navegação, seleção de unidade autorizada, estados de carregamento/erro/vazio e tratamento uniforme de ProblemDetails.
- Criar gestão de usuários, papéis e associações de unidade, incluindo os endpoints administrativos ainda necessários.
- Restringir a concessão e remoção do papel Owner a um Owner atuando sobre outro usuário do mesmo Tenant, impedindo elevação de privilégios por Admin.
- Restringir ativação e desativação de usuários com papel Owner a outro Owner ativo e impedir operações que deixem o Tenant sem Owner ativo, inclusive sob concorrência.
- Criar telas de catálogo, clientes, cupons e formas de pagamento sobre as APIs existentes.
- Completar as consultas administrativas do catálogo com listagens independentes de adicionais e grupos, incluindo ativos, inativos e recursos sem vínculos, preservando o contrato e os filtros do cardápio público.
- Aplicar autorização também na experiência visual, sem tratar ocultação de controles como substituta da autorização no servidor.
- Adicionar validação de formulários, paginação, filtros, confirmação de ações sensíveis e testes dos fluxos principais.

## Capabilities

### New Capabilities

- `administration/administration-web`: experiência web administrativa autenticada, navegação por capacidade e gestão das funcionalidades disponíveis.

### Modified Capabilities

- `identity/administrative-users`: usuários autorizados podem ser consultados e administrados, com papéis e associações explícitas por unidade.
- `administration/administration-api`: passa a oferecer os contratos paginados e operações necessárias à gestão de usuários administrativos e consultas independentes de adicionais e grupos do catálogo.

## Impact

Afeta o frontend Quasar, cliente HTTP, roteamento, estado de sessão, contratos da API, os fluxos de Identity necessários à gestão de usuários e as leituras administrativas de adicionais e grupos do catálogo. Depende de `add-administrative-mfa-authentication` concluída e não altera regras de negócio de catálogo, clientes, cupons ou pagamentos já protegidas pelo backend, nem o contrato do cardápio público.
