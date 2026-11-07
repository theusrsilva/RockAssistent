# Legends: Arceus — fatos extraídos do detonado da Pokémon Mythology

Fonte: https://www.pokemythology.net/detonados/detonado-legends-arceus/ (PDF "Detonado Legends_ Arceus – Pokémon
Mythology", salvo em 05/11/2026, 773 páginas; cobre o jogo base e o update gratuito 1.1.0 Daybreak).
Só **fatos de jogo** (chefes, níveis, encontros, itens) no nosso formato. Nenhuma prosa foi copiada. Golpes dos
chefes **não** constam no detonado: deixar `moves: []` e `verified: false` para golpes; espécies e níveis são
`verified: true` (uma fonte) até o analista conferir numa segunda.

Mapeamento de `kind` usado aqui (o jogo não tem ginásios): Noble Pokémon em frenesi = `gym` (equivalem aos
ginásios: um por região, liberam a próxima área e dão Plate); Rei/Akari = `rival`; Miss Fortune Sisters e Volo
(pós-jogo) = `evil`; Dialga/Palkia Origin (fim da história) = `champion`; guardiões (wardens), líderes de clã,
Kamado, Beni, Alphas de missão e lendários = `other`. "M" = Mission; "Req" = Request.

## Ordem da história e chefes (para `bosses/pla.json`)

| Order | Id | Kind | Nome | Local | Time (espécie lv) | Cap |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | volo-1 | other | Volo | Saída do Jubilife Village | Togepi 5 | 5 |
| 2 | kamado-0 | other | Kamado (combate corpo a corpo, sem Pokémon) | Galaxy Hall, último andar (M2) | — | — |
| 3 | rei-1 | rival | Rei/Akari (Akari se jogar com menino, Rei com menina) | Training Grounds | Pikachu 9 | 9 |
| 4 | mai-1 | other | Mai (guardiã, Diamond Clan) | Deertrack Path | Munchlax 10 | 10 |
| 5 | alpha-kricketune | other | Alpha Kricketune (M5) | Deertrack Heights | Alpha Kricketune 12 | 12 |
| 6 | lian-1 | other | Lian (guardião, Pearl Clan) | Grandtree Arena | Goomy 15 | 15 |
| 7 | irida-1 | other | Irida (líder do Pearl Clan) | Grandtree Arena (M7) | Glaceon 17 | 17 |
| 8 | noble-kleavor | gym | Noble Kleavor (balms; dá Insect Plate) | Grandtree Arena (M7) | Kleavor 18 | 18 |
| 9 | rei-2 | rival | Rei/Akari | Saída do Jubilife (antes do Crimson Mirelands) | Mime Jr. 19, Pikachu 20 | 20 |
| 10 | volo-2 | other | Volo | Solaceon Ruins | Togepi 22, Gible 22 | 22 |
| 11 | coin-1 | evil | Coin (Miss Fortunes, M8) | Perto das Solaceon Ruins (fogueira) | Toxicroak 23 | 23 |
| 12 | ursaluna-1 | other | Ursaluna enfurecido (não capturável; dá Earth Plate e a montaria) | Sludge Mound | Ursaluna 26 | 26 |
| 13 | noble-lilligant | gym | Noble Hisuian Lilligant (dá Meadow Plate) | Brava Arena | Lilligant 30 | 30 |
| 14 | irida-2 | other | Irida (dupla: os dois de uma vez) | Beachside Camp, Cobalt Coastlands (M9) | Glaceon 30, Eevee 15 | 30 |
| 15 | clover-1 | evil | Clover (M10) | Firespit Island | Abomasnow 35 | 35 |
| 16 | coin-2 | evil | Coin (M10) | Firespit Island | Toxicroak 34 | 34 |
| 17 | charm-1 | evil | Charm (M10) | Firespit Island | Rhydon 34, Gengar 35 | 35 |
| 18 | noble-arcanine | gym | Noble Hisuian Arcanine (dá Flame Plate) | Molten Arena, Firespit Island | Arcanine 36 | 36 |
| 19 | adaman-1 | other | Adaman (líder do Diamond Clan; dupla) | Frente do Galaxy Hall (antes do Coronet Highlands) | Leafeon 36, Eevee 18 | 36 |
| 20 | melli-1 | other | Melli (guardião, Diamond Clan) | Wayward Wood | Skuntank 40 | 40 |
| 21 | ingo-1 | other | Ingo (guardião da Sneasler; dá Toxic Plate) | Perto do Mountain Camp | Machoke 41, Tangela 41, Gliscor 42 | 42 |
| 22 | melli-2 | other | Melli (trio, M11) | Moonview Arena | Skuntank 44, Zubat 22, Skorupi 22 | 44 |
| 23 | noble-electrode | gym | Noble Hisuian Electrode (dá Zap Plate) | Moonview Arena | Electrode 46 | 46 |
| 24 | rei-3 | rival | Rei/Akari | Saída do Jubilife (antes do Alabaster Icelands) | Mr. Mime 46, Staravia 46, Pikachu 47 | 47 |
| 25 | gaeric-1 | other | Gaeric (guardião, Pearl Clan; dupla) | Avalugg's Legacy | Glalie 48, Froslass 24 | 48 |
| 26 | sabi-1 | other | Sabi (guardiã, Diamond Clan; trio) | Snowpoint Temple | Rhyperior 50, Magmortar 30, Electivire 30 | 50 |
| 27 | braviary-1 | other | Hisuian Braviary (teste; dá Sky Plate e a montaria) | Topo do Snowpoint Temple | Braviary 54 | 54 |
| 28 | noble-avalugg | gym | Noble Hisuian Avalugg (M12; dá Icicle Plate) | Icepeak Arena | Avalugg 56 | 56 |
| 29 | alpha-goodra-verity | other | Alpha Hisuian Goodra (M14) | Caverna do Lake Verity | Goodra 58 | 58 |
| 30 | alpha-overqwil-valor | other | Alpha Overqwil (M15) | Caverna do Lake Valor | Overqwil 58 | 58 |
| 31 | alpha-zoroark-acuity | other | Alpha Hisuian Zoroark (M16) | Caverna do Lake Acuity | Zoroark 58 | 58 |
| 32 | beni-1 | other | Beni (ninja, braço direito do Kamado) | Stone Portal, Mt. Coronet | Mismagius 59, Sneasler 59, Gardevoir 60, Gallade 60 | 60 |
| 33 | kamado-1 | other | Kamado (M17) | Subida do Temple of Sinnoh | Hisuian Braviary 61, Golem 61, Clefable 61, Snorlax 62 | 62 |
| 34 | dialga-palkia-1 | other | Dialga (se escolheu Adaman) ou Palkia (se escolheu Irida), batalha normal de captura | Temple of Sinnoh | Dialga/Palkia 65 | 65 |
| 35 | charm-2 | evil | Charm | Primeval Grotto | Rhydon 61, Gengar 62 | 62 |
| 36 | origin-final | champion | Palkia Origin (lado Adaman) ou Dialga Origin (lado Irida), com balms e Origin Ball; créditos | Temple of Sinnoh (M18) | Dialga/Palkia Origin 65 | 65 |
| 37 | alpha-vespiquen | other | Alpha Vespiquen (M20, pós-jogo; dá Stone Plate) | Grueling Grove | Vespiquen 60 | — |
| 38 | kamado-2 | other | Kamado de armadura (M25; dá Fist Plate) | Prelude Beach | Golem 65, Clefable 65, Hisuian Braviary 65, Heracross 65, Snorlax 66 | — |
| 39 | volo-3 | evil | Volo (revelação; batalha final do pós-jogo) | Temple of Sinnoh | Spiritomb 68, Hisuian Arcanine 68, Roserade 68, Lucario 68, Garchomp 68, Togekiss 68 | — |
| 40 | giratina-altered | other | Giratina (logo após Volo, sem curar) | Temple of Sinnoh | Giratina 70 | — |
| 41 | giratina-origin-battle | other | Giratina Origin (continuação; dá Spooky Plate e Azure Flute) | Temple of Sinnoh | Giratina Origin 70 | — |
| 42 | adaman-3 | other | Adaman (Req89; trio; dá Adamant Crystal) | Perto do Diamond Settlement | Leafeon 64, Umbreon 32, Vaporeon 32 | — |
| 43 | irida-3 | other | Irida (Req90; trio; dá Lustrous Globe) | Perto do Pearl Settlement | Glaceon 64, Espeon 32, Flareon 32 | — |
| 44 | arceus | other | Arceus (M27; balms; dá Legend Plate) | Plataforma nos céus, Temple of Sinnoh | Arceus 75 | — |
| 45 | rei-4 | rival | Rei/Akari (Req98, Daybreak) | Ginkgo Landing | Clefairy 60, Staraptor 61, Mr. Mime 61, Pikachu 62, inicial com desvantagem ao seu 62 | — |
| 46 | alpha-onix | other | Alpha Onix (Req103) | Oreburrow Tunnel | Onix 60 | — |
| 47 | ress-1 | other | Ress (Req104; só Alphas) | Saída do Jubilife | Alpha Bidoof 62, Alpha Shinx 62, Alpha Starly 62 | — |
| 48 | adaman-4 | other | Adaman (Req106; só Dialga Origin no seu time; trio) | Ginkgo Landing | Leafeon 65, Umbreon 34, Vaporeon 34 | — |
| 49 | irida-4 | other | Irida (Req107; só Palkia Origin no seu time; trio) | Diamond Settlement | Glaceon 65, Espeon 34, Flareon 34 | — |

