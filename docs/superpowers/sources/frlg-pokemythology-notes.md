# FireRed/LeafGreen — fatos extraídos do detonado da Pokémon Mythology

Fonte: https://www.pokemythology.net/detonados/detonado-firered-leafgreen/ (PDF "Detonado FireRed_LeafGreen – Pokémon
Mythology.pdf", salvo em 05/11/2026, 42 páginas).
Só **fatos de jogo** (chefes, níveis, encontros, itens) no nosso formato. Nenhuma prosa foi copiada. Golpes dos
chefes **não** constam no detonado: deixar `moves: []` e `verified: false` para golpes; espécies e níveis são
`verified: true` (uma fonte) até o analista conferir numa segunda. Rival chamado "Gary" no detonado (nome livre no jogo).

## Ordem da história e chefes (para `bosses/frlg.json`)

Variantes do rival por inicial do jogador (o rival pega o inicial com vantagem: Bulbasaur→Charmander,
Charmander→Squirtle, Squirtle→Bulbasaur).

| Order | Id | Kind | Nome | Local | Time (espécie lv) | Cap |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | rival-1 | rival | Rival (lab do Oak) | Oak's Lab, Pallet Town | inicial do rival (nível não listado) | — |
| 2 | rival-route22-1 | rival | Rival (opcional) | Route 22 | Pidgey 9, inicial do rival 9 | 9 |
| 3 | brock | gym | Brock (Rock) | Pewter Gym | Geodude 12, Onix 14 | 14 |
| 4 | misty | gym | Misty (Water) | Cerulean Gym | Staryu 18, Starmie 21 | 21 |
| 5 | rival-cerulean | rival | Rival | Nugget Bridge, saída norte de Cerulean (Route 24) | Pidgeotto 17, Rattata 15, Abra 16, inicial do rival 18 | 18 |
| 6 | rocket-cerulean | evil | Rocket Grunt (casa assaltada) | Cerulean City | não listado (dá TM28 Dig) | — |
| 7 | rival-ss-anne | rival | Rival | S.S. Anne | Pidgeotto 19, Raticate 16, Kadabra 18, inicial evoluído 20 | 20 |
| 8 | lt-surge | gym | Lt. Surge (Electric) | Vermilion Gym | Voltorb 21, Pikachu 18, Raichu 24 | 24 |
| 9 | erika | gym | Erika (Grass) | Celadon Gym | Victreebel 29, Vileplume 29, Tangela 24 | 29 |
| 10 | giovanni-1 | evil | Giovanni (Team Rocket) | Rocket Hideout B4F, Celadon | Onix 25, Rhyhorn 24, Kangaskhan 29 | 29 |
| 11 | rival-tower | rival | Rival | Pokémon Tower 2F, Lavender | Bulbasaur: Pidgeotto 25, Gyarados 22, Exeggcute 23, Kadabra 20, Charmeleon 25; Squirtle: Pidgeotto 25, Growlithe 22, Gyarados 23, Kadabra 20, Ivysaur 25; Charmander: Pidgeotto 25, Exeggcute 22, Growlithe 23, Kadabra 20, Wartortle 25 | 25 |
| 12 | ghost-marowak | other | Ghost Marowak (não capturável) | Pokémon Tower, penúltimo andar | Marowak (nível não listado) | — |
| 13 | fighting-dojo | other | Fighting Dojo (Karate Master) | Saffron City | não listado (prêmio Hitmonlee ou Hitmonchan) | — |
| 14 | rival-silph | rival | Rival | Silph Co. 7F (sala do teleporte) | Bulbasaur: Pidgeot 37, Gyarados 38, Exeggcute 35, Alakazam 35, Charizard 40; Squirtle: Pidgeot 37, Growlithe 38, Gyarados 35, Alakazam 35, Venusaur 40; Charmander: Pidgeot 37, Exeggcute 38, Growlithe 35, Alakazam 35, Blastoise 40 | 40 |
| 15 | giovanni-2 | evil | Giovanni (Team Rocket) | Silph Co. 11F | Nidorino 37, Rhyhorn 37, Kangaskhan 35, Nidoqueen 41 | 41 |
| 16 | sabrina | gym | Sabrina (Psychic) | Saffron Gym | Kadabra 38, Mr. Mime 37, Venomoth 38, Alakazam 43 | 43 |
| 17 | koga | gym | Koga (Poison) | Fuchsia Gym | Koffing 37, Muk 39, Koffing 37, Weezing 43 | 43 |
| 18 | blaine | gym | Blaine (Fire) | Cinnabar Gym | Growlithe 42, Ponyta 40, Rapidash 42, Arcanine 47 | 47 |
| 19 | bikers-three-island | other | Bikers (vários seguidos) | Three Island | Grimer/Koffing e evoluções 37–39 | 39 |
| 20 | giovanni-3 | gym | Giovanni (Ground) | Viridian Gym | Rhyhorn 45, Dugtrio 42, Nidoking 45, Nidoqueen 44, Rhyhorn 50 | 50 |
| 21 | rival-route22-2 | rival | Rival | Route 22/23 | Bulbasaur: Pidgeot 47, Rhyhorn 45, Gyarados 45, Exeggcute 45, Alakazam 47, Charizard 53; Squirtle: Pidgeot 47, Rhyhorn 45, Growlithe 45, Gyarados 45, Alakazam 47, Venusaur 53; Charmander: Pidgeot 47, Rhyhorn 45, Exeggcute 45, Growlithe 45, Alakazam 47, Blastoise 53 | 53 |
| 22 | lorelei-1 | elite | Lorelei (Ice) | Indigo Plateau | Dewgong 52, Cloyster 51, Slowbro 52, Jynx 54, Lapras 54 | 54 |
| 23 | bruno-1 | elite | Bruno (Fighting) | Indigo Plateau | Onix 51, Hitmonchan 53, Hitmonlee 53, Onix 54, Machamp 56 | 56 |
| 24 | agatha-1 | elite | Agatha (Ghost) | Indigo Plateau | Gengar 54, Golbat 54, Haunter 53, Arbok 56, Gengar 58 | 58 |
| 25 | lance-1 | elite | Lance (Dragon) | Indigo Plateau | Gyarados 56, Dragonair 54, Dragonair 54, Aerodactyl 58, Dragonite 60 | 60 |
| 26 | rival-champion-1 | champion | Rival (Campeão) | Indigo Plateau | Bulbasaur: Pidgeot 59, Rhydon 59, Gyarados 61, Exeggutor 59, Alakazam 57, Charizard 63; Squirtle: Pidgeot 59, Rhydon 59, Arcanine 61, Gyarados 59, Alakazam 57, Venusaur 63; Charmander: Pidgeot 59, Rhydon 59, Exeggutor 61, Arcanine 59, Alakazam 57, Blastoise 63 | 63 |
| 27 | rockets-mt-ember | evil | 2 Rocket Grunts (pós-jogo) | Mt. Ember, One Island | não listado | — |
| 28 | rockets-icefall | evil | Rockets (com Lorelei) | Icefall Cave, Four Island | não listado | — |
| 29 | rocket-scientist-warehouse | evil | Cientista Rocket (Sapphire) | Rocket Warehouse, Five Island | Electric + Porygon 46 (espécies não listadas) | 46 |
| 30 | lorelei-2 | elite | Lorelei (revanche) | Indigo Plateau | Dewgong 64, Cloyster 63, Piloswine 63, Jynx 66, Lapras 66 | 66 |
| 31 | bruno-2 | elite | Bruno (revanche) | Indigo Plateau | Steelix 65, Hitmonchan 65, Hitmonlee 65, Steelix 66, Machamp 68 | 68 |
| 32 | agatha-2 | elite | Agatha (revanche) | Indigo Plateau | Gengar 66, Crobat 66, Gengar 70, Misdreavus 65, Arbok 68 | 70 |
| 33 | lance-2 | elite | Lance (revanche) | Indigo Plateau | Gyarados 68, Kingdra 66, Dragonite 66, Aerodactyl 70, Dragonite 72 | 72 |
| 34 | rival-champion-2 | champion | Rival (Campeão, revanche) | Indigo Plateau | Bulbasaur: Heracross 72, Tyranitar 72, Gyarados 73, Exeggutor 73, Alakazam 73, Charizard 75; Squirtle: Heracross 72, Tyranitar 72, Arcanine 73, Gyarados 73, Alakazam 73, Venusaur 75; Charmander: Heracross 72, Tyranitar 72, Exeggutor 73, Arcanine 73, Alakazam 73, Blastoise 75 | 75 |

