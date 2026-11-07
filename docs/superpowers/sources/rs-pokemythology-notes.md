# Ruby/Sapphire — fatos extraídos do detonado da Pokémon Mythology

Fonte: https://www.pokemythology.net/detonados/detonado-ruby-sapphire/ (PDF "Detonado Ruby/Sapphire – Pokémon
Mythology", salvo em 05/11/2026, 36 páginas). Só **fatos de jogo** (chefes, níveis, encontros, itens) no nosso formato.
Nenhuma prosa foi copiada. Golpes dos chefes **não** constam no detonado: deixar `moves: []` e `verified: false` para
golpes; espécies e níveis são `verified: true` (uma fonte) até o analista conferir numa segunda.

Vilões por versão: `[Ruby]` Team Magma (líder Maxie); `[Sapphire]` Team Aqua (líder Archie). Variantes escritas como
"Sharpedo/Camerupt" = Archie (Sapphire) / Maxie (Ruby). Inicial do rival (Brendan/May): o que tem vantagem sobre o seu.

## Ordem da história e chefes (para `bosses/rs.json`)

| Order | Id | Kind | Nome | Local | Time (espécie lv) | Cap |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | rival-1 | rival | Brendan/May | Route 103 (norte de Oldale) | inicial do rival (nível não listado) | — |
| 2 | grunt-woods | evil | Aqua/Magma Grunt | Petalburg Woods | Poochyena 9 | 9 |
| 3 | roxanne | gym | Roxanne (Stone Badge) | Rustboro City Gym | Geodude 14, Nosepass 15 | 15 |
| 4 | grunt-rusturf | evil | Aqua/Magma Grunt (ladrão do Peeko) | Rusturf Tunnel | Poochyena 11 | 11 |
| 5 | brawly | gym | Brawly (Knuckle Badge) | Dewford Town Gym | Machop 17, Makuhita 18 | 18 |
| 6 | grunts-museum | evil | Aqua/Magma Grunts | Oceanic Museum, Slateport | time não listado | — |
| 7 | rival-2 | rival | Brendan/May (inicial já evoluído) | Route 110 | time não listado (dá Itemfinder) | — |
| 8 | wally-1 | other | Wally | Mauville City (porta do ginásio) | Ralts 16 | 16 |
| 9 | wattson | gym | Wattson (Dynamo Badge) | Mauville City Gym | Magnemite 22, Voltorb 22, Magneton 23 | 23 |
| 10 | winstrates | other | Winstrate family (4 seguidas) | Route 111 (casa amarela) | time não listado | — |
| 11 | maxie-archie-1 | evil | Maxie [Ruby] / Archie [Sapphire] | Mt. Chimney (topo) | Mightyena 24, Golbat 24, Camerupt [Ruby] / Sharpedo [Sapphire] 25 | 25 |
| 12 | flannery | gym | Flannery (Heat Badge) | Lavaridge Town Gym | Slugma 26, Slugma 26, Torkoal 28 | 28 |
| 13 | norman | gym | Norman (Balance Badge) | Petalburg City Gym | Slaking 28, Vigoroth 30, Slaking 31 | 31 |
| 14 | grunts-weather | evil | Aqua/Magma Grunts | Weather Institute (Route 119) | time não listado | — |
| 15 | rival-3 | rival | Brendan/May | Route 119 (depois do Weather Institute) | time não listado (dá HM02 Fly) | — |
| 16 | kecleon-bridge | other | Kecleon (com o Devon Scope) | Route 120 (ponte) | Kecleon 30 | — |
| 17 | winona | gym | Winona (Feather Badge) | Fortree City Gym | Swellow 31, Pelipper 30, Skarmory 32, Altaria 33 | 33 |
| 18 | rival-4 | rival | Brendan/May (última batalha) | Lilycove Department Store (entrada) | time não listado | — |
| 19 | grunts-mt-pyre | evil | Aqua/Magma (topo; roubam o Orb) | Mt. Pyre | time não listado | — |
| 20 | grunts-hideout | evil | Aqua/Magma Hideout (Electrodes e Master Ball) | Hideout perto de Lilycove | time não listado | — |
| 21 | tate-liza | gym | Tate & Liza (Mind Badge; batalha dupla) | Mossdeep City Gym | Lunatone 40, Solrock 40 | 40 |
| 22 | maxie-archie-2 | evil | Maxie [Ruby] / Archie [Sapphire] | Seafloor Cavern | Mightyena 41, Crobat 41, Camerupt [Ruby] / Sharpedo [Sapphire] 43 | 43 |
| 23 | groudon-kyogre | other | Groudon [Ruby] / Kyogre [Sapphire] (capturar ou derrotar) | Cave of Origin | 45 | 45 |
| 24 | wallace | gym | Wallace (Rain Badge) | Sootopolis City Gym | Luvdisc 40, Whiscash 42, Sealeo 40, Seaking 42, Milotic 43 | 43 |
| 25 | wally-2 | other | Wally | Victory Road (saída) | Altaria 44, Delcatty 43, Magneton 41, Roselia 44, Gardevoir 45 | 45 |
| 26 | sidney | elite | Sidney | Ever Grande City (Pokémon League) | Mightyena 46, Cacturne 46, Shiftry 48, Sharpedo 48, Absol 49 | 49 |
| 27 | phoebe | elite | Phoebe | Pokémon League | Dusclops 48, Banette 49, Banette 49, Sableye 50, Dusclops 51 | 51 |
| 28 | glacia | elite | Glacia | Pokémon League | Glalie 50, Sealeo 50, Sealeo 52, Glalie 52, Walrein 53 | 53 |
| 29 | drake | elite | Drake | Pokémon League | Shelgon 52, Flygon 53, Flygon 53, Altaria 54, Salamence 55 | 55 |
| 30 | steven | champion | Steven | Pokémon League | Skarmory 57, Aggron 56, Claydol 55, Cradily 56, Armaldo 56, Metagross 58 | 58 |

