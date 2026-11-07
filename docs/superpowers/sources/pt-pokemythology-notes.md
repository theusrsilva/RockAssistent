# Platinum — fatos extraídos do detonado da Pokémon Mythology

Fonte: https://www.pokemythology.net/detonados/detonado-platinum/ (PDF "Detonado Platinum – Pokémon Mythology.pdf",
salvo em 05/11/2026, 107 páginas).
Só **fatos de jogo** (chefes, níveis, encontros, itens) no nosso formato. Nenhuma prosa foi copiada. Golpes dos
chefes **não** constam no detonado: deixar `moves: []` e `verified: false` para golpes; espécies e níveis são
`verified: true` (uma fonte) até o analista conferir numa segunda.

## Ordem da história e chefes (para `bosses/pt.json`)

Variantes do rival por inicial do jogador (o rival pega o inicial com vantagem: Turtwig→Chimchar, Chimchar→Piplup,
Piplup→Turtwig). Assistente = Lucas (jogadora) ou Dawn (jogador).

| Order | Id | Kind | Nome | Local | Time (espécie lv) | Cap |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | rival-1 | rival | Rival | Route 201 | inicial do rival (nível não listado) | — |
| 2 | rival-2 | rival | Rival | Route 203 | Starly 7, inicial do rival 9 | 9 |
| 3 | roark | gym | Roark (Rock) | Oreburgh Gym | Geodude 12, Onix 12, Cranidos 14 | 14 |
| 4 | galactic-jubilife | evil | 2 Galactic Grunts (dupla com o assistente) | Jubilife City, saída para a Route 204 | Glameow 11, Stunky 11 | 11 |
| 5 | galactic-windworks | evil | Galactic Grunt | Valley Windworks (Route 205) | Glameow 13 | 13 |
| 6 | mars-1 | evil | Commander Mars | Valley Windworks | Zubat 15, Purugly 17 | 17 |
| 7 | gardenia | gym | Gardenia (Grass) | Eterna Gym | Turtwig 20, Cherrim 20, Roserade 22 | 22 |
| 8 | jupiter-1 | evil | Commander Jupiter | Team Galactic Eterna Building | Zubat 21, Skuntank 23 | 23 |
| 9 | fantina | gym | Fantina (Ghost) | Hearthome Gym | Duskull 24, Mismagius 26, Haunter 24 | 26 |
| 10 | rival-3 | rival | Rival | Hearthome City (saída leste) | Piplup: Staravia 25, Buizel 23, Ponyta 23, Grotle 27; Turtwig: Staravia 25, Buizel 23, Roselia 23, Monferno 27; Chimchar: Staravia 25, Roselia 23, Ponyta 23, Prinplup 27 | 27 |
| 11 | maylene | gym | Maylene (Fighting) | Veilstone Gym | Meditite 28, Machoke 29, Lucario 32 | 32 |
| 12 | galactic-veilstone | evil | 2 Galactic Grunts (dupla com o assistente) | Veilstone City | Zubat 25, Zubat 25, Stunky 26, Croagunk 26 | 26 |
| 13 | rival-4 | rival | Rival | Pastoria City | Piplup: Staravia 34, Buizel 32, Ponyta 32, Grotle 36; Turtwig: Staravia 34, Buizel 32, Roselia 32, Monferno 36; Chimchar: Staravia 34, Roselia 32, Ponyta 32, Prinplup 36 | 36 |
| 14 | crasher-wake | gym | Crasher Wake (Water) | Pastoria Gym | Gyarados 33, Quagsire 34, Floatzel 37 | 37 |
| 15 | galactic-valor-lakefront | evil | Galactic Grunt | Valor Lakefront | Croagunk 31 | 31 |
| 16 | galactic-celestic | evil | Galactic Grunt | Celestic Town | Houndour 32, Golbat 30 | 32 |
| 17 | cyrus-1 | evil | Cyrus (Team Galactic Boss) | Celestic Ruins (caverna de Celestic) | Murkrow 36, Sneasel 34, Golbat 34 | 36 |
| 18 | rival-5 | rival | Rival | Canalave City (ponte) | Piplup: Staraptor 36, Heracross 37, Floatzel 35, Rapidash 35, Torterra 38; Turtwig: Staraptor 36, Heracross 37, Floatzel 35, Roserade 35, Infernape 38; Chimchar: Staraptor 36, Heracross 37, Roserade 35, Rapidash 35, Empoleon 38 | 38 |
| 19 | byron | gym | Byron (Steel) | Canalave Gym | Magneton 37, Steelix 38, Bastiodon 41 | 41 |
| 20 | saturn-1 | evil | Commander Saturn | Lake Valor (caverna) | Golbat 38, Toxicroak 40, Bronzor 38 | 40 |
| 21 | mars-2 | evil | Commander Mars | Lake Verity | Golbat 38, Bronzor 38, Purugly 40 | 40 |
| 22 | candice | gym | Candice (Ice) | Snowpoint Gym | Sneasel 40, Piloswine 40, Abomasnow 42, Froslass 44 | 44 |
| 23 | cyrus-2 | evil | Cyrus | Team Galactic HQ, Veilstone | Sneasel 44, Honchkrow 46, Crobat 44 | 46 |
| 24 | saturn-2 | evil | Commander Saturn | Team Galactic HQ (sala do trio) | Golbat 42, Toxicroak 44, Bronzor 42 | 44 |
| 25 | mars-jupiter-spear-pillar | evil | Mars + Jupiter (dupla, com o rival) | Spear Pillar, Mt. Coronet | Mars: Bronzor 44, Golbat 44, Purugly 46; Jupiter: Bronzor 44, Golbat 44, Skuntank 46 | 46 |
| 26 | cyrus-3 | evil | Cyrus (final) | Distortion World | Houndoom 45, Honchkrow 47, Gyarados 46, Weavile 48, Crobat 46 | 48 |
| 27 | giratina-1 | other | Giratina (Origin Forme; capturar ou derrotar) | Distortion World | Giratina 47 | 47 |
| 28 | volkner | gym | Volkner (Electric) | Sunyshore Gym | Jolteon 46, Raichu 46, Luxray 48, Electivire 50 | 50 |
| 29 | rival-6 | rival | Rival | Pokémon League (antes da Elite Four) | Piplup: Staraptor 48, Snorlax 49, Heracross 48, Floatzel 47, Rapidash 47, Torterra 51; Turtwig: Staraptor 48, Snorlax 49, Heracross 48, Floatzel 47, Roserade 47, Infernape 51; Chimchar: Staraptor 48, Snorlax 49, Heracross 48, Roserade 47, Rapidash 47, Empoleon 51 | 51 |
| 30 | aaron-1 | elite | Aaron (Bug) | Pokémon League | Yanmega 49, Scizor 49, Vespiquen 50, Heracross 51, Drapion 53 | 53 |
| 31 | bertha-1 | elite | Bertha (Ground) | Pokémon League | Whiscash 50, Gliscor 53, Hippowdon 52, Golem 52, Rhyperior 55 | 55 |
| 32 | flint-1 | elite | Flint (Fire) | Pokémon League | Houndoom 52, Flareon 55, Rapidash 53, Infernape 55, Magmortar 57 | 57 |
| 33 | lucian-1 | elite | Lucian (Psychic) | Pokémon League | Mr. Mime 53, Espeon 55, Alakazam 56, Bronzong 54, Gallade 59 | 59 |
| 34 | cynthia-1 | champion | Cynthia (Campeã) | Pokémon League | Spiritomb 58, Togekiss 60, Milotic 58, Garchomp 62, Lucario 60, Roserade 58 | 62 |
| 35 | flint-volkner-fight-area | other | Flint + Volkner (dupla, com o rival) | Fight Area | Flint: Houndoom 56, Flareon 56, Magmortar 58; Volkner: Luxray 56, Jolteon 56, Electivire 58 | 58 |
| 36 | mars-3 | evil | Commander Mars | Stark Mountain (entrada) | Bronzong 58, Golbat 58, Purugly 60 | 60 |
| 37 | jupiter-2 | evil | Commander Jupiter | Stark Mountain (entrada) | Bronzong 58, Golbat 58, Skuntank 60 | 60 |
| 38 | rival-battleground-1 | rival | Rival (primeira revanche) | Survival Area (fora do Battleground) | Piplup: Staraptor 61, Snorlax 63, Heracross 62, Floatzel 59, Rapidash 59, Torterra 65; Turtwig: Staraptor 61, Snorlax 63, Heracross 62, Floatzel 59, Roserade 59, Infernape 65; Chimchar: Staraptor 61, Snorlax 63, Heracross 62, Roserade 59, Rapidash 59, Empoleon 65 | 65 |
| 39 | rival-weekend | rival | Rival (fins de semana) | Survival Area | mesmo time +10 (inicial final 75) | 75 |
| 40 | rival-weekend-20 | rival | Rival (após 20 vitórias na Liga) | Survival Area | mesmo time +20 (inicial final 85) | 85 |
| 41 | aaron-2 | elite | Aaron (revanche) | Pokémon League | Yanmega 65, Scizor 65, Vespiquen 66, Heracross 67, Drapion 69 | 69 |
| 42 | bertha-2 | elite | Bertha (revanche) | Pokémon League | Whiscash 66, Gliscor 69, Hippowdon 68, Golem 68, Rhyperior 71 | 71 |
| 43 | flint-2 | elite | Flint (revanche) | Pokémon League | Houndoom 68, Flareon 71, Rapidash 69, Infernape 71, Magmortar 73 | 73 |
| 44 | lucian-2 | elite | Lucian (revanche) | Pokémon League | Mr. Mime 69, Espeon 71, Alakazam 72, Bronzong 70, Gallade 75 | 75 |
| 45 | cynthia-2 | champion | Cynthia (revanche) | Pokémon League | Spiritomb 74, Togekiss 76, Milotic 74, Garchomp 78, Lucario 76, Roserade 74 | 78 |
| 46 | bg-roark | other | Roark (Battleground) | Survival Area | Aerodactyl 62, Probopass 61, Rampardos 63, Golem 61, Tyranitar 65 | 65 |
| 47 | bg-gardenia | other | Gardenia (Battleground) | Survival Area | Jumpluff 61, Cherrim 62, Bellossom 61, Torterra 63, Roserade 65 | 65 |
| 48 | bg-maylene | other | Maylene (Battleground) | Survival Area | Hitmontop 62, Breloom 62, Medicham 63, Lucario 66, Machamp 64 | 66 |
| 49 | bg-crasher-wake | other | Crasher Wake (Battleground) | Survival Area | Sharpedo 61, Floatzel 63, Quagsire 63, Gyarados 66, Ludicolo 65 | 66 |
| 50 | bg-fantina | other | Fantina (Battleground) | Survival Area | Banette 62, Mismagius 62, Gengar 63, Drifblim 66, Dusknoir 62 | 66 |
| 51 | bg-byron | other | Byron (Battleground) | Survival Area | Skarmory 61, Magnezone 62, Steelix 61, Bastiodon 63, Aggron 65 | 65 |
| 52 | bg-candice | other | Candice (Battleground) | Survival Area | Weavile 62, Abomasnow 61, Mamoswine 61, Froslass 63, Glaceon 65 | 65 |
| 53 | bg-volkner | other | Volkner (Battleground) | Survival Area | Jolteon 61, Raichu 61, Luxray 62, Electivire 65, Lanturn 63 | 65 |
| 54 | bg-cheryl | other | Cheryl (Battleground) | Survival Area | Wobbuffet 61, Hariyama 62, Wailord 63, Drifblim 61, Blissey 65 | 65 |
| 55 | bg-buck | other | Buck (Battleground) | Survival Area | Shuckle 61, Dusknoir 63, Torkoal 61, Umbreon 63, Claydol 65 | 65 |
| 56 | bg-marley | other | Marley (Battleground) | Survival Area | Ninjask 61, Electrode 61, Crobat 62, Weavile 63, Arcanine 65 | 65 |
| 57 | bg-riley | other | Riley (Battleground) | Survival Area | Absol 61, Salamence 63, Metagross 62, Ursaring 61, Lucario 65 | 65 |
| 58 | bg-mira | other | Mira (Battleground) | Survival Area | Porygon-Z 61, Magnezone 62, Gengar 61, Togekiss 63, Alakazam 65 | 65 |
| 59 | argenta | other | Hall Matron Argenta | Battle Hall, Battle Frontier | aleatório (50ª e 170ª batalha) | — |
| 60 | darach | other | Castle Valet Darach | Battle Castle | 21ª: Staraptor, Houndoom, Empoleon; 49ª: Empoleon, Gallade, Entei (lv 50) | 50 |
| 61 | dahlia | other | Arcade Star Dahlia | Battle Arcade | 21ª: Dusknoir, Medicham, Ludicolo; 49ª: Blaziken, Togekiss, Zapdos (lv 50) | 50 |
| 62 | thorton | other | Factory Head Thorton | Battle Factory | aleatório (21ª e 49ª) | — |
| 63 | palmer | other | Tower Tycoon Palmer | Battle Tower | 21ª: Dragonite, Rhyperior, Milotic; 49ª: Cresselia, Heatran, Regigigas (lv 50) | 50 |

