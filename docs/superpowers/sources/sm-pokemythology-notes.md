# Sun / Moon — fatos extraídos do detonado da Pokémon Mythology

Fonte: https://www.pokemythology.net/detonados/detonado-sun-moon/ (PDF salvo em 05/11/2026, 147 páginas).
Só **fatos de jogo** (chefes, níveis, encontros, itens) no nosso formato. Nenhuma prosa foi copiada. Golpes dos
chefes **não** constam no detonado: deixar `moves: []` e `verified: false` para golpes; espécies e níveis são
`verified: true` (uma fonte) até o analista conferir numa segunda. Exclusivos: `[Sun]` e `[Moon]`.

## Ordem da história e chefes (para `bosses/sm.json`)

Variantes por inicial escolhido (Rowlet / Litten / Popplio): Hau leva o inicial **fraco** ao seu (Rowlet→Popplio,
Litten→Rowlet, Popplio→Litten) e a eeveelution forte ao seu (Rowlet→Flareon, Litten→Vaporeon, Popplio→Leafeon);
Kukui e o Silvally do Gladion têm o tipo **forte** ao seu inicial.

| Order | Id | Kind | Nome | Local | Time (espécie lv) | Cap |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | hau-1 | rival | Hau | Route 1 (perto de Iki Town) | inicial 5 (Popplio/Rowlet/Litten) | 5 |
| 2 | hau-2 | rival | Hau (festival) | Iki Town | Pichu 6, inicial 7 | 7 |
| 3 | ilima-1 | other | Ilima (teste antes da trial) | Hau'oli City | Yungoos 9, Smeargle 10 | 10 |
| 4 | totem-gumshoos | gym | Totem Gumshoos [Sun] / Totem Alolan Raticate [Moon] (trial do Ilima, Normalium Z) | Verdant Cavern | Totem 12 + aliado Yungoos [Sun] / Alolan Rattata [Moon] 11 | 12 |
| 5 | hau-3 | rival | Hau | saída da Melemele Meadow (Route 3) | Pikachu 13, inicial 14 | 14 |
| 6 | hala-1 | gym | Hala (grand trial, Fightinium Z) | Iki Town | Mankey 14, Makuhita 14, Crabrawler 15 | 15 |
| 7 | ilima-2 | other | Ilima (revanche) | Trainers' School | Gumshoos 15, Smeargle 14 | 15 |
| 8 | dexio-sina-1 | other | Dexio [Sun] / Sina [Moon] | Heahea City (fonte) | Sun: Slowpoke 15, Espeon 16; Moon: Delibird 15, Glaceon 16 | 16 |
| 9 | hau-4 | rival | Hau | Paniola Town | Pikachu 16, inicial evoluído 17 (Brionne/Dartrix/Torracat) | 17 |
| 10 | gladion-1 | rival | Gladion | Route 5 (Paniola Ranch acima) | Zubat 17, Type: Null 18 | 18 |
| 11 | totem-wishiwashi | gym | Totem Wishiwashi (School Form) (trial da Lana, Waterium Z) | Brooklet Hill | Totem 20 + Alomomola 18, Wishiwashi 18 | 20 |
| 12 | battle-royal-1 | other | Battle Royal (Pro Wrestler The Royal, Hau, Gladion) | Battle Royal Dome, Royal Avenue | The Royal: Rockruff 19; Hau: inicial evoluído 19; Gladion: Type: Null 19 | 19 |
| 13 | totem-salazzle | gym | Totem Salazzle (trial do Kiawe, Firium Z) | Wela Volcano Park | Totem 22 + Salandit 20 | 22 |
| 14 | totem-lurantis | gym | Totem Lurantis (trial da Mallow, Grassium Z) | Lush Jungle | Totem 24 + Castform 22, Trumbeak 22 | 24 |
| 15 | plumeria-1 | evil | Plumeria (Team Skull) | Memorial Hill | Golbat 25, Salandit 26 | 26 |
| 16 | olivia-1 | gym | Olivia (grand trial, Rockium Z) | Ruins of Life (entrada), Akala Outskirts | Nosepass 26, Boldore 26, Lycanroc 27 | 27 |
| 17 | lana-captain | other | Lana (opcional) | casa ao norte de Konikoni City | Chinchou 26, Shellder 26, Araquinid 27 | 27 |
| 18 | kiawe-captain | other | Kiawe [Sun] (opcional, à noite) | Thrifty Megamart, Royal Avenue | Growlithe 26, Fletchinder 26, Alolan Marowak 27 | 27 |
| 19 | mallow-captain | other | Mallow [Moon] (opcional, de dia) | Lush Jungle | Phantump 26, Shiinotic 26, Steenee 27 | 27 |
| 20 | nihilego-1 | other | Ultra Beast (Nihilego, só derrotar) | Aether Paradise | sem nível listado | — |
| 21 | hau-5 | rival | Hau | Malie City (chegada) | Rowlet: Alolan Raichu 28, Flareon 28, Brionne 29; Litten: Alolan Raichu 28, Vaporeon 28, Dartrix 29; Popplio: Alolan Raichu 28, Leafeon 28, Torracat 29 | 29 |
| 22 | molayne-1 | other | Molayne | Hokulani Observatory | Skarmory 29, Metang 29, Alolan Dugtrio 30 | 30 |
| 23 | totem-vikavolt | gym | Totem Vikavolt (trial do Sophocles, Electrium Z; Molayne dá Steelium Z) | Hokulani Observatory | Totem 29 + Charjabug 27 | 29 |
| 24 | guzma-1 | evil | Guzma (Team Skull) | Malie Garden | Golisopod 31, Ariados 30 | 31 |
| 25 | totem-mimikyu | gym | Totem Mimikyu (trial da Acerola, Ghostium Z) | Abandoned Thrifty Megamart | Totem 33 + Haunter 27, Gengar 27 | 33 |
| 26 | plumeria-2 | evil | Plumeria | Aether House (Route 15) | Golbat 34, Salazzle 35 | 35 |
| 27 | guzma-2 | evil | Guzma (Buginium Z no baú) | Shady House, Po Town | Golisopod 37, Ariados 36 | 37 |
| 28 | gladion-2 | rival | Gladion | Aether House | Golbat 37, Sneasel 37, Type: Null 38 | 38 |
| 29 | nanu-1 | gym | Nanu (grand trial, Darkinium Z) | Ferry Terminal, Malie City | Sableye 38, Krokorok 38, Alolan Persian 39 | 39 |
| 30 | faba-1 | evil | Faba (Aether Foundation) | Aether Paradise | Hypno 39 | 39 |
| 31 | guzma-3 | evil | Guzma | Aether Paradise | Golisopod 41, Ariados 40, Masquerain 40, Pinsir 40 | 41 |
| 32 | lusamine-1 | evil | Lusamine | Aether Paradise | Clefable 41, Lilligant 41, Mismagius 41, Milotic 41, Bewear 41 | 41 |
| 33 | hapu-1 | gym | Hapu (grand trial, Groundium Z) | Ancient Poni Path | Alolan Dugtrio 47, Gastrodon 47, Flygon 47, Mudsdale 48 | 48 |
| 34 | totem-kommo-o | gym | Totem Kommo-o (Dragonium Z, sem captain) | Vast Poni Canyon | Totem 45 + Scizor 32, Hakamo-o 32 | 45 |
| 35 | lusamine-2 | evil | Lusamine (fundida a Nihilego, aura Totem) | Ultra Space | Clefable 50, Lilligant 50, Mismagius 50, Milotic 50, Bewear 50 | 50 |
| 36 | solgaleo-lunala | other | Solgaleo [Sun] / Lunala [Moon] (captura) | Altar of the Sunne / Altar of the Moone | 55 | — |
| 37 | gladion-3 | rival | Gladion | Mount Lanakila (entrada) | Golbat 52, Weavile 52, Lucario 52, Silvally 53 | 53 |
| 38 | hau-6 | rival | Hau | Mount Lanakila (perto da Liga) | Rowlet: Alolan Raichu 53, Flareon 53, Komala 53, Primarina 54; Litten: Alolan Raichu 53, Vaporeon 53, Komala 53, Decidueye 54; Popplio: Alolan Raichu 53, Leafeon 53, Komala 53, Incineroar 54 | 54 |
| 39 | hala-elite | elite | Hala | Pokémon League (1ª sala) | Hariyama 54, Primeape 54, Bewear 54, Poliwrath 54, Crabominable 55 | 55 |
| 40 | olivia-elite | elite | Olivia | Pokémon League (2ª sala) | Relicanth 54, Carbink 54, Alolan Golem 54, Probopass 54, Lycanroc 55 | 55 |
| 41 | acerola-elite | elite | Acerola | Pokémon League (3ª sala) | Sableye 54, Drifblim 54, Dhelmise 54, Froslass 54, Palossand 55 | 55 |
| 42 | kahili-elite | elite | Kahili | Pokémon League (4ª sala) | Skarmory 54, Crobat 54, Oricorio 54, Mandibuzz 54, Toucannon 55 | 55 |
| 43 | kukui-champion | champion | Professor Kukui | Pokémon League | Lycanroc 57, Alolan Ninetales 56, Braviary 56, Magnezone 56, Snorlax 56, inicial forte ao seu 58 (Rowlet→Incineroar, Litten→Primarina, Popplio→Decidueye) | 58 |
| 44 | tapu-koko | other | Tapu Koko (captura) | Ruins of Conflict | 60 | — |
| 45 | anabel-1 | other | Anabel | Akala Motel, Route 8 | Alakazam 61, Weavile 61, Mismagius 61, Salamence 61, Snorlax 61 | 61 |
| 46 | faba-2 | other | Faba | Aether Paradise 1F | Slowbro 61, Bruxish 61, Alolan Raichu 61, Alakazam 61, Hypno 61 | 61 |
| 47 | mina-1 | other | Mina | Akala Motel, Route 8 | Klefki 61, Granbull 61, Shiinotic 61, Wigglytuff 61, Ribombee 61 | 61 |
| 48 | nanu-2 | other | Nanu | floating restaurant, Seafolk Village | Sableye 63, Absol 63, Honchkrow 63, Krookodile 63, Alolan Persian 63 | 63 |
| 49 | dexio-2 | other | Dexio (Key Stone e Alakazite) | Ancient Poni Path → Poni Grove | Espeon 61, Alolan Raichu 61, Slowking 61, Metagross 61, Alakazam 61 (pode Mega) | 61 |
| 50 | guzma-4 | other | Guzma (revanche) | Hau'oli City Beachfront | Golisopod 63, Ariados 63, Pinsir 63, Masquerain 63, Scizor 63 | 63 |
| 51 | morimoto | other | Morimoto (1×/dia) | prédio da Game Freak, Heahea City | Kangaskhan 62, Jolteon 62, Flareon 62, Vaporeon 62, Machamp 62, Dragonite 62 | 62 |
| 52 | mina-2 | other | Mina (após todos da área) | Poni Gauntlet | Klefki 61, Granbull 61, Shiinotic 61, Wigglytuff 61, Ribombee 61 | 61 |
| 53 | red-entry | other | Red (escolha, entrada da Battle Tree) | Battle Tree | Pikachu 70, Lapras 65, Snorlax 65, Venusaur 65, Charizard 65, Blastoise 65 | 70 |
| 54 | blue-entry | other | Blue (escolha, entrada da Battle Tree) | Battle Tree | Alakazam 65, Machamp 65, Aerodactyl 65, Exeggutor 65, Arcanine 65, Gyarados 65 | 65 |
| 55 | hala-elite-2 | elite | Hala (revanche) | Pokémon League | Hariyama 63, Primeape 63, Bewear 63, Poliwrath 63, Crabominable 63 | 63 |
| 56 | olivia-elite-2 | elite | Olivia (revanche) | Pokémon League | Relicanth 63, Carbink 63, Alolan Golem 63, Probopass 63, Lycanroc 63 | 63 |
| 57 | acerola-elite-2 | elite | Acerola (revanche) | Pokémon League | Sableye 63, Drifblim 63, Dhelmise 63, Froslass 63, Palossand 63 | 63 |
| 58 | kahili-elite-2 | elite | Kahili (revanche) | Pokémon League | Skarmory 63, Crobat 63, Oricorio 63, Mandibuzz 63, Toucannon 63 | 63 |
| 59 | defense-hau | champion | Hau (defesa do título) | Pokémon League | Rowlet: Alolan Raichu 63, Flareon 63, Komala 63, Crabominable 63, Primarina 63; Litten: Alolan Raichu 63, Vaporeon 63, Komala 63, Crabominable 63, Decidueye 63; Popplio: Alolan Raichu 63, Leafeon 63, Komala 63, Crabominable 63, Incineroar 63 | 63 |
| 60 | defense-gladion | champion | Gladion (defesa) | Pokémon League | Golbat 63, Weavile 63, Porygon-Z 63, Lucario 63, Silvally 63 | 63 |
| 61 | defense-faba | champion | Faba (defesa; só no dia 1 de cada mês) | Pokémon League | Slowbro 61, Bruxish 61, Alolan Raichu 61, Alakazam 61, Hypno 61 | 61 |
| 62 | defense-plumeria | champion | Plumeria (defesa) | Pokémon League | Gengar 61, Alolan Muk 61, Crobat 61, Toxapex 61, Salazzle 61 | 61 |
| 63 | defense-sophocles | champion | Sophocles (defesa) | Pokémon League | Togedemaru 61, Magnezone 61, Electivire 61, Vikavolt 61, Alolan Golem 61 | 61 |
| 64 | defense-molayne | champion | Molayne (defesa) | Pokémon League | Skarmory 61, Alolan Sandslash 61, Magnezone 61, Metagross 61, Alolan Dugtrio 61 | 61 |
| 65 | defense-hapu | champion | Hapu (defesa) | Pokémon League | Alolan Dugtrio 63, Gastrodon 63, Krookodile 63, Flygon 63, Mudsdale 63 | 63 |
| 66 | defense-ryuki | champion | Ryuki (defesa) | Pokémon League | Garchomp 61, Drampa 61, Turtonator 61, Dragonite 61, Kommo-o 61 | 61 |
| 67 | defense-tristan | champion | Tristan (defesa) | Pokémon League | Emolga 59, Sharpedo 59, Alolan Exeggutor 59, Magmortar 59, Tauros 59 | 59 |
| 68 | defense-kukui | champion | Professor Kukui (defesa) | Pokémon League | Lycanroc 65, Alolan Ninetales 65, Braviary 65, Magnezone 65, Snorlax 65, inicial forte ao seu 65 | 65 |

