# Diamond/Pearl — fatos extraídos do detonado da Pokémon Mythology

Fonte: https://www.pokemythology.net/detonados/detonado-diamond-pearl/ (PDF "Detonado Diamond_Pearl – Pokémon
Mythology.pdf", salvo em 05/11/2026, 65 páginas).
Só **fatos de jogo** (chefes, níveis, encontros, itens) no nosso formato. Nenhuma prosa foi copiada. Golpes dos
chefes **não** constam no detonado (exceto o Tower Tycoon Palmer): deixar `moves: []` e `verified: false` para golpes;
espécies e níveis são `verified: true` (uma fonte) até o analista conferir numa segunda.

## Ordem da história e chefes (para `bosses/dp.json`)

Variantes do rival por inicial do jogador (o rival pega o inicial com vantagem: Turtwig→Chimchar, Chimchar→Piplup,
Piplup→Turtwig). Assistente = Lucas (jogadora) ou Dawn (jogador).

| Order | Id | Kind | Nome | Local | Time (espécie lv) | Cap |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | starly-intro | other | Starly selvagem (escolha do inicial) | Lake Verity | Starly 2 | 2 |
| 2 | rival-1 | rival | Rival | Route 203 | Starly 7, inicial do rival 9 | 9 |
| 3 | roark | gym | Roark (Rock) | Oreburgh Gym | Geodude 12, Onix 12, Cranidos 14 | 14 |
| 4 | galactic-jubilife | evil | 2 Galactic Grunts (dupla com o assistente) | Jubilife City, saída para a Route 204 | Zubat 9, Wurmple 7 | 9 |
| 5 | galactic-windworks | evil | Galactic Grunt | Valley Windworks (Route 205) | Glameow 11 | 11 |
| 6 | mars-1 | evil | Commander Mars | Valley Windworks | Zubat 14, Purugly 16 | 16 |
| 7 | gardenia | gym | Gardenia (Grass) | Eterna Gym | Cherubi 19, Turtwig 19, Roserade 22 | 22 |
| 8 | jupiter-1 | evil | Commander Jupiter | Team Galactic Eterna Building | Zubat 18, Skuntank 20 | 20 |
| 9 | rival-2 | rival | Rival | Hearthome City (saída leste) | Piplup: Starly 19, Buizel 20, Ponyta 20, Grotle 21; Turtwig: Starly 19, Buizel 20, Roselia 20, Monferno 21; Chimchar: Starly 19, Roselia 20, Ponyta 20, Prinplup 21 | 21 |
| 10 | maylene | gym | Maylene (Fighting) | Veilstone Gym | Meditite 27, Machoke 27, Lucario 30 | 30 |
| 11 | galactic-veilstone | evil | 2 Galactic Grunts (dupla com o assistente) | Veilstone City | Beautifly 25, Stunky 25, Dustox 25, Croagunk 25 | 25 |
| 12 | crasher-wake | gym | Crasher Wake (Water) | Pastoria Gym | Gyarados 27, Quagsire 27, Floatzel 30 | 30 |
| 13 | rival-3 | rival | Rival | Pastoria City (perto da Great Marsh) | Piplup: Starly 26, Buizel 25, Ponyta 25, Grotle 28; Turtwig: Starly 26, Buizel 25, Roselia 25, Monferno 28; Chimchar: Starly 26, Roselia 25, Ponyta 25, Prinplup 28 | 28 |
| 14 | galactic-valor-lakefront | evil | Galactic Grunt | Valor Lakefront | Glameow 25 | 25 |
| 15 | galactic-celestic | evil | Galactic Grunt | Celestic Town | Croagunk 27, Beautifly 25 | 27 |
| 16 | fantina | gym | Fantina (Ghost) | Hearthome Gym | Drifblim 32, Mismagius 36, Gengar 34 | 36 |
| 17 | rival-4 | rival | Rival | Canalave City (ponte) | Piplup: Staravia 31, Heracross 30, Buizel 32, Ponyta 32, Grotle 35; Turtwig: Staravia 31, Heracross 30, Buizel 32, Roselia 32, Monferno 35; Chimchar: Staravia 31, Heracross 30, Roselia 32, Ponyta 32, Prinplup 35 | 35 |
| 18 | byron | gym | Byron (Steel) | Canalave Gym | Bronzor 36, Steelix 36, Bastiodon 39 | 39 |
| 19 | candice | gym | Candice (Ice) | Snowpoint Gym | Snover 38, Medicham 40, Sneasel 38, Abomasnow 42 | 42 |
| 20 | saturn-1 | evil | Commander Saturn | Lake Valor (caverna) | Kadabra 35, Toxicroak 37, Bronzor 35 | 37 |
| 21 | mars-2 | evil | Commander Mars | Lake Verity | Golbat 37, Bronzor 37, Purugly 39 | 39 |
| 22 | cyrus-1 | evil | Cyrus (Team Galactic Boss) | Team Galactic HQ, Veilstone | Murkrow 40, Sneasel 43, Golbat 40 | 43 |
| 23 | saturn-2 | evil | Commander Saturn | Team Galactic HQ (sala do trio) | Kadabra 38, Toxicroak 40, Bronzor 38 | 40 |
| 24 | mars-jupiter-spear-pillar | evil | Mars + Jupiter (dupla, com o rival) | Spear Pillar, Mt. Coronet | Mars: Purugly 45 + Bronzor 41 + Golbat; Jupiter: Skuntank 46 + Bronzor 41 + Golbat (Golbats 42 e 41, divisão ambígua) | 46 |
| 25 | cyrus-2 | evil | Cyrus (final) | Spear Pillar | Honchkrow 45, Gyarados 45, Weavile 48, Crobat 46 | 48 |
| 26 | dialga-palkia | other | Dialga [Diamond] / Palkia [Pearl] | Spear Pillar | Dialga/Palkia 47 | 47 |
| 27 | volkner | gym | Volkner (Electric) | Sunyshore Gym | Raichu 46, Ambipom 47, Luxray 49, Octillery 48 | 49 |
| 28 | rival-5 | rival | Rival | Pokémon League (antes da Elite Four) | Piplup: Staraptor 48, Snorlax 51, Heracross 50, Floatzel 49, Rapidash 49, Torterra 53; Turtwig: Staraptor 48, Snorlax 51, Heracross 50, Floatzel 49, Roserade 49, Infernape 53; Chimchar: Staraptor 48, Snorlax 51, Heracross 50, Roserade 49, Rapidash 49, Empoleon 53 | 53 |
| 29 | aaron | elite | Aaron (Bug) | Pokémon League | Dustox 53, Drapion 57, Vespiquen 54, Heracross 54, Beautifly 53 | 57 |
| 30 | bertha | elite | Bertha (Ground) | Pokémon League | Quagsire 55, Hippowdon 59, Golem 56, Whiscash 55, Sudowoodo 56 | 59 |
| 31 | flint | elite | Flint (Fire) | Pokémon League | Rapidash 58, Infernape 61, Steelix 57, Drifblim 58, Lopunny 57 | 61 |
| 32 | lucian | elite | Lucian (Psychic) | Pokémon League | Mr. Mime 59, Medicham 60, Alakazam 60, Bronzong 63, Girafarig 59 | 63 |
| 33 | cynthia | champion | Cynthia (Campeã) | Pokémon League | Spiritomb 61, Gastrodon 60, Milotic 63, Garchomp 66, Lucario 63, Roserade 60 | 66 |
| 34 | palmer-21 | other | Tower Tycoon Palmer (21ª batalha) | Battle Tower, Battle Zone | Rhyperior (Expert Belt; Flamethrower, Thunderbolt, Ice Beam, Rock Wrecker), Dragonite (Wide Lens; Stone Edge, Dragon Rush, Aqua Tail, Iron Tail), Milotic (Salac Berry; Hypnosis, Dragon Pulse, Hydro Pump, Recover); lv 50 ou 100 conforme a regra | — |
| 35 | palmer-49 | other | Tower Tycoon Palmer (49ª batalha) | Battle Tower | Heatran (BrightPowder; Dark Pulse, Magma Storm, Flash Cannon, Explosion), Regigigas (Chople Berry; Stone Edge, Ice Punch, Crush Grip, Brick Break), Cresselia (Leftovers; Shadow Ball, Psychic, Calm Mind, Moonlight) | — |

