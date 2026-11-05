# Sword/Shield — fatos extraídos do detonado da Pokémon Mythology

Fonte: https://www.pokemythology.net/detonados/detonado-sword-shield/ (PDF salvo em 05/11/2026, 270 páginas; inclui a
atualização 1.1.0, Isle of Armor e Crown Tundra). Só **fatos de jogo** (chefes, níveis, encontros, itens) no nosso formato.
Nenhuma prosa foi copiada. Golpes dos chefes **não** constam no detonado: deixar `moves: []` e `verified: false` para golpes;
espécies e níveis são `verified: true` (uma fonte) até o analista conferir numa segunda.
Versões: `[Sword]` / `[Shield]`. "inicial" = o inicial do rival (Hop pega o inicial fraco ao seu).

## Ordem da história e chefes (para `bosses/swsh.json`)

| Order | Id | Kind | Nome | Local | Time (espécie lv) | Cap |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | hop-1 | rival | Hop | Postwick | Wooloo 3, inicial 5 | 5 |
| 2 | hop-2 | rival | Hop | Casa da Magnolia (Route 2) | Wooloo 6, Rookidee 5, inicial 8 | 8 |
| 3 | yell-motostoke | evil | Team Yell (2 + dupla com Hop) | Budew Drop Inn, Motostoke | sem time listado | — |
| 4 | hop-3 | rival | Hop | Route 3 (antes da Galar Mine) | Wooloo 11, Rookidee 12, inicial 14 | 14 |
| 5 | bede-1 | rival | Bede | Galar Mine | Solosis 13, Gothita 15, Hatenna 16 | 16 |
| 6 | milo | gym | Milo (Grass) | Turffield Stadium | Gossifleur 19, Dynamax Eldegoss 20 | 20 |
| 7 | hop-4 | rival | Hop | Route 5 (após a Rotom Bike) | Wooloo 18, Corvisquire 19, inicial 21 | 21 |
| 8 | nessa | gym | Nessa (Water) | Hulbury Stadium | Goldeen 22, Arrokuda 23, Dynamax Drednaw 24 | 24 |
| 9 | bede-2 | rival | Bede | Galar Mine No. 2 | Solosis 21, Gothita 22, Galarian Ponyta 22, Hatenna 23 | 23 |
| 10 | marnie-1 | rival | Marnie | Budew Drop Inn, Motostoke | Croagunk 24, Scraggy 24, Morpeko 26 | 26 |
| 11 | kabu | gym | Kabu (Fire) | Motostoke Stadium | Ninetales 25, Arcanine 25, Gigantamax Centiskorch 27 | 27 |
| 12 | hop-5 | rival | Hop | Stow-on-Side | Cramorant 29, Toxel 29, Silicobra 30, inicial 33 | 33 |
| 13 | bea | gym | Bea (Fighting) `[Sword]` | Stow-on-Side Stadium | Hitmontop 34, Pangoro 34, Sirfetch'd 35, Gigantamax Machamp 36 | 36 |
| 13 | allister | gym | Allister (Ghost) `[Shield]` | Stow-on-Side Stadium | Galarian Yamask 34, Mimikyu 34, Cursola 35, Gigantamax Gengar 36 | 36 |
| 14 | bede-3 | rival | Bede | Ruínas/mural de Stow-on-Side | Duosion 32, Gothorita 32, Galarian Ponyta 33, Hattrem 35 | 35 |
| 15 | opal | gym | Opal (Fairy) | Ballonlea Stadium | Galarian Weezing 36, Mawile 36, Togekiss 37, Gigantamax Alcremie 38 | 38 |
| 16 | hop-6 | rival | Hop | Hammerlocke (saída para a Route 7) | Trevenant 34, Heatmor 34, Snorlax 35, Boltund 35, inicial 37 | 37 |
| 17 | gordie | gym | Gordie (Rock) `[Sword]` | Circhester Stadium | Barbaracle 40, Shuckle 40, Stonjourner 41, Gigantamax Coalossal 42 | 42 |
| 17 | melony | gym | Melony (Ice) `[Shield]` | Circhester Stadium | Frosmoth 40, Galarian Darmanitan 40, Eiscue 41, Gigantamax Lapras 42 | 42 |
| 18 | hop-7 | rival | Hop | Hero's Bath, Circhester | Dubwool 40, Corviknight 40, Pincurchin 39, Snorlax 39, inicial 41 | 41 |
| 19 | yell-circhester-bay | evil | Team Yell | Circhester Bay | sem time listado | — |
| 20 | marnie-2 | rival | Marnie | Outer Spikemuth | Liepard 42, Toxicroak 43, Scrafty 43, Morpeko 44 | 44 |
| 21 | piers | gym | Piers (Dark; sem Dynamax) | Spikemuth | Scrafty 44, Malamar 45, Skuntank 45, Obstagoon 46 | 46 |
| 22 | raihan | gym | Raihan (Dragon; dupla com sandstorm) | Hammerlocke Stadium | Gigalith 46, Flygon 47, Sandaconda 46, Gigantamax Duraludon 48 | 48 |
| 23 | marnie-cup | rival | Marnie (Champion Cup, semifinal) | Wyndon Stadium | Liepard 47, Toxicroak 47, Scrafty 47, Morpeko 48, Gigantamax Grimmsnarl 49 | 49 |
| 24 | hop-cup | rival | Hop (Champion Cup, semifinal) | Wyndon Stadium | Dubwool 48, Corviknight 48, Pincurchin 47, Snorlax 47, Dynamax inicial 49 | 49 |
| 25 | league-staff | other | League Staff de óculos escuros (×3) | Wyndon | sem time listado | — |
| 26 | oleana | evil | Oleana (Macro Cosmos) | Topo da Rose Tower | Froslass 50, Tsareena 50, Salazzle 50, Milotic 51, Gigantamax Garbodor 52 | 52 |
| 27 | bede-cup | rival | Bede (não oficial) | Wyndon Stadium | Mawile 51, Gardevoir 51, Galarian Rapidash 52, Gigantamax Hatterene 53 | 53 |
| 28 | nessa-cup | other | Nessa (Champion Cup, final) | Wyndon Stadium | Golisopod 51, Pelipper 51, Barraskewda 52, Seaking 52, Gigantamax Drednaw 53 | 53 |
| 29 | bea-cup | other | Bea (final) `[Sword]` | Wyndon Stadium | Hawlucha 52, Grapploct 52, Sirfetch'd 53, Falinks 53, Gigantamax Machamp 54 | 54 |
| 29 | allister-cup | other | Allister (final) `[Shield]` | Wyndon Stadium | Dusknoir 52, Chandelure 52, Cursola 53, Polteageist 53, Gigantamax Gengar 54 | 54 |
| 30 | raihan-cup | other | Raihan (final) | Wyndon Stadium | Torkoal 53, Goodra 54, Turtonator 54, Flygon 54, Gigantamax Duraludon 55 | 55 |
| 31 | rose | evil | Chairman Rose | Energy Plant, Hammerlocke | Escavalier 55, Ferrothorn 55, Perrserker 55, Klinklang 56, Gigantamax Copperajah 57 | 57 |
| 32 | eternatus-1 | other | Eternatus (não capturável) | Topo da Energy Plant | Eternatus 60 | 60 |
| 33 | eternatus-max | other | Eternamax Eternatus (Max Raid; captura depois) | Topo da Energy Plant | Eternamax Eternatus 60 | 60 |
| 34 | leon | champion | Leon | Wyndon Stadium | Aegislash 62, Dragapult 62, Haxorus 63, X 64, inicial de Leon 64, Gigantamax Charizard 65 (Grookey→ Seismitoad + Cinderace; Scorbunny→ Mr. Rime + Inteleon; Sobble→ Rhyperior + Rillaboom) | 65 |
| 35 | hop-postgame-1 | rival | Hop | Slumbering Weald (ruínas) | Dubwool 59, Corviknight 59, Pincurchin 58, Snorlax 58, Cramorant 58, inicial 60 | 60 |
| 36 | sordward-1 | evil | Sordward `[Sword]` | Slumbering Weald | Sirfetch'd 60, Golisopod 60, Doublade 60, Bisharp 60 | 60 |
| 36 | shielbert-1 | evil | Shielbert `[Shield]` | Slumbering Weald | Sirfetch'd 60, Bronzong 60, Falinks 60, Klinklang 60 | 60 |
| 37 | dmax-tsareena | other | Dynamax Tsareena (com Milo) | Turffield Stadium | Tsareena 62 | 62 |
| 38 | dmax-gyarados | other | Dynamax Gyarados (com Nessa) | Hulbury Stadium | Gyarados 62 | 62 |
| 39 | dmax-torkoal | other | Dynamax Torkoal (com Kabu) | Motostoke Stadium | Torkoal 62 | 62 |
| 40 | sordward-shielbert-2 | evil | Sordward + Shielbert (dupla com Hop) | Laboratório de Wedgehurst | Sordward: Golisopod 62, Doublade 62, Bisharp 62; Shielbert: Bronzong 62, Falinks 62, Klinklang 62 | 62 |
| 41 | dmax-stow | other | Dynamax Conkeldurr `[Sword]` / Dusknoir `[Shield]` | Stow-on-Side Stadium | 62 | 62 |
| 42 | bede-leader | other | Bede (novo líder) | Ballonlea Stadium | Mawile 61, Gardevoir 61, Galarian Rapidash 62, Hattrene 63 | 63 |
| 43 | dmax-circhester | other | Dynamax Gigalith `[Sword]` / Froslass `[Shield]` | Circhester Stadium | 62 | 62 |
| 44 | dmax-haxorus | other | Dynamax Haxorus (com Raihan) | Hammerlocke Stadium | Haxorus 62 | 62 |
| 45 | sordward-shielbert-3 | evil | Sordward `[Sword]` / Shielbert `[Shield]` | Energy Plant | Sordward: Sirfetch'd 64, Golisopod 64, Doublade 64, Bisharp 64; Shielbert: Sirfetch'd 64, Bronzong 64, Falinks 64, Klinklang 64 | 64 |
| 46 | legend-opposite | other | Zacian `[Shield]` / Zamazenta `[Sword]` (descontrolado; só vencer) | Topo da Energy Plant | 70 | 70 |
| 47 | legend-own | other | Zacian `[Sword]` / Zamazenta `[Shield]` (captura) | Topo da Energy Plant | 70 | — |
| 48 | hop-final | rival | Hop (batalha final) | Slumbering Weald | Dubwool 68, Corviknight 69, Pincurchin 68, Snorlax 68, inicial 70, Zacian/Zamazenta (da versão oposta) 70 | 70 |
| 49 | marnie-rematch | other | Marnie (1×/dia) | Fora do Pokémon Center de Spikemuth | Liepard 59, Toxicroak 59, Scrafty 59, Morpeko 60, Grimmsnarl 60 | 60 |
| 50 | morimoto | other | Morimoto (dupla; 1×/dia) | Hotel de Circhester | Cursola 65, Grapploct 65, Stonjourner 65, Dragapult 65, Coalossal 65, Dynamax Snorlax 65 | 65 |
| 51 | leon-tower | other | Leon (Battle Tower, fim de cada nível; depois a cada 10 batalhas no Master Ball) | Battle Tower, Wyndon | sorteio entre Aegislash, Dragapult, Haxorus, Seismitoad, Mr. Rime, Rhyperior, Rillaboom, Cinderace, Inteleon; sempre Gigantamax Charizard; todos 50 | — |

