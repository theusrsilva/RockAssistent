# Scarlet/Violet — fatos extraídos do detonado da Pokémon Mythology

Fonte: https://www.pokemythology.net/detonados/detonado-scarlet-violet/ (PDF salvo em 05/11/2026, 178 páginas; o
rodapé numera até 179). Cobre o jogo base (Victory Road, Path of Legends, Starfall Street, The Way Home, pós-jogo) e a
DLC The Hidden Treasure of Area Zero (The Teal Mask, The Indigo Disk e o epílogo Mochi Mayhem).
Só **fatos de jogo** (chefes, níveis, encontros, itens) no nosso formato. Nenhuma prosa foi copiada. Golpes dos
chefes **só** constam para a Elite 4 da BB League, o Kieran campeão, o Cyrano e o Kieran como Special Coach (seção
"Golpes listados"); para os demais, `moves: []` e `verified: false` para golpes. Espécies e níveis são
`verified: true` (uma fonte) até o analista conferir numa segunda.

Rotas (`title`): **VR** = Victory Road (ginásios, Elite 4, campeã); **PL** = Path of Legends (Titans); **SS** = Starfall
Street (Team Star); **WH** = The Way Home (Area Zero); **PG** = pós-jogo; **TM** = The Teal Mask; **ID** = The Indigo
Disk; **MM** = epílogo Mochi Mayhem. O mapa de ordem e níveis recomendados do detonado é uma imagem (não legível no
texto); a coluna Cap é o nível recomendado aproximado (maior nível do time).

Inicial do rival: a Nemona sempre leva o inicial **fraco ao seu** (você Sprigatito → ela Quaxly/Quaxwell/Quaquaval;
você Fuecoco → ela Sprigatito/Floragato/Meowscarada; você Quaxly → ela Fuecoco/Crocalor/Skeledirge). Abaixo,
"inicial da Nemona" segue essa regra.

## Ordem da história e chefes (para `bosses/sv.json`)

Ordem sugerida pelo detonado (intercala as três rotas por nível).

