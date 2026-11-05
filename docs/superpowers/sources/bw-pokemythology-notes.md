# Black/White — fatos extraídos do detonado da Pokémon Mythology

Fonte: https://www.pokemythology.net/detonados/detonado-black-white/ (PDF salvo em 05/11/2026, 95 páginas).
Só **fatos de jogo** (chefes, níveis, encontros, itens) no nosso formato. Nenhuma prosa foi copiada. Golpes dos
chefes **não** constam no detonado: deixar `moves: []` e `verified: false` para golpes; espécies e níveis são
`verified: true` (uma fonte) até o analista conferir numa segunda. Versões: `[Black]` / `[White]`.

## Ordem da história e chefes (para `bosses/bw.json`)

Variantes por inicial (Snivy / Tepig / Oshawott):
- Bianca leva o inicial **fraco** contra o seu (Snivy→Oshawott, Tepig→Snivy, Oshawott→Tepig) e, a partir de Castelia,
  o macaco **com vantagem** sobre o seu inicial (Snivy→Pansear, Tepig→Panpour, Oshawott→Pansage).
- Cheren leva o inicial **forte** contra o seu (Snivy→Tepig, Tepig→Oshawott, Oshawott→Snivy) e, a partir da Route 4, o
  macaco **do mesmo tipo** do seu inicial (Snivy→Pansage, Tepig→Pansear, Oshawott→Panpour).
- Ginásio de Striaton: luta o líder com vantagem sobre o seu inicial (Snivy→Chili/Pansear, Tepig→Cress/Panpour,
  Oshawott→Cilan/Pansage).

