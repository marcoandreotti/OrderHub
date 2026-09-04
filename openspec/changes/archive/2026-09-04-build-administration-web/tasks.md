## 1. Fundação autenticada

- [x] 1.1 Confirmar que `add-administrative-mfa-authentication` está implementada e inventariar componentes, tokens e clientes existentes; verificar dependências antes de alterar o frontend.
- [x] 1.2 Estruturar módulos administrativos, rotas lazy e shell responsivo sem abstrações especulativas; verificar navegação e build do Quasar.
- [x] 1.3 Implementar store de sessão/unidade, guards e cliente HTTP com renovação serializada e ProblemDetails; verificar sessão expirada, troca de unidade e acesso negado em testes.

## 2. Gestão de usuários

- [x] 2.1 Implementar consultas, Commands, validators, handlers e regras para estado, papéis e associações; verificar Tenant isolation, proteção do último administrador e concessão/remoção de Owner exclusivamente por outro Owner, incluindo negação de autoelevação por Admin.
- [x] 2.2 Implementar adapters EF/Dapper e endpoints administrativos paginados de usuários; verificar filtros, conflito, acesso cruzado e negação sem alteração parcial de atribuição/remoção indevida de Owner, inclusive no cadastro e em chamadas diretas, em integração/API.
- [x] 2.3 Criar telas de listagem, formulário, papéis e associações com confirmações sensíveis; verificar percursos autorizados e proibidos na interface, sem disponibilizar concessão/remoção de Owner a Admin ou alteração do próprio papel Owner.

As tarefas 2.1–2.3 também abrangem ativação/desativação de Owner exclusivamente por outro Owner ativo e proteção do último Owner ativo. Cobrir regras de domínio/aplicação, HTTP 403 para Admin ou autoalteração de estado de Owner, conflito sem alteração parcial para perda do último Owner, operações concorrentes por Tenant e controles correspondentes na interface. Manter as tarefas pendentes até verificar esses cenários.

## 3. Módulos administrativos existentes

- [x] 3.1 Completar primeiro as consultas administrativas independentes de adicionais e grupos com contratos, Queries, validators, gateways Dapper e endpoints paginados/filtráveis; testar recursos ativos, inativos e sem vínculos, paginação estável, acesso negado, isolamento por Tenant/unidade e regressão do contrato público. Em seguida implementar gestão de categorias, produtos, variações, adicionais e grupos, usando essas consultas para pesquisa, edição e seleção inclusive além da primeira página; verificar recarga após cadastro sem vínculos, preservação de itens inativos associados, validações, ordenação e conflito de código.
- [x] 3.2 Implementar pesquisa e edição de clientes/endereços; verificar paginação, endereço principal e preservação de formulário em erro.
- [x] 3.3 Implementar gestão de cupons e formas de pagamento; verificar filtros, ativação/desativação e mensagens de conflito.

## 4. Experiência e qualidade

- [x] 4.1 Padronizar carregamento, vazio, falha, confirmação, notificações e formulários acessíveis; verificar teclado, foco e contraste nas páginas principais.
- [x] 4.2 Adicionar testes de componentes e percursos administrativos por papel/unidade; verificar que ocultação visual nunca substitui a negação da API.
- [x] 4.3 Executar typecheck, build web, formatação e todas as suítes backend relevantes; verificar zero erros/warnings relevantes e revisar contra specs/design.