| Order | Id | Kind | Rota | Nome | Local | Time (espécie lv) | Cap |
| --- | --- | --- | --- | --- | --- | --- | --- |
| 1 | nemona-1 | rival | VR | Nemona | Private Beach (Cabo Poco) | inicial da Nemona 5 | 5 |
| 2 | arven-1 | rival | PL | Arven | Poco Path Lighthouse | Skwovet 5 | 5 |
| 3 | nemona-2 | rival | VR | Nemona | Portão de Mesagoza (South) | inicial da Nemona 8, Pawmi 9 (Tera Electric) | 9 |
| 4 | katy | gym | VR | Katy (Bug) | Cortondo | Nymble 14, Tarountula 14, Teddiursa 15 (Tera Bug) | 15 |
| 5 | titan-klawf | other | PL | Klawf, the Stony Cliff Titan (Rock) | South Province (Area Three) | Titan Klawf 16 (2 lutas: solo e raid com Arven/Shellder) | 16 |
| 6 | brassius | gym | VR | Brassius (Grass) | Artazon | Petilil 16, Smoliv 16, Sudowoodo 17 (Tera Grass) | 17 |
| 7 | titan-bombirdier | other | PL | Bombirdier, the Open Sky Titan (Flying) | West Province (Area One) – Central | Titan Bombirdier 20 (2 lutas; raid com Arven/Nacli) | 20 |
| 8 | giacomo | evil | SS | Giacomo (Segin Squad, Dark Crew) | Base perto de West Province (Area One) – North | Pawniard 21, Segin Starmobile 20 (Dark) | 21 |
| 9 | nemona-3 | rival | VR | Nemona (ao entrar no 3º ginásio) | Levincia | Rockruff 21, Pawmi 21, Floragato (Tera Grass)/Crocalor (Tera Fire)/Quaxwell (Tera Water) 22 | 22 |
| 10 | iono | gym | VR | Iono (Electric) | Levincia | Wattrel 23, Bellibolt 23, Luxio 23, Mismagius 24 (Tera Electric) | 24 |
| 11 | mela | evil | SS | Mela (Schedar Squad, Fire Crew) | East Province (Area One) | Torkoal 27, Schedar Starmobile 26 (Fire) | 27 |
| 12 | titan-orthworm | other | PL | Orthworm, the Lurking Steel Titan (Steel) | East Province (Area Three) | Titan Orthworm 29 (2 lutas; raid com Arven/Toedscool) | 29 |
| 13 | kofu | gym | VR | Kofu (Water) | Cascarrafa | Veluza 29, Wugtrio 29, Crabominable 30 (Tera Water) | 30 |
| 14 | atticus | evil | SS | Atticus (Navi Squad, Poison Crew) | Tagtree Thicket (East Province Area Three, perto de Zapapico) | Skuntank 32, Muk 32, Revavroom 33, Navi Starmobile 32 (Poison) | 33 |
| 15 | larry | gym | VR | Larry (Normal) | Medali (Treasure Eatery) | Komala 35, Dudunsparce 35, Staraptor 36 (Tera Normal) | 36 |
| 16 | nemona-4 | rival | VR | Nemona (5º ginásio) | Medali | Lycanroc 36, Goomy 36, Pawmo 36, Meowscarada (Tera Grass)/Skeledirge (Tera Fire)/Quaquaval (Tera Water) 37 | 37 |
| 17 | ryme | gym | VR | Ryme (Ghost; batalha em dupla) | Montenevera | Banette 41, Mimikyu 41, Houndstone 41, Toxtricity 42 (Tera Ghost) | 42 |
| 18 | titan-great-tusk | other | PL | Great Tusk [Scarlet] / Iron Treads [Violet], the Quaking Earth Titan (Ground) | Asado Desert | Titan Great Tusk/Iron Treads 45 (2 lutas; raid com Arven/Scovillain) | 45 |
| 19 | nemona-5 | rival | VR | Nemona (7º ginásio) | Alfornada | Lycanroc 42, Sliggoo 42, Pawmot 42, Meowscarada/Skeledirge/Quaquaval 43 | 43 |
| 20 | tulip | gym | VR | Tulip (Psychic) | Alfornada | Farigiraf 41, Gardevoir 41, Espathra 41, Florges 42 (Tera Psychic) | 42 |
| 21 | grusha | gym | VR | Grusha (Ice) | Glaseado Mountain | Frosmoth 47, Beartic 47, Cetitan 47, Altaria 48 (Tera Ice) | 48 |
| 22 | ortega | evil | SS | Ortega (Ruchbah Squad, Fairy Crew) | North Province (Area Three) | Azumarill 50, Wigglytuff 50, Dachsbun 51, Ruchbah Starmobile 50 (Fairy) | 51 |
| 23 | titan-dondozo-tatsugiri | other | PL | Dondozo/Tatsugiri, the False Dragon Titan (Dragon) | Casseroya Lake | Titan Dondozo 56 (2 lutas; raid com Arven/Greedent), Titan Tatsugiri 57 | 57 |
| 24 | eri | evil | SS | Eri (Caph Squad, Fighting Crew) | North Province (Area Two) | Toxicroak 55, Passimian 55, Lucario 55, Annihilape 56, Caph Starmobile 56 (Fighting) | 56 |
| 25 | arven-2 | rival | PL | Arven (conclusão do Path of Legends) | Poco Path Lighthouse | Greedent 58, Cloyster 59, Scovillain 60, Toedscruel 61, Garganacl 62, Mabosstiff 63 (Tera Dark) | 63 |
| 26 | clavell-1 | other | SS | Clavell (revela ser o Clive) | Naranja/Uva Academy (entrada) | Oranguru 60, Abomasnow 60, Polteageist 60 + por inicial: Sprigatito → Amoonguss 60, Gyarados 60, Skeledirge 61 (Tera Fire); Fuecoco → Houndoom 60, Amoonguss 60, Quaquaval 61 (Tera Water); Quaxly → Gyarados 60, Houndoom 60, Meowscarada 61 (Tera Grass) | 61 |
| 27 | penny-1 | evil | SS | Penny (Cassiopeia, Big Boss da Team Star) | Schoolyard (noite) | Umbreon 62, Vaporeon 62, Jolteon 62, Flareon 62, Leafeon 62, Sylveon 63 (Tera Fairy) | 63 |
| 28 | rika | elite | VR | Rika (Ground) | Pokémon League | Whiscash 57, Camerupt 57, Donphan 57, Dugtrio 57, Clodsire 58 (Tera Ground) | 58 |
| 29 | poppy | elite | VR | Poppy (Steel) | Pokémon League | Copperajah 58, Magnezone 58, Bronzong 58, Corviknight 58, Tinkaton 59 (Tera Steel) | 59 |
| 30 | larry-elite | elite | VR | Larry (Flying) | Pokémon League | Tropius 59, Oricorio 59, Altaria 59, Staraptor 59, Flamigo 60 (Tera Flying) | 60 |
| 31 | hassel | elite | VR | Hassel (Dragon) | Pokémon League | Noivern 60, Haxorus 60, Dragalge 60, Flapple 60, Baxcalibur 61 (Tera Dragon) | 61 |
| 32 | geeta | champion | VR | Geeta (Top Champion) | Pokémon League | Espathra 61, Gogoat 61, Veluza 61, Avalugg 61, Kingambit 61, Glimmora 62 (Tera Rock) | 62 |
| 33 | nemona-6 | rival | VR | Nemona (conclusão da Victory Road) | Mesagoza (pátio central) | Lycanroc 65, Goodra 65, Dudunsparce 65, Orthworm 65, Pawmot 65, Meowscarada (Tera Grass)/Skeledirge (Tera Fire)/Quaquaval (Tera Water) 66 | 66 |
| 34 | az-glimmora | other | WH | Glimmora (raid com Nemona) | Area Zero, Estação de Pesquisa 1 | Glimmora 62 | 62 |
| 35 | az-paradox-1 | other | WH | Raid com Penny | Area Zero, Estação de Pesquisa 2 | Scream Tail 62 [Scarlet] / Iron Bundle 62 [Violet] | 62 |
| 36 | az-paradox-2 | other | WH | Raid com Arven | Area Zero, Estação de Pesquisa 3 | Great Tusk 63 [Scarlet] / Iron Treads 63 [Violet] | 63 |
| 37 | az-paradox-3 | other | WH | Raid com Nemona | Frente do Zero Lab | Great Tusk 64 [Scarlet] / Iron Treads 64 [Violet] | 64 |
| 38 | az-paradox-4 | other | WH | Raid com Penny | Frente do Zero Lab | Brute Bonnet 64 [Scarlet] / Iron Hands 64 [Violet] | 64 |
| 39 | az-paradox-5 | other | WH | Raid com Arven | Frente do Zero Lab | Flutter Mane 64 [Scarlet] / Iron Jugulis 64 [Violet] | 64 |
| 40 | ai-professor | other | WH | AI Sada [Scarlet] / AI Turo [Violet] | Zero Lab, máquina do tempo | Sada: Slither Wing 66, Scream Tail 66, Brute Bonnet 66, Flutter Mane 66, Sandy Shocks 66, Roaring Moon 67; Turo: Iron Moth 66, Iron Bundle 66, Iron Hands 66, Iron Jugulis 66, Iron Thorns 66, Iron Valiant 67 | 67 |
| 41 | paradise-protection-protocol | other | WH | Paradise Protection Protocol (final; raid só com o seu lendário lv 68) | Zero Lab | Koraidon 72 [Scarlet] / Miraidon 72 [Violet] | — |
| 42 | carmine-1 | rival | TM | Carmine | Mossui Town | Poochyena 60, Vulpix 60, Poltchageist 60 | 60 |
| 43 | kieran-1 | rival | TM | Kieran | Mossui Town | Sentret 59, Yanma 60 | 60 |
| 44 | kieran-2 | rival | TM | Kieran | Dreaded Den (entrada) | Furret 62, Yanma 62, Poliwhirl 66 | 66 |
| 45 | carmine-2 | rival | TM | Carmine | Casa de Kieran e Carmine (Mossui Town) | Morpeko 62, Swadloon 63, Mightyena 63, Sinistcha 64 | 64 |
| 46 | kieran-3 | rival | TM | Kieran | Paradise Barrens | Yanma 64, Furret 64, Poliwrath 66, Dipplin 67 | 67 |
| 47 | milotic-raid | other | TM | Milotic (raid com Carmine) | Crystal Pool | Milotic 73 | 73 |
| 48 | kieran-4 | rival | TM | Kieran | Loyalty Plaza | Yanmega 68, Gligar 66, Cramorant 65, Dipplin 69, Poliwrath 71 | 71 |
| 49 | munkidori-1 | other | TM | Munkidori | Dreaded Den | Munkidori 70 | 70 |
| 50 | okidogi-titanic | other | TM | The Titanic Okidogi (raid com Carmine; dá a Cornerstone Mask) | Paradise Barrens | Okidogi 85 | — |
| 51 | munkidori-titanic | other | TM | The Titanic Munkidori (raid; dá a Wellspring Mask) | Wisteria Pond (Wistful Fields) | Munkidori 85 | — |
| 52 | fezandipiti-titanic | other | TM | The Titanic Fezandipiti (raid; dá a Hearthflame Mask) | Fellhorn Gorge / Oni Mountain | Fezandipiti 85 | — |
| 53 | kieran-5 | rival | TM | Kieran | Dreaded Den | Shiftry 71, Yanmega 72, Dipplin 72, Probopass 71, Poliwrath 72, Gliscor 76 | 76 |
| 54 | ogerpon-test | other | TM | Ogerpon (4 formas seguidas) | Dreaded Den | Ogerpon Hearthflame Mask 70 (Tera Fire), Wellspring Mask 70 (Tera Water), Cornerstone Mask 70 (Tera Rock), Teal Mask 70 (Tera Grass) | 70 |
| 55 | carmine-3 | rival | TM | Carmine | Loyalty Plaza | Mightyena 70, Morpeko 71, Leavanny 72, Ninetales 72, Sinistcha 74 | 74 |
| 56 | lacey-intro | other | ID | Lacey (dupla) | Entrada da Blueberry Academy | Plusle 70, Minun 70, Excadrill 72 | 72 |
| 57 | carmine-4 | rival | ID | Carmine (dupla) | Terarium, Central Plaza | Toucannon 74, Mightyena 74, Scrafty 74, Sinistcha 75 (Tera Grass) | 75 |
| 58 | crispin | elite | ID | Crispin (Fire; BB League #5) | Savanna Plaza | Heat Rotom 77, Talonflame 77, Exeggutor 78, Camerupt 78, Magmortar 78, Blaziken 79 (Tera Fire) | 79 |
| 59 | amarys | elite | ID | Amarys (Steel; #4) | Canyon Plaza | Skarmory 78, Alolan Dugtrio 78, Scizor 79, Reuniclus 79, Empoleon 79, Metagross 80 (Tera Steel) | 80 |
| 60 | lacey | elite | ID | Lacey (Fairy; #3) | Coastal Plaza | Whimsicott 78, Granbull 78, Primarina 79, Galarian Slowbro 79, Alcremie 79, Excadrill 80 (Tera Fairy) | 80 |
| 61 | drayton | elite | ID | Drayton (Dragon; #2) | Polar Plaza | Dragonite 78, Flygon 78, Haxorus 79, Sceptile 79, Kingdra 79, Archaludon 80 (Tera Dragon) | 80 |
| 62 | kieran-champion | champion | ID | Kieran (campeão da BB League; dupla) | Entrada da Blueberry Academy | Politoed 80, Dragonite 80, Grimmsnarl 81, Porygon-Z 81, Incineroar 81, Hydrapple 82 (Tera Fighting) | 82 |
| 63 | ud-glimmora | other | ID | Glimmora Tera Stellar | Area Zero Underdepths | Glimmora 78 (Tera Stellar) | 78 |
| 64 | ud-noivern | other | ID | Noivern Tera Stellar | Area Zero Underdepths | Noivern 78 (Tera Stellar) | 78 |
| 65 | ud-paradox | other | ID | Paradoxo Tera Stellar | Area Zero Underdepths | Sandy Shocks 79 [Scarlet] / Iron Thorns 79 [Violet] (Tera Stellar) | 79 |
| 66 | ud-garganacl | other | ID | Garganacl Tera Stellar | Area Zero Underdepths (árvore cristalizada) | Garganacl 80 (Tera Stellar) | 80 |
| 67 | kieran-terapagos | rival | ID | Kieran (luta de Titan) | Area Zero Underdepths | Terapagos 85 | 85 |
| 68 | terapagos-raid | other | ID | Terapagos (raid com Carmine e Kieran; captura garantida) | Area Zero Underdepths | Terapagos 85 (Tera Stellar; Normal → Psychic → Water ao perder HP) | — |
| 69 | mochi-elders | other | MM | Vovô Yukito + Vovó Hideko (dupla com Kieran) | Escadaria do Kitakami Hall | Yukito: Arbok 80, Weezing 80; Hideko: Chandelure 82, Mamoswine 82 | 82 |
| 70 | mochi-arven-penny | other | MM | Arven + Penny (dupla com Kieran) | Mossui Town | Arven: Greedent 82, Cloyster 82, Mabosstiff 83; Penny: Umbreon 84, Leafeon 84, Sylveon 85 | 85 |
| 71 | mochi-nemona | rival | MM | Nemona (sob a maldição) | Loyalty Plaza | Lycanroc 86, Kommo-o 86, Goodra 86, Pawmot 86 + por inicial: Sprigatito → Ribombee 86, Quaquaval 87 (Tera Fighting); Fuecoco → Milotic 86, Meowscarada 87 (Tera Dark); Quaxly → Dusknoir 86, Skeledirge 87 (Tera Ghost) | 87 |
| 72 | pecharunt | other | MM | Pecharunt (raid solo; captura garantida; precisa da Mythical Pecha Berry por Mystery Gift) | Loyalty Plaza | Pecharunt 88 | — |

### Revanches e lutas do pós-jogo (jogo base)

- Team Star nos STCs (1×/dia, sem Starmobile): Giacomo (Cacturne 65, Honchkrow 65, Mabosstiff 65, Krookodile 65,
  Kingambit 66); Mela (Torkoal 65, Coalossal 65, Houndoom 65, Arcanine 65, Armarouge 66); Atticus (Skuntank 65, Muk 65,
  Dragalge 65, Toxapex 65, Revavroom 66); Ortega (Klefki 65, Azumarill 65, Wigglytuff 65, Hatterene 65, Dachsbun 66);
  Eri (Primeape 65, Toxicroak 65, Passimian 65, Lucario 65, Annihilape 66).
- Inspeção dos ginásios para a Geeta (qualquer ordem): Katy (Lokix 65, Forretress 65, Spidops 65, Heracross 65,
  Ursaring 66 Tera Bug); Brassius (Lilligant 65, Breloom 65, Tsareena 65, Arboliva 65, Sudowoodo 66 Tera Grass); Iono
  (Kilowattrel 65, Bellibolt 65, Electrode 65, Luxray 65, Mismagius 66 Tera Electric); Kofu (Veluza 65, Pelipper 65,
  Wugtrio 65, Clawitzer 65, Crabominable 66 Tera Water); Larry (Oinkologne 65, Komala 65, Braviary 65, Dudunsparce 65,
  Staraptor 66 Tera Normal); Ryme, dupla (Banette 65, Mimikyu 65, Spiritomb 65, Houndstone 65, Toxtricity 66 Tera Ghost);
  Tulip (Farigiraf 65, Gardevoir 65, Espathra 65, Gallade 65, Florges 66 Tera Psychic); Grusha (Frosmoth 65, Beartic 65,
  Cetitan 65, Weavile 65, Altaria 66 Tera Ice). Prêmio final: King's Rock (Geeta).
- Academy Ace Tournament, 1ª vez (4 lutas seguidas, sem Box): Arven (Greedent 67, Cloyster 67, Scovillain 67,
  Toedscruel 67, Garganacl 67, Mabosstiff 68 Tera Dark); Jacq (Arcanine 67, Lurantis 67, Swalot 67, Mudsdale 67,
  Slowbro 67, Farigiraf 68 Tera Psychic); Dendra (Falinks 65, Paldean Tauros Blaze 65, Paldean Tauros Aqua 65,
  Medicham 65, Hawlucha 65, Hariyama 66 Tera Fighting); Geeta (Espathra 69, Gogoat 69, Veluza 69, Avalugg 69,
  Kingambit 69, Glimmora 70 Tera Rock).
- Torneios seguintes (adversários aleatórios entre os acima e): Hassel (Noivern 67, Haxorus 67, Dragalge 67, Flapple 67,
  Dragonite 67, Baxcalibur 68 Tera Dragon); Miriam (Hypno 65, Pincurchin 65, Sawsbuck 65, Glalie 65, Eelektross 65,
  Toxapex 66 Tera Poison); Raifort (Zoroark 65, Seviper 65, Grumpig 65, Lumineon 65, Scizor 65, Gengar 66 Tera Ghost);
  Saguaro (Pachirisu 65, Froslass 65, Alomomola 65, Vespiquen 65, Goodra 65, Hatterene 66 Tera Fairy); Salvatore
  (Honchkrow 65, Persian 65, Palossand 65, Glaceon 65, Gothitelle 65, Raichu 66 Tera Electric); Tyme (Lycanroc 65,
  Lycanroc 65, Drednaw 65, Stonjourner 65, Coalossal 65, Garganacl 66 Tera Rock); Penny (Umbreon 69, Vaporeon 69,
  Jolteon 69, Flareon 69, Leafeon 69, Sylveon 70 Tera Fairy); Nemona (Lycanroc 71, Goodra 71, Dudunsparce 71,
  Orthworm 71, Pawmot 71, inicial da Nemona 72); Clavell (mesmo time da Starfall Street, lv 65/66).

### Pós-jogo do The Teal Mask

- Billy & O'Nare (Glitterati; a moça do galpão em Mossui Town diz onde estão): 1º Apple Hills, luta com O'Nare
  (Persian 65); 2º Fury Falls, 15 Big Pearls; 3º South Province (Area Five), 15 Nuggets; 4º Apple Hills, O'Nare
  (Persian 65); 5º ilha sul do Casseroya Lake, 10 Pearl Strings; 6º norte da Glaseado Mountain, 10 Big Nuggets;
  7º topo da Great Crater, O'Nare (Arboliva 65, Persian 69) + 25 Big Nuggets + Glitterati Case.
- Kitakami Ogre Clan (7 em qualquer ordem, depois o 8º): Raikiri, Electric, caverna da cachoeira em Apple Hills
  (Raichu 75, Probopass 75, Luxray 76, Vikavolt 77); Masamune, Fairy, Mossfell Confluence (Grimmsnarl 75,
  Gardevoir 75, Ribombee 76, Clefable 77); Hasebe, chuva, Fellhorn Gorge (Politoed 75, Ludicolo 75, Drednaw 76,
  Basculegion 77); Kanemitsu, sol, Paradise Barrens (Ninetales 75, Shiftry 75, Chandelure 76, Lilligant 77);
  Kunitsuna, Rock/Ice, lago do Infernal Pass (Carbink 75, Golem 75, Froslass 76, Mamoswine 77); Munechika, Normal,
  nordeste do Crystal Pool (Ambipom 75, Maushold 75, Greedent 76, Snorlax 77); Kotetsu, Fighting, caverna à direita
  do Crystal Pool (Heracross 75, Gallade 75, Conkeldurr 76, Kommo-o 77); 8º Muramasa, Ghost/Dark, Kitakami Hall
  (Gengar 75, Crawdaunt 75, Trevenant 76, Dusclops 77, Kingambit 80). Prêmios dos 7: Focus Sash, 5 Exp. Candy L,
  Assault Vest, 10 Rare Candy, Choice Specs, 3 Ability Capsule, Choice Band; Muramasa: Ability Patch.
- Perrin (com 150 da Kitakami Dex): Noctowl 68, Leafeon 70; depois Bloodmoon Ursaluna 70 (luta de Titan, captura
  garantida) no Timeless Woods em noite de neblina.

### Pós-jogo do The Indigo Disk (Special Coaches no League Club, 2 por vez; 1ª conversa só fala, 2ª permite batalha)

- Líderes (lv 80/81): Katy (Spidops, Heracross, Forretress, Lokix 80, Araquanid 81, Ursaring 81 Tera Bug); Brassius
  (Breloom, Arboliva, Vileplume, Tsareena, Lilligant 80, Sudowoodo 81 Tera Grass); Iono (Kilowattrel, Electrode,
  Bellibolt, Luxray, Electivire 80, Mismagius 81 Tera Electric); Kofu (Pelipper, Veluza, Feraligatr, Wugtrio,
  Clawitzer 80, Crabominable 81 Tera Water); Larry (Oinkologne, Tauros, Dudunsparce, Altaria 82, Flamigo 82 Tera Flying,
  Staraptor 83 Tera Normal); Ryme, dupla (Banette, Mimikyu, Golurk, Spiritomb, Houndstone 80, Toxtricity 81 Tera
  Ghost); Tulip (Farigiraf, Gardevoir, Espathra, Gallade, Delphox 80, Florges 81 Tera Psychic); Grusha (Alolan
  Ninetales, Frosmoth, Beartic, Cetitan, Weavile 80, Altaria 81 Tera Ice).
- Elite 4 da BB League no League Club (3 por dia, dupla): mesmos times da liga com +5 níveis (Crispin 82–84, Amarys
  83–85, Lacey 83–85, Drayton 83–85; Archaludon do Drayton troca Dragon Pulse por Draco Meteor).
- Após 3 trocas: Clavell (time da Starfall Street em lv 80/81); Jacq (Victreebel, Arcanine, Swalot, Mudsdale,
  Galarian Slowbro 80, Farigiraf 81 Tera Psychic). Professores (precisa da amizade): Raifort (Zoroark, Seviper,
  Grumpig, Malamar, Scizor 80, Gengar 81 Tera Ghost); Tyme (Kleavor, Lycanroc Dusk, Drednaw, Stonjourner, Coalossal 80,
  Garganacl 81 Tera Rock); Salvatore (Gothitelle, Honchkrow, Persian, Palossand, Alolan Sandslash 80, Raichu 81 Tera
  Electric); Dendra (Hitmontop, Paldean Tauros Blaze, Paldean Tauros Aqua, Medicham, Hawlucha 80, Hariyama 81 Tera
  Fighting); Hassel (Duraludon, Noivern, Haxorus, Dragalge, Dragonite 82, Baxcalibur 83 Tera Dragon); Saguaro
  (Pachirisu, Alomomola, Alcremie, Vespiquen, Goodra 80, Hatterene 81 Tera Fairy); Miriam (Hypno, Pincurchin, Meganium,
  Glalie, Eelektross 80, Toxapex 81 Tera Poison).
- Após 5 trocas: Rika (Dugtrio, Whiscash, Camerupt, Torterra, Donphan 82, Clodsire 83 Tera Ground); Poppy (Copperajah,
  Bastiodon, Bronzong, Magnezone, Corviknight 82, Tinkaton 83 Tera Steel).
- Após 8 trocas: Geeta (Glimmora 84 Tera Rock, Espathra 84, Chesnaught 84, Avalugg 84, Dragapult 84, Kingambit 85
  Tera Flying).
- Após 10 trocas: Cyrano, secret boss (pedido da "Saffron", 10 BP; dupla): Zebstrika 86, Mandibuzz 86, Gothitelle 86,
  Emboar 87, Samurott 87, Serperior 87 (Tera Rock). Repetível por 10 BP.
- Após o Mochi Mayhem: Carmine, dupla (Mightyena, Toucannon, Morpeko, Ninetales, Scrafty 82, Sinistcha 83 Tera
  Grass); Kieran, dupla (time de campeão +5: 85–87); Nemona (Lycanroc 86, Kommo-o 86, Pawmot 86 + por inicial:
  Sprigatito → Decidueye 86, Ribombee 86, Quaquaval 87 Tera Fighting; Fuecoco → Infernape 86, Milotic 86, Meowscarada
  87 Tera Dark; Quaxly → Greninja 86, Dusknoir 86, Skeledirge 87 Tera Ghost); Arven (Greedent, Cloyster, Scovillain,
  Toedscruel, Garganacl 82, Mabosstiff 83 Tera Dark); Penny (Umbreon, Vaporeon, Jolteon, Flareon, Leafeon 84,
  Sylveon 85 Tera Fairy).

### Golpes listados no detonado (os únicos; `verified: true` para estes)

- Crispin: Heat Rotom [Levitate] @Rocky Helmet – Will-O-Wisp/Overheat/Thunderbolt/Hex; Talonflame [Flame Body]
  @Focus Sash – Sunny Day/Brave Bird/Flare Blitz/Roost; Exeggutor [Harvest] @Sitrus Berry – Leech
  Seed/Psyshock/Protect/Solar Beam; Camerupt [Solid Rock] @Expert Belt – Earth Power/Heat Wave/Ancient Power/Zen
  Headbutt; Magmortar [Flame Body] @Passho Berry – Psychic/Thunderbolt/Heat Wave/Sunny Day; Blaziken [Speed Boost]
  @Assault Vest – Stone Edge/Focus Blast/Temper Flare/Shadow Claw.
- Amarys: Skarmory [Sturdy] @Rocky Helmet – Stealth Rock/Whirlwind/Body Press/Dual Wingbeat; Alolan Dugtrio [Tangling
  Hair] @Focus Sash – Earthquake/Sucker Punch/Iron Head/Stone Edge; Scizor [Technician] @Occa Berry – Bullet
  Punch/Trailblaze/Pounce/Aerial Ace; Reuniclus [Magic Guard] @Leftovers – Trick Room/Protect/Energy Ball/Psychic;
  Empoleon [Torrent] @Mystic Water – Ice Beam/Hydro Pump/Flash Cannon/Grass Knot; Metagross [Clear Body] @Expert Belt
  – Hard Press/Ice Punch/Zen Headbutt/Brick Break.
- Lacey: Whimsicott [Prankster] @Focus Sash – Light Screen/Tailwind/Energy Ball/Moonblast; Granbull [Intimidate]
  @Expert Belt – Play Rough/Fire Punch/Ice Punch/Stomping Tantrum; Primarina [Liquid Voice] @Throat Spray – Hyper
  Voice/Moonblast/Ice Beam/Shadow Ball; Galarian Slowbro [Quick Draw] @Sitrus Berry – Shell Side
  Arm/Psychic/Flamethrower/Ice Beam; Alcremie [Sweet Veil] @Leftovers – Decorate/Recover/Giga Drain/Alluring Voice;
  Excadrill [Mold Breaker] @Assault Vest – Tera Blast/Iron Head/Rock Tomb/High Horsepower.
- Drayton: Dragonite [Multiscale] @Yache Berry – Extreme Speed/Thunder Punch/Ice Spinner/Tailwind; Flygon [Levitate]
  @Life Orb – Earthquake/Stone Edge/Fire Punch/Breaking Swipe; Haxorus [Mold Breaker] @Focus Sash –
  Earthquake/Breaking Swipe/Iron Head/Dragon Dance; Sceptile [Unburden] @Weakness Policy – Dragon Cheer/Leaf
  Storm/X-Scissor/Acrobatics; Kingdra [Swift Swim] @Expert Belt – Rain Dance/Ice Beam/Hydro Pump/Hurricane;
  Archaludon [Stamina] @Assault Vest – Electro Shot/Dragon Pulse/Flash Cannon/Snarl (no League Club: Draco Meteor no
  lugar de Dragon Pulse).
- Kieran (campeão e Special Coach): Politoed [Drizzle] @Wacan Berry – Ice Beam/Weather Ball/Psychic/Helping Hand;
  Dragonite [Multiscale] @Expert Belt – Hurricane/Thunder/Extreme Speed/Breaking Swipe; Grimmsnarl [Prankster] @Focus
  Sash – Sucker Punch/Spirit Break/Light Screen/Reflect; Porygon-Z [Adaptability] @Life Orb – Hyper Beam/Ice
  Beam/Thunderbolt/Shadow Ball; Incineroar [Intimidate] @Sitrus Berry – Darkest Lariat/Brick Break/Flare Blitz/Fake
  Out; Hydrapple [Supersweet Syrup] @Assault Vest – Fickle Beam/Gyro Ball/Tera Blast/Earth Power.
- Cyrano: Zebstrika [Sap Sipper] @Air Balloon – Bulldoze/Supercell Slam/Flame Charge/Trailblaze; Mandibuzz [Overcoat]
  @Sitrus Berry – Toxic/Tailwind/Snarl/Protect; Gothitelle [Shadow Tag] @Chesto Berry – Calm Mind/Stored
  Power/Thunderbolt/Rest; Emboar [Reckless] @Life Orb – Flare Blitz/Head Smash/Hammer Arm/Sucker Punch; Samurott
  [Torrent] @Assault Vest – Aqua Jet/Sacred Sword/X-Scissor/Drill Run; Serperior [Contrary] @Leftovers – Leaf
  Storm/Tera Blast/Scale Shot/Glare.

## Áreas (para `areas/sv.json`, kind e ordem de liberação)

Paldea: mundo aberto; tudo abre depois da caça ao tesouro (exceto Area Zero). A coluna "Liberada em" segue a ordem
do detonado e as habilidades do Koraidon/Miraidon (correr após Klawf, nadar após Bombirdier, pulo alto após Orthworm,
planar após Great Tusk/Iron Treads, escalar após Dondozo/Tatsugiri).

| Ordem | Id | Nome | Kind | Liberada em |
| --- | --- | --- | --- | --- |
| 1 | cabo-poco | Cabo Poco | city | início |
| 2 | private-beach | Private Beach | zone | início |
| 3 | poco-path | Poco Path | route | início |
| 4 | inleth-grotto | Inleth Grotto | dungeon | início |
| 5 | poco-path-lighthouse | Poco Path Lighthouse | building | início |
| 6 | south-province-area-one | South Province (Area One) | route | início |
| 7 | los-platos | Los Platos | city | início |
| 8 | mesagoza | Mesagoza (South/West/East) | city | início |
| 9 | naranja-uva-academy | Naranja Academy [Scarlet] / Uva Academy [Violet] | building | início |
| 10 | south-province-area-two | South Province (Area Two) | route | caça ao tesouro |
| 11 | grand-olive-orchard | The Grand Olive Orchard (Ten Sights) | zone | caça ao tesouro |
| 12 | cortondo | Cortondo | city | caça ao tesouro (1º ginásio) |
| 13 | south-province-area-three | South Province (Area Three) | route | caça ao tesouro (1º Titan) |
| 14 | artazon | Artazon | city | após Klawf |
| 15 | south-province-area-five | South Province (Area Five) | route | após Brassius (opcional) |
| 16 | secluded-beach | Secluded Beach (Ten Sights) | zone | Area Five |
| 17 | west-province-area-one | West Province (Area One) – Central e North | route | após Brassius |
| 18 | segin-squad-base | Segin Squad's Base (Dark Crew) | building | West Province (Area One) – North |
| 19 | south-province-area-four | South Province (Area Four) | route | após Giacomo (opcional) |
| 20 | south-paldean-sea | South Paldean Sea | sea | nadar (após Bombirdier) |
| 21 | east-province-area-one | East Province (Area One) | route | após Giacomo |
| 22 | levincia | Levincia | city | East Province (Area One) |
| 23 | schedar-squad-base | Schedar Squad's Base (Fire Crew) | building | após Iono |
| 24 | east-province-area-two | East Province (Area Two) | route | após Mela |
| 25 | levincia-lighthouse | Levincia Lighthouse | building | após Mela |
| 26 | east-province-area-three | East Province (Area Three) | route | após Mela |
| 27 | million-volt-skyline | Million Volt Skyline (Ten Sights) | zone | após Orthworm |
| 28 | zapapico | Zapapico | city | após Orthworm |
| 29 | cascarrafa | Cascarrafa | city | após Orthworm (teste do ginásio) |
| 30 | asado-desert | Asado Desert | route | teste do Kofu |
| 31 | porto-marinada-lighthouse | Porto Marinada Lighthouse | building | teste do Kofu |
| 32 | porto-marinada | Porto Marinada (leilões) | city | teste do Kofu |
| 33 | navi-squad-base | Navi Squad's Base (Poison Crew) | building | após Kofu |
| 34 | tagtree-thicket | Tagtree Thicket | route | após Kofu |
| 35 | dalizapa-passage | Dalizapa Passage | dungeon | após Atticus |
| 36 | medali | Medali (Treasure Eatery) | city | após Atticus |
| 37 | west-province-area-three | West Province (Area Three) | route | após Larry |
| 38 | glaseados-grasp | Glaseado's Grasp (Ten Sights) | zone | após Larry |
| 39 | montenevera | Montenevera | city | após Larry |
| 40 | colonnade-hollow | Colonnade Hollow (Ten Sights) | zone | após Ryme |
| 41 | west-province-area-two | West Province (Area Two) | route | após Ryme |
| 42 | alfornada-cavern | Alfornada Cavern | dungeon | após Great Tusk/Iron Treads (pulo alto) |
| 43 | south-province-area-six | South Province (Area Six) | route | após Alfornada Cavern |
| 44 | leaking-tower | Leaking Tower of Paldea (Ten Sights) | zone | Area Six |
| 45 | alfornada | Alfornada | city | Area Six |
| 46 | glaseado-mountain | Glaseado Mountain (ginásio isolado, sem cidade) | route | após Tulip |
| 47 | paldeas-highest-peak | Paldea's Highest Peak (Ten Sights) | zone | Glaseado Mountain |
| 48 | north-province-area-three | North Province (Area Three) | route | após Grusha |
| 49 | ruchbah-squad-base | Ruchbah Squad's Base (Fairy Crew) + farol | building | North Province (Area Three) |
| 50 | casseroya-falls | Casseroya Falls | zone | após Ortega |
| 51 | casseroya-lake | Casseroya Lake (Islet) | sea | após Ortega |
| 52 | socarrat-trail | Socarrat Trail | route | após Ortega |
| 53 | gracia-stones | Gracia Stones | zone | Socarrat Trail |
| 54 | north-province-area-one | North Province (Area One) | route | escalar (após Dondozo/Tatsugiri) |
| 55 | north-province-area-two | North Province (Area Two) | route | escalar |
| 56 | fury-falls | Fury Falls (Ten Sights) | zone | North Province (Area Two) |
| 57 | caph-squad-base | Caph Squad's Base (Fighting Crew) | building | North Province (Area Two) |
| 58 | groundblight-shrine | Groundblight Shrine (Ting-Lu; Socarrat Trail) | building | 8 estacas verdes |
| 59 | icerend-shrine | Icerend Shrine (Chien-Pao; West Province Area One) | building | 8 estacas amarelas |
| 60 | grasswither-shrine | Grasswither Shrine (Wo-Chien; South Province Area One) | building | 8 estacas roxas |
| 61 | firescourge-shrine | Firescourge Shrine (Chi-Yu; acima da Fury Falls) | building | 8 estacas azuis |
| 62 | pokemon-league | Pokémon League | building | 8 insígnias |
| 63 | zero-gate | Zero Gate | building | após as 3 rotas |
| 64 | area-zero | Great Crater of Paldea – Area Zero (Estações de Pesquisa 1–4) | dungeon | The Way Home |
| 65 | zero-lab | Zero Lab | building | 4 trancas das estações |
| 66 | north-paldean-sea | North Paldean Sea | sea | nadar (citado para o Kyogre) |
| 67 | kitakami-road | Kitakami Road (Bus Stop) | route | The Teal Mask |
| 68 | mossui-town | Mossui Town | city | The Teal Mask |
| 69 | apple-hills | Apple Hills | route | The Teal Mask |
| 70 | loyalty-plaza | Loyalty Plaza | zone | The Teal Mask |
| 71 | revelers-road | Reveler's Road | route | The Teal Mask |
| 72 | kitakami-hall | Kitakami Hall | building | The Teal Mask |
| 73 | onis-maw | Oni's Maw | route | The Teal Mask |
| 74 | oni-mountain | Oni Mountain | route | The Teal Mask |
| 75 | dreaded-den | Dreaded Den | dungeon | The Teal Mask |
| 76 | infernal-pass | Infernal Pass (6 maravilhas) | zone | The Teal Mask |
| 77 | crystal-pool | Crystal Pool (6 maravilhas) | zone | The Teal Mask |
| 78 | mossfell-confluence | Mossfell Confluence (6 maravilhas) | zone | The Teal Mask |
| 79 | fellhorn-gorge | Fellhorn Gorge / Fallen Horn (6 maravilhas) | zone | The Teal Mask |
| 80 | wistful-fields | Wistful Fields / Wisteria Pond (6 maravilhas) | zone | The Teal Mask |
| 81 | kitakami-wilds | Kitakami Wilds / Gracious Stones (6 maravilhas) | zone | The Teal Mask |
| 82 | paradise-barrens | Paradise Barrens | zone | The Teal Mask |
| 83 | timeless-woods | Timeless Woods | zone | pós-Teal Mask (Perrin, 150 da Kitakami Dex) |
| 84 | blueberry-academy | Blueberry Academy (Entrance, League Club, Cafeteria, dormitórios) | building | The Indigo Disk |
| 85 | terarium-central | Terarium – Central Plaza | zone | The Indigo Disk |
| 86 | savanna-biome | Terarium – Savanna Biome (Savanna Plaza, 2 rest areas) | zone | The Indigo Disk |
| 87 | coastal-biome | Terarium – Coastal Biome (Coastal Plaza) | zone | The Indigo Disk |
| 88 | canyon-biome | Terarium – Canyon Biome (Canyon Plaza) | zone | The Indigo Disk |
| 89 | polar-biome | Terarium – Polar Biome (Polar Plaza, 2 outdoor classrooms) | zone | The Indigo Disk |
| 90 | area-zero-underdepths | Area Zero Underdepths | dungeon | Indigo Disk (o disco abre o elevador do Zero Lab) |
| 91 | underdepths-crystal-cave | Caverna secreta de cristal (Underdepths) | dungeon | pós-Indigo Disk (Garchomp Tera Stellar) |

## Encontros (para `encounters/sv.json`; method `walk` salvo indicação; exclusivos marcados como no detonado)

Cada área tem também Pokémon **terastalizados fixos** (`method: fixed`, voltam no dia seguinte após capturados; só
dá para capturar com o HP no vermelho).

- Poco Path: Lechonk, Tarountula, Pawmi, Fletchling, Hoppip, Scatterbug.
- Inleth Grotto (`cave`): Diglett, Yungoos, Houndour.
- South Province (Area One): Paldean Wooper, Fidough, Wiglett, Shroodle, Flamigo, Lechonk, Tarountula, Hoppip,
  Fletchling, Pawmi, Houndour, Psyduck, Buizel, Azurill, Wingull, Magikarp, Scatterbug, Spewpa, Surskit, Chewtle,
  Oricorio, Gastly, Drowzee, Pachirisu, Pichu, Happiny, Sunkern, Ralts, Igglybuff, Bonsly, Bounsweet. Gimmighoul
  (forma de moedas) só dá moedas; a forma de baú é capturável mais tarde.
- South Province (Area Two, inclui Grand Olive Orchard): Smoliv, Maschiff, Tadbulb, Nacli, Tinkatink, Fidough, Pawmi,
  Shroodle, Psyduck, Starly, Fletchling, Combee, Vespiquen, Sunkern, Pichu, Pikachu, Eevee, Igglybuff, Jigglypuff,
  Happiny, Yungoos, Mareep, Azurill, Marill, Makuhita, Hoppip, Bonsly, Buizel, Drowzee, Bronzor, Bounsweet, Chewtle,
  Skwovet, Surskit, Kricketot. Tera: Diglett (Rock), Meowth (Flying), Fletchling (Fire).
- South Province (Area Three): Charcadet, Klawf, Nacli, Nymble, Pawmi, Tinkatink, Growlithe, Gastly, Drowzee, Spoink,
  Gulpin [Violet], Makuhita, Hariyama, Bagon [Violet], Murkrow, Shinx, Bronzor, Happiny, Skiddo, Dunsparce, Rookidee,
  Yungoos, Shuppet, Squawkabilly. Tera: Jigglypuff (Water), Dunsparce (Poison).
- South Province (Area Five, inclui Secluded Beach): Nacli, Charcadet, Flittle, Wattrel, Finizen, Lechonk, Oinkologne,
  Pawmi, Pawmo, Tarountula, Paldean Wooper, Clodsire, Tadbulb, Shroomish, Stunky [Scarlet], Dreepy [Violet], Pawniard,
  Stantler, Litleo, Seviper, Zangoose, Drifloon [Scarlet], Misdreavus [Violet], Goomy, Bagon [Violet], Mankey, Slakoth,
  Vigoroth, Rookidee, Corvisquire, Shinx, Luxio, Axew, Deerling, Rockruff, Fomantis, Larvitar, Hoppip, Skiploom,
  Makuhita, Yungoos, Gumshoos, Dunsparce, Skiddo, Murkrow, Fletchling, Fletchinder, Pachirisu, Croagunk, Flamigo,
  Mudbray, Gastly, Basculin, Buizel, Psyduck, Surskit, Masquerain, Marill, Slowpoke, Shellos, Wiglett, Sandygast,
  Pincurchin, Crabrawler, Chewtle, Drednaw, Magikarp, Gyarados. Tera: Shinx (Ice), Flamigo (Bug), Houndstone (Ground).
- West Province (Area One): Capsakid, Nymble, Flittle, Charcadet, Nacli, Tinkatink, Tadbulb, Cyclizar, Mudbray, Skiddo,
  Phanpy, Rockruff, Mankey, Primeape, Pawmi, Pawmo, Flabébé, Floette, Diglett, Salandit, Sableye, Gible, Larvitar
  [Scarlet], Bagon [Violet], Psyduck, Buizel, Chewtle, Arrokuda, Basculin, Swablu, Drifloon [Scarlet], Misdreavus
  [Violet], Gastly, Haunter, Bronzor, Yungoos, Gumshoos, Makuhita, Numel, Falinks, Bombirdier, Combee, Vespiquen.
  Tera: Buizel (Ice), Torkoal (Normal), Lokix (Fighting), Axew (Ground), Tinkatuff (Dark), Rockruff (Psychic), Jolteon
  (Electric), Altaria (Fairy).
- South Province (Area Four): Tadbulb, Toedscool, Maschiff, Flittle, Nacli, Tinkatink, Pawmi, Pawmo, Lechonk,
  Tarountula, Grumpig, Scyther, Chansey, Komala, Persian, Primeape, Arcanine, Houndour, Houndoom, Flaaffy, Hatenna,
  Petilil, Lilligant, Hawlucha, Pikachu, Gastly, Rockruff, Skiddo, Goomy, Toxel, Psyduck, Swablu, Marill, Paldean
  Wooper, Clodsire, Meditite, Rufflet, Riolu, Mudbray, Flabébé, Floette, Phanpy, Drifloon [Scarlet], Misdreavus
  [Violet], Dreepy [Violet], Murkrow, Fletchinder, Salazzle, Deerling, Sawsbuck, Dunsparce, Chewtle, Bronzor, Makuhita,
  Yungoos, Pachirisu, Buizel, Starly, Staravia, Combee, Magikarp, Basculin. Tera: Scyther (Normal), Sliggoo (Poison),
  Golduck (Psychic), Leafeon (Grass).
- South Paldean Sea (`surf`): Finizen, Wattrel, Shellder, Cloyster, Arrokuda, Barraskewda, Magikarp, Gyarados, Wingull,
  Qwilfish, Luvdisc, Buizel, Floatzel, Skrelp, Clauncher, Clawitzer, Eelektrik.
- East Province (Area One): Paldean Tauros, Tandemaus, Veluza, Toedscool, Wattrel, Squawkabilly, Cyclizar, Shroodle,
  Tarountula, Spidops, Nacli, Pawmi, Pawmo, Lechonk, Oinkologne, Tadbulb, Venonat, Teddiursa, Skiddo, Oricorio, Pineco,
  Rookidee, Corvisquire, Deerling, Gulpin [Violet], Bounsweet, Steenee, Litleo, Komala, Makuhita, Dunsparce, Hoppip,
  Skiploom, Yungoos, Gumshoos, Murkrow, Pikachu, Azurill, Marill, Wiglett, Finizen, Buizel, Shellos, Magikarp,
  Gyarados, Mareanie, Slowpoke. Tera: Floette (Grass), Toxapex (Normal), Salandit (Poison).
- East Province (Area Two): Paldean Tauros, Tadbulb, Wattrel, Shroodle, Toedscool, Flittle, Tinkatink, Tinkatuff,
  Wiglett, Cyclizar, Flamigo, Spidops, Pawmo, Tandemaus, Squawkabilly, Nacli, Lechonk, Oinkologne, Pachirisu, Kirlia,
  Grimer, Magnemite, Chewtle, Drednaw, Dunsparce, Rookidee, Corvisquire, Psyduck, Slowpoke, Gastly, Litleo, Skiddo,
  Yungoos, Gumshoos, Crabrawler, Crabominable, Bounsweet, Steenee, Sandygast, Komala, Makuhita, Buizel, Marill,
  Deerling, Basculin, Hoppip, Skiploom, Murkrow, Magikarp, Pincurchin, Shuppet, Drifloon [Scarlet], Bronzor, Rotom,
  Misdreavus, Dreepy [Violet], Arrokuda, Barraskewda, Chansey, Pineco. Tera: Growlithe (Ground), Paldean Tauros
  (Electric).
- East Province (Area Three): Bramblin, Varoom, Orthworm, Glimmet, Nacli, Flamigo, Tadbulb, Lokix, Squawkabilly,
  Tandemaus, Cufant, Rolycoly, Carkol, Silicobra, Voltorb, Meowth, Rookidee, Corvisquire, Torkoal, Diglett, Dugtrio,
  Larvitar [Scarlet], Pawniard, Rufflet, Makuhita, Hariyama, Dunsparce, Gastly, Dreepy [Violet], Bagon [Violet],
  Growlithe, Magnemite, Shuppet, Skiddo, Gothita, Murkrow, Salandit, Salazzle, Sableye, Deino [Scarlet], Misdreavus
  [Violet], Goomy, Sinistea, Yungoos, Gumshoos, Barboach, Buizel, Psyduck, Magikarp, Chewtle, Drednaw, Arrokuda,
  Barraskewda, Basculin. Tera: Kirlia (Fire), Falinks (Steel), Pyroar (Dark), Espeon (Psychic).
- Tagtree Thicket: Shroodle, Grafaiai, Tarountula, Spidops, Tadbulb, Bellibolt, Toedscool, Charcadet, Oinkologne,
  Flamigo, Impidimp, Morgrem, Oranguru [Scarlet], Passimian [Violet], Mimikyu, Pawniard, Venonat, Venomoth, Foongus,
  Zorua, Komala, Pineco, Murkrow, Skwovet, Greedent, Dunsparce, Buizel, Combee, Psyduck, Magikarp, Basculin.
  Tera: Grafaiai (Bug).
- Dalizapa Passage (`cave`): Scovillain, Lokix, Flittle, Espathra, Greavard, Houndstone, Frigibax, Cetoddle, Glimmet,
  Tinkatuff, Nacli, Naclstack, Cyclizar, Snorunt, Snom, Sneasel, Weavile, Gogoat, Kirlia, Gallade, Vigoroth, Litleo,
  Pyroar, Teddiursa, Ursaring, Spoink, Grumpig, Rufflet, Dugtrio, Magnemite, Magneton, Larvitar [Scarlet], Axew,
  Fraxure, Gible, Gabite, Deino [Scarlet], Bagon [Violet], Torkoal, Cubchoo, Beartic, Misdreavus [Violet], Murkrow,
  Honchkrow, Salandit, Salazzle, Lycanroc, Medicham, Makuhita, Hariyama, Sableye, Mudbray, Mudsdale, Flareon, Umbreon,
  Chansey, Deerling, Sawsbuck, Dunsparce, Skwovet, Greedent, Gastly, Haunter, Cufant, Copperajah, Phanpy, Donphan,
  Hypno, Froslass, Yungoos, Gumshoos, Drifloon [Scarlet], Drifblim [Scarlet]. Tera: Sneasel (Fighting), Espathra
  (Steel), Ursaring (Flying), Lucario (Ice).
- West Province (Area Three): Fidough, Dachsbun, Maschiff, Mabosstiff, Toedscool, Cetoddle, Greavard, Bombirdier,
  Lokix, Tarountula, Spidops, Flamigo, Tinkatuff, Charcadet, Tandemaus, Shroodle, Tadbulb, Bellibolt, Nacli, Naclstack,
  Oinkologne, Pawmi, Pawmo, Tropius, Sudowoodo, Voltorb, Electrode, Hypno, Bronzong, Dedenne, Meowth, Persian, Hoppip,
  Jumpluff, Shroomish, Breloom, Pineco, Vigoroth, Scyther, Deerling, Sawsbuck, Gogoat, Jigglypuff, Ursaring, Chansey,
  Pachirisu, Pikachu, Eevee, Jolteon, Espeon, Leafeon, Sylveon, Ditto, Primeape, Staravia, Fletchinder, Murkrow,
  Honchkrow, Foongus, Lilligant, Steenee, Komala, Applin, Goomy, Sliggoo, Dunsparce, Drifloon [Scarlet], Skwovet,
  Greedent, Buizel, Floatzel, Marill, Azumarill, Psyduck, Golduck, Basculin, Chewtle, Drednaw, Magikarp, Barboach,
  Whiscash. Tera: Breloom (Rock), Rockruff (Psychic), Altaria (Fairy).
- West Province (Area Two, inclui Colonnade Hollow): Paldean Tauros, Paldean Tauros Blaze [Scarlet], Paldean Tauros
  Aqua [Violet], Cyclizar, Lokix, Tandemaus, Nacli, Toedscool, Pawmi, Pawmo, Maschiff, Flamigo, Oinkologne, Wattrel,
  Wiglett, Wugtrio, Finizen, Ditto, Croagunk, Tadbulb, Bellibolt, Flaaffy, Larvitar [Scarlet], Gible, Gabite, Noibat,
  Bagon [Violet], Dreepy [Violet], Girafarig, Meditite, Makuhita, Hariyama, Slowpoke, Slowbro, Diglett, Dugtrio,
  Chansey, Phanpy, Donphan, Skiploom, Sableye, Stunky [Scarlet], Murkrow, Salandit, Dunsparce, Meowth, Skiddo, Grimer,
  Kirlia, Deerling, Staravia, Yungoos, Gumshoos, Sandygast, Palossand, Psyduck, Marill, Azumarill, Chewtle, Drednaw,
  Buizel, Floatzel, Magikarp, Pincurchin, Wingull, Shellos. Tera: Naclstack (Ghost), Garchomp (Water).
- Asado Desert: Rellor, Bramblin, Flittle, Espathra, Lokix, Orthworm, Capsakid, Nacli, Tinkatink, Tinkatuff, Pawmi,
  Pawmo, Stonjourner, Silicobra, Sandaconda, Cacnea, Cacturne, Sandile, Rufflet, Braviary, Larvesta, Phanpy, Donphan,
  Hippopotas, Hippowdon, Skiddo, Yungoos, Gumshoos, Bronzor, Gastly, Falinks, Murkrow. Tera: Sandygast (Grass),
  Cyclizar (Flying).
- Alfornada Cavern (`cave`): Glimmet, Toxtricity, Deino, Gible, Gabite, Bagon, Shelgon, Larvitar, Pupitar, Diglett,
  Dugtrio, Dunsparce, Sableye, Salandit, Salazzle, Meditite, Medicham, Makuhita, Hariyama, Yungoos, Gumshoos. Tera:
  Houndoom (Ghost), Umbreon (Dark). (Aqui o detonado não marca versão em Deino/Bagon/Larvitar.)
- South Province (Area Six, inclui Leaking Tower): Fidough, Dachsbun, Scovillain, Nymble, Lokix, Flamigo, Flittle,
  Espathra, Bombirdier, Pawmi, Pawmo, Nacli, Naclstack, Tinkatuff, Bellibolt, Gothorita, Gothitelle, Gogoat, Klefki,
  Murkrow, Honchkrow, Toxtricity, Lycanroc, Flaaffy, Ampharos, Dreepy, Sinistea, Greavard, Houndstone, Drifloon,
  Drifblim, Banette, Vaporeon, Flareon, Sylveon, Rufflet, Mudbray, Mudsdale, Phanpy, Donphan, Cufant, Copperajah,
  Swablu, Altaria, Chansey, Meditite, Medicham, Combee, Vespiquen, Skwovet, Greedent, Yungoos, Gumshoos, Hypno,
  Bronzong, Chewtle, Drednaw, Psyduck, Golduck, Basculin, Magikarp. Tera: Dragonair (Electric), Hawlucha (Bug).
- Glaseado Mountain (inclui Paldea's Highest Peak): Greavard, Houndstone, Tadbulb, Bellibolt, Flamigo, Nacli,
  Naclstack, Lokix, Paldean Wooper, Clodsire, Cetoddle, Cetitan, Crabrawler, Crabominable, Snorunt, Glalie, Froslass,
  Snover, Abomasnow, Sneasel, Weavile, Glaceon, Cubchoo, Beartic, Cryogonal, Bergmite, Avalugg, Snom, Frosmoth,
  Delibird, Deerling, Sawsbuck, Spiritomb, Cufant, Copperajah, Murkrow, Honchkrow, Axew, Fraxure, Magnemite, Magneton,
  Bronzong, Hawlucha, Gastly, Haunter, Buizel, Floatzel, Chewtle, Drednaw, Psyduck, Golduck, Magikarp. Tera: Frigibax
  (Dark), Avalugg (Rock), Veluza (Normal), Primeape (Poison), Glaceon (Ice), Revavroom (Fire), Pincurchin (Electric).
- North Province (Area Three): Pawmi, Pawmo, Wugtrio, Nacli, Naclstack, Fomantis, Lurantis, Floette, Florges,
  Vivillon, Chansey, Blissey, Vaporeon, Espeon, Umbreon, Sunflora, Hoppip, Jumpluff, Hawlucha, Combee, Vespiquen,
  Cufant, Copperajah, Makuhita, Hariyama, Murkrow, Honchkrow, Golduck, Buizel, Floatzel. Tera: Sylveon (Fairy), Pawmo
  (Steel).
- Casseroya Lake / Casseroya Falls (`walk`/`surf`): Tatsugiri, Dondozo, Veluza, Lokix, Kilowattrel, Flamigo, Nacli,
  Naclstack, Gyarados, Dratini, Dragonair, Dragonite, Goomy, Sliggoo, Swablu, Altaria, Slowpoke, Slowbro, Scyther,
  Gogoat, Tropius, Hawlucha, Staraptor, Bronzong, Cufant, Copperajah, Stunky, Skuntank, Croagunk, Toxicroak, Chewtle,
  Drednaw, Marill, Azumarill, Psyduck, Golduck. Tera: Gyarados (Dragon), Slowbro (Grass), Vaporeon (Water), Drednaw
  (Ice), Azumarill (Fairy), Bellibolt (Water).
- Socarrat Trail (inclui Gracia Stones): Toedscool, Toedscruel, Maschiff, Mabosstiff, Spidops, Heracross, Forretress,
  Slaking, Amoonguss, Sudowoodo, Fomantis, Lurantis, Stunky, Skuntank, Murkrow, Honchkrow, Deerling, Sawsbuck.
  Tera: Tsareena (Psychic).
- North Province (Area One): Scovillain, Brambleghast, Varoom, Revavroom, Greavard, Houndstone, Nacli, Naclstack,
  Tinkatuff, Flittle, Espathra, Pawmi, Pawmo, Lokix, Indeedee, Noibat, Noivern, Teddiursa, Ursaring, Mareep, Ampharos,
  Spoink, Grumpig, Flareon, Vaporeon, Swablu, Altaria, Mudbray, Mudsdale, Lycanroc, Floette, Gogoat, Riolu, Lucario,
  Chansey, Blissey, Vivillon, Rufflet, Braviary, Hawlucha, Deerling, Sawsbuck, Bronzong, Dratini, Golduck. Tera:
  Wugtrio (Ground), Eelektross (Dragon).
- North Province (Area Two, inclui Fury Falls): Lokix, Grafaiai, Tarountula, Spidops, Nacli, Naclstack, Kricketot,
  Kricketune, Foongus, Amoonguss, Venomoth, Pawniard, Bisharp, Shinx, Luxray, Oranguru, Heracross, Teddiursa, Ursaring,
  Spoink, Grumpig, Gogoat, Scyther, Hawlucha, Falinks, Murkrow, Honchkrow, Fomantis, Lurantis, Houndour, Houndoom,
  Growlithe, Arcanine, Numel, Camerupt, Vaporeon, Umbreon, Cufant, Copperajah, Pupitar, Dugtrio, Salandit, Salazzle,
  Swablu, Altaria, Noibat, Noivern, Deino, Zweilous, Dratini, Dragonite, Golduck. Tera: Slaking (Ghost), Corviknight
  (Fighting), Dragonite (Steel), Mimikyu (Fairy).
- Area Zero: Paradoxos que vieram pela máquina do tempo ficam capturáveis a partir da caverna de cristais antes da
  Estação 4 (o detonado não lista as espécies). Na forma de baú, Gimmighoul aparece "mais adiante" (local não dito).
- Mass Outbreaks (`outbreak`): ícone de Pokémon no mapa; grupo de uma espécie; o detonado não lista espécies.
- Tera Raid Battles (`tera_raid`): cristais com o ícone do tipo Tera no mapa; ver "Mecânicas" para as estrelas. Raids
  de 5★ dão Herba Mystica (a Sweet pedida pelo Saguaro).
- Kitakami e Terarium: o detonado **não** traz listas de Pokémon por área. Fatos soltos: Coastal Biome tem formas de
  Alola (Alolan Exeggutor, Alolan Grimer, Alolan Muk, Alolan Diglett, Alolan Dugtrio); Pokémon Tera Stellar (brilho de
  arco-íris) passam a surgir no Terarium após o Terapagos; Pokémon selvagens terastalizam no Terarium.
- Iniciais de Kanto a Galar no Terarium (`walk`, após pedido do Terry no League Club, 3.000 BP cada bioma):
  Coastal: Bulbasaur, Chikorita, Mudkip, Froakie, Popplio, Grookey. Savanna: Charmander, Totodile, Snivy, Fennekin,
  Rowlet, Sobble. Canyon: Squirtle, Treecko, Turtwig, Tepig, Chespin, Litten. Polar: Cyndaquil, Torchic, Chimchar,
  Piplup, Oshawott, Scorbunny.

### Lendários e fixos (`method: fixed`)

- Titans capturáveis após vencer cada um, no mesmo nível e local: Klawf 16 (topo do penhasco), Bombirdier 20 (descida
  da montanha), Orthworm 29 (frente da caverna), Great Tusk [Scarlet]/Iron Treads [Violet] 45 (centro do deserto).
  (O detonado não diz como capturar Dondozo/Tatsugiri.)
- Tesouros da ruína, lv 60 (8 estacas cada): Ting-Lu (Groundblight Shrine, Socarrat Trail; estacas verdes em volta do
  Casseroya Lake); Chien-Pao (Icerend Shrine, West Province Area One; estacas amarelas nas West Areas 1–3); Wo-Chien
  (Grasswither Shrine, South Province Area One; estacas roxas nas South Areas 1, 3, 5); Chi-Yu (Firescourge Shrine,
  acima da Fury Falls; estacas azuis nas North Areas 1–2).
- Koraidon [Scarlet] / Miraidon [Violet] lv 72: segundo exemplar nas profundezas da Area Zero (pós-jogo; o de montaria
  não vai ao Box nem troca).
- Teal Mask: Ogerpon (captura garantida após o teste, lv 70); Okidogi lv 70 (Paradise Barrens), Munkidori lv 70
  (Wisteria Pond), Fezandipiti lv 70 (Oni Mountain), após a história; Bloodmoon Ursaluna lv 70 (Timeless Woods, noite
  de neblina, captura garantida).
- Indigo Disk: Terapagos lv 85 (captura garantida); Garchomp Tera Stellar lv 80 (abre a caverna de cristal);
  Gouging Fire lv 75 [Scarlet] (topo da Area Zero, perto de um satélite), Raging Bolt lv 75 [Scarlet] (fundo da Area
  Zero, águas rasas perto de cachoeira), Iron Boulder lv 75 [Violet] (parte baixa, sob grande rocha), Iron Crown lv 75
  [Violet] (parte alta, perto da Estação 1) — após 200 da Blueberry Dex (Perrin); Meloetta lv 70 (Coastal Biome, à
  esquerda da Outdoor Classroom: girar no sentido horário 10 s no redemoinho de folhas e pôr o filtro Sépia na
  câmera); Pecharunt lv 88 (Mochi Mayhem).
- Snacksworth (entrada da Blueberry Academy; 1 lanche a cada 10 BBQs solo ou 1 por Group Quest), todos lv 70 salvo
  Kubfu: ambos solo: Articuno (Glaseado Mountain, norte do lago à esquerda de Montenevera), Zapdos (topo do Poco Path
  Lighthouse), Moltres (montanha ao norte da saída do Asado Desert), Kubfu lv 30 (frente da Fury Falls).
  Solo em Scarlet / Group em Violet: Raikou (lago no topo da montanha, West Province Area One), Entei (extremo leste da
  East Province Area Three), Suicune (ilha à esquerda da ilha com Fly, Casseroya Lake), Ho-Oh (colina circular perto
  da cachoeira, South Province Area Four), Latios (último lago ao sul, North Province Area Two), Groudon (Alfornada
  Cavern), Reshiram (topo de cachoeira ao sul de Zapapico, East Province Area One), Solgaleo (telhado da Pokémon
  League), Glastrier (centro-sul da Glaseado Mountain). Group em Scarlet / solo em Violet: Cobalion (penhasco ao sul
  da Fury Falls), Terrakion (à direita da base da Dark Crew, junto à parede da cratera), Virizion (norte do rio, extremo
  leste do Tagtree Thicket), Lugia (ilhota no norte da North Province Area One), Latias (prainha no sul da South
  Province Area Four), Kyogre (mar ao norte das Gracia Stones), Zekrom (montanha de pedra ao sul de Artazon, South
  Province Area Five), Lunala (extremo oeste da faixa entre o Casseroya Lake e o rio ao sul), Spectrier (ruínas junto à
  parede da cratera, à esquerda do Dalizapa Passage). Só Group Quest: Rayquaza (topo da cratera perto da South Province
  Area Three), Kyurem (cavernas do Dalizapa Passage, ao norte e à esquerda do Centro Pokémon), Necrozma (montanha de
  pedra no meio do Socarrat Trail).

## Presentes, trocas in-game, ovos e iniciais (`method: gift` / `trade`)

- Inicial: Sprigatito/Fuecoco/Quaxly (Cabo Poco; nível do seu não dito, o da Nemona é 5).
- Koraidon [Scarlet] / Miraidon [Violet]: montaria (Poco Path Lighthouse, pela pokébola que o Arven entrega); batalha
  só no pós-jogo (7º Pokémon).
- Ovo de Turtwig/Chimchar/Piplup (aleatório; Jacq, Reveler's Road, Kitakami).
- Galarian Meowth lv 5 (Salvatore, fim da amizade). Hisuian Growlithe lv 15 + Choice Scarf (Perrin, após o Bloodmoon
  Ursaluna).
- Trocas com Special Coaches (na 3ª visita, dá um Pokémon seu capturado por você, não shiny, não lendário/mítico;
  recebido vem com Partner Ribbon): Combee lv 15 (Katy), Sunflora lv 40 (Brassius), Magnemite lv 22 (Iono), Veluza
  lv 20 (Kofu), Dudunsparce lv 35 (Larry), Greavard lv 10 (Ryme), Flittle lv 32 (Tulip), Cetoddle lv 37 (Grusha),
  Magby lv 12 (Crispin), Skarmory lv 40 (Amarys), Snubbull lv 18 (Lacey), Duraludon lv 36 (Drayton), Pawmo lv 24
  (Clavell), Gulpin lv 17 (Jacq), Gimmighoul lv 30 (Raifort), Rockruff lv 12 (Tyme), Alolan Meowth lv 21 (Salvatore),
  Meditite lv 28 (Dendra), Arctibax lv 45 (Hassel), Hattrem lv 33 (Saguaro), Mareanie lv 20 (Miriam), Paldean Wooper
  lv 8 (Rika), Tinkatuff lv 25 (Poppy), Glimmet lv 5 (Geeta), Blitzle shiny lv 20 (Cyrano), Poltchageist lv 30
  (Carmine), Applin lv 15 (Kieran), Paldean Tauros lv 50 (Nemona), Skwovet lv 7 (Arven), Eevee lv 1 (Penny).
- Itens de evolução por troca de materiais (Zapapico): Auspicious Armor [Scarlet] por 10 Bronzor Fragments (velho);
  Malicious Armor [Violet] por 10 Sinistea Chips (velha). Syrupy Apple à venda numa casa no Mossfell Confluence.
- Fósseis: o detonado não cita.

## TMs, itens-chave e recompensas citados (localização)

- TMs: TM032 Swift (Nemona, Inleth Grotto); TM082 Thunder Wave (rep. da Liga, Mesagoza South, 4 treinadores);
  TM027 Aerial Ace (rep., Cortondo, 5); TM084 Stomping Tantrum (rep., Levincia, 6); TM149 Earthquake (rep., Cascarrafa
  North, 6 do deserto); TM165 Flare Blitz (rep., Alfornada, 7); TM143 Blizzard (rep., Glaseado Mountain, 10);
  TM171 Tera Blast (Geeta, após Larry); TM169 Draco Meteor (Penny, fim da Starfall Street); TM057 False Swipe (Jacq,
  30 na Pokédex); TM140 Nasty Plot (Raifort, ao mostrar os 4 tesouros da ruína). TMs especiais também dos atendentes
  das rest areas do Terarium (não listados).
- Outras recompensas do representante da Liga (rapaz de terno; vencer N treinadores da área): Shell Bell (South Area
  Three, 7), Eviolite (Secluded Beach, 7), Clear Amulet (West Area One – North, 9), Rocky Helmet (South Area Four, 5),
  Loaded Dice (East Area One, 5), Punching Glove (Zapapico, 7), Expert Belt (Dalizapa Passage, 4), Amulet Coin (Medali
  East, 9), Focus Sash (West Area Two, perto de Porto Marinada, 4).
- Itens-chave: Rotom Phone (mãe, Cabo Poco); Tera Orb (Nemona, Mesagoza); Lucky Egg (Jacq, ginásio de Montenevera);
  Scarlet Book/Violet Book (Arven, Estação 2); Master Ball (Clavell, após a Area Zero; Drayton, após Kieran campeão);
  Roto-Stick (zelador, Mossui Town); Teal Mask e as outras 3 máscaras da Ogerpon; Teal Style Card e Indigo Style Card;
  Exp. Charm (1º jogo do ogro no festival); Indigo Disk (Geeta); Catching Charm + League Club Uniform (Drayton);
  Briar's Book (Briar, após créditos do Indigo Disk); Synchro Machine (Synclaire, Central Plaza); Mythical Pecha Berry
  (Mystery Gift).
- Pokédex: Jacq dá TM057 (30), 20 Ultra Balls (100), 20 Quick Balls (200), Shiny Charm + diploma (400 da Paldea Dex);
  Glimmering Charm (200 da Kitakami Dex, Jacq na Reveler's Road); Mark Charm (240 da Blueberry Dex, Cyrano); Sport Ball
  (Perrin, após os paradoxos das fotos).
- Amizade com professores: 50 Rock Tera Shards (Tyme), TM140 Nasty Plot (Raifort), Galarian Meowth (Salvatore),
  10 Protein (Dendra), 50 Dragon Tera Shards (Hassel), Slowpoke Cup (Saguaro, pela Sweet Herba Mystica), 10 Max Revives
  (Miriam), Big Nugget (Clavell). Tutor da Team Star: Gold Bottle Cap + roupas da Team Star.
- Aulas: 5 Exp. Candy S (prova intermediária), 5 Exp. Candy M (prova final), 5 Exp. Candy L (todas as provas).
- Recompensas das bases da Team Star: 5.000 / 6.000 / 7.000 / 10.000 / 20.000 LP (Giacomo, Mela, Atticus, Ortega, Eri)
  e mais TMs liberados na TM Machine. Nemona dá 5.000 LP em Los Platos.
- Minijogos repetíveis: Cortondo Olive Roll (30 Olive Oil; <10 min 10 Apples; <5 min 10 Pineapples; <2:30 B&W Grass
  Tablecloth); Artazon Sunflora Hide & Seek (10: Leaf Stone; 20: Sun Stone); Alfornada ESP (Berries); Glaseado Snow
  Slope Run (Powder Snow: Never-Melt Ice; Blizzard: Ice Stone; Sheer Cold: Comet Shard); voo da Amarys (Easy 50,
  Normal 100, Hard 200, Super-Hard 500 BP na 1ª vez; depois 10/20/30/50).
- Lojas: Poké Mart nos Centros Pokémon (dinheiro e LP); Delibird Presents e lojas de roupa em Mesagoza; leilões diários
  em Porto Marinada (3 por dia; 4º especial com item de forma); Treasure Eatery (Medali) troca o tipo Tera por 50 Tera
  Shards do tipo; mercearia de Mossui Town; School Store da Blueberry (BP); máquinas de compra nas plazas/rest areas.

## Mecânicas (para o guia de mecânicas, texto nosso)

- Três rotas livres (Victory Road, Path of Legends, Starfall Street) e depois The Way Home; níveis fixos por área.
- Insígnias de ginásio aumentam o nível de obediência dos Pokémon trocados e facilitam capturas de nível alto; bases da
  Team Star liberam TMs na máquina.
- Titans dão as Herba Mystica e as habilidades do Koraidon/Miraidon: Sweet (Klawf, correr), Bitter (Bombirdier,
  nadar), Salty (Orthworm, pulo alto), Sour (Great Tusk/Iron Treads, planar), Spicy (Dondozo/Tatsugiri, escalar). Voo
  livre só no Indigo Disk (suplemento da Amarys).
- Star Barrage (bases da Team Star): 3 primeiros Pokémon em Auto Battle, 30 Pokémon em 10 min; nos STCs, 50 Pokémon.
- Let's Go/Auto Battle com (R); shiny nunca é derrotado em Auto Battle. Treinador de balão preto = mais forte da área.
- Dia e noite: 72 min por dia de jogo (1 h = 3 min; manhã 4–6 h, dia 6–16 h, pôr do sol 16–18 h, noite 18–4 h); clima
  dura 5–6 min. Kitakami e Terarium têm "fuso" próprio e ficam travados durante a história.
- Tera Raid Battles: 1–2★ de início; 3★ com 3 insígnias (Hidden Ability, Tera Shards); 4★ com 7; 5★ após as 3 rotas e a
  Area Zero (Herba Mystica); 6★ após o 1º Academy Ace Tournament; 7★ só em eventos online. Equipe de 4. O cristal
  recarrega o Tera Orb.
- Terastal: 1 vez por recarga no Centro Pokémon (no Terarium e após o Terapagos, sem recarga). Tera Stellar: bônus uma
  vez por tipo de golpe (ilimitado em raids). Mudar o tipo Tera: 50 Tera Shards (Treasure Eatery, Medali).
- Picnic e sanduíches: banho, bola, cura; Meal Powers dependem de recheios e condimentos; Sparkling Power aumenta
  shiny; Egg Power ajuda ovos. O detonado **não** traz receitas.
- TM Machine com materiais de Pokémon e LP; filtro por Pokémon (Kitakami em diante). Item Printer (Reese, 200 BP;
  upgrades em 25/75/175 itens; Poké Ball Lotto; Stellar Tera Shards após o Terapagos).
- BBQs (Blueberry Quests) dão BP; bônus a cada 10; Group Quests pelo Union Circle valem mais. Atendentes dos biomas:
  100/200/500 BP por 5/10/todos os treinadores.
- Synchro Machine (só no Terarium): controla o 1º Pokémon; não captura nessa forma.
- Checagem de IVs no Box após a Area Zero. Chance de shiny citada no quiz: 1 em 4.000. Sem Repel; Poké Doll para
  fugir; dá para fugir de treinadores (conta como derrota).
- Gimmighoul: moedas no mapa evoluem a espécie; só a forma de baú é capturável.
- Batalhas em dupla em toda a Blueberry Academy.

### Pós-jogo (ordem)

1. Créditos → Master Ball (Clavell) e insígnia de Hall of Fame; troca de forma do Koraidon/Miraidon; IVs.
2. Capturar o 2º Koraidon/Miraidon (lv 72, Area Zero).
3. Revanches da Team Star (STC) e inspeção dos 8 ginásios → King's Rock.
4. Academy Ace Tournament (libera raids 6★); aulas, amizades, dormitórios de Nemona, Arven e Penny.
5. The Teal Mask (Kitakami): história → Ogerpon; Billy & O'Nare; Ogre Clan; Perrin e o Bloodmoon Ursaluna; Kitakami Dex.
6. The Indigo Disk (exige as 4 rotas e o Teal Mask): BB League → Kieran → Area Zero Underdepths → Terapagos.
7. Pós-Indigo: voo livre, Special Coaches, Cyrano, Snacksworth (lendários), Meloetta, Briar's Book no Crystal Pool,
   tutoria da Team Star, Perrin e os paradoxos, caverna de cristal (Garchomp Stellar), Mark Charm.
8. Epílogo Mochi Mayhem (Mythical Pecha Berry + 1º torneio + as duas DLCs) → Pecharunt; Carmine, Kieran, Nemona,
   Arven e Penny como Special Coaches.

## Dúvidas

- O mapa com ordem sugerida e níveis recomendados (página 10) é imagem; não foi lido (sem renderizador de PDF aqui).
- Os dois membros da Team Star em Mesagoza e os guardas das bases: o detonado não lista os times.
- O detonado escreve "Bombardier" e "Skovillain"; o certo no jogo é Bombirdier e Scovillain (corrigido acima).
- Também grafa "Kimgambit" (Giacomo), "Pelliper" (Kofu), "Amoongus" (Clavell), "Iron Valliant" (AI Turo); corrigidos.
- Fezandipiti: o povo diz "Oni Mountain", a seção da luta diz "Fellhorn Gorge" (atravessa uma caverna); a captura
  final é "Oni Mountain".
- Munkidori: o título da seção diz "Wisteria Pound" (provável Wisteria Pond, nos Wistful Fields).
- Níveis do DLC dependem do progresso: o detonado usa os níveis com as 4 rotas concluídas (~60+); sem isso, menores.
- Area Zero e Kitakami/Terarium não têm listas de espécies por área no texto.
- Dondozo/Tatsugiri: não diz se/como capturar o Titan depois.
- Nível do inicial do jogador e do 2º Gimmighoul (forma de baú) não aparecem.
- Ryme na revanche tem só 5 Pokémon listados (dupla).
- Tera Raid: quais espécies/níveis por estrela não constam; as raids de evento também não.
