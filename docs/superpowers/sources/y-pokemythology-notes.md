# Yellow — fatos extraídos do detonado da Pokémon Mythology

Fonte: https://www.pokemythology.net/detonados/detonado-yellow/ (PDF salvo em 05/11/2026, 15 páginas; o conteúdo vai até a
página 12, o resto é rodapé do site). Só **fatos de jogo** (chefes, níveis, encontros, itens) no nosso formato. Nenhuma
prosa foi copiada. Golpes dos chefes **não** constam no detonado: deixar `moves: []` e `verified: false` para golpes;
espécies e níveis são `verified: true` (uma fonte) até o analista conferir numa segunda. O detonado chama o rival de
"Gary" (no jogo em inglês o padrão é "Blue"); os ids usam `rival-N`. Várias seções do Yellow repetem o texto do Red/Blue
(Safari Zone, Seafoam, Victory Road, Unknown Dungeon).

## Ordem da história e chefes (para `bosses/y.json`)

| Order | Id | Kind | Nome | Local | Time (espécie lv) | Cap |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | rival-1 | rival | Gary (Blue) | Laboratório do Prof. Oak, Pallet Town | Eevee, nível não listado | — |
| 2 | rival-2 | rival | Gary (Blue) | Route 22 (oeste de Viridian) | time não listado ("lv 9") | 9 |
| 3 | brock-1 | gym | Brock (Boulder Badge) | Pewter Gym | Geodude 10, Onix 12 | 12 |
| 4 | jessie-james-1 | evil | Jessie & James (Team Rocket) | Mt. Moon, fim da caverna | time não listado | — |
| 5 | super-nerd-mtmoon | other | Cientista (fósseis) | Mt. Moon | time não listado | — |
| 6 | misty-1 | gym | Misty (Cascade Badge) | Cerulean Gym | Staryu 18, Starmie 20 | 20 |
| 7 | rival-3 | rival | Gary (Blue) | Nugget Bridge (norte de Cerulean) | 4 Pokémon, Pidgeotto primeiro; espécies e níveis não listados | — |
| 8 | rocket-cerulean | evil | Rocket (casa assaltada) | Cerulean City | time não listado | — |
| 9 | rival-4 | rival | Gary (Blue) | S.S. Anne | time não listado ("até lv 20") | 20 |
| 10 | lt-surge-1 | gym | Lt. Surge (Thunder Badge) | Vermilion Gym (2 interruptores nas lixeiras) | Raichu 28 | 28 |
| 11 | erika-1 | gym | Erika (Rainbow Badge) | Celadon Gym (precisa de Cut) | Weepinbell 32, Gloom 32, Tangela 30 | 32 |
| 12 | jessie-james-2 | evil | Jessie & James | Rocket Hideout B4F | time não listado | — |
| 13 | giovanni-1 | evil | Giovanni (Team Rocket) | Rocket Hideout B4F | Pokémon de terra, espécies e níveis não listados | — |
| 14 | rival-5 | rival | Gary (Blue) | Pokémon Tower, 2º andar | time não listado | — |
| 15 | ghost-marowak | other | Fantasma (Marowak, não capturável) | Pokémon Tower (precisa do Silph Scope) | Marowak, nível não listado | — |
| 16 | jessie-james-3 | evil | Jessie & James (Pokémon evoluídos) | Pokémon Tower, topo | time não listado | — |
| 17 | rival-6 | rival | Gary (Blue) | Silph Co., sala do teletransporte do 3F | time não listado ("até lv 40") | 40 |
| 18 | jessie-james-4 | evil | Jessie & James | Silph Co. | time não listado | — |
| 19 | giovanni-2 | evil | Giovanni (Team Rocket) | Silph Co., sala do presidente | mesmas espécies do Hideout, "até lv 40" | 40 |
| 20 | karate-master | other | Karate Master (Fighting Dojo) | Saffron City | time não listado (prêmio: Hitmonlee ou Hitmonchan) | — |
| 21 | sabrina-1 | gym | Sabrina (Marsh Badge) | Saffron Gym (teletransportes) | Abra 50, Kadabra 50, Alakazam 50 | 50 |
| 22 | koga-1 | gym | Koga (Soul Badge) | Fuchsia Gym | Venonat 44, Venonat 46, Venonat 48, Venomoth 50 | 50 |
| 23 | blaine-1 | gym | Blaine (Volcano Badge) | Cinnabar Gym (precisa da Secret Key) | Ninetales 48, Rapidash 50, Arcanine 54 | 54 |
| 24 | giovanni-3 | gym | Giovanni (Earth Badge) | Viridian Gym | Dugtrio 50, Persian 53, Nidoqueen 52, Nidoking 55, Rhydon 55 | 55 |
| 25 | rival-7 | rival | Gary (Blue) | Route 22, antes da Victory Road | time não listado | — |
| 26 | lorelei | elite | Lorelei (água/gelo) | Indigo Plateau | Dewgong 54, Cloyster 53, Slowbro 54, Jynx 56, Lapras 56 | 56 |
| 27 | bruno | elite | Bruno (pedra/lutador) | Indigo Plateau | Onix 53, Hitmonchan 55, Hitmonlee 55, Onix 56, Machamp 56 | 56 |
| 28 | agatha | elite | Agatha (fantasma/veneno) | Indigo Plateau | Gengar 56, Golbat 56, Haunter 55, Arbok 58, Gengar 60 | 60 |
| 29 | lance | elite | Lance (dragão) | Indigo Plateau | Gyarados 58, Dragonair 56, Dragonair 56, Aerodactyl 60, Dragonite 62 | 62 |
| 30 | rival-champion | champion | Gary (Blue), campeão | Indigo Plateau | Sandslash 61, Alakazam 59, Exeggutor 61, mais 3 pela evolução do Eevee (Jolteon: Cloyster 61, Ninetales 63, Jolteon 65; Flareon: Magneton 61, Cloyster 63, Flareon 65; Vaporeon: Ninetales 61, Magneton 63, Vaporeon 65) | 65 |

