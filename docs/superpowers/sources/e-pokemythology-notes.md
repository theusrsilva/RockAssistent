# Emerald — fatos extraídos do detonado da Pokémon Mythology

Fonte: https://www.pokemythology.net/detonados/detonado-emerald/ (PDF "Detonado Emerald – Pokémon Mythology", salvo em
05/11/2026, 81 páginas). Só **fatos de jogo** (chefes, níveis, encontros, itens) no nosso formato. Nenhuma prosa foi
copiada. Golpes dos chefes **não** constam no detonado, com exceção dos Frontier Brains (listados abaixo, com item e
golpes): nos demais, deixar `moves: []` e `verified: false` para golpes; espécies e níveis são `verified: true` (uma
fonte) até o analista conferir numa segunda.

Inicial do rival (Brendan/May): o que tem vantagem sobre o seu. No Emerald, Team Magma e Team Aqua aparecem os dois;
admins citados: Shelly (Aqua) e Tabitha (Magma).

## Ordem da história e chefes (para `bosses/e.json`)

| Order | Id | Kind | Nome | Local | Time (espécie lv) | Cap |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | rival-1 | rival | Brendan/May | Route 103 | inicial do rival (nível não listado; só golpes normais) | — |
| 2 | grunt-woods | evil | Aqua Grunt | Petalburg Woods | Poochyena 9 | 9 |
| 3 | roxanne | gym | Roxanne (Stone Badge) | Rustboro City Gym | Geodude 12, Geodude 12, Nosepass 15 | 15 |
| 4 | grunt-rusturf | evil | Aqua Grunt (ladrão do Peeko) | Rusturf Tunnel | Poochyena 11 | 11 |
| 5 | rival-2 | rival | Brendan/May | Rustboro City (sul) | time não listado | — |
| 6 | brawly | gym | Brawly (Knuckle Badge) | Dewford Town Gym | Machop 16, Meditite 16, Makuhita 19 | 19 |
| 7 | grunts-museum | evil | Aqua Grunts (2) | Oceanic Museum 2F, Slateport | Carvanha 15; Zubat 14, Carvanha 14 | 15 |
| 8 | rival-3 | rival | Brendan/May | Route 110 | time não listado (dá Itemfinder) | — |
| 9 | wally-1 | other | Wally | Mauville City (porta do ginásio) | Ralts 16 | 16 |
| 10 | wattson | gym | Wattson (Dynamo Badge) | Mauville City Gym | Voltorb 20, Electrike 20, Magneton 22, Manectric 24 | 24 |
| 11 | winstrates | other | Winstrate family (4 seguidas) | Route 111 (casa amarela) | time não listado | — |
| 12 | tabitha-1 | evil | Tabitha (Magma Admin) | Mt. Chimney | time não listado | — |
| 13 | maxie-1 | evil | Maxie | Mt. Chimney (topo) | time não listado | — |
| 14 | magma-jagged | evil | Magma Grunt (virado para a parede) | Jagged Pass | time não listado | — |
| 15 | flannery | gym | Flannery (Heat Badge) | Lavaridge Town Gym | Numel 24, Slugma 24, Camerupt 26, Torkoal 29 | 29 |
| 16 | norman | gym | Norman (Balance Badge) | Petalburg City Gym | Spinda 27, Vigoroth 27, Linoone 29, Slaking 31 | 31 |
| 17 | shelly-1 | evil | Shelly (Aqua Admin) | Weather Institute 2F | time não listado | — |
| 18 | rival-4 | rival | Brendan/May | Route 119 | time não listado (dá HM02 Fly) | — |
| 19 | kecleon-bridge | other | Kecleon (com o Devon Scope) | Route 120 (ponte) | Kecleon 30 | — |
| 20 | winona | gym | Winona (Feather Badge) | Fortree City Gym | Swablu 29, Tropius 29, Pelipper 30, Skarmory 31, Altaria 33 | 33 |
| 21 | rival-5 | rival | Brendan/May (última batalha) | Lilycove Department Store (entrada) | time não listado | — |
| 22 | archie-mt-pyre | evil | Team Aqua (topo; levam o Red Orb) | Mt. Pyre | time não listado | — |
| 23 | tabitha-2 | evil | Tabitha | Magma Hideout (Jagged Pass) | time não listado | — |
| 24 | maxie-2 | evil | Maxie (Groudon desperta) | Magma Hideout | time não listado | — |
| 25 | aqua-hideout-admin | evil | Aqua Admin + Electrodes (Master Ball) | Aqua Hideout (Lilycove) | time não listado | — |
| 26 | tate-liza | gym | Tate & Liza (Mind Badge; dupla) | Mossdeep City Gym | Claydol 41, Xatu 41, Lunatone 42, Solrock 42 | 42 |
| 27 | space-center | evil | Magma Grunts ×3 e depois Maxie + Tabitha (dupla com Steven) | Mossdeep Space Center | times não listados | — |
| 28 | shelly-2 | evil | Shelly | Seafloor Cavern | time não listado | — |
| 29 | archie-final | evil | Archie (Kyogre desperta) | Seafloor Cavern | time não listado | — |
| 30 | rayquaza-wake | other | Rayquaza (só despertar; sem batalha) | Sky Pillar (topo) | — | — |
| 31 | juan | gym | Juan (Rain Badge) | Sootopolis City Gym | Luvdisc 41, Whiscash 41, Sealeo 43, Crawdaunt 43, Kingdra 46 | 46 |
| 32 | wally-2 | other | Wally | Victory Road | Altaria 44, Delcatty 43, Magneton 41, Roselia 44, Gardevoir 45 | 45 |
| 33 | sidney | elite | Sidney | Pokémon League | Mightyena 46, Absol 49, Shiftry 48, Cacturne 46, Crawdaunt 48 | 49 |
| 34 | phoebe | elite | Phoebe | Pokémon League | Dusclops 48, Banette 49, Sableye 50, Banette 49, Dusclops 51 | 51 |
| 35 | glacia | elite | Glacia | Pokémon League | Glalie 50, Sealeo 50, Sealeo 52, Glalie 52, Walrein 53 | 53 |
| 36 | drake | elite | Drake | Pokémon League | Shelgon 52, Flygon 53, Altaria 54, Salamence 55, Kingdra 53 | 55 |
| 37 | wallace | champion | Wallace | Pokémon League | Wailord 57, Ludicolo 56, Tentacruel 55, Whiscash 56, Gyarados 56, Milotic 58 | 58 |
| 38 | steven-meteor-falls | other | Steven (pós-jogo) | Meteor Falls (sala interna, Surf + Waterfall) | Skarmory 77, Claydol 75, Cradily 76, Armaldo 76, Aggron 76, Metagross 78 | 78 |
| 39 | wally-rematch | other | Wally (revanches pelo Match Call) | Victory Road (fim) | mesma equipe da order 32, níveis maiores (não listados) | — |
| 40 | sudowoodo | other | Sudowoodo (fixo, único; Wailmer Pail) | Battle Frontier | Sudowoodo 40 | — |