| Order | Id | Kind | Nome | Local | Time (espécie lv) | Cap |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | bianca-1 | rival | Bianca | quarto, Nuvema Town | inicial fraco 5 (Oshawott/Snivy/Tepig) | 5 |
| 2 | cheren-1 | rival | Cheren | quarto, Nuvema Town | inicial forte 5 (Tepig/Oshawott/Snivy) | 5 |
| 3 | n-1 | evil | N | Accumula Town | Purrloin 7 | 7 |
| 4 | bianca-2 | rival | Bianca | Route 2 (fim) | Lillipup 6, inicial fraco 7 | 7 |
| 5 | cheren-2 | rival | Cheren | Trainers' School, Striaton | inicial forte 8, Purrloin 8 | 8 |
| 6 | striaton-trio | gym | Chili / Cress / Cilan (Trio Badge) | Striaton City (restaurante) | Lillipup 12, Pansear/Panpour/Pansage 14 (por inicial: Snivy→Chili Pansear; Tepig→Cress Panpour; Oshawott→Cilan Pansage) | 14 |
| 7 | plasma-dreamyard | evil | 2 Team Plasma Grunts | Dreamyard | um Pokémon lv 10 cada (espécie não listada) | 10 |
| 8 | cheren-3 | rival | Cheren | Route 3 (após o Day Care) | inicial forte 14, Purrloin 12 | 14 |
| 9 | plasma-wellspring | evil | Team Plasma Grunts (1 simples + dupla com Cheren) | Wellspring Cave | não listado | — |
| 10 | n-2 | evil | N | Nacrene City (entrada do museu) | Pidove 13, Tympole 13, Timburr 13 | 13 |
| 11 | lenora | gym | Lenora (Basic Badge) | Nacrene City (museu/biblioteca) | Herdier 18, Watchog 20 | 20 |
| 12 | plasma-pinwheel | evil | Team Plasma Grunts (Gorm aparece) | Pinwheel Forest | não listado | — |
| 13 | burgh | gym | Burgh (Insect Badge) | Castelia City (Gym Street) | Whirlipede 21, Dwebble 21, Leavanny 23 | 23 |
| 14 | bianca-3 | rival | Bianca | saída norte de Castelia (Route 4) | Herdier 18, Munna 18, macaco com vantagem 18, inicial fraco evoluído 20 (Snivy: Pansear + Dewott; Tepig: Panpour + Servine; Oshawott: Pansage + Pignite) | 20 |
| 15 | cheren-4 | rival | Cheren | Route 4 | Pidove 20, Liepard 20, macaco do mesmo tipo 20, inicial forte evoluído 22 (Snivy: Pansage + Pignite; Tepig: Pansear + Dewott; Oshawott: Panpour + Servine) | 22 |
| 16 | n-3 | evil | N (roda-gigante) | parque de diversões, Nimbasa City | Sandile 22, Darumaka 22, Scraggy 22, Sigilyph 22 | 22 |
| 17 | elesa | gym | Elesa (Bolt Badge) | Nimbasa City (montanha-russa) | Emolga 25, Emolga 25, Zebstrika 27 | 27 |
| 18 | cheren-5 | rival | Cheren | Route 5 (oeste de Nimbasa) | Liepard 24, Tranquill 24, macaco do mesmo tipo 24, inicial forte evoluído 26 (Snivy: Pansage + Pignite; Tepig: Pansear + Dewott; Oshawott: Panpour + Servine) | 26 |
| 19 | alder-kids | other | Dupla com Cheren contra duas crianças (teste do Alder) | Route 5 | não listado | — |
| 20 | plasma-cold-storage | evil | Team Plasma (Zinzolin presente), dupla com Cheren | Cold Storage | não listado | — |
| 21 | clay | gym | Clay (Quake Badge; TM entregue depois, na Chargestone Cave) | Driftveil City (construtora) | Krokorok 29, Palpitoad 29, Excadrill 31 | 31 |
| 22 | bianca-4 | rival | Bianca (dá HM02 Fly) | Route 6 (oeste de Driftveil) | Herdier 26, Musharna 26, macaco com vantagem 26, inicial fraco evoluído 28 (Snivy: Pansear + Dewott; Tepig: Panpour + Servine; Oshawott: Pansage + Pignite) | 28 |
| 23 | n-4 | evil | N | Chargestone Cave (último andar) | Boldore 28, Joltik 28, Ferroseed 28, Klink 28 | 28 |
| 24 | skyla | gym | Skyla (Jet Badge) | Mistralton City (aeroporto) | Swoobat 33, Unfezant 33, Swanna 35 | 35 |
| 25 | cheren-6 | rival | Cheren (Alder dá HM03 Surf) | Route 7, perto da Twist Mountain | Unfezant 33, Liepard 33, Simisage/Simisear/Simipour 33, inicial forte evoluído 35 (Snivy: Simisage + Pignite; Tepig: Simisear + Dewott; Oshawott: Simipour + Servine) | 35 |
| 26 | brycen | gym | Brycen (Freeze Badge) | Icirrus City | Vanillish 37, Cryogonal 37, Beartic 39 | 39 |
| 27 | plasma-dragonspiral | evil | Giallo + 4 Grunts em sequência, sem cura | Dragonspiral Tower | não listado | — |
| 28 | bianca-5 | rival | Bianca | Route 8 (antes da Tubeline Bridge) | Stoutland 38, Musharna 38, macaco final com vantagem 38, inicial fraco final 40 (Snivy: Simisear + Samurott; Tepig: Simipour + Serperior; Oshawott: Simisage + Emboar) | 40 |
| 29 | drayden-iris | gym | Drayden [Black] / Iris [White] (Legend Badge) | Opelucid City | Fraxure 41, Druddigon 41, Haxorus 43 | 43 |
| 30 | cheren-7 | rival | Cheren (última batalha da história; Bianca dá Max Revive) | Route 10 (após a ponte) | Unfezant 43, Liepard 43, macaco final do mesmo tipo 43, inicial forte final 45 (Snivy: Simisage + Emboar; Tepig: Simisear + Samurott; Oshawott: Simipour + Serperior) | 45 |
| 31 | shauntal | elite | Shauntal (qualquer ordem) | Pokémon League | Cofagrigus 48, Golurk 48, Jellicent 48, Chandelure 50 | 50 |
| 32 | grimsley | elite | Grimsley | Pokémon League | Scrafty 48, Liepard 48, Krookodile 48, Bisharp 50 | 50 |
| 33 | caitlin | elite | Caitlin | Pokémon League | Reuniclus 48, Sigilyph 48, Musharna 48, Gothitelle 50 | 50 |
| 34 | marshal | elite | Marshal | Pokémon League | Throh 48, Sawk 48, Conkeldurr 48, Mienshao 50 | 50 |
| 35 | guardian | other | Reshiram [Black] / Zekrom [White] (captura obrigatória) | N's Castle (sala do trono) | Reshiram/Zekrom 50 | — (não conta para cap) |
| 36 | n-final | evil | N (final) | N's Castle | Zekrom [Black] / Reshiram [White] 52, Carracosta 50, Archeops 50, Vanilluxe 50, Klinklang 50, Zoroark 50 | 52 |
| 37 | ghetsis | evil | Ghetsis | N's Castle | Cofagrigus 52, Bisharp 52, Bouffalant 52, Eelektross 52, Seismitoad 52, Hydreigon 54 | 54 |
| 38 | sage-rood | evil | Rood (sábio; dá TM32) | praia da Route 17 | sem batalha listada | — |
| 39 | sage-zinzolin | evil | Zinzolin (sábio; dá TM01) | Cold Storage | sem batalha listada | — |
| 40 | sage-bronius | evil | Bronius + 2 Grunts (dá TM69) | Chargestone Cave (subsolo) | não listado | — |
| 41 | sage-gorm | evil | Gorm (sábio; dá TM75) | Dreamyard (andar inferior) | sem batalha listada | — |
| 42 | sage-ryoku | evil | Ryoku (sábio; dá TM04) | Relic Castle (salas novas) | sem batalha listada | — |
| 43 | sage-giallo | evil | Giallo (sábio; dá TM08) | Route 14 | sem batalha listada | — |
| 44 | bianca-rematch | rival | Bianca (sábados no fim da tarde, ~18:00–20:00) | laboratório da Juniper, Nuvema | Stoutland 63, Musharna 63, macaco final com vantagem 63, Mienshao 63, Chandelure 63, inicial fraco final 65 (Snivy: Simisear + Samurott; Tepig: Simipour + Serperior; Oshawott: Simisage + Emboar) | 65 |
| 45 | cheren-rematch | rival | Cheren (após vê-lo na Route 5) | Victory Road, gruta à direita da saída sul da Liga | Unfezant 65, Liepard 65, macaco final do mesmo tipo 65, Gigalith 65, Haxorus 65, inicial forte final 67 (Snivy: Simisage + Emboar; Tepig: Simisear + Samurott; Oshawott: Simipour + Serperior) | 67 |
| 46 | cynthia | other | Cynthia (1×/dia na primavera e no verão após a 1ª) | casa à esquerda do Pokémon Center, Undella Town | Spiritomb 75, Eelektross 75, Milotic 75, Braviary 75, Lucario 75, Garchomp 77 | 77 |
| 47 | morimoto | other | Morimoto (1×/dia) | prédio da Game Freak, Castelia Street | Liepard 75, Simisage 75, Simisear 75, Simipour 75, Swoobat 75, Zebstrika 77 | 77 |
| 48 | shauntal-2 | elite | Shauntal (revanche) | Pokémon League | Cofagrigus 71, Golurk 71, Froslass 71, Drifblim 71, Jellicent 71, Chandelure 73 | 73 |
| 49 | grimsley-2 | elite | Grimsley (revanche) | Pokémon League | Sharpedo 71, Scrafty 71, Liepard 71, Drapion 71, Krookodile 71, Bisharp 73 | 73 |
| 50 | caitlin-2 | elite | Caitlin (revanche) | Pokémon League | Reuniclus 71, Sigilyph 71, Bronzong 71, Musharna 71, Gothitelle 71, Metagross 73 | 73 |
| 51 | marshal-2 | elite | Marshal (revanche) | Pokémon League | Breloom 71, Throh 71, Sawk 71, Toxicroak 71, Mienshao 71, Conkeldurr 73 | 73 |
| 52 | alder | champion | Alder | Pokémon League | Accelgor 75, Bouffalant 75, Druddigon 75, Vanilluxe 75, Escavalier 75, Volcarona 77 | 77 |

