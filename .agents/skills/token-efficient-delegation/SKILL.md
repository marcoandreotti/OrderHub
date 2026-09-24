---
name: token-efficient-delegation
description: Economize tokens ao decidir se, quando e com qual modelo delegar trabalho a subagentes. Use quando o usuário pedir execução econômica ou quando uma tarefa complexa tiver frentes independentes; não use para tarefas pequenas, estritamente sequenciais ou com edição concorrente dos mesmos arquivos.
---

# Delegação econômica

Trabalhe no Root por padrão. Delegação é uma otimização excepcional, não uma etapa obrigatória.

## Autorizar a delegação

Delegue somente quando o benefício esperado superar o custo de contexto e coordenação e ao menos uma condição for verdadeira:

- duas ou mais frentes podem avançar de forma independente;
- uma operação lenta bloquearia o Root enquanto ainda existe trabalho útil independente;
- uma investigação tem escopo pequeno, evidência definida e resultado reutilizável;
- uma revisão independente reduz materialmente risco de segurança, pagamentos, produção ou release.

Não delegue quando a tarefa é pequena, o próximo passo depende do resultado imediato, agentes tocariam os mesmos arquivos ou a coordenação custaria tanto quanto a execução direta. Não delegue apenas para aguardar.

## Manter o orçamento baixo

- Use no máximo dois subagentes ativos por padrão. Use três apenas quando três frentes forem realmente independentes.
- Prefira reutilizar um agente ocioso com `followup_task` a criar outro.
- Passe `fork_turns: "none"` e um briefing autocontido sempre que o contexto completo não for indispensável. Caso falte pouco contexto recente, passe somente os últimos turnos necessários.
- Defina escopo, fontes ou arquivos permitidos, saída esperada e critério de parada. Proíba trabalho adjacente.
- Não duplique a mesma investigação entre agentes, exceto quando uma revisão independente for o objetivo.
- Evite delegação em cascata. Somente um workstream owner pode criar um leaf agent, e apenas se isso já estiver autorizado no briefing.
- Espere resultados em uma única chamada longa e sintetize apenas o que altera a decisão ou implementação.

## Roteamento de modelos

Não altere o modelo do Root durante a tarefa. Para uma nova sessão, `gpt-6-sol` com esforço `high` é a recomendação de qualidade; reduza para `medium` quando economia for a prioridade. Use `max` apenas para ambiguidade central ou decisões de grande consequência.

Ao criar subagentes, use esta ordem:

1. `gpt-6-luna` com `medium` para leitura, busca de evidências, verificações e tarefas de folha bem delimitadas.
2. Eleve Luna para `high` quando houver raciocínio técnico relevante; use `xhigh` ou `max` somente se a dificuldade observada justificar o custo. Leaf agents não delegam.
3. Use `gpt-5.6-terra` com `high` para um workstream colaborativo que exige mais capacidade de programação ou coordenação. Use `max` apenas após Luna se mostrar insuficiente ou quando o risco justificar.
4. Use `gpt-6-sol` como sub-orquestrador somente para uma frente extensa que realmente precise coordenar outros agentes. `max` continua excepcional.

Se o modelo ou esforço escolhido não estiver disponível, use a alternativa mais barata disponível que satisfaça a tarefa; não aumente esforço automaticamente.

## Personas

Quando a delegação for aprovada, leia [personas.md](references/personas.md) e use apenas a persona necessária. Inclua no briefing que regras do repositório e limites de autorização continuam valendo.

## Encerramento

Interrompa ou não renove um agente quando ele sair do escopo, repetir trabalho ou não produzir evidência útil. O Root continua responsável por validar resultados, resolver conflitos e entregar uma resposta única.