Ordem 20–22 (Saffron e Fuchsia) é a do detonado; o jogo deixa escolher o caminho até Fuchsia.

## Áreas (para `areas/y.json`, kind e ordem de liberação)

Mesmo mapa do Red/Blue; diferenças de liberação citadas no detonado ficam na coluna "Libera".

| Id | Nome | Kind | Libera |
| --- | --- | --- | --- |
| pallet-town | Pallet Town | city | início |
| route-1 | Route 1 | route | início |
| viridian-city | Viridian City | city | início (ginásio só no fim) |
| route-22 | Route 22 | route | após o Pokédex |
| route-2 | Route 2 | route | após o Pokédex |
| viridian-forest | Viridian Forest | dungeon | após o Pokédex |
| pewter-city | Pewter City | city | após Viridian Forest |
| pewter-museum | Pewter Museum of Science | building | anexo do Old Amber só com Cut |
| route-3 | Route 3 | route | após Brock |
| route-4 | Route 4 | route | após Brock (Pokémon Center antes do Mt. Moon) |
| mt-moon | Mt. Moon | dungeon | após Brock |
| cerulean-city | Cerulean City | city | após Mt. Moon |
| nugget-bridge | Nugget Bridge (Route 24) | route | após Mt. Moon |
| route-25 | Route 25 / Bill's House | route | após Nugget Bridge |
| route-5 | Route 5 | route | após o S.S. Ticket |
| underground-path-5-6 | Underground Path (norte-sul) | building | após o S.S. Ticket |
| route-6 | Route 6 | route | após o S.S. Ticket |
| vermilion-city | Vermilion City | city | após o S.S. Ticket |
| ss-anne | S.S. Anne | building | com o S.S. Ticket |
| route-11 | Route 11 | route | após Vermilion |
| digletts-cave | Diglett's Cave | dungeon | após Vermilion |
| route-9 | Route 9 | route | com Cut |
| route-10 | Route 10 | route | com Cut |
| rock-tunnel | Rock Tunnel | dungeon | com Cut |
| lavender-town | Lavender Town | city | após Rock Tunnel |
| pokemon-tower | Pokémon Tower | building | topo só com o Silph Scope |
| route-8 | Route 8 | route | após Lavender |
| underground-path-7-8 | Underground Path (leste-oeste) | building | após Lavender |
| route-7 | Route 7 | route | após Lavender |
| celadon-city | Celadon City | city | após Lavender |
| celadon-mansion | Celadon Mansion | building | após Celadon |
| celadon-dept-store | Celadon Dept. Store | building | após Celadon |
| game-corner | Game Corner | building | após Celadon |
| rocket-hideout | Rocket Hideout | dungeon | após Celadon |
| route-16 | Route 16 | route | Snorlax com a Poké Flute; casa do HM02 com Cut |
| saffron-city | Saffron City | city | dar Fresh Water ao guarda |
| silph-co | Silph Co. | building | após entrar em Saffron |
| fighting-dojo | Fighting Dojo | building | após entrar em Saffron |
| cycling-road | Cycling Road (Route 17) | route | com Bicycle e Snorlax da Route 16 acordado |
| route-18 | Route 18 | route | após a Cycling Road |
| route-12 | Route 12 | route | Snorlax com a Poké Flute |
| route-13 | Route 13 | route | após a Route 12 |
| route-14 | Route 14 | route | após a Route 12 |
| route-15 | Route 15 | route | após a Route 12 |
| fuchsia-city | Fuchsia City | city | pela Cycling Road ou pela Route 12 |
| safari-zone | Safari Zone | zone | após Fuchsia |
| power-plant | Power Plant | dungeon | com Surf |
| route-19 | Route 19 | sea | com Surf |
| route-20 | Route 20 | sea | com Surf |
| seafoam-islands | Seafoam Islands | dungeon | com Surf e Strength |
| cinnabar-island | Cinnabar Island | city | com Surf |
| pokemon-mansion | Pokémon Mansion | building | após chegar em Cinnabar |
| route-21 | Route 21 | sea | com Surf |
| route-23 | Route 23 | route | com as 8 insígnias (guardas conferem) |
| victory-road | Victory Road | dungeon | com as 8 insígnias, Surf e Strength |
| indigo-plateau | Indigo Plateau | building | após Victory Road |
| unknown-dungeon | Unknown Dungeon (Cerulean Cave) | dungeon | pós-jogo, com Surf pela Route 24 |