### Revanches dos líderes (pós-jogo, Match Call; sempre em dupla; 4 por líder)

Divisão entre revanches deduzida pela ordem do texto (a tabela original é imagem; ver Dúvidas).

| Id | Líder | 1ª | 2ª | 3ª | 4ª |
| --- | --- | --- | --- | --- | --- |
| roxanne-r | Roxanne | Golem 32, Kabuto 35, Onix 35, Nosepass 37 | Omanyte 37, Golem 37, Kabutops 40, Onix 40, Nosepass 42 | Omastar 42, Golem 42, Kabutops 45, Onix 45, Nosepass 47 | Aerodactyl 47, Omastar 47, Golem 47, Kabutops 50, Onix 50, Nosepass 52 |
| brawly-r | Brawly | Machamp 33, Meditite 33, Hitmontop 35, Hariyama 37 | Machamp 38, Medicham 38, Hitmontop 40, Hariyama 42 | Hitmonchan 40, Machamp 43, Medicham 43, Hitmontop 45, Hariyama 47 | Hitmonlee 46, Hitmonchan 46, Machamp 48, Medicham 48, Hitmontop 50, Hariyama 52 |
| wattson-r | Wattson | Mareep 36, Electrode 36, Magneton 38, Manectric 40 | Pikachu 39, Flaaffy 41, Electrode 41, Magneton 43, Manectric 45 | Raichu 44, Ampharos 46, Electrode 46, Magneton 48, Manectric 50 | Electabuzz 50, Raichu 51, Ampharos 51, Electrode 53, Magneton 53, Manectric 55 |
| flannery-r | Flannery | Magcargo 38, Ponyta 36, Camerupt 38, Torkoal 40 | Growlithe 41, Magcargo 43, Ponyta 41, Camerupt 43, Torkoal 45 | Houndour 46, Growlithe 46, Magcargo 48, Rapidash 46, Camerupt 48, Torkoal 50 | Arcanine 51, Magcargo 53, Houndoom 51, Rapidash 51, Camerupt 53, Torkoal 55 |
| norman-r | Norman | Chansey 42, Slaking 42, Spinda 43, Slaking 45 | Chansey 47, Slaking 47, Kangaskhan 45, Spinda 48, Slaking 50 | Slaking 52, Blissey 52, Kangaskhan 50, Spinda 53, Slaking 55 | Slaking 57, Blissey 57, Kangaskhan 55, Tauros 57, Spinda 58, Slaking 60 |
| winona-r | Winona | Dratini 40, Tropius 38, Pelipper 41, Skarmory 43, Altaria 45 | Hoothoot 43, Dragonair 45, Tropius 43, Pelipper 46, Skarmory 48, Altaria 50 | Noctowl 48, Tropius 49, Dragonair 50, Pelipper 51, Skarmory 53, Altaria 55 | Noctowl 53, Tropius 54, Pelipper 55, Dragonite 55, Skarmory 58, Altaria 60 |
| tate-liza-r | Tate & Liza | Slowpoke 48, Claydol 49, Xatu 49, Lunatone 50, Solrock 50 | Drowzee 53, Slowpoke 53, Claydol 54, Xatu 54, Lunatone 55, Solrock 55 | Hypno 58, Claydol 59, Slowpoke 58, Xatu 59, Lunatone 60, Solrock 60 | Hypno 63, Claydol 64, Slowking 63, Xatu 64, Lunatone 65, Solrock 65 |
| juan-r | Juan | Poliwag 46, Whiscash 46, Walrein 48, Crawdaunt 48, Kingdra 51 | Poliwhirl 50, Whiscash 51, Walrein 53, Crawdaunt 53, Kingdra 56 | Lapras 56, Whiscash 58, Poliwhirl 56, Walrein 58, Crawdaunt 58, Kingdra 61 | Lapras 61, Whiscash 63, Politoed 61, Walrein 63, Crawdaunt 63, Kingdra 66 |

