# Legends: Z-A — fatos extraídos do detonado da Pokémon Mythology

Fonte: https://www.pokemythology.net/detonados/detonado-legends-z-a/ (PDF salvo em 05/11/2026, 136 páginas).
Só **fatos de jogo** (chefes, níveis, encontros, itens) no nosso formato. Nenhuma prosa foi copiada. Golpes dos
chefes **não** constam no detonado: deixar `moves: []` e `verified: false` para golpes; espécies e níveis são
`verified: true` (uma fonte) até o analista conferir numa segunda.

## Ordem da história e chefes (para `bosses/za.json`)

| Order | Id | Kind | Nome | Local | Time (espécie lv) | Cap |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | andi-1 | other | Andi | Lumiose (MM01) | Pancham 4 | 4 |
| 2 | urbain-1 | rival | Urbain/Taunie | Hotel Z (MM03) | os dois iniciais não escolhidos (Chikorita/Tepig/Totodile) 5 | 5 |
| 3 | zach-z | gym | Zach (Rank Z→Y) | Ponto de táxi, Vert Sector 7 | Slowpoke 8, Pidgey 9, Pikachu 9 | 9 |
| 4 | josee-1 | other | Josée (Fist of Justice) | Caminho do Looker Bureau | Machop 15, Meditite 15 | 15 |
| 5 | emma-1 | other | Emma | Looker Bureau | Mareep 15 | 15 |
| 6 | yvon-y | gym | Yvon (Rank Y→X) | Bleu Plaza | Spritzee 15, Swirlix 15, Vivillon 16 | 16 |
| 7 | naveen-1 | rival | Naveen | Hotel Z (MM07) | Spinarak 16, Sableye 16, Scraggy 17 | 17 |
| 8 | xavi-x | gym | Xavi (Rank X→W) | Escola, Rouge District | Venipede 20, Kadabra 21, Roselia 20, Furfrou 21 | 21 |
| 9 | lida-1 | rival | Lida | Vert Sector 5 (MM08) | Clauncher 22, Vanillite 22, Staryu 23 | 23 |
| 10 | rintaro-w | gym | Rintaro (Rank W→V) | Restaurant Le Nah, South Boulevard | Simisage 24, Simipour 24, Simisear 24 | 24 |
| 11 | rogue-absol | other | Rogue Mega Absol (só com Lucario emprestado) | Telhado (MM09) | Mega Absol 34 | — (não conta para cap) |
| 12 | urbain-2 | rival | Urbain/Taunie (MM10) | Hotel Z | Croconaw/Pignite/Bayleef 25 ×2 (os dois não escolhidos, evoluídos), Mega Manectric 26 | 26 |
| 13 | vinnie-v | gym | Vinnie (Rank V→F) | Quadra ao lado da Quasartico Inc. | Houndoom 30, Sharpedo 30, Buneary 30, Mega Drampa 32 | 32 |
| 14 | rogue-slowbro | other | Rogue Mega Slowbro | Aymlis Park (MM11) | 34 | — |
| 15 | rogue-camerupt | other | Rogue Mega Camerupt | Jaune Sector 3, telhado (MM12) | 34 | — |
| 16 | rogue-victreebel | other | Rogue Mega Victreebel | Magenta Sector 9 (MM13) | 34 | — |
| 17 | zygarde-1 | other | Zygarde 10% | Hotel Z | Zygarde 33 | — |
| 18 | naveen-2 | rival | Naveen (MM14) | Hotel Z | Ariados 33, Scraggy 33, Mega Sableye 33 | 33 |
| 19 | mani-quiz | other | Mani (Canari Quiz round 3) | Electrical Substation | Helioptile 34, Emolga 34, Raichu 35 | 35 |
| 20 | tarragon-1 | other | Tarragon | Racine Construction (garagem) | Hippowdon 36, Diggersby 36, Mega Excadrill 37 | 37 |
| 21 | canari-f | gym | Canari (Rank F→E) | Racine Construction | Heliolisk 37, Ampharos 38, Stunfisk 38, Mega Eelektross 39 | 39 |
| 22 | rogue-beedrill | other | Rogue Mega Beedrill | Vert Sector 5 (MM16) | 41 | — |
| 23 | rogue-hawlucha | other | Rogue Mega Hawlucha | Magenta Sector 3 (MM17) | 41 | — |
| 24 | rogue-banette | other | Rogue Mega Banette | Jaune Sector 9 (MM18) | 41 | — |
| 25 | josee-2 | other | Josée | Justice Dojo, Jaune Sector 2 | Machoke 43, Medicham 43 | 43 |
| 26 | canari-rematch | other | Canari (revanche, caso Gwynn) | Racine Construction | Heliolisk 42, Ampharos 43, Stunfisk 43, Mega Eelektross 44 | 44 |
| 27 | gwynn-1 | other | Gwynn | Esgotos (fonte do Clawitzer, Bleu Sector 7) | Banette 43, Gourgeist 44, Mega Chandelure 45 | 45 |
| 28 | ivor-e | gym | Ivor (Rank E→D) | Justice Dojo | Heracross 45, Machamp 46, Medicham 46, Mega Falinks 47 | 47 |
| 29 | philippe-1 | evil | Philippe (Rust Syndicate) | Prédio do Rust Syndicate, Bleu District | Steelix 46, Scizor 46, Mega Skarmory 47 | 47 |
| 30 | rogue-mawile | other | Rogue Mega Mawile | Magenta Sector 6 (MM21) | 48 | — |
| 31 | rogue-barbaracle | other | Rogue Mega Barbaracle | Esgotos, Bleu Sector 5 (MM22) | 48 | — |
| 32 | gwynn-2 | other | Gwynn (Holovator) | Rouge Sector 6 (MM23) | Banette 45, Gourgeist 46, Gengar 46, Mega Chandelure 47 | 47 |
| 33 | rogue-ampharos | other | Rogue Mega Ampharos | Rouge Sector 6 (MM23) | 41 (sic no detonado) | — |
| 34 | corbeau-d | gym | Corbeau (Rank D→C) | Rust Syndicate | Arbok 50, Gyarados 51, Roserade 51, Mega Scolipede 52 | 52 |
| 35 | lida-2 | rival | Lida (MM25) | Quadra, Vert Sector 4 | Clawitzer 46, Vanillish 46, Emolga 47, Staryu 48 | 48 |
| 36 | naveen-3 | rival | Naveen (MM25) | Quadra, Vert Sector 4 | Ariados 48, Sableye 48, Krookodile 49, Mega Scrafty 50 | 50 |
| 37 | asami-1 | other | Asami (Boutique Couture; vencer com mega) | Vernal Avenue | Florges 50 | 50 |
| 38 | rogue-froslass | other | Rogue Mega Froslass | Aymlis Park, telhado (MM27) | 53 | — |
| 39 | rogue-altaria | other | Rogue Mega Altaria | Magenta Sector 4 (MM28) | 53 | — |
| 40 | rogue-venusaur | other | Rogue Mega Venusaur | Jaune Sector 8, coreto (MM29) | 53 | — |
| 41 | lebanne-1 | other | Lebanne (Jacinthe Zone) | Hotel Z | Noivern 53, Tyrantrum 53, Garchomp 53, Mega Dragalge 54 | 54 |
| 42 | tournoi-canari | other | Canari (Super-Tournoi) | Hotel Richissime | Heliolisk 53, Ampharos 54, Stunfisk 54, Mega Eelektross 55 | 55 |
| 43 | tournoi-ivor | other | Ivor (Super-Tournoi) | Hotel Richissime | Heracross 53, Machamp 54, Medicham 54, Mega Falinks 55 | 55 |
| 44 | tournoi-corbeau | other | Corbeau (Super-Tournoi) | Hotel Richissime | Arbok 54, Gyarados 55, Roserade 54, Mega Scolipede 56 | 56 |
| 45 | zygarde-50 | other | Zygarde 50% | Hotel Richissime | Zygarde 57 | — |
| 46 | jacinthe-c | gym | Jacinthe (Rank C→B) | Hotel Richissime | Carbink 57, Aurorus 58, Mawile 58, Gardevoir 58, Mega Clefable 59 | 59 |
| 47 | rogue-dragonite | other | Rogue Mega Dragonite | Vert Sector 3 (MM32) | 59 | — |
| 48 | rogue-tyranitar | other | Rogue Mega Tyranitar | Jaune Sector 4 (MM33) | 59 | — |
| 49 | philippe-2 | evil | Philippe (MM34) | Bleu Sector 9 | Steelix 56, Scizor 56, Aggron 56, Mega Skarmory 57 | 57 |
| 50 | rogue-starmie | other | Rogue Mega Starmie | Bleu Sector 9 (MM34) | 59 | — |
| 51 | emma-2 | other | Emma (MM35) | Looker Bureau | Ampharos 57, Mawile 57, Lucario 57, Lopunny 57, Mega Malamar 58 | 58 |
| 52 | griselle-1 | evil | Griselle (Team Flare Nouveau) | Lysandre Café | Talonflame 59, Camerupt 59, Aerodactyl 60, Metagross 60, Mega Pyroar 61 | 61 |
| 53 | grisham-b | gym | Grisham (Rank B→A) | Nouveau Café, Centrico Plaza | Pangoro 61, Malamar 61, Pyroar 61, Tyranitar 62, Salamence 62, Mega Charizard X 63 | 63 |
| 54 | urbain-3 | rival | Urbain/Taunie (MM36, Prism Tower) | Centrico Plaza | Meowstic 62, Avalugg 62, Goodra 62, Manectric 63, inicial não escolhido A 63, Mega inicial não escolhido B 64 (por inicial: Chikorita→Feraligatr 63 + Mega Emboar 64; Tepig→Meganium 63 + Mega Feraligatr 64; Totodile→Emboar 63 + Mega Meganium 64) | 64 |
| 55 | rogue-gengar-heracross | other | Rogue Mega Gengar + Mega Heracross (dupla) | Centrico Plaza | 62, 62 | — |
| 56 | rogue-pidgeot-kangaskhan-garchomp | other | 3 Rogues (com Ivor/Gwynn) | MM37 | 62 ×3 | — |
| 57 | rogue-blastoise-steelix | other | 2 Rogues (com Canari) | MM37 | 62 ×2 | — |
| 58 | rogue-salamence-aggron | other | 2 Rogues (com Lebanne) | MM37 | 62 ×2 | — |
| 59 | rogue-gardevoir-gallade | other | 2 Rogues (com Corbeau) | MM37 | 62 ×2 | — |
| 60 | rogue-alakazam-metagross-aerodactyl | other | 3 Rogues (com Grisham/Griselle) | MM37 | 62 ×3 | — |
| 61 | hyperrogue-ange-floette | champion | Hyperrogue Ange Floette (final; Blue/Red Eternal Flower; Zygarde completo e Mega Zygarde) | Centrico Plaza | sem nível listado | — |
| 62 | zach-reward-1 | other | Zach (recompensa 1, pós-jogo) | Quasartico Inc. | Slowpoke 9, Pidgey 10, Pikachu 10 | — |
| 63 | l-final | other | L (Lysandre), treinador mais forte | Caminho do Hotel Z (MM42) | Pyroar 78, Florges 78, Sableye 78, Noivern 79, Garbodor 79, Mega Gyarados 80 | — |
| 64 | zygarde-complete | other | Zygarde (10%→50%→Completo), captura garantida | Wild Zone 20 (MM42) | Zygarde 84 | — |