Parceiros de dupla (não são chefes): assistente (inicial 13 em Jubilife; Clefairy 25, Kadabra 25, inicial evoluído 27
em Veilstone); Cheryl Chansey 20 (Eterna Forest); Mira Kadabra 18 (Wayward Cave); Riley Lucario 41 (Iron Island);
Marley Arcanine 59 (Victory Road, ala da Route 224); Buck Claydol 63 (Stark Mountain); rival na Spear Pillar (Piplup:
Staraptor 42, Munchlax 40, Heracross 42, Floatzel 40, Rapidash 40, Torterra 44; Turtwig: Staraptor 42, Munchlax 40,
Heracross 42, Floatzel 40, Roserade 40, Infernape 44; Chimchar: Staraptor 42, Munchlax 40, Heracross 42, Roserade 40,
Rapidash 40, Empoleon 44); rival na Fight Area (Piplup: Staraptor 53, Snorlax 54, Heracross 53, Floatzel 52,
Rapidash 52, Torterra 56; Turtwig: Staraptor 53, Snorlax 54, Heracross 53, Floatzel 52, Roserade 52, Infernape 56;
Chimchar: Staraptor 53, Snorlax 54, Heracross 53, Roserade 52, Rapidash 52, Empoleon 56).

Treinadores dos Pokémon Centers (1×/dia, sobem com as insígnias; time final pós-Liga): Reporter (Drifblim 57,
Luxray 57) e Cameraman (Magneton 54, Magneton 54, Mr. Mime 55, Alakazam 57) separados — seg/qua/qui Jubilife,
ter/sex Hearthome, sáb/dom Battle Area; Reporter & Cameraman em dupla (Gardevoir 59, Magnezone 59) — seg/ter/sáb/dom
Eterna, qua/sex Veilstone; Idol (Clefairy 53 ×6) — seg/sex Pastoria, ter/qui Oreburgh, sáb/dom Snowpoint; Guitarist
(Kricketune 53; restante não listado) — seg/ter Canalave, qua/sáb/dom Pastoria, qui/sex Sunyshore; PokéKid (Pichu 53,
Pikachu 53, Raichu 53) — seg/qui/dom Floaroma, ter/sex Solaceon, qua/sáb Resort Area; Clown (Mr. Mime 56, Alakazam 56,
Ambipom 56) — seg/qua/sex Celestic, ter/qui Pastoria, sáb/dom Hearthome.

