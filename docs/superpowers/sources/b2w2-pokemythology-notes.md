# Black 2 / White 2 — fatos extraídos do detonado da Pokémon Mythology

Fonte: https://www.pokemythology.net/detonados/detonado-black-2-white-2/ (PDF salvo em 05/11/2026, 135 páginas).
Só **fatos de jogo** (chefes, níveis, encontros, itens) no nosso formato. Nenhuma prosa foi copiada. Golpes dos
chefes **não** constam no detonado: deixar `moves: []` e `verified: false` para golpes; espécies e níveis são
`verified: true` (uma fonte) até o analista conferir numa segunda. Exclusivos marcados `[B2]` / `[W2]`.
`CM` = Challenge Mode (key liberada no Black 2 após a Elite Four; o detonado lista os times dos dois modos).

Regra de iniciais (do detonado): Hugh tem o inicial que **ganha** do seu e o macaco elemental que **perde** para o
seu inicial. Derivado da regra: Snivy → Hugh Tepig/Pignite/Emboar + Panpour/Simipour; Tepig → Oshawott/Dewott/Samurott +
Pansage/Simisage; Oshawott → Snivy/Servine/Serperior + Pansear/Simisear. Nate/Rosa (parceiro em Nimbasa) usa o
inverso. Nomes dos estágios não são escritos no detonado.

## Ordem da história e chefes (para `bosses/b2w2.json`)

