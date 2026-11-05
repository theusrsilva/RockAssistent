# Omega Ruby / Alpha Sapphire — fatos extraídos do detonado da Pokémon Mythology

Fonte: https://www.pokemythology.net/detonados/detonado-omegaruby-alphasapphire/ (PDF salvo em 05/11/2026, 164 páginas).
Só **fatos de jogo** (chefes, níveis, encontros, itens) no nosso formato. Nenhuma prosa foi copiada. Golpes dos
chefes **não** constam no detonado: deixar `moves: []` e `verified: false` para golpes; espécies e níveis são
`verified: true` (uma fonte) até o analista conferir numa segunda. Exclusivos: `[OR]` Omega Ruby, `[AS]` Alpha Sapphire.
Equipe vilã: Magma em OR, Aqua em AS (admins Tabitha/Courtney em OR, Shelly/Matt em AS).

## Ordem da história e chefes (para `bosses/oras.json`)

Variantes do rival (Brendan/May) por inicial escolhido: Treecko / Torchic / Mudkip.

| Order | Id | Kind | Nome | Local | Time (espécie lv) | Cap |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | rival-1 | rival | Brendan/May | Route 103 (norte de Oldale) | Treecko→Torchic 5; Torchic→Mudkip 5; Mudkip→Treecko 5 | 5 |
| 2 | roxanne | gym | Roxanne (Stone Badge) | Rustboro Gym | Geodude 12, Nosepass 14 | 14 |
| 3 | brawly | gym | Brawly (Knuckle Badge) | Dewford Gym | Machop 14, Makuhita 16 | 16 |
| 4 | rival-2 | rival | Brendan/May | Route 110 (depois da Trick House) | Treecko: Wailmer 18, Combusken 20; Torchic: Shroomish 18, Marshtomp 20; Mudkip: Slugma 18, Grovyle 20 | 20 |
| 5 | wally-1 | rival | Wally | Mauville City (porta do ginásio) | Ralts 17 | 17 |
| 6 | wattson | gym | Wattson (Dynamo Badge) | Mauville Gym | Magnemite 19, Voltorb 19, Magneton 21 | 21 |
| 7 | admin-meteor-falls | evil | Tabitha [OR] / Shelly [AS] + Grunt (dupla com o rival) | Meteor Falls | OR: Mightyena 23, Numel 23 + Grunt Koffing 23; AS: Mightyena 23, Carvanha 23 + Grunt Grimer 23 | 23 |
| 8 | admin-chimney | evil | Tabitha [OR] / Shelly [AS] | Mt. Chimney | OR: Koffing 24, Numel 24; AS: Grimer 24, Carvanha 24 | 24 |
| 9 | boss-chimney | evil | Maxie [OR] / Archie [AS] | Mt. Chimney | OR: Mightyena 25, Golbat 25, Camerupt 27; AS: Mightyena 25, Golbat 25, Sharpedo 27 | 27 |
| 10 | flannery | gym | Flannery (Heat Badge) | Lavaridge Gym | Slugma 26, Numel 26, Torkoal 28 | 28 |
| 11 | norman | gym | Norman (Balance Badge) | Petalburg Gym | Slaking 28, Vigoroth 28, Slaking 30 | 30 |
| 12 | admin-southern-island | evil | Courtney [OR] / Matt [AS] + Grunt (dupla com Steven) | Southern Island | OR: Camerupt 31 + Grunt Koffing 29; AS: Sharpedo 31 + Grunt Grimer 29 | 31 |
| 13 | admin-weather | evil | Tabitha [OR] / Shelly [AS] | Weather Institute | OR: Camerupt 32; AS: Sharpedo 32 | 32 |
| 14 | rival-3 | rival | Brendan/May | Route 119 (ponte depois do Weather Institute) | Treecko: Wailmer 31, Shroomish 31, Combusken 33; Torchic: Shroomish 31, Slugma 31, Marshtomp 33; Mudkip: Slugma 31, Wailmer 31, Grovyle 33 | 33 |
| 15 | kecleon-steven | other | Kecleon (Devon Scope, com Steven) | Route 120 (ponte) | Kecleon 30 | — (não conta para cap) |
| 16 | winona | gym | Winona (Feather Badge) | Fortree Gym | Swellow 33, Pelipper 33, Skarmory 33, Altaria 35 | 35 |
| 17 | rival-4 | rival | Brendan/May | Lilycove City (Dept. Store) | Treecko: Swellow 37, Wailord 37, Breloom 37, Blaziken 39; Torchic: Swellow 37, Breloom 37, Magcargo 37, Swampert 39; Mudkip: Swellow 37, Magcargo 37, Wailord 37, Sceptile 39 | 39 |
| 18 | admin-mt-pyre | evil | Courtney [OR] / Matt [AS] | Mt. Pyre (topo) | OR: Camerupt 38; AS: Sharpedo 38 | 38 |
| 19 | admin-hideout | evil | Courtney [OR] / Matt [AS] | Team Magma/Aqua Hideout (Lilycove) | OR: Camerupt 39; AS: Sharpedo 39 | 39 |
| 20 | tate-liza | gym | Tate & Liza (Mind Badge, dupla) | Mossdeep Gym | Lunatone 45, Solrock 45 | 45 |
| 21 | boss-seafloor | evil | Maxie [OR] / Archie [AS] | Seafloor Cavern | OR: Mightyena 41, Crobat 41, Weezing 41, Mega Camerupt 43; AS: Mightyena 41, Crobat 41, Muk 41, Mega Sharpedo 43 | 43 |
| 22 | primal-legend | other | Primal Groudon [OR] / Primal Kyogre [AS] | Cave of Origin | 45 | — |
| 23 | wallace-gym | gym | Wallace (Rain Badge) | Sootopolis Gym | Luvdisc 44, Whiscash 44, Sealeo 44, Seaking 44, Milotic 46 | 46 |
| 24 | wally-2 | rival | Wally | Victory Road (perto da saída) | Altaria 46, Delcatty 46, Magneton 46, Roselia 46, Mega Gallade 48 | 48 |
| 25 | sidney-1 | elite | Sidney | Pokémon League | Mightyena 50, Shiftry 50, Cacturne 50, Sharpedo 50, Absol 52 | 52 |
| 26 | phoebe-1 | elite | Phoebe | Pokémon League | Dusclops 51, Banette 51, Sableye 51, Banette 51, Dusknoir 53 | 53 |
| 27 | glacia-1 | elite | Glacia | Pokémon League | Glalie 52, Froslass 52, Glalie 52, Froslass 52, Walrein 54 | 54 |
| 28 | drake-1 | elite | Drake | Pokémon League | Altaria 53, Flygon 53, Kingdra 53, Flygon 53, Salamence 55 | 55 |
| 29 | steven-1 | champion | Steven | Pokémon League | Skarmory 57, Aggron 57, Claydol 57, Cradily 57, Armaldo 57, Mega Metagross 59 | 59 |
| 30 | rival-5 | rival | Brendan/May (após os créditos) | Route 103 (local da 1ª batalha) | Treecko: Swellow 48, Wailord 48, Breloom 48, Raichu 48, Mega Blaziken 50; Torchic: Swellow 48, Breloom 48, Magcargo 48, Raichu 48, Mega Swampert 50; Mudkip: Swellow 48, Magcargo 48, Wailord 48, Raichu 48, Mega Sceptile 50 | 50 |
| 31 | admin-delta-wally | evil | Courtney [OR] / Matt [AS] (Delta Episode) | Casa do Wally, Petalburg | OR: Camerupt 53; AS: Sharpedo 53 | 53 |
| 32 | zinnia-1 | other | Zinnia (Delta Episode) | Granite Cave (sala das gravuras) | Tyrantrum 55, Altaria 55, Salamence 57 | 57 |
| 33 | admin-space-center | evil | Courtney [OR] / Matt [AS] + Grunt (dupla com Steven) | Mossdeep Space Center | OR: Mega Camerupt 55 + Grunt Mightyena 51, Weezing 51; AS: Mega Sharpedo 55 + Grunt Mightyena 51, Muk 51 | 55 |
| 34 | wallace-sky-pillar | other | Wallace (Delta Episode) | Sky Pillar (Route 131) | Wailord 55, Ludicolo 55, Tentacruel 55, Whiscash 55, Gyarados 55, Milotic 57 | 57 |
| 35 | rayquaza | other | Rayquaza (captura) | Sky Pillar (topo) | Rayquaza 70 | — |
| 36 | zinnia-2 | other | Zinnia (com Mega Rayquaza) | Sky Pillar (topo) | Goodra 60, Tyrantrum 60, Altaria 60, Noivern 60, Mega Salamence 62 | 62 |
| 37 | deoxys | other | Deoxys (espaço) | Espaço (após o asteroide) | Deoxys 80 | — |
| 38 | sidney-2 | elite | Sidney (revanche) | Pokémon League | Scrafty 70, Shiftry 70, Sharpedo 70, Zoroark 70, Mandibuzz 70, Mega Absol 72 | 72 |
| 39 | phoebe-2 | elite | Phoebe (revanche) | Pokémon League | Banette 71, Mismagius 71, Drifblim 71, Chandelure 71, Dusknoir 71, Mega Sableye 73 | 73 |
| 40 | glacia-2 | elite | Glacia (revanche) | Pokémon League | Abomasnow 72, Beartic 72, Froslass 72, Vanilluxe 72, Walrein 72, Mega Glalie 74 | 74 |
| 41 | drake-2 | elite | Drake (revanche) | Pokémon League | Altaria 73, Dragalge 73, Kingdra 73, Flygon 73, Haxorus 73, Mega Salamence 55 (sic no detonado) | 73 |
| 42 | steven-2 | champion | Steven (revanche) | Pokémon League | Skarmory 77, Claydol 77, Carbink 77, Aerodactyl 77, Aggron 77, Mega Metagross 79 | 79 |
| 43 | nita-20 | other | Battle Chatelaine Nita (Single, 20ª) | Battle Maison | Wigglytuff 50, Purugly 50, Grumpig 50 | 50 |
| 44 | evelyn-20 | other | Battle Chatelaine Evelyn (Double, 20ª) | Battle Maison | Primeape 50, Persian 50, Pachirisu 50, Lumineon 50 | 50 |
| 45 | dana-20 | other | Battle Chatelaine Dana (Triple, 20ª) | Battle Maison | Dragalge 50, Whimsicott 50, Magcargo 50, Magneton 50, Piloswine 50, Girafarig 50 (ver Dúvidas) | 50 |
| 46 | morgan-20 | other | Battle Chatelaine Morgan (Rotation, 20ª) | Battle Maison | Sawsbuck 50, Mantine 50, Klefki 50, Swalot 50 | 50 |
| 47 | nita-50 | other | Battle Chatelaine Nita (Super Single, 50ª) | Battle Maison | Landorus 50, Tornadus 50, Thundurus 50 | 50 |
| 48 | evelyn-50 | other | Battle Chatelaine Evelyn (Super Double, 50ª) | Battle Maison | Suicune 50, Entei 50, Raikou 50, Latios 50 | 50 |
| 49 | dana-50 | other | Battle Chatelaine Dana (Super Triple, 50ª) | Battle Maison | Articuno 50, Zapdos 50, Moltres 50, Regirock 50, Regice 50, Registeel 50 (ver Dúvidas) | 50 |
| 50 | morgan-50 | other | Battle Chatelaine Morgan (Super Rotation, 50ª) | Battle Maison | Cobalion 50, Terrakion 50, Virizion 50, Latias 50 | 50 |
| 51 | wally-3 | rival | Wally (1ª revanche; após 1ª estátua na Battle Maison) | Battle Maison | Altaria 64, Delcatty 64, Magnezone 64, Roserade 64, Mega Gallade 66 | 66 |
| 52 | wally-4 | rival | Wally (revanche diária) | Battle Maison | Roserade 79, Talonflame 79, Azumarill 79, Magnezone 79, Garchomp 79, Mega Gallade 81 | 81 |