Níveis recomendados pelo detonado: Roark 20 e +5 por ginásio.

## Áreas (para `areas/pt.json`, kind e ordem de liberação)

| Ordem | Id | Nome | Kind | Libera |
| --- | --- | --- | --- | --- |
| 1 | twinleaf-town | Twinleaf Town | city | início |
| 2 | route-201 | Route 201 | route | início |
| 3 | lake-verity | Lake Verity / Verity Lakefront | zone | início |
| 4 | sandgem-town | Sandgem Town | city | após o inicial |
| 5 | route-202 | Route 202 | route | Poké Balls do assistente |
| 6 | jubilife-city | Jubilife City | city | Route 202 |
| 7 | route-203 | Route 203 | route | Jubilife |
| 8 | oreburgh-gate | Oreburgh Gate | dungeon | Route 203 (partes com Rock Smash, Surf, Strength) |
| 9 | oreburgh-city | Oreburgh City | city | Oreburgh Gate |
| 10 | oreburgh-mine | Oreburgh Mine | dungeon | Oreburgh |
| 11 | route-207 | Route 207 (sul) | route | após Coal Badge |
| 12 | route-204 | Route 204 | route | após Coal Badge |
| 13 | ravaged-path | Ravaged Path | dungeon | Rock Smash |
| 14 | floaroma-town | Floaroma Town | city | Route 204 |
| 15 | route-205 | Route 205 | route | Floaroma |
| 16 | valley-windworks | Valley Windworks | building | Works Key |
| 17 | floaroma-meadow | Floaroma Meadow | zone | após o grunt da Windworks |
| 18 | eterna-forest | Eterna Forest | dungeon | após Mars |
| 19 | eterna-city | Eterna City | city | Eterna Forest |
| 20 | route-211-west | Route 211 (oeste) | route | Eterna |
| 21 | galactic-eterna-building | Team Galactic Eterna Building | building | Forest Badge + Cut |
| 22 | old-chateau | Old Chateau | building | Cut (Eterna Forest) |
| 23 | route-206 | Route 206 (Cycling Road) | route | Bicycle |
| 24 | wayward-cave | Wayward Cave | dungeon | Bicycle (ala oculta sem Strength) |
| 25 | mt-coronet | Mt. Coronet | dungeon | Route 207 |
| 26 | route-208 | Route 208 | route | Mt. Coronet |
| 27 | hearthome-city | Hearthome City | city | Route 208 |
| 28 | amity-square | Amity Square | zone | Hearthome (Pokémon permitido) |
| 29 | route-209 | Route 209 | route | Relic Badge + rival |
| 30 | lost-tower | Lost Tower | building | Route 209 |
| 31 | solaceon-town | Solaceon Town | city | Route 209 |
| 32 | solaceon-ruins | Solaceon Ruins | dungeon | Solaceon |
| 33 | route-210-south | Route 210 (sul) | route | Solaceon |
| 34 | route-215 | Route 215 | route | Route 210 |
| 35 | veilstone-city | Veilstone City | city | Route 215 |
| 36 | route-214 | Route 214 | route | Cobble Badge |
| 37 | ruin-maniac-cave | Ruin Maniac Cave | dungeon | Route 214 |
| 38 | valor-lakefront | Valor Lakefront | zone | Route 214 |
| 39 | route-213 | Route 213 | route | Valor Lakefront |
| 40 | pastoria-city | Pastoria City | city | Route 213 |
| 41 | great-marsh | Great Marsh (Safari) | zone | Pastoria ($500), após o evento do Galactic |
| 42 | route-210-north | Route 210 (norte) | route | SecretPotion + Defog |
| 43 | celestic-town | Celestic Town | city | Route 210 norte |
| 44 | route-211-east | Route 211 (leste) | route | Celestic |
| 45 | route-212 | Route 212 | route | após Cyrus em Celestic |
| 46 | pokemon-mansion | Pokémon Mansion | building | Route 212 |
| 47 | trophy-garden | Trophy Garden | zone | Pokémon Mansion |
| 48 | route-219 | Route 219 | sea | Surf |
| 49 | route-220 | Route 220 | sea | Surf |
| 50 | route-221 | Route 221 | route | Surf |
| 51 | pal-park | Pal Park | building | Surf (uso com National Dex) |
| 52 | fuego-ironworks | Fuego Ironworks | building | Surf (Route 205) |
| 53 | route-218 | Route 218 | route | Surf |
| 54 | canalave-city | Canalave City | city | Route 218 |
| 55 | iron-island | Iron Island | dungeon | barco em Canalave |
| 56 | route-216 | Route 216 | route | Strength (Mt. Coronet) |
| 57 | route-217 | Route 217 | route | Route 216 |
| 58 | snowpoint-city | Snowpoint City | city | Route 217 |
| 59 | acuity-lakefront | Acuity Lakefront / Lake Acuity | zone | Icicle Badge (Rock Climb) |
| 60 | lake-valor | Lake Valor | zone | após a biblioteca de Canalave |
| 61 | galactic-warehouse | Galactic Warehouse (Veilstone) | building | Storage Key (Looker) |
| 62 | galactic-hq | Team Galactic HQ | building | Galactic Key |
| 63 | spear-pillar | Spear Pillar | zone | após libertar o trio |
| 64 | distortion-world | Distortion World | zone | após a Spear Pillar |
| 65 | sendoff-spring | Sendoff Spring | zone | após Giratina |
| 66 | route-222 | Route 222 | route | após o Distortion World |
| 67 | sunyshore-city | Sunyshore City | city | Route 222 |
| 68 | route-223 | Route 223 | sea | Waterfall |
| 69 | victory-road | Victory Road | dungeon | Route 223 |
| 70 | pokemon-league | Pokémon League | building | Victory Road |
| 71 | turnback-cave | Turnback Cave | dungeon | pós-jogo |
| 72 | full-moon-island | Fullmoon Island | zone | pós-jogo (National Dex) |
| 73 | snowpoint-temple | Snowpoint Temple | dungeon | pós-jogo (National Dex) |
| 74 | route-224 | Route 224 | route | pós-jogo (National Dex) |
| 75 | fight-area | Fight Area | city | pós-jogo (barco em Snowpoint) |
| 76 | battle-frontier | Battle Frontier | building | após Flint + Volkner |
| 77 | route-225 | Route 225 | route | Fight Area |
| 78 | survival-area | Survival Area | city | Route 225 |
| 79 | battleground | Battleground (Top Trainer Café) | building | após Stark Mountain (Buck) |
| 80 | route-226 | Route 226 | route | Survival Area |
| 81 | route-228 | Route 228 | route | Route 226 |
| 82 | route-229 | Route 229 | route | Route 228 |
| 83 | resort-area | Resort Area | city | Route 229 |
| 84 | route-230 | Route 230 | sea | Route 229 / Fight Area |
| 85 | route-227 | Route 227 | route | Route 226 |
| 86 | stark-mountain | Stark Mountain | dungeon | Route 227 |
| 87 | newmoon-island | Newmoon Island | zone | Member Card (evento) |
| 88 | flower-paradise | Seabreak Path / Flower Paradise | zone | Oak's Letter (evento) |
| 89 | hall-of-origin | Hall of Origin | zone | Azure Flute (evento) |

