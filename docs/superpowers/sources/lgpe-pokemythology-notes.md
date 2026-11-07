# Let's Go, Pikachu! / Let's Go, Eevee! — fatos extraídos do detonado da Pokémon Mythology

Fonte: https://www.pokemythology.net/detonados/detonado-lets-go-pikachu-lets-go-eevee/ (PDF salvo em 05/11/2026, 78 páginas).
Só **fatos de jogo** (chefes, níveis, encontros, itens) no nosso formato. Nenhuma prosa foi copiada. Golpes dos
chefes **não** constam no detonado: deixar `moves: []` e `verified: false` para golpes; espécies e níveis são
`verified: true` (uma fonte) até o analista conferir numa segunda. Exclusivos de versão: `[Pikachu]` / `[Eevee]`.
"inicial" = Pikachu ou Eevee do jogador; o rival fica com o oposto da versão (Eevee no Let's Go Pikachu, Pikachu no
Let's Go Eevee). Os números de rota **não** aparecem no detonado: foram deduzidos pelo mapa de Kanto (ver Dúvidas).

## Ordem da história e chefes (para `bosses/lgpe.json`)

| Order | Id | Kind | Nome | Local | Time (espécie lv) | Cap |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | rival-1 | rival | Rival | Laboratório do Oak, Pallet Town | Eevee/Pikachu (oposto da versão) 6 | 6 |
| 2 | rival-2 | rival | Rival | Route 22 | Pidgey 3, Eevee/Pikachu 7 | 7 |
| 3 | brock-1 | gym | Brock (Boulder Badge) | Pewter Gym | Geodude 11, Onix 12 | 12 |
| 4 | jessie-james-1 | evil | Jessie & James | Saída do Mt. Moon | Ekans 12, Koffing 12 | 12 |
| 5 | rival-3 | rival | Rival | Norte de Cerulean (antes da Nugget Bridge) | Pidgey 12, Oddish 12, Eevee/Pikachu 13 | 13 |
| 6 | misty-1 | gym | Misty (Cascade Badge) | Cerulean Gym | Psyduck 18, Starmie 19 | 19 |
| 7 | rival-4 | rival | Rival | S.S. Anne | Pidgeotto 20, Oddish 20, Eevee/Pikachu 21 | 21 |
| 8 | mina-1 | other | Mina (opcional) | Porto de Vermilion | Jigglypuff 21, Mr. Mime 22 | 22 |
| 9 | surge-1 | gym | Lt. Surge (Thunder Badge) | Vermilion Gym | Voltorb 25, Magnemite 25, Raichu 26 | 26 |
| 10 | rival-5 | rival | Rival | Pokémon Tower, Lavender | Pidgeotto 27, Gloom 27, Jolteon/Raichu 28 | 28 |
| 11 | erika-1 | gym | Erika (Rainbow Badge) | Celadon Gym | Tangela 33, Weepinbell 33, Vileplume 34 | 34 |
| 12 | jessie-james-2 | evil | Jessie & James | Rocket Hideout (último andar) | Arbok 32, Weezing 32 | 32 |
| 13 | archer-1 | evil | Archer (Admin Rocket) | Rocket Hideout | Weezing 33, Golbat 33 | 33 |
| 14 | giovanni-1 | evil | Giovanni (chefe Rocket) | Rocket Hideout | Persian 35, Rhyhorn 35 | 35 |
| 15 | jessie-james-3 | evil | Jessie & James | Pokémon Tower (topo) | Arbok 34, Weezing 34 | 34 |
| 16 | blue-1 | other | Blue | Entrada da Silph Co., Saffron | Exeggutor 38, Charizard 40 | 40 |
| 17 | archer-2 | evil | Archer + Rocket (dupla, com o rival ao lado) | Silph Co. 5F | Electrode 37, Golbat 37, Weezing 37 (time da Rocket parceira não listado) | 37 |
| 18 | jessie-james-4 | evil | Jessie & James | Silph Co. | Arbok 36, Weezing 36 | 36 |
| 19 | giovanni-2 | evil | Giovanni | Silph Co. (sala do presidente) | Persian 39, Rhyhorn 39, Nidoqueen 39 | 39 |
| 20 | sabrina-1 | gym | Sabrina (Marsh Badge) | Saffron Gym | Mr. Mime 43, Slowbro 43, Jynx 43, Alakazam 44 | 44 |
| 21 | snorlax-1 | other | Snorlax (5 min para derrotar, depois captura) | Saída oeste de Celadon (Route 16) | Snorlax 34 | — |
| 22 | snorlax-2 | other | Snorlax (mesma regra) | Sul de Lavender (Route 12) | nível não listado | — |
| 23 | koga-1 | gym | Koga (Soul Badge) | Fuchsia Gym | Weezing 43, Muk 43, Golbat 43, Venomoth 44 | 44 |
| 24 | zapdos-1 | other | Zapdos (5 min, depois captura) | Power Plant | Zapdos 50 | — |
| 25 | articuno-1 | other | Articuno (5 min, depois captura) | Seafoam Islands (fundo) | Articuno 50 | — |
| 26 | blaine-1 | gym | Blaine (Volcano Badge) | Cinnabar Gym | Magmar 47, Rapidash 47, Ninetales 47, Arcanine 48 | 48 |
| 27 | giovanni-gym | gym | Giovanni (Earth Badge) | Viridian Gym | Dugtrio 49, Nidoqueen 49, Nidoking 49, Rhydon 50 | 50 |
| 28 | rival-6 | rival | Rival | Route 22 (caminho da Victory Road) | Pidgeot 50, Vileplume 50, Marowak 50, Jolteon/Raichu 51 | 51 |
| 29 | moltres-1 | other | Moltres (5 min, depois captura) | Victory Road | Moltres 50 | — |
| 30 | lorelei-1 | elite | Lorelei | Indigo Plateau | Dewgong 51, Jynx 51, Cloyster 51, Slowbro 51, Lapras 52 | 52 |
| 31 | bruno-1 | elite | Bruno | Indigo Plateau | Onix 52, Hitmonlee 52, Hitmonchan 52, Poliwrath 52, Machamp 53 | 53 |
| 32 | agatha-1 | elite | Agatha | Indigo Plateau | Arbok 53, Gengar 53, Golbat 53, Weezing 53, Gengar 54 | 54 |
| 33 | lance-1 | elite | Lance | Indigo Plateau | Seadra 54, Aerodactyl 54, Gyarados 54, Mega Charizard X 54, Dragonite 55 | 55 |
| 34 | rival-champion | champion | Rival (campeão) | Indigo Plateau | Mega Pidgeot 56, Vileplume 56, Marowak 56, Rapidash 56, Slowbro 56, Jolteon/Raichu 57 | 57 |
| 35 | mina-2 | other | Mina (revanche diária, dá Bottle Cap) | Porto de Vermilion | Mr. Mime 54, Wigglytuff 54, Alolan Ninetales 55 | 55 |
| 36 | jessie-james-5 | evil | Jessie & James (última) | Pokémon Road (Route 17) | Arbok 52, Weezing 52 | 52 |
| 37 | archer-3 | evil | Archer (revanche) | Rocket Hideout | Electrode 54, Golbat 54, Magmar 54, Weezing 54 | 54 |
| 38 | harjit-1 | other | Harjit (Coach Trainer) | Entrada da Cerulean Cave | Rhydon 52, Rapidash 52, Dewgong 52, Seaking 52, Nidoking 52 | 52 |
| 39 | mewtwo-1 | other | Mewtwo (5 min, depois captura; aura reflete Poké Balls) | Cerulean Cave (fundo) | Mewtwo 70 | — |
| 40 | morimoto-1 | other | Morimoto (Coach Trainer, diário) | Game Freak, Celadon Condominiums | Kangaskhan 54, Flareon 54, Vaporeon 54, Jolteon 54, Machamp 54, Dragonite 54 | 54 |
| 41 | brock-2 | other | Brock (revanche diária) | Pewter Gym | Onix 56, Kabutops 56, Omastar 56, Aerodactyl 56, Golem 57 | 57 |
| 42 | misty-2 | other | Misty (revanche diária) | Cerulean Gym | Golduck 56, Dewgong 56, Starmie 56, Vaporeon 56, Gyarados 57 | 57 |
| 43 | surge-2 | other | Lt. Surge (revanche diária) | Vermilion Gym | Electrode 56, Jolteon 56, Electabuzz 56, Magneton 56, Raichu 57 | 57 |
| 44 | erika-2 | other | Erika (revanche diária) | Celadon Gym | Tangela 56, Parasect 56, Victreebel 56, Exeggutor 56, Vileplume 57 | 57 |
| 45 | sabrina-2 | other | Sabrina (revanche diária) | Saffron Gym | Mr. Mime 56, Jynx 56, Hypno 56, Slowbro 56, Alakazam 57 | 57 |
| 46 | koga-2 | other | Koga (revanche diária) | Fuchsia Gym | Tentacruel 56, Golbat 56, Weezing 56, Venomoth 56, Muk 57 | 57 |
| 47 | blaine-2 | other | Blaine (revanche diária) | Cinnabar Gym | Magmar 56, Rapidash 56, Ninetales 56, Flareon 56, Arcanine 57 | 57 |
| 48 | blue-gym | other | Blue (novo líder) | Viridian Gym | Tauros 66, Gyarados 66, Aerodactyl 66, Alakazam 66, Exeggutor 66, Mega Charizard Y 68 | 68 |
| 49 | green-1 | other | Green (depois repete em Cerulean a cada vitória na Elite 4) | Cerulean Cave (local do Mewtwo) | Clefable 66, Gengar 66, Kangaskhan 66, Victreebel 66, Ninetales 66, Mega Blastoise 68 | 68 |
| 50 | master-trainers | other | Master Trainers (1×1 com a mesma espécie, sem itens nem EXP) | Toda Kanto | uma espécie lv 65–80 | — |
| 51 | red-1 | other | Red (após vencer 6 Master Trainers; repete a cada vitória na Elite 4) | Entrada do Indigo Plateau | Pikachu 85, Machamp 85, Arcanine 85, Lapras 85, Snorlax 85, Mega Venusaur 85 | 85 |
| 52 | lorelei-2 | elite | Lorelei (revanche) | Indigo Plateau | Dewgong 61, Jynx 61, Cloyster 61, Slowbro 61, Alolan Sandslash 61, Lapras 62 | 62 |
| 53 | bruno-2 | elite | Bruno (revanche) | Indigo Plateau | Onix 62, Hitmonlee 62, Hitmonchan 62, Poliwrath 62, Alolan Golem 62, Machamp 63 | 63 |
| 54 | agatha-2 | elite | Agatha (revanche) | Indigo Plateau | Arbok 63, Gengar 63, Golbat 63, Weezing 63, Alolan Marowak 63, Gengar 64 | 64 |
| 55 | lance-2 | elite | Lance (revanche) | Indigo Plateau | Seadra 64, Aerodactyl 64, Gyarados 64, Alolan Exeggutor 64, Mega Charizard X 64, Dragonite 65 | 65 |
| 56 | rival-champion-2 | champion | Rival (revanche) | Indigo Plateau | Mega Pidgeot 66, Vileplume 66, Marowak 66, Rapidash 66, Slowbro 66, Jolteon/Raichu 67 | 67 |

Sem time listado: líder do Fighting Dojo (Saffron, prêmio Hitmonlee ou Hitmonchan); Rocket da Nugget Bridge; três Rockets
de Lorelei (Route 10); Master Trainers de Ditto, Articuno, Zapdos, Moltres, Mewtwo, Mew, Meltan e Melmetal (só pedem o
Pokémon com CP alto, sem batalha).

Requisitos de entrada dos ginásios: Brock = ter um Pokémon de planta ou água; Misty = um Pokémon lv 15+; Surge = cortar a
árvore com Chop Down; Erika = mostrar um Pokémon "fofo" (o inicial serve); Sabrina = um Pokémon lv 45+; Koga = 50 espécies
capturadas; Blaine = Secret Key (Pokémon Mansion) + 5 Blaine Dolls (quiz: Magmar, Eight, Not Very Effective, False/What's
That?!, qualquer); Giovanni = abre depois da 7ª insígnia.

## Áreas (para `areas/lgpe.json`, em ordem de progressão)

| id | nome | kind | libera |
| --- | --- | --- | --- |
| pallet-town | Pallet Town | city | início |
| route-1 | Route 1 | route | início |
| viridian-city | Viridian City | city | início |
| route-22 | Route 22 | route | após entregar a encomenda ao Oak |
| route-2 | Route 2 | route | após rival-2 |
| viridian-forest | Viridian Forest | dungeon | após rival-2 |
| pewter-city | Pewter City | city | após Viridian Forest |
| route-3 | Route 3 | route | após Brock |
| route-4 | Route 4 (Pokémon Center perto do Mt. Moon) | route | após Brock |
| mt-moon | Mt. Moon | dungeon | após Brock |
| cerulean-city | Cerulean City | city | após Mt. Moon |
| route-24 | Route 24 (Nugget Bridge) | route | após Mt. Moon |
| route-25 | Route 25 (casa do Bill) | route | após Nugget Bridge |
| cerulean-robbed-house | Casa vigiada pela Jenny (Cerulean) | building | após Misty |
| route-5 | Route 5 (Day Care) | route | após Misty |
| underground-path-5-6 | Underground Path (Cerulean–Vermilion) | dungeon | após Misty |
| route-6 | Route 6 | route | após Misty |
| vermilion-city | Vermilion City | city | após Underground Path |
| diglett-cave | Diglett's Cave | dungeon | Vermilion (atravessar após Surge) |
| route-11 | Route 11 | route | Vermilion (Snorlax bloqueia o fim) |
| ss-anne | S.S. Anne | building | S.S. Ticket (Bill) |
| vermilion-gym | Vermilion Gym | building | após Chop Down |
| pewter-museum-back | Área atrás do museu de Pewter (Old Amber) | zone | Chop Down |
| route-2-east | Route 2 (casa do assistente, Light Up) | route | após Surge |
| route-9 | Route 9 | route | após Light Up |
| route-10 | Route 10 (Pokémon Center, Rockets e Lorelei) | route | após Light Up |
| rock-tunnel | Rock Tunnel | dungeon | Light Up |
| lavender-town | Lavender Town | city | após Rock Tunnel |
| pokemon-tower | Pokémon Tower | building | Lavender (topo só com Silph Scope) |
| route-8 | Route 8 | route | Lavender |
| underground-path-7-8 | Underground Path (Lavender–Celadon) | dungeon | Lavender |
| route-7 | Route 7 | route | Lavender |
| celadon-city | Celadon City | city | após Underground Path |
| celadon-department-store | Celadon Department Store | building | Celadon |
| game-corner | Game Corner | building | Celadon |
| rocket-hideout | Rocket Hideout | dungeon | após Erika |
| saffron-city | Saffron City | city | Tea (do Brock) entregue ao guarda |
| fighting-dojo | Fighting Dojo | building | Saffron |
| silph-co | Silph Co. (11 andares) | building | Saffron |
| saffron-gym | Saffron Gym | building | após Silph Co.; Pokémon lv 45+ |
| route-16 | Route 16 | route | Poké Flute (Snorlax de Celadon) |
| route-17 | Route 17 (Pokémon Road) | route | após Route 16 |
| route-18 | Route 18 | route | após Route 17 |
| route-12 | Route 12 | route | Poké Flute (Snorlax de Lavender) |
| route-13 | Route 13 | route | após Route 12 |
| route-14 | Route 14 | route | após Route 13 |
| route-15 | Route 15 | route | após Route 14 |
| fuchsia-city | Fuchsia City | city | Route 15 ou 18 |
| go-park | GO Park Complex | building | Fuchsia |
| fuchsia-gym | Fuchsia Gym | building | 50 espécies capturadas |
| celadon-pond | Lagoa de Celadon (TM26) | sea | Sea Skim |
| power-plant | Power Plant | dungeon | Sea Skim (pela Route 10) |
| route-19 | Route 19 | sea | Sea Skim |
| route-20 | Route 20 | sea | Sea Skim |
| seafoam-islands | Seafoam Islands | dungeon | Sea Skim + Strong Push |
| cinnabar-island | Cinnabar Island | city | após Seafoam |
| pokemon-mansion | Pokémon Mansion | building | Cinnabar |
| cinnabar-gym | Cinnabar Gym | building | Secret Key |
| route-21 | Route 21 | sea | após Blaine |
| viridian-gym | Viridian Gym | building | após Blaine (Key Stone com o Blue) |
| route-23 | Route 23 | route | 8 insígnias |
| victory-road | Victory Road | dungeon | 8 insígnias |
| indigo-plateau | Indigo Plateau | building | após Victory Road |
| kanto-sky | Céu de Kanto (voo alto) | zone | pós-jogo (Charizard/Aerodactyl/Dragonite) |
| cerulean-cave | Cerulean Cave | dungeon | pós-jogo (campeão) |
| celadon-condominiums | Celadon Condominiums (escritório da Game Freak) | building | pós-jogo |

## Encontros (para `encounters/lgpe.json`; `method: walk` salvo indicação — não há batalha com selvagem, só captura)

- route-1: Rattata, Pidgey, Oddish [Pikachu], Bellsprout [Eevee].
- route-22: Spearow, Nidoran♂, Nidoran♀. Surf (pós Sea Skim): Poliwag, Poliwhirl.
- route-2: Caterpie, Weedle.
- viridian-forest: Caterpie, Weedle, Pikachu, Metapod, Kakuna; Butterfree [Pikachu] (raro), Beedrill [Eevee] (raro); Bulbasaur raro (catch combo/Lure).
- route-3: Sandshrew [Pikachu], Ekans [Eevee], Mankey [Pikachu]; Charmander raro (combo/Lure).
- route-4: Magikarp à venda no Pokémon Center (`method: gift`, compra).
- mt-moon: Zubat, Geodude, Paras, Clefairy; Clefable e Onix (menos comuns); Chansey raro (combo/Lure).
- route-4 (leste do Mt. Moon): Psyduck.
- route-24 / route-25: Venonat, Meowth [Eevee]; Squirtle raro (combo/Lure).
- route-5: Jigglypuff, Abra, Growlithe [Pikachu], Vulpix [Eevee].
- diglett-cave: Diglett, Dugtrio.
- route-11: Drowzee, Mr. Mime.
- route-9 / route-10 (perto do rio): Krabby (chance). Surf na Route 10: Dratini, Dragonair (raros).
- rock-tunnel: Golbat, Graveler, Machop, Rhyhorn, Cubone; Kangaskhan (raro).
- pokemon-tower (após Silph Scope): Gastly, Haunter, Cubone.
- route-16: Doduo, Dodrio.
- route-17: Eevee, Ponyta, Rapidash.
- route-12 / route-13: Farfetch'd, Kingler. Surf ao sul de Lavender: Magikarp, Horsea, Seadra, Tentacool, Tentacruel.
- route-14 / route-15: Tauros, Venomoth; Scyther [Pikachu] / Pinsir [Eevee] (bem raros).
- route-6 (surf, norte de Vermilion): Goldeen, Seaking.
- power-plant: Voltorb, Electrode, Magnemite, Magneton, Electabuzz; Grimer, Muk [Pikachu]; Koffing, Weezing [Eevee]; Electrode disfarçado de item (`fixed`). Zapdos lv 50 (`fixed`; se fugir, volta após a Elite 4).
- route-19 / route-20 (surf): Staryu, Starmie + os de surf já vistos; Lapras raro (combo/Lure).
- seafoam-islands: Slowpoke, Slowbro, Seel, Dewgong, Jynx; surf: Shellder, Cloyster. Articuno lv 50 (`fixed`).
- pokemon-mansion: Magmar, Ditto.
- route-21 (grama antes de Pallet): Tangela (único lugar).
- route-23: Exeggcute, Exeggutor.
- victory-road: Hitmonlee/Hitmonchan (raros, combo/Lure). Moltres lv 50 (`fixed`).
- kanto-sky (pós-jogo, voando alto): Pidgey, Pidgeotto, Pidgeot, Spearow, Fearow; raros: Charizard, Dragonite; super raros: Articuno, Zapdos, Moltres.
- cerulean-cave: Chansey (comum aqui), Lickitung, Snorlax (raro, combo/Lure). Mewtwo lv 70 (`fixed`). Itens escondidos sorteados por dia (Golden Berries, fósseis, Master Ball).
- Snorlax `fixed` lv 34 (Route 16); segundo Snorlax `fixed` na Route 12 (nível não listado).

## Presentes, trocas in-game e fósseis (`method: gift` / `trade`)

- Inicial: Pikachu [Pikachu] / Eevee [Eevee] (nível não listado); não evolui.
- Bulbasaur lv 12 (Cerulean, ao lado do Pokémon Center; requer 30 capturas no total).
- Charmander lv 14 (Route 24; requer 50 capturas).
- Squirtle lv 16 (Officer Jenny, Vermilion; requer 60 capturas).
- Persian [Pikachu] / Arcanine [Eevee] (Vermilion; capturar 5 Growlithe [Pikachu] ou 5 Meowth [Eevee]; nível não listado).
- Hitmonlee ou Hitmonchan lv 30 (Fighting Dojo, Saffron).
- Lapras lv 34 (Silph Co. 7F). Porygon lv 34 (fora do Pokémon Center de Saffron, após Silph Co.).
- Magikarp (compra no Pokémon Center da Route 4).
- Trocas por forma de Alola: Rattata → Alolan Rattata (Cerulean); Geodude → Alolan Geodude (Vermilion); Diglett → Alolan Diglett (Lavender); Sandshrew → Alolan Sandshrew [Pikachu] / Vulpix → Alolan Vulpix [Eevee] (Celadon); Raichu → Alolan Raichu (Saffron); Marowak → Alolan Marowak (Fuchsia); Grimer → Alolan Grimer [Pikachu] / Meowth → Alolan Meowth [Eevee] (Cinnabar); Exeggutor → Alolan Exeggutor (Indigo Plateau).
- Fósseis: Helix Fossil (Omanyte) ou Dome Fossil (Kabuto) — escolha após vencer o Super Nerd no Mt. Moon; Old Amber (Aerodactyl) atrás do museu de Pewter (Chop Down). Reviver no laboratório de Cinnabar.

## Itens, TMs e técnicas

- TMs: TM01 Headbutt (Brock); TM29 Scald (Misty); TM10 Dig (Rocket atrás da casa vigiada, Cerulean); TM36 Thunderbolt (Surge); TM11 (pessoa atrás de árvore, Viridian); TM53 Mega Drain (Erika); TM40 Psychic (casa em Saffron); TM33 Calm Mind (Sabrina); TM14 Fly (casa na Route 16); TM32 Dazzling Gleam (casa após o Snorlax da Route 12); TM27 Toxic (Koga); TM26 Poison Jab (lagoa de Celadon, surf); TM24 X-Scissor (surf ao sul de Lavender); TM46 Fire Blast (Blaine); TM41 Earthquake (Giovanni).
- Secret Techniques (do inicial, não ocupam golpe): Chop Down (capitão do S.S. Anne); Light Up (assistente do Oak, Route 2); Sky Dash (homem dos balões, fora do Rocket Hideout); Strong Push (Safari warden, Fuchsia, após devolver o Gold Teeth); Sea Skim (rapaz da lagoa do Lapras, Fuchsia).
- Itens-chave: Town Map (mãe); Razz Berries (Oak); S.S. Ticket (Bill); Lift Key (Rocket Hideout); Silph Scope (Giovanni, Hideout); Poké Flute (Mr. Fuji); Tea (Celadon, com o Brock); Card Key (Silph 5F); Secret Key (Pokémon Mansion); Gold Teeth (Jessie & James, praia ao sul de Fuchsia); Blaine Dolls.
- Master Ball: presidente da Silph Co. (também pode surgir escondida na Cerulean Cave).
- Mega Evolução: Key Stone + Venusaurite, Charizardite X, Charizardite Y, Blastoisinite (Blue, laboratório do Oak, antes do Viridian Gym). Loja do Indigo Plateau (pós-jogo): Beedrillite, Pidgeotite, Alakazite, Slowbronite, Gengarite, Kangaskhanite, Pinsirite, Gyaradosite, Aerodactylite. Mewtwonite X e Y (Green).
- Outros: 5 Great Balls (Blue, Pewter); 5 Heart Scales (loja de bicicletas, Cerulean); Nugget (Nugget Bridge); Big Pearl diária (cuidar do Slowpoke, Pewter); Nugget diário (Diglett na casa do warden, Fuchsia); 30 Ultra Balls (assistente na Route 16, 40 espécies); Shalour Sable (Blue, S.S. Anne); Pewter Crunchies (Brock); Bottle Cap (Mina, revanche diária); Shiny Charm + diploma (Game Director, 150 capturados).
- Tutores do inicial: Cerulean — Pikachu: Zippy Zap; Eevee: Bouncy Bubble, Buzzy Buzz, Sizzly Slide. Celadon — Pikachu: Floaty Fall; Eevee: Baddy Bad, Glitzy Glow. Fuchsia — Pikachu: Splishy Splash; Eevee: Freezy Frost, Sappy Seed, Sparkly Swirl.
- Serviços: Move Reminder (Indigo Plateau, 1 Heart Scale); Hyper Training (velho do Day Care, Route 5, com Bottle Cap); Day Care (Route 5).

## Mecânicas (para o guia de mecânicas, texto nosso)

- Selvagens aparecem no mapa; não há batalha, só captura com movimento do Joy-Con; cada captura dá EXP ao time todo.
- Catch combo (mesma espécie seguida) aumenta IVs, chance de shiny e de raros; Lures também liberam raros.
- Sem PC nem pesca: a Box fica na mochila (inclusive entre lutas da Elite 4).
- Inicial parceiro (Pikachu/Eevee) não evolui, fica no ombro/cabeça; outro Pokémon segue fora da Poké Ball; alguns grandes servem de montaria (Lapras/Gyarados na água; Charizard/Aerodactyl/Dragonite voam alto após a Elite 4).
- Coach Trainers (balão "…", prêmio ao vencer) e Master Trainers (pós-jogo, roupa vermelha, 1×1 com a mesma espécie; 150 títulos = Grand Master).
- Lendários e Snorlax: 5 minutos para derrotar em batalha, depois tela de captura.
- GO Park Complex (Fuchsia): recebe Pokémon do Pokémon GO por Bluetooth; 20 parques de 50; recaptura obrigatória, sem volta.
- Mega Evolução liberada antes do Viridian Gym.
- Pós-jogo (ordem do detonado): voo alto e Master Trainers → loja de Mega Stones no Indigo Plateau → Mina → Jessie & James (Route 17) → Archer (Hideout) → Harjit e Cerulean Cave (Mewtwo) → Morimoto e Game Director → revanches diárias dos líderes → Blue (Viridian Gym) → Green (Cerulean Cave) → Master Trainers → Red (6 Master Trainers) → revanche da Elite 4 e do rival.

## Dúvidas

- Números de rota não aparecem no detonado; foram deduzidos pela geografia de Kanto (conferir na segunda fonte).
- Nível do inicial e dos presentes Persian/Arcanine não constam.
- Nível do Snorlax da Route 12 não consta (só o de Celadon, lv 34).
- Time da Rocket parceira de Archer na dupla da Silph Co. 5F e do líder do Fighting Dojo não constam.
- Rival usa Oddish/Gloom/Vileplume nas duas versões segundo o texto (sem variação de versão citada).
- Páginas 3, 27, 29, 33 e 50 do PDF só têm imagem (sem texto); podem ter mapas ou tabelas não extraídas.
