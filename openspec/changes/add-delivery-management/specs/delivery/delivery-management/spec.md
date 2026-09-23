## Purpose

Define cobertura, taxa, estimativa e acompanhamento da entrega de cada unidade sem confiar em decisões calculadas pelo cliente.

## ADDED Requirements

### Requirement: Cobertura e taxa são determinadas no servidor
A unidade SHALL configurar regiões tenant-scoped e o sistema MUST determinar elegibilidade, taxa e estimativa a partir do endereço normalizado e da política ativa.

#### Scenario: Endereço fora da cobertura
- **WHEN** um endereço não atende nenhuma região ativa
- **THEN** a entrega é recusada sem revelar configurações internas

### Requirement: Entrega possui acompanhamento operacional
Uma entrega MAY registrar responsável e SHALL preservar despacho, saída e conclusão em sequência válida, com histórico de ator e instante.

#### Scenario: Conclusão antes da saída
- **WHEN** for solicitada conclusão sem estado anterior compatível
- **THEN** o sistema rejeita a transição e preserva o histórico
