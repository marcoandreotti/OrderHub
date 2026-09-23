## Why

Históricos de pedido não cobrem alterações administrativas em usuários, catálogo e configuração. A operação precisa identificar quem realizou ações sensíveis e qual foi o resultado.

## What Changes

- Registrar eventos de auditoria para ações administrativas relevantes.
- Capturar ator, unidade, operação, alvo, instante, resultado e correlação.
- Proteger registros contra alteração e oferecer consulta autorizada e paginada.
- Aplicar minimização e mascaramento de dados sensíveis.

## Capabilities

### New Capabilities

- `security/administrative-audit-trail`: registro e consulta tenant-scoped de ações administrativas.

### Modified Capabilities

- `administration/administration-api`: auditar operações administrativas selecionadas sem espalhar responsabilidade pelos Controllers.

## Impact

Pipeline de aplicação, persistência, endpoints administrativos e políticas de retenção. Não substitui históricos próprios dos aggregates.
