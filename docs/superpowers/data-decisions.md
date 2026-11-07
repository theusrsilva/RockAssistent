# Decisões dos dados pela maioria das fontes

Regra do autor (07/11/2026): cada dúvida de `data-doubts.md` foi pesquisada em várias fontes; vale o que a maioria
aponta e, em empate ou dúvida entre o Serebii e outra fonte, vale o Serebii. Esta tabela lista cada decisão tomada
assim, para o autor conferir depois. As linhas decididas saíram de `data-doubts.md`; os JSON levam `verified: true` e a
URL da fonte vencedora em `source`.

| jogo | item | decisão | fontes a favor | fontes contra |
| --- | --- | --- | --- | --- |
| frlg | areas (Sevii) | Ordem das ilhas no pós-jogo: Four → Six (Dotted Hole) → Five (Rocket Warehouse) → Seven; `order` 65–93, Cerulean Cave 94. Guias (walkthrough, postgame) nos 2 idiomas | Bulbapedia (busca: partes 18/19 do detonado); PokeDream (seções 17–20); game8 (Dotted Hole antes do Warehouse) | nenhuma |
| hgss | trainers/hgss.json (Bug Catcher Wayne) | Removido da Ilex Forest: o treinador 533 não está em nenhum mapa da decomp e a Serebii não lista treinador ali | pret/pokeheartgold; serebii pokearth ilexforest | página de classe Bug Catcher da Bulbapedia (por busca) |
| bw | bosses (golpes e itens) | Golpes e itens da Serebii confirmados (única diferença: Simipour da bianca-5 com Scald, fica a Serebii) | Serebii; dittobase (líderes, Elite 4, Alder); pokewiki.de (Cheren, Bianca, N, Ghetsis, Cynthia) | pokewiki talvez Seed Bomb no Simipour (leitura incerta) |
| bw | trainers/bw.json | Serebii decide (fonte única): 396 treinadores `verified: true`; Lass Dye = Lilligant (Black) e Whimsicott (White) | Serebii pokearth (amostra Route 7 inteira) | — |
| b2w2 | bosses/cheren, roxie, burgh, elesa (4º golpe) | Cheren e Burgh/Swadloon ficam com 3 golpes; Koffing da Roxie ganha `clear-smog`; Emolga e Flaaffy da Elesa ganham `electro-ball` (empate 2×2, vale a Serebii) | Cheren/Burgh: decomp, dittobase, pokewiki. Roxie: Serebii, pokewiki, rankedboost. Elesa: Serebii + 1 | Cheren/Burgh: Serebii, rankedboost. Roxie: decomp, dittobase. Elesa: decomp, dittobase |
| b2w2 | trainers/b2w2.json | Backpacker Blossom (Route 16, Liepard 25) e o Grunt do Plasma Frigate (Scraggy/Liepard 46, só Black 2: `versions: ["black-2"]`) existem | decomp pokebw2; thonky; arrpeegeez; Bulbapedia (busca) | Serebii (não lista) |
| xy | korrina-geosenge | Luta simples, sem parceiro: dois Lucario 25 em sequência. Notas, detonado e bosses.md nos 2 idiomas | Serebii ("Single Battle"); thonky; arrpeegeez | — |
| xy | guias/xy (TMV Pass) | O Sycamore entrega o TMV Pass na Lumiose Station depois do Hall da Fama | thonky; arrpeegeez; GamesRadar; Bulbapedia (busca) | dittobase ("não obtível") |
| xy | bosses (formas): Vivillon da Viola | Padrão Meadow (forma 6); estava 0 (Icy Snow) | Serebii; pokewiki.de | — |
| oras | shiny-methods/dexnav | Tabela da Serebii em 17 faixas (nível 901–999 = 1/542,37), Amuleto nas notas | Serebii (vence o empate) | Pokestats (1/476,49) |
| usum | shiny-methods/ultra_wormhole_* | O Amuleto Brilhante não muda a chance | pokemon.fandom (Shiny); pokebase 434759 | nenhuma (a Serebii não fala do Amuleto) |
| usum | bosses/usum.json (faltando) | +15 chefes `other`: necrozma-capture 65, tapu-koko/lele/bulu/fini 60, 8 eevee-user-* (55; Sylveon 54), kagetora (Eevee 57), red-entry/blue-entry (Battle Tree); faba-rr passou para o Rainbow Rocket Castle. Variantes de inicial de hau-5, hau-champion e defense-* seguem nas notas (convenção) | Serebii (pokearth, battletree, trainers); thonky | — |
| lgpe | bosses/lgpe.json archer-2 | Parceira = Rocket Grunt sem nome (Muk 34, Raticate 34), Multi Battle com o Trace no 5F da Silph Co.; fica na nota (convenção do faba-1-multi) | Serebii teamrocket; supercheats; gamerguides | alguns guias: Raticate 33 |
| swsh | bosses/swsh.json (faltando) | +mustard-ultimate (Mienshao 78, Luxray 78, Lycanroc 80, Corviknight 79, Kommo-o 80, Urshifu 80 G-Max); Max Raids lv 62 do Sordward/Shielbert como nota; yell-motostoke fica fora (Grunts comuns lv 9, a Serebii não os trata como chefe) | pokewiki.de Mastrich; Serebii mustard, sordwardshielbert; gamesradar/strategywiki (Yell lv 9) | — |
| bdsp | barry-route-201 | Removido: no BDSP a 1ª luta do Barry é na Route 203 (a luta lv 5 na Route 201 é do DP). `order` 1–50 | Serebii barry.shtml e pokearth route201 ("no trainers"); game8 342990 | nenhuma para o BDSP |
| bdsp | guias/pós-jogo (fósseis) | Fósseis revividos no nível 1 (guias já diziam 1; 7 linhas do JSON `verified: true`) | Serebii pokearth oreburghcity (aba BDSP); gamerguides; game8 348133 | segmentnext, Bulbapedia, rankedboost (20, dado do DP) |
| bdsp | :::boss no detonado | mars-1 → Valley Windworks; barry-postgame → Fight Area; barry-final → fim do pós-jogo | Serebii barry, teamgalactic | — |
| pla | alpha-*, ursaluna-1, braviary-1, charm-*, arceus, origin-palkia | Kricketune 12; Goodra/Overqwil/Zoroark 58; Ursaluna 26; Braviary 54; Arceus 75; Palkia/Dialga Origem 65; Charm com Rhydon/Gengar 34/35 e 61/62 (com golpes) | Serebii (missões, missfortune); game8; thonky; gamewith; gamerguides; Bulbapedia (busca) | nenhuma |
| pla | ress-1 | Pedido 104 no nível 62 (a revanche do Training Grounds, 63, é outra luta) | playerassist; notas | Serebii só cobre a revanche |
| pla | adaman-3, irida-3 | Leafeon/Glaceon 64 com duas Eeveelutions 32, como estava | Serebii requests; notas | gamerguides (busca) só cita os 32 |
| pla | encounters (Psyduck do Mass Outbreak) | Golden Lowlands 22–24 (Alpha 37–39); a Serebii escreve invertido | Serebii goldenlowlands (padrão das outras espécies) | — |
| pla | encounters (Celestica Ruins) | Machop/Machoke saíram; Bronzor/Bronzong ficam (uncommon); Darkrai 70 confirmado; Thundurus 70 vai para a Sand's Reach | Serebii; pokemondb; gamertweak; gfinity; segmentnext | notas |
| pla | guias/pós-jogo (Vulpix) | Alolan Vulpix da Keaka (Req83) nível 40, como o guia dizia | Serebii legendsarceus/gift; notas | nenhuma |
| pla | bosses/pla.json (faltando) | +18 chefes: alpha-vespiquen 60, alpha-onix 60, adaman-4, irida-4, mai-2, sabi-2, wenton-1, bren-1, zisu-1 e 9 tenacity-*; Grotle 62 do akari-4 confirmado (pedido 102) | Serebii ingobattles, pathoftenacity, requests; dittobase akari; game8; gamerguides; Bulbapedia (busca) | nenhuma |
| sv | bosses (Tera de Carmine e Kieran) | Como estava (bate com a Serebii) | Serebii mossuitown; dittobase (níveis) | nenhuma |
| sv | bosses/titan-* | 4º golpe (2ª fase): Klawf rock-tomb, Bombirdier torment, Orthworm sandstorm, Great Tusk/Iron Treads stomping-tantrum, Dondozo order-up, Tatsugiri dragon-pulse | game8; gamewith; thonky; marriland | marriland (Rock Throw no Klawf); game8 (troca o Knock Off; Serebii, marriland e gamewith mantêm) |
| sv | bosses/larry-rematch | Dudunsparce de duas partes (form 0); o Komala fica (time de 5) | pokewiki.de Aoki; Serebii gymrematches; game8 397599 | nenhuma |
| sv | bosses/mochi-* | Como estava (bate com a Serebii) | Serebii mochimayhem | nenhuma |
| sv | encounters (fixed) | Titans, lendários, Snacksworth (70) e Kubfu (30) `verified: true`; 2 linhas duplicadas de Koraidon/Miraidon na Underdepths saíram; firescourge-shrine no mapa da North Province (Area Two) | Serebii; pokewiki; rpgsite; videogamer; Bulbapedia (busca) | thenerdstash (Kubfu 70); resumo da Serebii (Wo-Chien na West Area One) |
| sv | encounters (Spiritomb, Eiscue) | Spiritomb fixo lv 49 (Glaseado Mountain); Eiscue [Violet] beach/ocean 20–50 na North Province (Area One) | Serebii glaseadomountain; pokemondb; powerpyx | Serebii North Area One (não lista o Eiscue) |
| sv | encounters (polar-biome) | Chespin é do Canyon Biome, não do Polar (JSON já certo) | Serebii terariumstarters; nintendolife; notas | alguns guias |
| sv | encounters (Kitakami e Terarium) | Serebii decide: 1060 linhas `verified: true` (o nível continua o teto da área, convenção) | Serebii pokearth | nenhuma |
| sv | encounters (presentes e trocas) | 10 presentes e 30 trocas `verified: true`; Growlithe de Hisui vai para a mossui-town | Serebii giftpokemon, leagueclubtrades; nintendolife; prima; game8 | nenhuma |
| sv | guias/Paradox de Area Zero | Paradox selvagens da Area Zero 52–60 (Iron Valiant e Roaring Moon também); caverna secreta perto da Research Station 3 | pokemondb; pokepedia; rpgsite; marriland | rankedboost (Iron Valiant 75) |
| sv | guias/raids | Shiny garantido em 2024: só o Rayquaza 5★ (Tera Dragon, 20/12/2024–05/01/2025); raids titânicas dos Loyal Three lv 35 (antes do fim) e 85 (depois) | Serebii teraraidbattleevents, kitakamimaskfestival; nintendolife | nenhuma |
| sv | :::boss no detonado | grusha-rematch vai para "Revanches dos ginásios" | Serebii; ordem do JSON | — |
| za | shiny-methods/za_hyperspace | Fica a conta de rolagens (1/683,08 e 1/585,57) | conta de rolagens; pokestats.gg | Serebii (1/638,08 e 1/585,37) |
| za | bosses/rogue-* | Hawlucha/Banette 41; Froslass/Altaria/Venusaur 53; Dragonite/Tyranitar/Starmie 59; missões 36–37 = 62; Rogue Mega Absol 25 (estava 34), no telhado do Jaune District | gmtreks; deltiasgaming; sportskeeda; notas (menos o Absol) | notas (Absol 34) |
| za | bosses/josee-1 | Machop 10 e Meditite 11 | Serebii; pokewiki.de | notas (15/15) |
| za | encounters (presentes) | Porygon e Gimmighoul `verified: true`; iniciais no vert-district; Absol (lv 30) no jaune-district | Serebii giftpokemon, ingametrades; gmtreks; game8; deltiasgaming | — |
| za | encounters (wild-zone-17) | Talonflame removido da WZ17 | Serebii wildzone17; nintendolife; dbltap; gamerguides | notas; deltiasgaming; thepokepedia (empate, vale a Serebii) |
| za | encounters (lysandre-labs) | Os dois existem: Alpha Arbok 59–60 e Alpha Aegislash fixo lv 50 | gmtreks missão 36; Serebii (cita o Alpha Aegislash); game8; notas | — |
| za | encounters (fixos) | Klefki 40 (esgotos), Sableye da WZ4 lv 10, Mareep shiny 11 | powerpyx; game8; Serebii; gmtreks; Bulbapedia (busca); thepokepedia | — |
| za | guias/Zygarde | Uma luta só, em 3 fases (10%, 50%, Completo), todas lv 84, na WZ20; saíram as lutas 33/57 "na história". Fica fora de bosses (convenção das capturas) | Serebii tokeeptheworldinbalance; gmtreks; videogameschronicle; nintendolife; dittobase | notas (33/57) |
| za | guias/Absol | Absol presente lv 30; Rogue Mega Absol lv 25 | Serebii giftpokemon; deltiasgaming; gmtreks | notas (34) |
| za | guias/Mega Stones do Mega Dimension | Origens SM139/141/189/197 confirmadas; "22 formas de 18 espécies"; Magearnite na Quasartico Inc. | Serebii dlc-megaevolutions, megastones | — |
| za | bosses (formas): Yvon e Gwynn | Vivillon da Yvon = Modern (forma 7, desempate); Gourgeist da Gwynn = Small (forma 1) | Serebii reachingrankx/reachingrankd; pokewiki (Gourgeist) | pokewiki (Vivillon Meadow) |
| za | bosses/za.json (lida-reward, faltando) | Lida usa Vanilluxe; +zach-reward-1 (Slowpoke 9, Pidgey 10, Pikachu 10); urbain-* já cobertos por taunie-*; Zygarde fica fora (captura) | Serebii quasarticoinc; gmtreks missão 40 | notas (Vanillish) |
| za | :::boss no detonado | Chefes do walkthrough na ordem da história (Slowbro, Camerupt, Victreebel e naveen-2 → Jaune; josee-2, canari-rematch e ivor-e → The Sewers) | ordem do JSON | — |
| vgc | vgc/regulation.json | M-C de 09/09 a 02/12/2026; lendários e míticos fora (regra nova); Stat Point custa 5 VP (tirar é grátis) | Datas: Serebii, victoryroad, site oficial. Lendários: roster da Serebii. VP: champdex, game8, bluestacks | Datas: Pokémon Zone (04/12). VP: Serebii training (2 VP, dado da pré-estreia) |
| mitologia | guias/mythology/hoenn.md (Mt. Chimney) | Inspirado no Monte Aso (o Sakurajima fica ligado ao Mt. Pyre); o guia cita nos 2 idiomas | Wikipedia (Mount Aso); Bulbapedia (busca); ap2hyc; sportskeeda | nenhuma |
