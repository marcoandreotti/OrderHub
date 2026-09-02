## 1. Registrar e preparar a fundação

- [x] 1.1 Criar um ADR para a adoção do monólito modular, estrutura de projetos, limites de módulos e regras de dependência definidas no design.
- [x] 1.2 Criar `OrderHub.sln` e os projetos Domain, Application, Infrastructure, Contracts e API em .NET 10, habilitando nullable reference types e warnings apropriados.
- [x] 1.3 Configurar referências entre projetos conforme a direção de dependências permitida e adicionar os quatro projetos de testes.
- [x] 1.4 Documentar no README os pré-requisitos e os comandos de build, teste e execução local.

## 2. Implementar a espinha dorsal CQRS

- [x] 2.1 Criar os contratos `ICommand`, `ICommand<TResult>`, `IQuery<TResult>` e seus handlers com suporte a `CancellationToken`.
- [x] 2.2 Implementar CommandDispatcher e QueryDispatcher usando o container nativo de DI e garantindo resolução inequívoca de handlers.
- [x] 2.3 Integrar FluentValidation aos dispatchers para impedir execução do handler quando a entrada for inválida.
- [x] 2.4 Adicionar testes unitários dos dispatchers para comandos e queries com e sem resultado, validação inválida e propagação de cancelamento.

## 3. Criar a base da API e preocupações transversais

- [x] 3.1 Configurar a API como composition root e expor apenas um endpoint de health check nesta fase.
- [x] 3.2 Criar a hierarquia mínima de exceções padronizadas e o middleware global que as converte em ProblemDetails.
- [x] 3.3 Implementar correlation ID e logging estruturado com propagação por requisição, sem configurar backend externo de telemetria.
- [x] 3.4 Criar a abstração de contexto de Tenant e uma implementação HTTP inicial que falhe de modo seguro quando o Tenant obrigatório não puder ser resolvido.
- [x] 3.5 Adicionar testes de integração para health check, ProblemDetails, correlation ID e ausência de Tenant em uma rota de teste protegida.

## 4. Preparar persistência e isolamento

- [x] 4.1 Configurar PostgreSQL e EF Core no projeto Infrastructure para escrita, sem expor DbContext à API ou ao Domain.
- [x] 4.2 Configurar a fábrica de conexões Dapper para leitura, mantendo SQL e tipos do provedor exclusivamente em Infrastructure.
- [x] 4.3 Criar as convenções iniciais de migrations, schemas por módulo e chaves tenant-scoped, sem criar tabelas de funcionalidades futuras.
- [x] 4.4 Adicionar testes de integração com PostgreSQL que comprovem conectividade dos adapters e isolamento entre dois contextos de Tenant.

## 5. Criar o shell web e o ambiente Docker

- [x] 5.1 Inicializar `web/OrderHub.Web` com Vue, Quasar, TypeScript e a organização modular definida no design.
- [x] 5.2 Criar layouts vazios para as áreas pública, operacional e administrativa, além do cliente HTTP central com suporte a ProblemDetails e correlation ID.
- [x] 5.3 Criar a infraestrutura central de tokens de tema e um tema padrão, sem implementar ainda personalização persistida por Tenant.
- [x] 5.4 Criar Dockerfiles da API e do frontend e um `docker-compose.yml` com PostgreSQL, rede interna, volume persistente e health checks.
- [x] 5.5 Criar `.env.example`, garantir que segredos reais permaneçam ignorados e documentar a configuração por ambiente.

## 6. Proteger e validar a arquitetura

- [x] 6.1 Adicionar testes arquiteturais que impeçam Domain de depender das demais camadas, Application de depender de API/Infrastructure e Controllers de acessar persistência diretamente.
- [x] 6.2 Adicionar verificações que impeçam referências a MediatR e AutoMapper nos projetos e dependências restauradas.
- [x] 6.3 Executar build da solution, testes unitários, testes de integração e testes arquiteturais e eliminar erros e warnings relevantes.
- [x] 6.4 Inicializar o ambiente Docker completo e verificar health checks e comunicação Browser/Web/API/PostgreSQL.
- [x] 6.5 Atualizar a documentação da Fase 0 com as decisões efetivamente implementadas e registrar qualquer divergência arquitetural em novo ADR antes de concluir.