## Encontros (para `encounters/pt.json`; method `walk` salvo indicação; `conditions`: morning/day/night)

- Route 202: Shinx.
- Route 204: Budew.
- Route 207: Ponyta.
- Oreburgh Mine (`cave`): Onix (outras não listadas).
- Route 205 / Valley Windworks: Shellos (West Sea), Buizel, Pachirisu.
- Eterna Forest: Buneary; Gastly (raro); Hoothoot (noite); Beautifly, Dustox (muito raros). Pescadores com Magikarp e
  Goldeen.
- Route 211 (oeste): Meditite, Chingling, Machop, Bronzor; Hoothoot (noite). Caverna: Cleffa (mais na manhã).
- Route 206: Gligar (único local do jogo).
- Mt. Coronet (sul, `cave`): Nosepass, Clefairy.
- Route 208: Ralts, Bibarel.
- Route 209: Ralts (comum), Duskull (noite), Chansey. `fish` Good Rod: Goldeen, Barboach, Magikarp 15–20.
- Lost Tower: fantasmas; Golbat (últimos andares).
- Route 210 (sul): Scyther.
- Route 215: Kadabra, Lickitung.
- Route 214: Graveler, Rhyhorn, Houndour.
- Ruin Maniac Cave (`cave`): Geodude; Hippopotas (chance cresce com o nº de letras de Unown).
- Route 213: Shellos (East Sea), Wingull, Chatot.
- Route 210 (norte): Swablu; Noctowl (noite).
- Route 212: Quagsire.
- Route 218: Gastrodon, Mr. Mime; `fish` Good Rod: Finneon.
- Iron Island (`cave`): Steelix.
- Route 216: Snover, Snorunt, Sneasel, Swinub.
- Mt. Coronet (subida ao Spear Pillar): Bronzong, Medicham; até lv 42; Chimecho (`cave`); Absol, Abomasnow (fora).
- Sendoff Spring: Dusclops.
- Route 222: Electabuzz, Magnemite, Magneton, Pelipper.
- Victory Road (`cave`): Onix, Steelix.
- Old Chateau: Gastly 14–17.
- Wayward Cave (ala oculta): Gible 15–17.
- Fuego Ironworks (fora): Magmar; lv 28–30.
- Trophy Garden: Pichu, Pikachu; pós-National Dex (Mr. Backlot, 1 por dia, 16–19): Meowth, Jigglypuff, Clefairy,
  Chansey, Eevee, Ditto, Marill, Castform, Plusle, Minun, Bonsly, Mime Jr., Happiny, Cleffa, Igglybuff, Azurill.