Battle Subway (lv 50; chefes do metrô também no double e no multi):
- Single (linha verde / verde-escuro): Subway Boss Ingo — 21ª: Crustle 50, Klinklang 50, Garbodor 50; 49ª: Excadrill 50, Haxorus 50, Chandelure 50.
- Double (linha vermelha / rosa): Subway Boss Emmet — 21ª: Crustle 50, Klinklang 50, Garbodor 50, Durant 50; 49ª: Excadrill 50, Haxorus 50, Chandelure 50, Eelektross 50.
- Multi (linha laranja / amarela; parceiro Hilda se menino, Hilbert se menina): Ingo & Emmet — 21ª: Klinklang 50, Garbodor 50, Durant 50, Galvantula 50; 49ª: Excadrill 50, Haxorus 50, Eelektross 50, Archeops 50.

Outros oponentes recorrentes: The Riches (mansão de Undella; a família cresce um membro por dia até 6 dias: filho,
filha, avó, avô, mãe, pai; times não listados); treinador da roda-gigante de Nimbasa (1×/dia, muda por estação e
pelo gênero do jogador); jogadores do Big Stadium e do Small Court (esporte pelo dia da semana; pós-jogo lv 60+);
dançarinos de Castelia (3, cada um com um macaco; recompensa Amulet Coin); treinadoras das trocas de Nacrene
(Whimsicot/Lilligant) e de Driftveil (Cinccino) revanche no pós-jogo; Fisherman com 6 Magikarp lv 60 (Route 13).

## Áreas (para `areas/bw.json`, kind e ordem de liberação)