| Order | Id | Kind | Nome | Local | Time (espécie lv) | Cap |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | hugh-1 | rival | Hugh | Aspertia City | inicial (vantagem) 5 | 5 |
| 2 | hugh-2 | rival | Hugh | Floccesy Ranch | inicial (vantagem) 8 | 8 |
| 3 | alder-kids | other | duas crianças na casa do Alder | Floccesy Town | Pansage/Pansear/Panpour (um forte e um fraco ao seu inicial; sem nível listado) | — |
| 4 | cheren | gym | Cheren | Aspertia City | Patrat 11, Lillipup 13 (CM: Patrat 12, Pidove 12, Lillipup 14) | 13 |
| 5 | roxie | gym | Roxie | Virbank City | Koffing 16, Whirlipede 18 (CM: Koffing 17, Grimer 17, Whirlipede 19) | 18 |
| 6 | plasma-virbank | evil | 3 Team Plasma Grunts (com Roxie e Hugh) e Grunt na saída oeste | Virbank City | sem time listado | — |
| 7 | burgh | gym | Burgh | Castelia City | Swadloon 22, Dwebble 22, Leavanny 24 (CM: Dwebble 24, Shelmet 24, Karrablast 24, Leavanny 26) | 24 |
| 8 | colress-1 | evil | Colress | Desert (Route 4*) | Magnemite 21, Klink 23 (CM: 23, 25) | 23 |
| 9 | ingo-emmet-1 | other | Ingo + Emmet (dupla, com Nate/Rosa) | entrada do metrô, Nimbasa City | Gurdurr 26, Boldore 26 (CM 28) | 26 |
| 10 | elesa | gym | Elesa | Nimbasa City | Emolga 28, Flaaffy 28, Zebstrika 30 (CM: Emolga 30, Flaaffy 30, Joltik 30, Zebstrika 32) | 30 |
| 11 | plasma-nimbasa | evil | 2 Grunts seguidos (com Hugh) | norte de Nimbasa City | sem time listado | — |
| 12 | charles-1 | other | Charles (motoqueiro; rotacional [B2] / trio [W2]; precisa da 4ª insígnia) | Route 5 | sem time listado | — |
| 13 | charles-2 | other | Charles (trio [B2] / rotacional [W2]) | Driftveil Market | sem time listado | — |
| 14 | rood | other | Rood (dá o Zorua depois) | norte de Driftveil City | Herdier 27, Swoobat 27 | 27 |
| 15 | clay | gym | Clay | Driftveil City | Krokorok 31, Sandslash 31, Excadrill 33 (CM: Krokorok 34, Sandslash 34, Onix 34, Excadrill 36) | 33 |
| 16 | pwt-hugh | rival | Hugh (PWT, 1ª rodada) | Pokémon World Tournament | inicial evoluído (vantagem) 25, macaco evoluído (fraco) 25, Tranquill 25 | 25 |
| 17 | pwt-cheren | other | Cheren (PWT, semifinal) | Pokémon World Tournament | Stoutland 25, Watchog 25, Cinccino 25 | 25 |
| 18 | pwt-colress | evil | Colress (PWT, final) | Pokémon World Tournament | Magneton 25, Elgyem 25, Klink 25 | 25 |
| 19 | plasma-frigate-1 | evil | Grunts (2 sozinho + duplas com Cheren e com Hugh) | Plasma Frigate, Driftveil | sem time listado | — |
| 20 | volcarona-relic | other | Volcarona | catacumba do Relic Castle (via Relic Passage) | Volcarona 35 (lv 65 após a Elite Four se derrotado) | — |
| 21 | skyla | gym | Skyla | Mistralton City | Swoobat 37, Skarmory 37, Swanna 39 (CM: Swoobat 40, Skarmory 40, Sigilyph 40, Swanna 42) | 39 |
| 22 | hugh-3 | rival | Hugh | norte de Undella Town | Unfezant 39, macaco evoluído (fraco) 39, inicial final (vantagem) 41 (CM: 43, 43, 45) | 41 |
| 23 | cobalion | other | Cobalion | fim da Route 13* | Cobalion 45 (lv 65 após a Elite Four se derrotado) | — |
| 24 | zinzolin-1 | evil | Zinzolin + Grunt (dupla, com Hugh) | Lacunosa Town | Zinzolin: Cryogonal 42, Sneasel 44 (CM 46, 48); Grunt: Golbat 39, Garbodor 39 (CM 43) | 44 |
| 25 | virizion | other | Virizion | Route 11* (entre Village Bridge e Opelucid) | Virizion 45 (lv 65 após a Elite Four se derrotado) | — |
| 26 | drayden | gym | Drayden | Opelucid City | Druddigon 46, Flygon 46, Haxorus 48 (CM: Druddigon 50, Flygon 50, Altaria 50, Haxorus 52) | 48 |
| 27 | zinzolin-2 | evil | Zinzolin | frente do ginásio, Opelucid congelada | Cryogonal 46, Cryogonal 46, Weavile 48 (CM 50, 50, 52) | 48 |
| 28 | shadow-1 | evil | Shadow (Shadow Triad) | leste de Opelucid City | Pawniard 46, Pawniard 46, Absol 48 (CM 50, 50, 52) | 48 |
| 29 | marlon | gym | Marlon | Humilau City | Carracosta 49, Wailord 49, Jellicent 51 (CM: Wailord 53, Carracosta 53, Mantine 53, Jellicent 55) | 51 |
| 30 | terrakion | other | Terrakion (Colress dá o Colress MCHN antes) | Route 22* | Terrakion 45 (lv 65 após a Elite Four se derrotado) | — |
| 31 | crustle-seaside | other | Crustle (Colress MCHN) | Seaside Cave | Crustle 42 | — |
| 32 | zinzolin-3 | evil | Zinzolin + Grunt (dupla, com Hugh) | Plasma Frigate (praia da Seaside Cave) | Zinzolin: Cryogonal 48, Cryogonal 48, Weavile 50 (CM 52, 52, 54); Grunt: Liepard 45, Watchog 45, Scolipede 45 (CM 49) | 50 |
| 33 | plasma-chasm | evil | Grunt (com Hugh) e Grunts do navio | Giant Chasm / Plasma Frigate | sem time listado | — |
| 34 | zinzolin-4 | evil | Zinzolin | Plasma Frigate, Giant Chasm (sala do Kyurem) | Cryogonal 49, Cryogonal 49, Weavile 51 (CM 53, 53, 55) | 51 |
| 35 | colress-2 | evil | Colress | Plasma Frigate, Giant Chasm | Magneton 50, Magnezone 50, Metang 50, Beheeyem 50, Klinklang 52 (CM +4: 54/56) | 52 |
| 36 | shadow-triad-1 | evil | Shadow Triad ×3 (seguidos) | Plasma Frigate, Giant Chasm | A: Pawniard 49, Pawniard 49, Absol 51; B: Pawniard 49, Pawniard 49, Banette 51; C: Pawniard 46 (sic), Pawniard 46 (sic), Accelgor 51 (CM: Pawniard 53, último 55) | 51 |
| 37 | kyurem-fused | other | Black Kyurem [B2] / White Kyurem [W2] (não capturável) | caverna do Giant Chasm | Kyurem fundido 55 | 55 |
| 38 | ghetsis | evil | Ghetsis | caverna do Giant Chasm | Cofagrigus 50, Drapion 50, Toxicroak 50, Eelektross 50, Seismitoad 50, Hydreigon 52 (CM 54, último 56) | 52 |
| 39 | hugh-4 | rival | Hugh (dá TM24 depois) | Victory Road (perto do topo) | Unfezant 55, Bouffalant 55, macaco evoluído (fraco) 55, inicial final (vantagem) 57 (CM 59, último 61) | 57 |
| 40 | shauntal | elite | Shauntal | Pokémon League | Cofagrigus 56, Drifblim 56, Golurk 56, Chandelure 58 (CM: Cofagrigus 60, Drifblim 60, Golurk 60, Banette 60, Chandelure 62) | 58 |
| 41 | grimsley | elite | Grimsley | Pokémon League | Liepard 56, Scrafty 56, Krookodile 56, Bisharp 58 (CM: Liepard 60, Scrafty 60, Krookodile 60, Absol 60, Bisharp 62) | 58 |
| 42 | caitlin | elite | Caitlin | Pokémon League | Musharna 56, Reuniclus 56, Sigilyph 56, Gothitelle 58 (CM: Musharna 60, Reuniclus 60, Sigilyph 60, Gothitelle 60, Metagross 62) | 58 |
| 43 | marshal | elite | Marshal | Pokémon League | Throh 56, Sawk 56, Mienshao 56, Conkeldurr 58 (CM: Throh 60, Sawk 60, Mienshao 60, Lucario 60, Conkeldurr 62) | 58 |
| 44 | iris | champion | Iris | Pokémon League | Hydreigon 57, Druddigon 57, Archeops 57, Aggron 57, Lapras 57, Haxorus 59 (CM 61, último 63) | 59 |
| 45 | n-castle | other | N (pós-jogo; dá a Light Stone [W2] / Dark Stone [B2]) | ruínas do N's Castle (via Victory Road, Zoroark) | Zekrom [B2] / Reshiram [W2] 70 (CM 75) | 70 |
| 46 | shadow-triad-icirrus | evil | Shadow Triad ×3 (single, trio, rotacional; 1×/estação) | ponte leste do PC, Icirrus City | A: Bisharp 65, Bisharp 65, Absol 67; B: Bisharp 65, Bisharp 65, Accelgor 67; C: Bisharp 65, Bisharp 65, Banette 67 (CM 70, último 72) | 67 |
| 47 | reshiram-zekrom | other | Reshiram [W2] / Zekrom [B2] (precisa da Stone) | topo da Dragonspiral Tower | 70 | — |
| 48 | kyurem | other | Kyurem | Giant Chasm (local do Ghetsis) | 70 | — |
| 49 | striaton-trio | other | Cilan / Chili / Cress (dupla diária: um é parceiro, dois são rivais) | antigo ginásio, Striaton City | Cilan: Maractus 65, Ferrothorn 65, Simisage 67; Chili: Heatmor 65, Darmanitan 65, Simisear 67; Cress: Basculin 65, Slowking 65, Simipour 67 (CM 70, último 72) | 67 |
| 50 | alder | other | Alder | casa do Alder, Floccesy Town | Accelgor 60, Escavalier 60, Bouffalant 60, Conkeldurr 60, Braviary 60, Volcarona 62 (CM 61, último 63) | 62 |
| 51 | cheren-memory | other | Cheren (só com Memory Link de B/W) | Route 5 | Liepard 65, Unfezant 65, Haxorus 65, Gigalith 65 + por inicial do B/W: Snivy→Simisage 65, Emboar 67; Tepig→Simisear 65, Samurott 67; Oshawott→Simipour 65, Serperior 67 (CM 67, inicial 69) | 67 |
| 52 | bianca-memory | other | Bianca (só com Memory Link de B/W) | norte de Nuvema Town | Stoutland 63, Chandelure 63, Musharna 63, Mienshao 63 + por inicial do B/W: Snivy→Simisear 63, Samurott 65; Tepig→Simipour 63, Serperior 65; Oshawott→Simisage 63, Emboar 65 (CM 68, inicial 70) | 65 |
| 53 | hugh-5 | rival | Hugh (dá HM06 Dive) | Undella Town | Unfezant 62, Bouffalant 62, Flygon 62, Eelektross 62, macaco evoluído (fraco) 62, inicial final (vantagem) 64 (CM 66, inicial 68) | 64 |
| 54 | hugh-daily | rival | Hugh (diário) | casa ao norte de Driftveil City | mesmo time do hugh-5 a 65, inicial 67 (CM 68, inicial 70) | 67 |
| 55 | colress-3 | evil | Colress (dá Master Ball; diário depois) | Plasma Frigate no P2 Laboratory | Magneton 72, Rotom 72, Magnezone 72, Metagross 72, Beheeyem 72, Klinklang 74 (CM 76, último 78) | 74 |
| 56 | n-seasonal | other | N (1×/estação; após Kyurem e passeio de sexta na roda-gigante) | N's Castle | Primavera (chuva): Politoed 77, Lanturn, Tentacruel, Omastar, Kabutops, Starmie 75; Verão (sol): Ninetales 77, Arcanine, Rapidash, Shiftry, Ninjask, Leafeon 75; Outono (areia): Hippowdon 77, Gastrodon, Cradily, Armaldo, Scizor, Rhyperior 75; Inverno (granizo): Abomasnow 77, Vanilluxe, Cloyster, Mamoswine, Froslass, Glaceon 75 (CM 82/80) | 77 |
| 57 | cynthia | other | Cynthia (diária na primavera e no verão) | casa à esquerda do PC, Undella Town | Spiritomb 76, Milotic 76, Togekiss 76, Lucario 76, Glaceon 76, Garchomp 78 (CM 80, último 82) | 78 |
| 58 | morimoto | other | Morimoto (diário) | Game Freak HQ, Castelia City | Liepard 76, Simisage 76, Simisear 76, Simipour 76, Swoobat 76, Zebstrika 78 (CM 78, último 80) | 78 |
| 59 | nishino | other | Nishino (diário) | Game Freak HQ, Castelia City | Clefable 76, Azumarill 76, Lickilicky 76, Wigglytuff 76, Alomomola 76, Snorlax 78 (CM 78, último 80) | 78 |
| 60 | shauntal-2 | elite | Shauntal (revanche) | Pokémon League | Cofagrigus 72, Mismagius 72, Froslass 72, Drifblim 72, Golurk 72, Chandelure 74 (CM: Cofagrigus 77, Gengar 77, Froslass 77, Drifblim 77, Golurk 77, Chandelure 79) | 74 |
| 61 | grimsley-2 | elite | Grimsley (revanche) | Pokémon League | Liepard 72, Honchkrow 72, Scrafty 72, Houndoom 72, Krookodile 72, Bisharp 74 (CM: Honchkrow 77, Scrafty 77, Krookodile 77, Houndoom 77, Tyranitar 77, Bisharp 79) | 74 |
| 62 | caitlin-2 | elite | Caitlin (revanche) | Pokémon League | Musharna 72, Sigilyph 72, Gallade 72, Reuniclus 72, Gothitelle 72, Metagross 74 (CM: Musharna 77, Alakazam 77, Gallade 77, Reuniclus 77, Gothitelle 77, Metagross 79) | 74 |
| 63 | marshal-2 | elite | Marshal (revanche) | Pokémon League | Throh 72, Sawk 72, Medicham 72, Lucario 72, Mienshao 72, Conkeldurr 74 (CM: Throh 77, Sawk 77, Machamp 77, Lucario 77, Mienshao 77, Conkeldurr 79) | 74 |
| 64 | iris-2 | champion | Iris (revanche) | Pokémon League | Hydreigon 76, Druddigon 76, Archeops 76, Aggron 76, Lapras 76, Haxorus 78 (CM: Hydreigon 81, Salamence 81, Archeops 81, Aggron 81, Lapras 81, Haxorus 83) | 78 |
| 65 | benga | other | Benga (Boss da Área 10; libera após Alder) | Black Tower [B2] / White Treehollow [W2] | [B2] Latios 80, Garchomp 80, Volcarona 80; [W2] Latias 80, Dragonite 80, Volcarona 80 (CM 85) | 80 |