A Elite Four repete sempre o mesmo time e níveis.

### Frontier Brains (Battle Frontier; golpes listados pelo detonado → `verified: true`)

| Id | Nome | Instalação | Símbolo | Time (espécie – item – golpes) |
| --- | --- | --- | --- | --- |
| lucy-silver | Pike Queen Lucy | Battle Pike (28 salas) | Luck (prata) | Seviper – Quick Claw – Crunch/Poison Fang/Swagger/Giga Drain; Shuckle – Chesto Berry – Toxic/Sandstorm/Protect/Rest; Milotic – Leftovers – Surf/Ice Beam/Recover/Mirror Coat |
| lucy-gold | Pike Queen Lucy | Battle Pike (140 salas) | Luck (ouro) | Seviper – Focus Band – Crunch/Sludge Bomb/Swagger/Giga Drain; Gyarados – Chesto Berry – Return/Dragon Dance/Rest/Roar; Steelix – Brightpowder – Rock Slide/Earthquake/Explosion/Screech |
| tucker-silver | Dome Ace Tucker | Battle Dome (19 vitórias seguidas; só simples) | Tactics (prata) | Swampert – Focus Band – Surf/Ice Beam/Earthquake/Counter; Salamence – Lum Berry – Brick Break/Aerial Ace/Earthquake/Dragon Claw; Charizard – White Herb – Overheat/Rock Slide/Earthquake/Aerial Ace |
| tucker-gold | Dome Ace Tucker | Battle Dome (39 seguidas) | Tactics (ouro) | Swampert – Leftovers – Surf/Ice Beam/Earthquake/Mirror Coat; Latias – Chesto Berry – Thunderbolt/Psychic/Calm Mind/Rest; Metagross – Quick Claw – Psychic/Meteor Mash/Earthquake/Protect |
| greta-silver | Arena Tycoon Greta | Battle Arena (27 vitórias) | Guts (prata) | Heracross – Salac Berry – Megahorn/Rock Tomb/Reversal/Endure; Umbreon – Leftovers – Body Slam/Psychic/Confuse Ray/Faint Attack; Shedinja – Brightpowder – Shadow Ball/Return/Aerial Ace/Confuse Ray |
| greta-gold | Arena Tycoon Greta | Battle Arena (55 vitórias) | Guts (ouro) | Umbreon – Chesto Berry – Double-Edge/Psychic/Confuse Ray/Rest; Gengar – Leftovers – Destiny Bond/Psychic/Hypnosis/Dream Eater; Breloom – Lum Berry – Spore/Focus Punch/Headbutt/Giga Drain |
| noland-silver | Factory Head Noland | Battle Factory (20 vitórias; só simples) | Knowledge (prata) | Pokémon aleatórios (alugados) |
| noland-gold | Factory Head Noland | Battle Factory (41 vitórias) | Knowledge (ouro) | Pokémon aleatórios (alugados) |
| anabel-silver | Salon Maiden Anabel | Battle Tower (34 seguidas; só simples) | Ability (prata) | Alakazam – Brightpowder – ThunderPunch/Ice Punch/Fire Punch/Disable; Entei – Lum Berry – Fire Blast/Return/Calm Mind/Roar; Snorlax – Quick Claw – Body Slam/Shadow Ball/Yawn/Belly Drum |
| anabel-gold | Salon Maiden Anabel | Battle Tower (69 seguidas) | Ability (ouro) | Raikou – Lum Berry – Thunderbolt/Calm Mind/Reflect/Rest; Snorlax – Chesto Berry – Return/Shadow Ball/Rest/Curse; Latios – Brightpowder – Psychic/Dragon Claw/Calm Mind/Recover |
| spenser-silver | Palace Maven Spenser | Battle Palace (20 vitórias; só simples) | Spirits (prata) | Crobat – Brightpowder – Confuse Ray/Toxic/Double Team/Fly; Slaking – Scope Lens – Shadow Ball/Brick Break/Earthquake/Swagger; Lapras – Quick Claw – Horn Drill/Ice Beam/Protect/Confuse Ray |
| spenser-gold | Palace Maven Spenser | Battle Palace (41 vitórias) | Spirits (ouro) | Arcanine – White Herb – Overheat/ExtremeSpeed/Protect/Roar; Slaking – Scope Lens – Shadow Ball/Brick Break/Earthquake/Swagger; Suicune – King's Rock – Surf/Blizzard/Bite/Calm Mind |
| brandon-silver | Pyramid King Brandon | Battle Pyramid (21 andares) | Brave (prata) | Regirock – Quick Claw – AncientPower/Superpower/Earthquake/Explosion; Regice – Leftovers – Ice Beam/Thunder/Amnesia/Rest; Registeel – Chesto Berry – Earthquake/Metal Claw/Iron Defense/Toxic |
| brandon-gold | Pyramid King Brandon | Battle Pyramid (70 andares) | Brave (ouro) | Articuno – Scope Lens – Blizzard/Water Pulse/Aerial Ace/Reflect; Zapdos – Lum Berry – Thunder/Drill Peck/Light Screen/Detect; Moltres – Brightpowder – Fire Blast/Hyper Beam/Aerial Ace/Safeguard |

