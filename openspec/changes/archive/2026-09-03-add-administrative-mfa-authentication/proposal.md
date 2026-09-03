## Why

As áreas administrativa e operacional ainda confiam em um principal previamente fornecido pelo ambiente, sem um fluxo real de autenticação. Antes de expor interfaces de gestão, o OrderHub precisa validar senha e um segundo fator para reduzir o risco de acesso indevido a dados e operações de estabelecimentos.

## What Changes

- Adicionar autenticação administrativa por código público do Tenant, e-mail e senha, seguida de código de uso único enviado por e-mail.
- Emitir sessão autenticada somente após os dois fatores e transportar Tenant, usuário, papéis e associações de unidade a partir de dados confiáveis do servidor.
- Introduzir uma identidade de plataforma `PlatformSuperUser`, separada dos usuários tenant-scoped, capaz de atuar em todos os Tenants e de nomear outros superusuários.
- Criar idempotentemente o primeiro superusuário na publicação da API a partir de secrets de implantação e obrigar a troca da senha temporária no primeiro acesso.
- Adicionar expiração, limite de tentativas, reenvio controlado, uso único e invalidação dos desafios.
- Adicionar encerramento e renovação segura de sessão, bloqueio defensivo contra tentativas abusivas e respostas que não permitam enumerar usuários.
- Definir uma porta para entrega do código e um adapter substituível, sem acoplar o domínio a um provedor de e-mail.
- Preservar um mecanismo de autenticação de teste apenas no ambiente de testes, sem aceitá-lo como credencial de produção.

## Capabilities

### New Capabilities

- `identity/administrative-authentication`: login tenant-scoped ou de plataforma em duas etapas, desafio de código por e-mail, bootstrap do primeiro superusuário, troca obrigatória de senha, sessão, renovação, logout e proteções contra abuso.

### Modified Capabilities

- `identity/administrative-users`: credenciais, estado de acesso e associações passam a alimentar um principal autenticado criado exclusivamente pelo servidor, enquanto identidades globais são administradas separadamente.
- `administration/administration-api`: rotas administrativas passam a aceitar somente sessões reais com segundo fator concluído fora do ambiente de testes e reconhecem explicitamente o escopo global do superusuário.

## Impact

Afeta Domain e Application de Identity/Tenancy, persistência EF Core, migration, contratos e endpoints de autenticação, middleware/configuração de segurança, bootstrap de implantação, envio de e-mail e testes de domínio, aplicação, integração, API e arquitetura. A identidade global exige ADR antes da implementação. Secrets nunca são versionados; não há mensageria nem fornecedor externo diretamente nas camadas internas.