Outros combates sem time no detonado: grunts da Team Skull (Hau'oli, Berry Fields, Verdant Cavern, Diglett's Tunnel em
dupla com Hau, Memorial Hill, Route 10, Tapu Village, Po Town, Shady House, Ancient Poni Path); Meredith (Oricorio
Pom-Pom na Melemele Meadow, Pa'u na Royal Avenue, Baile na Ula'ula Meadow, Sensu na Poni Meadow); duplas na Aether
Paradise (com Hau e com Gladion; Faba + membro em dupla); treinadores das eeveelutions (lv 55 cada) e o tio do
Thrifty Megamart com Eevee (Eevium Z); treinador do Pokémon Center de Seafolk (6 Nuggets); duas garotas do Poni
Breaker Coast (TMs Surf e Waterfall).

Battle Tree (lv 50 fixo; Battle Legend após 20 vitórias; 50 vitórias reabre Red/Blue):
- Red (Single/Multi): Venusaur, Charizard, Blastoise, Lapras, Snorlax (50). Blue (Double/Multi): Pidgeot, Alakazam, Machamp, Aerodactyl, Exeggutor, Arcanine, Gyarados, Tyranitar, Rhyperior (50).
- Especiais no modo Super (1 a cada 10 batalhas): Wally (Altaria, Garchomp, Magnezone, Gallade); Cynthia (Milotic, Spiritomb, Garchomp, Lucario, Togekiss); Anabel (Alakazam, Snorlax, Raikou, Entei, Latios, Lucario); Colress (Alolan Muk, Electrode, Porygon2, Metagross, Magnezone, Porygon-Z, Klinklang); Grimsley (Houndoom, Tyranitar, Sharpedo, Absol, Honchkrow, Drapion, Liepard, Scrafty, Bisharp); Plumeria [Sun] / Guzma [Moon], Sina [Sun] / Dexio [Moon], Kiawe [Sun] / Mallow [Moon] (times misturados no texto, ver Dúvidas).
- Prêmios por vitórias seguidas: 5 Moomoo Milk; 10 PP Up; 20 Rare Candy; 30 Bottle Cap; 40 PP Max; 50 Ability Capsule; 100 Lansat Berry; 200 Starf Berry. 1ª vitória sobre Red: Mega Stones dos iniciais de Kanto.
- Multi sem amigo: parceira Rada (Barbaracle, Hawlucha); "Scout" por 10 BP recruta treinadores derrotados no Super (menos Red e Blue).

