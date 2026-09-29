## ADDED Requirements

### Requirement: Cliente escolhe somente horários retornados pelo servidor
A aplicação SHALL apresentar opção imediata ou agendada conforme modalidade, unidade e slots vigentes e SHALL comunicar fuso e prazo esperado.

#### Scenario: Slot expira no checkout
- **WHEN** a confirmação rejeita o slot selecionado
- **THEN** a aplicação preserva o carrinho e solicita nova escolha