Treinadores opcionais da história: Ren (Req35, só com Pachirisu) Ursaring 30; Alpha Hippowdon 34 (Req40, Sludge
Mound); Abomasnow 55 (Req74); Snover 50 (Req76); Froslass 55 (Req82); Alpha Glalie 64 (Req86, Icepeak Cavern);
3 Paras 50 ao mesmo tempo (Req77); Rye (Req88, Training Grounds) Lucario 62. Miss Fortune Sisters aparecem soltas
pelos mapas depois da Sneasler (batalha rápida, time não listado).

Ingo no Training Grounds (liberado depois do Noble Electrode, antes do Alabaster Icelands):
Wenton (Mothim 55, Jolteon 55, Gastrodon-East 56); Bren (Wormadam-Plant 55, Sylveon 55, Gastrodon-West 56);
Zisu (Ambipom 57, Honchkrow 57, Luxray 57, Hisuian Zoroark 58).
Pós-jogo (após M19): Rei/Akari (Mr. Mime 61, Staraptor 61, Pikachu 62); Kamado (Golem 65, Clefable 65, Hisuian
Braviary 65, Heracross 65, Snorlax 66); Beni (Mismagius 63, Sneasler 63, Gardevoir 64, Gallade 64); Ingo
(Alakazam 67, Tangrowth 67, Probopass 67, Machamp 67, Magnezone 67, Gliscor 68).
Depois da Req102 (Daybreak), "Regular Pokémon battle": Rei/Akari (Clefairy 61, Mr. Mime 62, Staraptor 62, inicial
com desvantagem ao seu 63, Pikachu 63); Ingo "but tougher" (Alakazam 75, Tangrowth 75, Probopass 75, Machamp 76,
Magnezone 76, Gliscor 77); Mai, trio (Munchlax 60, Weavile 36, Crobat 36); Sabi, trio (Rhyperior 65, Magmortar 50,
Electivire 50); Ingo "using alphas", trio, só após o "but tougher" (Kleavor 75, Wyrdeer 75, Basculegion 75); Ress
(após Req104).

Path of Tenacity (Ingo, após Req102; três treinadores seguidos sem cura):
- Galaxy Team (Zisu → Beni → Kamado): 1ª Zisu (Rapidash 62, Lopunny 62, Infernape 63), Beni (Mismagius 63,
  Gardevoir 64, Gallade 64), Kamado (Golem 64, Heracross 64, Snorlax 65); 2ª Zisu (Rapidash 64, Lopunny 65,
  Infernape 65), Beni (Roserade 65, Magnezone 65, Gallade 66), Kamado (Golem 67, Heracross 67, Snorlax 67);
  3ª (texto embaralhado, ver Dúvidas) Zisu (Rapidash 67, Steelix 67, Lopunny 68, Infernape 68), Beni (Roserade 68,
  Magnezone 68, Gardevoir 69, Gallade 69), Kamado (Golem 69, Heracross 69, Ursaluna 69, Snorlax 70); 4ª Zisu
  (Rapidash 70, Steelix 71, Lopunny 71, Infernape 72), Beni (Roserade 72, Magnezone 72, Gardevoir 73, Gallade 73),
  Kamado (Golem 74, Heracross 74, Ursaluna 74, Snorlax 75); 5ª+ Zisu (Rapidash 75, Steelix 75, Lopunny 75,
  Drifblim 76, Infernape 76), Beni (Roserade 76, Magnezone 76, Sneasler 76, Gardevoir 77, Gallade 77), Kamado
  (Golem 77, Heracross 77, Probopass 78, Ursaluna 78, Snorlax 79).
- Diamond Clan (Arezu → Melli → Adaman, todos de uma vez; após Req106): 1ª Arezu (Hisuian Lilligant 63, Bronzor 32),
  Melli (Skuntank 64, Golbat 33), Adaman (Leafeon 65, Umbreon 34, Vaporeon 34); 2ª Arezu (Lilligant 65, Bronzong 34),
  Melli (Skuntank 66, Golbat 36), Adaman (Leafeon 67, Umbreon 37, Vaporeon 37); 3ª Arezu (Lilligant 68, Bronzong 40,
  Purugly 40), Melli (Skuntank 69, Drapion 41, Golbat 42), Adaman (Leafeon 70, Umbreon 44, Vaporeon 44); 4ª (texto
  embaralhado) Arezu (Lilligant 71, Bronzong 45, Purugly 45), Melli (Skuntank 72, Drapion 46, Golbat 47), Adaman
  (Leafeon 73, Umbreon 48, Vaporeon 48); 5ª+ Arezu (Lilligant 73, Bronzong 47, Purugly 47), Melli (Skuntank 74,
  Drapion 47, Golbat 48), Adaman (Leafeon 75, Umbreon 50, Vaporeon 50).