## Encontros (para `encounters/y.json`; method `walk` salvo indicação)

- Route 1 / Route 2: Pidgey, Rattata.
- Route 22: Mankey, Nidoran♂, Nidoran♀, Spearow.
- Viridian Forest: Caterpie, Metapod; Pidgeotto (procurar bem).
- Route 3 / Route 4: Sandshrew, Jigglypuff.
- Mt. Moon (`cave`): Geodude, Zubat, Paras; Clefairy (rara). Moon Stone no chão.
- Rotas ao norte de Cerulean (Route 24/25): Oddish, Bellsprout, Abra.
- Diglett's Cave (`cave`): Diglett; alguns Dugtrio.
- Rock Tunnel (`cave`): Onix, Machop, Zubat, Geodude.
- Pokémon Tower: Gastly, Haunter, Cubone.
- Cycling Road: Raticate, Doduo; Fearow (raro).
- Routes 12–15: Venonat; Ditto (raro).
- Safari Zone (texto repetido do Red/Blue): Exeggcute, família Nidoran, Venonat, Paras, Doduo, Rhyhorn; raros: Tauros, Chansey, Scyther, Pinsir, Kangaskhan, Venomoth, Parasect. `fish` com Super Rod: Dratini; senão Psyduck, Slowpoke, Krabby.
- Power Plant: Pikachu, Magnemite, Magneton; Voltorb e Electrode disfarçados de item (`fixed`).
- Seafoam Islands (`cave`/`surf`): Seel, Psyduck, Golduck, Horsea, Krabby, Zubat, Golbat, Shellder, Staryu, Kingler, Dewgong, Seadra, Slowbro.
- Victory Road (`cave`): Machop, Machoke, Geodude, Graveler, Zubat, Golbat, Onix, Marowak, Venomoth.
- Unknown Dungeon (`cave`): Dodrio, Venomoth, Kadabra, Rhydon, Marowak, Electrode, Chansey, Wigglytuff, Ditto, Parasect, Raichu, Sandslash/Arbok, Golbat, Hypno, Magneton.
- Snorlax (`fixed`, 2 no jogo): Route 16 e Route 12; acordar com a Poké Flute. Nível não listado.
- Lendários (`method: fixed`, nível não listado): Zapdos (Power Plant); Articuno (Seafoam Islands); Moltres (Victory Road); Mewtwo (Unknown Dungeon, andar mais baixo, canto inferior esquerdo, pós-jogo).
- Mew: o detonado remete ao guia separado e repete os avisos sobre os treinadores da Route 25 e da Route 8.

## Presentes, trocas in-game e fósseis (`method: gift` / `trade`)

- Inicial: Pikachu lv 5 (Prof. Oak, Pallet Town); anda fora da Poké Ball atrás do jogador.
- Bulbasaur lv 10 (menina na casa ao lado do Pokémon Center de Cerulean; exige Pikachu feliz).
- Charmander lv 10 (treinador ao norte da Nugget Bridge).
- Squirtle lv 10 (Officer Jenny, Vermilion City; exige a Thunder Badge).
- Eevee lv 25 (Celadon Mansion, entrada dos fundos, último andar).
- Lapras lv 25 (Silph Co., funcionário perto da sala do rival).
- Hitmonlee ou Hitmonchan (Fighting Dojo, após vencer o mestre).
- Fósseis: Helix Fossil → Omanyte ou Dome Fossil → Kabuto (Mt. Moon, após o cientista); Old Amber → Aerodactyl (anexo do Pewter Museum, Cut). Revividos no Cinnabar Lab (nível não listado).
- Trocas in-game: o detonado não cita nenhuma.

