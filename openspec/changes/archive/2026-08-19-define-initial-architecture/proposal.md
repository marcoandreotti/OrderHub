## Why

O OrderHub possui uma visão funcional ampla e regras arquiteturais obrigatórias, mas ainda precisa de uma definição inicial coerente que delimite a primeira entrega executável e sirva de contrato para as fases seguintes. Esta mudança consolida a fundação técnica, os limites do monólito modular e o roadmap incremental antes que qualquer código de produto seja criado.

## What Changes

- Define a arquitetura de referência combinando Modular Monolith, DDD, CQRS, Arquitetura Hexagonal e Clean Architecture sem introduzir microservices ou mensageria.
- Define a estrutura inicial da solution .NET, do frontend Vue/Quasar, dos testes, da persistência e dos artefatos Docker.
- Define os fluxos separados de escrita com EF Core e leitura com Dapper, usando dispatchers próprios e validação com FluentValidation.
- Define isolamento Multi-Tenant, segurança, tratamento de erros, observabilidade, auditoria e idempotência como requisitos transversais.
- Delimita os bounded contexts iniciais e diferencia capacidades da primeira versão de integrações futuras.
- Estabelece um roadmap incremental no qual cada fase deve permanecer executável e testável.

## Capabilities

### New Capabilities

- `architecture/solution-foundation`: Define a fundação arquitetural, estrutura da solução, dependências permitidas, fluxos CQRS, persistência, multi-tenancy, frontend, execução em Docker e roadmap incremental do OrderHub.

### Modified Capabilities

Nenhuma.

## Impact

- Cria o contrato arquitetural que orientará futuros projetos em `src/`, `web/`, `tests/` e `docker/`.
- Não altera APIs nem código executável nesta mudança de planejamento.
- Restringe escolhas futuras a .NET 10, PostgreSQL, EF Core para escrita, Dapper para leitura, Vue.js, Quasar, TypeScript e Docker.
- Proíbe MediatR, AutoMapper, acesso direto à persistência por Controllers e dependências de infraestrutura no Domain.