- Great Marsh: comuns Wooper, Quagsire, Bidoof, Bibarel, Budew, Tropius, Yanma, Hoothoot e Noctowl (noite); raros
  Staravia, Roselia, Carnivine, Skorupi, Croagunk, Tangela. `fish`: Gyarados, Barboach, Whiscash. Pós-National Dex:
  Paras, Exeggcute, Kangaskhan, Shroomish, Kecleon, Gulpin, Drapion, Toxicroak; `fish` Carvanha.
- Solaceon Ruins (`cave`): Unown 14–25; ! e ? após as 26 letras.
- Árvores de mel (`tree`, Honey, 9–12 h, lv 5–14): Wurmple, Combee, Burmy, Cherubi, Aipom (raro), Heracross (raro),
  Munchlax (muito raro).
- Route 224 (ala da Victory Road): `surf` Lapras; Azumarill, Dewgong.
- Route 225: Fearow, Raticate, Machoke, Graveler, Banette (noite); Spearow, Rattata (raros).
- Route 226: `fish` Super Rod: Relicanth, Seadra.
- Route 228: Dugtrio, Rhydon, Cacturne (mais à noite), Hippowdon; Diglett, Cacnea (raros).
- Route 229: Beautifly, Dustox, Pidgey, Volbeat, Illumise, Ledian (manhã), Ariados (noite); `surf` Surskit,
  Masquerain (muito raro).