Pós-jogo sem chefe de história: Battle Tower (S.S. Tidal; 7 batalhas seguidas com 3 Pokémon, categorias lv 50 e lv 100).

## Áreas (para `areas/rs.json`, kind e ordem de liberação)

| id | Nome | Kind | Quando libera |
| --- | --- | --- | --- |
| littleroot-town | Littleroot Town | city | início |
| route-101 | Route 101 | route | início |
| oldale-town | Oldale Town | city | início |
| route-103 | Route 103 | route | início (rival); parte leste com Surf |
| route-102 | Route 102 | route | depois da Pokédex |
| petalburg-city | Petalburg City | city | Route 102 |
| route-104 | Route 104 | route | Petalburg (Mr. Briney, Pretty Petal Flower Shop) |
| petalburg-woods | Petalburg Woods | route | Route 104 |
| rustboro-city | Rustboro City | city | Route 104 norte |
| devon-corporation | Devon Corporation | building | Rustboro |
| route-116 | Route 116 | route | Rustboro |
| rusturf-tunnel | Rusturf Tunnel | dungeon | Route 116; atravessar com Rock Smash |
| dewford-town | Dewford Town | city | barco do Mr. Briney |
| granite-cave | Granite Cave | dungeon | Dewford; Flash; área com Mach Bike |
| route-106 | Route 106 | sea | Dewford (Surf) |
| route-107 | Route 107 | sea | Surf |
| route-108 | Route 108 | sea | Surf (Abandoned Ship) |
| abandoned-ship | Abandoned Ship | building | Surf; completo com Dive |
| route-109 | Route 109 | sea | barco para Slateport |
| slateport-city | Slateport City | city | barco do Mr. Briney |
| oceanic-museum | Oceanic Museum | building | Slateport |
| route-110 | Route 110 | route | norte de Slateport |
| trick-house | Trick House | building | Route 110 (um desafio por insígnia) |
| cycling-road | Cycling Road | route | Route 110, com bicicleta |
| mauville-city | Mauville City | city | Route 110 |
| new-mauville | New Mauville | dungeon | Surf + Basement Key (Wattson) |
| route-117 | Route 117 | route | oeste de Mauville (Day-Care) |
| verdanturf-town | Verdanturf Town | city | Route 117 |
| route-111 | Route 111 | route | norte de Mauville; Rock Smash |
| desert | Route 111 Desert | zone | Go-Goggles (depois da Heat Badge) |
| desert-ruins | Desert Ruins | dungeon | depois do Braille da Sealed Chamber (Regirock) |
| route-112 | Route 112 | route | Route 111 |
| fiery-path | Fiery Path | dungeon | Route 112; Strength para os itens |
| route-113 | Route 113 | route | Fiery Path (cinzas; Soot Sack) |
| fallarbor-town | Fallarbor Town | city | Route 113 |
| route-114 | Route 114 | route | Fallarbor |
| meteor-falls | Meteor Falls | dungeon | Route 114; área interna com Waterfall |
| route-115 | Route 115 | route | Meteor Falls → Rustboro; área norte com Surf (Jigglypuff) |
| mt-chimney | Mt. Chimney | dungeon | Cable Car (depois de Meteor Falls) |
| jagged-pass | Jagged Pass | route | sul do Mt. Chimney; Acro Bike |
| lavaridge-town | Lavaridge Town | city | Jagged Pass |
| route-105 | Route 105 | sea | Surf (Island Cave a noroeste: Regice) |
| island-cave | Island Cave | dungeon | Route 105; depois do Braille (Regice) |
| route-118 | Route 118 | route | leste de Mauville, Surf (Good Rod) |
| route-119 | Route 119 | route | Route 118 (grama alta, Acro Bike) |
| weather-institute | Weather Institute | building | Route 119 |
| route-120 | Route 120 | route | depois do rival 3 |
| fortree-city | Fortree City | city | Route 119 |
| ancient-tomb | Ancient Tomb | dungeon | Route 120; depois do Braille (Registeel) |
| route-121 | Route 121 | route | depois da Feather Badge |
| safari-zone | Safari Zone | zone | Route 121; Pokéblock Case |
| route-122 | Route 122 | sea | Surf |
| mt-pyre | Mt. Pyre | dungeon | Route 122 |
| route-123 | Route 123 | route | Surf ao sul do Mt. Pyre (Berry Master) |
| lilycove-city | Lilycove City | city | Route 121 |
| lilycove-dept-store | Lilycove Department Store | building | Lilycove |
| hideout | Team Aqua [Sapphire] / Magma [Ruby] Hideout | building | depois do porto de Slateport |
| route-124 | Route 124 | sea | depois do Hideout (Wailmer saem) |
| mossdeep-city | Mossdeep City | city | Route 124 |
| route-125 | Route 125 | sea | norte de Mossdeep |
| shoal-cave | Shoal Cave | dungeon | Route 125; maré pelo relógio |
| route-127 | Route 127 | sea | Mossdeep (Dive) |
| route-126 | Route 126 | sea | Dive (Relicanth) |
| sootopolis-city | Sootopolis City | city | Dive (Route 126) |
| route-128 | Route 128 | sea | Dive (Corsola, Luvdisc) |
| seafloor-cavern | Seafloor Cavern | dungeon | Dive na Route 128 |
| cave-of-origin | Cave of Origin | dungeon | depois do Seafloor Cavern |
| route-129 | Route 129 | sea | Surf |
| route-130 | Route 130 | sea | Surf |
| route-131 | Route 131 | sea | Surf |
| pacifidlog-town | Pacifidlog Town | city | Surf |
| route-132 | Route 132 | sea | correntezas a oeste de Pacifidlog |
| route-133 | Route 133 | sea | correntezas |
| route-134 | Route 134 | sea | correntezas (Sealed Chamber com Dive) |
| sealed-chamber | Sealed Chamber | dungeon | Dive na Route 134; Dig; Relicanth 1º e Wailord último |
| ever-grande-city | Ever Grande City | city | Waterfall na Route 128 |
| victory-road | Victory Road | dungeon | 8 insígnias; Flash, Rock Smash, Strength, Surf, Waterfall |
| pokemon-league | Pokémon League | building | Victory Road |
| ss-tidal | S.S. Tidal | building | pós-jogo (S.S. Ticket) |
| battle-tower | Battle Tower | building | pós-jogo (S.S. Tidal; depois por Fly) |
| sky-pillar | Sky Pillar | dungeon | pós-jogo; Mach Bike (perto de Pacifidlog) |
| southern-island | Southern Island | zone | evento (Eon Ticket, Lilycove) |

