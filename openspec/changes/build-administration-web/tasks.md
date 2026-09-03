## 1. Fundação autenticada

- [ ] 1.1 Confirmar que `add-administrative-mfa-authentication` está implementada e inventariar componentes, tokens e clientes existentes; verificar dependências antes de alterar o frontend.
- [ ] 1.2 Estruturar módulos administrativos, rotas lazy e shell responsivo sem abstrações especulativas; verificar navegação e build do Quasar.
- [ ] 1.3 Implementar store de sessão/unidade, guards e cliente HTTP com renovação serializada e ProblemDetails; verificar sessão expirada, troca de unidade e acesso negado em testes.

## 2. Gestão de usuários

- [ ] 2.1 Implementar consultas, Commands, validators, handlers e regras para estado, papéis e associações; verificar Tenant isolation e proteção do último administrador em testes.
- [ ] 2.2 Implementar adapters EF/Dapper e endpoints administrativos paginados de usuários; verificar filtros, conflito e acesso cruzado em integração/API.
- [ ] 2.3 Criar telas de listagem, formulário, papéis e associações com confirmações sensíveis; verificar percursos autorizados e proibidos na interface.

## 3. Módulos administrativos existentes

- [ ] 3.1 Implementar gestão de categorias, produtos, variações, adicionais e grupos sobre a API de catálogo; verificar validações, ordenação e conflito de código.
- [ ] 3.2 Implementar pesquisa e edição de clientes/endereços; verificar paginação, endereço principal e preservação de formulário em erro.
- [ ] 3.3 Implementar gestão de cupons e formas de pagamento; verificar filtros, ativação/desativação e mensagens de conflito.

## 4. Experiência e qualidade

- [ ] 4.1 Padronizar carregamento, vazio, falha, confirmação, notificações e formulários acessíveis; verificar teclado, foco e contraste nas páginas principais.
- [ ] 4.2 Adicionar testes de componentes e percursos administrativos por papel/unidade; verificar que ocultação visual nunca substitui a negação da API.
- [ ] 4.3 Executar typecheck, build web, formatação e todas as suítes backend relevantes; verificar zero erros/warnings relevantes e revisar contra specs/design.