Parceiros (lutam do seu lado; não contam para cap): Hugh nos Castelia Sewers (inicial evoluído 20, Pidove 18);
Nate/Rosa em Nimbasa (inicial evoluído fraco ao seu 25, macaco com vantagem 18); Cheren no Plasma Frigate (Stoutland 33,
Watchog 31, Cinccino 31); Hugh no Plasma Frigate (inicial evoluído 33, macaco evoluído 31, Tranquill 31); Bianca na
Reversal Mountain (Musharna 40, Mienfoo 38, Stoutland 38); Hugh em Lacunosa (inicial final 43, macaco 41, Unfezant 41);
Hugh no Plasma Frigate 2 (inicial final 50, macaco 48, Unfezant 48); Cheren na Pinwheel Forest (Stoutland 67, Watchog 65,
Cinccino 65).

Black Tower [B2] / White Treehollow [W2] (pós-jogo, após vencer o Alder; 10 áreas; Boss Trainer sempre com o mesmo time;
CM +5):
1. Abed [B2]: Unfezant 53 + Pawniard/Golett 53 (ver Dúvidas); Abigail [W2]: Watchog 53 + Boldore/Whirlipede 53.
2. Bess [B2]: Watchog, Swoobat, Basculin 56; Brent [W2]: Unfezant, Eelektrik, Audino 56.
3. Carlen [B2]: Bouffalant, Swanna, Scolipede 59; Carleigh [W2]: Stoutland, Emolga, Leavanny 59.
4. Danelle [B2]: Gigalith, Seviper 62 (+ Durant ou Heatmor); Daneil [W2]: Scrafty, Zangoose 62 (+ Durant ou Heatmor).
5. Ellas [B2]: Bronzong, Kabutops, Golurk 65; Elie [W2]: Claydol, Omastar, Jellicent 65. Dá a Tower Key [B2] / Treehollow Key [W2].
6. Freira [B2]: Porygon-Z, Breloom, Armaldo 68; Frederick [W2]: Togekiss, Heracross, Rampardos 68.
7. Gaius [B2]: Gyarados, Dusknoir, Weavile 71; Gail [W2]: Milotic, Mamoswine, Lucario 71.
8. Harmony [B2]: Infernape, Archeops, Tornadus 74; Harmon [W2]: Torterra, Arcanine, Thundurus 74.
9. Ira [B2]: Hydreigon, Articuno, Entei 77; Iria [W2]: Salamence, Moltres, Raikou 77.
10. Benga (ver acima); depois dele: Janna [B2]: Suicune, Heatran, Latios 80; Jariel [W2]: Zapdos, Cresselia, Latias 80.

