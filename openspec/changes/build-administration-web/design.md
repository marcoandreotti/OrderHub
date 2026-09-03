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
