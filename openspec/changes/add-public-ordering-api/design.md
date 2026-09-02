## Context

As capacidades de domínio anteriores fornecem casos de uso internos; esta change adiciona somente a borda pública e contratos externos. O catálogo público existente permanece a fonte de ofertas apresentáveis.

## Goals / Non-Goals

**Goals:**

- Expor fluxo anônimo seguro e idempotente.
- Resolver escopo publicamente sem conceder privilégios administrativos.
- Manter Controllers/endpoints finos e contratos explícitos.

**Non-Goals:**

- Autenticação de cliente, notificações em tempo real ou frontend.
- Aceitar preços, totais ou TenantId do cliente como autoridade.

## Decisions

### Rotas públicas orientadas por slug e referência opaca

O slug identifica a unidade e uma referência aleatória identifica o pedido. GUIDs internos e números sequenciais não serão usados como credencial pública por serem enumeráveis ou revelarem implementação.

### Orquestração via Commands e Queries

Endpoints mapearão requests para dispatchers existentes; regras permanecerão no domínio. Acesso direto a EF/Dapper foi rejeitado pelas fronteiras arquiteturais.

### Confirmação idempotente como operação única

Cliente, endereço, composição, cupom e forma serão revalidados antes do commit. Gravar etapas independentes produziria pedidos parciais em falhas intermediárias.

## Risks / Trade-offs

- [Referência pública pode vazar por compartilhamento] → Usar alta entropia e limitar os dados retornados.
- [Simulação diverge da confirmação] → Declarar simulação não reservante e revalidar tudo no commit.

## Migration Plan

Publicar contratos e endpoints somente após as changes dependentes; validar OpenAPI, isolamento e idempotência com testes de integração antes de habilitar o frontend.