Champion Cup revanche (Wyndon Stadium, torneio de 8; convida até 10 com Rare League Card):
Milo `[Sword]` Shiftry 60, Eldegoss 60, Bellossom 61, Cherrim 61, Gigantamax Flapple 62; Milo `[Shield]` Ludicolo 60, Eldegoss 60,
Bellossom 61, Cherrim 61, Gigantamax Appletun 62; Nessa Golisopod 60, Pelipper 60, Quagsire 61, Toxapex 61, Gigantamax Drednaw 62;
Kabu Torkoal 60, Ninetales 60, Arcanine 61, Salazzle 61, Gigantamax Centiskorch 62; Bea Hawlucha 60, Grapploct 60, Sirfetch'd 61,
Falinks 61, Gigantamax Machamp 62; Allister Dusknoir 60, Chandelure 60, Cursola 61, Runerigus 61, Gigantamax Gengar 62; Bede Mawile 61,
Gardevoir 61, Galarian Rapidash 62, Sylveon 62, Gigantamax Hatterene 63; Gordie Barbaracle 60, Shuckle 60, Stonjourner 61, Tyranitar 61,
Gigantamax Coalossal 62; Melony Frosmoth 60, Mr. Rime 60, Eiscue 61, Galarian Darmanitan 61, Gigantamax Lapras 62; Marnie Liepard 59,
Toxicroak 59, Scrafty 59, Morpeko 60, Gigantamax Grimmsnarl 60; Piers Scrafty 60, Malamar 60, Skuntank 61, Toxtricity 61, Obstagoon 62;
Raihan Torkoal 60, Goodra 60, Turtonator 61, Flygon 61, Gigantamax Duraludon 62; Hop Dubwool 69, Snorlax 68, inicial 70,
Gigantamax Corviknight 70, Zacian/Zamazenta 70; Leon Aegislash 70, Dragapult 68, X 69, inicial de Leon 70, Gigantamax Charizard 70
(Grookey→ Seismitoad + Cinderace; Scorbunny→ Mr. Rime + Inteleon; Sobble: coluna cortada no PDF).
Treinadores avulsos (sem Dynamax), 56/56/57/57/58: Yue (Greedent, Cinccino, Diggersby, Dubwool, Snorlax); Polaire (Ribombee,
Golisopod, Crustle, Orbeetle, Vikavolt); Kent (Sigilyph, Unfezant, Mandibuzz, Cramorant, Braviary); Vega (Pelipper, Barraskewda,
Seismitoad, Araquanid, Gyarados); Cher (Gigalith, Stonjourner, Shuckle, Sudowoodo, Barbaracle); Deneb (Falinks, Grapploct, Hawlucha,
Lucario, Sirfetch'd); Icla (Toxicroak, Drapion, Garbodor, Galarian Weezing, Toxapex); Wei (Polteageist, Froslass, Trevenant, Golurk,
Jellicent); Izar (Vanilluxe, Frosmoth, Beartic, Mr. Rime, Mamoswine); Dunne (Sandaconda, Dugtrio, Claydol, Mudsdale, Flygon); Pia
(Shiinotic, Eldegoss, Ferrothorn, Vileplume, Tsareena); Corvin (Boltund, Raichu, Lanturn, Toxtricity, Dracozolt); Terry (Drampa,
Noivern, Turtonator, Dracovish, Haxorus); Theemin (Perrserker, Mawile, Bronzong, Steelix, Copperajah); Phoebus (Torkoal, Heatmor,
Heat Rotom, Chandelure, Arcanine).
Depois do Galarian Star Tournament, a Champion Cup sobe para 72/72/73/73/74 (Milo e Nessa/Kabu/... mesmos times, Milo troca
Bellossom por Tsareena; Raihan Torkoal 74, Goodra 74, Turtonator 75, Flygon 75, Gigantamax Duraludon 76; Leon Aegislash 80,
Dragapult 78, X 79, inicial 80, Gigantamax Charizard 80); avulsos sobem para 68/68/69/69/70.