## Áreas (para `areas/sm.json`, kind e ordem de liberação)

Números de rota só quando o detonado os cita (Routes 1, 2, 3, 8, 13, 15, 17); os demais estão em Dúvidas.

| Id | Nome | Kind | Liberada em |
| --- | --- | --- | --- |
| player-house | Casa do jogador (Route 1) | building | início |
| iki-town | Iki Town | city | início |
| mahalo-trail | Mahalo Trail | route | início |
| ruins-of-conflict | Ruins of Conflict | dungeon | após a Liga (ponte reconstruída) |
| route-1 | Route 1 (área do festival; área do Tauros depois) | route | após o inicial |
| kukui-lab | Laboratório do Kukui (Route 1, sul) | building | após o festival |
| trainers-school | Trainers' School | building | após o laboratório |
| hauoli-city | Hau'oli City (Tourist Bureau, Marina, Mall, Beachfront) | city | após a Trainers' School |
| route-2 | Route 2 (Hau'oli Cemetery, Berry Fields, Melemele Motel) | route | após Ilima |
| hauoli-cemetery | Hau'oli Cemetery | zone | após Ilima |
| verdant-cavern | Verdant Cavern | dungeon | após Route 2 (trial do Ilima) |
| route-3 | Route 3 | route | após Verdant Cavern (barricada do Ilima) |
| melemele-meadow | Melemele Meadow | zone | após Verdant Cavern |
| seaward-cave | Seaward Cave | dungeon | Melemele Meadow (completa com Lapras Paddle) |
| ten-carat-hill | Ten Carat Hill | dungeon | após Hala (Tauros Charge; Machamp para o Flyinium Z) |
| melemele-sea | Melemele Sea | sea | com Sharpedo Jet |
| heahea-city | Heahea City (Dimensional Research Lab, Tidal Song Hotel, prédio da Game Freak, Ferry Terminal) | city | barco após Hala |
| route-4 | Route 4 (Heahea→Paniola) | route | Akala |
| paniola-town | Paniola Town | city | Akala |
| paniola-ranch | Paniola Ranch (Route 5; Pokémon Nursery) | route | após Paniola Town |
| brooklet-hill | Brooklet Hill | zone | após Gladion (trial da Lana) |
| route-6 | Royal Avenue sul (Sudowoodo, Hapu) | route | após Lana |
| royal-avenue | Royal Avenue (Battle Royal Dome, Thrifty Megamart) | city | após Lana |
| route-7 | Rota ao norte da Royal Avenue (mar) | route | após Battle Royal |
| wela-volcano-park | Wela Volcano Park | zone | após Battle Royal (trial do Kiawe) |
| route-8 | Route 8 (Akala Motel; barricada liberada pelo Kiawe) | route | após Kiawe |
| lush-jungle | Lush Jungle | zone | após Kiawe (trial da Mallow) |
| dimensional-research-lab | Dimensional Research Lab | building | após Mallow |
| diglett-tunnel | Diglett's Tunnel | dungeon | após Mallow |
| route-9 | Saída do Diglett's Tunnel (pesca) | route | após Diglett's Tunnel |
| memorial-hill | Memorial Hill | zone | após Diglett's Tunnel |
| akala-outskirts | Akala Outskirts | route | após Memorial Hill |
| ruins-of-life | Ruins of Life | dungeon | após a Liga (Tapu Lele) |
| konikoni-city | Konikoni City | city | após Diglett's Tunnel |
| hano-grand-resort | Hano Grand Resort e Hano Beach | zone | após Olivia |
| aether-paradise | Aether Paradise | building | após Olivia (barco do Faba) |
| malie-city | Malie City (Library, Community Center, Ferry Terminal) | city | Ula'ula |
| malie-garden | Malie Garden | zone | Ula'ula |
| malie-outer-cape | Malie City Outer Cape | route | Ula'ula |
| route-10 | Rota à esquerda de Malie (ponto de ônibus) | route | Ula'ula |
| mount-hokulani | Mount Hokulani e Hokulani Observatory | zone | ônibus da Route 10 |
| route-11-hokulani-road | Estrada do ônibus (descida do Hokulani) | route | após Sophocles |
| route-12 | Rota ao sul de Malie (Komala; terreno rochoso com Mudsdale) | route | após Guzma (barricada) |
| blush-mountain | Blush Mountain (Geothermal Power Plant) | zone | após Mudsdale Gallop |
| secluded-shore | Secluded Shore | zone | após Blush Mountain |
| route-13 | Route 13 (Ula'ula Motel) | route | após Blush Mountain |
| tapu-village | Tapu Village | city | após Blush Mountain |
| route-14 | Rota ao sul da Tapu Village (Abandoned Thrifty Megamart) | route | após Tapu Village |
| abandoned-thrifty-megamart | Abandoned Thrifty Megamart | building | trial da Acerola |
| route-15 | Route 15 (Aether House) | route | após Tapu Village |
| haina-desert | Haina Desert | zone | após Acerola |
| ruins-of-abundance | Ruins of Abundance | dungeon | fim do Haina Desert (Tapu Bulu após a Liga) |
| route-16-17 | Rotas pela água até Po Town (Sharpedo Jet, Reassembly Unit) e Route 17 | route | após Plumeria 2 |
| ulaula-meadow | Ula'ula Meadow | zone | após Sharpedo Jet |
| lake-of-the-sunne-moone | Lake of the Sunne / Lake of the Moone | zone | Ula'ula Meadow (Cosmog no pós-jogo) |
| po-town | Po Town e Shady House | city | após Ula'ula Meadow |
| seafolk-village | Seafolk Village | city | Poni (barco da Aether Paradise) |
| poni-wilds | Poni Wilds | route | Poni |
| ancient-poni-path | Ancient Poni Path | route | Poni |
| poni-breaker-coast | Poni Breaker Coast | route | Poni |
| ruins-of-hope | Ruins of Hope | dungeon | Poni (Tapu Fini após a Liga) |
| exeggutor-island | Exeggutor Island | zone | após Ruins of Hope (barco) |
| vast-poni-canyon | Vast Poni Canyon | dungeon | após Hapu |
| altar-sunne-moone | Altar of the Sunne / Altar of the Moone | zone | após Vast Poni Canyon |
| ultra-space | Ultra Space | zone | história (Solgaleo/Lunala) |
| mount-lanakila | Mount Lanakila | dungeon | após Solgaleo/Lunala |
| pokemon-league | Pokémon League | building | Mount Lanakila |
| poni-grove | Poni Grove | route | pós-jogo |
| poni-plains | Poni Plains | route | pós-jogo |
| poni-meadow | Poni Meadow | zone | pós-jogo |
| resolution-cave | Resolution Cave | dungeon | pós-jogo |
| poni-coast | Poni Coast | route | pós-jogo |
| poni-gauntlet | Poni Gauntlet | route | pós-jogo |
| battle-tree | Battle Tree | building | pós-jogo |

## Encontros (para `encounters/sm.json`; method `walk` salvo indicação; `conditions`: day/night)

- Route 1: Pikipek, Caterpie; Yungoos (dia), Ledyba (dia), Alolan Rattata (noite), Spinarak (noite); mais adiante Grubbin, Pichu. Sul (perto do laboratório): Slowpoke, Wingull. Área do Tauros: Bonsly, Munchlax.
- Trainers' School: Magnemite, Alolan Meowth, Alolan Grimer.
- Hau'oli City (portões com grama): Abra.
- Route 2: Makuhita, Drowzee, Cutiefly, Growlithe, Spearow. Berry Fields: Crabrawler (árvore de berry, `method: tree`).
- Hau'oli Cemetery: Gastly, Zubat, Drifloon (dia), Misdreavus (noite).
- Verdant Cavern: Zubat, Alolan Diglett (`method: cave`); Yungoos [Sun] / Alolan Rattata [Moon] (nuvens de poeira).
- Route 3: Delibird, Mankey; sombras: Spearow, Rufflet [Sun], Vullaby [Moon].
- Melemele Meadow: Cottonee [Sun], Petilil [Moon] (flores); Oricorio Pom-Pom Style.
- Ten Carat Hill: Roggenrola, Carbink (`method: cave`); Sableye (`method: sos`); área externa: Rockruff, Machop, Spinda. Necrozma lv 75 (pós-jogo, ver lendários).
- Route 4: Lillipup, Mudbray, Igglybuff, Eevee.
- Paniola Ranch: Tauros, Miltank; parte de cima Fomantis.
- Brooklet Hill: Poliwag; Paras (dia), Dewpider (dia); Morelull (noite), Surskit (noite). Wishiwashi nas ondulações.
- Royal Avenue sul: Oricorio Pa'u Style.
- Norte da Royal Avenue (Lapras): Finneon, Tentacool, Pyukumuku (`method: surf`); pesca: Magikarp, Wishiwashi, Staryu (`method: fish`).
- Wela Volcano Park: Fletchling, Salandit, Magby, Cubone, Kangaskhan (raro).
- Route 8: Stufful (raro); Wimpod (foge para a toca); Chinchou (pesca, raro).
- Lush Jungle: Fomantis, Comfey, Passimian [Sun], Oranguru [Moon].
- Saída do Diglett's Tunnel: Luvdisc, Corsola (pesca); Mareanie (`method: sos`).
- Memorial Hill: Phantump. Akala Outskirts: Nosepass.
- Hano Beach: Staryu, Sandygast (sombras na areia).
- Malie City sul: Komala. Rota à esquerda de Malie: Fearow, Skarmory, Pancham.
- Estrada do Hokulani: Beldum, Minior, Ditto; Cleffa (noite).
- Terreno rochoso (sul de Malie): Alolan Geodude, Mudbray, Torkoal, Elekid. Blush Mountain: Togedemaru; Turtonator [Sun].
- Tapu Village (lago): Bruxish (pesca, raro). A oeste: Snorunt, Absol, Alolan Vulpix [Sun], Alolan Sandshrew [Moon].
- Abandoned Thrifty Megamart (após a trial): Klefki, Mimikyu.
- Haina Desert: Sandile, Trapinch (dia com Sunny Day, noite com Sandstorm).
- Ula'ula Meadow: Oricorio Baile Style.
- Poni Wilds: Granbull, Exeggcute, Gastrodon; Lapras (surf, raro).
- Exeggutor Island: Alolan Exeggutor (um ataca na chegada; outros na grama).
- Vast Poni Canyon: Jangmo-o (parte alta, raro).
- Mount Lanakila: Sneasel; Drampa [Moon] (caverna).
- Poni Grove: Riolu, Pinsir. Poni Meadow: Oricorio Sensu Style.

Lendários, Ultra Beasts e fixos (`method: fixed`):

| Espécie | Nível | Local | Condição |
| --- | --- | --- | --- |
| Solgaleo [Sun] / Lunala [Moon] | 55 | Altar of the Sunne / Moone | história |
| Tapu Koko | 60 | Ruins of Conflict | após a Liga |
| Tapu Lele | 60 | Ruins of Life | pós-jogo; Machamp para os blocos |
| Tapu Bulu | 60 | Ruins of Abundance | pós-jogo |
| Tapu Fini | 60 | Ruins of Hope | pós-jogo |
| Cosmog | 5 | Lake of the Sunne / Moone (mundo alternativo) | Solgaleo/Lunala na equipe; Solgaleo passa só à noite, Lunala só de dia; sem batalha |
| Nihilego | 55 | Wela Volcano Park ou Diglett's Tunnel (1, alterna) | missão UB-01 |
| Buzzwole [Sun] ×2 | 65 | Melemele Meadow | missão UB-02 |
| Pheromosa [Moon] ×4 | 60 | Verdant Cavern | missão UB-02 |
| Xurkitree ×2 | 65 | Lush Jungle ou Memorial Hill | missão UB-03 |
| Kartana [Sun] ×4 | 60 | Route 17 e Malie Garden | missão UB-04 |
| Celesteela [Moon] ×2 | 65 | Haina Desert e Malie Garden | missão UB-04 |
| Guzzlord ×1 | 70 | Resolution Cave | missão UB-05 |
| Necrozma | 75 | Ten Carat Hill (grama) | após as missões das UBs |
| Zygarde 10% / 50% | — | Reassembly Unit (Route 16) | 10 / 50 Zygarde Cells |

## Presentes, trocas in-game, fósseis, ovos e iniciais (`method: gift` / `trade`)

- Inicial: Rowlet/Litten/Popplio (Hala, Mahalo Trail; nível não citado).
- Ovo de Eevee (balconista do Pokémon Nursery, Paniola Ranch).
- Aerodactyl (pessoa em Seafolk Village; nível não citado).
- Porygon lv 30 (cientista da Aether House, Route 15, pós-jogo).
- Type: Null + todos os drives (memórias) do Silvally (Gladion, Aether Paradise 2F, pós-jogo).
- Cosmog lv 5 (ver lendários).
- Fósseis à venda na loja da Olivia, Konikoni City: Skull Fossil e Cover Fossil [Sun]; Armor Fossil e Plume Fossil [Moon].
- Trocas in-game: o detonado não cita.

## Z-Crystals (local)

| Z-Crystal | Local / condição |
| --- | --- |
| Normalium Z | Verdant Cavern (trial do Ilima) |
| Fightinium Z | Hala (grand trial, Iki Town) |
| Waterium Z | Lana (Brooklet Hill) |
| Firium Z | Kiawe (Wela Volcano Park) |
| Grassium Z | Mallow (Lush Jungle) |
| Rockium Z | Olivia (grand trial) |
| Electrium Z | Sophocles (Hokulani Observatory) |
| Steelium Z | Molayne (Hokulani Observatory) |
| Decidium Z / Incinium Z / Primarium Z | Kukui, Malie Garden (do seu inicial); Hau, pós-jogo (os outros dois) |
| Ghostium Z | Acerola (Abandoned Thrifty Megamart) |
| Psychium Z | pedestal no Haina Desert |
| Buginium Z | baú na Shady House, Po Town |
| Darkinium Z | Nanu (grand trial) |
| Flyinium Z | pedestal no Ten Carat Hill (Machamp Shove) |
| Poisonium Z | Plumeria, Ancient Poni Path |
| Groundium Z | Hapu (grand trial) |
| Fairium Z | Mina, Vast Poni Canyon |
| Dragonium Z | Vast Poni Canyon (Totem Kommo-o) |
| Icium Z | pedestal no Mount Lanakila |
| Eevium Z | tio do Thrifty Megamart (após os 8 treinadores de eeveelutions) |

## TMs, Poké Ride e itens-chave (local)

- Poké Ride (Ride Pager): Tauros Charge (Hala, Iki Town; quebra pedras); Stoutland Search (Mallow, Paniola Ranch; acha itens); Lapras Paddle (Lana, Brooklet Hill; surf); Charizard Glide (Kiawe, Wela Volcano Park; voo); Mudsdale Gallop (Hapu, sul de Malie; terreno rochoso); Sharpedo Jet (Grimsley, praia perto da Aether House; quebra pedras na água, sem pesca); Machamp Shove (avó da Hapu, Ancient Poni Path; empurra blocos).
- TMs: TM01 (treinador do campinho, Trainers' School); TM43 (Colress, Route 8); TM67 (Kukui, Lush Jungle); TM no Melemele Sea (Sharpedo, pedras ao sul do laboratório); TM Surf e TM Waterfall (duas garotas, Poni Breaker Coast, pós-jogo); TM do Kukui em casa (pós-jogo); TM da Kahili (Hano Grand Resort Hotel, pós-jogo). Nomes de TM01/43/67 e dos outros não citados.
- Itens-chave: Pokédex e Trainer Passport (Kukui/Hala); Z-Ring (Hala, festival); Rotom Dex (laboratório); Poké Finder (Tourist Bureau, Hau'oli); Lens Case (Lillie, Hau'oli); Exp. Share (Kukui, Trainers' School); Zygarde Cube (Dexio/Sina, Heahea); Fishing Rod (Lana); Forage Bag (Mallow); Professor's Mask (Molayne → Kukui); Sun Flute / Moon Flute (Lillie e Exeggutor Island); Master Ball (Gladion, Aether Paradise); Enigmatic Card (senhor na saída de casa, pós-jogo); Beast Balls ×10 por missão (Wicke e Looker); Key Stone (Dexio, pós-jogo).
- Itens do pós-jogo: DNA Splicers, Prison Bottle e Soul Dew (membro da Aether, B2F); Dubious Disc (Faba); Up-Grade (Aether House); Protector (casa à esquerda do Pokémon Center de Paniola); Sun Stone [Sun] / Moon Stone [Moon] (Haina Desert, mostrar Solrock/Lunatone); Lucky Egg (Kukui, 50 capturados); Dawn Stone (Guzma); Drives de Genesect (Colress, Route 8); Alakazite (Dexio); Oval Charm (Morimoto, 1ª vitória); Shiny Charm (Game Director, Alola Dex completa exceto Magearna e Marshadow); 1.000.000 de Pokédollars (Looker); Plates de Arceus (loja de antiguidades do Hau'oli Mall e escondidas na Route 1, Hau'oli City e Hau'oli Cemetery).
- Outros: Reveal Glass (Burnet, Dimensional Research Lab); Big Nugget (time de treinadores na ponte da Malie Garden); bolas de Johto (Samson: Malie Library mostrando Alolan Persian, Outer Cape, Mount Hokulani, Geothermal Power Plant); Oran Berry (velho, Berry Fields); 3 Revives (Hau, Route 2 e Paniola Ranch); 3 Max Revives (Hau, Mount Lanakila); bolas de recompensa das trials: 10 Great Balls (Ilima), 10 Dive Balls (Lana), 10 Quick Balls (Kiawe), 10 Nest Balls (Mallow), 10 Dusk Balls (Acerola), 5 Luxury Balls (Lillie).
- Lojas e serviços: Konikoni City (loja da Olivia: Fire/Water/Thunder/Leaf Stone e fósseis); refeições com kahunas (Konikoni "special Z-set" com Olivia, 4 Heart Scales/dia; Malie "Z-Kaiseki: Ronin Set" com Nanu, 8/dia; Seafolk "special Z-set" com Hapu, 2/dia); Move Reminder (Pokémon Center do Mount Lanakila, por Heart Scale); Mr. Hyper / Hyper Training (Hau'oli City Mall, pós-jogo); Pyukumuku em Hano Beach ($20000/dia); Stufful da senhora (rota à esquerda de Malie, $15000).

## Mecânicas (para o guia de mecânicas, texto nosso)

- Island Challenge: 4 ilhas, trials dos captains com Totem Pokémon (aura que sobe status e chama aliados) e grand trial do kahuna; carimbos no Trainer Passport.
- Z-Moves: Z-Ring + Z-Crystal; 1 por batalha. Mega Evolução só no pós-jogo (Key Stone do Dexio).
- SOS: pokémon selvagens chamam aliados (ex.: Sableye no Ten Carat Hill, Mareanie na pesca).
- Grama que se mexe, sombras de voadores, nuvens de poeira em cavernas e ondulações na água revelam encontros especiais.
- Poké Ride substitui as HMs. Poké Pelago (Poké Beans, berries, itens) liberado após o Kiawe. Pokémon Refresh no lugar do Pokémon-Amie.
- Pokémon Nursery cria ovos sem dar experiência.
- Ciclo dia/noite; Haina Desert com Sunny Day de dia e Sandstorm à noite; mundo alternativo pelo ultra wormhole do altar inverte o horário.
- Moss Rock (Lush Jungle) → Leafeon; Icy Rock (Mount Lanakila) → Glaceon; Vast Poni Canyon evolui Magneton, Nosepass e Charjabug; Crabrawler evolui no Mount Lanakila.
- Zygarde Cells espalhadas; Reassembly Unit junta (10 = 10%, 50 = 50%) ou separa.
- Battle Royal (Royal Avenue): 4 treinadores, cada um por si, até alguém ficar sem pokémon.
- Battle Tree: lv 50, sem itens da mochila, sem repetir espécie nem item; Single (3), Double (4), Multi (2); BP por vitória.
- Defesa do título: após cada vitória na Liga, um desafiante aleatório (Faba só no dia 1 do mês); volta à entrada sem créditos.
- Poké Finder: fotos em pontos marcados. Festival Plaza com rank. Treinadores que só lutam após todos os outros da área (dão item).

## Pós-jogo (ordem)

1. Festa em Iki Town → Tapu Koko (Ruins of Conflict) → partida da Lillie e créditos.
2. Z-Crystals dos outros iniciais (Hau), Enigmatic Card → Tapu Lele, Tapu Bulu, Tapu Fini (opcionais) → Cosmog no mundo alternativo.
3. Akala Motel (Route 8): Anabel → Aether Paradise (Type: Null do Gladion, Faba, itens e Beast Balls da Wicke).
4. Missões das Ultra Beasts: UB-01 Nihilego → UB-02 (Melemele Motel, Route 2) Buzzwole [Sun] / Pheromosa [Moon] → Mina e UB-03 Xurkitree → UB-04 (Ula'ula Motel, Route 13) Kartana [Sun] / Celesteela [Moon] → Nanu e UB-05 Guzzlord (via Dexio, Poni Grove, Poni Plains, Poni Meadow, Resolution Cave) → recompensa do Looker → Necrozma (Ten Carat Hill).
5. Guzma na Hau'oli City Beachfront; Morimoto (Heahea); treinadores das eeveelutions e Eevium Z.
6. Poni Coast → Poni Gauntlet (Mina) → Battle Tree (Red ou Blue; Battle Legends; modo Super).
7. Revanche da Elite Four e defesas do título.

## Dúvidas

- Battle Tree: os times de Plumeria/Guzma, Sina/Dexio e Kiawe/Mallow vêm em duas colunas misturadas no texto; a separação por treinador não é segura (Plumeria/Guzma: Pinsir, Gengar, Crobat, Toxapex, Lurantis, Salazzle, Honchkrow, Toxicroak, Liepard, Bisharp, Toxapex, Golisopod; Sina/Dexio: Alolan Sandslash, Abomasnow, Alolan Ninetales, Slowbro, Glaceon, Lilligant, Mandibuzz, Oranguru, Drampa, Espeon, Whimsicott, Braviary, Passimian, Turtonator; Kiawe/Mallow: Arcanine, Alolan Marowak, Kangaskhan, Sceptile, Talonflame, Trevenant, Talonflame, Salazzle, Toucannon, Lurantis, Tsareena, Comfey). Conferir no PDF/outra fonte.
- Hau (Malie City): o texto lista Brionne 29, Dartrix 29, Torracat 29 depois das três variantes; a associação Rowlet→Brionne, Litten→Dartrix, Popplio→Torracat segue a regra de Hau ter o inicial fraco ao seu.
- Kukui: o detonado diz só "inicial de vantagem ao seu"; as espécies finais (Incineroar/Primarina/Decidueye) são inferidas.
- Números das Routes 4, 5, 6, 7, 9, 10, 11, 12, 14, 16 são inferidos pela posição; o detonado só cita Routes 1, 2, 3, 8, 13, 15, 17 (e não diz em que rota ficam Hau'oli Cemetery e Berry Fields).
- Local de Hau-3 (saída da Melemele Meadow) e de Gladion-1 (área acima do Paniola Ranch): rota sem número no texto.
- Níveis do inicial, do Aerodactyl, do ovo de Eevee, dos Zygarde e dos selvagens não constam; Ultra Beast da Aether Paradise sem nível.
- Nomes dos TMs (TM01, TM43, TM67, Melemele Sea, Kukui, Kahili) não constam.
- Oricorio da Kahili: forma não citada.
