## ADDED Requirements

### Requirement: Catálogo pode ser administrado por unidade autorizada
O sistema SHALL permitir que atores autenticados com capacidade de gerenciamento criem, consultem e alterem categorias, produtos, imagens, variações, adicionais, grupos e seus vínculos somente na unidade operacional autorizada. O sistema MUST derivar o Tenant do contexto autenticado e MUST validar no servidor a associação ativa do usuário com a unidade, sem confiar em `TenantId` recebido do cliente.

#### Scenario: Gerente administra catálogo da unidade associada
- **WHEN** um gerente autenticado envia dados válidos para criar ou alterar um item na unidade à qual possui associação ativa
- **THEN** o sistema SHALL aplicar a alteração dentro do Tenant e da unidade resolvidos e SHALL retornar um contrato externo que não exponha entidades de domínio

#### Scenario: Tentativa administrativa cruzada
- **WHEN** um ator tenta consultar ou alterar catálogo de outro Tenant ou de unidade sem associação ativa
- **THEN** o sistema MUST negar a operação sem revelar a existência dos dados e MUST preservar o estado

#### Scenario: Entrada estruturalmente inválida
- **WHEN** uma operação administrativa contém campos obrigatórios ausentes, formatos inválidos ou referências incompatíveis com a mesma unidade
- **THEN** o sistema MUST rejeitar a operação com erro padronizado e MUST NOT persistir alteração parcial

### Requirement: Administração consulta a composição completa do catálogo
O sistema SHALL fornecer uma projeção administrativa ordenada do catálogo da unidade, incluindo itens ativos e inativos, hierarquia de categorias, produtos, imagens, variações, grupos e adicionais necessários para manutenção.

#### Scenario: Consulta administrativa da unidade
- **WHEN** um ator autorizado consulta o catálogo administrativo de sua unidade
- **THEN** o sistema SHALL retornar somente dados do Tenant e da unidade resolvidos, preservando hierarquia, ordenação, estados de ativação e vínculos cadastrados

### Requirement: Cardápio público apresenta somente ofertas vendáveis
O sistema SHALL permitir consultar o cardápio público por slug ativo da unidade e SHALL retornar uma projeção hierárquica ordenada contendo somente categorias, produtos, variações, grupos e adicionais ativos. A resolução pública MUST determinar Tenant e unidade no servidor e MUST NOT conceder privilégios administrativos.

#### Scenario: Visitante consulta cardápio ativo
- **WHEN** um visitante consulta o slug de uma unidade e Tenant ativos
- **THEN** o sistema SHALL retornar categorias e ofertas vendáveis dessa unidade com preços, imagens, variações e adicionais ativos em sua ordem cadastrada

#### Scenario: Componentes inativos no catálogo
- **WHEN** a unidade possui categorias, produtos, variações, grupos ou adicionais inativos
- **THEN** esses componentes MUST NOT aparecer como novas opções no cardápio público, embora permaneçam disponíveis na projeção administrativa e em históricos que os referenciem

#### Scenario: Slug público indisponível
- **WHEN** o slug não existe ou pertence a Tenant ou unidade inativos
- **THEN** o sistema MUST NOT revelar dados de catálogo nem informações internas sobre a unidade

### Requirement: Alterações relacionais do catálogo são consistentes
Operações que alterem um aggregate do catálogo e suas coleções ou vínculos SHALL ser persistidas atomicamente e o armazenamento MUST rejeitar códigos duplicados na mesma unidade, referências cruzadas e relações estruturalmente inválidas.

#### Scenario: Falha ao persistir coleção do produto
- **WHEN** a persistência de uma imagem, variação ou vínculo de grupo falha durante alteração do produto
- **THEN** nenhuma parte da alteração desse aggregate SHALL permanecer persistida

#### Scenario: Código de produto repetido na unidade
- **WHEN** uma operação tenta persistir código de produto já utilizado na mesma unidade
- **THEN** o sistema MUST rejeitar a alteração por conflito sem modificar o produto existente

#### Scenario: Mesmo código em unidades diferentes
- **WHEN** unidades distintas utilizam o mesmo código de produto
- **THEN** o sistema SHALL permitir os cadastros mantendo isolamento entre seus catálogos
