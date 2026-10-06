## Why

As telas e mensagens de validação do OrderHub devem oferecer uma experiência consistente em Português do Brasil. Hoje algumas modalidades e erros ainda aparecem em inglês.

## What Changes

- Definir Português do Brasil como idioma padrão das mensagens de validação geradas pelo FluentValidation.
- Exibir modalidades e demais rótulos de atendimento em Português do Brasil, preservando os valores técnicos dos contratos.
- Apresentar títulos e mensagens de erro HTTP em Português do Brasil.

## Capabilities

### New Capabilities

- `web/portuguese-user-interface`: idioma das saídas da aplicação visíveis a usuários.

### Modified Capabilities

Nenhuma.

## Impact

Frontend Vue/Quasar e respostas HTTP da API. Enums serializados, rotas, identificadores e conteúdo livre cadastrado por usuários permanecem inalterados.
