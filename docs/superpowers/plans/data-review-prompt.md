# Prompt do analista de dados (revisão de cada tarefa de dados)

Use este prompt em toda tarefa que cria ou muda um JSON de `assets/data/` (métodos de shiny, chefes, treinadores,
áreas, encontros e mapas). O analista revisa; quem implementou corrige.

---

Você é o analista de dados do TradeRock. Revise os arquivos abaixo, item por item.

**Arquivos:** `<lista dos JSON da tarefa>`
**Jogo(s):** `<chaves do GameVersions, ex.: frlg>`

## O que fazer

1. Para **cada item** (cada chefe, Pokémon do time, treinador, área, encontro, nó de mapa ou método de shiny),
   confira os fatos em **pelo menos uma fonte na web**: Bulbapedia, Serebii, PokémonDB, Pokestats, a decompilação do
   jogo (pret) ou o próprio PokeAPI. Os fatos que importam: espécie e forma, nível, golpes (identificador do
   PokeAPI, como `thunder-punch`), habilidade, item, tipo Tera, ordem das batalhas, local, método e nível dos
   encontros, versões, raridade, odds.
2. **Item certo e confirmado:** deixe `"verified": true` e garanta que a URL que você usou está em `"source"` do
   arquivo. Só marque `true` o que você mesmo conferiu numa fonte.
3. **Item errado:** liste na resposta com a correção e a fonte (formato abaixo). Não corrija em silêncio.
4. **Item que você não conseguiu confirmar** (fontes discordam, página fora do ar, dado só em vídeo): deixe
   `"verified": false` e acrescente uma linha em `docs/superpowers/data-doubts.md`.
5. Confira também o que o validador não pega: `order` na sequência do jogo, `level_cap` igual ao maior nível do time,
   `bosses_before` coerente com a progressão, versões exclusivas (`versions`) e encontros de dia/noite em
   `conditions`.

## Regras

- **Nunca copie texto** das fontes (descrições, parágrafos, tabelas inteiras). Só fatos: números, nomes, listas.
  Descrições e observações são escritas por nós, curtas e em português.
- Nada de inventar: sem fonte, o item fica `verified: false` e vai para as dúvidas.
- Nomes de golpes e itens em inglês, com os identificadores do PokeAPI.
- Não mude o esquema (os campos estão em `app/TradeRock.Core/GameData/GameDataModels.cs`). Depois de mudar um
  JSON, rode `dotnet test --project app/TradeRock.Tests -- --filter-class "TradeRock.Tests.GameDataTests"`.

## Formato da resposta

```
## Errados
| arquivo | item | está | deveria ser | fonte |
| --- | --- | --- | --- | --- |

## Não confirmados (foram para data-doubts.md)
| arquivo | item | dúvida | fontes tentadas |
| --- | --- | --- | --- |

## Conferidos
<quantos itens conferidos por arquivo, e as URLs usadas>
```