Pós-jogo, partidas de recompensa 2–20 (Infinite Z-A Royale, Quasartico Inc.), ordem fixa:
2 Griselle (Talonflame 69, Camerupt 69, Aerodactyl 70, Metagross 70, Mega Pyroar 71);
3 Lida (Clawitzer 69, Vanillish 69, Emolga 69, Vaporeon 69, Mega Starmie 70);
4 Tarragon (Hippowdon 69, Stunfisk 69, Diggersby 69, Krookodile 69, Mega Excadrill 71);
5 Xavi (Scolipede 68, Slowking 68, Alakazam 68, Roserade 68, Furfrou 69);
6 Gwynn (Banette 69, Gourgeist 69, Gengar 69, Froslass 69, Mega Chandelure 70);
7 Lebanne (Noivern 69, Tyrantrum 69, Altaria 69, Garchomp 69, Mega Dragalge 70);
8 Naveen (Ariados 69, Sableye 69, Krookodile 69, Aegislash 69 [imagem mostra Aegislash, texto diz Krookodile], Mega Scrafty 70);
9 Yvon (Aromatisse 68, Slurpuff 68, Slowbro 68, Trevenant 68, Vivillon 69);
10 Rintaro (Simisage 69, Simipour 69, Simisear 69, Espeon 69, Umbreon 69 [imagens; texto repete "Simisear"]);
11 Canari (Heliolisk 69, Ampharos 70, Stunfisk 70, Manectric 70, Dedenne 70, Mega Eelektross 71);
12 Mani (Heliolisk 68, Emolga 68, Jolteon 68, Alolan Raichu 68, Raichu 69);
13 Emma (Ampharos 70, Mawile 70, Abomasnow 70, Lucario 70, Lopunny 70, Mega Malamar 71);
14 Corbeau (Arbok 69, Barbaracle 70, Garbodor 70, Gyarados 70, Roserade 70, Mega Scolipede 71);
15 Urbain/Taunie (Meowstic 70, Goodra 70, Manectric 71, inicial A 71, Mega inicial B 71, Mega Floette 72);
16 Ivor (Heracross 69, Machamp 70, Medicham 70, Hawlucha 70, Gallade 70, Mega Falinks 71);
17 Philippe (Steelix 69, Scizor 69, Aggron 69, Klefki 69, Mega Skarmory 70);
18 Jacinthe (Carbink 69, Aurorus 70, Mawile 70, Gardevoir 70, Sylveon 70, Mega Clefable 71);
19 Grisham (Pangoro 70, Malamar 70, Pyroar 70, Tyranitar 71, Salamence 71, Mega Charizard X 72);
20 Vinnie (Houndoom 69, Sharpedo 69, Glalie 69, Lopunny 69, Mega Drampa 71).
21+: aleatório entre os 20 (+4 níveis; Zach +1; Urbain com Avalugg no lugar da Floette) e mais Josée (Machamp, Scrafty, Hawlucha, Pangoro, Medicham 73), Asami (Florges ×5, 73) e Andi (Carbink, Gogoat, Klefki, Audino, Pancham 73).
Side Mission 119: Jacinthe ×4 (Carbink 73, Aurorus 74, Mawile 74, Gardevoir 74, Sylveon 74, Mega Clefable 75).