- Route 227: Camerupt, Rhydon, Golbat (noite), Weezing; Skarmory, Numel, Rhyhorn (raros); `surf` Poliwag, Poliwhirl.
- Stark Mountain (`cave`): Slugma, Magcargo, Koffing.
- Poké Radar e irmã do assistente (Sandgem, 1 espécie por dia) no pós-jogo: espécies não listadas.
- Lendários e fixos (`method: fixed`):
  - Drifloon lv 22 (Valley Windworks, sextas-feiras, após Mars).
  - Rotom lv 20 (TV do Old Chateau, 22:00–4:00; já antes da National Dex).
  - Giratina lv 47 (Distortion World; obrigatório, capturar ou derrotar).
  - Azelf lv 50 (Lake Valor), Uxie lv 50 (Lake Acuity) — após o Distortion World.
  - Mesprit lv 50 (`walk`, errante após o Lake Verity).
  - Cresselia lv 50 (`walk`, errante após a Fullmoon Island; National Dex).
  - Articuno, Zapdos, Moltres lv 60 (`walk`, errantes; National Dex + Oak no Pal Park e em Eterna).
  - Dialga lv 70 e Palkia lv 70 (Spear Pillar; Adamant Orb e Lustrous Orb do Mt. Coronet + avó da Cynthia, pós-jogo).
  - Heatran lv 50 (Stark Mountain, após o evento do Buck/Charon).
  - Regigigas lv 1 (Snowpoint Temple; Regirock, Regice e Registeel no time).
  - Regirock, Regice, Registeel lv 30 (Route 228 norte, Mt. Coronet saída para a 216, Iron Island; exige o Regigigas
    do evento do 11º filme).
  - Spiritomb (Hallowed Tower, Route 209; Odd Keystone + 32 conversas no Underground).
  - Darkrai lv 50 (Newmoon Island; Member Card). Shaymin lv 30 (Flower Paradise; Oak's Letter). Arceus lv 80 (Hall of
    Origin; Azure Flute). Manaphy lv 1 (ovo do Pokémon Ranger); Phione lv 1 (Manaphy + Ditto).

