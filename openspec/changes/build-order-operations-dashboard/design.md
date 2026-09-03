## Context

A API administrativa já oferece pesquisa, detalhe e transições de pedido protegidas por políticas específicas. O frontend possui layout operacional vazio. A primeira versão deve funcionar sem infraestrutura de push ou mensageria.

## Goals / Non-Goals

**Goals:**

- Visão rápida e confiável do trabalho corrente por papel e unidade.
- Atualização periódica previsível sem sobrecarga ou concorrência local.
- Tratamento explícito de estado obsoleto e falhas de conectividade.

**Non-Goals:**

- SignalR, WebSockets, filas, impressão automática, roteirização ou aplicativo de entregador.
- Alterar a máquina de estados do domínio.

## Decisions

1. **Uma visão operacional com modos por capacidade.** Reutilizar componentes e filtrar ações por claims, evitando aplicações separadas para atendimento/cozinha/entrega.
2. **Store normalizada por pedido.** Separar coleção, detalhes e metadados de sincronização para atualizar itens sem reconstruir toda a tela.
3. **Polling com exclusão mútua e visibilidade.** Um coordenador inicia nova consulta somente após a anterior, pausa em página oculta e aplica backoff limitado após falhas.
4. **API permanece autoridade de transição.** A interface pode antecipar botões permitidos, mas não altera estado otimisticamente. Em conflito, recarrega detalhe e explica o novo estado.
5. **Destaques locais não mudam domínio.** “Novo” e “atrasado” são apresentações derivadas do snapshot e do horário, sem criar status ou eventos de negócio.
6. **Filtros refletidos na URL.** Estado, atendimento e busca podem ser restaurados/compartilhados sem persistir dados sensíveis.

## Risks / Trade-offs

- [Polling aumenta carga] → Consultas paginadas/filtradas, intervalo configurado, pausa por visibilidade e medição antes de tempo real.
- [Dois operadores agem juntos] → Sem atualização otimista; conflito recarrega a fonte autoritativa.
- [Tela cheia demais] → Visão resumida por colunas/listas e detalhe progressivo.
- [Relógios divergentes] → Tempos absolutos vêm da API; duração visual é aproximada e não decide regras.

## Migration Plan

1. Criar shell e rota protegida dependentes da autenticação real.
2. Implementar consulta/detalhe antes das ações mutáveis.
3. Adicionar transições por papel e cobertura de conflitos.
4. Substituir a página operacional de fundação; rollback retorna ao placeholder sem alterar backend.
