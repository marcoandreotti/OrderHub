## Context

O catálogo já possui variações e adicionais; a evolução deve preservar dados e pedidos históricos. Consulte proposal.md para motivação e os delta specs para os contratos observáveis.

## Goals / Non-Goals

**Goals:** preservar CQRS, isolamento Multi-Tenant, contratos explícitos, I/O assíncrono e evolução incremental.

**Non-Goals:** introduzir microservices, MediatR, AutoMapper ou mensageria externa sem necessidade especificada.

## Decisions

Generalizar grupos existentes de forma compatível, usando tipos e regras configuráveis em vez de subclasses por pizzaria. A estratégia de preço pertence ao grupo, junto aos limites e às regras de seleção que já são responsabilidade de `AdditionalGroup`; produtos podem reutilizar grupos e, portanto, reutilizam também sua semântica de preço. As estratégias iniciais são `Additive`, `HighestPrice`, `Proportional` e `NoPriceChange`.

O preço-base considerado continua sendo o da variação selecionada, quando houver, ou o preço-base do produto. Sem grupo de composição (`HighestPrice` ou `Proportional`), esse preço-base permanece como preço da unidade. Com exatamente um grupo de composição associado ao produto, o resultado desse grupo substitui o preço-base para formar o preço composto. Grupos `Additive` somam seus valores ao preço-base ou composto; `NoPriceChange` não altera o valor. Um produto não pode associar mais de um grupo de composição, evitando bases concorrentes. Quantidade do item do pedido multiplica o preço unitário resultante.

As regras de preço são: `Additive` soma preço da opção vezes sua quantidade; `HighestPrice` usa o maior preço de opção selecionada, sem ponderá-lo pelas frações; `Proportional` soma preço da opção vezes a fração ocupada; `NoPriceChange` resulta em zero. Nos grupos `HighestPrice` e `Proportional`, a seleção representa partes de uma unidade composta: cada opção selecionada informa uma fração e sua quantidade de ocorrência é um. Nos grupos aditivos, as quantidades contam unidades adicionais e não se aplicam frações.

Compatibilidades são configuradas na associação de uma opção a um grupo e referenciam uma associação alvo (grupo + opção) do mesmo estabelecimento. `Requires` é direcional: quando a opção de origem é selecionada, a opção-alvo também deve estar selecionada. `Excludes` é mútuo: as duas opções não podem ser selecionadas juntas, independentemente da direção cadastrada. Um produto só pode associar grupos que satisfaçam os alvos de suas regras `Requires`; na composição, o domínio valida todas as regras após resolver as escolhas do cliente. Os grupos continuam reutilizáveis, portanto regras e semântica de preço são compartilhadas por todos os produtos que os associam.

Não há conceito equivalente a fração no modelo atual. A composição usa um valor de domínio exato representado por numerador e denominador positivos, sem converter a fração para decimal durante validação ou cálculo. Uma composição configurada como integral só é válida quando a soma racional das frações é exatamente uma unidade. Deve suportar inicialmente 1/1, 1/2 + 1/2, três partes de 1/3 e quatro partes de 1/4. O resultado de `Proportional` é arredondado uma única vez ao construir o valor monetário do grupo, usando a regra existente de `Money` (duas casas decimais, midpoint away from zero).

O adapter de leitura resolve, dentro do tenant e estabelecimento autenticados, o produto, os grupos associados, as opções permitidas, sua disponibilidade e seus preços vigentes. A aplicação orquestra esse contexto resolvido com o domínio; validação de composição, compatibilidade e cálculo monetário são regras do domínio, não do Controller, do adapter ou da interface. O pedido persiste snapshot suficiente para auditoria: produto e preço-base considerado, grupo e estratégia, opção e nome, fração quando aplicável, preço de cada opção, quantidade, valor calculado por grupo e preço unitário final. Nenhuma alteração posterior no catálogo recalcula esse snapshot.

## Risks / Trade-offs

- [Risco] Modelo excessivamente genérico → limitar tipos e estratégias aos casos aprovados nesta change.
- [Risco] Migração de adicionais → mapear grupos atuais para `Additive`, com quantidades e preços atuais, preservando o cálculo preço-base + adicionais.
- [Risco] Estratégia de grupo reutilizado → todos os produtos que associam o mesmo grupo compartilham seus limites, compatibilidades e semântica de preço; configurações incompatíveis exigem grupos distintos.
- [Risco] Arredondamento proporcional → usar frações exatas e arredondar somente o valor monetário final do grupo, conforme `Money`.

## Migration Plan

Adicionar campos compatíveis e migrar grupos atuais para `Additive`, atualizar leitura/escrita, depois habilitar novos tipos e estratégias. Alterações de banco devem ser compatíveis com a versão anterior durante a implantação e possuir caminho de rollback. A migração deve preservar vínculos e preços existentes e comprovar que o preço de produto/variação mais adicionais permanece igual antes e depois do backfill.