Parceiros de dupla (não são chefes): assistente (inicial 13 em Jubilife; Clefairy 25, Kadabra 25, inicial evoluído 28
em Veilstone); Cheryl Chansey 15 (Eterna Forest); Mira Kadabra 18 (Wayward Cave); Riley Lucario 34 (Iron Island);
Marley Arcanine 57 (Victory Road, ala da Route 224); Buck Claydol 58 (Stark Mountain); rival na Spear Pillar (Piplup:
Staraptor 40, Munchlax 40, Heracross 42, Buizel 41, Ponyta 41, Torterra 43; Turtwig: Staraptor 40, Munchlax 40,
Heracross 42, Buizel 41, Roselia 41, Infernape 43; Chimchar: Staraptor 40, Munchlax 40, Heracross 42, Roselia 41,
Ponyta 41, Empoleon 43).

Níveis recomendados pelo detonado: Roark 20 e +5 por ginásio; lagos/Galactic ~50; Sunyshore >50.

## Áreas (para `areas/dp.json`, kind e ordem de liberação)

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
| 12 | route-204 | Route 204 | route | após Coal Badge (Galactic em Jubilife) |
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
| 24 | wayward-cave | Wayward Cave | dungeon | Bicycle (Flash recomendado; entrada oculta com Strength) |
| 25 | mt-coronet | Mt. Coronet | dungeon | Route 207 |
| 26 | route-208 | Route 208 | route | Mt. Coronet |
| 27 | hearthome-city | Hearthome City | city | Route 208 |
| 28 | amity-square | Amity Square | zone | Hearthome (Pokémon permitido) |
| 29 | route-209 | Route 209 | route | após o rival em Hearthome |
| 30 | lost-tower | Lost Tower | building | Route 209 |
| 31 | solaceon-town | Solaceon Town | city | Route 209 |
| 32 | solaceon-ruins | Solaceon Ruins | dungeon | Solaceon |
| 33 | route-210-south | Route 210 (sul) | route | Solaceon |
| 34 | route-215 | Route 215 | route | Route 210 |
| 35 | veilstone-city | Veilstone City | city | Route 215 |
| 36 | route-212 | Route 212 | route | Relic Badge (sul de Hearthome) |
| 37 | pokemon-mansion | Pokémon Mansion | building | Route 212 |
| 38 | trophy-garden | Trophy Garden | zone | Pokémon Mansion |
| 39 | pastoria-city | Pastoria City | city | Route 212 |
| 40 | great-marsh | Great Marsh (Safari) | zone | Pastoria ($500) |
| 41 | route-213 | Route 213 | route | após o rival em Pastoria |
| 42 | valor-lakefront | Valor Lakefront | zone | Route 213 |
| 43 | route-214 | Route 214 | route | Valor Lakefront |
| 44 | ruin-maniac-cave | Ruin Maniac Cave | dungeon | Route 214 |
| 45 | route-210-north | Route 210 (norte) | route | SecretPotion + Defog |
| 46 | celestic-town | Celestic Town | city | Route 210 norte |
| 47 | route-211-east | Route 211 (leste) | route | Celestic |
| 48 | route-219 | Route 219 | sea | Surf + Relic Badge |
| 49 | route-220 | Route 220 | sea | Surf |
| 50 | route-221 | Route 221 | route | Surf |
| 51 | pal-park | Pal Park | building | Surf (uso com National Dex) |
| 52 | fuego-ironworks | Fuego Ironworks | building | Surf (Route 205) |
| 53 | route-218 | Route 218 | route | Surf |
| 54 | canalave-city | Canalave City | city | Route 218 |
| 55 | iron-island | Iron Island | dungeon | barco em Canalave |
| 56 | route-216 | Route 216 | route | Mine Badge (Mt. Coronet com Strength) |
| 57 | route-217 | Route 217 | route | Route 216 |
| 58 | acuity-lakefront | Acuity Lakefront / Lake Acuity | zone | Route 217 |
| 59 | snowpoint-city | Snowpoint City | city | Route 217 |
| 60 | lake-valor | Lake Valor | zone | após a biblioteca de Canalave |
| 61 | galactic-warehouse | Galactic Warehouse (Veilstone) | building | Storage Key |
| 62 | galactic-hq | Team Galactic HQ | building | Galactic Key |
| 63 | spear-pillar | Spear Pillar | zone | após libertar o trio (Mt. Coronet com Rock Climb) |
| 64 | route-222 | Route 222 | route | após Spear Pillar |
| 65 | sunyshore-city | Sunyshore City | city | Route 222 |
| 66 | route-223 | Route 223 | sea | Waterfall |
| 67 | victory-road | Victory Road | dungeon | Route 223 |
| 68 | pokemon-league | Pokémon League | building | Victory Road |
| 69 | spring-path | Spring Path | route | pós-jogo (National Dex) |
| 70 | turnback-cave | Turnback Cave | dungeon | Spring Path |
| 71 | full-moon-island | Fullmoon Island | zone | pós-jogo (National Dex) |
| 72 | snowpoint-temple | Snowpoint Temple | dungeon | pós-jogo (National Dex) |
| 73 | route-224 | Route 224 | route | pós-jogo (National Dex) |
| 74 | fight-area | Fight Area | city | pós-jogo (barco em Snowpoint) |
| 75 | battle-park | Battle Park / Battle Tower | building | Fight Area |
| 76 | route-225 | Route 225 | route | Fight Area |
| 77 | survival-area | Survival Area | city | Route 225 |
| 78 | route-226 | Route 226 | route | Survival Area |
| 79 | route-227 | Route 227 | route | Route 226 |
| 80 | stark-mountain | Stark Mountain | dungeon | Route 227 |
| 81 | route-228 | Route 228 | route | Route 226 |
| 82 | route-229 | Route 229 | route | Route 228 |
| 83 | resort-area | Resort Area | city | Route 229 |
| 84 | route-230 | Route 230 | sea | Route 229 / Fight Area |
| 85 | newmoon-island | Newmoon Island | zone | Member Card (evento) |
| 86 | flower-paradise | Seabreak Path / Flower Paradise | zone | Oak's Letter (evento) |
| 87 | hall-of-origin | Hall of Origin | zone | Azure Flute (evento) |