## Encontros (para `encounters/rs.json`; method `walk` salvo indicação)

- Route 116: Skitty (raro).
- Route 115 (área norte, Surf): Jigglypuff.
- New Mauville: Magnemite, Voltorb e evoluções (raras); Voltorbs disfarçados de itens (`fixed`).
- Route 111 Desert (areia que deixa pegadas): selvagens (espécies não listadas).
- Route 120: Kecleon fixo lv 30 (`fixed`, ponte; Devon Scope). Outro Kecleon bloqueia o ginásio de Fortree.
- Route 126 (`dive`, use `walk` com nota "Dive"): Relicanth.
- Route 128 (`fish`, Super Rod; também surfando): Corsola, Luvdisc.
- Wailmer (`fish`, Super Rod; local não especificado) → evoluir para Wailord.
- Shoal Cave: Spheal; Snorunt 10% (caverna de gelo, maré baixa).
- Victory Road: selvagens lv 35–45 (espécies não listadas).
- Sky Pillar: selvagens por volta do lv 50.
- Safari Zone (raridades do detonado):
  - Área sul-direita: Pikachu 5%, Oddish 40%, Gloom 5%, Doduo 10%, Natu 10%, Wobbuffet 10%, Girafarig 20%.
  - Área sul-esquerda: mesma tabela; `fish` (Super Rod) Goldeen, Seaking; `surf` Psyduck.
  - Área Mach Bike (norte-esquerda): Oddish 30%, Gloom 15%, Doduo 15%, Dodrio 5%, Rhyhorn 30%, Pinsir 5%; `fish` Goldeen, Seaking; `surf` Psyduck, Golduck 5%.
  - Área Acro Bike (norte-direita): Oddish 30%, Gloom 15%, Natu 15%, Xatu 5%, Heracross 5%, Phanpy 30%; `rock_smash` Geodude.