- Pearl Clan (Lian → Gaeric → Irida, todos de uma vez; após Req107): 1ª Lian (Hisuian Sliggoo 63, Swinub 32), Gaeric
  (Glalie 64, Froslass 33), Irida (Glaceon 65, Espeon 34, Flareon 34); 2ª Lian (Hisuian Goodra 65, Piloswine 34),
  Gaeric (Glalie 66, Froslass 36), Irida (Glaceon 67, Espeon 37, Flareon 37); 3ª Lian (Goodra 68, Piloswine 40,
  Whiscash 40), Gaeric (Glalie 69, Hisuian Avalugg 41, Froslass 42), Irida (Glaceon 70, Espeon 44, Flareon 44);
  4ª Lian (Goodra 71, Mamoswine 45, Whiscash 45), Gaeric (Glalie 72, Avalugg 46, Froslass 47), Irida (Glaceon 73,
  Espeon 48, Flareon 48); 5ª+ Lian (Goodra 73, Mamoswine 47, Whiscash 47), Gaeric (Glalie 74, Avalugg 47,
  Froslass 48), Irida (Glaceon 75, Espeon 50, Flareon 50).

Path of Solitude (Ingo, um Pokémon só, sem itens; Requests de Gared 108–122, recompensa Seed of Mastery + Exp. Candy):
108 Bidoof vs Eevee 50; 109 Eevee vs Hisuian Zoroark 50; 110 Wormadam vs Togekiss 70; 111 Abra vs Mr. Mime 84;
112 Blissey vs Snorlax 70; 113 Sneasel vs Gligar 70; 114 Rotom vs Garchomp 70; 115 Bastiodon vs Blissey 70;
116 Roselia vs Staravia 60; 117 Magikarp vs Hisuian Electrode 55; 118 Roserade vs Staraptor 80; 119 Kleavor vs
Hisuian Goodra 84; 120 Hisuian Lilligant vs Hisuian Braviary 80; 121 Wurmple vs Staraptor 50; 122 Kricketot vs
Gyarados 60. As 15 completas: 10 Exp. Candy XL.

Revanche dos Noble (após a Pokédex completa e Arceus; balms; prêmio em Exp. Candies): Kleavor 70 (Grandtree Arena,
Lian), Lilligant 70 (Brava Arena, Calaba), Arcanine 70 (arena da Firespit Island, Palina), Electrode 70 (Moonview
Arena, Melli), Avalugg 70 (Icepeak Arena, Gaeric).

Eternal Battle Reverie (após Req102; dormir no alojamento): batalhas seguidas contra desafios de Arceus por tipo ou
tema; lv 80 no início, lv 90 a partir da 11ª, lv 100 a partir da 21ª; na 50ª só Arceus. Sem itens da bolsa, sem cura
entre batalhas; pontos trocados por "wishes". Listas de Pokémon de cada desafio só em imagem no PDF.

Rank da Galaxy Team (pontos de pesquisa reportados à Cyllene → estrela → receita/prêmio):
1★ 500 (Heavy Ball, Revive); 2★ 1.800 (Feather Ball, Super Potion); 3★ 3.500 (Great Ball; Zisu ganha golpes);
4★ 6.000 (Leaden Ball, Hyper Potion); 5★ 8.500 (Wing Ball); 6★ 11.000 (Ultra Ball, Max Potion; Zisu ganha golpes);
7★ 15.000 (Gigaton Ball); 8★ 20.000 (Jet Ball, Full Restore); 9★ 30.000 (10 Rare Candy, 20 Grit Pebble);
10★ 60.000 (20 Rare Candy, 10 Grit Rock). Acesso: Crimson Mirelands 2★, Cobalt Coastlands 3★, Coronet Highlands 4★,
Alabaster Icelands 5★.

## Áreas (para `areas/pla.json`, kind e ordem de liberação)

Cinco regiões de expedição saindo do Jubilife Village; cada uma tem acampamentos (camp = cura, loja, Pastures,
bancada). Sub-áreas abaixo com kind sugerido.

