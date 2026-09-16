## Context

Tenant, Establishment, tema, usuários, associações, mesas e horários já existem no domínio e na persistência, porém não há API completa nem experiência guiada que coordene sua configuração. A mudança cruza quatro capacidades sem fundi-las em um novo agregado artificial.

## Goals / Non-Goals

**Goals:**

- Coordenar configuração progressiva preservando a autoridade de cada agregado.
- Calcular prontidão de maneira verificável e retomável.
- Expor QR Codes sem identificadores internos e com revogação efetiva.

**Non-Goals:**

- Cobrança de assinatura, documentos fiscais, domínio personalizado, integração com mapas ou provisionamento de infraestrutura.
- Criar transação longa que mantenha todas as etapas abertas.

## Decisions

1. **Onboarding como processo de Application, não agregado centralizador.** Cada etapa usa commands do bounded context proprietário. Um read model agrega progresso; duplicar regras em um “Onboarding” de domínio foi rejeitado.
2. **Prontidão calculada com marco de conclusão.** Critérios objetivos são recalculados dos recursos atuais; registrar `CompletedAt` apenas como histórico, não como fonte única de verdade.
3. **Etapas idempotentes.** Operações de criação iniciadas pela interface usam chave de intenção quando repetição puder duplicar recursos; updates naturais usam identificador estável.
4. **Substituição atômica da grade semanal.** O payload completo simplifica validação e impede horários parcialmente atualizados.
5. **Token de mesa opaco e rotacionável.** Persistir representação protegida quando possível; QR é renderizado no frontend a partir da URL pública retornada. Armazenar imagens de QR no banco foi rejeitado.
6. **APIs permanecem finas e separadas por capacidade.** Uma consulta de progresso pode compor leituras, mas writes despacham comandos específicos e não acessam EF/Dapper diretamente.
7. **Interface como wizard retomável dentro da administração.** Rotas por etapa permitem recarregar e voltar; o servidor informa estado atual e valida cada avanço.

## Risks / Trade-offs

- [Coordenação entre agregados deixa estado parcial válido] → Tratar cada etapa como commit independente e retomável; conclusão exige revalidação total.
- [Critérios de prontidão evoluem] → Centralizar cálculo na Application/read model e cobrir cenários, sem gravar booleano irreversível.
- [QR antigo continua circulando] → Renovação invalida imediatamente o token anterior e a UI avisa sobre reimpressão.
- [Escopo cresce para todo o backoffice] → Limitar às cinco etapas declaradas e reutilizar capacidades existentes.

## Migration Plan

1. Adicionar portas, endpoints e eventual persistência de progresso de forma aditiva.
2. Entregar consultas e comandos por capacidade com testes de isolamento.
3. Integrar o wizard após autenticação e shell administrativo disponíveis.
4. Ativar conclusão guiada sem impedir unidades existentes de operar; calcular estado inicial a partir de seus dados.
5. Rollback remove a interface/processo, preservando configurações de negócio já válidas.
