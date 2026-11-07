# Dúvidas dos dados dos jogos

Lista consolidada (Entrega 20, RockAssistent) do que ficou incerto nos dados embutidos (`assets/data/`) e nos guias
(`content/*/guides/`). **Vai ser perguntada ao autor** antes do merge; ele pode responder com o detonado, o jogo ou
outra fonte.

- **Dúvidas** (por jogo, depois as que valem para vários jogos): itens com `"verified": false`, fontes que discordam
  ou convenções que adotamos e que o autor pode querer mudar. Cada linha diz o que falta saber e onde já procuramos.
  Quando uma dúvida for resolvida, corrija o JSON ou o guia, marque `verified: true` e apague a linha.
- **Dados a corrigir nos JSON** (no fim): erros já confirmados pelos analistas de dados durante a escrita dos guias.
  Não são dúvidas: falta só aplicar.

## Dúvidas por jogo

### Crystal (`c`)

| item | dúvida | fontes tentadas |
| --- | --- | --- |
| shiny-methods/odd_egg | Chance de shiny do Ovo Estranho fora do Japão: 14% na Pokestats e 1/10 na Rotom Labs. | pokestats.gg/shiny-hunting-hub/methods/odd-egg; rotomlabs.net/shiny-rates |

### Ruby / Sapphire (`rs`)

| item | dúvida | fontes tentadas |
| --- | --- | --- |
| areas/rs/hideout | Hideout da Magma (Ruby) e da Aqua (Sapphire) numa área só, sem `poke_api_location_id`: no PokeAPI o team-magma-hideout tem encontros do Ruby e do Sapphire e o team-aqua-hideout só do Sapphire, então nenhum dos dois serve para a área inteira. | pokeapi.co |
| trainers/rs.json (versões) | Os grunts da Magma (Ruby) e da Aqua (Sapphire) e o Collector Hector (times diferentes) vêm de `constants/version.inc`; levam `versions`. Para isso `TrainerBattle` ganhou `versions` (como os chefes). | pokeruby |

### Emerald (`e`)

| item | dúvida | fontes tentadas |
| --- | --- | --- |
| bosses (Frontier Brains) | O nível ficou 50 (categoria Lv. 50); no Open Level o jogo usa o nível do seu Pokémon mais forte (mínimo 60). O Noland (Battle Factory) não entrou: usa Pokémon alugados, sem time fixo na decompilação. | pret/pokeemerald src/frontier_util.c |
| areas/e/magma-hideout | Apontado para `magma-hideout` (486, encontros do Emerald); o PokeAPI também tem `team-magma-hideout` (694) com encontros do Emerald, que ficou sem área. | pokeapi.co |
| guides `walkthrough.md`, `postgame.md`, `bosses.md` | Regras dos Battle Tents (Slateport/Verdanturf/Fallarbor) e times/dicas dos Frontier Brains não reconferidos numa segunda fonte (times tirados de `bosses/e.json`). | poketools.com, notas do pokemythology |

### FireRed / LeafGreen (`frlg`)

| item | dúvida | fontes tentadas |
| --- | --- | --- |
| areas/frlg/dunsparce-tunnel | O PokeAPI põe o Dunsparce em `three-isle-port` (não em `three-isle-path`); a área aponta para o port. | pokeapi.co |
| areas (Sevii) | Acrescentados Cape Brink, Water Path, Ruin Valley, Green Path, Outcast Island, Pattern Bush, Altering Cave, Canyon Entrance, Five Isle Meadow, Memorial Pillar, Water Labyrinth, Resort Gorgeous, Lost Cave e as 7 câmaras de Tanoby (locais do PokeAPI com encontros que as notas não citavam). Ordem dentro das ilhas aproximada. | pokeapi.co |

### Diamond / Pearl (`dp`)

| item | dúvida | fontes tentadas |
| --- | --- | --- |
| trainers/dp.json | Os eventos do DP na decomp (`zone_event_release` + `map_header.c`, script 3000+id) não batem com os times (ex.: Clarice Lv. 54 e Deandre Lv. 56 na Eterna Forest, Psychics Lv. 15 na Victory Road). Por isso o lugar vem do Platinum, pelo mesmo índice e nome (os índices do DP e do Pt batem: 840 de 850 nomes iguais; eventos e `scripts.s`, que trazem também as lutas de script). `verified: true` só quando os eventos do DP dão a mesma área (196 de 433). Os 6 sem lugar no Pt ficam na área dos eventos do DP, com `verified: false`. Classe pelo nome do Pt. Objetos `TRAINER_DUMMY_*` (um Rattata Lv. 5, "Mickey"/"Angelica") ficaram fora. Conferido: nenhum treinador fica mais de 14 níveis acima da mediana da área, salvo Veteran Grant e Hiker Alexander (acesso tardio, o Pt também os põe ali). | pokediamond / pokeplatinum; sem fonte nova (é convenção de método, não um fato a conferir) |

### Platinum (`pt`)

| item | dúvida | fontes tentadas |
| --- | --- | --- |

### HeartGold / SoulSilver (`hgss`)

| item | dúvida | fontes tentadas |
| --- | --- | --- |
| trainers/hgss.json | Lutas de script de todos os `scr_seq_<n>_<mapa>.s` do mapa (Fuchsia Gym, ginásio do Blaine nas Seafoam Islands, Route 24, 25 e 34...). Goldenrod Tunnel (MAPSEC Goldenrod City) → goldenrod-underground. Bug Catcher Wayne (índice 533, Ilex Forest) está nos dados mas em nenhum evento ou script da decomp: entrou na ilex-forest com `verified: false`. | pokeheartgold; serebii pokearth ilexforest (sem lista de treinadores); strategywiki HGSS Ilex Forest; Bulbapedia por busca (só o Wayne do Crystal) |