| Id | Nome | Kind | Liberada em |
| --- | --- | --- | --- |
| prelude-beach | Prelude Beach | route | início |
| practice-field | Practice Field | route | início |
| jubilife-village | Jubilife Village (Floaro Main Street, Canala Avenue, Galaxy Hall, Craftworks, The Wallflower, Pastures, Farm, Training Grounds, Trading Post, Folk Shrine, Photo Studio, General Store, Seaside Gate, Front Gate) | city | início |
| obsidian-fieldlands | Obsidian Fieldlands (região) | zone | M2 |
| fieldlands-camp | Fieldlands Camp | building | M2 |
| aspiration-hill | Aspiration Hill | route | M2 |
| horseshoe-plains | Horseshoe Plains | route | M2 |
| floaro-gardens | Floaro Gardens | route | M2 |
| deertrack-path | Deertrack Path | route | M5 (guarda libera a ponte) |
| windswept-run | Windswept Run | route | M5 |
| natures-pantry | Nature's Pantry | route | M5 |
| deertrack-heights | Deertrack Heights (Heights Camp após M5) | route | M5 |
| worn-bridge | Worn Bridge | route | M5 |
| grueling-grove | Grueling Grove | route | M5 |
| oreburrow-tunnel | Oreburrow Tunnel | dungeon | M6 (quebrar rocha grande) |
| the-heartwood | The Heartwood | route | M6 |
| grandtree-arena | Grandtree Arena | building | M6 |
| tidewater-dam | Tidewater Dam | route | M6 |
| obsidian-falls | Obsidian Falls | route | M6 |
| lake-verity | Lake Verity | sea | M6 (água só com Basculegion) |
| sandgem-flats | Sandgem Flats | route | M6 |
| ramanas-island | Ramanas Island | route | após Basculegion |
| crimson-mirelands | Crimson Mirelands (região) | zone | M8 (2★) |
| mirelands-camp | Mirelands Camp | building | M8 |
| golden-lowlands | Golden Lowlands | route | M8 |
| bolderoll-slope | Bolderoll Slope | route | M8 |
| gapejaw-bog | Gapejaw Bog | route | M8 |
| scarlet-bog | Scarlet Bog | route | M8 |
| bogbound-camp | Bogbound Camp | building | Req31 |
| sludge-mound | Sludge Mound | route | M8 |
| ursas-ring | Ursa's Ring | route | M8 |
| holm-of-trials | Holm of Trials | route | após Basculegion (ou pulo com Wyrdeer) |
| diamond-settlement | Diamond Settlement | city | M8 |
| diamond-heath | Diamond Heath | route | M8 |
| cloudpool-ridge | Cloudpool Ridge | route | M8 |
| shrouded-ruins | Shrouded Ruins | dungeon | M8 |
| cottonsedge-prairie | Cottonsedge Prairie | route | M8 |
| droning-meadow | Droning Meadow | route | M8 |
| lake-valor | Lake Valor | sea | M8 (água com Basculegion) |
| solaceon-ruins | Solaceon Ruins | dungeon | M8 |
| brava-arena | Brava Arena | building | M8 |
| cobalt-coastlands | Cobalt Coastlands (região) | zone | M9 (3★) |
| beachside-camp | Beachside Camp | building | M9 |
| ginkgo-landing | Ginkgo Landing | route | M9 |
| crossing-slope | Crossing Slope | route | M9 |
| aipom-hill | Aipom Hill | route | M9 |
| coastlands-camp | Coastlands Camp | building | Req46 |
| bathers-lagoon | Bathers' Lagoon | route | M9 |
| hideaway-bay | Hideaway Bay | route | M9 |
| deadwood-haunt | Deadwood Haunt | route | M9 |
| sands-reach | Sand's Reach | route | M9 |
| tombolo-walk | Tombolo Walk | route | M9 |
| castaway-shore | Castaway Shore | route | M9 |
| windbreak-stand | Windbreak Stand | route | M9 |
| veilstone-cape | Veilstone Cape | route | M9 |
| tranquility-cove | Tranquility Cove | sea | após Basculegion |
| lunkers-lair | Lunker's Lair | sea | após Basculegion |
| seagrass-haven | Seagrass Haven | sea | após Basculegion |
| seaside-hollow | Seaside Hollow (Manaphy) | dungeon | Req66 |
| islespy-shore | Islespy Shore | route | após Basculegion |
| spring-path | Spring Path | route | após Basculegion |
| firespit-island | Firespit Island | route | M10 |
| molten-arena | Molten Arena | building | M10 |
| lava-dome-sanctum | Lava Dome Sanctum (Heatran) | dungeon | M22 (pós-jogo) |
| coronet-highlands | Coronet Highlands (região) | zone | M11 (4★) |
| highlands-camp | Highlands Camp | building | M11 |
| heavenward-lookout | Heavenward Lookout | route | M11 |
| wayward-cave | Wayward Cave | dungeon | M11 |
| wayward-wood | Wayward Wood | route | M11 |
| ancient-quarry | Ancient Quarry | dungeon | M11 |
| sonorous-path | Sonorous Path | route | M11 |
| lonely-spring | Lonely Spring | route | M11 |
| mountain-camp | Mountain Camp | building | Req65 |
| clamberclaw-cliffs | Clamberclaw Cliffs | route | após Sneasler |
| celestica-ruins | Celestica Ruins | dungeon | após Sneasler |
| primeval-grotto | Primeval Grotto | dungeon | após Sneasler |
| celestica-trail | Celestica Trail | route | após Sneasler |
| fabled-spring | Fabled Spring | route | após Sneasler |
| bolderoll-ravine | Bolderoll Ravine | route | após Sneasler |
| stonetooth-rows | Stonetooth Rows | route | após Sneasler |
| sacred-plaza | Sacred Plaza | route | após Sneasler |
| cloudcap-pass | Cloudcap Pass | route | após Sneasler |
| moonview-arena | Moonview Arena | building | M11 |
| alabaster-icelands | Alabaster Icelands (região) | zone | M12 (5★) |
| snowfields-camp | Snowfields Camp | building | M12 |
| whiteout-valley | Whiteout Valley | route | M12 |
| crevasse-passage | Crevasse Passage | dungeon | M12 |
| bonechill-wastes | Bonechill Wastes | route | M12 |
| icebound-falls | Icebound Falls | route | M12 |
| avalanche-slopes | Avalanche Slopes | route | M12 |
| ice-column-chamber | Ice Column Chamber | dungeon | M12 |
| arenas-approach | Arena's Approach | route | M12 |
| snowfall-hot-spring | Snowfall Hot Spring | route | M12 |
| glacier-terrace | Glacier Terrace | route | M12 |
| lake-acuity | Lake Acuity | sea | M12 |
| icepeak-camp | Icepeak Camp | building | Req78 |
| icepeak-cavern | Icepeak Cavern | dungeon | M12 |
| hearts-crag | Heart's Crag | route | M12 |
| pearl-settlement | Pearl Settlement | city | M12 |
| avaluggs-legacy | Avalugg's Legacy | route | M12 |
| snowpoint-temple | Snowpoint Temple | building | M12 |
| icepeak-arena | Icepeak Arena | building | M12 |
| secret-hollow | Secret Hollow | dungeon | Req99 (Daybreak) |
| ancient-retreat | Ancient Retreat (casa da Cogita) | building | M13 |
| summit-camp | Summit Camp | building | M17 |
| stone-portal | Stone Portal | dungeon | M17 |
| temple-of-sinnoh | Temple of Sinnoh (vira Spear Pillar) | building | M17 |

## Encontros (para `encounters/pla.json`; method `walk` salvo indicação; `conditions`: day/night/weather)

Convenções: "(dia)"/"(noite)" = período; "[tree]" = cai de árvore que balança (`tree`); "[rock]" = sai de rocha de
minério (`rock_smash`); "[água]" = nadando (`surf`); "voando longe" = `walk` com nota. Alphas fixos listados como
"Alpha X". Depois de acalmar o Noble de cada região, os Pokémon comuns dela também podem surgir como Alpha. Mass
Outbreaks (`outbreak`) começam numa região a partir da segunda visita.

### Obsidian Fieldlands
- Aspiration Hill: Bidoof, Starly.
- Horseshoe Plains: Wurmple, Eevee (raro), Ponyta, Buizel, Mime Jr. (raro); Alpha Rapidash. Shiny Ponyta fixo (Req19).
- Floaro Gardens: Shinx, Silcoon, Pichu (raro), Beautifly (nível alto); Alpha Luxio. Norte do lago: Luxray.
- Deertrack Path: Kricketot, Munchlax (raro), Zubat (noite), Burmy-Plant [tree], Geodude [rock].
- Windswept Run: Kricketot, Kricketune, Abra, Buizel, Drifloon (noite), Zubat (noite), Burmy-Plant [tree], Geodude
  [rock]. Chimchar fixo em árvore, à noite (Req20).
- Nature's Pantry: Kricketot, Kricketune, Paras, Pikachu (raro), Pichu (raro), Zubat (noite), Burmy-Plant [tree],
  Geodude [rock].
- Deertrack Heights: Geodude, Shinx, Stantler, Chimchar (raro), Monferno (raríssimo), Zubat (noite), Burmy-Plant
  [tree], Geodude [rock].
- Alphas da ilha central: Floatzel, Staravia, Kricketune, Parasect.
- Grueling Grove: Combee, Mothim, Beautifly (dia), Dustox (noite), Burmy-Plant e Wormadam-Plant [tree]; Alpha
  Heracross.
- Lado leste da Worn Bridge: Geodude, Starly, Staravia, Buizel, Zubat (noite), Golbat (noite), Magikarp [água];
  Alpha Graveler.