## TMs/HMs e itens-chave (localização)

| Item | Local |
| --- | --- |
| Potion | PC do quarto, Pallet Town; outro com o rapaz na Route 1 |
| Oak's Parcel | Poké Mart de Viridian |
| Pokédex | Prof. Oak, após entregar o Oak's Parcel |
| Town Map | Irmã do rival, Pallet Town |
| Moon Stone | Mt. Moon |
| Nugget | Prêmio por vencer os 5 treinadores da Nugget Bridge |
| S.S. Ticket | Bill, Route 25 |
| TM28 Dig | Rocket da casa assaltada, Cerulean |
| Old Rod | Fishing Guru, à esquerda do Pokémon Center de Vermilion |
| Bike Voucher → Bicycle | Pokémon Fan Club (acima do ginásio de Vermilion) → Bike Shop de Cerulean |
| HM01 Cut | Capitão do S.S. Anne |
| HM05 Flash | Assistente do Oak, ao sul da saída do Diglett's Cave |
| Old Amber | Anexo do Pewter Museum (Cut) |
| HM02 Fly | Casa a oeste de Celadon (Cut) |
| TMs (números não citados) | Menina do terraço do Celadon Dept. Store, por Fresh Water, Soda Pop e Lemonade |
| Lift Key | Rocket do Rocket Hideout |
| Silph Scope | Giovanni, Rocket Hideout |
| Poké Flute | Mr. Fuji, Lavender Town |
| Card Key | Silph Co. 5F |
| Master Ball | Presidente da Silph Co. |
| Super Rod | Casa na Route 12 (depois do Snorlax) |
| Good Rod | Parente do Fishing Guru, Fuchsia City |
| HM03 Surf + Gold Teeth | Casa secreta da Safari Zone |
| HM04 Strength | Safari Zone Warden (entregar o Gold Teeth) |
| Rare Candy | Atrás da pedra do Warden (Strength) |
| Secret Key | Pokémon Mansion |

Lojas: Celadon Dept. Store (5 andares); Poké Mart + Pokémon Center juntos no Indigo Plateau.

## Mecânicas (para o guia de mecânicas, texto nosso)

- Pikachu inicial segue o jogador fora da Poké Ball; a felicidade dele libera o Bulbasaur de Cerulean.
- Os três iniciais de Kanto são presentes (Bulbasaur, Charmander, Squirtle lv 10).
- O rival começa com Eevee; o time final depende da evolução do Eevee.
- Jessie e James substituem parte dos Rockets (Mt. Moon, Rocket Hideout, Pokémon Tower, Silph Co.).
- Rocket Hideout e Viridian Gym: pisos de setas; Saffron Gym e Silph Co.: teletransportes; cura no Silph Co. 9F.
- Pokémon Tower: Silph Scope revela os fantasmas; piso de cura.
- Cycling Road só com Bicycle.
- Seafoam Islands: correnteza até as pedras caírem nos buracos (Strength).
- Cinnabar Gym: quiz; respostas yes, no, no, no, yes, no pulam os treinadores.
- Victory Road: pedras nos botões (Strength).
- Elite Four: 5 lutas seguidas sem Pokémon Center nem PC.
- Mew: guia separado do site.

Pós-jogo (ordem): Hall of Fame → créditos → Continue → recomeça em Pallet Town → Unknown Dungeon (Route 24, Surf) → Mewtwo.

## Dúvidas

- Misty: o detonado dá Starmie lv 20; conferir (outras fontes dão 21).
- Bruno: o detonado dá Machamp lv 56; conferir (outras fontes dão 58).
- Giovanni (Viridian Gym): o detonado dá Nidoqueen lv 52; conferir.
- Bulbasaur de Cerulean: o detonado fala em Pikachu feliz e também "level acima de 20"; não fica claro se o nível é exigência.
- Times dos rivais 1–7, de Jessie & James e de Giovanni no Hideout e na Silph não são listados (só limites "até lv 20/40").
- Níveis dos lendários, dos Snorlax e dos fósseis revividos não constam; números das TMs do terraço e das rotas não citadas vêm do mapa padrão, conferir.
- O detonado não marca exclusivos nem lista Pokémon que só o Yellow tem/deixa de ter (ex.: Ekans, Meowth); conferir numa segunda fonte.
