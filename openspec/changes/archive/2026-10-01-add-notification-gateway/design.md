## Context

Fornecedores e canais variam; casos de uso não devem conhecer SDKs externos. Consulte proposal.md para motivação e os delta specs para os contratos observáveis.

## Goals / Non-Goals

**Goals:** preservar CQRS, isolamento Multi-Tenant, contratos explícitos, I/O assíncrono e evolução incremental.

**Non-Goals:** introduzir microservices, MediatR, AutoMapper ou mensageria externa sem necessidade especificada.

## Decisions

Definir porta de envio na Application, orquestração no módulo Communications e adapters na Infrastructure. Templates e preferências são tenant-scoped. Processamento assíncrono consome outbox; cada adapter traduz estados externos. O escopo inicial habilita e-mail via SMTP e WhatsApp via Meta Cloud API; os contratos são neutros para que SMS e outros canais possam ser adicionados como adapters em changes futuras.

## Risks / Trade-offs

[Risco] Duplicidade → chave idempotente por finalidade e deduplicação no banco; respeitar limites de idempotência de cada provedor. [Risco] Lock-in → contratos internos neutros e adapter por fornecedor. Credenciais SMTP/Meta vêm da configuração protegida do ambiente, nunca de campos editáveis pelo Tenant. O status retornado pela API do fornecedor significa aceito pelo provedor; entrega final exige callback/webhook em evolução posterior.

## Migration Plan

Aplicar após outbox, iniciar com SMTP sandbox (por exemplo Mailpit) e Meta WhatsApp Cloud API de teste, com credenciais vindas de configuração protegida do ambiente, e ativar templates gradualmente. Alterações de banco devem ser compatíveis com a versão anterior durante a implantação e possuir caminho de rollback.