Outros combates fixos citados sem time: horda de 5 Poochyena no Hideout (nível baixo, sem número); 4 treinadores da
Winstrate Family (Route 111, nomes com V) e o 5º na Victory Road; 3 grunts seguidos na Devon Corp. (Delta Episode).
Mauville Food Court: Village Sub Combo (singles lv 17, 5 turnos, Nugget); pós-Liga: Magnemite Croquette (duplas lv 55,
6 turnos, Pearl String) e Mauville Ramen Bowl (trios lv 65, 8 turnos, Big Nugget; 9º treinador lv 75 se cada vitória
levar 1 turno). Battle Maison: lv 50 fixo; castelã na 20ª (normal) e na 50ª (super).

## Áreas (para `areas/oras.json`, kind e ordem de liberação)

Ordem de progressão do detonado. Números de rota entre parênteses só quando o detonado cita o número; os demais
seguem a posição descrita (ver Dúvidas).

| Id | Nome | Kind | Liberada em |
| --- | --- | --- | --- |
| littleroot-town | Littleroot Town | city | início |
| route-101 | Route 101 (Littleroot→Oldale) | route | início |
| oldale-town | Oldale Town | city | início |
| route-103 | Route 103 (norte de Oldale) | route | início |
| route-102 | Route 102 (Oldale→Petalburg) | route | após Pokédex |
| petalburg-city | Petalburg City | city | após Pokédex |
| route-104 | Route 104 (praia do Mr. Briney) | route | após Petalburg |
| petalburg-woods | Petalburg Woods | dungeon | após Petalburg |
| pretty-petal-flower-shop | Pretty Petal Flower Shop | building | após Petalburg Woods |
| rustboro-city | Rustboro City (Devon Corp., Trainer School) | city | após Petalburg Woods |
| route-116 | Route 116 (leste de Rustboro) | route | após Roxanne |
| rusturf-tunnel | Rusturf Tunnel | dungeon | após Roxanne (atravessar com Rock Smash) |
| dewford-town | Dewford Town | city | barco do Mr. Briney |
| granite-cave | Granite Cave | dungeon | após Brawly (área da Mach Bike depois) |
| route-109 | Route 109 (praia de Slateport) | route | barco do Mr. Briney |
| slateport-city | Slateport City (Oceanic Museum, Contest Spectacular, Fan Club) | city | após Granite Cave |
| route-110 | Route 110 (Trick House, Cycling Road) | route | após Slateport |
| trick-house | Trick House | building | 1 desafio por insígnia; completa após a 5ª |
| mauville-city | Mauville City (Rydel's Cycles, Food Court, Battle Institute) | city | após Route 110 |
| cycling-road | Cycling Road | route | com bicicleta |
| route-117 | Route 117 (Day Care) | route | após Wattson |
| verdanturf-town | Verdanturf Town | city | após Wattson |
| route-111 | Route 111 (Winstrate Family) | route | após Wattson (deserto só com Go-Goggles) |
| route-112 | Route 112 (Cable Car) | route | após Wattson |
| fiery-path | Fiery Path | dungeon | após Wattson (área com Strength depois) |
| route-113 | Route 113 (cinzas, Soot Sack) | route | após Fiery Path |
| fallarbor-town | Fallarbor Town (casa do Cozmo, Move Maniac) | city | após Route 113 |
| route-114 | Route 114 (casa da Lanette) | route | após Fallarbor |
| meteor-falls | Meteor Falls | dungeon | após Fallarbor (área alta com Waterfall) |
| mt-chimney | Mt. Chimney (Cable Car) | zone | após Meteor Falls |
| jagged-pass | Jagged Pass | route | após Mt. Chimney (Acro Bike) |
| lavaridge-town | Lavaridge Town | city | após Jagged Pass |
| route-111-desert | Route 111 — deserto (fósseis) | zone | com Go-Goggles (após Flannery) |
| route-105-108 | Routes 105–108 (mar entre Petalburg, Dewford e Slateport) | sea | com Surf (após Norman) |
| sea-mauville | Sea Mauville (navio afundado) | dungeon | com Surf; completo com Dive |
| route-118 | Route 118 (leste de Mauville, Good Rod) | route | com Surf |
| southern-island | Southern Island | zone | evento com Steven (após Route 118) |
| route-123 | Route 123 (casa do Berry Master) | route | após Southern Island |
| route-119 | Route 119 (Weather Institute, Feebas) | route | com Surf |
| weather-institute | Weather Institute | building | Route 119 |
| fortree-city | Fortree City | city | após Weather Institute |
| route-120 | Route 120 (Scorched Slab, Kecleon) | route | após Fortree |
| scorched-slab | Scorched Slab | dungeon | Route 120 (Heatran no pós-Primal) |
| route-121 | Route 121 (Safari Zone) | route | após Winona |
| safari-zone | Safari Zone | zone | após Winona (áreas com Mach/Acro Bike) |
| lilycove-city | Lilycove City (Dept. Store, Contest Hall, Cove Lily Hotel) | city | após Safari Zone |
| route-122 | Route 122 (mar até o Mt. Pyre) | sea | após Lilycove |
| mt-pyre | Mt. Pyre | dungeon | após Lilycove |
| team-hideout | Team Magma/Aqua Hideout | dungeon | após Mt. Pyre |
| route-124 | Route 124 (mar Lilycove→Mossdeep) | sea | após Hideout |
| mossdeep-city | Mossdeep City (casa do Steven, Space Center) | city | após Hideout |
| route-125 | Route 125 | sea | após Hideout |
| shoal-cave | Shoal Cave (maré alta/baixa) | dungeon | após Hideout |
| route-126 | Route 126 (Dive até Sootopolis) | sea | com Dive (após Tate & Liza) |
| route-127 | Route 127 | sea | com Dive |
| route-128 | Route 128 | sea | com Dive |
| seafloor-cavern | Seafloor Cavern | dungeon | com Dive |
| sootopolis-city | Sootopolis City | city | com Dive |
| cave-of-origin | Cave of Origin | dungeon | após Seafloor Cavern |
| routes-129-131 | Routes 129–131 (mar ao sul) | sea | com Surf (após Mossdeep) |
| pacifidlog-town | Pacifidlog Town | city | com Surf (após Mossdeep) |
| routes-132-134 | Routes 132–134 (correntezas até Slateport) | sea | após Pacifidlog |
| mauville-hills | Mauville Hills | building | após Wallace |
| new-mauville | New Mauville | dungeon | após Wallace (Mauville Hills) |
| sealed-chamber | Sealed Chamber (Route 134, Dive) | dungeon | após Wallace |
| desert-ruins | Desert Ruins (Route 111) | dungeon | após Sealed Chamber |
| island-cave | Island Cave (Route 105) | dungeon | após Sealed Chamber |
| ancient-tomb | Ancient Tomb (Route 120) | dungeon | após Sealed Chamber |
| ever-grande-city | Ever Grande City | city | com Waterfall (Route 128) |
| victory-road | Victory Road | dungeon | Ever Grande |
| pokemon-league | Pokémon League | building | após Victory Road |
| mirage-spots | Mirage Spots (Island, Cave, Forest, Mountain) | zone | após Primal (Eon Flute), 1 por dia |
| sky-pillar | Sky Pillar (Route 131) | dungeon | Delta Episode |
| mossdeep-space-center | Mossdeep Space Center | building | Delta Episode |
| ss-tidal | S.S. Tidal | building | após Delta Episode (S.S. Ticket) |
| battle-resort | Battle Resort (Battle Maison, 2º Day Care) | zone | após Delta Episode |

## Encontros (para `encounters/oras.json`; method `walk` salvo indicação)

Na história (o detonado não dá níveis de selvagens nem raridade, salvo onde indicado):
- Route 101: Zigzagoon, Poochyena, Wurmple. Poochyena pelo DexNav com Thunder Fang (Treecko) / Ice Fang (Torchic) / Fire Fang (Mudkip) (`method: dexnav`).
- Route 103: Wingull (além dos da 101).
- Route 102: Surskit, Ralts, Seedot [OR], Lotad [AS]. Ralts capturado por Wally (evento).
- Route 104: Taillow (raro).
- Petalburg Woods: Silcoon, Cascoon, Shroomish, Slakoth.
- Route 116: Whismur, Nincada, Skitty.
- Granite Cave: Zubat, Geodude, Makuhita, Abra; área da Mach Bike: Aron, Mawile [OR], Sableye [AS] (`method: cave`).
- Route 110: Electrike, Oddish, Gulpin, Voltorb, Minun [OR], Plusle [AS]; Magnemite (`method: horde`).
- Route 117: Roselia, Marill, Volbeat, Illumise.
- Route 112: Numel, Machop.
- Fiery Path: Grimer, Torkoal, Slugma, Koffing (`method: cave`).
- Route 113: Spinda, Sandshrew, Skarmory.
- Route 114: Swablu; Nuzleaf, Zangoose [OR]; Lombre, Seviper [AS].
- Meteor Falls: Solrock [OR], Lunatone [AS] (`method: cave`); Bagon na área do Waterfall.
- Jagged Pass: Spoink.
- Route 111 deserto (areia escura): Trapinch, Cacnea, Baltoy.
- Surf em lagos: Marill, Azumarill, Surskit, Masquerain. Surf no mar: Wingull, Tentacool, Pelipper, Tentacruel (`method: surf`).
- Route 118/119 (grama alta): Linoone, Kecleon; mais ao norte Gloom, Tropius.
- Route 119: Feebas (`method: fish`): 5% em qualquer ponto com qualquer vara; 100% na sombra da ponte 06:00–20:00; 100% em volta da pedra 20:00–06:00.
- Route 120: Absol; Kecleon fixo lv 30 (Devon Scope, `method: fixed`); outros Kecleon invisíveis pela região.
- Safari Zone: norte-esquerda Gloom, Doduo, Psyduck, Rhyhorn, Heracross; norte-direita + Donphan, Pinsir; sul-esquerda + Xatu, Wobbuffet; sul-direita + Pikachu, Girafarig.
- Mt. Pyre: dentro Shuppet, Duskull (`method: cave`); fora Vulpix, Meditite.
- Shoal Cave: maré alta (09–15h e 21–03h) Golbat, Spheal, Sealeo; maré baixa (03–09h e 15–21h) + Snorunt.
- Route 126 (Dive, algas): Clamperl, Chinchou, Lanturn, Relicanth. Wailmer pela Super Rod nas rotas marítimas.
- Route 128 (Super Rod): Corsola, Luvdisc.
- Victory Road: Aron, Lairon, Zubat, Golbat, Hariyama, Loudred, Medicham, Mawile [OR], Sableye [AS].
- Sea Mauville: Spiritomb (`method: fixed`; sala "sendo observado", ler a estante e abrir/fechar a mochila; único meio).
- Voando com Mega Latios/Latias (bandos): Taillow, Wingull, Pelipper, Swablu, Murkrow, Drifloon, Braviary.

Após a vitória sobre Wallace (pokémon de outras regiões):
- Route 101: Lillipup, Sewaddle, Zorua. Route 102: Gothita, Lillipup, Tympole. Route 103: Chatot, Lillipup, Shellos.
- Route 104: Chatot, Pidove, Sewaddle. Petalburg Woods: Cottonee, Paras, Phantump.
- Routes 105–109: Frillish, Krabby, Skrelp [OR], Clauncher [AS]. Granite Cave: Axew, Onix, Timburr.
- Route 110: Chatot, Shellos, Trubbish. Route 111: Dwebble, Gible, Sandile. Route 112: Ponyta, Tyrogue, Throh [OR], Sawk [AS].
- Route 113: Bouffalant, Klefki, Scraggy. Route 114: Misdreavus, Skorupi, Tympole. Meteor Falls: Clefairy, Deino, Druddigon.
- Jagged Pass: Mankey, Ponyta, Tyrogue. Route 115: Clefairy, Misdreavus, Pidove. Route 116: Eevee, Joltik, Pidove.
- Route 117: Deerling, Rattata, Tympole. Route 118: Aipom, Luxio, Rattata. Route 121: Aipom, Hypno, Elgyem.
- Safari Zone: Buneary, Kakuna, Pidgeotto. Mt. Pyre: Bronzong, Elgyem, Growlithe. Route 125: Finneon, Frillish, Seel.
- Shoal Cave: Cubchoo, Delibird, Dewgong. Routes 122, 124, 126–134: Alomomola, Finneon, Frillish.

Mirage Spots (1 por dia; também por StreetPass):
- Mirage Island: oeste da Route 104 (Darmanitan, Venomoth, Xatu, Zebstrika); oeste de Dewford (Tangela, Venomoth, Xatu, Zebstrika); perto da Route 114 (Darmanitan, Larvesta); Route 124 (Persian, Venomoth, Xatu, Zebstrika); leste da Route 125 (Porygon, Purugly); sul de Pacifidlog (Audino, Binacle, Graveler, Xatu); Route 132 (Ditto, Munna); sul da Route 134 (Graveler, Maractus, Venomoth, Xatu, Zebstrika).
- Mirage Cave: oeste de Rustboro (Boldore, Graveler, Klink, Tynamo); sul da Route 107 (Unown); norte de Fallarbor (Slowpoke, Tynamo); norte de Fortree (Excadrill, Klink, Onix, Tynamo); norte da Route 122 (Ditto, Excadrill, Tynamo); norte da Route 124 (Boldore, Cofagrigus, Graveler, Klink); sul de Pacifidlog (Cofagrigus, Slowpoke, Tynamo); sudeste de Hoenn (Boldore, Graveler, Onix, Tynamo).
- Mirage Forest: oeste da Route 105 (Forretress, Graveler, Happiny); sul da Route 109 (Audino, Sunkern); norte da Route 111 (Kricketune, Larvesta); oeste da Route 114 (Petilil, Purugly, Sunkern, Tangela); norte de Lilycove (Cherrim, Purugly, Sunkern, Tangela); leste de Mossdeep (Cinccino, Glameow, Sunkern, Tangela); norte da Route 124 (Purugly, Sunkern, Tangela, Vulpix); sul da Route 132 (Audino, Petilil, Sunkern).
- Mirage Mountain: oeste da Route 104 (Donphan, Forretress, Kricketune, Sawsbuck); norte de Lilycove (Donphan, Forretress, Kricketune, Rufflet); leste de Mossdeep (Audino, Happiny, Tangela); norte da Shoal Cave (Darmanitan, Magby); sul da Route 129 (Elekid, Zebstrika); sul da Route 131 (Donphan, Forretress, Girafarig, Kricketune); nordeste de Hoenn (Donphan, Forretress, Kricketune, Vullaby); sudeste de Hoenn (Crustle, Graveler, Munna, Porygon, Xatu).

Lendários (`method: fixed`):

| Espécie | Nível | Local | Condição |
| --- | --- | --- | --- |
| Latios [OR] / Latias [AS] | 30 | Southern Island | presente da história, com a Mega Stone |
| Primal Groudon [OR] / Primal Kyogre [AS] | 45 | Cave of Origin | história; se não capturar, volta só após a Liga |
| Regirock | 40 | Desert Ruins (Route 111) | Sealed Chamber aberto; Strength no ponto certo |
| Regice | 40 | Island Cave (Route 105) | Sealed Chamber aberto; ficar parado 2 min |
| Registeel | 40 | Ancient Tomb (Route 120) | Sealed Chamber aberto; Fly no centro |
| Regigigas | 50 | Island Cave | 3 Regis na equipe, Regice com apelido e item de gelo, 10:00–18:00 |
| Heatran | 50 | Scorched Slab (Route 120) | após Primal |
| Ho-Oh [OR] / Lugia [AS] | 50 | Sea Mauville | Clear Bell [OR] / Tidal Bell [AS] (Scanner ao Capt. Stern) |
| Cresselia | 50 | Crescent Isle (sul de Ever Grande) | Mirage Spot aleatório |
| Raikou / Entei / Suicune | 50 | Trackless Forest (perto de Rustboro) | Ho-Oh ou Lugia na equipe; :00–:20 Raikou, :20–:40 Entei, :40–:00 Suicune |
| Cobalion / Terrakion / Virizion | 50 | Pathless Plain (Route 131) | 3 pokémon com EVs máximos; Cobalion dom/qua/sex, Terrakion ter/sáb, Virizion seg/qui |
| Uxie / Mesprit / Azelf | 50 | Nameless Cave (Route 126) | 3 pokémon com felicidade máxima; Uxie 20–21h, Mesprit 04–20h, Azelf 21–04h |
| Palkia [OR] / Dialga [AS] | 50 | fenda no céu sobre Dewford | Uxie, Mesprit e Azelf na equipe |
| Giratina | 50 | mesma fenda | Dialga e Palkia na equipe (um por troca) |
| Reshiram [OR] / Zekrom [AS] | 50 | Fabled Cave (Route 134) | pokémon lv 100 na equipe |
| Kyurem | 50 | Gnarled Den (perto do Mt. Chimney) | Reshiram e Zekrom na equipe; DNA Splicers no local |
| Tornadus [OR] / Thundurus [AS] | 50 | nuvem negra sobre Fortree | Castform na equipe |
| Landorus | 50 | mesma nuvem | Tornadus e Thundurus na equipe |
| Latias [OR] / Latios [AS] | 30 | Southern Island | Eon Ticket (evento/StreetPass); Soul Dew no local |
| Rayquaza | 70 | Sky Pillar | Delta Episode |
| Deoxys | 80 | espaço | Delta Episode; se derrotado, volta no Sky Pillar após nova vitória na Liga |

## Presentes, trocas in-game, fósseis e ovos (`method: gift` / `trade`)

- Inicial: Treecko/Torchic/Mudkip (Route 101, bolsa do Birch; nível não citado).
- Ovo de Wynaut (senhora da fonte termal, Lavaridge). Ovo de Togepi (mesma senhora, após Wallace).
- Castform (cientista, Weather Institute, após Tabitha/Shelly).
- Cosplay Pikachu lv 20 (Lisia, após o 1º concurso, Slateport).
- Fóssil no deserto (Route 111): Root Fossil → Lileep ou Claw Fossil → Anorith (o outro some); reviver na Devon Corp. (Rustboro).
- Iniciais de Johto: Chikorita/Cyndaquil/Totodile (Birch, Littleroot, início do Delta Episode).
- Iniciais de Unova: Snivy/Tepig/Oshawott (Birch, Littleroot, após o Delta Episode).
- Iniciais de Sinnoh: Turtwig/Chimchar/Piplup (Birch, Littleroot, após a revanche da Liga).
- Beldum lv 1 (mesa da casa do Steven, Mossdeep, após o Delta Episode).
- Camerupt lv 40 e Sharpedo lv 40 (casal ex-Magma/Aqua, Battle Resort, 2ª visita).
- Troca citada: Pokémon Trainer School oferece troca por Seedot (pergunta do quiz da Trick House; o detonado não diz o que se recebe).

## Mega Stones (local)

| Mega Stone | Local / condição |
| --- | --- |
| Alakazite | feira de Slateport |
| Manectite | Cycling Road |
| Steelixite | Granite Cave (área da Mach Bike) |
| Mawilite | Route 117 |
| Aggronite | Rusturf Tunnel (ajudar a Wanda com Rock Smash) |
| Pidgeotite | Mr. Stone, Devon Corp. (mostrar a Intriguing Stone) |
| Latiosite [OR] / Latiasite [AS] | com o Latios/Latias da Southern Island |
| Mega Stone do inicial | Steven, Route 120 |
| Absolite | Safari Zone (Acro Bike) |
| Altarianite | Lilycove (mostrar um Altaria) |
| Medichamite | Mt. Pyre (topo, dentro) |
| Banettite | Mt. Pyre (lado de fora) |
| Gyaradosite | Route 123 (Poochyena numa casa) |
| Glalitite | Shoal Cave (maré baixa) |
| Slowbronite | Shoal Cave (4 Shoal Shell + 4 Shoal Salt, com o Shell Bell) |
| Heracronite | Route 127 |
| Pinsirite | Route 124 (Dive) |
| Sablenite | Sootopolis City |
| Mewtwonite X | Littleroot Town (após Wallace) |
| Scizorite | Petalburg Woods (área do Cut, Moss Rock) |
| Gardevoirite | Wanda, Verdanturf (após Wallace) |
| Charizardite X | Fiery Path |
| Tyranitarite | Jagged Pass |
| Houndoominite | Fallarbor Town |
| Mega Stones dos outros iniciais de Hoenn | careca da Route 114, $1500 |
| Venusaurite | Route 119 (grama alta) |
| Abomasite | Route 123 (perto do Berry Master) |
| Lopunnite | Mauville Hills (homem do aluguel) |
| Ampharosite | New Mauville |
| Kangaskhanite | Pacifidlog (perto do Centro Pokémon) |
| Aerodactylite | Meteor Falls (área do Waterfall) |
| Charizardite Y | Scorched Slab (após Primal) |
| Beedrillite | Sea Mauville |
| Mewtwonite Y | saída da Victory Road |
| Cameruptite / Sharpedonite | Maxie/Archie (Delta Episode) e o líder oposto no Battle Resort |
| Latiasite [OR] / Latiosite [AS] | mãe, após o Delta Episode |
| Galladite | Cozmo, Fallarbor (após o Delta Episode) |
| Salamencite | anciã do Meteor Falls (após o Delta Episode) |
| Blastoisinite | S.S. Tidal |
| Gengarite | casa no Battle Resort |
| Audinite | Looker, Battle Resort |
| Metagrossite | Steven, após a revanche da Liga |
| Lucarionite | vencer Lisia (Master Rank em todas as categorias dos concursos) |
| Garchompite | Aarune, Fortree (Platinum Rank da Super Secret Base) |

## TMs, HMs e itens-chave (local)

- HMs: HM01 Cut (casa em Rustboro); HM02 Fly (rival, Route 119); HM03 Surf (pai do Wally, Petalburg, após Norman); HM04 Strength (rival, Route 112); HM05 Waterfall (Wallace, Sootopolis Gym); HM06 Rock Smash (tio do Wally, Mauville); HM07 Dive (Steven, Mossdeep, após Tate & Liza).
- TMs: TM70 Flash (entrada da Granite Cave); TM51 Steel Wing (Steven, Granite Cave); TM46 (grunt do Oceanic Museum; nome não citado); TM94 Secret Power (Aarune, saída do Fiery Path); TM96 (Aarune, Fiery Path com Strength; nome não citado); TM28 Dig (Fossil Maniac, Fallarbor); TM23 Smack Down (Cozmo, Fallarbor); TM24 Thunderbolt (Wattson, após New Mauville); TM13 Ice Beam (Sea Mauville). TMs de líderes não citados (só que o Wallace dá a HM05 no lugar de TM). Safari Zone com as duas bikes: 2 TMs (nomes não citados).
- Itens-chave: Exp. Share (cientista da Devon, Petalburg Woods); Wailmer Pail (Pretty Petal Flower Shop); Old Rod (Dewford); Good Rod (pescador, Route 118); Super Rod (casa em Mossdeep); Dowsing Machine (rival, Route 110); Mach Bike / Acro Bike (Rydel's Cycles, Mauville; as duas após mostrar a Hex Maniac da Route 111, o Bird Keeper da Route 119 e o Triathlete do Battle Resort); Contest Pass e Pokéblock Kit (Lisia, Slateport); Soot Sack (vidraceiro, Route 113); Go-Goggles (rival, Lavaridge); Devon Scope (Steven, Route 120); Mega Bracelet (Steven, Southern Island); Meteorite (Mt. Chimney); Magma Suit [OR] / Aqua Suit [AS] (Cave of Origin); Red Orb / Blue Orb (Mt. Pyre e Maxie/Archie após Primal); Eon Flute (Steven, após Primal); Devon Scuba Gear (Steven, Mossdeep); Scanner e Storage Key (Sea Mauville); Clear Bell [OR] / Tidal Bell [AS] (Capt. Stern, Slateport); DNA Splicers (Gnarled Den); Meteorite Shards (Zinnia, Granite Cave; Steven, Meteor Falls); S.S. Ticket (pai, após o Delta Episode); Eon Ticket (evento ou StreetPass); Vs. Recorder (Battle Institute, Mauville).
- Outros itens: Master Ball (Team Hideout, sala com 2 Electrode e um Nugget); Amulet Coin (mãe, após Norman); Soothe Bell (Fan Club, Slateport); Quick Claw (Trainer School); Macho Brace (casa da Winstrate Family); Charcoal (Herb Shop, Lavaridge); Safety Goggles (deserto); Shell Bell (Shoal Cave); Dawn Stone (Wally, Victory Road); Leftovers (S.S. Tidal); Soul Dew (Southern Island com Eon Ticket); Intriguing Stone (menina do Shroomish, Verdanturf); Honey (Bug Maniac no Centro Pokémon de Fallarbor, diário); pedras de clima Damp/Heat/Icy/Smooth Rock (recepcionista do Weather Institute, diária); Fire/Water/Thunder/Leaf Stone por Red/Blue/Yellow/Green Shard (ilha na Route 124); Gracidea (casa do Berry Master, mostrar Shaymin); Drives de Genesect (Mauville, mostrar Genesect); Oval Charm (Birch, Hoenn Dex completa); Shiny Charm (Birch, National Dex completa).
- Recompensas da Trick House: 1 Lava Cookie ×2 (Cut); 2 Rage Candy Bar ×2 (Rock Smash); 3 Lumiose Galette ×2 (escuro); 4 Casteliacone ×2 (Strength); 5 Old Gateau ×2 (quiz); 6 Shalour Sable ×2 (setas).
- Lojas: PokéMart de Rustboro vende Repeat Ball e Timer Ball (após ajudar o cientista no Rusturf Tunnel); Lilycove Dept. Store; Herb Shop (Lavaridge); Lava Cookie (senhora do Mt. Chimney, após Flannery); Moomoo Milk (Centro Pokémon de Lavaridge); Snowball (Battle Maison); Move Tutors por BP (Battle Resort).

## Mecânicas (para o guia de mecânicas, texto nosso)

- DexNav: mostra os pokémon da área; andar devagar com o analógico até o que se mexe na grama; podem ter golpes diferentes.
- Hordas: Honey atrai hordas na grama/cavernas (ex.: Magnemite na Route 110).
- Mega Evolução: Mega Bracelet após a Southern Island; Primal Reversion segurando a Red/Blue Orb.
- Voo livre (Soaring): Eon Flute chama o Mega Latios/Latias; A pousa, B acelera, Y desce, X sobe, L/R manobras. Fly também vai para rotas e cavernas.
- Mirage Spots: um por dia; itens e pokémon de outras regiões; também por StreetPass.
- Shoal Cave: maré alta 09–15h e 21–03h, baixa 03–09h e 15–21h. Feebas por horário (Route 119).
- Moss Rock (Petalburg Woods) evolui Eevee em Leafeon; Icy Rock (Shoal Cave, maré baixa) em Glaceon. New Mauville evolui Magneton (Magnezone) e Nosepass (Probopass).
- Contest Spectacular (Slateport, Verdanturf, Fallarbor, Lilycove); Pokéblocks.
- Super Secret Bases (TM94 Secret Power; ranks com Aarune).
- Trick House: 1 desafio liberado por insígnia, completa após a 5ª.
- Berries: árvores de colheita e plantio com o Wailmer Pail; vários NPCs dão berries diárias; Berry Master (Route 123).
- Day Care: Route 117 e Battle Resort (dois cruzamentos ao mesmo tempo).
- Revanches de treinadores visíveis no mapa do PokéNav Plus. Exp. Share é item-chave e reparte com a equipe toda.
- PSS, Pokémon-Amie e Super Training liberados no PlayNav (Wally, Petalburg). Super Training dá EVs (requisito do trio de Unova).
- Battle Maison: lv 50, sem itens da mochila, sem repetir espécie nem item; Single (3), Double (4), Triple (6), Rotation (4), Multi; castelã na 20ª e na 50ª (super); BP por vitória.
- Inverse Battle Shop (Mauville), batalhas rotacionais diárias (Mauville Hills), Mauville Food Court.
- Time Travel Award (Game Freak Director, Cove Lily Hotel): mostrar pokémon trazido de Ruby/Sapphire; diplomas da Hoenn Dex e National Dex.
- Formas: caixas de Rotom (laboratório do Birch), Meteorite muda Deoxys (casa do Cozmo), Narcissus Mirror Shop (Tornadus/Thundurus/Landorus), Crooner's Cafe (Meloetta e Keldeo), penteados de Furfrou (Slateport), DNA Splicers (Kyurem), Gracidea (Shaymin).

## Pós-jogo (ordem)

1. Créditos → revanche do rival (Route 103) → "To be Continued".
2. Delta Episode: Tickets do pai e Zinnia → iniciais de Johto → Courtney/Matt na casa do Wally → Mr. Stone (Devon) → Zinnia na Granite Cave → Space Center → Steven no Meteor Falls → 3 grunts na Devon → Courtney/Matt no Space Center (dupla com Steven) → Team Hideout (Zinnia leva a Key Stone; Mega Stone do líder) → Wallace no Sky Pillar → Rayquaza lv 70 → Zinnia → asteroide → Deoxys lv 80.
3. S.S. Ticket, Latiasite/Latiosite da mãe, iniciais de Unova, Galladite (Cozmo), Salamencite (anciã), Beldum (casa do Steven).
4. S.S. Tidal → Battle Resort (Battle Maison, Move Tutors por BP, IVs no Centro Pokémon, Looker/Audinite, Day Care, Camerupt/Sharpedo).
5. Duas bicicletas; Food Court pós-Liga.
6. Revanche da Liga (todos com Mega) → Metagrossite → iniciais de Sinnoh.
7. Battle Maison: castelãs; após a 1ª estátua, Wally (lv 64–66) e depois revanche diária (lv 79–81).
8. Diplomas/Oval Charm/Shiny Charm pela Pokédex.

## Dúvidas

- Battle Chatelaine Dana: o texto embaralha os dois times; a divisão 20ª = Dragalge, Whimsicott, Magcargo, Magneton, Piloswine, Girafarig e 50ª = trio de aves + trio Regi é inferida pela ordem.
- Drake (revanche): Mega Salamence "lv.55" no detonado, provavelmente erro (o resto do time é 73).
- Morgan 20ª: o texto diz "awsbuck"; lido como Sawsbuck.
- Números das Routes 101–104, 110–123, 125, 129–133 inferidos pela posição descrita; o detonado só numera 105, 107, 108, 109, 111, 114, 119, 120, 123, 124, 126, 127, 128, 130, 131, 134.
- Local do Poochyena com Fang pelo DexNav: "ao sair da cidade" após o laboratório (lido como Route 101).
- Ovo de Togepi: o texto cita a senhora do ovo de Wynaut no trecho de Fallarbor; ela fica em Lavaridge.
- Linoone/Kecleon e Gloom/Tropius: o detonado não separa Route 118 de Route 119.
- Níveis dos iniciais, dos ovos, dos fósseis, do Castform e dos selvagens não constam.
- Nomes de TM46 e TM96 não constam; TMs dos líderes de ginásio não constam.