Battle Subway (Nimbasa; todos lv 50; chefes na 21ª e na 49ª vitória seguida):
- Single (linha verde): Ingo 21ª Crustle, Klinklang, Garbodor; 49ª Excadrill, Haxorus, Chandelure.
- Double (linha vermelha): Emmet 21ª Crustle, Klinklang, Garbodor, Durant; 49ª Excadrill, Haxorus, Chandelure, Eelektross.
- Multi (laranja/amarela): Ingo + Emmet 21ª Klinklang, Garbodor, Durant, Galvantula; 49ª Excadrill, Haxorus, Eelektross, Archeops.
- Linhas super (verde-escuro, rosa, amarela) liberam após vencer o chefe no normal; Wi-Fi (azul-claro) sem chefes; marrom vai a Anville Town.
- Proibidos: Mewtwo, Mew, Lugia, Ho-Oh, Celebi, Kyogre, Groudon, Rayquaza, Jirachi, Deoxys, Dialga, Palkia, Giratina, Phione, Manaphy, Darkrai, Shaymin, Arceus, Victini, Reshiram, Zekrom, Kyurem, Keldeo, Meloetta, Genesect.

PWT (Driftveil; lv 50 salvo o Driftveil Tournament, lv 25): torneios Driftveil (Bianca, Cheren, Roxie, Burgh, Elesa,
Clay), Rental, Mix, Download, Unova Leaders (pós-Elite Four), Kanto/Johto/Hoenn/Sinnoh Leaders (após Unova Leaders),
World Leaders (após os cinco), Champions (10 vitórias no World Leaders), Rental Masters, Mix Masters, Type Expert.
Times do Unova Leaders Tournament (sem nível individual; lv 50):
Cheren (Stoutland, Zangoose, Cinccino, Lopunny, Castform, Bouffalant); Roxie (Scolipede, Seviper, Garbodor, Crobat,
Drapion, Amoonguss); Burgh (Leavanny, Vespiquen, Crustle, Escavalier, Accelgor, Durant); Elesa (Zebstrika, Ampharos,
Galvantula, Emolga, Eelektross, Stunfisk); Clay (Excadrill, Claydol, Krookodile, Seismitoad, Mamoswine, Golurk); Skyla
(Swanna, Unfezant, Swoobat, Mandibuzz, Archeops, Braviary); Drayden (Haxorus, Druddigon, Archeops, Flygon, Altaria,
Salamence); Marlon (Jellicent, Carracosta, Starmie, Alomomola, Mantine, Wailord); Chili (Simisear, Camerupt, Heatmor,
Darmanitan, Arcanine, Magmortar); Cress (Simipour, Golduck, Floatzel, Azumarill, Slowking, Basculin); Cilan (Simisage,
Ferrothorn, Maractus, Jumpluff, Whimsicott, Lilligant); Lenora (Watchog, Audino, Clefable, Sawsbuck, Kangaskhan,
Dunsparce); Brycen (Cryogonal, Beartic, Vanilluxe, Weavile, Dewgong, Walrein); Bianca (Toxicroak, Gothitelle,
Reuniclus, Bisharp, Abomasnow, Nidoqueen).
Champions Tournament (participantes listados): Blue (Aerodactyl, Exeggutor, Gyarados, Alakazam, Arcanine, Machamp);
Lance (Dragonite, Salamence, Kingdra, Hydreigon, Haxorus, Flygon); Steven (Metagross, Aggron, Excadrill, Archeops,
Cradily, Armaldo); Wallace (Milotic, Sharpedo, Walrein, Ludicolo, Swampert, Starmie); Cynthia (Garchomp, Spiritomb,
Togekiss, Lucario, Roserade, Glaceon); Alder (Volcarona, Conkeldurr, Reuniclus, Krookodile, Chandelure, Braviary);
Red (Venusaur, Charizard, Blastoise, Pikachu, Snorlax, Lapras). Times dos líderes de Kanto/Johto/Hoenn/Sinnoh e da
versão World Leaders estão nas páginas 128–134 do PDF (listas planas, ver Dúvidas).

## Áreas (para `areas/b2w2.json`, kind e ordem de liberação)

`*` = número da rota não escrito no detonado (vem da numeração oficial de Unova; conferir).

