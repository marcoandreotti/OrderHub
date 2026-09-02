## Purpose

Define a borda HTTP anônima que permite ao visitante compor, confirmar, acompanhar e cancelar pedidos sem expor identificadores internos ou confiar em cálculos do cliente.

## ADDED Requirements

### Requirement: API pública resolve o estabelecimento no servidor
Toda operação pública SHALL resolver Tenant e estabelecimento por slug ativo ou token público opaco e MUST NOT aceitar TenantId como autoridade enviado pelo cliente.

#### Scenario: Slug de unidade inativa
- **WHEN** uma requisição usar slug de Tenant ou estabelecimento inativo
- **THEN** a API MUST NOT revelar dados nem permitir criação de pedido

### Requirement: Criação pública de pedido é idempotente
A confirmação pública MUST exigir chave idempotente, recalcular composição e totais no servidor e retornar referência pública opaca.

#### Scenario: Cliente repete confirmação
- **WHEN** a mesma confirmação for reenviada com a mesma chave e conteúdo
- **THEN** a API SHALL retornar o pedido originalmente criado sem duplicação

### Requirement: Acompanhamento público limita dados expostos
A consulta por referência pública SHALL retornar somente informações necessárias ao cliente, incluindo composição, totais e histórico apresentável, sem dados administrativos ou internos.

#### Scenario: Referência inválida
- **WHEN** uma referência pública inexistente ou alterada for consultada
- **THEN** a API MUST NOT revelar se pedidos próximos existem

### Requirement: Erros públicos seguem ProblemDetails
Falhas de validação, conflito, indisponibilidade e regra de domínio SHALL produzir ProblemDetails consistente sem stack trace ou detalhes de infraestrutura.

#### Scenario: Composição inválida
- **WHEN** o visitante confirmar seleção que viola limites de adicionais
- **THEN** a API SHALL rejeitar a operação com resposta padronizada e sem persistência parcial