- Oreburrow Tunnel (`cave`): Machop, Happiny, Zubat (noite), Geodude e Graveler [rock]. Alpha Onix 60 (Req103).
- The Heartwood: Buneary, Psyduck, Shinx, Luxio, Combee, Scyther, Wurmple, Silcoon (dia), Cascoon (noite), Beautifly
  (dia), Dustox (noite), Drifloon (noite), Zubat (noite), Golbat (noite); Alpha Golbat (noite), Alpha Lopunny.
  Moss Rock (Eevee → Leafeon).
- Tidewater Dam: Bidoof, Bibarel, Starly, Staravia, Zubat (noite); Alpha Bibarel.
- Partes altas (com Wyrdeer): Alpha Stantler, Alpha Blissey, Alpha Scyther. Obsidian Falls: Gyarados voando longe;
  rio após a cachoeira: Alpha Magikarp.
- Sandgem Flats: Staravia, Luxio, Luxray, Abra, Kadabra, Mime Jr., Mr. Mime, Shellos-West, Gastrodon-West, Drifblim
  (noite), Wormadam-Plant [tree], Graveler [rock]; Alpha Snorlax (no rio raso a caminho), Alpha Alakazam.
- Lake Verity [água]: Magikarp, Gyarados; Alpha Gyarados. Águas ao sul do Sandgem Flats: Hisuian Qwilfish. Ilha
  menor: Aipom, Drifloon e Drifblim (noite).
- Ramanas Island: Shellos-West, Gastrodon-West, Aipom, Starly, Staravia, Chimchar, Monferno (raro), Drifloon e
  Drifblim (noite), Combee [tree], Graveler [rock]; Alpha Infernape. Mass Outbreak de Hisuian Zorua (Req95).
- Distorção do espaço-tempo (Obsidian): comuns Eevee, Leafeon, Sylveon, Haunter, Gengar, Lickitung, Lickilicky,
  Onix, Steelix, Ursaring, Toxicroak; especiais Sneasel, Weavile.

### Crimson Mirelands
- Golden Lowlands: Psyduck, Kricketot, Kricketune, Paras, Parasect, Pichu, Pikachu, Carnivine, Budew (dia), Gastly
  (noite), Burmy-Sandy [tree], Geodude e Graveler [rock]; Alpha Raichu.
- Bolderoll Slope: Rhyhorn, Geodude, Graveler, Lickitung, Onix, Zubat e Golbat (noite), Burmy-Sandy [tree]; Alpha Onix.
- Gapejaw Bog: Bidoof, Bibarel, Croagunk, Tangela, Ralts, Pachirisu, Barboach, Teddiursa, Hippopotas, Carnivine,
  Yanma, Psyduck, Geodude, Graveler, Budew e Roselia (dia), Gastly e Haunter (noite), Burmy-Sandy [tree]; Alpha
  Ursaring, Alpha Pachirisu, Alpha Tangrowth, Alpha Hippowdon.
- Scarlet Bog: Hippopotas, Hippowdon, Stunky, Skuntank, Croagunk, Geodude, Graveler, Gastly e Haunter (noite);
  Alpha Skuntank. Enamorus 70 (Req94, perto do Alpha Skuntank).
- Sludge Mound: Geodude e Graveler [rock]; Alpha Hippowdon 34 (Req40).
- Ursa's Ring: Teddiursa, Yanma, Geodude, Graveler, Gastly e Haunter (noite), Barboach e Whiscash [água],
  Burmy-Sandy e Pachirisu [tree]; Alpha Ursaring.
- Holm of Trials: Psyduck, Golduck, Croagunk, Toxicroak, Petilil, Goomy, Carnivine, Gastly e Haunter (noite),
  Burmy-Sandy e Pachirisu [tree]; Alpha Hisuian Sliggoo, Alpha Toxicroak, Alpha Torterra.
- Diamond Heath: Paras, Parasect, Rhyhorn, Zubat e Golbat (noite), Bonsly [rock]; Alpha Rhyhorn.
- Cloudpool Ridge: Combee, Roselia, Kricketot, Kricketune, Paras, Parasect, Bonsly, Sudowoodo, Carnivine, Zubat e
  Golbat (noite), Murkrow (noite), Burmy-Sandy [tree]; Alpha Honchkrow, Alpha Roserade.
- Shrouded Ruins: Rhyhorn, Lickitung, Ralts, Kirlia, Carnivine, Gastly e Haunter (noite), Burmy-Sandy [tree]; Alpha
  Rhyhorn, Alpha Lickilicky. Spiritomb 60 (Req22, à noite, após os 107 espíritos).
- Cottonsedge Prairie: Pachirisu, Togepi, Petilil, Togetic (voando longe), Gastly e Haunter (noite), Combee,
  Vespiquen e Pachirisu [tree]; Alpha Carnivine, Alpha Vespiquen.
- Droning Meadow: Paras, Parasect, Bidoof, Bibarel, Yanma, Gastly e Haunter (noite), Combee, Vespiquen e Pachirisu
  [tree]; Alpha Yanmega. Turtwig e Grotle no laguinho do canto inferior direito.
- Rios e Lake Valor [água]: Barboach, Whiscash; Alpha Whiscash (Lake Valor).
- Solaceon Ruins: Unown (cada letra aparece aqui depois da primeira captura dela; também shiny e alpha).
- Distorção (Mirelands): comuns Eevee, Flareon, Umbreon, Drifblim, Floatzel, Luxio, Luxray, Heracross, Lopunny,
  Snorlax; especiais Porygon, Porygon2, Porygon-Z. Pós-jogo: Cyndaquil, Quilava, Hisuian Typhlosion.

### Cobalt Coastlands
- Ginkgo Landing: Skorupi, Spheal, Buizel, Floatzel, Murkrow (noite), Drifloon (noite), Burmy-Trash e
  Wormadam-Trash [tree], Geodude e Graveler [rock]; Alpha Walrein, Alpha Drapion, Alpha Machoke.
- Crossing Slope: Glameow, Purugly, Eevee, Tangela, Starly, Staravia, Murkrow (noite), Drifloon e Drifblim (noite).
- Aipom Hill: Aipom, Ambipom, Combee, Mothim, Buizel, Floatzel, Murkrow (noite), Aipom [tree]; Alpha Ambipom.
- Bathers' Lagoon: Starly, Staravia, Togepi, Buizel, Floatzel, Psyduck, Golduck, Beautifly (dia), Dustox (noite),
  Drifloon e Drifblim (noite), Murkrow (noite); Alpha Golduck.
- Hideaway Bay: Spheal, Sealeo, Happiny, Chansey, Aipom, Remoraid e Hisuian Qwilfish (nadando perto da margem),
  Murkrow (noite), Graveler [rock].
- Deadwood Haunt: Duskull (noite), Dusclops (noite), Chatot (dia), Starly, Staravia e Staraptor (dia), Drifloon e
  Drifblim (noite); Alpha Dusknoir (noite).
- Sand's Reach: Starly e Staravia (dia), Drifloon e Drifblim (noite), Remoraid (margem).
- Tombolo Walk: Burmy-Trash e Wormadam-Trash [tree], Graveler [rock]; Alpha Chansey.
- Castaway Shore: Machop, Machoke, Skorupi, Murkrow (noite); Alpha Drapion, Alpha Octillery.
- Windbreak Stand: Paras, Parasect, Pikachu (raro), Stantler, Hisuian Growlithe, Murkrow (noite).
- Veilstone Cape: Glameow, Vulpix, Hisuian Growlithe, Mothim, Beautifly (dia), Dustox (noite), Murkrow (noite);
  Alpha Purugly.
