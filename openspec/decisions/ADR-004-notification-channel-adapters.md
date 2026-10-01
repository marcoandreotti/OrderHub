# ADR-004 — Adapters substituíveis para canais de notificação

## Contexto

A change `add-notification-gateway` define uma capability modular para comunicações e requer processamento assíncrono pela outbox. O escopo inicial solicitado habilita e-mail por SMTP e WhatsApp pela Cloud API da Meta, com possibilidade de adicionar outros canais depois. O monólito já possui um sender SMTP específico para autenticação que não oferece templates, consentimento, histórico nem idempotência da capability.

## Problema

Acoplar casos de uso ou o modelo de comunicação a SDKs e payloads de fornecedores tornaria novos canais e substituição de fornecedores invasivos. Também é necessário impedir que credenciais por Tenant sejam armazenadas em texto aberto junto aos dados de negócio.

## Opções

### Opção A — Integrações em cada caso de uso

É simples no início, mas duplica políticas, retries e interpretação de estados e acopla o domínio aos fornecedores.

### Opção B — Um adapter comum com branches por canal

Reduz a quantidade inicial de classes, mas mistura protocolos e dificulta evolução isolada.

### Opção C — Porta de envio por canal e adapters de fornecedor

Application publica contratos neutros de solicitação e resultado. Communications resolve finalidade, template, destino, consentimento e idempotência; Infrastructure implementa um adapter SMTP e um adapter Meta WhatsApp Cloud API. O worker recebe a solicitação pela outbox e registra resultados no banco local.

## Decisão

Adotar a Opção C. E-mail e WhatsApp são os canais habilitados inicialmente. Cada adapter declara o canal que suporta; canais futuros implementam a mesma porta sem alterar o contrato do caso de uso. SMTP reaproveita a configuração técnica e bibliotecas já disponíveis, sem alterar o sender de autenticação. WhatsApp usa a Graph API da Meta por `HttpClient`, com versão do Graph, Phone Number ID e token fornecidos por configuração protegida de implantação. Segredos não são persistidos em tabelas tenant-scoped nem registrados em logs.

As configurações específicas de destinatário, template, consentimento, finalidade e idempotência são tenant-scoped. A outbox fornece entrega pelo menos uma vez; os adapters usam a chave idempotente sempre que o protocolo permitir. Timeouts ambíguos permanecem identificados como resultado incerto para que a recuperação não afirme sucesso sem confirmação.

## Consequências

### Positivas

- Novos canais podem ser adicionados como adapters sem alterar os contratos de comunicação.
- Casos de uso não dependem dos protocolos SMTP ou Meta.
- Credenciais são fornecidas pelo ambiente de implantação e não expostas a operações de Tenant.
- A outbox desacopla commits de negócio da disponibilidade dos fornecedores.
- O adapter inicial usa credenciais de sandbox do processo, compartilhadas pelos Tenants. Configuração de contas produtivas independentes por Tenant exige uma integração explícita com secret manager e não deve armazenar segredos em `appsettings` versionado ou nas tabelas de Communications.

### Negativas

- Entrega externa não pode participar atomicamente da transação PostgreSQL; o envio pode ser repetido após falhas.
- Status de entrega definitivo de provedores que dependam de webhook pode exigir uma change posterior, com autenticação e correlação de callbacks.
- Sandbox/produção precisam de credenciais e templates habilitados no fornecedor para envio real.
- Esta change não habilita envio produtivo com contas de fornecedor independentes por Tenant; adapters permanecem configurados em nível de implantação.