Níveis dos Frontier Brains: categoria lv 50 (todos lv 50) ou Open Level (nível do seu Pokémon mais alto, mínimo lv 60;
na Factory, lv 100). Scott dá Lansat Berry (7 símbolos de prata), Starf Berry (7 de ouro), Silver Shield (50 vitórias
seguidas na Battle Tower) e Gold Shield (100).

## Áreas (para `areas/e.json`, kind e ordem de liberação)

| id | Nome | Kind | Quando libera |
| --- | --- | --- | --- |
| littleroot-town | Littleroot Town | city | início |
| route-101 | Route 101 | route | início |
| oldale-town | Oldale Town | city | início |
| route-103 | Route 103 | route | início (rival); parte leste com Surf |
| route-102 | Route 102 | route | depois da Pokédex |
| petalburg-city | Petalburg City | city | Route 102 |
| route-104 | Route 104 | route | Petalburg (cabana do Mr. Briney, Pretty Petal Flower Shop) |
| petalburg-woods | Petalburg Woods | route | Route 104 |
| rustboro-city | Rustboro City | city | Route 104 norte |
| devon-corporation | Devon Corporation | building | Rustboro |
| route-116 | Route 116 | route | Rustboro |
| rusturf-tunnel | Rusturf Tunnel | dungeon | Route 116; atravessar com Rock Smash |
| dewford-town | Dewford Town | city | barco do Mr. Briney |
| granite-cave | Granite Cave | dungeon | Dewford; Flash; área com Mach Bike |
| route-106 | Route 106 | sea | Surf |
| route-107 | Route 107 | sea | Surf |
| route-108 | Route 108 | sea | Surf (Abandoned Ship) |
| abandoned-ship | Abandoned Ship | building | Surf; completo com Dive |
| route-109 | Route 109 | sea | barco para Slateport |
| slateport-city | Slateport City | city | barco do Mr. Briney |
| oceanic-museum | Oceanic Museum | building | Slateport |
| slateport-battle-tent | Slateport Battle Tent | building | Slateport (Pokémon alugados) |
| route-110 | Route 110 | route | norte de Slateport |
| trick-house | Trick House | building | Route 110 (um desafio por insígnia) |
| cycling-road | Cycling Road | route | Route 110, com bicicleta |
| mauville-city | Mauville City | city | Route 110 |
| new-mauville | New Mauville | dungeon | Surf + Basement Key (Wattson) |
| route-117 | Route 117 | route | oeste de Mauville (Day-Care) |
| verdanturf-town | Verdanturf Town | city | Route 117 |
| verdanturf-battle-tent | Verdanturf Battle Tent | building | Verdanturf (Pokémon escolhem o golpe) |
| route-111 | Route 111 | route | norte de Mauville; Rock Smash |
| trainer-hill | Trainer Hill | building | pós-jogo (Route 111, perto dos Winstrate) |
| desert | Route 111 Desert | zone | Go-Goggles (depois da Heat Badge) |
| mirage-tower | Mirage Tower | dungeon | deserto; Mach Bike e Rock Smash (aparece e some) |
| desert-ruins | Desert Ruins | dungeon | depois do Braille da Sealed Chamber (Regirock) |
| route-112 | Route 112 | route | Route 111 |
| fiery-path | Fiery Path | dungeon | Route 112; Strength para os itens |
| route-113 | Route 113 | route | Fiery Path (cinzas; Soot Sack) |
| fallarbor-town | Fallarbor Town | city | Route 113 |
| fallarbor-battle-tent | Fallarbor Battle Tent | building | Fallarbor (estilo Arena) |
| route-114 | Route 114 | route | Fallarbor |
| meteor-falls | Meteor Falls | dungeon | Route 114; área interna com Waterfall; sala do Steven no pós-jogo |
| route-115 | Route 115 | route | Meteor Falls → Rustboro; área norte com Surf (Jigglypuff) |
| mt-chimney | Mt. Chimney | dungeon | Cable Car (depois de Meteor Falls) |
| jagged-pass | Jagged Pass | route | sul do Mt. Chimney; Acro Bike |
| magma-hideout | Magma Hideout | dungeon | Jagged Pass, com Magma Emblem (depois do Mt. Pyre) |
| lavaridge-town | Lavaridge Town | city | Jagged Pass |
| route-105 | Route 105 | sea | Surf (Island Cave: Regice) |
| island-cave | Island Cave | dungeon | Route 105; depois do Braille (Regice) |
| route-118 | Route 118 | route | leste de Mauville, Surf (Good Rod) |
| route-119 | Route 119 | route | Route 118 (grama alta, Acro Bike) |
| weather-institute | Weather Institute | building | Route 119 |
| route-120 | Route 120 | route | depois do rival 4 |
| fortree-city | Fortree City | city | Route 119 |
| ancient-tomb | Ancient Tomb | dungeon | Route 120; depois do Braille (Registeel) |
| route-121 | Route 121 | route | depois da Feather Badge |
| safari-zone | Safari Zone | zone | Route 121; Pokéblock Case; áreas norte e sul novas no pós-jogo |
| route-122 | Route 122 | sea | Surf |
| mt-pyre | Mt. Pyre | dungeon | Route 122 |
| route-123 | Route 123 | route | Surf ao sul do Mt. Pyre (Berry Master) |
| lilycove-city | Lilycove City | city | Route 121 (único Contest Hall) |
| lilycove-dept-store | Lilycove Department Store | building | Lilycove |
| aqua-hideout | Aqua Hideout | dungeon | depois do porto de Slateport |
| route-124 | Route 124 | sea | depois do Aqua Hideout |
| mossdeep-city | Mossdeep City | city | Route 124 |
| mossdeep-space-center | Mossdeep Space Center | building | depois da Mind Badge |
| route-125 | Route 125 | sea | norte de Mossdeep |
| shoal-cave | Shoal Cave | dungeon | Route 125; maré pelo relógio |
| route-127 | Route 127 | sea | Mossdeep (Dive) |
| route-126 | Route 126 | sea | Dive (Relicanth) |
| sootopolis-city | Sootopolis City | city | Dive (Route 126) |
| route-128 | Route 128 | sea | Dive (Corsola, Luvdisc) |
| seafloor-cavern | Seafloor Cavern | dungeon | Dive na Route 128; Rock Smash e Strength |
| cave-of-origin | Cave of Origin | dungeon | depois do Seafloor Cavern (Wallace) |
| route-129 | Route 129 | sea | Surf |
| route-130 | Route 130 | sea | Surf |
| route-131 | Route 131 | sea | Surf |
| pacifidlog-town | Pacifidlog Town | city | Surf |
| sky-pillar | Sky Pillar | dungeon | depois da Cave of Origin; captura do Rayquaza com Mach Bike |
| route-132 | Route 132 | sea | correntezas a oeste de Pacifidlog |
| route-133 | Route 133 | sea | correntezas |
| route-134 | Route 134 | sea | correntezas (Sealed Chamber com Dive) |
| sealed-chamber | Sealed Chamber | dungeon | Dive na Route 134; Dig; Wailord 1º e Relicanth último |
| mirage-island | Mirage Island | zone | aleatória (número diário; velho em Pacifidlog avisa) |
| ever-grande-city | Ever Grande City | city | Waterfall na Route 128 |
| victory-road | Victory Road | dungeon | 8 insígnias; Flash, Rock Smash, Strength, Surf, Waterfall |
| pokemon-league | Pokémon League | building | Victory Road |
| desert-underpass | Desert Underpass | dungeon | pós-jogo (casa a noroeste de Fallarbor; segundo fóssil) |
| terra-cave | Terra Cave | dungeon | pós-jogo (Weather Institute indica a rota: 114, 115, 116 ou 118) |
| marine-cave | Marine Cave | dungeon | pós-jogo (Weather Institute indica a rota: 105, 125, 127 ou 129) |
| ss-tidal | S.S. Tidal | building | pós-jogo (S.S. Ticket) |
| battle-frontier | Battle Frontier | zone | pós-jogo (S.S. Tidal + Scott; depois por Fly) |
| artisan-cave | Artisan Cave | dungeon | Battle Frontier, depois do Sudowoodo |
| southern-island | Southern Island | zone | evento (Eon Ticket, Lilycove) |
| navel-rock | Navel Rock | dungeon | evento (Mystic Ticket, Lilycove) |
| birth-island | Birth Island | zone | evento (Aurora Ticket, Lilycove) |
| faraway-island | Faraway Island | zone | evento (Old Sea Map, Lilycove) |