## Encontros (para `encounters/dp.json`; method `walk` salvo indicação; `conditions`: morning/day/night)

- Route 202: Shinx; Kricketot (noite e manhã).
- Route 204: Budew.
- Oreburgh Mine (`cave`): Onix (outras não listadas).
- Route 205: Buizel, Shellos (West Sea), Pachirisu.
- Eterna Forest: Buneary; noite: Murkrow [Diamond], Misdreavus [Pearl]. Pescadores com Magikarp e Goldeen.
- Route 211 (oeste): Meditite, Chingling, Ponyta; Hoothoot (noite). Caverna (Mt. Coronet): Cleffa (mais na manhã).
- Route 206: Bronzor, Kricketune; Stunky [Diamond].
- Route 208: Bibarel.
- Route 209: Gastly (noite), Chansey; Mime Jr. [Diamond], Bonsly [Pearl] (mais na manhã). `fish` Good Rod: Goldeen,
  Barboach, Magikarp 15–20.
- Lost Tower: fantasmas; Golbat; noite: Murkrow [Diamond], Misdreavus [Pearl].
- Route 215: Kadabra.
- Route 213: Shellos (East Sea), Wingull, Floatzel.
- Valor Lakefront: Girafarig.
- Ruin Maniac Cave (`cave`): Geodude; Hippopotas (chance cresce com o nº de letras de Unown).
- Route 210 (norte): Noctowl (noite).
- Route 211 (leste): Machoke, Graveler.
- Route 218: Gastrodon; Mr. Mime [Diamond], Glameow [Pearl].
- Route 216: Snover, Sneasel.
- Mt. Coronet (subida ao Spear Pillar, `cave`): lv 36–38, depois até 42; Chimecho.
- Route 222: Chatot.
- Victory Road (`cave`): Onix, Steelix.
- Old Chateau: Gastly 12–16.
- Wayward Cave (entrada oculta): Gible 15–17.
- Fuego Ironworks (fora): lv 28–30 (espécies não listadas).
- Trophy Garden: Pichu, Pikachu; pós-National Dex (Mr. Backlot, 1 por dia, 16–19): Meowth, Jigglypuff, Clefairy,
  Chansey, Eevee, Porygon, Marill, Castform, Plusle, Minun, Bonsly, Mime Jr., Happiny, Cleffa, Igglybuff, Azurill.