### Isle of Armor (Expansion Pass, versão 1.2)

| Order | Id | Kind | Nome | Local | Time (espécie lv) | Cap |
| --- | --- | --- | --- | --- | --- | --- |
| 52 | galarian-slowpoke-1-1-0 | other | Galarian Slowpoke (captura; update 1.1.0, grátis) | Wedgehurst Station | Galarian Slowpoke 12 | — |
| 53 | klara-1 | rival | Klara `[Sword]` | Armor Station | Venipede 58, Galarian Slowpoke 60 | 60 |
| 53 | avery-1 | rival | Avery `[Shield]` | Armor Station | Abra 58, Galarian Slowpoke 60 | 60 |
| 54 | mustard-1 | other | Mustard | Master Dojo | Mienfoo 61, Shinx 61 | 61 |
| 55 | klara-2 | rival | Klara `[Sword]` | Warm-Up Tunnel | Galarian Slowpoke 62, Whirlipede 63, Koffing 62 | 63 |
| 55 | avery-2 | rival | Avery `[Shield]` | Warm-Up Tunnel | Galarian Slowpoke 62, Kadabra 63, Woobat 62 | 63 |
| 56 | klara-3 | rival | Klara `[Sword]` | Battle Court, Master Dojo | Skorupi 65, Galarian Weezing 66, Whirlipede 66, Dynamax Galarian Slowbro 67 | 67 |
| 56 | avery-3 | rival | Avery `[Shield]` | Battle Court, Master Dojo | Galarian Ponyta 65, Swoobat 66, Kadabra 66, Dynamax Galarian Slowbro 67 | 67 |
| 57 | mustard-tower | other | Mustard (topo da torre; só Kubfu) | Tower of Darkness / Tower of Water | Kubfu 70 | 70 |
| 58 | dmax-vespiquen | other | Dynamax Vespiquen (Max Raid) | Honeycalm Island | Vespiquen 70 | 70 |
| 59 | mustard-final | other | Mustard (Urshifu oposto ao seu; 1×/dia depois) | Battle Court | Mienshao 73, Luxray 73, Corviknight 74, Kommo-o 75, Lycanroc 75, Gigantamax Urshifu 75 | 75 |
| 60 | klara-rematch | other | Klara `[Sword]` (1×/dia) | Master Dojo | Drapion 65, Galarian Weezing 66, Scolipede 66, Dynamax Galarian Slowbro 67 | 67 |
| 60 | avery-rematch | other | Avery `[Shield]` (1×/dia) | Master Dojo | Galarian Rapidash 65, Swoobat 66, Alakazam 66, Dynamax Galarian Slowbro 67 | 67 |
| 61 | honey-1 | other | Honey (1.000.000 watts) | Battle Court | Blissey 60, Galarian Darmanitan 58, Gallade 59, Salazzle 58, Togekiss 60, Gigantamax Venusaur/Blastoise (o que você não escolheu) 60 | 60 |
| 62 | honey-rematch | other | Honey (1×/dia) | Battle Court | Blissey 75, Galarian Darmanitan 73, Gallade 74, Salazzle 73, Togekiss 75, Gigantamax Venusaur/Blastoise 75 | 75 |