### Black / White (`bw`)

| item | dúvida | fontes tentadas |
| --- | --- | --- |
| bosses (golpes e itens) | Não há decompilação do Black/White: espécies e níveis conferidos na Serebii, na pokemondb (ginásios e Elite 4) e nas notas; golpes e itens vêm só da Serebii. | serebii.net/blackwhite (gyms, elitefour, cheren, bianca, plasma, strongtrainers); pokemondb.net/black-white/gymleaders-elitefour; sem 2ª fonte com golpes nesta rodada |
| trainers/bw.json | Sem decompilação: tudo da Serebii (Pokéarth, seção Black & White), fonte única, `verified: false`. Black City só no Black e White Forest só no White (área `black-city`); "Black Only"/"White Only" viram `versions`. Classe separada do nome pela lista de classes do pokebw2 (a Serebii junta "Classe Nome"); "Cycling" virou "Cyclist". Lass Dye (Nacrene): a Serebii põe "Black Only" na tabela errada; ficou Lilligant no Black e Whimsicott no White. Mesmo treinador com o mesmo time nas duas versões (ex.: Ace Trainer Glinda, Marvelous Bridge) virou uma entrada só, com `versions` nulo. Golpe repetido na Serebii (Harden, Harden) entrou uma vez. Challenger's Cave entrou (o apóstrofo da URL tinha quebrado o download). | serebii.net/pokearth/unova |

### Black 2 / White 2 (`b2w2`)

| item | dúvida | fontes tentadas |
| --- | --- | --- |
| bosses (Modo Desafio e Modo Fácil) | O time do Modo Desafio dos líderes, da Elite 4 e da Iris está nas notas (espécies da decompilação, níveis da Serebii). Nos outros treinadores o jogo só sobe os níveis (de 1 a 5); no Modo Fácil, baixa. Esses níveis não foram listados. | fuddlesworth/pokebw2 res/trainers/data; serebii.net/black2white2/easychallengemode.shtml |
| bosses (fora) | PWT, Black Tower/White Treehollow (Benga e Boss Trainers), Charles (motoqueiro) e Grunts não entraram. A decompilação usada (fuddlesworth/pokebw2) é um projeto novo e não da pret. | fuddlesworth/pokebw2; notas da Pokémon Mythology |
| bosses/cheren, roxie, burgh, elesa | A decompilação dá 3 golpes explícitos a alguns Pokémon; a Serebii mostra um 4º (Detect, Helping Hand, Clear Smog, Protect, Electro Ball). O JSON segue a decompilação. | fuddlesworth/pokebw2 res/trainers/data; serebii.net/black2white2/gyms.shtml; sem 3ª fonte nesta rodada |
| trainers/b2w2.json | Decomp pokebw2 (B2 US, com os mapas das duas versões) conferida com a Serebii: 405 de 407 times batem (Backpacker Blossom na Route 16 e um grunt do Plasma Frigate só na decomp). `versions` pelas notas "Black 2 Only"/"White 2 Only" da Serebii (Route 4 e Reversal Mountain). 82 treinadores que a decomp não põe em mapa (ginásios e lutas de script) vieram da Serebii com o time da decomp. | pokebw2; serebii.net/pokearth/unova |

### X / Y (`xy`)

| item | dúvida | fontes tentadas |
| --- | --- | --- |
| guias/xy (detonado) + bosses/korrina-geosenge | Korrina em Geosenge: o guia e o JSON dizem dupla com a Serena/Calem do seu lado; nenhuma fonte diz o formato (a Serebii lista os dois Lucario Lv 25 como luta comum e nenhuma cita o rival). O resto da linha (Lucario da torre, Sycamore ao sul de Couriway, revanche diária em Kiloude, fósseis Lv 20) foi confirmado. | thonky, serebii pokearth, Bulbapedia por busca; gamerguides/supercheats bloqueiam (403) |

### Omega Ruby / Alpha Sapphire (`oras`)

| item | dúvida | fontes tentadas |
| --- | --- | --- |
| shiny-methods/dexnav | Nível de busca 999: a Pokestats dá 1/476,49 e a Serebii dá 1/542,37 (faixa 901–999). Também faltam as odds abaixo do nível 100 e o efeito do Amuleto fora dos níveis 100, 200 e 999. O DexNav não segue a conta de rolagens. | pokestats.gg/shiny-hunting-hub/methods/dexnav; serebii.net; pokebase 372339 (regras da Bulbapedia: 0,06%/rolagem no nível 100, 0,08% no 200, +0,0001%/nível acima; 4% de chance de 4 rolagens extras; sem tabela final) |
| areas | Rotas juntas das notas (105–108, 129–131, 132–134) separadas; Route 115 e os 11 lugares de Mirage (Mirage Spots, Crescent Isle, Fabled Cave...) acrescentados, todos no nó `mirage-spots`. Team Hideout segue sem location (como o RS na Task 12). | pokeapi.co |

### Sun / Moon (`sm`)

