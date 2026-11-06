# Dúvidas dos dados dos jogos

Itens de `assets/data/` que ficaram com `"verified": false` porque não deu para confirmar numa fonte. Cada linha diz
o que falta saber e onde já procuramos. Quando a dúvida for resolvida, corrija o JSON, marque `verified: true` e
apague a linha.

| jogo | item | dúvida | fontes tentadas |
| --- | --- | --- | --- |
| oras | shiny-methods/dexnav | Nível de busca 999: a Pokestats dá 1/476,49 e a Serebii dá 1/542,37 (faixa 901–999). Também faltam as odds abaixo do nível 100 e o efeito do Amuleto fora dos níveis 100, 200 e 999. O DexNav não segue a conta de rolagens. | pokestats.gg/shiny-hunting-hub/methods/dexnav; serebii.net |
| usum | shiny-methods/ultra_wormhole_* | Efeito do Amuleto Brilhante desconhecido. As faixas min/max entre os pontos de 500 anos-luz são interpolação nossa. | pokestats.gg/shiny-hunting-hub/methods/ultra-wormhole |
| c | shiny-methods/odd_egg | Chance de shiny do Ovo Estranho fora do Japão: 14% na Pokestats e 1/10 na Rotom Labs. | pokestats.gg/shiny-hunting-hub/methods/odd-egg; rotomlabs.net/shiny-rates |
| dp, pt | shiny-methods/poke_radar_dppt | Corrente 40: 1/200 (Smogon, usado), 1/199,4 (fórmula da Serebii), 1/199,8 e 1/195,07 (Pokestats). | smogon.com/ingame/guides/dpp_pokeradar; serebii.net/games/shiny.shtml; pokestats.gg/shiny-hunting-hub/methods/poke-radar |
| xy, oras, sm, usum, swsh, sv | ovo sem Masuda com o Amuleto | 1/1365,67 pela Bulbapedia e 1/2048,25 pela Rotom Labs. Hoje não há método de ovo sem Masuda nesses jogos (só no BDSP, egg_bdsp). | bulbapedia (bloqueia acesso automático); rotomlabs.net/shiny-rates |
| za | shiny-methods/za_hyperspace | A Serebii lista com o Amuleto 1/638,08 (nível 2) e 1/585,37 (nível 3), que parecem erros de digitação de 1/683,08 e 1/585,57 (conta de rolagens e Pokestats). Usamos a conta. | serebii.net/legendsz-a/shinypokemon.shtml; pokestats.gg/shiny-hunting-hub/methods/za-dlc-sparkling-power |
| gs, c | bosses (ordem de Kanto) | A ordem dos 8 ginásios de Kanto no pós-jogo é livre; usamos Vermilion, Saffron, Celadon, Fuchsia, Cerulean, Pewter, Seafoam, Viridian. Os times estão confirmados (decompilação). | pret/pokecrystal, pret/pokegold |
| e | bosses (Frontier Brains) | O nível ficou 50 (categoria Lv. 50); no Open Level o jogo usa o nível do seu Pokémon mais forte (mínimo 60). O Noland (Battle Factory) não entrou: usa Pokémon alugados, sem time fixo na decompilação. | pret/pokeemerald src/frontier_util.c |
| rb, y, gs, c, rs, e, frlg | bosses (golpes) | Times "golpes padrão do nível" ficaram com `moves: []` (o jogo tira os golpes do learnset no nível). Falta derivar os golpes do learnset de cada geração. | pret (decompilações) |