| Ordem | Id | Nome | Kind | Libera |
| --- | --- | --- | --- | --- |
| 1 | aspertia-city | Aspertia City | city | início |
| 2 | route-19 | Route 19* | route | início |
| 3 | floccesy-town | Floccesy Town (casa do Alder) | city | início |
| 4 | route-20 | Route 20* (parte leste após a 1ª insígnia) | route | início |
| 5 | floccesy-ranch | Floccesy Ranch | zone | início |
| 6 | virbank-city | Virbank City | city | após Cheren |
| 7 | virbank-complex | Virbank Complex | zone | Virbank |
| 8 | pokestar-studios | Pokéstar Studios | building | após Roxie |
| 9 | castelia-city | Castelia City (Main/Gym/Narrow/Mode/Castelia Street, Central Plaza, piers) | city | após Roxie (barco) |
| 10 | royal-unova | Royal Unova (cruzeiro) | building | Castelia, fim de tarde, $1000 |
| 11 | liberty-garden | Liberty Garden | zone | Castelia (Liberty Pier) |
| 12 | castelia-sewers | Castelia Sewers | dungeon | Castelia (Thumb Pier) |
| 13 | castelia-park | Castelia Park | zone | via Sewers |
| 14 | relic-passage | Relic Passage | dungeon | parcial via Sewers; completa após o PWT |
| 15 | route-4 | Route 4* (deserto; prédios [B2] / casinhas [W2]) | route | após Burgh |
| 16 | desert-resort | Desert Resort | zone | Route 4 |
| 17 | relic-castle | Relic Castle | dungeon | Desert Resort |
| 18 | join-avenue | Join Avenue | zone | Route 4 |
| 19 | nimbasa-city | Nimbasa City (metrô, Musical, estádios, roda-gigante) | city | após Join Avenue |
| 20 | anville-town | Anville Town (só pelo metrô) | city | após Ingo/Emmet |
| 21 | route-16 | Route 16 | route | Nimbasa |
| 22 | lostlorn-forest | Lostlorn Forest | zone | Route 16 (Strength) |
| 23 | route-5 | Route 5 (trailer) | route | após Elesa |
| 24 | driftveil-drawbridge | Driftveil Drawbridge | route | após Charles (4 insígnias) |
| 25 | driftveil-city | Driftveil City (Market) | city | após a ponte |
| 26 | pokemon-world-tournament | Pokémon World Tournament | building | após Clay |
| 27 | plasma-frigate | Plasma Frigate | building | após o PWT (volta no Seaside Cave, Giant Chasm e P2 Lab) |
| 28 | route-6 | Route 6* (Season Research Lab) | route | após o Plasma Frigate |
| 29 | mistralton-cave | Mistralton Cave | dungeon | Route 6 com Surf (Flash, Strength) |
| 30 | chargestone-cave | Chargestone Cave | dungeon | Route 6 |
| 31 | mistralton-city | Mistralton City (aeroporto) | city | após Chargestone |
| 32 | route-7 | Route 7* | route | Mistralton |
| 33 | celestial-tower | Celestial Tower | building | Route 7 |
| 34 | twist-mountain | Twist Mountain | dungeon | pós-jogo (Marshal bloqueia) |
| 35 | lentimas-town | Lentimas Town | city | após Skyla (avião) |
| 36 | strange-house | Strange House | building | fora da Reversal Mountain |
| 37 | reversal-mountain | Reversal Mountain (água [B2] / lava [W2]) | dungeon | Lentimas |
| 38 | undella-town | Undella Town | city | após Reversal Mountain |
| 39 | undella-bay | Undella Bay | sea | Undella com Surf |
| 40 | route-14 | Route 14 (sul de Undella) | route | Undella; cachoeiras após HM05 |
| 41 | route-13 | Route 13* (norte de Undella) | route | Undella |
| 42 | lacunosa-town | Lacunosa Town | city | após Cobalion |
| 43 | route-12 | Route 12* | route | Lacunosa |
| 44 | village-bridge | Village Bridge | route | após Route 12 |
| 45 | route-11 | Route 11* | route | após Village Bridge |
| 46 | opelucid-city | Opelucid City (moderna [B2] / natural [W2]) | city | após Virizion |
| 47 | route-9 | Route 9* (Shopping Mall Nine) | route | Opelucid |
| 48 | marine-tube | Marine Tube | building | após Drayden |
| 49 | humilau-city | Humilau City | city | após Marine Tube |
| 50 | route-22 | Route 22* (Terrakion, entrada do Giant Chasm) | route | Humilau |
| 51 | route-21 | Route 21* (mar ao sul de Humilau) | sea | após Marlon |
| 52 | seaside-cave | Seaside Cave | dungeon | Route 21 |
| 53 | giant-chasm | Giant Chasm (caverna, cratera, floresta) | dungeon | após o Plasma Frigate 2 |
| 54 | route-23 | Route 23* (TM35) | route | após Ghetsis |
| 55 | victory-road | Victory Road | dungeon | 8 insígnias |
| 56 | pokemon-league | Pokémon League | building | após Victory Road |
| 57 | abundant-shrine | Abundant Shrine | zone | Route 14 com Waterfall |
| 58 | ns-castle | ruínas do N's Castle | building | pós-jogo (Zoroark na Victory Road) |
| 59 | tubeline-bridge | Tubeline Bridge | route | pós-jogo |
| 60 | route-8 | Route 8* (poças) | route | pós-jogo |
| 61 | moor-of-icirrus | Moor of Icirrus | zone | pós-jogo |
| 62 | icirrus-city | Icirrus City | city | pós-jogo |
| 63 | dragonspiral-tower | Dragonspiral Tower | building | pós-jogo |
| 64 | marvelous-bridge | Marvelous Bridge | route | pós-jogo (leste da Route 16) |
| 65 | route-15 | Route 15* (Poké Transfer Lab) | route | pós-jogo |
| 66 | black-city-white-forest | Black City [B2] / White Forest [W2] (Black Tower / White Treehollow após Alder) | city | pós-jogo |
| 67 | skyarrow-bridge | Skyarrow Bridge | route | pós-jogo |
| 68 | pinwheel-forest | Pinwheel Forest | zone | pós-jogo |
| 69 | nacrene-city | Nacrene City (museu) | city | pós-jogo |
| 70 | route-3 | Route 3* (Day Care) | route | pós-jogo |
| 71 | wellspring-cave | Wellspring Cave | dungeon | pós-jogo (Flash) |
| 72 | striaton-city | Striaton City | city | pós-jogo |
| 73 | dreamyard | Dreamyard | zone | pós-jogo |
| 74 | route-2 | Route 2* | route | pós-jogo |
| 75 | accumula-town | Accumula Town | city | pós-jogo |
| 76 | route-1 | Route 1 (extensão com Surf) | route | pós-jogo |
| 77 | nuvema-town | Nuvema Town (lab da Juniper) | city | pós-jogo |
| 78 | nature-preserve | Nature Preserve (avião em Mistralton) | zone | pós-jogo, Permit (ver 297 da Unova Dex) |
| 79 | abyssal-ruins | Abyssal Ruins | dungeon | Undella Bay com Dive (500 passos por visita) |
| 80 | cave-of-being | Cave of Being | dungeon | Route 20 com Surf + Waterfall |
| 81 | clay-tunnel | Clay Tunnel | dungeon | pós-jogo (Rock Smash) |
| 82 | underground-ruins | Underground Ruins (Rock Peak, Iron e Iceberg Chamber) | dungeon | via Clay Tunnel |
| 83 | route-17 | Route 17* (mar, correntezas) | sea | pós-jogo (Route 1 com Surf) |
| 84 | route-18 | Route 18* (ilha) | route | pós-jogo |
| 85 | p2-laboratory | P2 Laboratory | building | pós-jogo |
| 86 | battle-institute | Battle Institute (Nimbasa) | building | após a Elite Four |