Ticket Points por promoção: Z→Y 1.000; Y→X 3.000; X→W 4.000; W→V 5.000; V→F 6.000; F→E 32.000; E→D 35.000; D→C 38.000; C→B 41.000; B→A 45.000; pós-jogo 50.000.

## Áreas (para `areas/za.json`, kind e ordem de liberação)

Cidade (Lumiose City) dividida em distritos/setores: Vert Sectors 1–9, Bleu Sectors 1–10, Magenta Sectors 1–9, Rouge Sectors 1–8, Jaune Sectors 1–12; avenidas Vernal, Autumnal, Hibernal, Estival; North/South Boulevard; Saison Canal; Centrico Plaza (Prism Tower → Wild Zone 20). Lugares: Gare de Lumiose, Hotel Z, Vert/Bleu/Magenta/Rouge/Hibernal/Vernal/Jaune Pokémon Centers, Pokémon Research Lab, Looker Bureau, Racine Construction, Quasartico Inc. (Magenta District), Nouveau Café, Stone Emporium (Vernal Street), Poké Ball Boutique (Autumnal Avenue, Rouge Sector 3), Justice Dojo (Jaune Sector 2), Rust Syndicate (Bleu District), Hotel Richissime (North Boulevard), Lysandre Café/Labs, Electrical Substation (Magenta District), Lumiose Museum, Académie Étoile, Aymlis Park (Bleu District), esgotos (Sewers).

