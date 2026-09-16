## Why

Os blocos de Tenant, unidade, usuários, mesas, horários e tema existem no modelo, mas ainda não formam um processo guiado para colocar um estabelecimento em operação. O onboarding reduz configuração manual e garante que uma nova unidade termine em estado consistente e seguro.

## What Changes

- Criar um fluxo administrativo guiado para configurar dados da unidade, identidade visual, horários, mesas e QR Codes.
- Permitir associação inicial e manutenção de usuários autorizados à unidade.
- Adicionar consultas e comandos administrativos ausentes para tema, horários, mesas, tokens públicos e associações.
- Gerar e permitir revogar/renovar tokens opacos de mesa; o QR Code deve conter somente a URL pública segura.
- Exibir progresso e validações por etapa, permitindo retomada sem duplicar recursos já persistidos.
- Finalizar o onboarding somente quando os requisitos mínimos de operação estiverem válidos.

## Capabilities

### New Capabilities

- `tenancy/establishment-onboarding`: processo guiado e retomável que prepara uma unidade para operar.

### Modified Capabilities

- `tenancy/establishment-management`: dados, tema e prontidão operacional da unidade tornam-se administráveis por usuários autorizados.
- `operations/service-configuration`: horários e mesas passam a possuir operações administrativas, incluindo ciclo de vida dos tokens e QR Codes.
- `identity/administrative-users`: associações de usuários a unidades passam a integrar o onboarding e sua manutenção posterior.
- `administration/administration-api`: passa a expor os contratos e endpoints necessários à configuração segura da unidade.

## Impact

Afeta Tenancy, Operations, Identity, Application, persistência, migrations quando necessárias, contratos/API e frontend administrativo. Depende de autenticação administrativa real e reutiliza a estrutura visual de `build-administration-web`; integrações fiscais, cobrança da assinatura e provisionamento automatizado de infraestrutura ficam fora do escopo.
