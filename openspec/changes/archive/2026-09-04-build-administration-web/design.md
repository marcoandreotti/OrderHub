## Context

O frontend possui layouts públicos, operacionais e administrativos, tokens de tema, Axios e Pinia, mas apenas uma página de fundação. A API já atende catálogo, clientes, pedidos, cupons e pagamentos; gestão completa de usuários ainda requer endpoints próprios.

## Goals / Non-Goals

**Goals:**

- Estabelecer uma arquitetura frontend modular por capacidade e uma experiência administrativa coerente.
- Consumir contratos da API sem duplicar regras de domínio no navegador.
- Tornar autorização, troca de unidade e ProblemDetails comportamentos transversais previsíveis.

**Non-Goals:**

- Implementar o painel operacional ou a jornada pública nesta mudança.
- Introduzir SSR, novo framework de estado ou biblioteca de componentes além de Quasar.

## Decisions

1. **Módulos frontend por capacidade.** Cada área terá páginas, componentes, client tipado e store apenas quando houver estado compartilhado. Um diretório global de services foi rejeitado porque mistura contextos.
2. **Router como fronteira de acesso.** Metadados de rota expressam sessão e capacidade necessárias; guards hidratam o contexto e tratam expiração. A API continua sendo autoridade final.
3. **Unidade ativa em store de sessão.** A seleção usa somente associações retornadas pelo servidor, é persistida sem claims sensíveis e invalida caches ao mudar.
4. **Cliente HTTP central.** Interceptors tratam correlação, CSRF quando aplicável, renovação serializada e tradução de ProblemDetails; cada módulo preserva seus DTOs explícitos.
5. **Server state simples.** Usar composables e stores pequenas, sem adicionar biblioteca de cache antes de dois casos reais justificarem a abstração.
6. **Formulários orientados a contratos.** Validar ergonomia no cliente, mas sempre exibir a decisão autoritativa do servidor. Edições usam páginas/drawers consistentes e confirmações para ações sensíveis.
7. **Gestão de usuários como extensão vertical.** Adicionar CQRS, persistência/leitura e endpoints faltantes antes de ligar as telas; regras de último administrador ficam no domínio/aplicação.
8. **Papel Owner protegido no servidor.** Na gestão de usuários do Tenant, somente um Owner pode conceder ou remover Owner de outro usuário do mesmo Tenant. Admin não pode atribuir Owner no cadastro, promover a si próprio ou terceiros, nem remover Owner; um Owner também não pode alterar seu próprio papel Owner. A autorização usa o contexto autenticado, aplica-se a todas as operações que alterem papéis e preserva a proteção do último administrador. A interface reflete essas restrições, mas chamadas diretas também são negadas sem alterações parciais. Esta decisão não redefine a identidade ou os poderes globais de PlatformSuperUser.

9. **Estado de Owner e continuidade da propriedade.** Ativar ou desativar um usuário que possui Owner exige outro Owner ativo do mesmo Tenant; a restrição considera o papel persistido mesmo quando o destinatário está inativo. Admin não pode executar essas operações, e Owner não pode desativar a si próprio. Além da proteção do último administrador, nenhuma alteração de estado ou papéis pode deixar o Tenant sem Owner ativo. Verificação e escrita devem ser atômicas e coordenadas por Tenant, com revalidação do autor e do estado vigente, para impedir que operações concorrentes removam os últimos Owners. Falta de autorização resulta em proibição; violação da continuidade por uma operação autorizada resulta em conflito, sempre sem alterações parciais. Não são redefinidos os poderes globais de PlatformSuperUser; a continuidade do Owner ativo é uma invariante do Tenant, não uma permissão dispensável.

10. **Consultas independentes para manutenção do catálogo.** A árvore atual retorna grupos somente dentro de produtos e adicionais somente dentro de grupos; ela não permite manter recursos ainda sem vínculos. Adicionar GET em `/api/admin/establishments/{establishmentId}/catalog/additionals` e `/api/admin/establishments/{establishmentId}/catalog/additional-groups`, mantendo os endpoints de escrita existentes. Usar Queries, validators e gateways Dapper com capacidade `management`, Tenant resolvido pelo contexto e validação da unidade autorizada. As coleções aceitam pesquisa por nome, filtro opcional de atividade e paginação limitada (padrão 20, máximo 100), com ordenação estável por nome e ID. Sem filtro de atividade, incluir ativos e inativos; a existência de vínculos não condiciona a listagem. Grupos retornam limites e itens associados com IDs, ordem e estado necessários à edição, inclusive itens inativos. A interface usa essas consultas para listagem e seleção, sem depender da árvore de produtos nem apenas da primeira página. O contrato público e seus filtros de vendabilidade permanecem inalterados. Não exige nova persistência ou mudança arquitetural.

## Risks / Trade-offs

- [Grande quantidade de telas] → Entregar por fatias: shell/sessão, usuários, catálogo, clientes, promoções/pagamentos.
- [Contratos frontend divergirem da API] → Centralizar tipos de transporte e testar serialização/fluxos contra a API.
- [Ocultar botão ser confundido com segurança] → Testes garantem negação do backend mesmo por chamada direta.
- [Estado vazar ao trocar unidade] → Chavear ou invalidar todo estado tenant-scoped na transição.

## Migration Plan

1. Introduzir shell administrativo e integração de sessão sem remover a página de fundação.
2. Entregar gestão de usuários e depois módulos que já possuem API.
3. Substituir a rota administrativa inicial quando os percursos essenciais estiverem cobertos.
4. Rollback preserva APIs e retorna temporariamente à página de fundação.
