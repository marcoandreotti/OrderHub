## Context

A API usa hoje um handler que aceita um principal já existente. O cadastro administrativo possui e-mail normalizado, hash de senha, papéis e associações, e permite o mesmo e-mail em Tenants distintos. Falta um código público estável para resolver o Tenant no login e não existe identidade de plataforma. A mudança atravessa API, Application, Identity, Tenancy, persistência, bootstrap e um canal externo de e-mail.

## Goals / Non-Goals

**Goals:**

- Autenticação em duas etapas resistente a enumeração, repetição e abuso básico.
- Sessões revogáveis e tenant-scoped sem confiar em dados enviados pelo cliente.
- Canal de entrega substituível e testável.
- Separar de forma explícita autorização global e autorização tenant-scoped.
- Criar o primeiro administrador da plataforma sem credencial hardcoded ou operação manual no banco.

**Non-Goals:**

- Login social, SMS, aplicativo autenticador, SSO empresarial ou recuperação de senha nesta mudança.
- Criar um servidor OAuth/OIDC genérico ou armazenar sessão no Domain.
- Tratar o código do Tenant ou da plataforma como segredo ou segundo fator.

## Decisions

1. **ADR antes do código.** A identidade global atravessa a fronteira tenant-scoped existente; um ADR documentará modelo separado, autorização, auditoria e consequências antes da implementação.
2. **Código público resolve o contexto de login.** Cada Tenant recebe código normalizado, imutável por padrão e único globalmente. Superusuários usam um código de plataforma configurável. Esses códigos identificam o contexto, não são segredo, e respostas continuam não enumeráveis.
3. **Identidade global separada.** `PlatformUser`/equivalente possui e-mail, credencial, estado e flag de troca obrigatória, sem implementar `ITenantScopedEntity`. Reutiliza value object de e-mail e serviço de hashing, mas não reutiliza papéis ou associações tenant-scoped.
4. **Código por e-mail como segundo fator inicial.** O e-mail reduz infraestrutura inicial. A porta de Application recebe a mensagem sem conhecer fornecedor; SMTP/API transacional fica em Infrastructure. SMS e TOTP foram adiados, mas o desafio não codifica o fornecedor.
5. **Fluxo em duas transações.** A primeira resolve contexto, valida credenciais e cria um desafio opaco; a segunda consome o desafio e emite a sessão. O identificador do desafio não revela usuário ou Tenant.
6. **Códigos e tokens protegidos.** Persistir somente hashes dos códigos e tokens de renovação. Usar comparação em tempo constante quando aplicável, validade curta, tentativas limitadas e rotação de refresh token.
7. **Bootstrap idempotente no startup.** Um initializer scoped verifica a existência de identidade global e cria exatamente uma a partir de options vinculadas a environment variables/user secrets. Configuração ausente na primeira publicação impede startup; depois do bootstrap, secrets não alteram o registro.
8. **Sessão restrita para troca obrigatória.** Após MFA com senha temporária, a sessão recebe claim/estado `password_change_required` e policy que permite somente contexto, troca e logout. A troca valida a senha atual, grava novo hash, revoga a família e exige novo login.
9. **Access token curto e refresh token rotativo.** O access token carrega identificador de sessão, tipo de identidade e claims mínimas. O servidor reconsulta o estado na renovação e mantém família de refresh tokens para revogação/reuso.
10. **Cookies HttpOnly para a aplicação web.** Preferir cookie `Secure`, `HttpOnly` e `SameSite` compatível com a topologia, com proteção CSRF nas operações mutáveis. Armazenamento de bearer token no navegador foi rejeitado pelo maior impacto de XSS.
11. **Proteção em camadas.** Limitar por origem, contexto e identidade normalizada, usar respostas uniformes e registrar eventos de segurança sem segredos. Bloqueio permanente por conta foi rejeitado por permitir abuso de negação de serviço.
12. **Handler de teste isolado.** O esquema atual permanece disponível apenas via configuração explícita no host de testes e não é registrado em produção.

## Risks / Trade-offs

- [Comprometimento do e-mail compromete os dois fatores] → Documentar que este é um segundo passo de verificação inicial e preservar evolução para TOTP/WebAuthn.
- [Entrega atrasada ou indisponível] → Expiração clara, reenvio controlado, observabilidade e adapter substituível.
- [Revogação imediata versus token curto] → Validar sessão em operações/renovação conforme risco e manter access token com vida curta.
- [Cookie entre origens] → Manter web e API sob topologia compatível e configurar CORS/CSRF explicitamente por ambiente.
- [Superusuário amplia o impacto de comprometimento] → MFA obrigatório, sessão curta, auditoria, gestão exclusiva por pares e proteção do último usuário ativo.
- [Secrets de bootstrap permanecem configurados] → Ignorá-los após criação, alertar operacionalmente e documentar sua remoção/rotação após o primeiro acesso.
- [Código público confundido com segredo] → Nomear e documentar como identificador, mantendo senha e MFA como fatores reais.

## Migration Plan

1. Registrar o ADR da identidade global e do bootstrap.
2. Adicionar código público aos Tenants e tabelas de identidade global, desafios e sessões sem remover o esquema atual.
3. Configurar secrets de bootstrap e publicar a API para criar a primeira identidade global de modo idempotente.
4. Concluir MFA e troca obrigatória da senha temporária; remover/rotacionar os secrets de bootstrap.
5. Implantar os demais endpoints e entrega de e-mail com o esquema real inicialmente controlado por configuração segura.
6. Validar fluxos tenant e plataforma em ambiente não produtivo e ativá-los para as rotas administrativas.
7. Remover o registro do handler legado fora do ambiente de testes.
8. Em rollback, desabilitar emissão e revogar sessões; nunca recriar/reescrever o superusuário a partir dos secrets antigos.