### Crown Tundra (Expansion Pass, versão 1.3)

| Order | Id | Kind | Nome | Local | Time (espécie lv) | Cap |
| --- | --- | --- | --- | --- | --- | --- |
| 63 | peony-1 | other | Peony | Crown Tundra Station | Copperajah 70, Aggron 70 | 70 |
| 64 | calyrex-1 | other | Calyrex (não capturável) | Freezington | nível "???" no detonado | — |
| 65 | steed-1 | other | Glastrier (Snowslide Slope) / Spectrier (Old Cemetery), só vencer | Freezington | 75 | 75 |
| 66 | dmax-greedent | other | Dynamax Greedent (Max Raid, não capturável) | Dyna Tree, Dyna Tree Hill | Greedent 90 | 90 |
| 67 | star-tournament | other | Galarian Star Tournament (duplas; 1ª vez com Hop ou Marnie) | Wyndon Stadium | Milo+Nessa → Bea+Gordie `[Sword]` / Allister+Melony `[Shield]` → Raihan+Leon (parceiro Hop) ou Raihan+Piers (parceira Marnie) | — |
| 68 | peony-rematch | other | Peony (1×/dia, após as 3 Legendary Clues) | Max Lair | Perrserker 74, Scizor 74, Bronzong 74, Aggron 74, Copperajah 74 | 74 |
| 69 | marnie-final | other | Marnie (pós-Star Tournament, 1×/dia) | Spikemuth | Liepard 73, Toxicroak 72, Scrafty 72, Morpeko 73, Grimmsnarl 74 | 74 |
| 70 | klara-final | other | Klara `[Sword]` (pós-Star Tournament) | Master Dojo | Galarian Slowking 68, Galarian Weezing 68, Drapion 68, Scolipede 69, Dynamax Galarian Slowbro 70 | 70 |
| 70 | avery-final | other | Avery `[Shield]` (pós-Star Tournament) | Master Dojo | Galarian Slowbro 68, Swoobat 68, Galarian Rapidash 68, Alakazam 69, Dynamax Galarian Slowking 70 | 70 |
| 71 | mustard-ultimate | other | Mustard (pós-Star Tournament) | Master Dojo | Mienshao 78, Luxray 78, Corviknight 79, Kommo-o 80, Lycanroc 80, Gigantamax Urshifu 80 | 80 |

Galarian Star Tournament, times (3 por treinador): Hop Dubwool 69, Zacian/Zamazenta 70, Gigantamax Rillaboom/Cinderace/Inteleon 70;
Milo `[Sword]` Shiftry 72, Cherrim 73, Gigantamax Flapple 74; Milo `[Shield]` Ludicolo 72, Cherrim 73, Gigantamax Appletun 74;
Nessa Golisopod 72, Toxapex 73, Gigantamax Drednaw 74; Kabu Torkoal 72, Arcanine 73, Gigantamax Centiskorch 74; Bea Hawlucha 72,
Falinks 73, Gigantamax Machamp 74; Allister Dusknoir 72, Cursola 73, Gigantamax Gengar 74; Bede Mawile 72, Galarian Rapidash 73,
Gigantamax Hatterene 74; Opal Galarian Weezing 68, Togekiss 69, Gigantamax Alcremie 70; Gordie Shuckle 72, Tyranitar 73, Gigantamax
Coalossal 74; Melony Mr. Rime 72, Galarian Darmanitan 73, Gigantamax Lapras 74; Marnie Morpeko 73, Scrafty 72, Gigantamax Grimmsnarl 74;
Piers Skuntank 73, Toxtricity 73, Obstagoon 74; Raihan Flygon 75, Goodra 74, Gigantamax Duraludon 76; Leon Aegislash 80, Dragapult 78,
Gigantamax Charizard 80; Klara Galarian Slowking 68, Scolipede 69, Dynamax Galarian Slowbro 70; Avery Galarian Slowbro 68, Alakazam 69,
Dynamax Galarian Slowking 70; Mustard Mienshao 80, Kommo-o 80, Gigantamax Urshifu 80; Peony Perrserker 74, Aggron 74, Gigantamax
Copperajah 74. Após 15 batalhas: Sordward (Sirfetch'd 65, Golisopod 65, Bisharp 65) e Shielbert (Sirfetch'd 65, Bronzong 65,
Klinklang 65). Prêmio da 1ª vitória: Sport Uniform e $1.000.000.