Wild Zones, com nível recomendado e ordem de liberação:

| WZ | Local | Nível | Liberada em |
| --- | --- | --- | --- |
| 1 | perto do Vert PC | — | MM03 |
| 2 | lago com ponte (Vert Sector 4) | — | MM05 |
| 3 | Rouge Plaza (praça com cercas) | — | MM05 |
| 4 | Dormez Bien Cemetery, Rouge District | 9 | MM05 |
| 5 | Bleu Sectors 1–2 / Bleu Street | 13 | MM05 |
| 6 | Jaune Sector 12 / North Boulevard | 15 | MM05 |
| 7 | Magenta Plaza (Magenta Sectors 3–4) | 18 | MM10 |
| 8 | Jaune Plaza | 21 | MM10 |
| 9 | Magenta Sector 5 | 25 | MM10 |
| 10 | Bleu Sector 10 (hidrovias) | 29 | MM10 |
| 11 | Saison Canal (Jaune Sector 10 / Vert Sector 6) | 32 | MM15 |
| 12 | Aymlis Park, Bleu District (neve) | 34 | MM15 |
| 13 | North Boulevard, norte do Rouge Sector 7 | 36 | MM15 |
| 14 | Electrical Substation, Magenta District | 38 | MM20 |
| 15 | North Boulevard, perto do Jaune Sector 7 | 40 | MM20 |
| 16 | Bleu Plaza | 42 | MM24 (pós-Corbeau) |
| 17 | Vert Sector 7 | 44 | MM24 |
| 18 | Magenta Sector 9 / North Boulevard | 46 | MM31 |
| 19 | Jaune Sector 8 | 48 | MM31 |
| 20 | Ruínas da Prism Tower, Centrico Plaza | 55 | pós-jogo (MM39) |

