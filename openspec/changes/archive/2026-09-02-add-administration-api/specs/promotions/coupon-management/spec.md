## ADDED Requirements

### Requirement: Gestão administra cupons da unidade autorizada
A API SHALL permitir criar, consultar, alterar, ativar e desativar cupons somente a atores com capacidade de gestão no estabelecimento.

#### Scenario: Código duplicado
- **WHEN** a gestão cadastrar código normalizado já existente na unidade
- **THEN** a API SHALL retornar conflito padronizado sem alterar o cupom existente
