## 1. Fundação operacional

- [ ] 1.1 Confirmar que a autenticação administrativa real está implementada e inventariar endpoints/políticas de pedidos; verificar dependências e lacunas antes do frontend.
- [ ] 1.2 Estruturar rotas e shell operacional protegidos, reutilizando sessão e seleção de unidade; verificar acesso por papel e limpeza ao trocar unidade.
- [ ] 1.3 Implementar store normalizada de pedidos, detalhes e sincronização; verificar atualização incremental sem duplicação.

## 2. Consulta e atualização

- [ ] 2.1 Criar visão por estado com filtros e detalhes necessários à operação; verificar pedido, itens, observações, atendimento, pagamento e histórico.
- [ ] 2.2 Implementar coordenador de polling sem sobreposição, com pausa por visibilidade, atualização manual e backoff; verificar temporizadores determinísticos em testes.
- [ ] 2.3 Exibir última sincronização, estado possivelmente desatualizado e recuperação de falhas; verificar indisponibilidade temporária da API.

## 3. Ações operacionais

- [ ] 3.1 Mapear capacidades do usuário para ações visuais de atendimento, cozinha e entrega; verificar matriz de papéis sem considerar a UI como autorização.
- [ ] 3.2 Integrar transições sem atualização otimista e com confirmação quando necessária; verificar sucesso, proibição e regra de domínio.
- [ ] 3.3 Tratar conflito concorrente recarregando o pedido e explicando o estado vigente; verificar dois operadores atuando sobre o mesmo pedido.

## 4. Qualidade

- [ ] 4.1 Adicionar destaques locais de pedidos novos/atrasados sem criar estado de domínio; verificar cálculo e apresentação por texto/ícone.
- [ ] 4.2 Cobrir teclado, foco, desktop/tablet e percursos por papel com testes de componentes e integração.
- [ ] 4.3 Executar typecheck, build web e testes backend relevantes; verificar zero erros/warnings relevantes e revisar contra specs/design sem mensageria ou tempo real.