## Encontros (para `encounters/za.json`; method `walk` salvo indicação; `conditions`: day/night)

- WZ1: Fletchling, Bunnelby, Scatterbug, Weedle, Mareep; Pichu (fujão, saída norte); Alpha Pidgey. Shiny Mareep fixo (SM017, lv 11).
- WZ2: Budew, Magikarp, Binacle, Kakuna, Patrat, Staryu (noite); Alpha Magikarp (dia), Alpha Staryu (noite).
- WZ3: Skiddo, Flabébé (vermelha), Espurr, Litleo, Pancham, Pikachu; Alpha Fletchling, Alpha Litleo. TM022 Aerial Ace no topo da torre.
- WZ4: Gastly, Spinarak, Ekans (dia), Spewpa (dia), Patrat (noite), Honedge (noite); Alpha Spinarak. Sableye fixo (SM003).
- WZ5: Bellsprout, Venipede, Electrike, Abra, Pidgey, Pidgeotto, Bunnelby; Alpha Bellsprout, Alpha Whirlipede.
- WZ6: Binacle, Meditite (dia), Buneary, Magikarp, Houndour, Swablu, Flaaffy; Alpha Pikachu, Alpha Houndoom, Alpha Binacle.
- WZ7: Hippopotas (dia), Audino, Vanillite, Kakuna, Floette (vermelha, laranja), Roselia, Shuppet (noite); Alpha Fletchinder.
- WZ8: Krokorok, Sandile, Gible, Drilbur, Machop, Numel; Alpha Krokorok, Alpha Camerupt.
- WZ9: Carbink, Espurr, Fletchinder, Kadabra, Sableye, Mawile; Alpha Meowstic (M/F), Alpha Manectric.
- WZ10: Slowpoke, Arbok, Watchog, Bellsprout, Carvanha, Staryu, Tynamo; Alpha Sharpedo, Alpha Watchog, Alpha Arbok.
- WZ11: Gyarados, Clauncher, Furfrou, Inkay, Slowpoke, Stunfisk; Alpha Slowbro, Alpha Clawitzer. Galarian Stunfisk fixo (SM072, lv 38).
- WZ12: Delibird, Machop, Snover, Bergmite, Vanillite, Gogoat, Snorunt, Machoke; Alpha Abomasnow, Alpha Avalugg. TM Ice Beam aqui.
- WZ13: Heracross, Pinsir, Vivillon (dia), Phantump (noite), Weepinbell (dia), Scyther (noite); Alpha Weepinbell (dia), Alpha Trevenant (noite).
- WZ14: Helioptile (dia), Drilbur, Onix, Aron, Lairon, Excadrill, Emolga; Alpha Excadrill.
- WZ15: Larvitar, Beedrill (dia), Haunter (noite), Shuppet (noite), Whirlipede (dia), Scolipede (dia), Pumpkaboo (small/medium dia; large/jumbo noite); Alpha Beedrill (dia), Alpha Banette (noite), Alpha Gourgeist (medium).
- WZ16: Froakie, Falinks, Flaaffy, Medicham (dia), Florges (dia; vermelha/azul/branca), Starmie (noite), Barbaracle (noite); Alpha Ampharos.
- WZ17: Chespin, Skarmory, Pyroar (M/F), Talonflame, Diggersby, Klefki (dia), Lampent (noite); Alpha Pyroar (M), Alpha Mawile.
- WZ18: Fennekin, Bagon, Swablu (dia), Altaria (dia), Noibat (noite), Noivern (noite); Alpha Salamence, Alpha Lopunny.
- WZ19: Eevee, Furfrou, Audino, Kangaskhan, Drampa (dia), Cleffa (noite), Clefairy (noite); Alpha Furfrou (dia), Alpha Clefairy (noite).
- WZ20: Bulbasaur, Charmander, Squirtle, Chikorita, Tepig, Totodile, Aggron, Scrafty, Garbodor, Roserade, Gardevoir, Hippowdon, Lucario, Malamar, Dragalge; Alphas: Raichu, Clefable, Alakazam, Machamp, Victreebel, Gengar, Kangaskhan, Starmie, Pinsir, Gyarados, Vaporeon, Jolteon, Flareon, Dragonite, Ariados, Heracross, Delibird, Skarmory, Tyranitar, Gardevoir, Sableye, Aggron, Medicham, Altaria, Absol, Metagross, Roserade, Garchomp, Lucario, Hippowdon, Leafeon, Glaceon, Gallade, Froslass, Simisage, Simisear, Simipour, Scolipede, Krookodile, Scrafty, Garbodor, Vanilluxe, Eelektross, Chandelure, Stunfisk, Diggersby, Talonflame, Vivillon (Meadow), Florges (Red), Gogoat, Pangoro, Furfrou, Aegislash, Malamar, Barbaracle, Dragalge, Heliolisk, Sylveon, Hawlucha, Dedenne, Carbink, Goodra, Klefki, Gourgeist (Medium), Noivern, Drampa, Falinks.
- Esgotos (Sewers): Noibat, Scraggy, Inkay, Klefki, Patrat, Litwick, Goomy, Gastly, Haunter, Skrelp, Dragalge. Klefki fixo lv 40 (missão Gwynn).
- Lysandre Labs: Ekans, Arbok, Houndour, Houndoom, Electrike, Manectric, Beldum, Metang, Honedge, Doublade, Noibat, Noivern; Alpha Aegislash fixo.
- Fora das Wild Zones (cidade, `method: city`): Pidgey, Fletchling (postes), Scatterbug, Weedle (canteiros), Kakuna, Spinarak (árvores), Flabébé (canteiros/telhados), Pansage/Pansear/Panpour (árvores de avenidas; Jaune Sector 4), Trubbish (lixeiras), Budew, Patrat, Bunnelby, Buneary, Bellsprout, Espurr, Ekans. Goomy só com chuva.
- Raros por setor (telhados/interiores): Dedenne (Vert 3, Vert 8, Bleu 4, Bleu 6, Magenta 2/4/6/7, Rouge 6/8, Jaune 4/11); Emolga (Vert 7/8, Magenta 7/8, Rouge 1); Abra (Vert 8, WZ5, Magenta 6/7, Rouge 6/8, Jaune 5); Cleffa (Vert 3, Bleu 4, Magenta 1, Rouge 5, Jaune 11); Swablu (Vert 3/9, Bleu 7/8, Magenta 6/8, Rouge 1/8, Jaune 5, WZ6); Ralts (Vert 4, Bleu 7, Magenta 7, Rouge 2); Spritzee (South Boulevard, Bleu 5, Magenta 1/6, Rouge 1/2/4/6/7); Swirlix (South Boulevard, Bleu 5/6, Magenta 1/3/4/6, Rouge 1/2/6); Scraggy (Bleu 8, Rouge 3); Eevee (Vert 8/9, Bleu 7, Magenta 7/9, Rouge 8, Jaune 7); Riolu (Vert 8, Bleu 5, Magenta 9, Rouge 4); Mawile (Vert 9, Bleu 8, Magenta 9, Rouge 3, Jaune 2); Hawlucha (Vert 1/7/9, Bleu 3/4/6, Magenta 1/9, Rouge 1/3, Jaune 3/7); Dratini (Vert 6, Bleu 9, Magenta 8, Rouge 8, Jaune 3/5); Larvitar (Vert 2, Bleu 7, Magenta 2, Rouge 7, Jaune 6, WZ15).
- Lendários (`method: fixed`): Xerneas lv 75 (WZ11, MM40); Yveltal lv 75 (Galerie de la Lune, Rouge Sector 2, MM41); Zygarde lv 84 (WZ20, captura garantida, MM42); Diancie lv 70 (telhados, precisa da Diancite por Mystery Gift, SM EX1); Mewtwo lv 70 (Lysandre Labs B3F, precisa das Mewtwonites X e Y por Mystery Gift, SM EX2).

