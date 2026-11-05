# HeartGold/SoulSilver — fatos extraídos do detonado da Pokémon Mythology

Fonte: https://www.pokemythology.net/detonados/detonado-heartgold-soulsilver/ (PDF salvo em 05/11/2026, 157 páginas).
Só **fatos de jogo** (chefes, níveis, encontros, itens) no nosso formato. Nenhuma prosa foi copiada. Golpes dos
chefes **não** constam no detonado, com uma exceção (Red, listado abaixo): nos demais, deixar `moves: []` e
`verified: false` para golpes; espécies e níveis são `verified: true` (uma fonte) até o analista conferir numa segunda.
Versões: `[HG]` = HeartGold, `[SS]` = SoulSilver.

## Ordem da história e chefes (para `bosses/hgss.json`)

Iniciais do rival: ele leva o inicial com vantagem sobre o seu (Chikorita→rival Cyndaquil; Cyndaquil→rival Totodile;
Totodile→rival Chikorita). "inicial evoluído" / "inicial final" = essa linha evolutiva.

| Order | Id | Kind | Nome | Local | Time (espécie lv) | Cap |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | rival-1 | rival | Rival (Silver) | saída de Cherrygrove para New Bark | inicial com vantagem (nível não listado) | — |
| 2 | sage-li | other | Monge ancião (topo da Sprout Tower; dá TM70) | Sprout Tower | não listado | — |
| 3 | falkner | gym | Falkner (Zephyr Badge, TM51 Roost) | Violet City | Pidgey 9, Pidgeotto 13 | 13 |
| 4 | proton-1 | evil | Proton (Rocket Executive) | Slowpoke Well | Zubat 8, Koffing 12 | 12 |
| 5 | bugsy | gym | Bugsy (Hive Badge, TM89 U-turn) | Azalea Town | Scyther 17, Metapod 15, Kakuna 15 | 17 |
| 6 | rival-2 | rival | Rival | saída oeste de Azalea | Gastly 14, Zubat 16, inicial evoluído 18 | 18 |
| 7 | whitney | gym | Whitney (Plain Badge, TM45 Attract) | Goldenrod City | Clefairy 17, Miltank 19 | 19 |
| 8 | rocket-dance-theater | evil | Rocket Grunt (Dance Theater; recompensa HM03) | Ecruteak City | não listado | — |
| 9 | rival-3 | rival | Rival | Burnt Tower | Gastly 20, Zubat 20, Magnemite 18, inicial evoluído 22 | 22 |
| 10 | morty | gym | Morty (Fog Badge, TM30 Shadow Ball) | Ecruteak City | Gastly 21, Haunter 21, Haunter 23, Gengar 25 | 25 |
| 11 | eusine-1 | other | Eusine | Cianwood City (norte) | Drowzee 25, Electrode 27, Haunter 25 | 27 |
| 12 | chuck | gym | Chuck (Storm Badge, TM01 Focus Punch) | Cianwood City | Primeape 29, Poliwrath 31 | 31 |
| 13 | jasmine | gym | Jasmine (Mineral Badge, TM23 Iron Tail) | Olivine City (após SecretPotion) | Magnemite 30, Magnemite 30, Steelix 35 | 35 |
| 14 | petrel-1 | evil | Petrel (Rocket Executive) | Team Rocket Hideout, Mahogany | Zubat 22, Koffing 22, Raticate 24 | 24 |
| 15 | ariana-1 | evil | Ariana + Rocket Grunt (dupla; Lance ajuda com Dragonite 40) | Team Rocket Hideout | Arbok 25, Gloom 25, Murkrow 27, Drowzee 18, Grimer 20 (divisão entre os dois não listada) | 27 |
| 16 | pryce | gym | Pryce (Glacier Badge, TM07 Hail) | Mahogany Town | Seel 30, Dewgong 32, Piloswine 34 | 34 |
| 17 | petrel-2 | evil | Petrel (falso diretor; dá Basement Key) | Radio Tower, Goldenrod | Koffing 30 ×5, Weezing 32 | 32 |
| 18 | rival-4 | rival | Rival | Goldenrod Underground (porão) | Golbat 32, Magnemite 30, Haunter 32, Sneasel 34, inicial final 34 (se você escolheu Cyndaquil: tudo 2 níveis abaixo; se Chikorita: inicial dele ainda não evoluído) | 34 |
| 19 | proton-2 | evil | Proton | Radio Tower | Golbat 28, Weezing 33 | 33 |
| 20 | ariana-2 | evil | Ariana | Radio Tower | Arbok 32, Vileplume 32, Murkrow 32 | 32 |
| 21 | archer | evil | Archer (chefe da nova Team Rocket) | Radio Tower (topo) | Houndour 35, Koffing 35, Houndoom 38 | 38 |
| 22 | clair | gym | Clair (Rising Badge após o teste do Dragon's Den; TM59 Dragon Pulse) | Blackthorn City | Gyarados 38, Dragonair 38, Dragonair 38, Kingdra 41 | 41 |
| 23 | kimono-girls | other | 5 Kimono Girls em sequência, sem cura (Zuki, Naoko, Miki, Sayo, Kuni) | Ecruteak Dance Theater | Umbreon 38, Espeon 38, Flareon 38, Jolteon 38, Vaporeon 38 | 38 |
| 24 | guardian | other | Ho-Oh [HG] / Lugia [SS] (obrigatório antes da Liga) | Bell Tower [HG] / Whirl Islands [SS] | Ho-Oh 45 [HG] / Lugia 45 [SS] | — (não conta para cap) |
| 25 | kiyo | other | Líder do Fighting Dojo (Kiyo; dá Tyrogue) | Mt. Mortar (requer Waterfall) | Hitmonlee 34, Hitmonchan 34 | 34 |
| 26 | rival-5 | rival | Rival | Victory Road (saída) | Sneasel 36, Golbat 38, Magneton 37, Haunter 37, Kadabra 37, inicial final 40 | 40 |
| 27 | will | elite | Will | Indigo Plateau | Xatu 40, Jynx 41, Slowbro 41, Exeggutor 41, Xatu 42 | 42 |
| 28 | koga | elite | Koga | Indigo Plateau | Ariados 40, Forretress 43, Muk 42, Venomoth 41, Crobat 44 | 44 |
| 29 | bruno | elite | Bruno | Indigo Plateau | Hitmontop 42, Hitmonlee 42, Hitmonchan 42, Onix 43, Machamp 46 | 46 |
| 30 | karen | elite | Karen | Indigo Plateau | Umbreon 42, Vileplume 42, Murkrow 44, Gengar 45, Houndoom 47 | 47 |
| 31 | lance | champion | Lance | Indigo Plateau | Gyarados 46, Dragonite 49, Dragonite 49, Aerodactyl 48, Charizard 48, Dragonite 50 | 50 |
| 32 | lt-surge | gym | Lt. Surge (Thunder Badge, TM34 Shock Wave) | Vermilion City | Raichu 51, Electrode 47, Electrode 47, Magneton 47, Electabuzz 53 | 53 |
| 33 | sabrina | gym | Sabrina (Marsh Badge, TM48 Skill Swap) | Saffron City | Espeon 53, Mr. Mime 53, Alakazam 55 | 55 |
| 34 | erika | gym | Erika (Rainbow Badge, TM19 Giga Drain) | Celadon City | Jumpluff 51, Tangela 52, Victreebel 56, Bellossom 56 | 56 |
| 35 | janine | gym | Janine (Soul Badge, TM84 Poison Jab) | Fuchsia City | Crobat 47, Weezing 44, Ariados 47, Ariados 47, Venomoth 50 | 50 |
| 36 | rocket-nugget-bridge | evil | Rocket Grunt (esconde a Machine Part) | Nugget Bridge (Route 24) | Golbat (nível não listado) | — |
| 37 | misty | gym | Misty (Cascade Badge, TM03 Water Pulse) | Cerulean City | Golduck 49, Quagsire 49, Lapras 52, Starmie 54 | 54 |
| 38 | brock | gym | Brock (Boulder Badge, TM80 Rock Slide) | Pewter City | Graveler 51, Rhyhorn 51, Omastar 53, Onix 54, Kabutops 52 | 54 |
| 39 | rival-6 | rival | Rival | Mt. Moon (entrada) | Sneasel 46, Golbat 47, Magneton 46, Gengar 48, Alakazam 48, inicial final 50 | 50 |
| 40 | clair-lance-double | other | Clair + Lance (dupla; rival como parceiro: inicial final 60, Crobat 58, Gengar 56) | Dragon's Den | Clair: Dragonair 52, Dragonite 60, Kingdra 56; Lance: Gyarados 68, Charizard 68, Dragonite 75 | 75 |
| 41 | blaine | gym | Blaine (Volcano Badge, TM50 Overheat) | Seafoam Islands (ginásio) | Magcargo 54, Magmar 54, Rapidash 59 | 59 |
| 42 | blue | gym | Blue (Earth Badge, TM92 Trick Room; só com 15 insígnias) | Viridian City | Exeggutor 55, Machamp 56, Arcanine 58, Rhydon 58, Gyarados 52, Pidgeot 60 | 60 |
| 43 | red | champion | Red (pico do Mt. Silver; neva na batalha) | Mt. Silver | Pikachu 88, Venusaur 84, Charizard 84, Blastoise 84, Lapras 80, Snorlax 82 | 88 |
| 44 | rival-rematch | rival | Rival (seg/qua, escadas da Liga) | Indigo Plateau | Crobat 58, Sneasel 55, Magneton 55, Gengar 58, Alakazam 56, inicial final 60 | 60 |
| 45 | will-2 | elite | Will (revanche, após HM08) | Indigo Plateau | Bronzong 58, Jynx 60, Grumpig 59, Gardevoir 61, Slowbro 60, Xatu 62 | 62 |
| 46 | koga-2 | elite | Koga (revanche) | Indigo Plateau | Skuntank 61, Toxicroak 60, Muk 62, Venomoth 63, Crobat 64, Swalot 62 | 64 |
| 47 | bruno-2 | elite | Bruno (revanche) | Indigo Plateau | Hitmontop 62, Hitmonlee 61, Hitmonchan 61, Hariyama 62, Machamp 64, Lucario 64 | 64 |
| 48 | karen-2 | elite | Karen (revanche) | Indigo Plateau | Weavile 62, Spiritomb 62, Absol 62, Honchkrow 64, Houndoom 63, Umbreon 64 | 64 |
| 49 | lance-2 | champion | Lance (revanche) | Indigo Plateau | Salamence 72, Garchomp 72, Altaria 73, Gyarados 68, Charizard 68, Dragonite 75 | 75 |
| 50 | giovanni | evil | Giovanni (evento do Celebi, passado) | Tohjo Falls (viagem no tempo) | Nidoking 42, Honchkrow 43, Nidoqueen 46, Kangaskhan 40 | 46 |

Golpes do Red (únicos listados no detonado, `verified: true` com uma fonte): Pikachu (Light Ball) Volt Tackle, Quick
Attack, Iron Tail, Thunderbolt; Venusaur Sludge Bomb, Giga Drain, Sleep Powder, Frenzy Plant; Charizard Flare Blitz,
Air Slash, Blast Burn, Dragon Pulse; Blastoise Hydro Cannon, Focus Blast, Blizzard, Flash Cannon; Lapras Body Slam,
Brine, Psychic, Blizzard; Snorlax Blizzard, Giga Impact, Crunch, Shadow Ball. Revanche do Red: mesmo time e níveis, volta
ao Mt. Silver a cada vitória sobre a Elite Four. Outros golpes citados de passagem: Ariados de Koga tem Giga Drain;
Hitmonchan de Bruno tem Thunder Punch, Fire Punch, Ice Punch, Bullet Punch; Aerodactyl de Lance tem Thunder Fang; Xatu
de Will tem U-turn.

Kanto: ginásios em qualquer ordem; Viridian só abre com as outras 7 insígnias. A ordem da tabela é a do detonado.

Revanches dos líderes (Fighting Dojo de Saffron, após ligar no dia/período certo):
Falkner (Staraptor 50, Noctowl 52, Swellow 52, Honchkrow 54, Pelipper 48, Pidgeot 56);
Bugsy (Scizor 56, Shedinja 48, Yanmega 52, Pinsir 55, Heracross 54, Vespiquen 52);
Whitney (Girafarig 52, Lickilicky 50, Bibarel 54, Delcatty 54, Clefable 52, Miltank 58);
Morty (Drifblim 52, Dusknoir 52, Sableye 52, Mismagius 54, Gengar 57, Gengar 57);
Chuck (Medicham 54, Hitmonchan 52, Hitmonlee 55, Breloom 54, Primeape 56, Poliwrath 60);
Jasmine (Metagross 52, Magnezone 56, Skarmory 52, Bronzong 50, Empoleon 52, Steelix 62);
Pryce (Abomasnow 56, Dewgong 58, Glalie 52, Froslass 52, Walrein 54, Mamoswine 60);
Clair (Gyarados 56, Dragonair 52, Aerodactyl 52, Kingdra 56, Charizard 52, Dragonite 60);
Brock (Golem 55, Relicanth 54, Omastar 56, Onix 61, Kabutops 55, Rampardos 57);
Misty (Starmie 60, Quagsire 56, Lapras 56, Lanturn 54, Floatzel 54, Milotic 60);
Lt. Surge (Raichu 60, Manectric 52, Magnezone 52, Electrode 52, Pachirisu 58, Electivire 56);
Erika (Shiftry 54, Jumpluff 53, Victreebel 56, Bellossom 56, Tangrowth 60, Roserade 55);
Janine (Crobat 52, Weezing 56, Toxicroak 52, Ariados 58, Venomoth 59, Drapion 55);
Sabrina (Alakazam 60, Espeon 58, Mr. Mime 56, Jynx 54, Wobbuffet 53, Gallade 53);
Blaine (Torkoal 54, Camerupt 57, Rapidash 60, Magcargo 58, Houndoom 54, Magmortar 62);
Blue (Exeggutor 67, Machamp 69, Arcanine 68, Rhyperior 70, Tyranitar 70, Pidgeot 72).

Como conseguir o telefone de cada líder (local; quando; requisito):
Falkner (Celadon Dept. Store 4F; segunda o dia todo; ter vencido Janine); Bugsy (Viridian Forest; quinta; Elite Four);
Whitney (Goldenrod Dept. Store 6F; todo dia 12:00–16:00; Elite Four); Morty (Bellchime Trail; seg e ter; Elite Four);
Chuck (esposa, Cianwood; todo dia; Elite Four); Jasmine (Olivine Café; todo dia 13:00–14:00; Elite Four);
Pryce (Lake of Rage; todo dia 6:00–10:00; Elite Four); Clair (Dragon's Den; todo dia 6:00–10:00; dupla Clair+Lance);
Brock (Diglett's Cave, entrada de Pewter; todo dia 12:00–15:00; 16 insígnias); Misty (Cerulean Cape, Route 25; todo dia
16:00–18:00; 16 insígnias + Suicune); Lt. Surge (fora do Power Plant, mostrar Pikachu da Viridian Forest; todo dia
9:00–12:00; 16 insígnias + Zapdos); Erika (Celadon City, com Jasmine; sáb e dom 14:00–17:00; 16 insígnias);
Janine (Pokémon League Reception Gate; todo dia 16:00–18:00; ter vencido Janine); Sabrina (Olivine Harbor, ao embarcar no
S.S. Aqua; sexta; ter vencido Sabrina); Blaine (Cinnabar Island; terça; 16 insígnias); Blue (Daisy, Pallet; todo dia;
Pokémon com felicidade máxima massageado por Daisy). Períodos: manhã 4:00–9:59, "tarde" 10:00–19:59, noite 20:00–3:59.
A tabela dia×período de cada líder é imagem no PDF (ver Dúvidas); exemplo dado: Misty aceita quarta de manhã.

Frontier Brains (Battle Frontier, só no modo single; níveis ajustados a 50 pela regra):
Tower Tycoon Palmer (21ª: Dragonite, Rhyperior, Milotic; 49ª: Cresselia, Heatran, Regigigas);
Castle Valet Darach (21ª: Staraptor, Houndoom, Empoleon; 49ª: Empoleon, Gallade, Entei);
Arcade Star Dahlia (21ª: Dusknoir, Medicham, Ludicolo; 49ª: Blaziken, Togekiss, Zapdos);
Factory Head Thorton (21ª e 49ª; time aleatório); Hall Matron Argenta (50ª e 170ª; time aleatório).

Treinadores de telefone (revanche por dia/período; Team 1 início, Team 2 após a Radio Tower, Team 3 após National Dex,
Team 4 após 16 insígnias; níveis "L"):
- Seg manhã: School Kid Jack (National Park): Oddish 12, Voltorb 14 / Gloom 29, Voltorb 17 / Vileplume 38, Electrode 31 / Electrode 43, Vileplume 52, Arcanine 52. PokéManiac Brent (Route 43): Nidorina 20, Nidorino 24 / Nidorina 32, Nidorino 34, Gyarados 32 / Gyarados 41, Lickitung 38, Nidorina 38, Nidorino 43 / Gyarados 48, Lickitung 40, Nidoqueen 58, Nidoking 58. Biker Aiden (Route 17): Tentacruel 45 / Hypno 50, Tentacruel 49 / Hypno 55, Tentacruel 55 / Tentacruel 55, Hypno 55, Tentacruel 58.
- Seg tarde: Youngster Joey (Route 30): Rattata 4 / Raticate 34 / Raticate 40 / Raticate 54. Firebreather Walt (Route 35): Magmar 11, Magmar 15 / Magmar 25, 32 / Magmar 34, 37 / Magby 26, Magmar 49, Magmortar 58.
- Seg noite: Camper Tanner (Route 13): Sudowoodo 38 / Sudowoodo 41, Chinchou 41 / Sudowoodo 48, Lanturn 47, Skiploom 44 / Sudowoodo 52, Jumpluff 52, Lanturn 52.
- Ter manhã: Bug Catcher Arnie (Route 35): Venonat 15 / Venonat 36 / Venomoth 43 / Nincada 45, Venomoth 56.
- Ter tarde: Picnicker Tiffany (Route 43): Clefairy 21 / Clefairy 34 / Clefable 41 / Clefable 61. Gentleman Alfred (Olivine Lighthouse 2F): Noctowl 22 / 36 / 42 / 58.
- Ter noite: Bug Catcher Wade (Route 31): Caterpie 2, Caterpie 2, Weedle 3, Caterpie 2 / Metapod 18, Butterfree 29, Kakuna 22, Metapod 18 / Butterfree 28, Butterfree 40, Beedrill 37, Butterfree 28 / Butterfree 36, Butterfree 52, Beedrill 43, Butterfree 36. Bird Keeper Josh (Route 14): Fearow 37 / Fearow 47, Skarmory 44 / Skarmory 48, Fearow 51 / Skarmory 55, Fearow 55.
- Qua manhã: Fisherman Ralph (Route 32): Goldeen 10 / Seaking 40 / Qwilfish 45, Seaking 42 / Qwilfish 53, Seaking 56. Bug Catcher Doug (Route 2): Butterfree 39, Ariados 37 / 40, 40 / 47, 46 / 52, 50.
- Qua tarde: School Kid Alan (Route 36): Tangela 16 / Tangela 32, Yanma 30 / Xatu 32, Tangrowth 36, Quagsire 32, Yanmega 34 / Xatu 47, Tangrowth 54, Quagsire 47, Yanmega 54. Fisherman Kyle (Route 12): Goldeen 39, Qwilfish 35, Goldeen 39 / Seaking 40, Qwilfish 38, Seaking 40 / Seaking 43, Qwilfish 41, Seaking 47, Krabby 39 / Seaking 48, Qwilfish 49, Seaking 52, Kingler 51.
- Qua noite: Sailor Huey (Olivine Lighthouse 2F): Poliwag 18, Poliwhirl 20 / Poliwhirl 34, 36 / Poliwhirl 38, Poliwrath 41 / Politoed 48, Poliwrath 52. Bird Keeper Vance (Route 44): Hoothoot 28, Pidgeotto 28 / Hoothoot 33, Pidgeotto 32 / Pidgeotto 37, Pidgeot 38, Noctowl 42 / Pidgeot 56, Pidgeot 44, Noctowl 54. School Kid Torin (Route 14): Sandslash 41, Golbat 39 / Sandslash 46, Golbat 45, Vulpix 41 / Sandslash 48, Golbat 51, Vulpix 48 / Sandslash 52, Golbat 53, Vulpix 52.
- Qui manhã: Fisherman Wilton (Route 44): Goldeen 26, Goldeen 26, Seaking 28 / Goldeen 32, 32, Seaking 36 / Goldeen 32, Seaking 36, Seaking 40 / Seaking 42, 46, 63. Ace Trainer Gaven (Route 26): Victreebel 32, Kingler 32, Flareon 32 / 34, 34, 34 / 38, 39, 38 / 52, 49, 50.
- Qui tarde: Picnicker Liz (Route 32): Nidoran♀ 8 / Weepinbell 35, Nidorina 35 / Weepinbell 39, Nidorino 38, Nidoqueen 40 / Weepinbell 47, Nidoking 58, Nidoqueen 70. Fisherman Kyler (Route 12): Magikarp 18 ×3 / ×4 / ×5 / ×6.
- Qui noite: Lass Dana (Route 38): Flaaffy 19, Psyduck 19 / Flaaffy 32, Golduck 31 / Golduck 36, Ampharos 38 / Golduck 51, Ampharos 54. Teacher Hillary (Route 15): Sunkern 41, Aipom 31 / Ambipom 43, Sunflora 43 / 49, 49 / 55, 55.
- Sex manhã: School Kid Chad (Route 38): Mr. Mime 20 / Mr. Mime 29, Magnemite 30 / Mr. Mime 39, Magneton 38 / Mr. Mime 46, Magneton 56. Bug Catcher Rob (Route 2): Beedrill 40, Butterfree 39 / 45, 42 / 52, 48 / 56, 56.
- Sex tarde: Hiker Parry (Route 45): Onix 30 / Onix 38, Graveler 35 / Steelix 41, Graveler 37 / Steelix 56, Golem 52. Young Couple Tim & Sue (Route 13): Kadabra 41, Psyduck 46 / Kadabra 45, Golduck 50 / Kadabra 48, Golduck 52 / Alakazam 54, Golduck 56. Ace Trainer Jamie (Route 26): Rapidash 36, Flaaffy 31 / Rapidash 38, Ampharos 37 / 42, 40 / 56, 59.
- Sex noite: Hiker Anthony (Route 33): Geodude 11, Machop 11 / Graveler 33, Machoke 32 / Graveler 37, Graveler 38, Machoke 40 / Graveler 41, Golem 49, Machamp 55. School Kid Billy (Route 15): Parasect 39, Poliwhirl 37, Ditto 35 / 42, 43, 40 / 46, 47, 46 / Mr. Mime 50, Parasect 55, Poliwrath 53, Ditto 51.
- Sáb manhã: Camper Todd (Route 34): Psyduck 14 / Geodude 17, Geodude 17, Psyduck 31 / Geodude 23, Graveler 33, Golduck 36, Slugma 32 / Graveler 36, Graveler 44, Magcargo 54, Golduck 56. Youngster Ian (Route 34): Mankey 10, Diglett 12 / Mankey 27, Dugtrio 33 / Primeape 38, Dugtrio 40 / Primeape 47, Dugtrio 56.
- Sáb tarde: Hiker Kenny (Route 13): Sandslash 27, Graveler 29, Golem 33 / Onix 16, Graveler 33, Golem 37, Sandslash 32 / Sandslash 42, Graveler 37, Golem 44, Onix 46 / Sandslash 46, Graveler 44, Golem 49, Steelix 54.
- Sáb noite: Picnicker Erin (Route 46): Ponyta 17, Oddish 14 / Gloom 32, Rapidash 41 / Bellossom 46, Sunflora 39, Rapidash 48 / Cherrim 42, Sunflora 46, Bellossom 49, Rapidash 53. Twins Kay & Tia (Route 15): Quagsire 41 ×2 / 48 ×2 / 53 ×2 / 55 ×2. Bird Keeper Jose (Route 27, requer Whirlpool): Farfetch'd 40 / 44 / 38, 40 / 50, 52, 53.
- Dom manhã: Lass Krise (National Park): Oddish 14, Cubone 17 / Oddish 21, Cubone 31 / Gloom 32, Marowak 38 / Vileplume 53, Marowak 54. Ace Trainer Reena (Route 27): Growlithe 36, Staryu 36, Nidorina 33 / 38, 38, 36 / Arcanine 47, Nidoqueen 43, Starmie 47 / Arcanine 54, Nidoqueen 53, Starmie 56.
- Dom tarde: Picnicker Gina (Route 34): Hoppip 9, Hoppip 9, Bulbasaur 12 / Hoppip 24, 24, Ivysaur 32 / Skiploom 26, 26, Ivysaur 44 / Skiploom 30, Jumpluff 36, Venusaur 55. Fisherman Tully (Route 42): Qwilfish 19 / Qwilfish 33 / Goldeen 30, Goldeen 30, Qwilfish 38 / Seaking 41, Seaking 50, Qwilfish 53. Biker Ernest (Route 17): Teddiursa 45, Marill 44 / Teddiursa 46, Azumarill 47 / 48, 49 / Ursaring 58, Azumarill 54.
- Dom noite: Biker Reese (Route 17): Weezing 45 / Arbok 42, Weezing 48 / Arbok 48, Weezing 50 / Arbok 56, Weezing 55.

Treinadores que dão pedras por telefone: Picnicker Gina (Leaf Stone), School Kid Alan (Fire Stone), Lass Dana (Thunder
Stone), Fisherman Tully (Water Stone), Bird Keeper Josh (Shiny Stone), Bikers Aiden/Reese/Ernest (dois deles: Dawn
Stone e Dusk Stone).

## Áreas (para `areas/hgss.json`, kind e ordem de liberação)

| id | nome | kind | libera |
| --- | --- | --- | --- |
| new-bark-town | New Bark Town | city | início |
| route-29 | Route 29 | route | início |
| cherrygrove-city | Cherrygrove City | city | início |
| route-30 | Route 30 | route | início (Mr. Pokémon) |
| route-31 | Route 31 | route | após o Mystery Egg |
| route-46 | Route 46 (trecho norte pela casinha da Route 29) | route | início (só o trecho curto) |
| dark-cave | Dark Cave | dungeon | entrada da Route 31; completa com Flash, Rock Smash, Surf, Strength |
| violet-city | Violet City | city | após a Route 31 |
| sprout-tower | Sprout Tower | building | Violet |
| ruins-of-alph | Ruins of Alph | dungeon | via Route 32 (parcial); resto com Surf/Strength pela Union Cave |
| route-36 | Route 36 | route | oeste de Violet (trecho); inteira após Sudowoodo |
| route-32 | Route 32 | route | após Zephyr Badge |
| union-cave | Union Cave | dungeon | após Route 32; partes com Surf |
| route-33 | Route 33 | route | após Union Cave |
| azalea-town | Azalea Town | city | após Route 33 |
| slowpoke-well | Slowpoke Well | dungeon | Azalea |
| ilex-forest | Ilex Forest | dungeon | após Hive Badge; atravessar com HM01 Cut |
| route-34 | Route 34 (Day Care) | route | após Ilex Forest |
| goldenrod-city | Goldenrod City (Dept. Store, Radio Tower, Underground, Game Corner, Bike Shop, Flower Shop, Magnet Train) | city | após Route 34 |
| route-35 | Route 35 | route | após Plain Badge |
| national-park | National Park | zone | Route 35 |
| pokeathlon-dome | Pokéathlon Dome | building | Route 35 |
| route-37 | Route 37 | route | após Sudowoodo (SquirtBottle) |
| ecruteak-city | Ecruteak City (Dance Theater) | city | após Route 37 |
| burnt-tower | Burnt Tower | building | Ecruteak |
| route-38 | Route 38 | route | Ecruteak oeste |
| route-39 | Route 39 (Moomoo Farm) | route | após Route 38 |
| olivine-city | Olivine City (porto, Café) | city | após Route 39 |
| olivine-lighthouse | Glitter Lighthouse | building | Olivine |
| route-40 | Route 40 (mar; nº não citado) | sea | Surf (Fog Badge) |
| route-41 | Route 41 (mar; nº não citado) | sea | Surf |
| cianwood-city | Cianwood City (Pharmacy) | city | Surf |
| cliff-cave | Cliff Cave / Cliff Edge Gate (oeste de Cianwood) | dungeon | Cianwood |
| route-47 | Route 47 | route | após Cliff Cave |
| route-48 | Route 48 | route | após Route 47 |
| safari-zone | Safari Zone (Johto) | zone | após curar o Ampharos |
| route-42 | Route 42 | route | Ecruteak leste |
| mt-mortar | Mt. Mortar | dungeon | Route 42 (Strength); completa com Waterfall e Rock Climb |
| mahogany-town | Mahogany Town | city | após Route 42 |
| route-43 | Route 43 | route | Mahogany norte |
| lake-of-rage | Lake of Rage | zone | Route 43 |
| rocket-hideout | Team Rocket Hideout | building | após Lake of Rage |
| radio-tower | Goldenrod Radio Tower (invasão) | building | após Glacier Badge |
| goldenrod-underground | Goldenrod Underground / porão | dungeon | invasão da Radio Tower |
| route-44 | Route 44 | route | após a Radio Tower |
| ice-path | Ice Path | dungeon | após Route 44 |
| blackthorn-city | Blackthorn City | city | após Ice Path |
| dragons-den | Dragon's Den | dungeon | atrás do ginásio de Blackthorn (Surf, Whirlpool) |
| route-45 | Route 45 | route | Blackthorn sul (só desce) |
| bell-tower | Bell Tower (Tin Tower) | building | após Kimono Girls [HG] / Rainbow Wing [SS] |
| whirl-islands | Whirl Islands | dungeon | Whirlpool; Lugia após Kimono Girls [SS] / Silver Wing [HG] |
| route-27 | Route 27 | route | após o guardião da versão (leste de New Bark) |
| tohjo-falls | Tohjo Falls | dungeon | Route 27 (Waterfall) |
| route-26 | Route 26 | route | após Route 27 |
| victory-road | Victory Road | dungeon | 8 insígnias |
| indigo-plateau | Indigo Plateau | building | após Victory Road |
| ss-aqua | S.S. Aqua | building | após a Elite Four (S.S. Ticket) |
| vermilion-city | Vermilion City (Fan Club) | city | pós-Elite Four |
| route-6 | Route 6 | route | Vermilion norte |
| saffron-city | Saffron City (Silph Co., Fighting Dojo, Magnet Train, Copycat) | city | após Route 6 |
| route-7 | Route 7 | route | Saffron oeste |
| celadon-city | Celadon City (Dept. Store, Condominiums, Game Corner) | city | após Route 7 |
| cycling-road | Cycling Road (Routes 16–18) | route | Celadon oeste |
| fuchsia-city | Fuchsia City (Pal Park) | city | após Cycling Road |
| route-15 | Route 15 | route | Fuchsia leste |
| route-14 | Route 14 | route | após Route 15 |
| route-13 | Route 13 | route | após Route 14 |
| route-12 | Route 12 (casa da Super Rod) | route | após Route 13 |
| route-11 | Route 11 | route | entrada a oeste da ponte da Route 12 |
| lavender-town | Lavender Town (Kanto Radio Tower) | city | após Route 12 |
| route-8 | Route 8 | route | Lavender oeste |
| route-5 | Route 5 (Cleanse Tag) | route | Saffron norte |
| cerulean-city | Cerulean City | city | após Route 5 |
| route-9 | Route 9 | route | Cerulean leste (Cut) |
| route-10 | Route 10 (Pokémon Center) | route | após Route 9 |
| rock-tunnel | Rock Tunnel | dungeon | Lavender norte; completa com Rock Climb |
| power-plant | Power Plant | building | Route 10 (Surf) |
| route-24 | Route 24 (Nugget Bridge) | route | Cerulean norte |
| route-25 | Route 25 / Cerulean Cape | route | após Route 24 |
| underground-path | Underground Path (Routes 5–6) | dungeon | após consertar o Power Plant |
| digletts-cave | Diglett's Cave | dungeon | após Snorlax (Route 11) |
| route-2 | Route 2 | route | após Diglett's Cave (Cut) |
| pewter-city | Pewter City (museu) | city | após Route 2 |
| route-3 | Route 3 | route | Pewter leste |
| mt-moon | Mt. Moon (e Mt. Moon Square) | dungeon | após Route 3 |
| route-4 | Route 4 | route | após Mt. Moon |
| viridian-forest | Viridian Forest | dungeon | Pewter sul |
| viridian-city | Viridian City (Trainer House) | city | após Viridian Forest |
| route-1 | Route 1 | route | Viridian sul |
| pallet-town | Pallet Town | city | após Route 1 |
| route-21 | Route 21 | sea | Pallet sul (Surf) |
| cinnabar-island | Cinnabar Island | city | após Route 21 |
| route-20 | Route 20 | sea | Cinnabar leste |
| seafoam-islands | Seafoam Islands (ginásio de Blaine) | dungeon | Route 20 |
| route-19 | Route 19 | sea | após Seafoam (até Fuchsia) |
| route-22 | Route 22 | route | Viridian oeste |
| cerulean-cave | Cerulean Cave | dungeon | 16 insígnias (Surf, Rock Smash, Rock Climb, Flash) |
| route-28 | Route 28 | route | após HM08 (portão da Route 22) |
| mt-silver | Mt. Silver | dungeon | Route 28 (Surf, Rock Smash, Rock Climb, Waterfall) |
| embedded-tower | Embedded Tower | building | após vencer Red (Blue Orb [HG] / Red Orb [SS]) |
| battle-frontier | Battle Frontier | building | National Dex (oeste de Olivine) |
| sinjoh-ruins | Sinjoh Ruins | zone | evento (Arceus de evento) |

## Encontros (para `encounters/hgss.json`; method `walk` salvo indicação; `conditions`: day/night/morning)

- Route 29: Rattata, Pidgey, Sentret (dia), Hoothoot (noite).
- Route 30: [HG] Caterpie, Metapod, Spinarak (noite); [SS] Weedle, Kakuna, Ledyba (manhã).
- Route 46 (trecho norte): Geodude, Spearow.
- Route 31: Bellsprout.
- Dark Cave: Zubat, Geodude; Dunsparce (`rock_smash`); Wobbuffet (único lugar do jogo).
- Route 32: Mareep, Hoppip, Wooper (noite), Ekans [SS]. Old Rod no Pokémon Center da Route 32.
- Sprout Tower: Gastly (noite).
- Union Cave: Onix, Sandshrew [HG]. Lapras lv 20 (`fixed`, toda sexta, parte com Surf).
- Ruins of Alph (área externa via Union Cave): Natu, Smeargle. Unown (26 letras, por quebra-cabeça).
- Ilex Forest: Paras, Oddish (noite); `tree` (Headbutt): Pineco, Exeggcute. Azalea sul `tree`: Aipom, Heracross.
- Route 34: Drowzee, Abra, Ditto.
- Route 35: Nidoran♀, Nidoran♂; Yanma (1%).
- National Park: Sunkern. Bug-Catching Contest (ter, qui, sáb; 20 Sport Balls, 20 min): Scyther, Pinsir, Venonat, Paras, Butterfree/Beedrill; após National Dex, qui e sáb também insetos de Hoenn e Sinnoh.
- Route 36: Growlithe [HG], Vulpix [SS], Stantler (raro). Sudowoodo lv 20 (`fixed`, SquirtBottle; volta após Elite Four).
- Burnt Tower: Koffing (andar de cima), Magmar (andar de baixo).
- Route 38: Raticate, Magnemite, Farfetch'd, Miltank, Tauros, Snubbull (raro), Meowth [SS].
- Routes 40–41 (`surf`): Tentacool, Tentacruel; Mantine [HG] perto de Cianwood.
- Olivine (`fish`, Good Rod): Krabby; Chinchou, Corsola, Shellder (sorte).
- Cliff Cave: Kingler, Machop, Machoke, Graveler, Steelix (raro), Misdreavus (noite). `surf` fora da caverna: Seel, Staryu.
- Routes 47–48 (perto do Safari): Fearow, Gloom, Girafarig, Diglett.
- Route 42: Spearow, Flaaffy, Mankey [HG]; `surf`: Goldeen, Seaking.
- Route 43 / Lake of Rage: Girafarig, Pidgeotto, Noctowl (noite). Red Gyarados lv 30 shiny (`fixed`, dá Red Scale; volta após Elite Four).
- Team Rocket Hideout: quadrados "minados" geram selvagens (espécies não listadas).
- Route 44 (`surf` até o mato): Weepinbell, Tangela, Lickitung.
- Ice Path: Swinub, Jynx, Delibird [SS].
- Routes 45–46: Gligar, Phanpy [HG]; Teddiursa, Skarmory [SS].
- Whirl Islands: Seel.
- Routes 26–27: Doduo, Sandslash [HG], Arbok [SS]; Dodrio (ver Dúvidas).
- Victory Road: Rhyhorn, Donphan [HG], Ursaring [SS].
- Route 7: Murkrow, Houndour (noite).
- Celadon (`surf`): Grimer, Muk.
- Cycling Road (Route 17): Slugma.
- Route 15: Chansey (raro).
- Route 11: Hypno.
- Route 8: Kadabra, Haunter (noite).
- Routes 9–10 (fora do Power Plant): Voltorb, Electabuzz.
- Route 3: Jigglypuff. Mt. Moon Square: Clefairy dançando (noite de segunda para terça, 20:00–4:00; fogem e deixam Moon Stone).
- Viridian Forest: Pikachu; `tree`: Seedot, Shroomish.
- Route 21: Tangela, Mr. Mime (raro).
- Rock Tunnel: Cubone; Marowak, Kangaskhan (sorte).
- Cerulean Cave: evoluções de selvagens comuns, até lv 50.
- Route 28: Sneasel (noite).
- Mt. Silver: Larvitar (5%).
- `tree` (com Rock Climb): Taillow (Cherrygrove), Cherubi (National Park), Burmy (Route 39, norte de Olivine).
- Headbutt pós-National Dex: alguns Pokémon de Hoenn e Sinnoh (não listados).
- Hoenn Sound (quarta) e Sinnoh Sound (quinta) no rádio, após National Dex: atraem Pokémon de Hoenn/Sinnoh por grupos de locais (espécies em imagem no PDF; ver Dúvidas).
- Rádio do Prof. Oak (pós-National Dex): `outbreak` diário em rota de Johto/Kanto (espécie anunciada no rádio).
- Safari Zone: 6 áreas iniciais (Plains, Swamp, Forest, Meadow, Wetlands, Peak) e 6 extras (Marshland, Wasteland, Mountain, Savannah, Desert, Rocky Beach); comuns lv 15–17, raros lv 37–48 por blocos (Plains/Forest/Rocks/Water); métodos `walk`, `surf`, `fish` (Old/Good/Super Rod). Espécies em imagem no PDF (ver Dúvidas). Citados no texto: Geodude (Peak, desafio 1), Sandshrew (Desert, desafio 2), Spheal (Peak, 35 Water), Bagon (Swamp, Rocks+Forest).
- Lendários e únicos (`method: fixed` salvo indicação):
  - Raikou lv 40 e Entei lv 40: vagam pela grama de Johto após a Burnt Tower (fogem no 1º turno; dano persiste).
  - Suicune lv 40: Cerulean Cape (Route 25), após vê-lo na Burnt Tower, norte de Cianwood, Route 42 (perto das apricorns), Vermilion e Route 14; refaz na Burnt Tower após a Elite Four.
  - Ho-Oh: lv 45 [HG] (Bell Tower, Clear Bell, Kimono Girls) / lv 70 [SS] (Rainbow Wing de Pewter).
  - Lugia: lv 45 [SS] (Whirl Islands, Tidal Bell) / lv 70 [HG] (Silver Wing de Pewter).
  - Latias [HG] / Latios [SS] lv 35: vagam por Kanto após o Lost Item.
  - Latios [HG] / Latias [SS] lv 40: Enigma Stone (Mystery Gift) → Soul Dew no museu de Pewter, com Steven.
  - Snorlax lv 50: Route 11 (Poké Flute pelo rádio, EXPN Card).
  - Articuno lv 50: Seafoam Islands, último subsolo (Strength).
  - Zapdos lv 50: fora do Power Plant (16 insígnias).
  - Moltres lv 50: Mt. Silver (entrada B).
  - Mewtwo lv 70: fundo da Cerulean Cave.
  - Kyogre [HG] / Groudon [SS] lv 50: Embedded Tower (orb de Mr. Pokémon após vencer Red).
  - Rayquaza lv 50: Embedded Tower com a Jade Orb (Oak, ao mostrar Kyogre e Groudon capturados na Embedded Tower).
  - Dialga, Palkia ou Giratina lv 1 (com a orb): Sinjoh Ruins, só com Arceus de evento.
  - Pichu de orelha espetada lv 30 (Zap Plate): Ilex Forest, com Pichu shiny de evento.
  - Celebi (evento): viagem no tempo, Ilex Forest.
  - Únicos e lendários derrotados voltam após cada vitória sobre a Elite Four.

## Presentes, trocas in-game, fósseis, ovos e iniciais (`method: gift` / `trade`)

- Inicial: Chikorita/Cyndaquil/Totodile lv 5 (Prof. Elm, New Bark).
- Ovo do Mystery Egg: Togepi lv 1 (Violet, após Zephyr Badge; 2500 passos).
- Eevee lv 5 (Bill, Goldenrod).
- Spearow correio (guarda da saída norte de Goldenrod; levar ao homem da Route 31).
- Shuckle lv 20 (Cianwood, casa ao lado da Pharmacy).
- Dratini lv 15 com ExtremeSpeed (ancião do Dragon's Den, após Rising Badge).
- Tyrogue lv 10 (Kiyo, Mt. Mortar).
- Iniciais de Kanto lv 5: Bulbasaur/Charmander/Squirtle (Oak, Pallet, após vencer Red).
- Iniciais de Hoenn lv 5: Treecko (Green)/Torchic (Red)/Mudkip (Blue) (Steven, Silph Co., após vencer Red).
- Trocas: Drowzee → Machop (Goldenrod Dept. Store 5F); Forretress → Beldum segurando Dawn Stone (Steven, Silph Co., após vê-lo no museu de Pewter).
- Fósseis: de Kanto quebrando rochas nas Ruins of Alph; de Hoenn quebrando rochas na Cliff Cave; revividos lv 20 no museu de Pewter.

## TMs, HMs e itens-chave (localização)

- HMs: HM01 Cut (Ilex Forest, chefe do carvoeiro); HM02 Fly (esposa de Chuck); HM03 Surf (Dance Theater, Ecruteak); HM04 Strength (Route 42, entrada do Mt. Mortar); HM05 Whirlpool (Lance, Rocket Hideout); HM06 Rock Smash (homem a oeste de Violet, Route 36); HM07 Waterfall (Ice Path 1F); HM08 Rock Climb (Oak, após 16 insígnias).
- TMs: TM51 Roost (Falkner); TM70 Flash (Sprout Tower); TM89 U-turn (Bugsy); TM05 Roar (Route 32, árvore de Cut); TM27 Return / TM21 Frustration (Goldenrod Dept. Store, domingos, pela felicidade); TM45 Attract (Whitney); TM44 Rest (Route 31, entrega do Spearow); TM28 Dig (National Park); TM30 Shadow Ball (Morty); TM01 Focus Punch (Chuck); TM23 Iron Tail (Jasmine); TM07 Hail (Pryce); TM36 Sludge Bomb (casa da Route 43); TM59 Dragon Pulse (Clair); TM34 Shock Wave (Lt. Surge); TM29 (Mr. Psychic, Saffron); TM48 Skill Swap (Sabrina); TM67 (atrás do Pokémon Center de Celadon); TM19 Giga Drain (Erika); TM84 Poison Jab (Janine); TM03 Water Pulse (Misty); TM57 (Power Plant, ao devolver a Machine Part); TM64 (Underground Path, troca pela RageCandyBar); TM80 Rock Slide (Brock); TM50 Overheat (Blaine); TM92 Trick Room (Blue); TM47 (casa da Route 28); TM35 (Route 28).
- Itens-chave: Running Shoes (Cherrygrove); Pokégear e mapa; Apricorn Box (Route 30); Mystery Egg (Mr. Pokémon); Johto Dex (Oak); Vs. Recorder (Violet); Lure Ball ×2 e Old Rod (Pokémon Center da Route 32); Fast Ball (Kurt); Bicycle (Bike Shop, Goldenrod); Fashion Case (Goldenrod Underground); Coin Case (Game Corner); Radio Card (quiz da Radio Tower: Sim, Sim, Não, Sim, Não); Blue Card (Buena, 2F); SquirtBottle (Flower Shop); Apriblender (Pokéathlon); Berry Pots (após Sudowoodo); Dowsing MCHN (Ecruteak); Seal Case (Moomoo Farm, após curar a Miltank com Oran Berries); Good Rod (Olivine); SecretPotion (Cianwood Pharmacy); Red Scale → Exp. Share (Mr. Pokémon); Basement Key (Petrel); Card Key (diretor no porão); Rainbow Wing [HG] / Silver Wing [SS] (diretor da Radio Tower); Master Ball (Elm, 8 insígnias); Clear Bell [HG] / Tidal Bell [SS] (Kimono Girls); S.S. Ticket (Elm, pós-Elite Four); Metal Coat (avô no S.S. Aqua); Plate de Arceus (capitão, viagens seguintes do S.S. Aqua); Up-Grade (guarda da Silph Co.); Super Rod (casa da Route 12); Machine Part (bóias do ginásio de Cerulean); Lost Item (Fan Club, Vermilion); Pass (Copycat, Saffron); EXPN Card (diretor da Kanto Radio Tower); Silver Wing [HG] / Rainbow Wing [SS] (velho em Pewter); Sacred Ash (casa da Route 2); GB Sounds (Celadon Condominiums 3F, 16 insígnias); Blue Orb [HG] / Red Orb [SS] (Mr. Pokémon, após Red); Jade Orb (Oak); Gracidea (Flower Shop, com Shaymin de evento); Enigma Stone (Mystery Gift) → Soul Dew.
- Outros itens citados: 5 Potions (assistente de Elm); 5 Poké Balls (Ethan/Lyra); Charcoal (casa do carvoeiro, Azalea); HP Up (guarda de Goldenrod); Rare Candy (presidente do Fan Club); Nugget (Route 24 e casa da Route 2); Cleanse Tag (casa da Route 5); Leaf Stone (Viridian Forest); Water Stone (Seafoam Islands); Shiny Stone (National Park, Rock Climb); pedras evolutivas (avô de Bill, Route 25, ao mostrar o Pokémon pedido); Moon Stone (Clefairy dançando).
- Lojas: Goldenrod Dept. Store (andar por categoria; máquinas de bebida no topo; sorteio de $300 diário); Celadon Dept. Store; Cianwood Pharmacy; Mahogany (loja após a Rocket Hideout); Mt. Moon Square (Fresh Water, Lemonade, Soda Pop em quantidade, 10:00–18:00); Pokéathlon (pontos: Apricorns, Moomoo Milk, Heart Scale, Rare Candy, PP Up, Nugget, pedras evolutivas; Dawn/Shiny/Dusk Stone após National Dex); Moomoo Farm (Moomoo Milk); Goldenrod Underground (Herb Shop nos fins de semana); Dusk Balls em Vermilion, Lavender, Fuchsia; tendas de BP da Battle Frontier.
- Tutores: Blackthorn (Move Deleter; Move Relearner por Heart Scale; Draco Meteor; Frenzy Plant / Blast Burn / Hydro Cannon para inicial final de qualquer geração); Headbutt (Ilex Forest).

## Mecânicas (para o guia de mecânicas, texto nosso)

- Primeiro Pokémon da equipe anda atrás do jogador fora da Poké Ball (não na Battle Frontier).
- Pokégear: telefone, mapa (mostra lendários errantes), rádio (Radio Card, EXPN Card, Poké Flute, Hoenn/Sinnoh Sound).
- Relógio real: manhã 4:00–9:59, dia 10:00–19:59, noite 20:00–3:59; eventos por dia da semana (Lapras sexta, Bug Contest ter/qui/sáb, S.S. Aqua de Vermilion dom e qua e de Olivine seg e sex).
- Apricorns: uma por árvore por dia; Kurt faz a Poké Ball especial em 1 dia.
- Berry Pots com SquirtBottle (Oran Berry: 16 h, 2–5 frutos).
- Bug-Catching Contest (prêmios: 1º Sun Stone, 2º Everstone, 3º Sitrus Berry, demais Shed Shell; outras pedras após National Dex).
- Pokéathlon: Speed, Power, Skill, Stamina, Jump; Aprijuice melhora as capacidades; pontos trocados por itens.
- Voltorb Flip no Game Corner (moedas por prêmios).
- Buena's Password (rádio às 2, 5, 8 e 11 h; pontos no Blue Card; 30 pontos = telefone da Buena).
- Mãe guarda dinheiro e compra itens (entrega pelo Delivery Man nos PokéMarts).
- Day Care na Route 34 (ovos com casal compatível).
- Seals nas Poké Balls (Seal Case).
- Safari Zone de Johto: $500, 30 Safari Balls; comida/lama; layout de áreas trocável; blocos ficam mais fortes com os dias (x2 a partir de 10/20/30/40 dias para Plains/Forest/Rocks/Water, até x7); máx. 30 objetos por área; Baoba dá 24 blocos em 4 ligações.
- Revanches por telefone com líderes (Fighting Dojo) e treinadores (Teams 1–4).
- Trainer House (Viridian): 1 batalha por dia, níveis em 50, 1 BP, sem EXP; até 10 treinadores pelo Pokéwalker.
- Battle Frontier (National Dex): Tower, Factory, Arcade, Castle, Hall; rounds de 7 (Hall: 10); lv 50; BP; Brains só no single; Castle usa Castle Points; Hall com 1 Pokémon e ranks de tipo 1–10. Proibidos: Mewtwo, Mew, Lugia, Ho-Oh, Celebi, Kyogre, Groudon, Rayquaza, Jirachi, Deoxys, Dialga, Palkia, Giratina, Phione, Manaphy, Darkrai, Shaymin, Arceus, Pichu de orelha espetada.
- Pal Park em Fuchsia (traz Pokémon de R/S/E e FR/LG).
- Magnet Train Goldenrod↔Saffron (Pass, após Power Plant).
- Ruins of Alph: 4 quebra-cabeças (Kabuto, Aerodactyl, Ho-Oh, Omanyte) liberam Unown; palavras secretas: Escape (Escape Rope), Light (Flash), Ho-Oh (Ho-Oh na frente da equipe), Water (Water Stone); salas com 4 itens aleatórios; Unown Report.
- Rotom muda de forma no elevador da Silph Co.; Shaymin Sky Forme com Gracidea (não à noite).
- Diamond Dust no pico do Mt. Silver em 10 de outubro; neve (Hail) durante a batalha com Red.
- GB Sounds troca a música para a de G/S/C.
- Daisy (Pallet, 15:00–16:00): massagem diária (felicidade e beleza).
- Meteoritos da Route 3 mudam a forma de Deoxys.
- Tyrogue evolui no lv 20 (Hitmonlee se Atk > Def; Hitmonchan se Atk < Def; Hitmontop se iguais).

## Pós-jogo (ordem)

1. Vencer Lance → créditos → S.S. Ticket (Elm) → National Dex (Oak, porto de Olivine; libera Battle Frontier, Hoenn/Sinnoh Sound, Bug Contest ampliado).
2. S.S. Aqua → Vermilion → 8 ginásios de Kanto (Viridian por último), Power Plant (Machine Part), Lost Item/Pass, EXPN Card, Snorlax, rival no Mt. Moon, dupla Clair+Lance.
3. Blue em Cinnabar (15 insígnias) → Earth Badge → HM08 Rock Climb (Oak) → GB Sounds → Zapdos, Mewtwo, Route 28, Mt. Silver e Moltres.
4. Elite Four reforçada (após HM08) → Red no Mt. Silver.
5. Após Red: iniciais de Kanto e de Hoenn, Beldum, Embedded Tower (Kyogre/Groudon, Rayquaza).
6. Revanches (líderes, rival, Elite Four, Red, treinadores por telefone); eventos de Mystery Gift (Celebi/Giovanni, Pichu, Arceus/Sinjoh, Enigma Stone).

## Dúvidas

- Nível e espécie do time do rival na 1ª batalha não estão no texto (só "inicial com vantagem").
- Rocket Hideout dupla: o texto lista Arbok 25, Gloom 25, Murkrow 27, Drowzee 18, Grimer 20 juntos, sem dizer o que é de Ariana e o que é do Grunt.
- Monge ancião da Sprout Tower, Rocket do Dance Theater e Rocket da Nugget Bridge: sem time/nível no texto.
- Dodrio: o texto diz [SS] na Route 27 e depois "apenas em HeartGold" no trecho seguinte (Route 26); contraditório.
- Tabela dia × período das revanches dos líderes e as tabelas de espécies do Safari Zone e do Hoenn/Sinnoh Sound vieram como imagem no PDF; só os níveis/locais sobraram no texto. Conferir no PDF (págs. 80, 110–125).
- Números das rotas marítimas 40/41 e de algumas rotas de Kanto não aparecem no texto; usei os nomes do jogo.
- Frontier Brains: o detonado não dá níveis (regra geral da Frontier: lv 50).
- Kiyo: o texto só diz "líder do Fighting Dojo (que veio de Kanto)"; nome "Kiyo" vem de "Dojo de Kyo" citado em Saffron.
- TMs 29, 35, 47, 57, 64, 67: o detonado não diz o golpe.
- Ho-Oh/Lugia do guardião: o detonado não diz se a Liga só abre após capturar ou basta enfrentar (diz "ficar cara a cara").