## Encontros (para `encounters/b2w2.json`; method `walk` salvo indicação; grama escura = `walk` com chance de dupla;
grama que mexe, poeira, sombra e água borbulhante são variantes do método)

- Route 19*: Patrat, Purrloin. `surf`: Basculin (borbulhante: forma da outra versão). `fish`: Poliwag; borbulhante: Poliwhirl, Politoed (raros).
- Route 20*: Sewaddle, Pidove, Sunkern (mais no verão); leste: Venipede; grama que mexe: Audino, Dunsparce. Outono: folhas cobrem buracos.
- Floccesy Ranch: Lillipup, Mareep, Azurill, Psyduck, Riolu (raro).
- Virbank Complex: Magnemite, Magby [B2] / Elekid [W2]; parte sul: Koffing, Growlithe. Virbank `surf`: Frillish; borbulhante: Alomomola; `fish`: Finneon, Krabby, Qwilfish; borbulhante: Kingler, Lumineon.
- Aspertia City `surf`: Basculin; `fish`: Goldeen; borbulhante: Seaking (raro).
- Castelia Sewers: Rattata, Zubat, Grimer (`surf`: Grimer, só primavera/verão; seco no outono/inverno).
- Castelia Park: Cottonee, Buneary [B2] / Petilil, Skitty [W2], Eevee (raro); grama que mexe: Lopunny [B2] / Delcatty [W2], Whimsicott [B2] / Lilligant [W2], Audino.
- Relic Passage: Roggenrola, Woobat, Timburr, Onix (raro); poeira (`cave`): Drilbur, Onix; parte de Driftveil: Boldore, Gurdurr; 2ª área: Raticate.
- Route 4* (areia funda): Sandile, Darumaka, Scraggy, Trubbish [B2] / Minccino [W2]. `fixed`: Mandibuzz lv 25 às quintas [B2] / Braviary lv 25 às segundas [W2]. `fish`: Clamperl; borbulhante: Relicanth, Huntail [B2] / Gorebyss [W2].
- Antes do Desert Resort: Maractus, Sandshrew, Dwebble. Desert Resort: + Sigilyph, Trapinch (raro).
- Relic Castle: Yamask. Volcarona `fixed` lv 35 (ou 65 no pós-jogo).
- Route 16: Minccino, Liepard, Gothita [B2] / Solosis [W2]; grama que mexe: Emolga, Cinccino.
- Lostlorn Forest: Swadloon, Combee, Roselia, Heracross [B2] / Pinsir [W2]; grama que mexe: Vespiquen, Roserade, Leavanny, Pansage, Pansear, Panpour, Emolga, Whimsicott [B2] / Lilligant [W2], Audino; `surf`: Buizel. Hidden Grotto ao norte.
- Route 5: Hidden Grotto (Minccino).
- Driftveil Drawbridge (sombra): Ducklett (e penas).
- Route 6*: Foongus (também disfarçado de item), Deerling, Shelmet, Karrablast, Marill; grama que mexe: Azumarill, Castform, Unfezant. Hidden Grotto.
- Mistralton Cave: Aron, Axew.
- Chargestone Cave: Joltik, Nosepass, Klink, Ferroseed, Tynamo (raro).
- Route 7*: Zebstrika, Seviper, Zangoose (raros); Cubchoo (inverno). Hidden Grotto.
- Celestial Tower: Litwick, Elgyem, Golbat.
- Fora da Reversal Mountain: Skorupi, Trapinch, Drifblim, Skarmory (raro), Spoink, Grumpig [B2] / Numel, Camerupt [W2]; grama escura: Vibrava (raro). Strange House: Banette.
- Undella Town `surf`: Staryu. Undella Bay `surf`: Mantyke, Remoraid (raro), Spheal (inverno); borbulhante: Jellicent, Wailmer, Mantine e Wailord (raros), Sealeo e Walrein (inverno); `fish`: Chinchou, Remoraid; borbulhante: Lanturn, Octillery. Entrada da Seaside Cave: Golduck, Seel. `fixed`: Jellicent macho às segundas [B2] / fêmea às quintas [W2] (sem nível listado).
- Route 14: Golduck, Swablu, Mienfoo, Absol, Altaria (raro).
- Route 13*: Tangela, Pelipper, Solrock, Lunatone. Hidden Grotto. Cobalion `fixed` lv 45 (65 se derrotado).
- Village Bridge: Lapras (borbulhante, muito raro); `fish`: Carvanha; borbulhante: Sharpedo.
- Route 11*: Gligar. Virizion `fixed` lv 45.
- Route 9*: Pawniard, Garbodor. Hidden Grotto.
- Humilau City `surf` borbulhante: Corsola; `fish`: Luvdisc, Shellder; borbulhante: Cloyster.
- Route 22*: Delibird (raro); Amoonguss disfarçado de item. Hidden Grotto. Terrakion `fixed` lv 45.
- Seaside Cave: poeira: Excadrill; andar de baixo: Shuckle (raro); Crustle `fixed` lv 42.
- Giant Chasm: Sneasel, Piloswine, Vanillish, Clefairy; cratera: Ditto, Metang. `fish` borbulhante: Poliwrath em vez de Politoed. Hidden Grotto.
- Route 23*: Bouffalant, Sawk [B2], Vullaby [B2], Throh [W2], Rufflet [W2]. Hidden Grotto.
- Victory Road: Golurk (caverna da entrada).
- Abundant Shrine: Bronzong, Vulpix. Duas Hidden Grottos.
- Route 8* (poças): Palpitoad, Stunfisk, Croagunk (sem encontros no inverno).
- Icirrus City `fish`: Barboach, Stunfisk; borbulhante: Whiscash.
- Fora da Dragonspiral Tower: Druddigon, Sawsbuck, Mienshao; inverno: Vanillite, Beartic. `fish`: Dratini; borbulhante: Dragonair, Dragonite.
- Marvelous Bridge (sombra): Swanna. Route 15*: Pupitar; grama que mexe: Tyranitar (raro).
- Pinwheel Forest: Vigoroth; grama que mexe: Slaking (raro), dois Audino com Cheren. Hidden Grotto. Fora: Toxicroak, Yanma; Hidden Grotto.
- Route 3*: duas Hidden Grottos.
- Dreamyard: Munna, Jigglypuff.
- Route 2*: Lickitung. Hidden Grotto.
- Route 1 (extensão) `fish`: Feebas (mais em borbulhante), Milotic (raro).
- Route 17* (mar) `fish`: Horsea; borbulhante: Seadra, Kingdra.
- Route 18* (ilha): Crustle, Tropius, Carnivine; `fish`: Corsola. Hidden Grotto.
- Nature Preserve: Nuzleaf, Girafarig, Noctowl, Kecleon (entre outros); Haxorus shiny `fixed` lv 60.
- Clay Tunnel: Lairon, Durant; poeira: Steelix (raro).
- Twist Mountain: Heatmor (raro), Cryogonal (inverno).
- Pós-jogo: Pokémon de Kanto/Johto/Hoenn/Sinnoh anunciados no letreiro das portarias, um por dia (`outbreak`).
- Lendários (`fixed`): Black/White Kyurem 55 (não capturável); Reshiram [W2] / Zekrom [B2] 70 (topo da Dragonspiral Tower, com a Light/Dark Stone do N); Kyurem 70 (Giant Chasm; dá DNA Splicers); Cresselia 68 (Marvelous Bridge, precisa da Lunar Wing da Strange House); Latios [B2] / Latias [W2] 68 (Dreamyard; dá Soul Dew); Uxie 65 (frente do museu, Nacrene); Mesprit 65 (sino da Celestial Tower); Azelf 65 (Victory Road, após arbusto de Cut); os três só após a Cave of Being; Regirock 65 (Rock Peak Chamber: do centro do olho, 6 passos ao sul e 9 à direita, alavanca); Registeel [B2] / Regice [W2] 65 (Iron Key [B2] / Iceberg Key [W2], liberada pelo Regirock; a outra por troca de key); Regigigas 68 (Twist Mountain, com Regirock, Regice e Registeel na equipe); Heatran 68 (cratera da Reversal Mountain, com a Magma Stone da Route 18*); Volcarona 35/65; Cobalion, Virizion, Terrakion 45/65.