## Presentes, trocas in-game e fósseis (`method: gift` / `trade`)

- Inicial: Chikorita/Tepig/Totodile lv 5 (MM01). Iniciais de Kanto: Bulbasaur/Charmander/Squirtle lv 5 (SM022, Mable). Iniciais de Kalos: Chespin lv 14 (SM007), Fennekin lv 14 (SM008), Froakie lv 14 (SM009).
- Absol (após Rogue Mega Absol, MM09); Lucario (herança do AZ, MM38); Floette Eternal Flower + Floettite (Urbain/Taunie, fim da MM39).
- Trocas: Pikachu → Heracross lv 12 (SM002); Abra → Riolu lv 25 (SM024); Slowpoke → Galarian Slowpoke lv 30 (SM039); Raichu → Alolan Raichu lv 64 (SM108). Presentes: Spewpa lv 9 (SM021, evolui para Vivillon Marine), Galarian Stunfisk lv 38 (SM072).
- Fósseis (Stone Emporium, Reg no Lab 2F, SM027): Jaw Fossil → Tyrunt, Sail Fossil → Amaura, Old Amber → Aerodactyl; revividos lv 20 (lv 35 se Alpha).

## Mega Stones

- Rogue Mega Evolutions dão a Mega Stone correspondente: Absol, Slowbro, Camerupt, Victreebel, Beedrill, Hawlucha, Banette, Mawile, Barbaracle, Ampharos, Froslass, Altaria, Venusaur, Dragonite, Tyranitar, Starmie.
- Quasartico Inc. (Mega Shards): inicial: Pidgeot, Manectric, Houndoom, Audino, Lopunny, Sableye, Sharpedo, Gyarados, Lucario, Heracross, Aerodactyl, Alakazam, Drampa; MM15: Eelektross, Glalie, Pinsir, Excadrill; MM20: Falinks, Chandelure, Aggron; MM24: Scolipede, Gardevoir, Gallade, Skarmory; MM31: Clefable, Dragalge, Scrafty, Salamence, Metagross.
- Stone Emporium (compra): Gengar, Medicham, Abomasnow, Scizor, Garchomp, Steelix, Kangaskhan, Charizard, Blastoise; pós-jogo: Meganium, Emboar, Feraligatr.
- AZ dá a do inicial escolhido (MM10). Zygardite = Pebble (final). Diancite e Mewtwonite X/Y por Mystery Gift.