| id | nome | kind | libera |
| --- | --- | --- | --- |
| nuvema-town | Nuvema Town | city | início |
| route-1 | Route 1 | route | início |
| accumula-town | Accumula Town | city | após Route 1 |
| route-2 | Route 2 | route | após Accumula |
| striaton-city | Striaton City (Trainers' School, restaurante-ginásio) | city | após Route 2 |
| dreamyard | Dreamyard | zone | leste de Striaton; com HM01 Cut; parte nova no pós-jogo |
| route-3 | Route 3 (Day Care, escolinha) | route | após Trio Badge |
| wellspring-cave | Wellspring Cave | dungeon | Route 3; andar inferior com Surf e Flash |
| nacrene-city | Nacrene City (museu/ginásio) | city | após Route 3 |
| pinwheel-forest | Pinwheel Forest (área externa e floresta) | dungeon | após Basic Badge |
| skyarrow-bridge | Skyarrow Bridge | route | após Pinwheel Forest |
| castelia-city | Castelia City (Main, Castelia, Mode, Narrow e Gym Street; Battle Company; píeres; Game Freak) | city | após Skyarrow Bridge |
| route-4 | Route 4 | route | após Insect Badge |
| desert-resort | Desert Resort | zone | Route 4 |
| relic-castle | Relic Castle | dungeon | Desert Resort; fundo após Dragonspiral; salas novas e Volcarona no pós-jogo |
| nimbasa-city | Nimbasa City (Musical, Big Stadium, Small Court, parque, Battle Subway, Battle Institute) | city | após Route 4 |
| anville-town | Anville Town | city | Battle Subway, linha marrom |
| route-16 | Route 16 | route | Nimbasa leste |
| lostlorn-forest | Lostlorn Forest | dungeon | Route 16 (Strength) |
| route-5 | Route 5 | route | após Bolt Badge |
| driftveil-drawbridge | Driftveil Drawbridge | route | após Route 5 |
| driftveil-city | Driftveil City (Driftveil Market) | city | após a ponte |
| cold-storage | Cold Storage | building | sul de Driftveil |
| route-6 | Route 6 (Season Research Lab) | route | após Quake Badge |
| chargestone-cave | Chargestone Cave | dungeon | Route 6 (Clay remove a teia) |
| mistralton-city | Mistralton City (aeroporto) | city | após Chargestone Cave |
| route-7 | Route 7 | route | Mistralton norte |
| celestial-tower | Celestial Tower | building | Route 7 |
| mistralton-cave | Mistralton Cave | dungeon | Surf + Flash (Route 6) |
| route-17 | Route 17 (mar com correntezas; nº não citado) | sea | Surf (Route 1, oeste) |
| route-18 | Route 18 (casa do ovo de Larvesta; nº não citado) | route | Surf |
| p2-laboratory | P2 Laboratory | building | correntezas da Route 17 (Genesect de evento) |
| twist-mountain | Twist Mountain | dungeon | após Jet Badge + Surf (Route 7) |
| icirrus-city | Icirrus City | city | após Twist Mountain |
| dragonspiral-tower | Dragonspiral Tower | building | após Freeze Badge (Strength) |
| route-8 | Route 8 | route | Icirrus leste |
| moor-of-icirrus | Moor of Icirrus | zone | Route 8 norte |
| tubeline-bridge | Tubeline Bridge | route | após Bianca na Route 8 |
| route-9 | Route 9 | route | após Tubeline Bridge |
| shopping-mall-nine | Shopping Mall Nine | building | Route 9 |
| opelucid-city | Opelucid City (versões diferentes no visual) | city | após Route 9 |
| route-10 | Route 10 | route | após Legend Badge |
| badge-check-gates | Portões das insígnias (Trio, Basic, Insect, Bolt, Quake, Jet, Freeze, Legend) | route | Route 10 |
| victory-road | Victory Road | dungeon | após os portões |
| pokemon-league | Pokémon League | building | após Victory Road |
| ns-castle | N's Castle | building | após a Elite Four |
| challengers-cave | Challenger's Cave | dungeon | pós-jogo (Route 9 sul, Flash) |
| route-11 | Route 11 | route | pós-jogo (Opelucid leste; Waterfall) |
| village-bridge | Village Bridge | route | após Route 11 |
| route-12 | Route 12 | route | após Village Bridge |
| lacunosa-town | Lacunosa Town | city | após Route 12 |
| route-13 | Route 13 | route | Lacunosa leste |
| giant-chasm | Giant Chasm | dungeon | Route 13 (Surf, Strength) |
| undella-town | Undella Town (casa da Cynthia, mansão The Riches) | city | Route 13 sul |
| undella-bay | Undella Bay | sea | Surf |
| abyssal-ruins | Abyssal Ruins | dungeon | Dive em Undella Bay (Flash, Strength; 500 passos) |
| route-14 | Route 14 | route | Undella sul (Waterfall) |
| abundant-shrine | Abundant Shrine | zone | Route 14 |
| black-city | Black City [Black] / White Forest [White] | zone | após Route 14 |
| route-15 | Route 15 (PokéTransfer Lab, trailer do Rotom) | route | após Black City/White Forest |
| marvelous-bridge | Marvelous Bridge | route | após Route 15 |
| royal-unova | Royal Unova (cruzeiro) | building | pós-jogo, píer ao sul do Pokémon Center de Castelia |
| liberty-garden | Liberty Garden | zone | Liberty Pass (evento), píer 1 de Castelia |

## Encontros (para `encounters/bw.json`; method `walk` salvo indicação; "grama que mexe" = shaking grass)

- Route 1: Patrat, Lillipup; Audino (grama que mexe, em todas as rotas). Pós-jogo (`fish`, água que borbulha, área oeste): Feebas.
- Route 2: Purrloin, Patrat, Lillipup.
- Dreamyard: Munna; Musharna (5%, grama que mexe). Pós-jogo (grama escura): Raticate, Venomoth, Ledian, Ariados, Kricketune. Musharna lv 50 (`fixed`, só sexta; volta na sexta seguinte).
- Route 3: Pidove, Blitzle; grama escura (batalhas duplas, níveis mais altos).
- Wellspring Cave: Roggenrola, Woobat; Drilbur (nuvem de poeira).
- Pinwheel Forest (área externa): Tympole, Timburr, Sawk [Black] / Throh [White]; o da versão oposta 5% na grama que mexe.
- Pinwheel Forest: Sewaddle, Venipede, Cottonee [Black], Petilil [White]; grama que mexe: Audino, Pansage, Pansear, Panpour, Whimsicot [Black] / Lilligant [White].
- Route 4 (areia funda): Sandile, Darumaka, Scraggy.
- Desert Resort: Sandile, Darumaka, Scraggy, Maractus, Dwebble, Sigilyph. Darmanitan (estátuas, `fixed`, com RageCandyBar).
- Relic Castle: Yamask; andares de baixo: Krokorok, Cofagrigus; pós-jogo: Sandslash, Onix; sala do Volcarona: Claydol.
- Route 16: Minccino, Trubbish, Liepard, Gothita [Black], Solosis [White]; grama que mexe: Emolga, Cinccino.
- Lostlorn Forest: Swadloon, Tranquill; grama que mexe: Unfezant, Leavanny, Pansage, Pansear, Panpour, Emolga, Whimsicot [Black], Lilligant [White], Audino.
- Driftveil Drawbridge: Ducklett (sombras no chão; também dão penas).
- Cold Storage (área externa): Vanillite.
- Route 6: Deerling (forma por estação), Karrablast, Foongus (também disfarçado de item); Tranquill (Vanillish no inverno).
- Chargestone Cave: Boldore, Joltik, Klink, Ferroseed, Tynamo (raro).
- Driftveil (mar, `surf`): Frillish; água que borbulha: Alomomola, Jellicent (raro); águas calmas: Basculin Red-Striped [Black] / Blue-Striped [White] (o oposto na água que borbulha). Pós-jogo `fish`: Krabby, Chinchou; Luvdisc (borbulha).
- Mistralton Cave: Axew.
- Route 7: Zebstrika; Cubchoo (inverno).
- Celestial Tower: Litwick, Elgyem.
- Route 18 (grama escura, lv 32–35): Crustle.
- Route 17 (pós-jogo, `fish`): Finneon, Horsea.
- Twist Mountain: Cubchoo e Cryogonal (mais fáceis no inverno).
- Icirrus City (poças, fora do inverno): Stunfisk, Shelmet, Palpitoad.
- Dragonspiral Tower (fora, grama escura): Sawsbuck; Vanillish e Beartic (inverno). Dentro: Golett, Mienfoo, Druddigon.
- Route 9: Pawniard, Garbodor, Gothorita [Black], Duosion [White]; grama que mexe (raro): Gothitelle [Black] / Reuniclus [White].
- Route 10: Bouffalant, Vullaby [Black], Rufflet [White], Amoonguss.
- Victory Road: fora: Fraxure, Heatmor; caverna: Durant, Deino; Excadrill (poeira).
- Challenger's Cave: Graveler, Lickitung, Sableye, Mawile, Riolu (sorte); `fish`: Poliwag; borbulha: Poliwhirl.
- Route 11: Gligar, Golduck, Zangoose, Seviper; grama escura: Mandibuzz [Black] / Braviary [White]; grama que mexe: Gliscor (raro); `surf`: Buizel; `fish`: Goldeen.
- Village Bridge: Bibarel; `surf` borbulha: Lapras (muito raro); `fish`: Carvanha.
- Route 12: Combee, Sunkern, Rapidash, Cherrim, Kakuna [Black], Metapod [White], Heracross, Pinsir, Dunsparce; grama que mexe: Beedrill [Black], Butterfree [White], Vespiquen, Sunflora.
- Route 13: Tangela, Swellow, Golbat, Lunatone, Solrock, Drifblim, Absol; grama que mexe: Crobat, Tangrowth; `surf`: Staryu, Wingull, Pelipper; borbulha: Corsola, Starmie; `fish`: Shellder.
- Giant Chasm (caverna): Piloswine, Sneasel, Jynx, Delibird; `surf`: Seel. Floresta: Ditto, Clefairy, Metang; grama que mexe: Clefable, Mamoswine, Metagross, Audino (os de nível mais alto).
- Undella Bay: `surf`: Mantyke, Spheal (inverno); borbulha: Wailmer, Wailord, Mantine, Sealeo e Walrein (inverno); `fish`: Remoraid; borbulha: Octillery, Cloyster.
- Route 14: Jigglypuff, Tropius, Altaria, Shuckle, Beheeyem; grama escura: Mienshao; grama que mexe: Wigglytuff.
- Abundant Shrine: Vulpix, Noctowl, Stantler, Murkrow, Misdreavus, Chimecho, Bronzong; grama que mexe: Ninetales, Mismagius, Honchkrow; `surf`: Slowpoke.
- White Forest [White]: Pokémon de fora de Unova, conforme visitas pelo Entralink (espécies não listadas).
- Route 15: Marowak, Fearow, Kangaskhan, Pupitar; grama que mexe: Tyranitar (raro).
- Marvelous Bridge: Swanna (sombras).
- Placas dos portões (pós-National Dex): `outbreak` diário de um Pokémon de Kanto/Johto/Hoenn/Sinnoh numa rota.
- Lendários (`method: fixed` salvo indicação):
  - Cobalion lv 42: fundo da Mistralton Cave (Surf, Flash).
  - Virizion lv 42: Pinwheel Forest, trecho a nordeste (após Cobalion).
  - Terrakion lv 42: Victory Road, gruta atrás de pedra de Strength (após Cobalion).
  - Tornadus [Black] / Thundurus [White] lv 40: vaga pelas rotas após o evento da Route 7 (tempestade indica a rota; foge no 1º turno; dano persiste).
  - Reshiram [Black] / Zekrom [White] lv 50: N's Castle (história).
  - Volcarona lv 70: Relic Castle, salas novas (pós-jogo; não volta se derrotado).
  - Kyurem lv 75: Giant Chasm (caverna após a nevasca).
  - Landorus lv 70: Abundant Shrine, com Tornadus e Thundurus na equipe.
  - Victini lv 15: Liberty Garden (Liberty Pass de evento).
  - Zoroark lv 25 disfarçado de cão lendário: trailer da Lostlorn Forest, com cão lendário shiny de evento.

## Presentes, trocas in-game, fósseis, ovos e iniciais (`method: gift` / `trade`)

- Inicial: Snivy/Tepig/Oshawott lv 5 (Nuvema).
- Macaco lv 10 (Dreamyard): Snivy→Panpour, Tepig→Pansage, Oshawott→Pansear.
- Fóssil (Relic Castle, escolha um): Cover Fossil → Tirtouga, Plume Fossil → Archen; revividos lv 25 no museu de Nacrene.
- Fósseis diários de Kanto, Hoenn e Sinnoh (pessoa na Twist Mountain, pós-jogo); revividos lv 25 em Nacrene.
- Ovo de Larvesta (casa da Route 18; nasce lv 1 após 10455 passos; evolui no lv 59).
- Trocas: Cottonee [Black] ↔ Petilil [White] (Nacrene, casa à direita do Pokémon Center); Minccino → Basculin (Driftveil); Ditto → Rotom (trailer da Route 15).
- Compra: Magikarp por $500 (Marvelous Bridge; único jeito de ter Magikarp/Gyarados).
- Zorua só registrado na Pokédex por foto (Castelia Street).

## TMs, HMs e itens-chave (localização)

- HMs: HM01 Cut (Fennel, Striaton); HM02 Fly (Bianca, Route 6); HM03 Surf (Alder, Route 7); HM04 Strength (casa de Nimbasa; uso após Bolt Badge); HM05 Waterfall (Route 18); HM06 Dive (menina em Undella, pós-jogo).
- TMs: TM94 Rock Smash (garota fora da Pinwheel Forest); TM70 Flash (Narrow Street, Castelia); TM44 (elevador da Castelia Street); TM28 Dig (casa da Route 4); TM49 (Musical Hall, Nimbasa); TM54 (Juniper, 30 vistos); TM17 (Juniper, 60 vistos); TM10 (Juniper, 100 vistos); TM do Clay (na Chargestone Cave; número não citado); TM40 (sob a pista do aeroporto de Mistralton); TM24 Thunderbolt (Route 18); TM31 Brick Break (senhora no Pokémon Center de Icirrus); TM32 (Rood); TM01 (Zinzolin); TM69 (Bronius); TM75 (Gorm); TM04 (Ryoku); TM26 Earthquake (Relic Castle, topo das salas novas); TM13 Ice Beam (Giant Chasm); TM89 (Route 13, 3 cartas do Wingull); TM08 (Giallo); TM35 Flamethrower (Abundant Shrine). TMs não se consomem.
- Itens-chave: Xtransceiver (mãe); Town Map (mãe); Pokédex (Juniper); Running Shoes (mãe, Route 2); C-Gear (Fennel); Pal Pad (Amanita); Dowsing MCHN (Bianca, Nacrene); Vs. Recorder (Battle Subway); Bicycle (vovô do Day Care, Nimbasa); Prop Case (Musical Hall); Lucky Egg (Juniper, Chargestone Cave); Old Gateau (Cedric, Dragonspiral); Light Stone [Black] / Dark Stone [White] (Lenora); Master Ball (Juniper, Opelucid); Super Rod (Looker, pós-jogo); National Dex (Cedric); Adamant Orb, Lustrous Orb, Griseous Orb (Tríade das Sombras, Marvelous Bridge); Prism Scale (Pokémon Center de Undella); Gracidea (Lacunosa, com Shaymin de evento); 2 Plates de Arceus (casa ao norte da casa dos golpes finais, Route 13) e os demais nas Abyssal Ruins; Drives (P2 Laboratory com Genesect: Burn e Shock [Black], Douse e Chill [White]); Liberty Pass (evento).
- Outros itens citados: Potion (Route 1); Poké Ball (Accumula); Dusk Ball e Great Ball (Striaton); Full Heal (quiz da escola); 3 Oran Berry (Cheren); 3 Heal Ball (garotinha, Route 3); 3 Chesto Berry (Cheren, Nacrene); Miracle Seed / Charcoal / Mystic Water (escolha, Nacrene; Miracle Seed também na Pinwheel Forest); Soda Pop (lanchonete de Nacrene, quarta); Moon Stone (Lenora); Star Piece (pedra da Pinwheel com Pokémon lutador, diário); Quick Claw (Skyarrow Bridge); pedra do macaco (píer 1 de Castelia); Lemonade (bar da Narrow Street); Amulet Coin (dançarinos); Ultra Balls (Juniper, Nimbasa); Sun Stone e Soothe Bell (Nimbasa); Repeat Ball, Shell Bell (50 vistos) e Heart Scale (golpe do dia) em Driftveil; Leaf Stone (pesquisador da Route 6, todas as formas de Deerling); Shiny Stone (casa da Route 6); Nuggets (Chargestone Cave); RageCandyBar (casa do ex-Rocket em Icirrus, só no inverno; também Juniper no pós-jogo); Max Revive (Bianca, Route 10); Leftovers (lixo da Village Bridge); Dive Ball (Pokémon pescado pedido, Village Bridge); item diário (homem de óculos, Route 13: Big Pearl, flautas, DeepSeaScale/Tooth, Dragon Scale, Dubious Disc, Electirizer, Heart Scale, King's Rock, Lucky Egg, Lucky Punch, Magmarizer, Metal Coat, Metal Powder, Protector, Razor Claw, Razor Fang, Reaper Cloth, Shoal Salt, Shoal Shell, Stick, Thick Club, Up-Grade); Big Mushroom (jogo dos Patrat, Marvelous Bridge, diário); Rare Candy e item semanal (Anville Town); Fresh Water (porteiro de cada ginásio).
- Lojas: PokéMart dentro do Pokémon Center; Nimbasa vende TMs; Driftveil vende Dusk Balls; Icirrus vende TM Blizzard, Thunder e Fire Blast e tem o comprador que paga o dobro; Driftveil Market (ervas; incensos no pós-jogo); Shopping Mall Nine; Black City [Black] (itens); Battle Subway (itens por BP); Casteliacone (Mode Street, terças, fora do inverno).
- Tutores: Driftveil (Grass/Fire/Water Pledge para iniciais); Mistralton (Move Relearner por Heart Scale e Move Deleter); Route 13 (Frenzy Plant / Blast Burn / Hydro Cannon para inicial final); casa do Drayden em Opelucid (Draco Meteor para dragões); Moor of Icirrus (Keldeo de evento aprende Secret Sword com o trio); bar da Narrow Street (Meloetta de evento aprende Relic Song).

## Mecânicas (para o guia de mecânicas, texto nosso)

- Estações por mês: primavera jan/mai/set, verão fev/jun/out, outono mar/jul/nov, inverno abr/ago/dez; mudam encontros, acesso a áreas (neve) e formas (Deerling/Sawsbuck).
- Grama que mexe, nuvens de poeira, sombras nas pontes e água que borbulha geram encontros raros ou itens.
- Grama escura: batalhas duplas selvagens (só dá para capturar com um em campo).
- Batalhas 3×3: Rotation [Black] e Triple [White]; uma treinadora na Route 15 faz o estilo da versão oposta.
- TMs reutilizáveis (e mais caros nas lojas); Rock Smash e Flash viraram TM.
- Médicos e enfermeiras curam após a batalha; Pokémon Rangers dão Berry.
- Battle Subway (8 linhas; 7 rounds; níveis 50; Super liberado após vencer o chefe; empate não elimina); Battle Institute (5 batalhas, nota por desempenho; após a Elite Four). Proibidos no Subway: Mewtwo, Mew, Lugia, Ho-Oh, Celebi, Kyogre, Groudon, Rayquaza, Jirachi, Deoxys, Dialga, Palkia, Giratina, Phione, Manaphy, Darkrai, Shaymin, Arceus, Victini, Reshiram, Zekrom, Kyurem, Keldeo, Meloetta, Genesect.
- Pokémon Musical (Prop Case), C-Gear e Entralink (povoa Black City / White Forest com o jogo de um amigo da versão oposta).
- Elite Four em qualquer ordem; campeão só no pós-jogo (N vence Alder na história).
- PokéTransfer (Route 15): traz Pokémon de D/P/Pt/HG/SS sem item nem HM, por minijogo, sem limite diário, sem volta.
- Royal Unova (Castelia, $1000; horários: primavera e outono 17:00–19:59, verão 19:00–20:59, inverno 16:00–18:59; prêmio por dia: seg/qua/sex Lava Cookie, ter Berry Juice, qui Old Gateau, sáb RageCandyBar, dom Rare Candy).
- Abyssal Ruins: 500 passos por mergulho; artefatos vendidos ao carateca da mansão The Riches.
- Pedras de evolução: Moss Rock (Leafeon) na Pinwheel Forest, Ice Rock (Glaceon) na Twist Mountain; Magneton e Nosepass evoluem subindo de nível na Chargestone Cave.
- Opelucid moderna [Black] / natural [White]; líder Drayden [Black] / Iris [White].
- Rotom muda de forma no Shopping Mall Nine; meteorito de Nacrene muda Deoxys.
- Castelia Street: massagem diária (felicidade); galeria da Mode Street dá Berry pelo tipo pedido; Ferris wheel diária em Nimbasa.

## Pós-jogo (ordem)

1. Vencer N e Ghetsis no N's Castle → créditos → Looker em casa (Super Rod) → National Dex (Cedric).
2. Caçar os 6 sábios (qualquer ordem): Rood (Route 17), Zinzolin (Cold Storage), Bronius (Chargestone Cave), Gorm (Dreamyard; Musharna de sexta), Ryoku (Relic Castle; Volcarona e TM26), Giallo (Route 14).
3. Royal Unova e Challenger's Cave; lado leste: Route 11, Village Bridge, Route 12, Lacunosa, Route 13, Giant Chasm (Kyurem), Undella (HM06 Dive, Abyssal Ruins), Route 14 (Abundant Shrine, Landorus), Black City/White Forest, Route 15 (PokéTransfer), Marvelous Bridge (orbs), volta a Nimbasa (Battle Institute, Battle Subway).
4. Treinadores fortes: Bianca (sábados), Cheren (Victory Road), Cynthia, Morimoto, Elite Four reforçada e Alder (campeão, Hall of Fame).
5. Eventos: Victini (Liberty Pass), Keldeo, Meloetta, Genesect, cães shiny/Zoroark, Shaymin.

## Dúvidas

- Lenora, Burgh, Elesa, Clay, Skyla, Brycen e Drayden/Iris: o detonado não diz o número nem o nome do TM de cada um.
- Grunts do Dreamyard: "um Pokémon lv 10 cada" sem espécie; demais grunts e sábios sem time no texto.
- Números de rota 4–18: o detonado só cita "rota 16" e "rota 7" pelo número; os demais vêm dos nomes do jogo (conferir).
- Treinadoras e treinadores pós-jogo do Big Stadium/Small Court e The Riches: sem times listados.
- Times do parceiro Hilbert/Hilda no multi do Battle Subway estão em imagem no PDF (pág. 88).
- Mapas das Abyssal Ruins e locais exatos dos Plates estão em imagem no PDF (pág. 80).
