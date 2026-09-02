## 1. Domínio de clientes

- [x] 1.1 Implementar o aggregate Customer com normalização tenant-scoped e verificar invariantes com testes de domínio
- [x] 1.2 Implementar endereços controlados pelo Customer e verificar com testes que somente um endereço permanece principal

## 2. Casos de uso

- [x] 2.1 Criar Commands, handlers e validators de manutenção de cliente e verificar os testes de Application
- [x] 2.2 Criar portas e Queries de pesquisa tenant-scoped e verificar que filtros nunca retornam outra unidade

## 3. Persistência

- [x] 3.1 Mapear escrita EF Core e constraints por Tenant/Establishment e verificar os testes de modelo
- [x] 3.2 Implementar projeções Dapper de clientes e endereços e verificar consultas de integração
- [x] 3.3 Criar migration de Customers e verificar upgrade, rollback e reapply no PostgreSQL

## 4. Verificação

- [x] 4.1 Testar concorrência e atomicidade da troca de endereço principal em integração
- [x] 4.2 Executar build, testes Domain/Application/Integration/Architecture e verificar zero erros e warnings relevantes