## Encontros (para `encounters/e.json`; method `walk` salvo indicação)

- Route 116: Skitty (raro).
- Route 115 (área norte, Surf): Jigglypuff.
- New Mauville: Magnemite, Voltorb e evoluções (raras); Voltorbs disfarçados de itens (`fixed`).
- Route 111 Desert (areia que deixa pegadas): selvagens (espécies não listadas).
- Mt. Pyre: Pokémon fantasmas (espécies não listadas).
- Route 120: Kecleon fixo lv 30 (`fixed`, ponte; capturável). Kecleon do ginásio de Fortree foge.
- Routes 124/126 (Dive): Relicanth.
- Route 128 (`fish`, Super Rod; também surfando): Corsola, Luvdisc.
- Wailmer (`fish`, Super Rod, "quase todo lugar") → Wailord.
- Shoal Cave: Spheal; Snorunt 10% (caverna de gelo, maré baixa).
- Victory Road: selvagens lv 35–45 (espécies não listadas).
- Sky Pillar: selvagens por volta do lv 30.
- Mirage Island: Wynaut lv 5–50; Berries novas.
- Artisan Cave: Smeargle até lv 50.
- Safari Zone (raridades do detonado):
  - Área sul-direita: Pikachu 5%, Oddish 40%, Gloom 5%, Doduo 10%, Natu 10%, Wobbuffet 10%, Girafarig 20%.
  - Área sul-esquerda: mesma tabela; `fish` (Super Rod) Goldeen, Seaking; `surf` Psyduck.
  - Área Mach Bike (norte-esquerda): Oddish 30%, Gloom 15%, Doduo 15%, Dodrio 5%, Rhyhorn 30%, Pinsir 5%; `fish` Goldeen, Seaking; `surf` Psyduck, Golduck 5%.
  - Área Acro Bike (norte-direita): Oddish 30%, Gloom 15%, Natu 15%, Xatu 5%, Heracross 5%, Phanpy 30%; `rock_smash` Geodude.
  - Área Norte (pós-jogo): Hoothoot 5%, Ledyba 10%, Aipom 30%, Sunkern 10%, Pineco 5%, Teddiursa 30%, Houndour 5%, Miltank 5%; `rock_smash` Shuckle.
  - Área Sul (pós-jogo): Hoothoot 5%, Spinarak 10%, Mareep 30%, Aipom 10%, Sunkern 30%, Gligar 5%, Snubbull 5%, Stantler 5%; `fish` Goldeen, Magikarp, Remoraid, Octillery 1% (Super Rod); `surf` Marill, Wooper, Quagsire 1%.
