# Gold/Silver — fatos extraídos do detonado da Pokémon Mythology

Fonte: https://www.pokemythology.net/detonados/detonado-gold-silver/ (PDF salvo em 05/11/2026, 26 páginas; o conteúdo vai
até a página 24, o resto é rodapé do site). Só **fatos de jogo** (chefes, níveis, encontros, itens) no nosso formato.
Nenhuma prosa foi copiada. Golpes dos chefes **não** constam no detonado: deixar `moves: []` e `verified: false` para
golpes; espécies e níveis são `verified: true` (uma fonte) até o analista conferir numa segunda. O rival não tem nome
fixo (o jogador escolhe); os ids usam `rival-N`. "Inicial evoluído" = o inicial com vantagem que o rival roubou do Elm
(Chikorita→rival Cyndaquil; Cyndaquil→rival Totodile; Totodile→rival Chikorita), na forma do estágio indicado.

## Ordem da história e chefes (para `bosses/gs.json`)

| Order | Id | Kind | Nome | Local | Time (espécie lv) | Cap |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | rival-1 | rival | Rival | Saída de Cherrygrove para New Bark | inicial com vantagem, nível não listado (detonado recomenda lv 8) | — |
| 2 | sage-li | other | Chefe dos monges (Sprout Tower) | Sprout Tower, Violet City | time não listado (prêmio: HM05 Flash) | — |
| 3 | falkner | gym | Falkner (voador) | Violet Gym | Pidgey 7, Pidgeotto 9 | 9 |
| 4 | rocket-slowpoke-well | evil | Team Rocket | Slowpoke Well, Azalea Town | time não listado | — |
| 5 | bugsy | gym | Bugsy (inseto) | Azalea Gym | Metapod 14, Kakuna 14, Scyther 16 | 16 |
| 6 | rival-2 | rival | Rival | Saída oeste de Azalea (antes do Ilex Forest) | Gastly 12, Zubat 14, inicial evoluído 16 | 16 |
| 7 | whitney | gym | Whitney (normal) | Goldenrod Gym | Clefairy 18, Miltank 20 | 20 |
| 8 | rival-3 | rival | Rival | Burned Tower, Ecruteak City | Haunter 20, Zubat 20, Magnemite 18, inicial evoluído 22 | 22 |
| 9 | kimono-girls | other | Kimono Girls (5 evoluções do Eevee) | Dance Theater, Ecruteak | time não listado (prêmio: HM03 Surf) | — |
| 10 | morty | gym | Morty (fantasma) | Ecruteak Gym | Gastly 21, Haunter 21, Gengar 25, Haunter 23 | 25 |
| 11 | chuck | gym | Chuck (lutador) | Cianwood Gym | Primeape 27, Poliwrath 30 | 30 |
| 12 | jasmine | gym | Jasmine (aço) | Olivine Gym (após entregar a SecretPotion) | Magnemite 30, Magnemite 30, Steelix 35 | 35 |
| 13 | rocket-mahogany | evil | Team Rocket (com Lance) | Rocket Hideout, Mahogany Town | time não listado; 3 Electrode na sala do gerador | — |
| 14 | pryce | gym | Pryce (gelo) | Mahogany Gym | Seel 27, Dewgong 29, Piloswine 31 | 31 |
| 15 | rocket-radio-tower | evil | Chefe Rocket (topo da torre, falso diretor) | Radio Tower, Goldenrod | time não listado (prêmio: Basement Key) | — |
| 16 | rival-4 | rival | Rival | Goldenrod Underground (porta da Basement Key) | Golbat 30, Magnemite 28, Sneasel 32, Haunter 30, inicial evoluído 32 | 32 |
| 17 | rocket-radio-tower-2 | evil | Team Rocket restante (portão da Card Key) | Radio Tower, Goldenrod | time não listado | — |
| 18 | clair | gym | Clair (dragão) | Blackthorn Gym (insígnia só após o Dragon's Den) | Dragonair 37, Dragonair 37, Dragonair 37, Kingdra 40 | 40 |
| 19 | rival-5 | rival | Rival | Victory Road, fim da caverna | Sneasel 34, Kadabra 35, Golbat 36, Magneton 35, Haunter 35, inicial totalmente evoluído 38 | 38 |
| 20 | will | elite | Will (psíquico) | Indigo Plateau | Xatu 40, Exeggutor 41, Jynx 41, Slowbro 41, Xatu 41 | 41 |
| 21 | koga | elite | Koga (veneno/inseto) | Indigo Plateau | Ariados 40, Forretress 43, Muk 42, Venomoth 41, Crobat 44 | 44 |
| 22 | bruno | elite | Bruno (lutador/pedra) | Indigo Plateau | Hitmontop 42, Hitmonchan 42, Hitmonlee 42, Onix 43, Machamp 46 | 46 |
| 23 | karen | elite | Karen (noturno/veneno) | Indigo Plateau | Umbreon 42, Murkrow 44, Vileplume 42, Gengar 45, Houndoom 47 | 47 |
| 24 | lance | champion | Lance (dragão/voador) | Indigo Plateau | Gyarados 44, Dragonite 47, Dragonite 47, Aerodactyl 46, Charizard 46, Dragonite 50 | 50 |
| 25 | lt-surge | gym | Lt. Surge (elétrico) | Vermilion Gym (Kanto) | Raichu 44, Magneton 40, Electrode 40, Electrode 40, Electabuzz 46 | 46 |
| 26 | sabrina | gym | Sabrina (psíquico) | Saffron Gym | Espeon 46, Mr. Mime 46, Alakazam 48 | 48 |
| 27 | erika | gym | Erika (planta) | Celadon Gym | Tangela 44, Jumpluff 40, Victreebel 40, Bellossom 40 | 44 |
| 28 | janine | gym | Janine (veneno) | Fuchsia Gym (paredes invisíveis) | Crobat 36, Ariados 33, Weezing 36, Weezing 36, Venomoth 39 | 39 |
| 29 | rocket-cerulean | evil | Rocket (ladrão da Machine Part) | Norte de Cerulean (Route 24) | Golbat, nível não listado | — |
| 30 | misty | gym | Misty (água) | Cerulean Gym (após voltar do encontro na Route 25) | Golduck 42, Quagsire 42, Lapras 44, Starmie 47 | 47 |
| 31 | brock | gym | Brock (pedra) | Pewter Gym | Graveler 41, Rhyhorn 41, Kabutops 42, Omastar 42, Onix 44 | 44 |
| 32 | rival-6 | rival | Rival (última luta) | Entrada do Mt. Moon | Sneasel 41, Magneton 41, Golbat 42, Alakazam 43, Gengar 43, inicial totalmente evoluído 45 | 45 |
| 33 | blaine | gym | Blaine (fogo) | Caverna-ginásio nas Seafoam Islands | Magcargo 45, Magmar 45, Rapidash 50 | 50 |
| 34 | blue | gym | Blue (vários tipos) | Viridian Gym | Pidgeot 56, Alakazam 54, Rhydon 56, Exeggutor 58, Gyarados 58, Arcanine 58 | 58 |
| 35 | karate-king | other | Karate King (líder do Fighting Dojo) | Mt. Mortar (opcional, Surf + Waterfall) | Hitmonlee 34, Hitmonchan 34 | — (opcional) |
| 36 | red | other | Red | Topo do Mt. Silver (após as 16 insígnias) | Pikachu 81, Espeon 73, Snorlax 75, Blastoise 77, Venusaur 77, Charizard 77 | 81 |

Ordem 25–34 (Kanto) é a do detonado; os ginásios de Kanto podem ser feitos em outra ordem. Detonado recomenda equipe
lv 50+ para a Elite Four e lv 80–90 para o Red.

## Áreas (para `areas/gs.json`, kind e ordem de liberação)

| Id | Nome | Kind | Libera |
| --- | --- | --- | --- |
| new-bark-town | New Bark Town | city | início |
| route-29 | Route 29 | route | início |
| cherrygrove-city | Cherrygrove City | city | início |
| route-30 | Route 30 (casa do Mr. Pokémon) | route | início; treinadores após o Pokédex |
| route-31 | Route 31 | route | após o Pokédex |
| dark-cave | Dark Cave | dungeon | entrada perto de Violet já aberta; exploração com Flash, Surf e Rock Smash |
| violet-city | Violet City | city | após o Pokédex |
| sprout-tower | Sprout Tower | building | ao chegar em Violet |
| ruins-of-alph | Ruins of Alph | dungeon | após Violet (partes com Surf e pela Union Cave) |
| route-32 | Route 32 | route | após Falkner |
| union-cave | Union Cave | dungeon | após Falkner (andar de baixo com Surf) |
| route-33 | Route 33 | route | após a Union Cave |
| azalea-town | Azalea Town | city | após a Union Cave |
| slowpoke-well | Slowpoke Well | dungeon | após falar com o Kurt |
| ilex-forest | Ilex Forest | dungeon | após Bugsy (Cut para atravessar) |
| route-34 | Route 34 | route | após o Ilex Forest |
| goldenrod-city | Goldenrod City | city | após o Ilex Forest |
| goldenrod-underground | Goldenrod Underground | building | após Goldenrod; porta trancada com a Basement Key |
| radio-tower | Goldenrod Radio Tower | building | quiz já; Rockets após a 7ª insígnia |
| goldenrod-dept-store | Goldenrod Dept. Store | building | após Goldenrod |
| route-35 | Route 35 | route | após Goldenrod |
| national-park | National Park | zone | após Goldenrod |
| route-36 | Route 36 | route | após Goldenrod (Sudowoodo com SquirtBottle) |
| route-37 | Route 37 | route | após o Sudowoodo |
| ecruteak-city | Ecruteak City | city | após o Sudowoodo |
| burned-tower | Burned Tower | building | após chegar em Ecruteak |
| dance-theater | Dance Theater | building | após chegar em Ecruteak |
| tin-tower | Tin Tower | building | com o Rainbow Wing |
| route-38 | Route 38 | route | após Morty |
| route-39 | Route 39 (Moomoo Farm) | route | após Morty |
| olivine-city | Olivine City | city | após Morty |
| glitter-lighthouse | Glitter Lighthouse | building | ao chegar em Olivine |
| route-40 | Route 40 | sea | com Surf |
| route-41 | Route 41 | sea | com Surf |
| whirl-islands | Whirl Islands | dungeon | com Surf e Whirlpool (Flash dentro) |
| cianwood-city | Cianwood City | city | com Surf |
| route-42 | Route 42 | route | após Olivine (Surf para cortar caminho) |
| mt-mortar | Mt. Mortar | dungeon | após Olivine; interior com Surf, Waterfall e Strength |
| mahogany-town | Mahogany Town | city | após Route 42 |
| lake-of-rage | Lake of Rage (Route 43) | zone | após Mahogany |
| rocket-hideout | Rocket Hideout (Mahogany) | dungeon | após falar com Lance |
| route-44 | Route 44 | route | após a Radio Tower |
| ice-path | Ice Path | dungeon | após a Radio Tower (Strength) |
| blackthorn-city | Blackthorn City | city | após o Ice Path |
| dragons-den | Dragon's Den | dungeon | após Clair (Surf + Whirlpool) |
| route-45 | Route 45 | route | após Blackthorn |
| route-46 | Route 46 | route | após Blackthorn |
| route-27 | Route 27 | sea | com Surf e as 8 insígnias (a leste de New Bark) |
| tohjo-falls | Tohjo Falls | dungeon | com Waterfall |
| route-26 | Route 26 | route | após Tohjo Falls |
| victory-road | Victory Road | dungeon | com as 8 insígnias |
| indigo-plateau | Indigo Plateau | building | após Victory Road |
| ss-aqua | S.S. Aqua | building | pós-Liga, com o S.S. Ticket (Olivine → Vermilion) |
| vermilion-city | Vermilion City | city | pós-Liga |
| saffron-city | Saffron City | city | pós-Liga |
| route-7 | Route 7 | route | pós-Liga |
| celadon-city | Celadon City | city | pós-Liga |
| cycling-road | Cycling Road (Routes 16–17) | route | pós-Liga |
| fuchsia-city | Fuchsia City | city | pós-Liga (Safari Zone fechada) |
| routes-12-15 | Routes 12–15 | route | pós-Liga |
| lavender-town | Lavender Town | city | pós-Liga |
| lavender-radio-tower | Kanto Radio Tower | building | pós-Liga |
| rock-tunnel | Rock Tunnel | dungeon | pós-Liga (Flash) |
| power-plant | Power Plant | building | pós-Liga |
| cerulean-city | Cerulean City | city | pós-Liga |
| route-24-25 | Routes 24–25 | route | pós-Liga |
| route-11 | Route 11 | route | Snorlax acordado com a Poké Flute do rádio |
| digletts-cave | Diglett's Cave | dungeon | após o Snorlax |
| pewter-city | Pewter City | city | após o Diglett's Cave |
| mt-moon | Mt. Moon | dungeon | após Pewter |
| route-2 | Route 2 | route | após Pewter |
| viridian-city | Viridian City | city | após Pewter |
| pallet-town | Pallet Town | city | após Viridian |
| route-21 | Route 21 | sea | com Surf |
| cinnabar-island | Cinnabar Island | city | com Surf (só o Pokémon Center) |
| route-20 | Route 20 | sea | com Surf |
| seafoam-islands | Seafoam Islands (ginásio do Blaine) | dungeon | com Surf |
| route-22 | Route 22 | route | após Viridian |
| route-28 | Route 28 | route | após as 16 insígnias e falar com o Oak |
| mt-silver | Mt. Silver | dungeon | após as 16 insígnias (Flash e Waterfall) |

## Encontros (para `encounters/gs.json`; method `walk` salvo indicação; `conditions`: day/night/dia da semana)

- Rotas iniciais (Route 29–31): Sentret (`day`), Pidgey.
- Sprout Tower: Bellsprout.
- Árvores de Headbutt (`tree`): Pokémon em árvores específicas por todo o continente (TM Headbutt, Ilex Forest); espécies não listadas.
- National Park: Bug-Catching Contest às terças, quintas e sábados (20 Park Balls; Scyther ou Pinsir recomendados; 1º prêmio Sun Stone).
- Union Cave (andar de baixo, `fish`): Tentacruel, Shellder, Chinchou, Lanturn. Lapras (`fixed`) toda sexta no fundo.
- Lake of Rage (`fixed`): Gyarados vermelho (shiny).
- Rocket Hideout (`fixed`): 3 Electrode na sala do gerador (capturáveis).
- Whirl Islands: Seel.
- Dark Cave: Wobbuffet (único local do jogo).
- Route 7 (Saffron → Celadon): Murkrow, Houndour (`night`).
- Rota a leste de Fuchsia (Routes 13–15): Chansey (raro).
- Rock Tunnel (`cave`): Kangaskhan (raro).
- Mt. Silver / Route 28: Pokémon lv 40+ (espécies não listadas).
- Ruins of Alph: Unown; 4 quebra-cabeças (Kabuto, Aerodactyl, Ho-Oh, Omanyte) liberam grupos de letras (listas só em imagem).
- Mt. Moon: Clefairy dançando às segundas à noite (evento; Moon Stone com Rock Smash).
- Fixos (`fixed`): Sudowoodo (Route 36, SquirtBottle, único); Snorlax lv 50 (Route 11, rádio na Poké Flute, único).
- Errantes (`walk`, qualquer mato de Johto): Raikou, Entei, Suicune, soltos ao cair no subsolo da Burned Tower. Nível não listado.
- Lendários (`fixed`): Ho-Oh — Tin Tower, lv 40 [Gold] (após a Radio Tower) / lv 70 [Silver] (com o Rainbow Wing de Pewter); Lugia — Whirl Islands (entrar pela ilha B), lv 40 [Silver] (após a Radio Tower) / lv 70 [Gold] (com o Silver Wing de Pewter).

## Presentes, trocas in-game e fósseis (`method: gift` / `trade`)

- Inicial: Chikorita, Cyndaquil ou Totodile (Prof. Elm, New Bark; nível não listado).
- Mystery Egg → Togepi (Mr. Pokémon/Prof. Oak na Route 30; devolvido pelo assistente do Elm no Pokémon Center de Violet após Falkner; eclode após ~1000 passos segundo o detonado).
- Eevee (Bill, Goldenrod, depois de encontrá-lo no Pokémon Center de Ecruteak).
- Shuckle (menino numa casa de Cianwood).
- Tyrogue (Karate King, Mt. Mortar, após vencê-lo; precisa de vaga na equipe).
- Trocas in-game: o detonado não cita nenhuma. Fósseis: nenhum citado.

## TMs/HMs e itens-chave (localização)

| Item | Local |
| --- | --- |
| PokéGear | Mãe, New Bark |
| Potion | Assistente do Elm |
| Map Card | Velho de Cherrygrove (após o tour) |
| Pokédex | Prof. Oak, casa do Mr. Pokémon |
| Mystery Egg | Prof. Oak / Mr. Pokémon |
| 5 Poké Balls | Assistente do Elm |
| HM05 Flash | Chefe dos monges, Sprout Tower |
| Old Rod | Pescador no Pokémon Center da Route 32 |
| Lure Ball | Kurt, após o Slowpoke Well |
| HM01 Cut | Dono do Farfetch'd, Ilex Forest |
| TM Headbutt | Homem no Ilex Forest |
| Coin Case | Goldenrod Underground |
| Radio Card | Quiz da Radio Tower (yes, yes, no, yes, no) |
| Bicycle | Bike Shop de Goldenrod (grátis) |
| SquirtBottle | Casa das flores em Goldenrod, após Whitney |
| Sun Stone | 1º prêmio do Bug-Catching Contest |
| TM08 Rock Smash | Homem na Route 36, após o Sudowoodo |
| Item Finder | Casa à esquerda do Pokémon Center de Ecruteak |
| HM03 Surf | Dance Theater, após as Kimono Girls |
| TM (número não citado) | Mãe da Moomoo Farm, após curar o Miltank com 8–10 berries |
| HM04 Strength | Marinheiro na última casa antes da praia de Olivine |
| Good Rod | Pescador ao lado do Poké Mart de Olivine |
| SecretPotion | Farmácia de Cianwood |
| HM02 Fly | Esposa do Chuck, fora do Cianwood Gym |
| Everstone | Prof. Elm, ao mostrar o Togepi |
| HM06 Whirlpool | Lance, após o Rocket Hideout |
| Basement Key | Chefe Rocket do topo da Radio Tower |
| Card Key | Diretor da Radio Tower, Goldenrod Underground |
| Rainbow Wing [Gold] / Silver Wing [Silver] | Diretor da Radio Tower, após expulsar os Rockets |
| HM07 Waterfall | Ice Path |
| Dragon Fang | Dragon's Den |
| Master Ball | Prof. Elm, após a 8ª insígnia |
| S.S. Ticket | Prof. Elm, pós-Liga |
| Machine Part | Cerulean Gym, piscina (Surf) |
| TM (número não citado) | Gerente da Power Plant, ao devolver a Machine Part |
| Clefairy Doll | Casa abaixo do Pokémon Center de Vermilion |
| Pass (Magnet Train) | Menina em Saffron, em troca da Clefairy Doll |
| Expansion Card | Kanto Radio Tower, Lavender |
| Super Rod | Casa na rota a leste de Fuchsia |
| Silver Wing [Gold] / Rainbow Wing [Silver] | Homem perto do Poké Mart de Pewter |
| Moon Stone | Mt. Moon, segunda à noite (Rock Smash) |

Lojas e serviços: Goldenrod Dept. Store; Kurt faz Poké Balls com Apricorns (White Apricorn → Fast Ball); Moomoo Milk na
Moomoo Farm; Magnet Train Goldenrod ↔ Saffron.

## Mecânicas (para o guia de mecânicas, texto nosso)

- Relógio real: hora, minuto e dia da semana definidos no início; dia/noite muda encontros (Sentret de dia, Murkrow/Houndour à noite).
- Eventos por dia: Bug-Catching Contest (ter/qui/sáb), Lapras na Union Cave (sex), Clefairy no Mt. Moon (seg à noite).
- Berry trees e Apricorn trees: um fruto por dia; Apricorns viram Poké Balls com o Kurt.
- PokéGear: relógio, mapa, telefone (revanches com treinadores registrados) e rádio (Expansion Card; estação da Poké Flute acorda o Snorlax).
- Mãe guarda dinheiro e às vezes compra itens.
- TM Headbutt sacode árvores com Pokémon.
- Shiny: Gyarados vermelho no Lake of Rage.
- Cães lendários errantes após a Burned Tower.
- Unown Dex: liberada ao capturar 3 Unown diferentes (Select na Pokédex).
- Exclusivos citados: Ho-Oh lv 40 no Gold e lv 70 no Silver; Lugia lv 40 no Silver e lv 70 no Gold; asas trocadas por versão.
- Itens de recuperação liberados entre as lutas da Elite Four; Abra no Indigo Plateau teleporta para New Bark.
- 16 insígnias: 8 de Johto e 8 de Kanto; Magnet Train liga as regiões.

Pós-jogo (ordem do detonado): créditos → S.S. Ticket do Elm → S.S. Aqua → Vermilion (Lt. Surge) → Saffron (Sabrina) →
Celadon (Erika) → Fuchsia (Janine) → Lavender/Rock Tunnel/Power Plant → Cerulean (Rocket, Misty) → Machine Part →
Clefairy Doll → Pass → Expansion Card → Snorlax (Route 11) → Pewter (asa, Brock) → Mt. Moon (rival-6) → Pallet →
Cinnabar → Seafoam (Blaine) → Viridian (Blue) → Oak → Route 28 → Mt. Silver (Red).

## Dúvidas

- Rival na saída de Cherrygrove: espécie e nível não listados (só o inicial com vantagem).
- Erika (Kanto): o detonado dá Tangela 44, Jumpluff 40, Victreebel 40, Bellossom 40; conferir os níveis.
- O detonado escreve "Hunter" nos times dos rivais 4 e 5; lido como Haunter.
- Unown por quebra-cabeça só aparecem em imagens (o PDF não pôde ser aberto); faltam as letras.
- Níveis dos cães lendários, do Lapras, do Sudowoodo, do Gyarados vermelho, dos Electrode e dos presentes (Eevee, Shuckle, Tyrogue, Togepi) não constam.
- Números das TMs da Moomoo Farm e da Power Plant não constam; números de rota que o detonado não cita vêm do mapa padrão, conferir.
- Espécies da Route 28 / Mt. Silver e das árvores de Headbutt não constam.
- O detonado só cita os itens de captura de Ho-Oh/Lugia por versão; outros exclusivos de versão (ex.: Gold vs Silver no mato) não constam.
