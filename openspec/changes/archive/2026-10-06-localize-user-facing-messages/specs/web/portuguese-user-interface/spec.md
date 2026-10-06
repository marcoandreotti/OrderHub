## Purpose

Estabelece Português do Brasil como idioma padrão para modalidades, validações e mensagens de erro apresentadas a usuários.

## ADDED Requirements

### Requirement: Saídas da aplicação são apresentadas em Português do Brasil
A interface SHALL apresentar rótulos de modalidades em Português do Brasil, e a API SHALL retornar títulos e mensagens de validação em Português do Brasil. Valores de enum e identificadores usados nos contratos MUST permanecer estáveis.

#### Scenario: Modalidades
- **WHEN** uma pessoa consulta ou seleciona modalidades na interface
- **THEN** ela vê “Mesa”, “Retirada” e “Entrega” conforme aplicável

#### Scenario: Validação de formulário
- **WHEN** uma entrada viola uma regra de validação
- **THEN** a mensagem retornada ao usuário está em Português do Brasil

#### Scenario: Erro da aplicação
- **WHEN** a API retorna um erro conhecido
- **THEN** o título e os detalhes apresentados ao usuário estão em Português do Brasil

#### Scenario: Indisponibilidade já comunicada na página
- **WHEN** uma recusa de pedido corresponde ao aviso de disponibilidade que a página já apresenta
- **THEN** a interface não repete a mesma informação em um banner de erro adicional
- **AND** outros erros continuam visíveis com sua referência de suporte