| item | dúvida | fontes tentadas |
| --- | --- | --- |
| bosses/battle-royal, nihilego-1 | Battle Royal: o Rockruff do Royal só no detonado. Nihilego 27 só na Serebii. | serebii.net/sunmoon; notas |
| guias/diferenças de versão | "Treinador em Heahea: Dexio (Sun)/Sina (Moon)" e "pedra do Haina Desert: Sun Stone/Moon Stone". | serebii.net/sunmoon/versionmisc |

### Ultra Sun / Ultra Moon (`usum`)

| item | dúvida | fontes tentadas |
| --- | --- | --- |
| shiny-methods/ultra_wormhole_* | Efeito do Amuleto Brilhante desconhecido. As faixas min/max entre os pontos de 500 anos-luz são interpolação nossa. | pokestats.gg/shiny-hunting-hub/methods/ultra-wormhole |
| bosses/necrozma-1, gladion-3, defense-tristan | Nível 50 do Necrozma (Dusk Mane/Dawn Wings) só no detonado. Gladion no Mount Lanakila: Lucario 53 (Pokéwiki, detonado) contra 55 (Serebii). Tristan: time só da Serebii. | serebii.net; pokewiki.de; notas |
| bosses/aristo-1 | Aristo (Mount Lanakila, 5 × nível 65) só no detonado. | notas |
| guias/Episode RR | Mega Mewtwo X (US)/Y (UM) do Giovanni; o que libera os itens do B2F; regras da Battle Agency. | thonky episode-rr; serebii |

### Let's Go Pikachu / Eevee (`lgpe`)

| item | dúvida | fontes tentadas |
| --- | --- | --- |
| guias/aves lendárias no céu | Aves voando no céu de Kanto: depois de virar Campeão (Bulbapedia) ou depois de capturar as 3 (rankedboost). | bulbapedia; rankedboost |

### Sword / Shield (`swsh`)

| item | dúvida | fontes tentadas |
| --- | --- | --- |
| bosses (Champion Cup) | O Sword/Shield não tem Elite 4: a final da Champion Cup (Nessa, Bea/Allister, Raihan) é `other` com title "Champion Cup"; as semifinais (Marnie, Hop) e o Bede são `rival`. | serebii.net/swordshield/championcup.shtml |
| bosses/eternatus-1, peony-rematch | Eternatus 60 só no detonado (a Peony foi conferida à parte). | pokewiki.de/Peony; notas |
| bosses/honey-1, honey-rematch | Honey (Battle Court) só no detonado; o Venusaur/Blastoise Gigantamax depende do inicial de Kanto escolhido. | notas |
| areas | `wild-area` virou as 17 sub-áreas do PokeAPI + Meetup Spot; sub-áreas do Isle of Armor e da Crown Tundra acrescentadas (ordem dentro de cada DLC aproximada). Tower of Darkness / Tower of Waters numa área só, apontando para `tower-of-darkness`. | pokeapi.co |
| guias/Champion Cup | Rare League Card para convidar no Champion Cup; ordem dos 4 últimos estádios do pós-jogo. | serebii championcuprematch; gamerguides |

### Brilliant Diamond / Shining Pearl (`bdsp`)

| item | dúvida | fontes tentadas |
| --- | --- | --- |
| bosses/dawn-postgame | A forma da Wormadam da Dawn com cada inicial: a Serebii dá Sandy (Chimchar), Trash (Piplup) e Plant (Turtwig); o detonado troca Sandy e Trash; a Pokéwiki usa a mesma forma nas três. Ficou a da Serebii; as variantes por inicial continuam sem confirmação independente. | serebii.net/brilliantdiamondshiningpearl/dawnlucas.shtml; pokewiki.de/Lucia; notas |
| areas/bdsp | Grand Underground e Ramanas Park sem location no PokeAPI nem nó no mapa; o PokeAPI não tem encontros do BDSP (ids só pelo nome do local). | pokeapi.co |
| guias/pós-jogo | Slates do Ramanas Park liberadas em etapas; nível dos fósseis (1 ou 20); TM42 na Survival Area; banidos da Battle Tower; tutores (Draco Meteor etc.). | game8; gamerguides; gosunoob |

### Legends: Arceus (`pla`)

| item | dúvida | fontes tentadas |
| --- | --- | --- |
| bosses (estrutura) | Sem ginásios: Nobres são `other` (title "Noble"), guardiões `other` (title "Warden"), Miss Fortune Sisters `evil`. O "campeão" é o Palkia Origem (lado do Adaman; Dialga Origem do lado da Irida), como nas notas; nível só no detonado. | notas; serebii.net/legendsarceus/noblepokemon.shtml |
| bosses/alpha-*, ursaluna-1, braviary-1, charm-*, arceus, origin-palkia | Lutas só no detonado (sem golpes): Alphas das missões, Ursaluna e teste do Braviary (Charm, Arceus e Palkia Origem foram conferidos à parte). Beni: a Pokéwiki dá Gardevoir 59 e o detonado, 60. | notas; pokewiki.de |
| bosses/ress-1 | Ress (pedido 104, Alphas nível 62) só no detonado; a revanche no Training Grounds da Serebii tem nível 63. | notas; serebii.net/legendsarceus/ingobattles.shtml |
| bosses/adaman-3, irida-3 | Times do pedido 89/90 (Leafeon/Glaceon 64 com eeveelutions 32) só na Pokéwiki e nas notas, sem golpes conferidos. | pokewiki.de; notas |
| areas | Jubilife Village foi para o primeiro lugar (as notas começam na Prelude Beach); Tidal Passage e Turnback Cave (esta no fim, pós-jogo) acrescentadas. Regiões e acampamentos não têm location no PokeAPI. | pokeapi.co |
| encounters/pla.json | Chance pela maior porcentagem do Serebii. Clima só quando a espécie aparece em parte dos climas daquele horário (`conditions` = horário,clima). Serebii tem Psyduck em Mass Outbreak de Golden Lowlands como 24–22 (invertido na fonte; ficou 22–24). | serebii.net/pokearth/hisui |
| encounters/pla.json (Celestica Ruins) | Espécies das notas que a Serebii não lista na sub-área entraram como `walk` `verified: false`, com a faixa de nível da área. Darkrai 70 (noite) e Thundurus 70 (rainstorm) também só das notas. Alphas da seção "Alpha Spawn" da Serebii viraram `fixed` com nível único (ex.: Luxray 55, Glalie 62, Abomasnow 68). | pla-pokemythology-notes.md; serebii.net |
| guias/pós-jogo | Níveis do Spiritomb (60, Req22) e do Alolan Vulpix (40, Req83): recompensas confirmadas, níveis não. Quais Plates abrem o Snowpoint Temple. Revanches dos Nobres "depois do Arceus". | gamerguides; thonky; gamewith |