- Great Marsh (lv 20–26): comuns Wooper, Quagsire, Bidoof, Bibarel, Budew, Starly, Hoothoot e Noctowl (noite), Azurill,
  Marill, Psyduck; raros Staravia, Golduck, Roselia, Carnivine, Skorupi, Drapion, Croagunk, Toxicroak. `fish`: Gyarados,
  Barboach, Whiscash. Pós-National Dex: Paras, Kangaskhan, Yanma, Shroomish, Gulpin, Tropius; `fish` Carvanha.
- Solaceon Ruins (`cave`): Unown 14–25; ! e ? após as 26 letras (passagem pela Ruin Maniac Cave).
- Árvores de mel (`tree`, Honey, 9–12 h, lv 5–14): Wurmple, Combee, Burmy, Cherubi, Silcoon [Diamond], Cascoon
  [Pearl], Aipom (raro), Heracross (raro), Munchlax (muito raro).
- Spring Path: lv 52–54 (espécies não listadas).
- Route 225: Fearow, Raticate, Machoke, Roselia, Banette (noite), Skuntank [Diamond] 50–52; Spearow 20, Rattata 22.
- Route 226: Golduck.
- Route 227: Camerupt, Rhydon, Golbat (noite), Weezing, Skarmory 54–56; Numel 22, Rhyhorn 24.
- Stark Mountain (`cave`): Slugma, Magcargo.
- Route 228: Dugtrio, Rhydon, Cacturne, Hippowdon 52–54; Diglett 23, Cacnea 25.
- Route 229: Oddish, Gloom, Bellsprout, Weepinbell, Volbeat, Illumise, Ledian (manhã), Ariados (noite); Roselia e
  Scyther [Diamond]; Purugly e Pinsir [Pearl].