## Presentes, trocas in-game, ovos e fósseis (`method: gift` / `trade`)

- Inicial: Turtwig/Chimchar/Piplup (Route 201; nível não listado).
- Ovo de Togepi (Cynthia, loja de bicicletas de Eterna; 2.560 passos; nasce lv 1).
- Eevee lv 20 (Bebe, Hearthome).
- Porygon lv 25 (casa ao norte do Pokémon Center de Veilstone).
- Ovo de Riolu (Riley, Iron Island; nasce lv 1).
- Troca: Buizel → Chatot (prédio ao lado do Poké Mart de Eterna).
- Fósseis: Explorer Kit (Eterna) para o Underground; espécies não listadas.

## TMs/HMs e itens-chave (local)

- HM01 Cut (Cynthia, Eterna); HM02 Fly (casa em Veilstone, após os grunts); HM03 Surf (avó da Cynthia, Celestic); HM04
  Strength (Iron Island); HM05 Defog (Solaceon Ruins); HM06 Rock Smash (Oreburgh Gate); HM07 Waterfall (Jasmine,
  Sunyshore); HM08 Rock Climb (Route 217).
- TMs: TM27 (Rowan, Sandgem) e TM27 Return (Lost Tower) [o detonado cita os dois]; TM70 Flash (Oreburgh Gate); TM39 Rock
  Tomb (Ravaged Path); TM09 Bullet Seed (Route 204); TM88 Pluck (Floaroma); TM67 Recycle (Eterna); TM12 Taunt (Route
  211); TM86 Grass Knot (Gardenia); TM46 Thief (fora do Galactic Building de Eterna); TM31 Brick Break (Oreburgh Gate,
  rampa); TM78 Captivate (Route 204, Cut); TM82 Sleep Talk (Route 205, Cut); TM65 Shadow Claw (Fantina); TM51 Roost
  (Route 210); TM66 Payback (Route 215); TM60 Drain Punch (Maylene); TM28 Dig (Ruin Maniac Cave); TM92 Trick Room (Valor
  Lakefront); TM40 Aerial Ace (Route 213, Rock Smash); TM55 Brine (Crasher Wake); TM30 Shadow Ball (Route 210 norte);
  TM24 Thunderbolt (Valley Windworks, Surf); TM84 (Route 212, Surf); TM48 (Canalave); TM29, TM05, TM85, TM13 Ice Beam
  (Rock Climb: Route 211, Route 213, Lake Valor, Route 216); TM49, TM36, TM21 (Galactic Warehouse); TM80 Rock Slide e
  TM02 (Mt. Coronet); TM25 (Lake Valor), TM38 (Lake Verity), TM14 (Lake Acuity); TM35 Flamethrower (Fuego Ironworks);
  TM26 Earthquake (Wayward Cave, ala oculta); TM45, TM43 (Amity Square); TM42 (Survival Area).
- Itens-chave: Running Shoes, Journal (mãe); Battle Recorder (Looker, Jubilife); Town Map (rival); Pokétch (Jubilife);
  Old Rod (Jubilife); Quick Claw (Jubilife); Dusk Ball (Oreburgh); Works Key e Honey (Floaroma Meadow); Sprayduck/
  "Psyduck Pail" (Floaroma); Explorer Kit (Eterna); Dragon Fang (atrás da estátua de Eterna); Up-Grade (Galactic
  Building de Eterna); Bicycle (Eterna); Exp. Share (Route 206, 35 vistos); VS Seeker e Dowsing Machine (assistente,
  Route 207); Odd Keystone (Route 208; também ao sul de Twinleaf com Surf); Poffin Case, Shell Bell (Hearthome); Good
  Rod (Route 209); Oval Stone (Lost Tower); Seal Case (Solaceon); Coin Case (palhaço, Veilstone); Sticky Barb
  (Veilstone Dept. Store); Suite Key → White Flute (Valor Lakefront); Footprint Ribbon (Route 213); SecretPotion
  (Cynthia); Old Charm (Cynthia); Storage Key e Galactic Key (Veilstone); Master Ball (Cyrus no HQ); Black Flute
  (Looker, Mt. Coronet); Adamant Orb e Lustrous Orb (Mt. Coronet, Waterfall); Griseous Orb (Distortion World pela
  Turnback Cave); Thunder Stone (Sunyshore); Dawn Stone, Protein (Mt. Coronet); Rare Candy (Route 214, Route 230); Poké
  Radar (Rowan); Lunar Wing (Fullmoon Island); Metal Coat (Byron, Iron Island); Up-Grade (Oak, Eterna); Super Rod e
  Scope Lens (Fight Area); Fresh Water (Route 225); Mind Plate, Odd Incense, Nugget (Solaceon Ruins); Spooky Plate,
  Amulet Coin (Amity Square); Hard Stone, NeverMeltIce, Metal Coat (salas dos Regis); Gracidea (Floaroma, com Shaymin);
  Secret Key (evento) para as formas do Rotom.