### Scarlet / Violet (`sv`)

| item | dúvida | fontes tentadas |
| --- | --- | --- |
| bosses (ordem e rotas) | `order` segue o nível recomendado do detonado, intercalando as três rotas. Starmobile entra no time do chefe da Team Star (Revavroom com a forma da equipe). Titans ficam com um Pokémon (as duas fases são o mesmo). Os golpes do Teal Mask são os do pós-jogo (o DLC antes do fim da história usa times mais fracos, na nota). | serebii.net/scarletviolet; pokewiki.de |
| bosses/tulip | O detonado dá Tulip 41/42; a Serebii e a Pokéwiki dão 44/45 (ficou 44/45). | serebii.net/scarletviolet/gyms.shtml; pokewiki.de/Tulia |
| bosses (Tera de Carmine e Kieran) | O Tera Type de cada Pokémon da Carmine (Terarium) e do Kieran campeão vem da revisão/Serebii; falta uma 2ª fonte por Pokémon. | serebii.net |
| bosses/titan-* | O 4º golpe de alguns Titans não aparece nas fontes. | serebii.net/scarletviolet/pathoflegends.shtml |
| bosses/larry-rematch | Forma do Dudunsparce (duas ou três partes) na revanche do Larry não confirmada. | serebii.net |
| bosses/loyal-*, mochi-* | Loyal Three (nível 35 na história) e as duplas do Mochi Mayhem vêm só da Serebii (as notas confirmam Arven/Penny e os avós). | serebii.net/scarletviolet/kitakamimaskfestival.shtml, mochimayhem.shtml |
| encounters/sv.json (regra de nível) | Regra para o Nuzlocke: uma linha por bioma do pokemondb (`conditions` com o bioma: grass, flower_patch, lake...), com a faixa daquele bioma; teto da área = maior nível dos Tera selvagens fixos da área (serebii.net/scarletviolet/wildteratypepokemon.shtml; no Kitakami vale o 1º nível, o de antes do fim da história) ou, sem Tera, o maior nível dos treinadores da página da área no Serebii; sem treinadores, o maior nível dos Pokémon fixos da página (1º nível no Kitakami; ex.: Oni's Maw 19); a Inleth Grotto, sem nada disso, usa o teto da South Province (Area One), por onde se entra (12); `max_level` <= teto + 5 (e `min_level` <= `max_level`). Linhas sem o pokemondb (só Serebii ou só notas, sem chance) ficam com min = max = teto e `verified: false`. Linhas do pokemondb sem a espécie na área no Serebii ou nas notas foram tiradas (1775). Tera Raid não entrou (as notas não dizem quais espécies só aparecem em raid). | pokemondb.net; serebii.net |
| encounters/sv.json (fixed) | Lendários e Titans vêm só das notas (nível e lugar); os do Snacksworth estão todos em lv 70 e nas áreas descritas (Kubfu 30). | sv-pokemythology-notes.md |
| encounters/sv.json (Spiritomb, Eiscue) | Spiritomb (Glaseado Mountain) e Eiscue [Violet] (North Province Area One) só nas notas: linhas `verified: false` com o nível do teto da área. | sv-pokemythology-notes.md |
| encounters/sv.json (polar-biome) | A lista de filtros da Serebii do Polar Biome traz Chespin, que não é desse bioma (iniciais: Cyndaquil, Torchic, Chimchar, Piplup, Oshawott, Scorbunny): tirado. | serebii.net/pokearth/terarium/polarbiome.shtml |
| encounters/sv.json (Kitakami e Terarium) | Só a Serebii: espécie confirmada só por ela e nível = teto da área (Tera fixo ou treinadores); no Kitakami o nível muda com o progresso (o teto usa o nível de antes do fim da história). | serebii.net |
| encounters/sv.json (presentes e trocas) | Meowth de Galar (Naranja/Uva, Salvatore), Growlithe de Hisui (Timeless Woods, Perrin), ovos de Turtwig/Chimchar/Piplup lv 1 (Reveler's Road) e as 30 trocas dos Special Coaches (Blueberry Academy) só das notas. Terrakion foi para a West Province (Area One) e Cobalion e Kubfu para a North Province (Area Two). | sv-pokemythology-notes.md |
| guias/Paradox de Area Zero | Níveis dos Paradox selvagens em Area Zero (~55–65; Iron Valiant 58?). | serebii.net areazero; marriland; gamerant |
| guias/raids | Raids de shiny garantido de 2024 (lista e estrelas) e nível das raids "Titanic" dos Loyal Three (notas: 85). | bulbapedia; ggrecon |

### Legends: Z-A (`za`)

| item | dúvida | fontes tentadas |
| --- | --- | --- |
| shiny-methods/za_hyperspace | A Serebii lista com o Amuleto 1/638,08 (nível 2) e 1/585,37 (nível 3), que parecem erros de digitação de 1/683,08 e 1/585,57 (conta de rolagens e Pokestats). Usamos a conta. | serebii.net/legendsz-a/shinypokemon.shtml; pokestats.gg/shiny-hunting-hub/methods/za-dlc-sparkling-power |
| bosses (estrutura) | As partidas de promoção são `gym` (title "Promotion Match", badge = rank alcançado; o Vinnie leva do V direto ao F, então são 10). Não há campeão: a luta final contra a Floette Hyperrogue não tem nível em nenhuma fonte (a Serebii só dá 25.000 de HP) e ficou de fora. Partidas de recompensa 2–20 do Infinite Z-A Royale entraram como `other`. | serebii.net/legendsz-a/mainmissions; pokewiki.de; notas |
| bosses/rogue-* | Rogue Megas: os níveis 34, 41 (Beedrill) e 48 (Mawile, Barbaracle, Ampharos; o detonado dá 41 ao Ampharos) foram conferidos por fora; os de 41 (Hawlucha, Banette), 53, 59 e 62 seguem só no detonado. A Serebii só dá o nível da revanche (70). | notas; serebii.net/legendsz-a/roguerematch.shtml |
| bosses/josee-1 | O detonado dá Machop 15 e Meditite 15; a Serebii e a Pokéwiki, 10 e 11 (ficou 10/11). | serebii.net; pokewiki.de |
| areas | As notas não têm tabela de áreas: a lista (distritos, Wild Zones, esgotos, prédios) e a ordem por missão foram montadas aqui. Distritos, avenidas e bulevares apontam para `lumiose-city`, a estação para `lumiose-station`; as Wild Zones não têm location. A posição de cada Wild Zone no mapa segue o distrito citado nas notas. | serebii.net/pokearth/lumiosecity |
| encounters/za.json (cidade) | Os raros da cidade estão agregados por distrito: a Serebii lista os setores, mas as páginas de distrito juntam tudo, e a lista "raros por setor" das notas também foi somada no distrito. Só os Alphas garantidos têm `alpha: true`; dia/noite vem das notas. Hyperspace (DLC) e Diancie (Mystery Gift) ficaram de fora. | serebii.net/pokearth/lumiosecity |
| encounters/za.json (presentes) | Presentes e trocas pelas páginas de presentes e trocas da Serebii, conferidos com as notas. Lugar aproximado onde a fonte só diz "Lumiose City": iniciais (vert-district) e Absol (vert-district). Porygon (troca, Bleu Sector 6) e Gimmighoul (Rouge Sector 1) só na Serebii (`verified: false`). Académie Étoile e Lumiose Museum viraram áreas sem nó no mapa (distrito não confirmado). | serebii.net/legendsz-a/giftpokemon.shtml, ingametrades.shtml |
| encounters/za.json (wild-zone-17) | Talonflame está nas notas da WZ17 mas não na página da Serebii: linha `verified: false` com a faixa de nível da zona. | za-pokemythology-notes.md; serebii.net |
| encounters/za.json (lysandre-labs) | As notas citam Alpha Aegislash fixo; a Serebii mostra Alpha Arbok. Os dois ficaram; o Aegislash com `verified: false` e o nível dos Alphas da Serebii no Labs (59–60). | za-pokemythology-notes.md; serebii.net |
| encounters/za.json (Flabébé/Floette/Florges) | Formas pelas cores da Serebii (sufixos r/y/o/b/w = 0–4); as notas só citam algumas cores por zona. Florges da WZ16 nas formas 0/3/4 e Pumpkaboo da WZ15 nos 4 tamanhos (0 Average e 1 Small de dia, 2 Large e 3 Super à noite) pelas notas. | za-pokemythology-notes.md |
| encounters/za.json (fixos) | Klefki 40 (esgotos), Sableye (WZ4, nível = faixa da zona: as notas não dizem) e Mareep shiny 11 (WZ1) só das notas (`verified: false`). | za-pokemythology-notes.md |
| guias/Zygarde | Zygarde 10% (lv 33) e 50% (lv 57) "durante a história" pelas notas; a web fala em luta final em 3 fases, lv 84. | nintendolife; VGC; dittobase; notas da Pokémon Mythology |
| guias/Floette e Absol | Floette Eternal Flower lv 72 (do rival, na 15ª partida do Infinite Z-A Royale) e Absol lv 30 (selvagem após o Rogue Mega Absol): níveis não confirmados. | deltiasgaming; rpgsite |
| guias/Mega Stones do Mega Dimension | Origem das Mega Stones da DLC (SM139/141/189/197) e a contagem "22 novas Megas" (a Serebii lista 22 entradas de 18 espécies). | serebii.net/legendsz-a/dlc-megaevolutions |

## Dúvidas que valem para vários jogos

| jogos | item | dúvida | fontes tentadas |
| --- | --- | --- | --- |
| gs, c | bosses (ordem de Kanto) | A ordem dos 8 ginásios de Kanto no pós-jogo é livre; usamos Vermilion, Saffron, Celadon, Fuchsia, Cerulean, Pewter, Seafoam, Viridian. Os times estão confirmados (decompilação). | pret/pokecrystal, pret/pokegold |
| rb, y, gs, c, rs, e, frlg, dp, pt, hgss, bw, b2w2 | bosses (golpes) | Times "golpes padrão do nível" (rivais do começo, Kimono Girls etc.) ficaram com `moves: []` (o jogo tira os golpes do learnset no nível). Falta derivar os golpes do learnset de cada geração. | decompilações (pret, pokebw2) |
| dp, pt, hgss | bosses (Battle Tower/Frontier) | Tower Tycoon Palmer e os Frontier Brains (Pt/HGSS) não entraram: os times ficam nos dados da Frontier (vários conjuntos por série, Thorton e Argenta aleatórios). | pret/pokeplatinum res/trainers/frontier |
| xy, oras, sm, usum, lgpe, swsh, bdsp, pla, sv, za | bosses (fontes) | Não há decompilação dos jogos de 3DS e Switch. Cada luta veio da Serebii (golpes, itens, habilidades, Tera) ou da Pokéwiki alemã (`Team/Zeile`, com número da Dex); `verified: true` quando duas das três fontes (Serebii, Pokéwiki, notas da Pokémon Mythology) batem em espécie e nível. Golpes e habilidades em alemão foram traduzidos pela tabela do PokeAPI. | serebii.net; pokewiki.de (action=raw); notas da Pokémon Mythology |
| xy, oras, sm, usum, lgpe, swsh, bdsp, pla, sv, za | bosses (Mega, Gigantamax, Tera) | Mega Evolução e Gigantamax ficam na forma base (`form` 0) com a pedra no `item` e uma nota ("X megaevolui", "X usa Gigantamax"). `tera_type` só no Pokémon que terastaliza; na BB League do SV todos têm Tera Type (nota). | serebii.net; pokewiki.de |
| xy, oras, sm, usum, swsh, bdsp, pla, sv, za | bosses (fora) | Capturas e encontros fixos (lendários, Snorlax, Zygarde, Tapus, Ultra Beasts da captura), Battle Maison/Tree/Tower/Chateau, Champion Cup de revanche e Galarian Star Tournament (adversários sorteados), Max Raids, Tera Raids de Area Zero, Titanic do Teal Mask, Master Trainers, Perrin, Cyrano e o DLC Mega Dimension do Z-A não entraram. | notas da Pokémon Mythology; serebii.net |
| sm, usum | bosses (trials) | Cada trial é uma entrada `gym` com o capitão no `name`, title "Captain" e o Totem (com os aliados) no time; o "badge" é o cristal Z. O trial do Kommo-o não tem capitão (name "Totem Kommo-o"). No Sun/Moon a Mina só tem trial no pós-jogo (other); no Ultra Sun/Moon são 8 trials (com a Mina e o Kommo-o). Defesas do título são `other` (title "Title Defense"). | serebii.net/sunmoon/islandtrials.shtml; serebii.net/ultrasunultramoon/islandtrials.shtml |
| xy, za | bosses (formas) | Padrão do Vivillon da Viola (XY) incerto: a Serebii e a Pokéwiki discordam. No Z-A o Vivillon da Yvon ficou Meadow (forma 6, o Z-A só tem Meadow e Marine) e o Gourgeist da Gwynn ficou Super (forma 3) nas três lutas; o tamanho do Gourgeist não foi confirmado numa 2ª fonte. | serebii.net; pokewiki.de |
| sm, usum | bosses (Hau no Aether Paradise) | O Hau (A-Raichu 38, Vaporeon 38, Decidueye 39 no SM; 42/41/43 no USUM) luta do seu lado nas duplas contra os funcionários da Aether, não contra você, por isso não entrou. A dupla Faba + funcionário entrou (faba-1-multi). | serebii.net/sunmoon/hau.shtml, aetherfoundation.shtml |
| todos | areas/*.json bosses_before | Convenção: cada chefe aparece uma vez, na primeira área da lista alcançada depois da luta (a união das áreas visitadas = chefes já vencidos), com um ponteiro que só anda para a frente na ordem da história (chefe lutado numa área que já ficou para trás vai para a área seguinte à do chefe anterior). Ordem da lista = ordem de liberação das notas, não a de visita. Nos jogos abertos (SV, Z-A, PLA) muitos chefes caem juntos na mesma área e as recompensas do pós-jogo do Z-A ficam na última área; chefes opcionais, revanches e lutas sem local ficaram sem área (ex.: rogue-absol e os chefes da missão 37 do Z-A). | docs/superpowers/sources/*-pokemythology-notes.md |
| gs, c, hgss | maps/johto.json | O mapa da região do jogo é o de Johto (o validador usa `game.Region`), então a metade de Kanto desses jogos está no próprio `johto.json`, à direita. O `kanto.json` fica para rb/y/frlg/lgpe. | GameDataValidator.cs |
| gs, c, hgss | areas (Kanto) | As notas juntavam rotas (Routes 12–15, 24–25, Cycling Road 16–18) ou não citavam algumas (1, 3, 4, 5, 6, 8, 9, 10, 19, 43): foram separadas/acrescentadas para casar com os locais do PokeAPI que têm encontros. | pokeapi.co |
| todos | maps/*.json | Coordenadas esquemáticas de 0 a 1 (o validador usa 0..1; o brief dizia 0–100), só vizinhança e direção aproximada. | GameDataValidator.cs |
| sm, usum | areas | `player-house` saiu (a primeira área é Iki Town). Áreas juntas das notas foram separadas para casar com o PokeAPI: Routes 16/17, Lake of the Sunne/Moone, Altar of the Sunne/Moone (sm), Routes 14/15 (usum). Acrescentadas: Route 5, Kala'e Bay, Hano Beach e Berry Fields (sm), Route 13 e Rainbow Rocket Castle (usum). `route-11-hokulani-road` virou `route-11`. | pokeapi.co |
| bw, b2w2 | areas | Guidance Chamber (dentro da Mistralton Cave) e Trial Chamber (Victory Road, bw) acrescentadas; as três câmaras das Underground Ruins (b2w2) também. Badge Check Gates, Castelia Park, PWT e Battle Institute sem location. | pokeapi.co |
| todos | maps (unova, kalos, alola, galar, hisui, paldea, lumiose) | Cidades fixadas à mão; os outros nós são posicionados pelos vizinhos (média + repulsão), então a direção é aproximada. Algumas ligações são suposições (ex.: Kala'e Bay pela Route 3, Reverse World pelo Altar of the Moone, Chamber of Emptiness pela Pokémon Village). | bulbapedia.bulbagarden.net |
| todos | maps (galar, unova, hisui) | Crown Tundra no sul/sudoeste, abaixo de Postwick; Pokémon League de Unova é o nó mais ao norte (Opelucid → Route 10 → Victory Road → League); Jubilife Village a oeste do Obsidian Fieldlands. | bulbapedia.bulbagarden.net |
| todos | trainers/*.json (golpes) | Treinador sem golpes na fonte (`NoItemDefaultMoves`, `TRAINERTYPE_NORMAL`, golpes vazios no DS) ficou com `moves: []` = golpes padrão do nível. Não derivamos do learnset (mesmo critério das Tasks 9–11). | decompilações pret / pokebw2 |
| todos | trainers/*.json (o que ficou fora) | Revanches (Match Call, VS Seeker, Pokégear, `loadtrainer` de telefone), instalações de batalha (Frontier, Trainer Hill/Tower, Battle Tower, Big Stadium/Small Court do BW e do B2W2 com o Waiter Clint, PWT, Black Tower/White Treehollow) e o Modo Desafio do B2W2. As Kimono Girls do HGSS já estão em bosses/. Há treinadores bem acima da mediana da área que são de verdade (acesso tardio ou pós-jogo): Trick House (rs, e), Veteran Grant (Oreburgh Gate) e Hiker Alexander (Route 208) no dp/pt, Super Nerd Hugh (Mt. Mortar) no c/hgss, Pokéfan Alex (Route 13) no hgss e os de pós-jogo do bw/b2w2. | decompilações; serebii.net |
| todos | trainers/*.json (mapa → área) | Prédios, andares, ginásios e salas contam na área que os contém (ex.: ginásio → cidade, MossdeepCity_SpaceCenter → mossdeep-space-center no e e mossdeep-city no rs, que não tem essa área). Gen 4: MAPSEC do cabeçalho do mapa (dp, hgss) ou o nome da pasta (pt); Restaurant (Hotel Grand Lake) → valor-lakefront, Cafe → route-210-south, Trainers' School → jubilife-city, Wise Trio's Room (c) → ecruteak-city. | decompilações |
| rb, y | trainers/rb.json, y.json | A Gen 1 não dá nome aos treinadores: `name` = classe (o id fica `<classe>-<área>`). Jr. Trainer ♂/♀ e Swimmer ♂/♀ ficam com a classe sem o símbolo (em todos os jogos). | pokered / pokeyellow |
| gs, c | trainers/gs.json, c.json (itens) | `BERRY` da Gen 2 virou `oran-berry` e `PINK_BOW` virou `silk-scarf` (o PokeAPI não tem os itens da Gen 2; mesmo efeito). | pokegold / pokecrystal |
| rb, y, gs, c, rs, e, frlg | guides `events.md` | Eventos e distribuições da Gen 1–3 (Mew, Surfing Pikachu, Mystery Gift do Stadium, Virtual Console, Jirachi, Old Sea Map só no Japão) escritos de memória ou com uma fonte só; não conferidos. | serebii.net, pokemondb.net (Bulbapedia deu 403) |
| sm, usum | guias/Battle Tree | Red e Blue na 20ª e na 50ª batalha da Battle Tree. | serebii battletree; thonky |

## VGC Champions

| item | dúvida | fontes tentadas |
| --- | --- | --- |
| vgc/regulation.json | Regulation Set M-C conferido (09/09 a 02/12/2026; Victory Road e Serebii; a Pokémon Zone dá 04/12). A lista completa de Pokémon permitidos só aparece em imagens ou dentro do jogo (Recruit → Roster Info): o JSON guarda as adições do M-C, não o roster inteiro (~229 Pokémon, MetaVGC). Lendários restritos: nenhuma fonte diz explicitamente que estão fora. Custo em VP dos Stat Points diverge (2 ou 5 VP por SP); não usado. | victoryroad.pro/champions-regulations; serebii.net/pokemonchampions/rankedbattle/regulationm-c; gamewith.ai/pokemon-champions/en/articles/regulation-mc; metavgc.com/regulations; champdex.com/guides/stat-points |
| vgc/teams.json | Times nossos com os Pokémon e os sets mais usados no M-C (Pikalytics, sem data na página). Os pastes usam EVs (252 ≈ 32 SP) e espécies/itens que o PKHeX conhece; ficaram de fora Mega Raichu, Mega Garchomp Z, Floette Eternal e Mega Golisopod (fortes no M-C) para os 6 existirem no SV, que é onde a análise e a importação rodam (o GameDataTests confere). Poder e PP de alguns golpes mudam no Champions (ex.: Wish e Strength Sap com 8 PP); a análise usa os valores do SV. | pikalytics.com/pokedex/championstournaments |

## Mecânicas e Mitologia (guias)

| item | dúvida | fontes tentadas |
| --- | --- | --- |
| guias/mechanics/status.md (Duração do sono) | Gen 2: contador 2–8 ou 1–6? O guia diz "1–7 (Gen 1–2)" como aproximação. | Bulbapedia Sleep; dragonflycave |
| guias/mythology/galar.md (Meteorito do Eternatus) | "Há 20.000 anos" só aparece em wikis de fã; o guia ficou sem data. | Bulbapedia Darkest Day e Eternatus |
| guias/mythology/hoenn.md (Mt. Chimney) | Inspirado no Sakurajima ou no Monte Aso (Sakurajima ≈ Mt. Pyre)? O guia não cita mais o vulcão real. | Bulbapedia Hoenn |
| guias/mythology/kalos.md (Pedras de Geosenge) | Túmulo dos Pokémon usados na máquina? A Bulbapedia só diz que lembram Carnac; o guia diz só que a arma fica sob a cidade. | Bulbapedia Geosenge Town |
| guias/mythology/hisui.md (Cogita e Volo) | Parentes da Cynthia? Só sugestão do jogo; o guia escreve como sugestão. | Bulbapedia Cogita e Volo |
| guias/mythology/lumiose.md (Época do Z-A) | "Cinco anos depois" de X/Y tem uma fonte só; o guia diz "depois dos eventos de X/Y". | Bulbapedia Team Flare |
| guias/mythology/paldea.md (Academy = Universidade de Salamanca) | Comparação de fãs; tirada do guia. | Bulbapedia Paldea |

## Dados a corrigir nos JSON

Achados pelos analistas de dados nas Tarefas 27 e 28 (guias), conferidos em fonte; os guias já estão certos, os JSON
ainda não. A Tarefa 35 aplicou os que tinham fonte clara (parceiro da Tabitha/Shelly no ORAS, Alder no BW, Loyal
Three do SV, Hapu e Dexio do SM, ordem dos lagos do BDSP, fósseis do BDSP, 1ª luta do Barry no BDSP, nomes do SwSh e
os dois Rocket Grunts do HGSS; o Raichu de Alola do mani-reward já estava certo). Ficaram abaixo os que dependem de
listas longas sem time nas fontes ou de decisão do autor.

Novos `verified: false` da Tarefa 35:

- bosses/sv.json `loyal-okidogi`, `loyal-munkidori`, `loyal-fezandipiti`: nível 70 e locais corrigidos, mas os golpes
  continuam os do nível 35 (nenhuma fonte com os golpes da luta).
- bosses/bdsp.json `barry-route-201`: Piplup 5 com os golpes padrão do nível, sem fonte com o time exato.
- guias/bdsp (detonado): as seções ainda seguem a ordem antiga (Lake Acuity antes de Snowpoint, Lake Valor depois);
  o JSON já tem a ordem certa.

Ainda a corrigir:

| arquivo | item | problema | fonte |
| --- | --- | --- | --- |
| bosses/za.json | lida-reward, faltando | Lida: Vanilluxe ou Vanillish (notas), não resolvido. Faltam zygarde-1 (33), zygarde-50 (57), zygarde-complete (84), hyperrogue-ange-floette, zach-reward-1 e urbain-*. | notas |
| bosses/lgpe.json | Megas, archer-2 | As Megas não têm marcação (o esquema não tem campo); falta a parceira Rocket no archer-2. | serebii lgpe elitefour; notas |
| bosses/swsh.json | faltando | yell-motostoke, eternatus-max, Dynamax dos estádios, leon-tower, star-tournament, calyrex/steed, dmax-vespiquen/greedent, mustard-ultimate; só uma variante de inicial em leon e hop-*. | notas |
| bosses/usum.json | faltando | red-entry/blue-entry, Tapus, eevee-trainers, Necrozma nível 65; só uma variante em hau-5, hau-champion e defense-*. | notas |
| bosses/pla.json | faltando | alpha-* das missões, Dialga/Palkia de captura, adaman-4, irida-4, Mai/Sabi, Wenton/Bren/Zisu, Path of Tenacity; akari-4 "Grotle 62" incerto. | notas |
| guias (sm, usum, bdsp, sv, za) | :::boss no detonado | Alguns chefes aparecem fora do trecho certo no detonado. | revisão das Tarefas 27 e 28 |
| guias/xy | TMV Pass | Quem entrega o TMV Pass. | revisão da Tarefa 27 |

Notas dos detonados do autor que estão erradas (o JSON já está certo, nada a mudar): Totem Mimikyu 35, Kommo-o 49,
Scizor 46 e Noivern 48 (usum); Crobat do Gladion (sm, usum); josee-1 10/11 e rogue-ampharos 48 (za); Dubwool 69 do
hop-final (swsh); Tulip 44/45, jacq-aat 65/66 e as formas 2/3 do Tauros do dendra-aat (sv).