- Lendários (`method: fixed` salvo indicação):
  - Rayquaza lv 70, topo do Sky Pillar (depois de acalmar Groudon e Kyogre; Mach Bike; antes da Elite Four).
  - Groudon lv 70, Terra Cave; Kyogre lv 70, Marine Cave (pós-jogo; rota indicada pelo cientista do Weather Institute 2F; a entrada some depois de certo número de passos).
  - Latias (resposta "red" à mãe) ou Latios ("blue") lv 50, errante (`walk`/`surf`, nunca em cavernas nem debaixo d'água), pós-jogo; foge no primeiro golpe.
  - O outro (Latios/Latias) lv 50, Southern Island (Eon Ticket).
  - Regirock lv 40, Desert Ruins (Braille; 2 passos para baixo, 2 para a esquerda, Rock Smash).
  - Regice lv 40, Island Cave (Route 105; Braille e uma volta completa na sala em sentido horário).
  - Registeel lv 40, Ancient Tomb (Route 120; no centro, usar Flash).
  - Requisito dos Regis: 7ª insígnia, Dive, Pokémon com Dig, Wailord 1º e Relicanth último no time (Sealed Chamber).
  - Ho-Oh lv 70 (esquerda) e Lugia lv 70 (direita), Navel Rock (Mystic Ticket).
  - Deoxys (Speed Forme) lv 30, Birth Island (Aurora Ticket; triângulo).
  - Mew lv 30, Faraway Island (Old Sea Map).
  - Sudowoodo lv 40, Battle Frontier (Wailmer Pail; único).

## Presentes, trocas in-game, fósseis, ovos e iniciais (`method: gift` / `trade`)

- Inicial: Treecko / Torchic / Mudkip (Prof. Birch, Route 101; nível não listado).
- Wynaut (ovo, velha das fontes termais de Lavaridge; no máximo 5 no time).
- Castform (Weather Institute, depois da Shelly; no máximo 5 no time).
- Beldum lv 5 (casa do Steven, Mossdeep, pós-jogo).
- Iniciais de Johto: Chikorita / Cyndaquil / Totodile, um à escolha (Prof. Birch, depois de capturar os 200 da Hoenn Dex, sem Jirachi e Deoxys).
- Fósseis: Mirage Tower (topo, Rock Smash; escolher um): Root Fossil → Lileep, Claw Fossil → Anorith; o outro no Desert Underpass (pós-jogo). Revividos lv 20 na Devon Corp. 2F (no máximo 5 no time).
- Master Ball (Aqua Hideout; sala com 4 bolas: 2 Electrode, Nugget e a Master Ball). Loto (Lilycove Dept. Store) pode dar Master Ball.
- Trocas in-game: não citadas no detonado.

## TMs, HMs e itens-chave (localização)

| Item | Local / condição |
| --- | --- |
| Running Shoes | Mãe, ao sair para Oldale |
| Pokédex, Poké Balls | Prof. Birch / Brendan-May, depois do rival 1 |
| Wailmer Pail | Pretty Petal Flower Shop (Route 104) |
| HM01 Cut | Casa ao lado do Pokémon Center de Rustboro |
| PokéNav, Letter, Devon Goods | Mr. Stone, Devon Corp. |
| Match Call | PokéNav (depois da ligação de teste) |
| Old Rod | Pescador de Dewford |
| HM05 Flash | Homem na entrada da Granite Cave |
| TM47 Steel Wing | Steven, Granite Cave (entrega da Letter) |
| Exp. Share | Mr. Stone, Devon Corp. |
| TM46 (Thief) | Último Aqua Grunt do Oceanic Museum (nome do golpe não citado) |
| Soda Pop ×6 | Casa da praia de Slateport (3 treinadores) |
| Harbor Mail | Mart de Slateport → Coin Case (menina na 1ª casa de Mauville) |
| Itemfinder | Rival, Route 110 |
| HM06 Rock Smash | Casa ao lado do Mart de Mauville |
| Mach Bike / Acro Bike | Rydel's Cycles, Mauville (troca quando quiser) |
| HM04 Strength | Casal no Rusturf Tunnel |
| Repeat Ball | Homem da Devon, saída do Rusturf Tunnel (depois Repeat/Timer Ball à venda em Rustboro) |
| TM43 Secret Power | Garoto perto da árvore, Route 111/112 |
| Soot Sack | Glass Workshop, Route 113 |
| TM28 Dig | Menina em Fallarbor (casa a oeste) |
| Nugget | Cratera de Fallarbor |
| Meteorite | Máquina do Mt. Chimney (devolver ao Prof. Cozmo → TM, número não listado) |
| Charcoal | Herb Shop, Lavaridge |
| Go-Goggles | Rival, Lavaridge |
| HM03 Surf | Pai do Wally, Petalburg |
| Amulet Coin | Mãe, Littleroot (depois do Norman) |
| Good Rod | Pescador da Route 118 |
| HM02 Fly | Rival, Route 119 |
| Devon Scope | Steven, ponte da Route 120 |
| Pokéblock Case | Contest Hall, Lilycove |
| Magma Emblem | Velha do Mt. Pyre |
| HM08 Dive | Steven, casa em Mossdeep (depois do Space Center) |
| Super Rod | Casa perto do Space Center, Mossdeep |
| HM07 Waterfall | Wallace, Sootopolis (depois do Rayquaza) |
| Basement Key | Wattson, Mauville (depois do Surf) |
| TM24 Thunderbolt | Wattson, depois de desligar o gerador do New Mauville |
| Storage Key, TM13 Ice Beam | Abandoned Ship |
| Dive Ball, Luxury Ball, Water Stone, TM18 Rain Dance | Abandoned Ship (cabines) |
| Scanner | Abandoned Ship, cabine 2 → Capt. Stern troca por DeepSeaTooth ou DeepSeaScale |
| National Pokédex | Prof. Birch, logo depois do Hall of Fame |
| S.S. Ticket | Norman, casa em Littleroot (pós-jogo) |
| Leftovers | Lixeira do S.S. Tidal |
| Frontier Pass | Battle Frontier (entrada) |
| Shell Bell | Velho da Shoal Cave (4 Shoal Shell + 4 Shoal Salt) |
| Eon / Mystic / Aurora Ticket, Old Sea Map | eventos da Nintendo |

Battle Tents (Slateport, Verdanturf, Fallarbor): um homem dá um TM em cada (números não listados). Trick House (Route
110), prêmio por desafio: 1 (Brawly) Rare Candy; 2 (Wattson) Timer Ball; 3 (Flannery) Everstone; 4 (Norman) Smoke Ball;
5 (Winona) TM12 Taunt; 6 (Tate & Liza) Magnet; 7 (Juan) PP Max; 8 (Elite Four) Red Tent ou Blue Tent (à escolha).

Outros: Move Tutor de Fallarbor (golpe esquecido por Heart Scale); Lanette no Pokémon Center de Fallarbor (PC); casa
perto de Mossdeep troca Shards por pedras evolutivas; Berry Master (Route 123) dá Berries.

## Mecânicas (para o guia de mecânicas, texto nosso)

- Relógio interno: Berries; marés da Shoal Cave (alta 9h–15h e 21h–3h; baixa 15h–21h e 3h–9h).
- Match Call do PokéNav: treinadores pedem revanche; líderes de ginásio voltam com times novos (4 revanches, em dupla) no pós-jogo.
- Batalhas duplas frequentes; Tate & Liza é a única dupla de líderes; Space Center em dupla com Steven.
- Secret Bases (TM43 Secret Power).
- Battle Tents no lugar dos Contests de Slateport, Verdanturf e Fallarbor; único Contest Hall em Lilycove (20 medalhas).
- Pokéblocks e Safari Zone (30 Safari Balls, passos limitados).
- Bicicletas: Mach (rampas, chão rachado) e Acro (pular, trilhos).
- Dive em manchas de água escura.
- Clima: Castform; Groudon e Kyogre disputando o tempo (chuva e sol) até o Rayquaza.
- Braille nas câmaras dos Regis (ordem invertida em relação ao R/S: Wailord primeiro).
- Mirage Tower e Mirage Island aparecem só às vezes.
- Trainer Hill (pós-jogo): 4 andares, 2 treinadores por andar em dupla, nível igual ao seu mais alto; modos Normal, Variety, Unique e Expert; prêmio conforme o tempo.
- Battle Frontier: 7 instalações (Tower, Dome, Palace, Arena, Factory, Pike, Pyramid), 3 Pokémon, lv 50 ou Open Level, Battle Points, Battle Record; lendários proibidos: Mewtwo, Mew, Lugia, Ho-Oh, Celebi, Kyogre, Groudon, Rayquaza, Jirachi, Deoxys.
- Day-Care na Route 117; ovos chocam com passos.

### Trainer Hill: Pokémon por modo e andar

| Modo | 1º | 2º | 3º | 4º |
| --- | --- | --- | --- | --- |
| Normal | Flareon, Magneton, Pinsir, Misdreavus, Claydol, Solrock | Meditite, Heracross, Hitmontop, Mr. Mime, Plusle, Togepi | Vaporeon, Dodrio, Omastar, Nidoqueen, Charizard, Ninetales | Alakazam, Grumpig, Blissey, Tropius, Bellossom, Meganium |
| Variety | Delibird, Clefairy, Pikachu, Corsola, Starmie, Clamperl | Illumise, Spheal, Vigoroth, Jigglypuff, Jynx, Exploud | Wooper, Poliwag, Psyduck, Baltoy, Pineco, Voltorb | Unown K, P, G, O, L, N |
| Unique | Sunflora, Venusaur, Tangela, Magcargo, Rapidash, Moltres | Smeargle ×3, Cubone, Beedrill, Raticate | Cleffa, Wynaut, Magby, Charmeleon, Wartortle, Ivysaur | Houndoom, Stantler, Articuno, Sudowoodo, Slowking, Entei |
| Expert | Snorlax, Miltank, Ursaring, Gengar, Alakazam, Gardevoir | Dusclops, Ninetales, Banette, Swellow, Machamp, Ursaring | Wobbuffet, Exploud, Crobat, Dewgong, Politoed, Marowak | Forretress, Electrode, Exeggutor, Gengar, Dusclops, Misdreavus |

## Pós-jogo (ordem de eventos)

1. Hall of Fame (Wallace); recomeça em casa; Norman dá o S.S. Ticket; mãe pergunta a cor (red → Latias, blue → Latios errante).
2. Prof. Birch dá a National Pokédex.
3. Beldum lv 5 (Mossdeep); Desert Underpass (segundo fóssil); Steven no Meteor Falls; Trainer Hill; áreas norte e sul do Safari Zone.
4. Revanches dos líderes pelo Match Call; Trick House 8º desafio.
5. Groudon e Kyogre pelo Weather Institute (Terra Cave e Marine Cave).
6. S.S. Tidal: Scott libera o Battle Frontier; Frontier Pass; Sudowoodo e Artisan Cave.
7. Símbolos de prata (7 Frontier Brains) → Lansat Berry; símbolos de ouro → Starf Berry; Silver/Gold Shield na Battle Tower.
8. Iniciais de Johto depois de completar a Hoenn Dex.
9. Eventos: Southern Island, Navel Rock, Birth Island, Faraway Island.

## Dúvidas

- Times de Maxie, Archie, Tabitha, Shelly, dos rivais (exceto Wally) e da Winstrate family: o texto não lista.
- Revanches dos líderes: no texto a tabela vem embaralhada (era imagem); a divisão entre a 1ª e a 4ª revanche foi deduzida pela ordem e pelos níveis; conferir numa segunda fonte (em especial Roxanne 3ª/4ª, Brawly 2ª/3ª e Winona 1ª/2ª).
- Níveis das revanches do Wally não listados ("sobe alguns níveis").
- Southern Island: o texto diz Latias se respondeu blue e Latios se respondeu red (oposto ao errante), coerente; confirmar.
- Nome do golpe do TM46 (o texto só diz "TM 46") e o TM do Prof. Cozmo e dos Battle Tents: não citados.
- O texto escreve "Ludiculo" (Ludicolo), "Poliwhril" (Poliwhirl), "Manetric" (Manectric), "Wartotle" (Wartortle) e "Pochyena" (Poochyena).
- Local exato do TM43 Secret Power: "garoto perto da árvore" no caminho da Fiery Path (Route 111/112).
