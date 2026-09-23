## ADDED Requirements

### Requirement: Interface comunica indisponibilidade acionável
A aplicação SHALL distinguir unidade fechada, modalidade pausada e oferta indisponível, informar próxima abertura quando conhecida e impedir envio sabidamente inválido.

#### Scenario: Unidade fechada
- **WHEN** o visitante acessa o cardápio fora da disponibilidade
- **THEN** a navegação pode permanecer visível, mas a confirmação imediata é bloqueada com motivo claro