- Tranquility Cove [água/margem]: Shellos-East, Remoraid, Basculin-White, Hisuian Qwilfish, Mantyke, Mantine,
  Staravia, Staraptor, Togepi, Togetic, Drifloon e Drifblim (noite); Alpha Mantine, Alpha Gyarados. Thundurus 70
  (Req94, só com tempestade, perto do Alpha Gyarados).
- Lunker's Lair [água]: Hisuian Qwilfish, Tentacool, Tentacruel, Drifloon e Drifblim (noite); Alpha Tentacruel.
- Seagrass Haven [água/ilhotas]: Hisuian Qwilfish, Tentacool, Tentacruel, Shellos-East, Gastrodon-East, Finneon,
  Lumineon, Drifloon e Drifblim (noite); Alpha Gastrodon-East, Alpha Lumineon, Alpha Hisuian Qwilfish.
- Islespy Shore: Octillery, Sealeo, Walrein, Golduck, Murkrow (noite); Alpha Empoleon.
- Spring Path: Bibarel, Toxicroak, Mothim, Piplup e Prinplup (laguinho no alto); Alpha Mothim. Giratina Origin 70
  (Req91, caverna da cratera no topo da montanha).
- Firespit Island: Mothim, Magby, Magmar, Graveler; Alpha Ninetales. Heatran 70 (M22, Lava Dome Sanctum).
- Seaside Hollow: Manaphy 50 e 3 Phione 34 (Req66; time com Mantyke, Buizel e Overqwil, período Evening, passar entre
  as rochas em pinça perto do Coastlands Camp).
- Distorção (Coastlands): comuns Eevee, Flareon, Vaporeon, Kadabra, Alakazam, Rhydon, Rhyperior, Mr. Mime,
  Carnivine, Skuntank; especiais Magnemite, Magneton, Magnezone.

### Coronet Highlands
- Heavenward Lookout: Luxio, Luxray, Paras, Parasect, Yanma, Yanmega, Zubat e Golbat (noite), Burmy-Sandy e
  Wormadam-Sandy [tree], Graveler [rock].
- Wayward Cave (`cave`): Barboach, Whiscash, Gible, Zubat, Golbat; Alpha Crobat.
- Wayward Wood: Psyduck, Golduck, Stantler, Heracross, Paras, Parasect, Mothim, Zubat e Golbat (noite), Bronzor
  [rock]; Alpha Mothim.
- Ancient Quarry: Bronzor, Bronzong. Parte alta: Mothim, Geodude, Graveler, Bronzor, Stunky, Skuntank, Croagunk,
  Toxicroak, Goomy, Hisuian Sliggoo; Alpha Hisuian Goodra.
- Sonorous Path: Teddiursa, Ursaring, Shinx, Luxio, Luxray, Heracross [tree], Bronzor [rock].
- Lonely Spring: Psyduck, Golduck, Carnivine, Cherubi e Cherrim [tree], Bronzor [rock]; Alpha Carnivine.
- Clamberclaw Cliffs: Gligar, Geodude, Graveler, Gible, Magnezone (voando longe), Gastly e Haunter (noite),
  Burmy-Sandy e Wormadam-Sandy [tree], Gible e Bronzor [rock]; Alpha Gabite. Darkrai 70 (Req93, à noite, precisa de
  save de BDSP).
- Celestica Ruins: Gligar, Geodude, Graveler, Sudowoodo, Nosepass, Bronzor, Bronzong, Machop, Machoke, Gastly e
  Haunter (noite), Bonsly [rock]; Alpha Gligar. Cânion abaixo: Scyther, Hisuian Sneasel, Gyarados [água]; Alpha
  Gliscor, Alpha Probopass.
- Primeval Grotto (`cave`): Nosepass, Nosepass e Bronzor [rock].
- Celestica Trail: Bronzor, Bronzong, Onix, Rhyhorn, Rhydon, Hippopotas, Hippowdon, Chimecho, Hisuian Sneasel,
  Stunky, Skuntank, Graveler [rock]; Alpha Bronzong, Alpha Steelix.
- Fabled Spring: Budew, Roselia, Cleffa (noite), Clefairy (noite), Basculin-White [água]; Alpha Clefable (noite).
- Bolderoll Ravine: Magikarp [água], Machop, Machoke, Geodude, Graveler, Magnezone (voando longe), Gastly e Haunter
  (noite), Cherubi e Cherrim [tree]; Alpha Golem.
- Stonetooth Rows: Bronzor, Bronzong, Rotom, Misdreavus (noite); Alpha Mismagius (noite).
- Sacred Plaza: Chingling, Chimecho, Hisuian Voltorb, Geodude, Graveler, Rhyhorn, Rhydon, Rotom, Shinx, Luxio,
  Luxray, Gastly e Haunter (noite), Misdreavus (noite), Nosepass [rock]; Alpha Luxray, Alpha Rhyperior.
- Cloudcap Pass: Luxio, Luxray, Elekid, Electabuzz, Nosepass [rock]; Alpha Electivire.
- Moonview Arena: Cresselia 70 (M23).
- Distorção (Highlands): comuns Eevee, Jolteon, Sylveon, Magmar, Magmortar, Dusclops, Dusknoir, Drapion, Octillery,
  Ambipom; especiais Cranidos, Rampardos, Shieldon, Bastiodon. Pós-jogo: Rowlet, Dartrix, Hisuian Decidueye.
- Magneton e Nosepass evoluem nesta região.

### Alabaster Icelands
- Whiteout Valley: Snorunt, Aipom, Burmy-Trash e Wormadam-Trash [tree], Bergmite [rock].
- Crevasse Passage (`cave`): Bergmite.
- Bonechill Wastes: Aipom, Swinub, Piloswine, Buneary, Lopunny, Bergmite, Snover, Abomasnow, Snorunt, Glalie,
  Froslass (noite), Gastly e Haunter (noite), Drifloon e Drifblim (noite), Snorunt e Glalie [rock]; Alpha Glalie,
  Alpha Abomasnow. Tornadus 70 (Req94, só com nevasca, perto do Alpha Glalie).
- Icebound Falls: Machop, Machoke, Riolu, Lucario (nevasca), Elekid, Electabuzz, Crobat (voando longe); Alpha
  Electabuzz, Alpha Lucario.
- Avalanche Slopes: Aipom, Ambipom, Happiny, Chansey, Blissey, Lickitung, Lickilicky, Snorunt, Glalie, Swinub,
  Piloswine, Gible, Gabite, Duskull e Dusclops (noite), Froslass (noite), Gible [rock]; Alpha Garchomp, Alpha Froslass.
