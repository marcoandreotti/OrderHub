## 1. Convenções administrativas

- [x] 1.1 Criar contratos comuns de paginação, filtros e respostas e verificar limites e ordenação em testes
- [x] 1.2 Completar políticas por capacidade e verificar a matriz de papéis em testes de autorização

## 2. Consultas e Commands

- [x] 2.1 Implementar consultas paginadas Dapper para clientes e pedidos e verificar isolamento e filtros em integração
- [x] 2.2 Implementar consultas e manutenção de cupons, formas e pagamentos e verificar o escopo autorizado
- [x] 2.3 Implementar Commands de transição operacional com registro do ator e verificar histórico nos testes de Application

## 3. Endpoints

- [x] 3.1 Mapear endpoints administrativos de clientes e pedidos via dispatchers e verificar contratos no OpenAPI
- [x] 3.2 Mapear endpoints administrativos de cupons e pagamentos via dispatchers e verificar políticas de gestão
- [x] 3.3 Alinhar endpoints existentes de catálogo às convenções comuns e verificar compatibilidade dos testes atuais

## 4. Segurança e conclusão

- [x] 4.1 Testar cada papel contra operações permitidas e negadas, incluindo acesso cruzado entre unidades e Tenants
- [x] 4.2 Executar build e todas as suítes Domain/Application/Integration/Architecture/API e verificar zero erros e warnings relevantes