- Lojas: Herb Shop (Eterna); Café Cabin, Moo-Moo Milk (Route 210); Veilstone Department Store (TM70, bebidas);
  Dusk/Quick Ball (Pastoria); Timer Ball e BlackGlasses (Celestic); Seals por dia da semana (Sunyshore); BP (Battle
  Frontier).
- Move Tutors: Frenzy Plant, Blast Burn, Hydro Cannon (Route 228); tutores por Shards (Snowpoint, Route 226); Move
  Reminder por Heart Scale (Pastoria); Move Deleter (Canalave).
- Rotom (Secret Key, Galactic Building de Eterna): forno → Overheat; ventilador → Air Slash; geladeira → Blizzard;
  máquina de lavar → Hydro Pump; cortador de grama → Leaf Storm.

## Mecânicas (para o guia de mecânicas, texto nosso)

- Sinnoh Dex de 210 (Manaphy visto na foto do livro da Pokémon Mansion); National Dex com Oak em Sandgem.
- Pokétch e ciclo dia/noite pelo relógio do DS (joggers da Route 209 só de manhã, 04:00–09:59; policiais à noite).
- Distortion World: gravidade e plataformas; puzzle com Strength guiado por Uxie, Mesprit e Azelf.
- Tag battles com parceiros que curam após cada luta.
- Honey trees, Great Marsh ($500, 30 Safari Balls, 500 passos, 6 áreas), Day Care (Solaceon), Poké Radar, Underground.
- Treinadores diários nos Pokémon Centers (ver acima); revanches de líderes no Battleground; rival aos sábados/domingos.
- Battle Frontier (lv 50; 7 batalhas por série; Frontier Brains só no single; prata e ouro): Battle Tower (Palmer, 21ª
  e 49ª; itens diferentes), Battle Factory (Pokémon alugados; lv 50 ou aberto; Thorton 21ª e 49ª), Battle Arcade
  (roleta de efeitos; Dahlia 21ª e 49ª), Battle Castle (Castle Points, sem cura automática, itens retirados; Darach
  21ª e 49ª), Battle Hall (1 Pokémon, 10 batalhas por série, escolha do tipo, ranks 1–10; Argenta 50ª e 170ª).
  Proibidos: Mewtwo, Mew, Lugia, Ho-Oh, Celebi, Kyogre, Groudon, Rayquaza, Jirachi, Deoxys, Dialga, Palkia, Giratina,
  Phione, Manaphy, Darkrai, Shaymin, Arceus.
- Formas: Giratina Origin com Griseous Orb; Shaymin Sky com Gracidea; Rotom com eletrodomésticos.
- Pal Park: migração de 6 Pokémon por dia dos jogos de GBA, sem golpes de HM.
- Clima: chuva (Routes 212/213/215), neblina (Route 210), granizo (216/217), cinza (227), areia (228).

## Pós-jogo (ordem de eventos)

1. Ver os 210 (foto do Manaphy na Pokémon Mansion) → National Dex (Oak) → Poké Radar. 2. Turnback Cave (Griseous Orb);
Fullmoon Island (Cresselia); Snowpoint Temple; Route 224; aves de Kanto (Oak no Pal Park e em Eterna). 3. Dialga e
Palkia lv 70 (avó da Cynthia + Orbs). 4. Barco em Snowpoint → Fight Area: Flint + Volkner → Battle Frontier.
5. Routes 225–230 → Stark Mountain (Mars, Jupiter, Charon, Buck) → Battleground e rival aos fins de semana → Heatran.
6. Revanche da Elite Four (+16). 7. Eventos: Darkrai, Shaymin, Arceus, Manaphy, Regis.

## Dúvidas

- Níveis do inicial e do primeiro rival (Route 201) não aparecem.
- O detonado diz que Rowan dá o "TM 27" em Sandgem e também cita TM27 Return na Lost Tower.
- Níveis dos Pokémon dos Frontier Brains não aparecem (lv 50 pela regra); times de Argenta e Thorton são aleatórios.
- Guitarist só tem Kricketune 53 listado.
- Nomes dos TMs 84, 48, 29, 05, 85, 49, 36, 21, 02, 25, 38, 14, 45, 43 e 42 não aparecem.
- Thunder, Blizzard e Fire Blast ficam acessíveis após libertar o trio, sem local listado.
- O detonado diz "Snorant" e "Pelliper" (grafia); assumido Snorunt e Pelipper.