- Lendários (`method: fixed` salvo indicação):
  - Groudon [Ruby] / Kyogre [Sapphire] lv 45, Cave of Origin, depois do Seafloor Cavern (obrigatório capturar ou derrotar).
  - Latias/Latios errante (`walk`/`surf`, nunca em cavernas nem debaixo d'água), depois do Hall of Fame; um por versão (ver Dúvidas).
  - Latias/Latios da outra versão lv 50, Southern Island (Eon Ticket, evento).
  - Rayquaza, topo do Sky Pillar, pós-jogo (nível não listado).
  - Regirock lv 40, Desert Ruins (Braille; 2 passos à direita, 2 ao sul, Strength).
  - Regice lv 40, Island Cave (Route 105; ler o Braille e esperar parado).
  - Registeel lv 40, Ancient Tomb (Route 120; no centro, usar Fly).
  - Requisito dos Regis: 7ª insígnia, Dive, Pokémon com Dig, Relicanth 1º e Wailord último no time (Sealed Chamber).

## Presentes, trocas in-game, fósseis, ovos e iniciais (`method: gift` / `trade`)

- Inicial: Treecko / Torchic / Mudkip (Prof. Birch, Route 101; nível não listado).
- Wynaut (ovo, velha das fontes termais de Lavaridge).
- Castform (Weather Institute, depois dos vilões; no máximo 5 no time).
- Beldum lv 5 (casa do Steven, Mossdeep, pós-jogo).
- Fósseis no deserto (extremo nordeste; escolher um): Root Fossil → Lileep; Claw Fossil → Anorith (o detonado diz "Cradily"). Revividos na Devon Corp. 2F (Rustboro), com no máximo 5 no time.
- Troca citada: Seedot oferecido na Pokémon Trainer's School (Rustboro; só em pergunta da Trick House, pedido não listado).
- Master Ball (Hideout dos vilões).

## TMs, HMs e itens-chave (localização)

| Item | Local / condição |
| --- | --- |
| Pokédex, 5 Poké Balls | Prof. Birch, depois do rival 1 |
| Running Shoes | Mãe, Oldale Town |
| Wailmer Pail | Pretty Petal Flower Shop (Route 104) |
| HM01 Cut | Rustboro City (morador) |
| PokéNav | Devon Corp. (depois de recuperar o Devon Goods) |
| Letter, Devon Goods | Devon Corp. (entregar a Steven e ao Capt. Stern) |
| HM05 Flash | Granite Cave, primeira pessoa (uso fora de batalha com a Knuckle Badge) |
| Old Rod | Pescador de Dewford |
| Soda Pop ×6 | Casa da praia de Slateport (3 treinadores) |
| Pokéblock Case | Pokémon Contest Hall, Slateport |
| Itemfinder | Rival, Route 110 |
| Mach Bike / Acro Bike | Rydel's Cycles, Mauville (troca quando quiser) |
| HM06 Rock Smash | Casa em Mauville |
| Coin Case | Casa em Mauville, em troca do Harbor Mail |
| Contest Pass | Contest Hall de Verdanturf |
| HM04 Strength | Casal no Rusturf Tunnel (depois do Rock Smash) |
| Repeat Ball | Homem da Devon, saída do Rusturf Tunnel |
| Soot Sack | Glass Workshop, Route 113 (flautas com cinzas) |
| TM28 Dig | Menina em Fallarbor (casa a oeste) |
| Nugget | Cratera de Fallarbor |
| Meteorite | Máquina do Mt. Chimney (devolver ao Prof. Cozmo em Fallarbor) |
| Charcoal | Herb Shop, Lavaridge |
| Go-Goggles | Rival, Lavaridge |
| HM03 Surf | Casa ao lado do ginásio de Petalburg (Wally's father) |
| Amulet Coin | Mãe, Littleroot (depois do Norman) |
| Good Rod | Pescador da Route 118 |
| HM02 Fly | Rival, Route 119 |
| Devon Scope | Steven, ponte da Route 120 |
| Red Orb / Blue Orb | Velha do Mt. Pyre (o oposto do que o líder levou) |
| Super Rod | Mossdeep City |
| HM08 Dive | Steven, Mossdeep |
| HM07 Waterfall | Cave of Origin (Wallace) |
| Basement Key | Wattson, Mauville (depois do Surf) |
| TM24 Thunderbolt | Wattson, depois de desligar o gerador do New Mauville |
| Storage Key, TM13 Ice Beam | Abandoned Ship |
| Dive Ball, Luxury Ball, Water Stone, TM18 Rain Dance | Abandoned Ship (cabines) |
| Scanner | Abandoned Ship, cabine 2 → Capt. Stern troca por DeepSeaTooth ou DeepSeaScale |
| S.S. Ticket | Norman, casa em Littleroot (pós-jogo) |
| Leftovers | Lixeira do S.S. Tidal |
| Shell Bell | Velho da Shoal Cave (4 Shoal Shell + 4 Shoal Salt) |
| Eon Ticket | evento (Nintendo) |

Trick House (Route 110), prêmio por desafio: 1 (depois do Brawly) Rare Candy; 2 (Wattson) Timer Ball; 3 (Flannery, Rock
Smash) Everstone; 4 (Norman, Strength) Smoke Ball; 5 (Winona, quiz) TM12 Taunt; 6 (Tate & Liza) Magnet; 7 (Wallace)
PP Max; 8 (Elite Four) Red Tent [Ruby] / Blue Tent [Sapphire].

Outros: Move Tutor de Fallarbor (golpe esquecido por Heart Scale); casa a nordeste de Mossdeep troca Shards por pedras
evolutivas; Berry Master (Route 123) dá Berries.

## Mecânicas (para o guia de mecânicas, texto nosso)

- Relógio interno (acertado no início): Berries crescem com o tempo; marés da Shoal Cave: alta 9h–15h e 21h–3h, baixa 15h–21h e 3h–9h.
- Batalhas duplas contra pares de treinadores (a partir de Petalburg Woods).
- Berries: árvores, plantio com Wailmer Pail.
- Pokéblocks e Contests (Contest Pass); condição do Pokémon no PokéNav.
- PokéNav: mapa, condição, registro de treinadores (revanches), fitas.
- Bicicletas: Mach (rampas e chão rachado) e Acro (pular, trilhos estreitos).
- Itens escondidos (Itemfinder) e cinzas da Route 113 (flautas com Soot Sack).
- Dive em manchas de água escura; cidades e cavernas submersas.
- Clima: Castform muda com sol/chuva/granizo; Groudon (Drought) / Kyogre (Drizzle).
- Braille nas câmaras dos Regis.
- Day-Care na Route 117 (até 2 Pokémon; ovos).
- Trick House: 8 desafios liberados por insígnia.
- Safari Zone: 30 Safari Balls e passos limitados; 4 áreas (2 com bicicleta).
- Battle Tower: 7 batalhas com 3 Pokémon, lv 50 e lv 100.

## Pós-jogo (ordem de eventos)

1. Hall of Fame (Steven); recomeça em casa; Norman dá o S.S. Ticket.
2. Latias/Latios errante por Hoenn (anúncio na TV).
3. Beldum lv 5 na casa do Steven (Mossdeep).
4. S.S. Tidal (Slateport ↔ Lilycove), Leftovers, Capt. Briney → Battle Tower.
5. Trick House 8º desafio (Tent).
6. Sky Pillar → Rayquaza.
7. Opcionais a qualquer hora depois da 7ª insígnia e do Dive: Regis; com Eon Ticket: Southern Island.

## Dúvidas

- Times dos rivais (Brendan/May) nas quatro batalhas, dos Grunts/Admins e da Winstrate family: o texto não lista.
- Latias/Latios errante: o detonado diz Latias no Sapphire e Latios no Ruby, e na Southern Island diz Latias no Ruby (o que contradiz a primeira frase); confirmar a versão de cada um.
- Fóssil: o texto diz que o Claw Fossil vira "Cradily" (deve ser Anorith; Cradily é evolução do Lileep).
- O detonado chama o Peeko de "Peecko" e o Devon Scope de "um item que serve para identificá-los".
- Níveis de Rayquaza e dos iniciais/Castform/Wynaut não aparecem.
- Local exato da Super Rod, do HM01 Cut e do HM06 Rock Smash: só "uma casa da cidade".
- Quem dá o HM08 Dive: o texto diz "fuce nas casas" e depois atribui ao Steven.
