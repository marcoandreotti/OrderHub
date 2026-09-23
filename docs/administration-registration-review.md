# Revisão dos cadastros administrativos

## Escopo

Revisão executada pela change `review-cadastros-existentes` em 23/09/2026. O objetivo foi restaurar os fluxos já previstos pelas specs, sem criar novos cadastros ou ampliar a API.

## Matriz de diagnóstico

| Área | Ação | Causa observada | Comportamento esperado | Evidência |
|---|---|---|---|---|
| Catálogo | Cadastrar categoria, produto, adicional ou grupo | A ação dependia de `loading`; o GET da listagem mantinha o botão desabilitado | Com unidade autorizada, abrir editor mesmo durante carga; sem unidade, manter bloqueado | `registration-actions.test.ts` e testes de catálogo |
| Clientes | Cadastrar cliente | A ação dependia de `loading` | Com unidade autorizada, abrir editor mesmo durante carga; sem unidade, manter bloqueado | `registration-actions.test.ts` e `customers.test.ts` |
| Cupons | Cadastrar cupom | A ação dependia de `loading` | Com unidade autorizada, abrir editor mesmo durante carga; sem unidade, manter bloqueado | `registration-actions.test.ts` e testes de promoções |
| Formas de pagamento | Cadastrar forma | A ação dependia de `loading` | Com unidade autorizada, abrir editor mesmo durante carga; sem unidade, manter bloqueado | `registration-actions.test.ts` e testes de pagamentos |
| Usuários | Novo usuário | A abertura já dependia somente da unidade, mas `save()` aceitava reentrada | Abrir durante carga, bloquear sem unidade e enviar no máximo uma criação por vez | `registration-actions.test.ts` e `users.test.ts` |
| Mesas e onboarding | Criar/editar mesa e salvar configuração | Ações dependem de `busy`, não da listagem; nenhuma regressão comum encontrada | Manter proteção durante escrita e regras de prontidão | `onboarding.test.ts` |

## Contratos HTTP

Os clients web de catálogo, clientes, cupons, formas de pagamento e usuários foram comparados com os endpoints e contracts atuais. Métodos, rotas e payloads permanecem compatíveis; nenhuma mudança de API ou domínio foi necessária. ProblemDetails continua sendo interpretado por `ApiError`, preservando dados editados nos cenários já cobertos.

## Resultado

A permissão para iniciar cadastro agora é determinada pela existência de uma unidade autorizada, não pelo estado transitório da consulta da listagem. O estado `busy` continua prevenindo duplicidade durante escritas, incluindo o cadastro de usuários após a correção desta revisão.
