## Why

Botões de ações repetidas em listas e grids variam entre telas e competem com as informações dos registros. Ícones quadrados tornam esses controles mais compactos, enquanto ícone e rótulo nas confirmações deixam a ação principal mais fácil de reconhecer.

## What Changes

- Padronizar ações contextuais de linhas, cards e itens repetidos como botões quadrados com ícone, nome acessível e dica de ação.
- Padronizar confirmações de operações como botões com ícone à esquerda e rótulo visível, preservando estado de carregamento, desabilitado e cor semântica.
- Aplicar o padrão nas superfícies Public, Administration, Operations/KDS e Platform sem alterar regras ou transições existentes.
- Atualizar a documentação do Design System com critérios para ícones, rótulos, cores, touch e teclado.

## Capabilities

### New Capabilities

- `web/action-controls`: padrões visuais e de acessibilidade para ações de coleção e confirmações em todas as superfícies da aplicação.

### Modified Capabilities

Nenhuma. O padrão muda a apresentação e acessibilidade dos controles, sem alterar as capacidades ou regras funcionais existentes.

## Impact

- Frontend Vue/Quasar: cards, grids, listas, tabelas e confirmações de Administration, Operations/KDS, Public e Platform.
- `docs/web-design-system.md` e nova spec OpenSpec para controles de ação.
- Nenhuma alteração de API, domínio, autorização, fluxo de dados ou dependência.