## Presentes, trocas in-game e fósseis (`method: gift` / `trade`)

- Inicial: Snivy / Tepig / Oshawott (Bianca, Aspertia; nível não listado; o do Hugh vem no 5).
- Zorua lv 25 (Rood, Driftveil, com vaga na equipe).
- Deerling (pesquisador, Season Research Lab da Route 6*; nível não listado).
- Eevee lv 10 com Hidden Ability (Amanita, lab da Fennel, Castelia, pós-jogo).
- Ovo de Happiny (menina na portaria leste de Nacrene).
- Fósseis: Cover Fossil → Tirtouga ou Plume Fossil → Archen (Lenora, museu de Nacrene, pós-jogo); fóssil diário de Kanto/Hoenn/Sinnoh (operário, Twist Mountain); revividos no museu lv 25.
- Gible [B2] / Dratini [W2] shiny lv 1 (Alder, Floccesy, após vencer o Benga).
- Magikarp por $500 (Marvelous Bridge).
- Trocas: Cottonee [B2] ↔ Petilil [W2] (menina, Route 4*); Ditto → Rotom (trailer, Route 15*; único Rotom do jogo); Curtis/Yancy aceita trocas após 50 ligações.
- Recompensas: Pansage perdido (Anville Town, achado com um maquinista em Nimbasa).

## TMs, HMs e itens-chave (localização)

- HM01 Cut (Roxie, Virbank); HM04 Strength (Hugh, Castelia Sewers); HM02 Fly (Bianca, portaria oeste de Nimbasa); HM03 Surf (Cheren, Route 6*); HM05 Waterfall (N, entrada da Victory Road); HM06 Dive (Hugh, Undella Town, pós-jogo).
- TM70 Flash (homem de preto, Narrow Street, Castelia); TM94 Rock Smash (careca do Virbank Complex, após vencer todos os treinadores); TM28 Dig (deserto); TM49 (menina, Pokémon Musical); TM95 (Lostlorn Forest); TM53 (Aspertia, Surf); TM80 Rock Slide (Mistralton Cave); TM40 (sob a pista do aeroporto, Mistralton); TM57 (casa de Lentimas); TM13 Ice Beam (Giant Chasm, Strength); TM35 Flamethrower (casa da Route 23*); TM24 Thunderbolt (Hugh, Victory Road); TM de Bianca após Cheren (número não listado).
- Itens-chave: Running Shoes e 2 Town Maps (mãe, Aspertia); Medal Box (Floccesy); C-Gear (Bianca); bicicleta (arlequim, Castelia); Dowsing MCHN (Bianca, saída norte de Castelia); Vs. Recorder (Nate/Rosa, Nimbasa); Prop Case (Pokémon Musical); Colress MCHN (Colress, Route 22*); Plasma Card (Grunt no Plasma Frigate; [W2] no 1º navio, [B2] no 2º); DNA Splicers (Giant Chasm, após Kyurem); Light Stone [W2] / Dark Stone [B2] (N); Permit (Juniper); Super Rod (Cedric Juniper, Nuvema); Lunar Wing (Strange House); Magma Stone (Route 18*); Reveal Glass (Abundant Shrine, com Landorus Therian do Dream Radar); Soul Dew (Dreamyard).
- Raros: Master Ball (Juniper, Mistralton); 2ª Master Ball (Colress, P2 Lab); Lucky Egg (Juniper, Celestial Tower); Rare Candy (5 arlequins de Castelia; maquinista de Anville; Surf em Virbank); 3 Max Revive (Rood, Giant Chasm); Round Charm (capturar a Unova Dex); Shining Charm (capturar a National Dex); Exp. Share / Cleanse Tag / King's Rock (Fan Club de Icirrus, por níveis subidos).
- Pedras: Sun Stone (Nimbasa); Shiny Stone (casa da Route 6*); Thunderstone (Chargestone Cave); Leaf Stone (Route 7*); Fire Stone (Lentimas); Dusk Stone (Strange House); Water Stone (Route 19*, Surf); Moon Stone (Giant Chasm); pedras climáticas (moça de guarda-chuva, Route 8*, uma por dia); orbes de Dialga/Palkia/Giratina (Dragonspiral Tower); Up-Grade (Cheren, Pinwheel Forest); Star Piece diário (pedra socada, fora da Pinwheel Forest).
- Lojas e serviços: Move Tutors por Red Shards (Driftveil), Blue Shards (Lentimas), Yellow Shards (Humilau), Green Shards (Nacrene); PWT com Move Reminder/Deleter e tutor dos golpes elementais e Pledges; Draco Meteor (Drayden, Opelucid, pós-jogo); Relic Song para Meloetta (Café Sonata, Castelia); Secret Sword para Keldeo (casa do Alder); Gracidea (Striaton, com Shaymin); meteorito para Deoxys (museu de Nacrene); caixas de forma do Rotom (Shopping Mall Nine); Drives do Genesect (P2 Lab: Burn/Shock [B2], Douse/Chill [W2]); Casteliacone (Mode Street, fora do inverno); comprador de itens pelo dobro (PC de Icirrus); comprador de Rare Bone (casa da Route 18*); comprador de artefatos das Abyssal Ruins (Undella; Relic Crown → Big Pearl com Zinzolin); lojas da Black City/White Forest por área vencida (pedras e itens de evolução, $10000–$60000).