Teto de captura por insígnia: Grass 25, Water 30, Fire 35, Fighting/Ghost 40, Fairy 45, Rock/Ice 50, Dark 55, Dragon sem limite.

## Áreas (para `areas/swsh.json`, kind e ordem de liberação)

| id | nome | kind | quando libera |
| --- | --- | --- | --- |
| postwick | Postwick | city | início |
| slumbering-weald | Slumbering Weald | dungeon | início (névoa); volta no Darkest Day e no pós-jogo |
| route-1 | Route 1 | route | início |
| wedgehurst | Wedgehurst (laboratório da Sonia, Wedgehurst Station) | city | início |
| route-2 | Route 2 (casa da Magnolia) (nº não citado no detonado) | route | início |
| wild-area | Wild Area (Meetup Spot; estação de entrada) | zone | após o trem parar; volta após a 3ª insígnia |
| motostoke | Motostoke (Budew Drop Inn, Stadium, Battle Café) | city | após a Wild Area |
| route-3 | Route 3 (nº não citado) | route | após a cerimônia de abertura |
| galar-mine | Galar Mine | dungeon | após a Route 3 |
| route-4 | Route 4 (nº não citado) | route | após a Galar Mine |
| turffield | Turffield | city | após a Route 4 |
| route-5 | Route 5 (Pokémon Nursery) (nº não citado) | route | após a 1ª insígnia |
| hulbury | Hulbury (Captain's Table) | city | após a Route 5 |
| galar-mine-2 | Galar Mine No. 2 | dungeon | após a 2ª insígnia |
| motostoke-outskirts | Motostoke Outskirts | route | após a Galar Mine No. 2 |
| hammerlocke | Hammerlocke (castelo/Stadium, Energy Plant) | city | após a 3ª insígnia (pela Wild Area) |
| route-6 | Route 6 (cientista de fósseis) (nº não citado) | route | após Hammerlocke |
| stow-on-side | Stow-on-Side | city | após a Route 6 |
| glimwood-tangle | Glimwood Tangle | dungeon | após a 4ª insígnia |
| ballonlea | Ballonlea | city | após a Glimwood Tangle |
| route-7 | Route 7 | route | após a 5ª insígnia |
| route-8 | Route 8 (ruínas e túneis; trecho nevado) (nº não citado) | route | após a Route 7 |
| circhester | Circhester (hotel, Hero's Bath) | city | após a Route 8 |
| route-9 | Route 9 (Circhester Bay, Outer Spikemuth) (nº não citado) | route | após a 6ª insígnia; água após a Rotom Bike Water Mode |
| route-9-tunnel | Route 9 Tunnel | dungeon | após Outer Spikemuth |
| spikemuth | Spikemuth | city | após vencer Marnie em Outer Spikemuth |
| route-10 | Route 10 (rota nevada antes de Wyndon) (nº não citado) | route | após a 8ª insígnia (de trem) |
| wyndon | Wyndon (Stadium, hotel, monotrilho) | city | após a Route 10 |
| rose-tower | Rose Tower (Macro Cosmos) | building | Champion Cup, após a semifinal |
| energy-plant | Energy Plant | building | Darkest Day |
| battle-tower | Battle Tower (antiga Rose Tower) | building | após a batalha final com Hop |
| armor-station | Armor Station / Isle of Armor | zone | Expansion Pass (1.2) |
| master-dojo | Master Dojo (Battle Court) | building | Isle of Armor |
| warm-up-tunnel | Warm-Up Tunnel | dungeon | 2ª prova do dojo |
| forest-of-focus | Forest of Focus | zone | Isle of Armor |
| towers-of-two-fists | Tower of Darkness / Tower of Water | building | após a 3ª prova e a felicidade do Kubfu |
| honeycalm-island | Honeycalm Island | zone | missão do Gigantamax Urshifu |
| potbottom-desert | Potbottom Desert | zone | Isle of Armor |
| crown-tundra-station | Crown Tundra Station / Crown Tundra | zone | Expansion Pass (1.3) |
| max-lair | Max Lair | dungeon | Crown Tundra |
| freezington | Freezington | city | após o Max Lair |
| frostpoint-field | Frostpoint Field | zone | Crown Tundra |
| giants-bed | Giant's Bed (Rock Peak Ruins, Iron Ruins) | zone | Crown Tundra |
| old-cemetery | Old Cemetery | zone | Crown Tundra |
| snowslide-slope | Snowslide Slope (Iceberg Ruins) | zone | Crown Tundra |
| crown-shrine | Crown Shrine | building | após a Reins of Unity |
| ballimere-lake | Ballimere Lake | zone | Crown Tundra |
| lakeside-cave | Lakeside Cave | dungeon | Crown Tundra |
| dyna-tree-hill | Dyna Tree Hill | zone | Crown Tundra |
| frigid-sea | Frigid Sea | sea | Crown Tundra |
| roaring-sea-caves | Roaring-Sea Caves | dungeon | Crown Tundra |
| three-point-pass | Three-Point Pass (Split-Decision Ruins) | zone | Crown Tundra |

## Encontros (para `encounters/swsh.json`; `walk` = andando no cenário, `grass` vira `walk` com nota "matinho")

- Slumbering Weald (início): Skwovet, Rookidee, Hoothoot, Blipbug, Grubbin (matinho). Pós-Darkest Day: Galarian Weezing, Munna, Orbeetle, Corviknight (walk); Butterfree, Galarian Stunfisk (matinho); Barboach, Whiscash (fish).
- Route 1: Skwovet, Caterpie, Hoothoot, Blipbug, Grubbin (matinho); Rookidee, Wooloo, Nickit (mais raro) (walk).
- Route 2: Chewtle, Yamper, Galarian Zigzagoon (mais raro) (walk); Purrloin, Seedot `[Sword]`, Lotad `[Shield]` (matinho); Magikarp, Chewtle, Arrokuda (raro) (fish).
- Wild Area: clima muda os encontros por sub-área; Max Raid nos pilares de luz; árvores de berry derrubam Pokémon (lista não dada).
- Route 3: Gossifleur, Stunky, Trubbish, Tyrogue, Vulpix `[Sword]`, Growlithe `[Shield]` (walk); Machop, Pancham, Mudbray, Klink, Sizzlipede (mais raro) (matinho); Cherubi (tree); Rolycoly (sai do chão).
- Galar Mine: Rolycoly, Woobat, Timburr, Diglett, Drilbur, Roggenrola (cave).
- Route 4: Electrike, Galarian Meowth, Pumpkaboo, Pikachu, Eevee (Eevee mais raro em `[Sword]`, Pikachu mais raro em `[Shield]`) (walk); Milcery, Joltik, Cutiefly, Budew, Ferrothorn (mais raro) (matinho); Goldeen (fish).
- Route 5: Stufful, Swirlix `[Sword]`, Spritzee `[Shield]`, Minccino, Wobbuffet, Galarian Farfetch'd, Drifloon (walk); Dottler, Espurr, Nuzleaf `[Sword]`, Lombre `[Shield]`, Dewpider, Applin, Nincada (matinho).
- Hulbury (ponto de pesca): Arrokuda, Chinchou, Basculin, Wishiwashi (fish).
- Galar Mine No. 2: Shellos, Wimpod, Binacle, Scraggy `[Sword]`, Croagunk `[Shield]`, Shuckle, Noibat (cave); Galarian Stunfisk (escondido no chão); Barboach, Corphish (fish).
- Motostoke Outskirts: Noctowl, Sudowoodo, Koffing, Hatenna, Sawk `[Sword]`, Throh `[Shield]`, Salandit `[Shield]` (walk); Impidimp, Pawniard (matinho).
- Route 6: Galarian Yamask, Helioptile, Dugtrio, Maractus, Axew, Trapinch (Trapinch mais raro em `[Sword]`, Axew mais raro em `[Shield]`) (walk); Silicobra, Durant, Heatmor, Duskull, Skorupi, Hippopotas, Torkoal, Hawlucha (mais raro) (matinho); Drednaw (fish).
- Glimwood Tangle: Morgrem, Shiinotic, Sinistea, Hattrem, Galarian Ponyta `[Shield]`, Phantump, Indeedee, Passimian `[Sword]`, Oranguru `[Shield]` (walk); Impidimp (cogumelos).
- Route 7: Perrserker, Thievul, Galvantula, Liepard, Inkay, Morpeko (walk); Toxel, Meowstic, Corviknight, Seismitoad, Karrablast, Shelmet (Shelmet mais raro em `[Sword]`, Karrablast mais raro em `[Shield]`) (matinho).
- Route 8 (ruínas): Pawniard, Golett, Gurdurr, Boldore, Togedemaru, Rufflet `[Sword]`, Vullaby `[Shield]`, Solrock `[Sword]`, Lunatone `[Shield]` (walk); Sandaconda, Rhyhorn, Dusclops, Haunter, Bronzong, Hippowdon, Drapion, Falinks (mais raro) (matinho); Falinks nos túneis; Crustle bloqueando passagem (fixed).
- Route 8 (neve): Snom, Snorunt, Sneasel, Vanillish, Sawk `[Sword]`, Throh `[Shield]` (walk); Snover, Delibird, Galarian Darumaka `[Sword]` (matinho).
- Route 9 (antes de Circhester Bay): Pelipper, Gastrodon, Mareanie, Jellicent, Pyukumuku (walk); Cramorant, Octillery, Kingler, Pincurchin (matinho); Octillery, Wishiwashi, Pyukumuku (fish); Glalie (becos); Pelipper, Qwilfish, Mantyke, Jellicent (surf, com Rotom Bike).
- Circhester Bay: Clobbopus, Octillery, Toxapex, Barbaracle, Bergmite, Dhelmise (mais raro) (walk); Wailmer, Remoraid, Mantyke, Mantine (surf); Grapploct (becos); Wailord, Lapras (fish, raros).
- Outer Spikemuth: Perrserker, Thievul, Liepard, Morpeko (matinho).
- Route 10: Galarian Mr. Mime, Cubchoo, Snover, Glalie, Vanilluxe, Sneasel, Duraludon (mais raro) (walk); Klang, Vanillish, Rhydon, Snom, Galarian Darumaka `[Sword]`, Beartic, Abomasnow, Stonjourner `[Sword]`, Eiscue `[Shield]` (matinho).
- Isle of Armor: Wailord lv 80 único, fixo sobre o mar; Alolan Diglett ×150 enterrados (fixed). Max Mushrooms nascem a cada 3 Max Raids (cavernas ou Forest of Focus).
- Lendários (`method: fixed`, salvo indicação): Zacian `[Sword]` / Zamazenta `[Shield]` lv 70 (Energy Plant, pós-jogo); Eternatus lv 60 (Max Raid, Energy Plant, Darkest Day); Galarian Slowpoke lv 12 (Wedgehurst Station, update 1.1.0); Kubfu lv 10 (gift, Master Dojo); Calyrex lv 80 fundido (Crown Shrine; Ice Rider se plantar no Snowslide Slope, Shadow Rider no Old Cemetery); Cobalion lv 70 (Frigid Sea), Terrakion lv 70 (Lakeside Cave), Virizion lv 70 (Giant's Bed), após 50 pegadas de cada; Keldeo lv 65 (ilha do Ballimere Lake, com os três na equipe e um curry); Spiritomb lv 72 (lápide do Ballimere Lake, falar com 32 jogadores online); Regirock lv 70 (Rock Peak Ruins, 1º com Everstone); Regice lv 70 (Iceberg Ruins, 1º Cryogonal); Registeel lv 70 (Iron Ruins, assobiar); Regieleki ou Regidrago lv 70 (Split-Decision Ruins, com os 3 Regis; só um por save); Regigigas lv 100 (`max_raid`, poço cercado por 4 pedras na Giant's Bed, com os 5 Regis); Galarian Articuno lv 70 (Crown Tundra), Galarian Zapdos lv 70 (Wild Area, perseguir), Galarian Moltres lv 70 (Isle of Armor, voando).
- Max Lair (`method: max_raid`, Dynamax Adventure; 1º lendário fixo: Suicune). Ambas as versões: Articuno, Zapdos, Moltres, Mewtwo, Raikou, Entei, Suicune, Rayquaza, Uxie, Mesprit, Azelf, Heatran, Giratina, Cresselia, Landorus, Kyurem, Zygarde, Tapu Koko, Tapu Lele, Tapu Bulu, Tapu Fini, Nihilego, Buzzwole, Pheromosa, Xurkitree, Celesteela, Kartana, Guzzlord, Necrozma, Stakataka, Blacephalon (UBs só depois da missão das Ultra Beasts). `[Sword]`: Ho-Oh, Latios, Groudon, Dialga, Tornadus, Reshiram, Xerneas, Solgaleo. `[Shield]`: Lugia, Latias, Kyogre, Palkia, Thundurus, Zekrom, Yveltal, Lunala. (Lista só em imagens; nomes identificados pelas figuras.)

## Presentes, trocas, fósseis e ovos (`method: gift`)

- Inicial: Grookey/Scorbunny/Sobble lv 5 (Postwick).
- Pikachu (com save de Let's Go Pikachu) ou Eevee (com save de Let's Go Eevee), Gigantamax, não evoluem (estação da Wild Area).
- Toxel lv 1 (Route 5, Pokémon Nursery; Nature define a forma do Toxtricity) + 5 Exp. Candy XS.
- Charmander com Gigantamax (quarto do Leon, casa do Hop, pós-jogo).
- Type: Null + 17 memórias do Silvally (Battle Tower).
- Isle of Armor: Bulbasaur ou Squirtle com Gigantamax (2ª prova); Kubfu lv 10 (fim da 3ª prova); Porygon lv 10 (Hyde, após Mustard final).
- Alolan Diglett (dono na ponte do dojo): 5 → Alolan Meowth lv 5; 10 → Slowpoke lv 10; 20 → Alolan Vulpix lv 5; 30 → Alolan Sandshrew lv 5; 40 → Alolan Raichu lv 30; 50 → Alolan Marowak lv 30; 75 → Alolan Exeggutor lv 30; 100 → Rowlet/Litten/Popplio lv 5 (segue o seu inicial Grookey/Scorbunny/Sobble); 150 → Alolan Diglett lv 20.
- Crown Tundra: Cosmog lv 5 (casa em Freezington, após Calyrex); Poipole lv 20 (entrada do Max Lair, após 5 Ultra Beasts).
- Fósseis: um homem no Pokémon Center de Stow-on-Side dá pedaços; a cientista na Route 6 monta e revive (combinações não listadas).
- Trocas in-game: nenhuma citada no detonado.

## TMs, itens-chave e lojas (localização)

TM40 Swift (Hop, Wedgehurst Station). Steel Beam por tutor (escadaria de metal em Motostoke, pós-jogo). Tutor do Master Dojo
(paga com Amorite Ore; Honey dá 5). Itens: Letter of Endorsement (Leon), Wishing Star → Dynamax Band (Magnolia), Camping Gear
(Wedgehurst Station), Pokémon Box Link (Sonia, Wild Area), 5 Poké Dolls (estação da Wild Area), Escape Rope (key item, Sonia,
Route 3), Rotom Bike (Route 5; Water Mode em Circhester Bay), Miracle Seed/Charcoal/Mystic Water (Leon, Motostoke, conforme o
inicial), Catching Charm (hotel de Circhester), Rusted Sword/Shield (Slumbering Weald), Master Ball (Magnolia, pós-jogo), Sonia's Book,
Exp. Candy L + 5 Nuggets + 2 Big Nuggets + Lucky Egg (entregas do Captain's Table, Hulbury), Oval Charm (Morimoto), Shiny Charm
(médico do hotel de Circhester, com a Galar Dex de 400 completa), Exp. Charm (Hyde), Style Card (Klara/Avery), Max Honey
(Vespiquen), Master Ball + Expedition Uniform + Legendary Clues (Peony), Wooden Crown, Carrot Seeds (8 Dynite Ore), Iceroot/
Shaderoot Carrot, White/Black Mane Hair, Radiant Petal, Reins of Unity, Gold Expedition Uniform (Peonia), Beast Ball + Ability Patch
(pesquisadora, após Necrozma). Lojas: Battle Café (Motostoke e Wyndon), boutiques e salões; vendedores de watts na Wild Area;
Cram-o-matic (dojo, watts); loja de Dynite Ore (entrada do Max Lair); BP na Battle Tower e no Restricted Sparring.
Melhorias do dojo (watts para a Honey): 5.000 cabeleireiro; 10.000 Rotomi (inativo); 20.000 Rotomi ativo; 30.000 Fresh Water;
40.000 Soda Pop; 50.000 Lemonade; 100.000 ingredientes (Fruit Bunch, Moomoo Cheese, Large Leek, Sausages); 200.000 Protein e Iron;
300.000 Calcium e Zinc; 400.000 HP Up e Carbos; 500.000 fundo de League Card; 800.000 League Card da Honey; 1.000.000 batalha com
a Honey; 3.280.000 Rare League Card da Honey.

## Mecânicas (para o guia de mecânicas, texto nosso)

- Dynamax: 3 turnos, só em estádios e na Wild Area (não em Spikemuth); Gigantamax em espécies específicas; Max Soup (3 Max Mushrooms) libera Gigantamax, Max Honey para o Urshifu.
- Max Raid Battles: 4 contra 1, pilares de luz; Wishing Piece ativa pilar; Dynamax Candy sobe o nível de Dynamax; watts como moeda.
- Wild Area: sub-áreas com clima próprio; encontros mudam com o clima; Pokémon de nível alto antes das insígnias (teto de captura).
- Encontros visíveis no cenário e no matinho; assobio (botão do analógico esquerdo) atrai Pokémon.
- Gym missions em cada estádio (Wooloos, pilares de água, pontos de captura, rampa, quiz da Opal, armadilhas, Team Yell, duplas com clima).
- Champion Cup no lugar da Elite Four (torneio).
- Rotom Bike com turbo; Water Mode para atravessar água.
- Flying Taxi (viagem rápida); Pokémon Box Link.
- Pokémon Camp e curry; Pokémon Nursery para ovos.
- Battle Tower: 11 ranks em 5 níveis (Beginner, Poké, Great, Ultra, Master Ball); perder só desce de rank; lv 50; sem repetição de espécie ou item; lendários permitidos; prêmios de BP, Rare Candy, Bottle Cap, Mint, Ability Capsule e Gold Bottle Cap.
- Hyper Training (Bottle Caps, lv 100) e função "judge" do PC (após vencer Leon na Battle Tower).
- Rare League Cards convidam treinadores para a Champion Cup.
- Isle of Armor: Pokémon segue o jogador; Restricted Sparring (3 Pokémon de um tipo, 5 batalhas, 2 curas; vencer os 18 tipos dá roupa do Mustard).
- Crown Tundra: Dynamax Adventures (aluguel de Pokémon, 4 batalhas, lendário no fim, fica com 1); pegadas (150) dos Swords of Justice.

## Pós-jogo (ordem de eventos)

1. Master Ball (Magnolia) → Hop na Slumbering Weald → Sordward/Shielbert.
2. Pokémon Dynamax nos estádios: Turffield → Hulbury → Motostoke → dupla no laboratório → Stow-on-Side → Ballonlea (Bede) → Circhester → Hammerlocke.
3. Energy Plant: irmão da versão → lendário oposto (vencer) → lendário da versão (captura) → Hop final na Slumbering Weald.
4. Battle Tower libera; Charmander do Leon; Rare League Cards (Ball Guy, Rose); Captain's Table; Marnie diária; Morimoto; Shiny Charm.
5. Update 1.1.0: Galarian Slowpoke. Isle of Armor: Klara/Avery → dojo (3 provas) → Kubfu → torre → Urshifu → Gigantamax Urshifu → Mustard final.
6. Crown Tundra: Peony → Max Lair (Suicune) → Freezington → Swords of Justice e Keldeo → 3 Legendary Clues (Calyrex, Regis/Regigigas, aves de Galar) em qualquer ordem → missão das Ultra Beasts → Galarian Star Tournament → Necrozma → Poipole → revanches finais.

## Dúvidas

- Leon (campeão e revanches): o time do inicial Sobble aparece na 1ª batalha (Rhyperior + Rillaboom), mas a coluna fica cortada no PDF das revanches da Champion Cup.
- Regirock e Regice: o detonado escreve "Registeel estará no lv.70" nas duas seções; o nível 70 é assumido para os dois.
- Calyrex na 1ª batalha: nível "???"; Eternatus 60 e Eternamax 60 como no detonado.
- Números das rotas (2 a 10) não aparecem no detonado, só "área com matinhos"; o mapeamento acima é por ordem.
- Klara/Avery na 2ª batalha: os dois times aparecem misturados no texto; a divisão (Klara: Whirlipede, Koffing; Avery: Kadabra, Woobat) é inferida.
- Nomes com grafia irregular no detonado: "Melody" = Melony, "Hattrene" = Hatterene, "Toxicorak" = Toxicroak, "Stounjourner", "Shielberd"/"Shilbert" = Shielbert, "Dynate Ores" = Dynite Ore, "Split-DecisiRuins".
- Times da 1ª Galarian Star Tournament (Milo+Nessa etc.) ficam numa tabela de imagem; só os times da tabela geral estão no texto.
- Lista do Max Lair identificada por imagens; conferir numa segunda fonte.
