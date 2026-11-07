# Crystal — fatos extraídos do detonado da Pokémon Mythology

Fonte: https://www.pokemythology.net/detonados/detonado-crystal/ (PDF "Detonado Crystal – Pokémon Mythology", salvo em
05/11/2026, 42 páginas). Só **fatos de jogo** (chefes, níveis, encontros, itens) no nosso formato. Nenhuma prosa foi
copiada. Golpes dos chefes **não** constam no detonado: deixar `moves: []` e `verified: false` para golpes; espécies e
níveis são `verified: true` (uma fonte) até o analista conferir numa segunda.

Inicial do rival: o que tem vantagem sobre o seu (você Chikorita → rival Cyndaquil; Cyndaquil → Totodile; Totodile →
Chikorita). O detonado escreve só "Inicial" com o nível; o estágio de evolução não aparece no texto.

## Ordem da história e chefes (para `bosses/c.json`)

| Order | Id | Kind | Nome | Local | Time (espécie lv) | Cap |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | rival-1 | rival | Rival (Silver) | Route 29/Cherrygrove, volta do Mr. Pokémon | inicial do rival (nível não listado; o detonado sugere lv 9 para você) | — |
| 2 | sprout-elder | other | Elder Li (monge da Sprout Tower) | Sprout Tower, topo (Violet City) | time não listado (dá HM05 Flash) | — |
| 3 | falkner | gym | Falkner (Zephyr Badge) | Violet City Gym | Pidgey 7, Pidgeotto 9 | 9 |
| 4 | rocket-slowpoke-well | evil | Team Rocket (Slowpoke Well) | Slowpoke Well, Azalea Town | time não listado | — |
| 5 | bugsy | gym | Bugsy (Hive Badge) | Azalea Town Gym | Metapod 14, Scyther 16, Kakuna 14 | 16 |
| 6 | rival-2 | rival | Rival | Saída de Azalea (para a Ilex Forest) | Gastly 12, Zubat 14, inicial do rival 16 | 16 |
| 7 | whitney | gym | Whitney (Plain Badge) | Goldenrod City Gym | Clefairy 18, Miltank 20 | 20 |
| 8 | sudowoodo | other | Sudowoodo (fixo, uma vez só) | Route 36 | nível não listado | — |
| 9 | rival-3 | rival | Rival | Burned Tower, Ecruteak City | Haunter 20, Zubat 20, Magnemite 18, inicial do rival 22 | 22 |
| 10 | morty | gym | Morty (Fog Badge) | Ecruteak City Gym | Gastly 21, Haunter 21, Gengar 25, Haunter 23 | 25 |
| 11 | eusine | other | Eusine | Cianwood City (norte) | Drowzee 23, Electrode 25, Haunter 23 | 25 |
| 12 | chuck | gym | Chuck ("Chucky" no detonado; Storm Badge) | Cianwood City Gym | Primeape 27, Poliwrath 30 | 30 |
| 13 | jasmine | gym | Jasmine (Mineral Badge) | Olivine City Gym (depois do SecretPotion) | Magnemite 30, Magnemite 30, Steelix 35 | 35 |
| 14 | red-gyarados | other | Red Gyarados (shiny fixo) | Lake of Rage | nível não listado | — |
| 15 | rocket-hideout | evil | Team Rocket (esconderijo; Rocket Executives em dupla com Lance; 3 Electrode) | Team Rocket Hideout, Mahogany Town | time não listado | — |
| 16 | pryce | gym | Pryce (Glacier Badge) | Mahogany Town Gym | Seel 27, Dewgong 29, Piloswine 31 | 31 |
| 17 | rocket-radio-tower | evil | Rocket Executive (topo da Radio Tower; dá Basement Key) | Goldenrod Radio Tower | time não listado | — |
| 18 | rival-4 | rival | Rival | Goldenrod Underground (porta da Basement Key) | Golbat 30, Magnemite 28, Sneasel 32, Haunter 30, inicial do rival 32 | 32 |
| 19 | rocket-radio-tower-2 | evil | Team Rocket (restantes, com a Card Key) | Goldenrod Radio Tower | time não listado | — |
| 20 | clair | gym | Clair (Rising Badge) | Blackthorn City Gym | Dragonair 37, Dragonair 37, Dragonair 37, Kingdra 40 | 40 |
| 21 | rival-5 | rival | Rival | Victory Road (fim da caverna) | Sneasel 34, Kadabra 35, Golbat 36, Magneton 35, Haunter 35 ("Hunter" no texto), inicial do rival 38 | 38 |
| 22 | will | elite | Will | Indigo Plateau | Xatu 40, Exeggutor 41, Jynx 41, Slowbro 41, Xatu 41 | 41 |
| 23 | koga | elite | Koga | Indigo Plateau | Ariados 40, Forretress 43, Muk 42, Venomoth 41, Crobat 44 | 44 |
| 24 | bruno | elite | Bruno | Indigo Plateau | Hitmontop 42, Hitmonchan 42, Hitmonlee 42, Onix 43, Machamp 46 | 46 |
| 25 | karen | elite | Karen | Indigo Plateau | Umbreon 42, Murkrow 44, Vileplume 42, Gengar 45, Houndoom 47 | 47 |
| 26 | lance | champion | Lance | Indigo Plateau | Gyarados 44, Dragonite 47, Dragonite 47, Aerodactyl 46, Charizard 46, Dragonite 50 | 50 |
| 27 | lt-surge | gym | Lt. Surge (Kanto, pós-jogo) | Vermilion City Gym | Raichu 44, Magneton 40, Electrode 40, Electrode 40, Electabuzz 46 | 46 |
| 28 | sabrina | gym | Sabrina | Saffron City Gym | Espeon 46, Mr. Mime 46, Alakazam 48 | 48 |
| 29 | erika | gym | Erika | Celadon City Gym | Tangela 44, Jumpluff 40, Victreebel 40, Bellossom 40 | 44 |
| 30 | janine | gym | Janine | Fuchsia City Gym | Crobat 36, Ariados 33, Weezing 36, Weezing 36, Venomoth 39 | 39 |
| 31 | rocket-cerulean | evil | Rocket Grunt (ladrão da peça da Power Plant) | Route 24/25, norte de Cerulean | time não listado | — |
| 32 | misty | gym | Misty | Cerulean City Gym | Golduck 42, Quagsire 42, Lapras 44, Starmie 47 | 47 |
| 33 | snorlax | other | Snorlax (fixo, uma vez só; acorda com o rádio PokéFlute) | Route 11, entrada do Diglett's Cave | Snorlax 50 | — |
| 34 | brock | gym | Brock | Pewter City Gym | Graveler 41, Rhyhorn 41, Kabutops 42, Omastar 42, Onix 44 | 44 |
| 35 | rival-6 | rival | Rival (última batalha) | Mt. Moon | Sneasel 41, Magneton 41, Golbat 42, Alakazam 43, Gengar 43, inicial do rival 45 | 45 |
| 36 | blaine | gym | Blaine | Seafoam Islands (ginásio provisório) | Magcargo 45, Magmar 45, Rapidash 50 | 50 |
| 37 | blue | gym | Blue | Viridian City Gym | Pidgeot 56, Alakazam 54, Rhydon 56, Exeggutor 58, Gyarados 58, Arcanine 58 | 58 |
| 38 | red | other | Red (treinador final) | Mt. Silver, fim da caverna | Pikachu 81, Espeon 73, Snorlax 75, Blastoise 77, Venusaur 77, Charizard 77 | 81 |
| — | karate-king | other | Karate King (líder do Fighting Dojo; opcional) | Mt. Mortar (entrada H) | Hitmonlee 34, Hitmonchan 34 | 34 |

