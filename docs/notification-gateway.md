# Notification Gateway

O módulo Communications mantém templates, consentimentos, solicitações e tentativas por Tenant e unidade. `Email` e `WhatsApp` são os canais habilitados nesta change. Novos canais implementam `INotificationChannelSender`; Commands, templates, histórico e outbox usam o enum/canal neutro sem referências aos SDKs dos fornecedores.

As credenciais SMTP e Meta desta primeira entrega são de sandbox e pertencem ao processo da API, compartilhadas pelos Tenants. Não habilitar envio produtivo antes de uma extensão para contas e identidades de remetente independentes por Tenant, com credenciais resolvidas em secret manager.

## Fluxo

1. Um administrador com permissão `management` cria ou atualiza template e registra consentimento comprovado.
2. A solicitação de notificação usa uma chave idempotente, destino e parâmetros. A API grava a solicitação e a mensagem de outbox na mesma transação.
3. O worker verifica o consentimento novamente antes da chamada externa e registra uma tentativa.
4. Falhas definitivas não são repetidas. Falhas temporárias recebem uma nova mensagem outbox com backoff. Timeout ou falha de transporte com resultado ambíguo fica como `Uncertain`, sem retry automático para reduzir duplicidades.
5. `AcceptedByProvider` significa aceito pelo servidor SMTP ou pela Meta. Não significa entregue ao destinatário. Confirmação final por webhook não faz parte desta change.

O histórico de tentativas fica disponível em `GET /api/admin/establishments/{establishmentId}/communications/notifications/{notificationId}/attempts`. Dados de destino são PII e somente a API autenticada de administração os expõe.

## SMTP sandbox local

O `docker-compose.yml` inicia Mailpit no perfil de desenvolvimento:

- SMTP: `localhost:1025` no host, `mailpit:1025` dentro da rede Docker;
- interface web: `http://localhost:8025`.

Ao executar a API diretamente no host, `appsettings.Development.json` já aponta para `localhost:1025`. O container API usa `mailpit:1025`. Valores SMTP podem ser substituídos por variáveis `NOTIFICATION_SMTP_*`; em produção, fornecer usuário, senha e TLS usando o secret manager da implantação.

## WhatsApp de teste pela Meta

Configure no ambiente da API:

- `META_WHATSAPP_GRAPH_API_VERSION`: versão Graph atualmente suportada pela aplicação Meta;
- `META_WHATSAPP_PHONE_NUMBER_ID`: ID do número de teste habilitado para a Cloud API;
- `META_WHATSAPP_ACCESS_TOKEN`: token de acesso com permissão de envio;
- `META_WHATSAPP_TIMEOUT_SECONDS`: timeout HTTP, opcional (padrão 20).

Use variáveis de ambiente em desenvolvimento e secret manager em ambientes compartilhados ou produção. Não armazene o token em template, banco, imagem de container, logs ou frontend. O container lê esses valores pelas variáveis `META_WHATSAPP_*`.

O template administrativo de WhatsApp deve conter o nome de um template aprovado no WhatsApp Manager e idioma idêntico ao aprovado. Parâmetros numéricos JSON, como `{"1":"Ana","2":"42"}`, são enviados na ordem numérica como variáveis do componente body. Mensagens são enviadas como template aprovado; texto livre não é enviado por este adapter.

Consulte a documentação oficial da [WhatsApp Cloud API](https://developers.facebook.com/docs/whatsapp/cloud-api/overview) e o [guia oficial de chamadas da API pela Meta](https://www.postman.com/meta/whatsapp-business-platform/documentation/wlk6lh4/whatsapp-cloud-api). O token e a versão variam conforme o app e a configuração da Meta.

## Templates e consentimento

- `purpose` é uma chave estável em minúsculas, por exemplo `order.update`.
- E-mail usa `subject` e `body` em texto simples. Placeholders como `{{name}}` são substituídos pelo JSON de parâmetros.
- WhatsApp usa o nome do template aprovado no fornecedor. Placeholders numéricos `{{1}}`, `{{2}}` documentam quais parâmetros fornecer; a Meta valida o número e a ordem configurados no template aprovado.
- Quando `requiresConsent=true`, a ausência de consentimento concedido para a mesma unidade, canal, finalidade e destino bloqueia o envio sem chamar o fornecedor.
- Consentimento deve corresponder a um evento verificável de opt-in. Informar a origem no formulário administrativo e registrar uma nova entrada (`isGranted=false`) quando houver opt-out; decisões anteriores não são sobrescritas, e somente a mais recente vale para autorização do envio.
- Reutilizar a mesma chave idempotente retorna a solicitação existente; reutilizá-la para conteúdo ou destino diferente retorna conflito.

Estados de notificação: `Queued`, `AcceptedByProvider`, `BlockedByConsent`, `RetryScheduled`, `Failed` e `Uncertain`. Códigos de erro são seguros para suporte e não contêm conteúdo da mensagem, destino nem credenciais.

## Manutenção

As mensagens de notificação ficam em `communications.notification`; tentativas ficam em `communications.notification_attempt`. A retenção e expurgo seguem a política do negócio e legislação de privacidade antes de ativar o gateway em produção; esta change não apaga esse histórico automaticamente.