- Ice Column Chamber (`cave`): Bergmite, Misdreavus; Alpha Froslass.
- Arena's Approach: Bergmite, Gligar, Elekid, Electabuzz, Aipom, Lickitung, Munchlax, Snorlax, Riolu, Lucario,
  Buneary, Lopunny, Machop, Machoke, Duskull e Dusclops (noite); Alpha Machamp, Alpha Piloswine.
- Snowfall Hot Spring: Lickitung, Aipom, Machop, Machoke, Riolu, Lucario, Munchlax, Snorlax, Buneary, Lopunny.
- Glacier Terrace: Gligar, Aipom, Snover, Abomasnow, Hisuian Sneasel, Drifloon e Drifblim (noite).
- Lake Acuity (margens): Abra, Kadabra, Chingling, Chimecho, Bronzor, Bronzong, Rufflet, Basculin-White [água],
  Bronzor [rock]; Alpha Chimecho.
- Icepeak Cavern (`cave`): Bergmite, Misdreavus, Hisuian Zorua, Hisuian Zoroark (raro), Bergmite [rock]. Ice Rock
  (Eevee → Glaceon).
- Heart's Crag: Ralts, Kirlia, Aipom, Ambipom, Rufflet, Basculin-White [água], Drifloon e Drifblim (noite); Alpha
  Gardevoir.
- Avalugg's Legacy: Bergmite, Hisuian Avalugg, Bibarel, Swinub, Piloswine, Glalie, Froslass (noite), Drifloon e
  Drifblim (noite); Alpha Mamoswine, Alpha Swinub, Alpha Hisuian Sneasel.
- Snowpoint Temple (sem montarias): Glalie, Graveler, Bronzor, Bronzong, Zubat, Golbat, Ralts, Kirlia, Froslass
  (noite); Alpha Gallade. Regigigas 70 (M24, só em batalha).
- Distorção (Icelands): comuns Eevee, Espeon, Glaceon, Pikachu, Raichu, Electabuzz, Electivire, Sealeo, Walrein,
  Scyther, Rapidash, Tangrowth; especial Scizor. Pós-jogo: Oshawott, Dewott, Hisuian Samurott.

### Unown e espíritos do Spiritomb (Req22, 107 no total)
Jubilife (até a Prelude Beach): Unown W, ?, H; 7 espíritos. Obsidian: I, S, E, G, X; 20. Mirelands: D, K, A, V, M; 20.
Coastlands: Z, N, R, Q, B; 20. Highlands: P, F, J, L, C; 20. Icelands: O, Y, !, T, U; 20. Unown só por captura
direta (sem batalha).

### Lendários e míticos (`method: fixed`)
- Dialga ou Palkia 65 (Temple of Sinnoh, M17; o primeiro conforme o clã escolhido) e o outro na forma Origin 65 (M18,
  Origin Ball).
- Uxie, Mesprit, Azelf 70 (cavernas dos lagos, M21; dão Draco Plate).
- Heatran 70 (Lava Dome Sanctum, M22; Iron Plate). Cresselia 70 (Moonview Arena, M23; Dread Plate). Regigigas 70
  (Snowpoint Temple, M24; precisa das Stone, Iron e Icicle Plates; Blank Plate).
- Giratina Origin 70 (Spring Path, Req91; dá Griseous Core).
- Landorus 70 (Ramanas Island), Thundurus 70 (Tranquility Cove, tempestade), Tornadus 70 (Bonechill Wastes, nevasca),
  Enamorus 70 (Scarlet Bog) — Req94; Reveal Glass no fim.
- Arceus 75 (M27, com a Pokédex completa e a Azure Flute; dá Legend Plate).
- Manaphy 50 e Phione 34 (Seaside Hollow, Req66). Shaymin 70 (Floaro Gardens, Req92, precisa de save de SwSh).
  Darkrai 70 (Clamberclaw Cliffs, Req93, precisa de save de BDSP). Os quatro não contam para chegar ao Arceus.
- Spiritomb 60 (Shrouded Ruins, Req22).

## Presentes, trocas e iniciais (`method: gift` / `trade`)

- Inicial: Rowlet/Cyndaquil/Oshawott (Galaxy Hall, antes da M2; nível não citado). Os outros dois: Laventon dá no
  pós-jogo, lv 5.
- Alolan Vulpix lv 40 (Keaka, Req83, após achar 5 Vulpix nas Avalanche Slopes).
- Pedra evolutiva à escolha: Water, Fire ou Thunder Stone (Floaro, Req52).
- Trocas: Trading Post (Simona, entrada do Training Grounds) troca Pokémon e vende itens evolutivos por Merit Points
  (MP vêm das bolsas perdidas entregues no Lost & Found). Nenhuma troca in-game específica listada.
- Pokémon entregues em Requests (você perde): Wurmple (Req1), Buizel grande ≥ 2'8" (Req4), Geodude (Req12), Petilil
  (Req29), Parasect (Req33), Croagunk (Req34), Nosepass (Req68), Pokémon de fogo (Req81).
- Fósseis: não há revivência; Cranidos, Rampardos, Shieldon, Bastiodon só em distorções do Coronet Highlands. Sem ovos.
- Montarias (não são Pokémon do time): Wyrdeer (Heights Camp, após 3★; Mind Plate), Ursaluna (Sludge Mound; Earth
  Plate), Basculegion (Ginkgo Landing, Iscan; Splash Plate), Sneasler (Ingo; Toxic Plate), Hisuian Braviary (Sabi;
  Sky Plate).

## Plates (ordem de obtenção)

Mind (Wyrdeer), Insect (Noble Kleavor), Earth (Ursaluna), Meadow (Noble Lilligant), Splash (Basculegion), Flame
(Noble Arcanine), Toxic (Sneasler), Zap (Noble Electrode), Sky (Braviary), Icicle (Noble Avalugg), Stone (Alpha
Vespiquen, M20), Fist (Kamado, M25), Draco (trio dos lagos, M21), Iron (Heatran), Dread (Cresselia), Blank
(Regigigas), Pixie (Cogita, troca por 3 Wood, M26), Spooky (Volo, após Giratina) = 18; Legend (Arceus).

## TMs, lojas e itens-chave

Sem TMs nem HMs: golpes novos com Zisu (Move Tutor pago no Training Grounds; novos golpes com 3★ e 6★). Golpe de
tutor só ganha Strong/Agile Style com Seed of Mastery entregue a ela. Troca de golpes livre pelo menu.

