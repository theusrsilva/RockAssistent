# Dúvidas dos dados dos jogos

Itens de `assets/data/` que ficaram com `"verified": false` porque não deu para confirmar numa fonte. Cada linha diz
o que falta saber e onde já procuramos. Quando a dúvida for resolvida, corrija o JSON, marque `verified: true` e
apague a linha.

| jogo | item | dúvida | fontes tentadas |
| --- | --- | --- | --- |
| bdsp | shiny-methods/masuda_bdsp | A Bulbapedia descreve um bug no breeding do BDSP (com rolagens extras, a primeira é pulada). As odds reais do Masuda no BDSP podem ser 5/4096 em vez de 6/4096. | pokestats.gg/shiny-hunting-hub/methods/masuda; bulbapedia.bulbagarden.net/wiki/Shiny_Charm (só pela busca; a página bloqueia acesso automático) |
| oras | shiny-methods/dexnav | Odds abaixo do nível de busca 100 e o efeito do Amuleto fora dos níveis 100, 200 e 999. O DexNav não segue a conta de rolagens. | pokestats.gg/shiny-hunting-hub/methods/dexnav |
| dp, pt | shiny-methods/poke_radar_dppt | Corrente 40: a fórmula da Serebii dá 1/199,39 e a Pokestats dá 1/195,07. | serebii.net/games/shiny.shtml; pokestats.gg/shiny-hunting-hub/methods/poke-radar |
| usum | shiny-methods/ultra_wormhole_* | Efeito do Amuleto Brilhante nas viagens do Ultra Wormhole (a fonte diz "desconhecido"). | pokestats.gg/shiny-hunting-hub/methods/ultra-wormhole |