Total de insígnias: 16 (8 de Johto + 8 de Kanto). Recompensas de TM dos líderes: Falkner TM31 Mud-Slap; Bugsy TM49 Fury
Cutter; Whitney TM45 Attract; Morty TM30 Shadow Ball; Chuck TM01 DynamicPunch; Jasmine TM23 Iron Tail; Pryce TM16 Icy
Wind; Clair TM24 DragonBreath (insígnia só depois do teste do Dragon's Den).

## Áreas (para `areas/c.json`, kind e ordem de liberação)

| id | Nome | Kind | Quando libera |
| --- | --- | --- | --- |
| new-bark-town | New Bark Town | city | início |
| route-29 | Route 29 | route | início |
| cherrygrove-city | Cherrygrove City | city | início |
| route-30 | Route 30 | route | início |
| route-31 | Route 31 | route | depois da Pokédex (Mr. Pokémon) |
| dark-cave | Dark Cave | dungeon | entrada na Route 31 (sem Flash não vai longe); exploração completa com Surf, Rock Smash e Flash |
| violet-city | Violet City | city | Route 31 |
| sprout-tower | Sprout Tower | building | Violet City |
| ruins-of-alph | Ruins of Alph | dungeon | sul de Violet (parcial); demais câmaras com Surf/Strength |
| route-32 | Route 32 | route | depois da Zephyr Badge |
| union-cave | Union Cave | dungeon | Route 32; andar inferior com Surf; Lapras às sextas |
| route-33 | Route 33 | route | Union Cave |
| azalea-town | Azalea Town | city | Route 33 |
| slowpoke-well | Slowpoke Well | dungeon | Azalea; parte interna com Strength |
| ilex-forest | Ilex Forest | route | depois do rival 2; atravessar com HM01 Cut |
| route-34 | Route 34 | route | Ilex Forest (Day-Care aqui) |
| goldenrod-city | Goldenrod City | city | Route 34 |
| goldenrod-underground | Goldenrod Underground | building | Goldenrod; ala trancada com Basement Key |
| goldenrod-radio-tower | Goldenrod Radio Tower | building | Goldenrod |
| goldenrod-dept-store | Goldenrod Dept. Store | building | Goldenrod |
| route-35 | Route 35 | route | Goldenrod |
| national-park | National Park | route | Route 35/36 (Bug-Catching Contest ter/qui/sáb) |
| route-36 | Route 36 | route | depois do Squirtbottle (Sudowoodo) |
| route-37 | Route 37 | route | depois do Sudowoodo |
| ecruteak-city | Ecruteak City | city | Route 37 |
| burned-tower | Burned Tower | building | Ecruteak |
| ecruteak-dance-theater | Ecruteak Dance Theater | building | Ecruteak (HM03 Surf) |
| route-38 | Route 38 | route | depois da Fog Badge |
| route-39 | Route 39 | route | Route 38 (Moomoo Farm) |
| olivine-city | Olivine City | city | Route 39 |
| olivine-lighthouse | Olivine Lighthouse | building | Olivine |
| battle-tower | Battle Tower | building | perto de Olivine |
| route-40 | Route 40 | sea | Surf, a partir de Olivine |
| route-41 | Route 41 | sea | Surf |
| whirl-islands | Whirl Islands | dungeon | HM06 Whirlpool (depois da Glacier Badge); Lugia com Silver Wing |
| cianwood-city | Cianwood City | city | Route 41 |
| route-42 | Route 42 | route | leste de Ecruteak |
| mt-mortar | Mt. Mortar | dungeon | Route 42; interior com Surf/Waterfall/Strength |
| mahogany-town | Mahogany Town | city | Route 42 |
| route-43 | Route 43 | route | norte de Mahogany (o detonado chama de "Route 33") |
| lake-of-rage | Lake of Rage | route | Route 43 |
| team-rocket-hideout | Team Rocket Hideout | building | depois do Red Gyarados e do Lance |
| route-44 | Route 44 | route | depois da Radio Tower |
| ice-path | Ice Path | dungeon | Route 44; Strength |
| blackthorn-city | Blackthorn City | city | Ice Path |
| dragons-den | Dragon's Den | dungeon | atrás do ginásio; Surf e Whirlpool |
| route-45 | Route 45 | route | sul de Blackthorn (descida sem volta) |
| route-46 | Route 46 | route | sul da Route 45 |
| tin-tower | Tin Tower | building | depois da Radio Tower (Clear Bell); Ho-Oh com os 3 cães capturados |
| route-27 | Route 27 | route | leste de New Bark, com Surf (8 insígnias) |
| tohjo-falls | Tohjo Falls | dungeon | Route 27; Waterfall |
| route-26 | Route 26 | route | Tohjo Falls |
| victory-road | Victory Road | dungeon | 8 insígnias |
| indigo-plateau | Indigo Plateau | building | Victory Road |
| ss-aqua | S.S. Aqua | building | pós-jogo (S.S. Ticket) |
| vermilion-city | Vermilion City | city | pós-jogo |
| route-6 | Route 6 | route | pós-jogo |
| saffron-city | Saffron City | city | pós-jogo (Magnet Train com Pass) |
| route-7 | Route 7 | route | pós-jogo (Murkrow e Houndour à noite) |
| celadon-city | Celadon City | city | pós-jogo |
| route-16 | Route 16 | route | pós-jogo |
| cycling-road | Cycling Road (Route 17) | route | pós-jogo |
| route-18 | Route 18 | route | pós-jogo |
| fuchsia-city | Fuchsia City | city | pós-jogo (Safari Zone fechado) |
| route-15 | Route 15 | route | pós-jogo |
| route-14 | Route 14 | route | pós-jogo |
| route-13 | Route 13 | route | pós-jogo (Chansey raro) |
| route-12 | Route 12 | route | pós-jogo (casa do Super Rod) |
| route-11 | Route 11 | route | pós-jogo (Snorlax bloqueia) |
| lavender-town | Lavender Town | city | pós-jogo |
| lavender-radio-tower | Kanto Radio Tower | building | pós-jogo (EXPN Card) |
| route-8 | Route 8 | route | pós-jogo |
| route-10 | Route 10 | route | pós-jogo |
| rock-tunnel | Rock Tunnel | dungeon | pós-jogo; Flash |
| power-plant | Power Plant | building | pós-jogo; Surf |
| route-9 | Route 9 | route | pós-jogo |
| cerulean-city | Cerulean City | city | pós-jogo |
| route-24 | Route 24 | route | pós-jogo |
| route-25 | Route 25 | route | pós-jogo (Cerulean Cape) |
| route-5 | Route 5 | route | pós-jogo (atalho Cerulean–Vermilion) |
| diglett-cave | Diglett's Cave | dungeon | depois do Snorlax |
| route-2 | Route 2 | route | saída do Diglett's Cave (Cut) |
| pewter-city | Pewter City | city | pós-jogo |
| route-3 | Route 3 | route | pós-jogo |
| mt-moon | Mt. Moon | dungeon | pós-jogo |
| mt-moon-square | Mt. Moon Square | zone | pós-jogo (Clefairy às segundas à noite) |
| route-4 | Route 4 | route | pós-jogo |
| viridian-city | Viridian City | city | pós-jogo |
| route-1 | Route 1 | route | pós-jogo |
| pallet-town | Pallet Town | city | pós-jogo |
| route-21 | Route 21 | sea | pós-jogo; Surf |
| cinnabar-island | Cinnabar Island | city | pós-jogo (só o Pokémon Center) |
| route-20 | Route 20 | sea | pós-jogo; Surf |
| seafoam-islands | Seafoam Islands | dungeon | pós-jogo (ginásio do Blaine) |
| route-22 | Route 22 | route | pós-jogo (16 insígnias) |
| route-28 | Route 28 | route | pós-jogo (16 insígnias; selvagens lv 40+) |
| silver-cave | Mt. Silver | dungeon | pós-jogo (16 insígnias); Flash e Waterfall |

## Encontros (para `encounters/c.json`; method `walk` salvo indicação; `conditions`: day/night/dia da semana)

- Route 29: Rattata, Pidgey, Sentret, Hoppip (dia); Hoothoot (noite).
- Route 30/31: Pokémon comuns (o detonado não lista espécies).
- Dark Cave (entrada da Route 31): Geodude, Zubat, Dunsparce (raro). Wobbuffet: só aqui no jogo.
- Sprout Tower: Gastly (noite).
- Route 32 (perto de Violet): Bellsprout.
- Ruins of Alph (grama externa): Natu, Smeargle. Unown nas câmaras (formas liberadas por quebra-cabeça: Kabuto, Aerodactyl, Ho-Oh, Omanyte).
- Union Cave, andar inferior (`fish`/`surf`): Tentacruel, Shellder, Chinchou, Lanturn. Lapras fixo (`fixed`, só às sextas; nível não listado).
- Slowpoke Well (parte com Strength): Slowpoke, Slowbro.
- Ilex Forest: Farfetch'd (o do evento do Cut; selvagens não listados).
- Route 35: Yanma (raro).
- National Park / Bug-Catching Contest (ter/qui/sáb): Scyther, Pinsir (prêmio Sun Stone ao vencer).
- Route 36: Sudowoodo fixo (`fixed`, uma vez só).
- Goldenrod (água em frente ao Day-Care, `surf`): Tentacool, Tentacruel.
- Árvores com Headbutt (`tree`): Aipom, Pineco, Heracross, Exeggcute "entre outros".
- Rock Smash (`rock_smash`): pedras podem ter itens ou Pokémon (espécies não listadas).
- Lake of Rage: Red Gyarados shiny fixo (`fixed`; dá Red Scale).
- Team Rocket Hideout: Electrode ×3 (`fixed`, capturáveis).
- Whirl Islands: Seel (`surf`), Horsea (`fish`). Lugia fixo (`fixed`, Silver Wing na bolsa; ilha B, passagens 2 e 3, descer a cachoeira).
- Route 7 (Kanto): Murkrow, Houndour (noite).
- Route 13 (Kanto): Chansey (raro).
- Rock Tunnel: Kangaskhan (raro).
- Route 11: Snorlax fixo lv 50 (`fixed`, uma vez só).
- Mt. Moon Square: Clefairy dançando (segunda à noite; depois, Rock Smash dá Moon Stone).
- Route 28 / Mt. Silver: selvagens lv 40+.
- Lendários: Raikou e Entei (`walk`, errantes em Johto depois da Burned Tower; lv 40 segundo a dica do "lv 39 com Repel"); Suicune fixo na Tin Tower (depois da Radio Tower, com o item do diretor; nível não listado); Ho-Oh fixo no topo da Tin Tower (precisa ter capturado Suicune, Raikou e Entei no próprio jogo; nível não listado); Lugia fixo nas Whirl Islands (Silver Wing; nível não listado).

## Presentes, trocas in-game, ovos e iniciais (`method: gift` / `trade`)

- Inicial: Chikorita / Cyndaquil / Totodile (Prof. Elm, New Bark; nível não listado).
- Mystery Egg (Mr. Pokémon, Route 30) → entregue ao Elm; ovo devolvido no Pokémon Center de Violet depois da Zephyr Badge → Togepi (choca em ~1000 passos). Elm dá um presente ao ver o Togepi (item não nomeado).
- Ovo do Day-Care (Route 34, com no máximo 5 no time): ovo exclusivo do Crystal com um Pokémon bebê aleatório.
- Eevee (Bill, Goldenrod; depois de encontrá-lo no Pokémon Center de Ecruteak; no máximo 5 no time).
- Shuckle (casa ao lado do Pokémon Center de Cianwood; no máximo 5 no time).
- Dratini com ExtremeSpeed (monge do Dragon's Den, depois da Rising Badge; no máximo 5 no time).
- Tyrogue (Karate King, Mt. Mortar, depois de vencê-lo).
- Spearow com Mail (guarita norte de Goldenrod) → entregar ao homem da Route 31 (dá TM50 Nightmare); guarda devolve HP Up.
- Trocas: Bellsprout → Onix (Violet City); Drowzee → Machop (Goldenrod Dept. Store 5F); Dratini → Rhydon (Blackthorn; o detonado diz "quer trocar um Rhydon por um Dratini", direção ambígua).
- Master Ball (Prof. Elm, depois da Rising Badge).

## Apricorns e Poké Balls do Kurt (Azalea)

- White Apricorn (atrás da casa do Kurt) → Fast Ball ("Speed Ball" no detonado). Kurt dá Lure Ball depois do Slowpoke Well.
- Route 37: Blue Apricorn → Lure Ball; Red Apricorn → Level Ball; Black Apricorn → Heavy Ball.
- Route 42 (cercado com Suicune): Pink, Green e Yellow Apricorn (bolas não listadas no detonado).
- Uma Apricorn por dia; bola pronta no dia seguinte.

## TMs, HMs e itens-chave (localização)

| Item | Local / condição |
| --- | --- |
| PokéGear | Mãe, New Bark |
| Map Card | Guia de Cherrygrove |
| Pokédex, Mystery Egg | Mr. Pokémon, Route 30 |
| HM05 Flash | Elder Li, topo da Sprout Tower (uso fora de batalha com a Zephyr Badge) |
| TM31 Mud-Slap | Falkner |
| Old Rod | Pescador no Pokémon Center da Route 32 |
| HM01 Cut | Ilex Forest (depois de levar o Farfetch'd) |
| Charcoal | Pai do garoto da Ilex Forest, Azalea |
| TM49 Fury Cutter | Bugsy |
| TM05 Roar | Route 32 (cortar o arbusto) |
| Bicycle | Bike Shop, Goldenrod |
| Coin Case | Goldenrod Underground |
| TM45 Attract | Whitney |
| Radio Card | Radio Tower 1F (quiz: sim, sim, não, sim, não) |
| TM50 Nightmare | Homem da Route 31 (entrega do Spearow) |
| TM04 Rollout | Route 35 |
| Squirtbottle | Casa das flores, Goldenrod (depois da Whitney) |
| TM08 Rock Smash | Homem da Route 36 (depois do Sudowoodo) |
| Quick Claw | Garota com Persian, National Park |
| TM28 Dig | National Park |
| Item Finder | Casa à esquerda do Pokémon Center de Ecruteak |
| HM03 Surf | Ecruteak Dance Theater (vencer as Kimono Girls) |
| TM30 Shadow Ball | Morty |
| Mystic Water | Homem no lago de Cherrygrove (Surf) |
| Soft Sand | Treinadoras na água em frente ao Day-Care (Surf) |
| TM02 Headbutt | citado (local não listado no texto) |
| TM13 Snore | Moomoo Farm, Route 39 (depois de curar a Miltank com 8–10 Berries) |
| TM34 Swagger | Olivine Lighthouse |
| Good Rod | Pescador ao lado do Mart de Olivine |
| HM04 Strength | Marinheiro na casa ao lado do Pokémon Center de Olivine |
| King's Rock, TM18 Rain Dance | Slowpoke Well (parte com Strength) |
| SecretPotion | Cianwood Pharmacy |
| TM01 DynamicPunch | Chuck |
| HM02 Fly | Esposa do Chuck, Cianwood |
| TM23 Iron Tail | Jasmine |
| Red Scale | Red Gyarados, Lake of Rage |
| HM06 Whirlpool | Lance, Team Rocket Hideout |
| TM16 Icy Wind | Pryce |
| Basement Key | Rocket Executive, topo da Radio Tower |
| Card Key | Diretor, Goldenrod Underground |
| Clear Bell | Diretor da Radio Tower (necessário para o Suicune da Tin Tower; o detonado não nomeia o item) |
| HM07 Waterfall | Ice Path |
| TM24 DragonBreath | Clair |
| Master Ball | Prof. Elm |
| S.S. Ticket | Prof. Elm (pós-jogo) |
| Super Rod | Pescador na casa da Route 12 |
| Machine Part | Piscina do ginásio de Cerulean (devolver na Power Plant → TM, número não listado) |
| Lost Item (boneco de Clefairy) | Casa abaixo do Pokémon Center de Vermilion |
| Pass (Magnet Train) | Copycat, Saffron, em troca do boneco |
| EXPN Card | Kanto Radio Tower, Lavender |
| Silver Wing | Homem perto do Mart de Pewter |
| Moon Stone | Mt. Moon Square (Rock Smash depois da dança dos Clefairy) |

Pedras por telefone: Gina (Route 34) Leaf Stone; Alan (Route 36) Fire Stone; Dana (Route 38) Thunder Stone; Tully (Route
42) Water Stone (necessária para a câmara Omanyte das Ruins of Alph). Frieda (Route 32, sextas): Poison Barb.

## Mecânicas (para o guia de mecânicas, texto nosso)

- Relógio real: dia/noite e dia da semana definidos no início; encontros e eventos mudam com isso.
- Telefone da PokéGear: treinadores pedem revanche e dão itens (pedras evolutivas).
- Berry trees: uma fruta por árvore por dia. Apricorns viram Poké Balls especiais no Kurt.
- Itens escondidos: apertar A em pontos específicos; Item Finder ajuda.
- Bug-Catching Contest no National Park (ter/qui/sáb).
- Buena's Password (rádio à noite) → pontos trocados por prêmios (Ultra Ball, Nugget, Rare Candy) na Radio Tower 2F.
- Day-Care (Route 34): cruzamento de macho e fêmea gera ovos; ovos chocam com passos.
- Mãe guarda dinheiro e às vezes compra itens.
- Raikou e Entei errantes depois da Burned Tower; Suicune aparece em vários pontos antes da captura (Cianwood, Route 42); Eusine o persegue.
- Headbutt em árvores e Rock Smash em pedras dão encontros.
- Ruins of Alph: 4 quebra-cabeças (Kabuto, Aerodactyl, Ho-Oh, Omanyte); Unown Dex depois de 3 formas; palavras na parede abrem salas secretas: ESCAPE (usar Escape Rope), LIGHT (usar Flash), HO OH (Ho-Oh no time), WATER (Water Stone na bolsa); cada sala tem 4 itens.
- Pokérus: pode vir de batalhas (o detonado cita Yanma).
- Escolha de menino ou menina no início (exclusivo do Crystal).
- Battle Tower perto de Olivine.
- Magnet Train Goldenrod ↔ Saffron (com Pass).

## Pós-jogo (ordem de eventos)

1. Créditos depois do Lance; Prof. Elm dá o S.S. Ticket.
2. S.S. Aqua (Olivine → Vermilion; procurar a menina perdida).
3. Lt. Surge → Sabrina → Erika → Janine (Fuchsia; Safari Zone fechado).
4. Routes 12–15 (Super Rod), Lavender Town, Rock Tunnel, Power Plant (peça roubada).
5. Cerulean: Rocket Grunt, peça na piscina do ginásio, Misty na Cerulean Cape → Misty.
6. Peça devolvida na Power Plant (TM); boneco de Clefairy (Vermilion) → Pass (Saffron); EXPN Card (Lavender).
7. Snorlax (Route 11, rádio PokéFlute) → Diglett's Cave → Pewter: Silver Wing, Brock.
8. Mt. Moon: última batalha com o rival.
9. Pallet, Cinnabar (Blue na ilha) → Blaine nas Seafoam Islands → Blue em Viridian (16 insígnias).
10. Prof. Oak libera o Mt. Silver → Route 28 → Mt. Silver → Red.

## Dúvidas

- Times das batalhas mostradas só em imagem: Elder Li (Sprout Tower), Rocket Executives (Hideout e Radio Tower), Rocket do Slowpoke Well, Kimono Girls; o texto não lista.
- Nível e estágio do inicial do rival em cada batalha: o texto só diz "Inicial lv X" (a primeira batalha nem tem nível).
- Níveis de Sudowoodo, Red Gyarados, Lapras, Suicune, Ho-Oh e Lugia não aparecem; Raikou/Entei lv 40 inferido da dica "lv 39".
- O detonado chama a rota ao norte de Mahogany de "Route 33" (é a Route 43) e o líder de Cianwood de "Chucky" (Chuck).
- Troca de Blackthorn: o texto diz "quer trocar um Rhydon por um Dratini", sem deixar claro qual dos dois o jogador entrega.
- TM dado pelo técnico da Power Plant e itens das duas pessoas da Radio Tower: não nomeados.
- Bolas feitas com Pink, Green e Yellow Apricorn: não listadas.