## Mecânicas (para o guia de mecânicas, texto nosso)

- Estações do ano mudam mapas: folhas no outono, esgoto seco no outono/inverno, neve em Icirrus e Twist Mountain, poças congeladas, Casteliacone fora do inverno.
- Key System (Unova Link): Challenge Mode [B2] / Easy Mode [W2] após a Elite Four; Tower/Treehollow Key; Iron/Iceberg Key. Keys passam entre jogadores.
- Memory Link com B/W libera Cheren e Bianca no pós-jogo e flashbacks.
- Grama escura (encontros em dupla), grama que mexe, poeira, sombras em pontes e água borbulhante dão encontros especiais.
- Hidden Grottos: Pokémon com Hidden Ability e itens; podem renovar a cada 256 passos.
- Batalhas em trio e rotacionais; Pkmn Breeders revanche ao reentrar na rota.
- Pokéstar Studios (filmes), Pokémon Musical, Join Avenue (lojas e popularidade), Medal Box.
- Battle Subway, Battle Institute, PWT, Black Tower / White Treehollow (sem mochila, 1 cura por área).
- Royal Unova: horário por estação (primavera/outono 17:00–19:59, verão 19:00–20:59, inverno 16:00–18:59); prêmio por dia da semana.
- Estádios de Nimbasa: esporte por dia da semana; times mudam a cada 2 insígnias.
- Roda-gigante: um treinador por estação; pós-jogo N (sexta), Hugh (terça), Bianca (quarta), Cheren (sábado).
- Poké Transfer Lab (Route 15*): transfere de D/P/Pt/HG/SS sem itens e sem HMs.
- Abyssal Ruins: 500 passos até o tremor expulsar; Plates de Arceus.
- Nature Preserve: exige ver os 297 da Unova Dex (sem Victini, Keldeo, Meloetta, Genesect); três precisam de troca para serem vistos.

Pós-jogo, ordem de eventos: National Dex → letreiros com Pokémon de outras regiões → Eevee da Amanita → N's Castle (N) → Tubeline Bridge e Route 8* → Icirrus (Shadow Triad) → Dragonspiral Tower (Reshiram/Zekrom) → Giant Chasm (Kyurem) → Marvelous Bridge (Cresselia) → Route 15* (Rotom, Poké Transfer Lab) → Skyarrow Bridge → Pinwheel Forest → Nacrene (fóssil) → Striaton (trio) → Dreamyard (Latios/Latias) → Accumula → Nuvema (Permit, Super Rod) → Nature Preserve (Haxorus shiny) → Alder → Hugh (Dive) → Abyssal Ruins → Cave of Being (Uxie, Mesprit, Azelf) → Clay Tunnel → Underground Ruins (Regis) → Twist Mountain (Regigigas) → rotas marítimas → P2 Lab (Colress) → Reversal Mountain (Heatran) → N sazonal → Cynthia → Morimoto/Nishino → revanche da Elite Four → Black Tower/White Treehollow (Benga).

## Dúvidas

- Black Tower/White Treehollow Áreas 1 e 4: as duas colunas do PDF se misturam no texto; a divisão de Pawniard/Golett/Boldore/Whirlipede e de Durant/Heatmor entre os dois Boss Trainers não é certa.
- Shadow Triad C no Plasma Frigate: Pawniard lv 46 no normal e 53 no CM (os outros dois estão a 49/53); provável erro de digitação.
- Cheren e Bianca do Memory Link: a associação inicial do B/W → macaco + inicial final vem da ordem das colunas no texto.
- Páginas 51 e 64–71 do PDF não têm texto (só imagens); podem esconder detalhes de batalhas (Opelucid, Kyurem). Sem poppler para renderizar aqui.
- Times do PWT de Kanto/Johto/Hoenn/Sinnoh/World/Champions: listas planas sem nível; alguns trainers aparecem com 5 ou 7 nomes por quebra de coluna. Nomes com erro: "Marmortar", "Clafable", "Mienchao", "Politoad", "Luario", "Eeleektrik", "Boufflant", "Zebstraika", "Vaniluxe".
- Números das rotas marcados com `*` não aparecem no texto (só 1, 5, 14 e 16 são citados).
- Níveis do inicial, do Deerling, do Jellicent fixo e dos Pokémon da Nature Preserve: não listados.
- Charles (Route 5 e Driftveil) e os Grunts de Virbank, Nimbasa e dos navios: sem time listado.
