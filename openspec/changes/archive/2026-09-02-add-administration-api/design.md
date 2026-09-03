## Context

O catálogo já possui endpoints administrativos e políticas iniciais. Esta change uniformiza a borda autenticada para os demais módulos sem mover regras para a API.

## Goals / Non-Goals

**Goals:**

- Aplicar autorização por capacidade e escopo operacional em todas as rotas.
- Padronizar paginação, contratos, validação e erros.
- Separar consultas Dapper de Commands EF Core.

**Non-Goals:**

- Criar telas administrativas, relatórios analíticos avançados ou tempo real.
- Alterar as regras de domínio entregues nas changes anteriores.

## Decisions

### Grupos de endpoints por estabelecimento e módulo

As rotas usarão o estabelecimento na URL e o validarão pelo resolvedor de escopo autenticado. Um estabelecimento apenas em claim dificultaria seleção explícita e múltiplas unidades; confiar na URL sem validação permitiria acesso cruzado.

### Políticas por capacidade, não verificações manuais

Os endpoints declararão políticas centrais e os handlers ainda validarão o escopo de dados. Condicionais de papel espalhadas seriam difíceis de auditar.

### Contrato comum de paginação

Listagens usarão página, tamanho limitado, total e ordenação estável, com filtros específicos por recurso. Retornar coleções ilimitadas foi rejeitado por previsibilidade operacional.

### Reutilização da infraestrutura de dispatch

Controllers/endpoints somente mapearão contratos e executarão Commands/Queries. Não será criada uma camada genérica de CRUD, pois os módulos possuem comportamentos distintos.

## Risks / Trade-offs

- [Políticas incorretas ampliam privilégios] → Cobrir matriz papel/capacidade e isolamento com testes de integração.
- [Consultas operacionais crescem] → Usar projeções Dapper dedicadas e índices guiados pelos filtros especificados.

## Migration Plan

Adicionar contratos e endpoints por módulo, validar OpenAPI e matriz de autorização e liberar somente após todas as changes dependentes estarem aplicadas.
