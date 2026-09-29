## Context

O `PlatformSuperUser` já é uma identidade global separada, autenticada por sessão persistida e reconhecida por políticas próprias. `Tenant`, `Establishment`, `AdministrativeUser`, papéis, associações e onboarding existem, mas a única criação de unidade disponível na Application depende de `ITenantContext.GetRequiredTenantId()` e não é exposta pela API. A consulta de contexto retorna unidades existentes para a plataforma, portanto um banco vazio não oferece ponto de entrada. Consulte `proposal.md` para a motivação e os delta specs para os contratos observáveis.

## Goals / Non-Goals

**Goals:**

- oferecer um bootstrap global seguro sem converter a identidade de plataforma em usuário de Tenant;
- garantir atomicidade, isolamento, idempotência e retomada após resposta desconhecida;
- reutilizar aggregates, hashing, dispatchers, políticas, `ProblemDetails` e onboarding existentes;
- separar claramente a administração global das rotas e do estado tenant-scoped.

**Non-Goals:**

- autoatendimento público, cobrança, assinatura, trial ou provisionamento de infraestrutura;
- substituir o onboarding ou concluir automaticamente sua prontidão;
- introduzir mensageria, microservices, MediatR, AutoMapper ou abstrações genéricas especulativas;
- conceder papel tenant-scoped ao `PlatformSuperUser`.

## Decisions

### 1. Usar casos de uso globais dedicados

Serão criados Commands e Queries específicos de plataforma para provisionar Tenant, adicionar unidade, listar Tenants e consultar intenção. Eles validarão a identidade global vigente por uma porta de autenticação, em vez de fabricar `TenantId` no `HttpTenantContext`.

Reutilizar diretamente o `CreateEstablishmentCommand` atual foi rejeitado porque ele deriva obrigatoriamente o Tenant do principal e representa uma operação tenant-scoped. Alterar o contexto para aceitar Tenant informado pelo cliente criaria ambiguidade de autorização.

### 2. Persistir o provisionamento em uma única transação

Tenant, primeira unidade, primeiro Owner, papel Owner, associação e registro da intenção serão gravados por uma unidade de trabalho EF Core na mesma transação. O handler orquestra os aggregates; repositories não conterão regras de negócio. Constraints de banco continuarão sendo a última defesa contra concorrência.

Criar cada recurso por endpoints independentes foi rejeitado porque exporia estados órfãos e exigiria recuperação manual após falhas intermediárias.

### 3. Modelar idempotência com intenção persistida

Uma intenção de provisionamento armazenará ator global, chave opaca, hash canônico dos dados relevantes, estado e IDs do resultado. A unicidade será por ator e chave. Repetições equivalentes retornam o resultado; conteúdo diferente gera conflito. A intenção concluída participa da mesma transação dos recursos.

Confiar apenas nas unicidades de Tenant, slug ou e-mail foi rejeitado porque não diferencia retry legítimo de uma nova solicitação conflitante e não recupera o resultado original.

### 4. Manter contratos globais em namespace e rotas próprios

A borda HTTP usará grupo global, por exemplo `/api/platform/tenants`, protegido por política exclusiva de plataforma. Contratos não receberão `TenantId` como autoridade no provisionamento inicial; para unidade adicional, o identificador do Tenant é o alvo explícito de uma operação global e será resolvido no servidor. As rotas `/api/admin/establishments/{id}` permanecem operacionais e tenant-scoped.

### 5. Criar credencial temporária restrita para o primeiro Owner

O primeiro Owner será criado ativo, associado à primeira unidade e marcado para troca de senha. O fluxo de autenticação tenant existente será estendido para emitir sessão restrita também para `AdministrativeUser`, permitindo somente contexto, troca de senha e logout até a alteração. A mudança revoga sessões anteriores e exige novo login.

Manter a senha definida pelo operador de plataforma como definitiva foi rejeitado por ampliar desnecessariamente o conhecimento de credenciais. Convite sem senha foi adiado porque exigiria contrato de token e entrega de e-mail adicional além do necessário para desbloquear o provisionamento.

### 6. Separar shell global e seleção operacional na web

A web terá rota de plataforma, estado vazio, lista/formulário e retomada pela chave de intenção. Após sucesso, reutilizará a store de sessão para selecionar a unidade retornada e seguirá para o onboarding. Voltar à área global invalidará caches tenant-scoped sem encerrar a sessão.

### 7. Preparar auditoria sem acoplamento prematuro

O registro de intenção preservará o ator global e os alvos criados. Se `add-audit-trail` já estiver aplicado, a operação publicará o registro pelo contrato interno definido naquela change; caso contrário, os dados mínimos continuarão disponíveis no próprio provisionamento. Não será criada dependência circular nem mensageria apenas para esta feature.

## Risks / Trade-offs

- [Risco] Concorrência criar recursos duplicados antes da confirmação da intenção → constraints únicas, transação única e tradução padronizada para conflito.
- [Risco] Senha temporária aparecer em logs ou respostas posteriores → recebê-la somente na criação, armazenar hash e nunca retorná-la em consultas ou `ProblemDetails`.
- [Risco] Sessão restrita de usuário tenant afetar logins existentes → coluna aditiva com padrão compatível e testes separados para usuário comum, Owner provisionado e plataforma.
- [Risco] Estado global vazar para telas da unidade → stores distintas e invalidação de consultas na transição de contexto.
- [Trade-off] A primeira versão exige que o operador entregue a senha temporária por canal seguro → convite autoatendido fica fora do escopo e pode evoluir em change própria.

## Migration Plan

1. Adicionar de forma compatível a marca de troca de senha do usuário administrativo e a persistência das intenções, com constraints e índices.
2. Publicar domínio/aplicação, adapters e rotas globais mantendo a web anterior funcional.
3. Publicar a área web de plataforma e habilitar a jornada após validar API, migration e autorização em ambiente não produtivo.
4. Verificar banco vazio, banco com Tenants existentes, retry, concorrência e rollback de falha intermediária.
5. Em rollback, remover a exposição web e das rotas; manter tabelas/colunas aditivas até uma migration posterior segura, sem apagar Tenants já provisionados.
