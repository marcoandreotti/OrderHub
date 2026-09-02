# OrderHub

Fundação do monólito modular SaaS Multi-Tenant para gestão de pedidos de estabelecimentos de alimentação.

## Pré-requisitos

- .NET SDK 10
- Node.js 22.22 ou superior
- Docker 29 com Docker Compose

## Backend

```powershell
dotnet restore OrderHub.sln
dotnet build OrderHub.sln --no-restore
dotnet test OrderHub.sln --no-build
dotnet run --project src/OrderHub.Api
```

A API expõe liveness em `GET /health` e readiness do PostgreSQL em `GET /health/ready`.

## Frontend

```powershell
cd web/OrderHub.Web
npm install
npm run dev
```

## Docker

Copie `.env.example` para `.env`, ajuste apenas valores locais e execute:

```powershell
docker compose up --build
```

Serviços locais:

- Web: `http://localhost:9000`
- API: `http://localhost:8080`
- PostgreSQL: `localhost:5432`

## Arquitetura

- `OrderHub.Domain`: modelo e regras de domínio, sem dependências externas.
- `OrderHub.Application`: Commands, Queries, handlers e portas.
- `OrderHub.Infrastructure`: EF Core, Dapper e adapters técnicos.
- `OrderHub.Contracts`: contratos externos explícitos.
- `OrderHub.Api`: composition root e borda HTTP.

EF Core atende escrita e migrations; Dapper atende leitura. MediatR e AutoMapper são proibidos. Consulte `openspec/architecture.md`, `openspec/conventions.md` e os ADRs em `openspec/decisions/`.

## Configuração

Configurações não sensíveis ficam em `appsettings*.json`. Valores por ambiente usam variáveis com separador `__`, por exemplo `ConnectionStrings__OrderHub`. Senhas reais, tokens e arquivos `.env` não devem ser versionados.