- Itens-chave: Arc Phone; Celestica Flute (montarias; vira Azure Flute após Volo); Crafting Kit (Rei/Akari, M4);
  Odd Keystone (Vessa, Req22); Red Chain (Shrouded Ruins, após Mesprit's Plume, Azelf's Fang e Uxie's Claw); Origin
  Ore (Primeval Grotto) → Origin Ball; Adamant Crystal (Req89, Dialga Origin); Lustrous Globe (Req90, Palkia Origin);
  Griseous Core (Req91); Reveal Glass (Req94); Gracidea (Req92); Shiny Charm (Cyllene, após Arceus); Basculegion
  Food; Eternal Ice; Secret Medicine (Req32); Old Verse ×20 (enterrados, Ursaluna; colecionáveis).
- Recompensas dos espíritos (Req22): 5 Oval Stone; 10 Rare Candy; 20 Seed of Mastery; 30 Dusk Stone; 40 Exp. Candy L;
  50 Linking Cord; 60 5 Grit Pebble; 70 Reaper Cloth; 80 3 Seed of Mastery; 90 2 Exp. Candy XL; 100 3 Grit Rock; 107
  3 Twice-Spiced Radish + Spiritomb.
- Itens evolutivos em Requests: Linking Cord (Req64; também 50 espíritos); Moon Stone (Req67); Black Augurite
  (Req68); Leaf Stone (Req73, Req106); Shiny Stone (Req74); Dawn Stone (Req82, Req102); Sun Stone (Req85); Ice Stone
  (Req86, Req107); Adamant Mint + Modest Mint (Req72, 100 Medicinal Leeks).
- Pedras de evolução no mapa: Moss Rock (The Heartwood, Leafeon), Ice Rock (Icepeak Cavern, Glaceon).
- Lojas e serviços no Jubilife: General Store (Choy; estoque cresce com Req23 Heart Grains, Req43 Pop Pods, Req61
  Crunchy Salts, Req71 Sand Radishes); Craftworks (Anvin; vende receitas, novas a cada Mission; Max Revive depois do
  Avalugg; Sticky Glob); Ginkgo Guild (Ginter, promoções e os 5 itens Mechanical do Rotom por $20.000/40.000/60.000/
  80.000/100.000; Tuli, itens diferentes); Bonn (Exp. Candies, após o Noble Lilligant); Simona (Trading Post, MP);
  Lucille (Charms: Tempting, Survival, Warding, 5 cores cada); Bagin (espaço da bolsa, a partir de $100); Anthe
  (roupas; brindes com save de BDSP, SwSh e Let's Go); Edith e Arezu (cabelo, $500); Colza (Farm: Apricorns e plantas;
  vegetais e cogumelos após Req27; Berries após Req42; Mints após Req60; Berries especiais no Daybreak); Dagero (Photo
  Studio); Belamy (mostra a felicidade, Req28); Galaxy Member dos acampamentos (cura, loja, Pastures).
- Receitas de Rei/Akari: Smoke Bomb (após Kleavor), Stealth Spray (após rei-2), Scatter Bang (após Lilligant).

## Mecânicas (para o guia de mecânicas, texto nosso)

- Batalhas por "ordem de ação" (velocidade define quem age e quantas vezes); Strong Style (mais poder, age depois) e
  Agile Style (menos poder, age antes), cada um gasta 2 PP; só em golpes masterizados.
- Captura sem batalha (lançar a Poké Ball; de costas aumenta a chance) ou em batalha; comportamentos: passivo,
  assustado (foge) e agressivo (ataca e rebate a bola). Pokémon enviado é o selecionado no menu, não o primeiro do time.
- O jogador leva dano e pode desmaiar (perde itens); esquiva com (Y). Itens de atordoar: Ball of Mud, Spoiled
  Apricorn, Snowball, Sticky Glob (dobro do efeito).
- Evolução manual pelo menu; troca substituída pelo Linking Cord; itens de troca usados direto.
- Ciclo dia/noite de 24 min reais (manhã 7, dia 6, tarde 3, noite 8); a tenda e o leito avançam o período; climas:
  chuva, tempestade, nevasca, lua cheia.
- Alphas: olhos vermelhos, maiores, golpes extras; Seed of Mastery costuma vir deles.
- Noble Pokémon: barra de frenesi baixada com balms; quando atordoado, abre batalha normal.
- Mass Outbreaks (mais shinies; a partir da segunda visita) e, no Daybreak, Massive Mass Outbreaks em chuva forte
  (nevasca no Icelands), revelados dando Aguav Berries ao Munchlax da Mai.
- Distorções do espaço-tempo: aviso, 5 min para formar, duram ~4 min; Pokémon exclusivos, Shards, itens raros
  (Upgrade, Dubious Disc).
- Pokédex com níveis de pesquisa (lv 10 completa; tarefa vermelha +2); pontos de pesquisa dão dinheiro e Rank.
- Crafting (bancada nos acampamentos e no Craftworks, ou Crafting Kit); bolsa com espaço limitado e baú.
- Lost satchels (bolsas perdidas) → Merit Points; online também de outros jogadores.
- Corridas de balões: Req15 (Wyrdeer, 17 de 30 em 45 s), Req47 (Basculegion, 24 de 40 em 1min20), Req79 (todas,
  30 de 50 em 2 min), Req105 (Summit Camp, 40 de 60 em 1min50). Req26: tiro ao balão, 10.000 pontos em 1 min.

### Pós-jogo (ordem)

1. Créditos após M18 → M19 (Volo vira guia de lendas; Daybreak liberado; Req89–93; iniciais restantes; Ingo com
   Rei/Akari, Kamado, Beni e Ingo).
2. M20 Alpha Vespiquen (Stone Plate) → Cogita no Ancient Retreat abre M21–M25 (lagos, Heatran, Cresselia,
   Regigigas, Kamado na Prelude Beach).
3. M26 Pixie Plate (Cogita) → Volo no Sacred Plaza → batalha com Volo e Giratina no Temple of Sinnoh (Spooky Plate,
   Azure Flute).
4. Req91 Giratina, Req94 forças da natureza e Enamorus; completar a Pokédex (10★ com 60.000).
5. M27 Arceus → Shiny Charm; revanche dos Noble em lv 70.
6. Daybreak: Req95–102 (Massive Mass Outbreaks), novos modos do Ingo, Req103–107, Path of Solitude (Req108–122),
   Eternal Battle Reverie, eventos extras (Kamado e Cyllene à noite; médico viajante na Seaside Gate).

## Dúvidas

- Nível do inicial escolhido no começo não aparece no texto (só o lv 5 dos iniciais do pós-jogo).
- Rei/Akari na Req98 e na batalha regular do Daybreak: o texto diz "inicial de Sinnoh com desvantagem ao seu"; não
  fica claro se é um inicial de Hisui (Decidueye/Typhlosion/Samurott) ou de Sinnoh, nem a espécie exata.
- Path of Tenacity Galaxy Team 3ª partida e Diamond Clan 4ª partida: a tabela saiu embaralhada no texto; a divisão
  dos Pokémon entre os três treinadores foi reconstruída pela ordem e precisa de conferência no PDF.
- Arena do Arcanine: chamada de "Molten Arena" na história e de "Magma Arena" na revanche.
- Req39 (Magikarp) pede, no texto, pesquisa lv 10 de "Roselia" (provável erro, deveria ser Magikarp).
- Nível do Noble Pokémon serve como "cap"; o jogo não tem limite de obediência por nível citado no detonado.
- Listas de Pokémon dos desafios do Eternal Battle Reverie e mapas dos Unown/espíritos só aparecem em imagens.
- Times das Miss Fortune Sisters que aparecem soltas pelos mapas após a Sneasler não são listados.
- Shaymin, Darkrai: o detonado exige save de SwSh/BDSP; Manaphy/Phione sem requisito externo.