Liga: o Pokémon mais forte de cada membro segura Sitrus Berry; usam Full Restore com HP baixo. Nível sugerido: 65
(primeira vez) e 75 (revanche). Revanche liberada ao ajudar Lorelei na Icefall Cave.

## Áreas (para `areas/frlg.json`, kind e ordem de liberação)

| Ordem | Id | Nome | Kind | Libera |
| --- | --- | --- | --- | --- |
| 1 | pallet-town | Pallet Town | city | início |
| 2 | route-1 | Route 1 | route | início |
| 3 | viridian-city | Viridian City | city | início (ginásio só com 7 insígnias) |
| 4 | route-22 | Route 22 | route | após entregar o Oak's Parcel |
| 5 | route-2 | Route 2 | route | após Pokédex |
| 6 | viridian-forest | Viridian Forest | dungeon | após Pokédex |
| 7 | pewter-city | Pewter City | city | após Viridian Forest |
| 8 | pewter-museum | Pewter Museum of Science | building | Pewter (entrada lateral com Cut) |
| 9 | route-3 | Route 3 | route | após Boulder Badge |
| 10 | mt-moon | Mt. Moon | dungeon | após Route 3 |
| 11 | route-4 | Route 4 | route | após Mt. Moon |
| 12 | cerulean-city | Cerulean City | city | após Route 4 |
| 13 | route-24 | Route 24 (Nugget Bridge) | route | Cerulean |
| 14 | route-25 | Route 25 (Bill's House) | route | Cerulean |
| 15 | route-5 | Route 5 | route | após S.S. Ticket |
| 16 | underground-path-5-6 | Underground Path (Routes 5–6) | building | após S.S. Ticket |
| 17 | route-6 | Route 6 | route | após S.S. Ticket |
| 18 | vermilion-city | Vermilion City | city | após Underground Path |
| 19 | ss-anne | S.S. Anne | building | S.S. Ticket |
| 20 | route-11 | Route 11 | route | Vermilion |
| 21 | digletts-cave | Diglett's Cave | dungeon | Vermilion (Route 11) |
| 22 | route-9 | Route 9 | route | HM01 Cut |
| 23 | route-10 | Route 10 | route | HM01 Cut |
| 24 | rock-tunnel | Rock Tunnel | dungeon | HM05 Flash recomendado |
| 25 | lavender-town | Lavender Town | city | após Rock Tunnel |
| 26 | route-8 | Route 8 | route | Lavender |
| 27 | underground-path-7-8 | Underground Path (Routes 7–8) | building | Lavender |
| 28 | route-7 | Route 7 | route | Underground Path |
| 29 | celadon-city | Celadon City | city | Route 7 |
| 30 | rocket-hideout | Rocket Hideout (Game Corner) | dungeon | Celadon |
| 31 | route-16 | Route 16 | route | Celadon (Snorlax: Poké Flute) |
| 32 | pokemon-tower | Pokémon Tower | building | Silph Scope |
| 33 | saffron-city | Saffron City | city | Tea |
| 34 | silph-co | Silph Co. | building | Saffron |
| 35 | route-12 | Route 12 | route | Poké Flute |
| 36 | route-13 | Route 13 | route | Poké Flute |
| 37 | route-14 | Route 14 | route | Poké Flute |
| 38 | route-15 | Route 15 | route | Poké Flute |
| 39 | route-17 | Route 17 (Cycling Road) | route | Poké Flute + Bicycle |
| 40 | route-18 | Route 18 | route | Poké Flute + Bicycle |
| 41 | fuchsia-city | Fuchsia City | city | Poké Flute |
| 42 | safari-zone | Safari Zone | zone | Fuchsia ($500) |
| 43 | route-19 | Route 19 | sea | HM03 Surf + Soul Badge |
| 44 | route-20 | Route 20 | sea | Surf |
| 45 | seafoam-islands | Seafoam Islands | dungeon | Surf (precisa Strength) |
| 46 | power-plant | Power Plant | dungeon | Surf (a partir da Route 10) |
| 47 | route-21 | Route 21 | sea | Surf (Pallet → Cinnabar) |
| 48 | cinnabar-island | Cinnabar Island | city | Surf |
| 49 | pokemon-mansion | Pokémon Mansion | building | Cinnabar |
| 50 | one-island | One Island (Knot Island) | city | após Volcano Badge (Bill) |
| 51 | kindle-road | Kindle Road | route | One Island |
| 52 | ember-spa | Ember Spa | building | One Island |
| 53 | mt-ember | Mt. Ember | dungeon | One Island (Rock Smash e Strength) |
| 54 | treasure-beach | Treasure Beach | route | One Island |
| 55 | two-island | Two Island (Boon Island) | city | Tri-Pass |
| 56 | three-island | Three Island (Kin Island) | city | Tri-Pass |
| 57 | bond-bridge | Bond Bridge | route | após os bikers |
| 58 | berry-forest | Berry Forest | dungeon | Three Island |
| 59 | route-23 | Route 23 | route | 8 insígnias |
| 60 | victory-road | Victory Road | dungeon | 8 insígnias (Strength) |
| 61 | indigo-plateau | Indigo Plateau | building | após Victory Road |
| 62 | ruby-path | Ruby Path (Mt. Ember) | dungeon | pós-jogo (National Dex) |
| 63 | dunsparce-tunnel | Dunsparce Tunnel / caverna da Three Island | dungeon | pós-jogo (Rainbow Pass) |
| 64 | four-island | Four Island (Floe Island) | city | Rainbow Pass |
| 65 | icefall-cave | Icefall Cave | dungeon | Rainbow Pass (Surf) |
| 66 | seven-island | Seven Island (Quest Island) | city | Rainbow Pass |
| 67 | trainer-tower | Trainer Tower | building | Rainbow Pass |
| 68 | sevault-canyon | Sevault Canyon / Tanoby Key | route | Rainbow Pass (Strength) |
| 69 | tanoby-ruins | Tanoby Ruins (7 câmaras) | dungeon | após mover as pedras do Tanoby Key |
| 70 | six-island | Six Island (Fortune Island) | city | Rainbow Pass |
| 71 | dotted-hole | Dotted Hole | dungeon | Six Island (Cut) |
| 72 | five-island | Five Island (Chrono Island) | city | Rainbow Pass |
| 73 | rocket-warehouse | Rocket Warehouse | building | após Ruby e Dotted Hole |
| 74 | cerulean-cave | Cerulean Cave (Unknown Dungeon) | dungeon | National Dex + Ruby e Sapphire entregues |
| 75 | navel-rock | Navel Rock | dungeon | Mystery Ticket (evento) |
| 76 | birth-island | Birth Island | zone | Aurora Ticket (evento) |

## Encontros (para `encounters/frlg.json`; method `walk` salvo indicação)

- Route 1: sem captura antes das Poké Balls (espécies não listadas).
- Route 22: Mankey, Pidgey.
- Viridian Forest: insetos (espécies não listadas); Pikachu ~5%.
- Route 3: Jigglypuff.
- Mt. Moon (`cave`): Clefairy (raro); espécies restantes não listadas.
- Diglett's Cave (`cave`): Diglett, Dugtrio.
- Routes 12–15: Ditto (raro); aquáticos com vara (espécies não listadas).
- Safari Zone: comuns Nidoran♂, Nidoran♀, Nidorina, Nidorino, Paras, Venonat, Exeggcute, Rhyhorn, Doduo; raros
  Parasect, Venomoth, Chansey, Kangaskhan, Tauros, Scyther [FireRed], Pinsir [LeafGreen]. `fish` (Super Rod): Goldeen,
  Seaking, Dratini; Dragonair ~1%.
- Power Plant: só elétricos (espécies não listadas); Voltorb disfarçado de item.
- Dunsparce Tunnel (Three Island, pós-jogo): Dunsparce.
- Icefall Cave (`cave`): Swinub; Delibird [FireRed], Sneasel [LeafGreen] (raros).
- Tanoby Ruins (`cave`, após o terremoto): câmara 1 Unown ?, A; 2 C, D, H, O, U; 3 E, I, N, S; 4 J, L, P, Q, R;
  5 F, G, K, T, Y; 6 B, M, V, W, X; 7 !, Z.
- Cerulean Cave: comuns Golbat, Parasect, Primeape, Machoke, Kadabra, Magneton, Ditto; raros Wobbuffet, Electrode;
  `rock_smash` Geodude, Graveler; `surf` Psyduck/Slowpoke, Golduck/Slowbro.
- Lendários e fixos (`method: fixed`):
  - Snorlax lv 30 ×2 (Route 12 e Route 16; precisa Poké Flute; Leftovers no lugar depois).
  - Hypno lv 30 (Berry Forest, evento da Lostelle; capturável).
  - Zapdos lv 50 (Power Plant; após Surf).
  - Articuno lv 50 (Seafoam Islands; após Surf e Soul Badge).
  - Moltres lv 50 (topo do Mt. Ember; após Volcano Badge, Strength e Rock Smash).
  - Raikou/Entei/Suicune (`walk`, errante pela grama de Kanto após a Elite Four; um só por jogo: Squirtle→Raikou,
    Bulbasaur→Entei, Charmander→Suicune; foge no primeiro turno; nível não listado).
  - Mewtwo lv 70 (Cerulean Cave, fim; golpes citados: Psychic, Swift, Recover, Safeguard).
  - Ho-Oh lv 70 e Lugia lv 70 (Navel Rock; Mystery Ticket).
  - Deoxys (Birth Island; Aurora Ticket; Attack Forme [FireRed], Defense Forme [LeafGreen]; nível não listado).

## Presentes, trocas in-game, fósseis e ovos (`method: gift` / `trade`)

- Inicial: Bulbasaur/Charmander/Squirtle (Oak's Lab; nível não listado).
- Fósseis: Helix Fossil (Omanyte) ou Dome Fossil (Kabuto), escolha de um no fim do Mt. Moon; Old Amber (Aerodactyl),
  Pewter Museum (entrada lateral, Cut). Revividos no Cinnabar Lab.
- Eevee: último andar do prédio dos fundos (Celadon Mansion), Celadon.
- Hitmonlee ou Hitmonchan (um): Fighting Dojo, Saffron.
- Lapras: Silph Co. 7F, após o rival.
- Togepi (ovo; 1000 passos): velho numa ilhota a oeste da Five Island (Water Labyrinth).
- Trocas in-game: não listadas no detonado.

## TMs/HMs e itens-chave (local)

- TM39 Rock Tomb (Brock); TM03 Water Pulse (Misty); TM34 Shock Wave (Lt. Surge); TM19 Giga Drain (Erika); TM04 Calm
  Mind (Sabrina); TM06 Toxic (Koga); TM38 Fire Blast (Blaine); TM26 Earthquake (Giovanni); TM28 Dig (Rocket, Cerulean);
  TM42 (Five Island, troca por Lemonade numa rocha; nome não listado).
- HM01 Cut (capitão do S.S. Anne); HM02 Fly (casa a oeste de Celadon); HM03 Surf (Secret House, Safari Zone); HM04
  Strength (Warden, Fuchsia, pela Gold Teeth); HM05 Flash (assistente do Oak, Route 2, ≥10 espécies); HM06 Rock Smash
  (Ember Spa, One Island); HM07 Waterfall (Icefall Cave).
- Itens-chave: Potion (PC do quarto); Oak's Parcel (Poké Mart de Viridian); Town Map (casa do rival); Teachy TV
  (velho de Viridian); Running Shoes (saída leste de Pewter); Fame Checker (rival, Cerulean); S.S. Ticket (Bill);
  VS Seeker (Pokémon Center de Vermilion); Old Rod (Vermilion); Bike Voucher (Pokémon Fan Club, Vermilion) → Bicycle
  (Cerulean); Lift Key e Silph Scope (Rocket Hideout); Tea (Celadon Mansion); Poké Flute (Mr. Fuji); Card Key (Silph
  Co. 5F); Master Ball (presidente da Silph); Good Rod (Fuchsia); Super Rod (casa na Route 12); Gold Teeth (Safari
  Zone); Secret Key (Pokémon Mansion); Meteorite e Tri-Pass (Celio, One Island); Moon Stone (pai da Lostelle, pelo
  Meteorite); Rainbow Pass (Celio, pelo Ruby); Ruby (Ruby Path); Sapphire (Rocket Warehouse); Rare Candy (casa do
  Warden, Strength); Leftovers (lugar dos Snorlax); Nugget (Nugget Bridge); Metal Coat (Five Island, sul).
- Itens da Cerulean Cave: Nugget, Full Restore ×2, Max Elixir, PP Up, Ultra Ball ×2, Max Revive.
- Move Tutors: Mega Punch/Mega Kick (Route 4); Dream Eater (Viridian, com Cut); Frenzy Plant, Blast Burn, Hydro
  Cannon (Two Island; só o inicial final, em 1º no time).

## Mecânicas (para o guia de mecânicas, texto nosso)

- Sevii Islands: One a Three com o Tri-Pass; Four a Seven com o Rainbow Pass; Navel Rock e Birth Island por tickets de
  evento, a partir de Vermilion.
- National Dex: exige 60 espécies registradas; o Oak entrega após o Hall of Fame.
- Trocas com Ruby/Sapphire/Emerald/Colosseum/XD: liberadas ao entregar Ruby e Sapphire ao Celio.
- VS Seeker: pede revanche de treinadores.
- Safari Zone: $500, 30 Safari Balls, 600 passos; isca, pedra ou bola.
- Trainer Tower: subida cronometrada; prêmio pelo tempo; níveis igualados ao seu mais forte.
- Pokémon Daycare/Breeding Center na Four Island.
- Teachy TV: tutorial de captura e batalha.
- Teste no Cinnabar Gym: quiz que pula treinadores.
- Cães lendários errantes: o local muda a cada troca de rota.

## Pós-jogo (ordem de eventos)

1. National Dex do Oak (60 espécies). 2. Vermilion → One Island; Celio pede Ruby e Sapphire. 3. Rockets do Mt. Ember
→ Ruby Path → Ruby → Rainbow Pass. 4. Four Island: Icefall Cave, HM07, ajudar Lorelei (libera revanche da Liga).
5. Seven Island: Trainer Tower e Tanoby Ruins. 6. Six Island: Dotted Hole (Sapphire roubada). 7. Five Island: Rocket
Warehouse, recuperar a Sapphire; trocas com Hoenn e Cerulean Cave liberadas. 8. Cães errantes. 9. Mewtwo. 10. Revanche
da Elite Four e do rival. 11. Navel Rock e Birth Island (tickets de evento).

## Dúvidas

- Níveis do inicial, do primeiro rival e do Ghost Marowak não aparecem no detonado.
- Nível dos cães errantes e do Deoxys não aparece.
- Time do Karate Master (Fighting Dojo) e dos grunts não aparece.
- TM42 não tem nome no detonado (troca por Lemonade na Five Island).
- O detonado diz "HM05 na rota 2" e "ao sul da Diglett's Cave": o local exato do assistente do Oak fica ambíguo.
- Espécies do cientista da Rocket Warehouse só como "elétricos e Porygon".
- Dois Snorlax: o detonado cita "Route 16–18" e "Route 12–15" sem fixar a rota exata.