- Poké Radar (pós-National Dex): espécies exclusivas de cada grama (não listadas).
- Lendários e fixos (`method: fixed`):
  - Drifloon lv 22 (Valley Windworks, sextas-feiras, após Mars).
  - Dialga lv 47 [Diamond] / Palkia lv 47 [Pearl] (Spear Pillar; obrigatório).
  - Azelf lv 50 (Lake Valor, caverna; após Spear Pillar).
  - Uxie lv 50 (Lake Acuity, caverna; após Spear Pillar).
  - Mesprit lv 50 (`walk`, errante após o encontro no Lake Verity; foge após um turno; rastreado no Marking Map).
  - Cresselia lv 50 (`walk`, errante após a Fullmoon Island; National Dex).
  - Rotom lv 15 (TV do Old Chateau, 22:00–4:00, National Dex).
  - Giratina lv 70 (Turnback Cave; National Dex).
  - Heatran lv 70 (Stark Mountain, após o evento do Buck).
  - Regigigas lv 70 (Snowpoint Temple; Regirock, Regice e Registeel no time).
  - Spiritomb (Hallowed Tower, Route 209; Odd Keystone + 32 conversas no Underground; o detonado marca "não confirmado").
  - Darkrai lv 40 (Newmoon Island; Member Card).
  - Shaymin lv 30 (Flower Paradise; Oak's Letter).
  - Arceus lv 80 (Hall of Origin; Azure Flute).
  - Manaphy lv 1 (ovo do Pokémon Ranger); Phione lv 1 (ovo de Manaphy + Ditto).

## Presentes, trocas in-game, ovos e fósseis (`method: gift` / `trade`)

- Inicial: Turtwig/Chimchar/Piplup (Lake Verity; nível não listado).
- Ovo de Happiny (homem na saída de Hearthome; 10.496 passos citados; nasce lv 1).
- Ovo de Riolu (Riley, Iron Island; nasce lv 1).
- Eevee lv 5 (Bebe, Hearthome; National Dex).
- Troca: Buizel → Chatot (prédio ao lado do Poké Mart de Eterna).
- Fósseis: Explorer Kit (Eterna) para escavar no Underground; espécies não listadas.

## TMs/HMs e itens-chave (local)

- HM01 Cut (Cynthia, Eterna); HM02 Fly (Galactic Warehouse, Veilstone); HM03 Surf (velha, Celestic); HM04 Strength
  (Lost Tower, topo); HM05 Defog (Great Marsh, menina na entrada); HM06 Rock Smash (Oreburgh Gate); HM07 Waterfall
  (Jasmine, Sunyshore); HM08 Rock Climb (Route 217).
- TMs: TM10 Hidden Power (escola de Jubilife); TM67 Recycle (Eterna); TM31 (Oreburgh Gate, rampa com bike); TM78
  (Route 204, Cut); TM82 (Route 205, Cut); TM28 Dig (Ruin Maniac Cave); TM89 (Valor Lakefront, palhaço); TM24
  Thunderbolt (Valley Windworks, Surf); TM84 (Route 212 sul, Surf); TM48 (Canalave); TM29 (Route 211, Rock Climb); TM05
  (Route 213, Rock Climb); TM85 (Lake Valor, Rock Climb); TM13 Ice Beam (Route 216, Rock Climb); TM49, TM36, TM21
  (Galactic Warehouse); TM80 Rock Slide (Mt. Coronet); TM02 (Mt. Coronet, após o Galactic sair); TM25 (Lake Valor);
  TM38 (Lake Verity); TM14 (Lake Acuity); TM35 Flamethrower (Fuego Ironworks); TM70 Flash (Oreburgh Gate; à venda no
  Veilstone Department Store); TM26 Earthquake (Wayward Cave, ala oculta); TM45, TM43 (Amity Square); TM42 (Survival
  Area). Thunder, Blizzard e Fire Blast ficam acessíveis após libertar o trio (local não listado).
- Itens-chave: Running Shoes (mãe); Journal (mãe); Parcel → Town Map (rival, escola de Jubilife); Poké Balls ×5
  (assistente); Pokétch (Jubilife, 3 cupons dos palhaços); Old Rod (Jubilife, oeste); Quick Claw (Jubilife); Dusk Ball e
  Great Ball (Oreburgh); Works Key e Honey (Floaroma Meadow); Sprayduck/"Psyduck Pail" (Floaroma); Explorer Kit (Eterna);
  Bicycle (loja de Eterna); Exp. Share (Route 206, 35 vistos); VS Seeker e Dowsing Machine (assistente, Route 207);
  Odd Keystone (Route 208); Poffin Case (Fan Club, Hearthome); Shell Bell (Hearthome); Good Rod (Route 209); Oval Stone
  (Lost Tower); Seal Case (Solaceon); Coin Case (Veilstone); SecretPotion (Cynthia, Valor Lakefront); Old Charm
  (Cynthia, Route 210); Storage Key e Galactic Key (Veilstone); Master Ball (Cyrus); Adamant Orb [Diamond] / Lustrous
  Orb [Pearl] (Spear Pillar); Keystone (sul de Twinleaf, Surf); Dawn Stone e Protein (Mt. Coronet, Surf); Thunder Stone
  (Sunyshore); Rare Candy (Route 214 e Route 230); Poké Radar (Rowan, National Dex); Lunar Wing (Fullmoon Island);
  Metal Coat (Byron, Canalave); Super Rod (Fight Area); Point Card (Battle Park); Scope Lens (Fight Area); Razor Fang
  (Battle Park); Amulet Coin e Spooky Plate (Amity Square); Fresh Water (Route 225).
- Lojas: Herb Shop (Eterna); Pokémon Café, Moo-Moo Milk (Route 210); Veilstone Department Store; Dusk/Quick Ball
  (Pastoria); Timer Ball e Wise Glasses (Celestic); loja de Seals (Sunyshore); troca de BP (Battle Park).
- Move Tutors: Frenzy Plant, Blast Burn, Hydro Cannon (Route 228; inicial final em 1º); Move Deleter (Canalave).

## Mecânicas (para o guia de mecânicas, texto nosso)

- Pokétch: relógio com apps recebidos ao longo do jogo (Pokétch Company, Jubilife).
- Ciclo dia/noite pelo relógio do DS (manhã/dia/noite muda encontros; Drifloon só às sextas; policiais da 212 e 222 só à noite).
- Tag battles com parceiros que curam após cada luta.
- Honey trees: mel atrai Pokémon em 9–12 h; some após ~12 h a mais.
- Great Marsh: $500, 30 Safari Balls, 500 passos; 6 áreas por trem; binóculos ($100).
- Day Care (Solaceon): 1 EXP por passo, $100 + $100 por nível; ovo do grupo da mãe; ovos nascem lv 1.
- Poké Radar: chains na grama; shiny raro.
- Underground: escavação de fósseis/itens (Explorer Kit).
- Super Contests (Hearthome); Poffins; Amity Square com Pokémon fora da Poké Ball.
- Clima: chuva (Routes 212/213/215), neblina (Route 210, Defog), granizo (Routes 216/217), cinza (Route 227),
  tempestade de areia (Route 228).
- Pal Park: migração de 6 Pokémon por dia dos jogos de GBA (Ruby, Sapphire, Emerald, FireRed, LeafGreen), sem golpes
  de HM; capturados com Park Ball.
- Battle Tower: 7 batalhas por série, lv 50 ou 100, 3 Pokémon; Palmer na 21ª e 49ª; multi com parceiros (Cheryl, Mira,
  Riley, Marley, Buck). Proibidos: Mewtwo, Mew, Lugia, Ho-Oh, Celebi, Kyogre, Groudon, Rayquaza, Jirachi, Deoxys,
  Dialga, Palkia, Giratina, Phione, Manaphy, Darkrai, Shaymin, Arceus.
- TMs fora de batalha: Dig (sai do local) e Flash (ilumina).

## Pós-jogo (ordem de eventos)

1. Ver os 150 da Sinnoh Dex (foto de Palkia/Dialga com a velha de Celestic) → National Dex (Oak, em Sandgem) → Poké
Radar. 2. Eevee da Bebe; Rotom; Cresselia (Fullmoon Island); Giratina (Spring Path/Turnback Cave); Regigigas
(Snowpoint Temple); Route 224 (com Marley). 3. Barco em Snowpoint → Fight Area → Battle Tower. 4. Routes 225–227 →
Stark Mountain (Buck) → Survival Area → Heatran. 5. Routes 228–230 → Resort Area. 6. Eventos: Darkrai, Shaymin, Arceus,
Manaphy.

## Dúvidas

- Divisão dos Pokémon entre Mars e Jupiter na Spear Pillar fica ambígua no texto (Golbat 42 e Golbat 41).
- Níveis do inicial e dos Pokémon do Pal Park não aparecem.
- Spiritomb: o detonado diz que o método não é confirmado.
- Nomes dos TMs 31, 78, 82, 89, 84, 48, 29, 05, 85, 49, 36, 21, 02, 25, 38, 14, 45, 43 e 42 não aparecem.
- Times dos treinadores dos ginásios (não líderes) e da Battle Tower (fora do Palmer) não aparecem.
- Nível do Palmer depende da modalidade escolhida (50 ou 100).
- Item "Psyduck Pail" (tradução do detonado) corresponde ao regador do jogo (provável "Sprayduck").