## TMs e itens citados (localização)

TM004 Rock Smash (Research Level 2); TM022 Aerial Ace (WZ3); TM057 Will-O-Wisp (SM026); TM085 Substitute (SM029); TM088 Spikes (SM041); TM031 Reflect (SM034); TM043 Fly (SM046); TM094 Whirlwind (SM060); TM045 Knock Off (SM094); TM047 Agility (SM088); TM048 Self-Destruct (SM096); TM099 Metronome (SM079); TM053 Sludge Bomb (Research Level 32); Ice Beam (WZ12). TMs não se consomem. Seed of Mastery habilita Plus Move (Josée, SM074, SM098, SM117).

## Mecânicas (para o guia de mecânicas, texto nosso)

- Sem batalhas por turno: tempo real, cooldown por golpe; Plus Moves (+); mega evolução pelo analógico direito com Mega Power coletado em orbes.
- Ciclo dia/noite: 48 min reais (dia 32, noite 16); bancos avançam o tempo e mudam o clima (claro, nublado, neblina, chuva, arco-íris).
- Battle Zones à noite: Ticket Points e Prize Medals; Bonus Cards (até 3).
- Shiny: não some até ser enfrentado; aparições mudam entre dia e noite. Alphas: olhos vermelhos, golpes extras, mais difíceis; tamanhos XS–XL.
- Canari Plush (amuletos): Red (EXP), Gold (dinheiro), Pink (Mega Shards), Green (resistência), Blue (captura); lv1 3 screws, lv2 +5, lv3 +8 (16 total).
- Evolução manual; Phantump/Trevenant por troca; Furfrou trims liberados por Side Missions 047/062/087.
- Avaliação de IVs liberada no pós-jogo (MM38). Lumi Cab $710.
