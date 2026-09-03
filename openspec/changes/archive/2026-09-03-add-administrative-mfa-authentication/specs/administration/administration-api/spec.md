## ADDED Requirements

### Requirement: Produção aceita somente autenticação administrativa real
O sistema MUST rejeitar em ambientes não destinados a testes qualquer principal administrativo que não tenha sido emitido após a conclusão dos fatores exigidos.

#### Scenario: Cabeçalhos de identidade forjados
- **WHEN** um cliente envia diretamente identificadores, papéis ou associações por cabeçalhos em produção
- **THEN** a API ignora esses valores e retorna acesso não autorizado

#### Scenario: Autenticação de testes
- **WHEN** testes automatizados executam no ambiente de testes explicitamente configurado
- **THEN** o mecanismo substituto pode criar principals sem ficar disponível nos demais ambientes

### Requirement: Escopo global é explícito e auditável
A API MUST reconhecer acesso global somente por uma identidade de plataforma persistida e por sessão plenamente autenticada, sem permitir que TenantId, código de Tenant, papel ou cabeçalho enviado pelo cliente conceda esse escopo.

#### Scenario: Papel global forjado
- **WHEN** uma requisição tenta declarar diretamente papel ou escopo de superusuário
- **THEN** a API ignora a declaração e rejeita o acesso

#### Scenario: Superusuário seleciona Tenant
- **WHEN** uma identidade global válida executa operação permitida em Tenant específico
- **THEN** a API resolve o alvo explicitamente, aplica isolamento aos dados acessados e registra o ator global
