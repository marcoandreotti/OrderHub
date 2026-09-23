## Why

Os fluxos administrativos existentes não permitem concluir cadastros pela aplicação web porque ações de criação permanecem desabilitadas ou não chegam corretamente à API. A correção é prioritária porque impede o uso das capacidades já entregues.

## What Changes

- Revisar os fluxos de criação e edição já previstos na área administrativa.
- Corrigir critérios de habilitação, validação, envio e feedback dos formulários.
- Validar contratos entre web e API e cobrir os fluxos afetados com testes.
- Não introduzir novos tipos de cadastro nem alterar regras de domínio.

## Capabilities

### New Capabilities

Nenhuma.

### Modified Capabilities

Nenhuma. A change restaura comportamentos já exigidos por `administration/administration-web` e pelas capabilities de cadastro existentes.

## Impact

Aplicação Vue/Quasar administrativa, clients HTTP, estado dos formulários e testes de integração web/API. Não há mudança planejada no modelo de domínio ou no contrato público além de correções de incompatibilidades acidentais.
