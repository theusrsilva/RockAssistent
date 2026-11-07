# Red/Blue — fatos extraídos do detonado da Pokémon Mythology

Fonte: https://www.pokemythology.net/detonados/detonado-red-blue/ (PDF salvo em 05/11/2026, 17 páginas; o conteúdo vai
até a página 14, o resto é rodapé do site). Só **fatos de jogo** (chefes, níveis, encontros, itens) no nosso formato.
Nenhuma prosa foi copiada. Golpes dos chefes **não** constam no detonado: deixar `moves: []` e `verified: false` para
golpes; espécies e níveis são `verified: true` (uma fonte) até o analista conferir numa segunda. O detonado chama o
rival de "Gary" (nome do anime; no jogo em inglês o padrão é "Blue"); os ids usam `rival-N`.

## Ordem da história e chefes (para `bosses/rb.json`)

| Order | Id | Kind | Nome | Local | Time (espécie lv) | Cap |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | rival-1 | rival | Gary (Blue) | Laboratório do Prof. Oak, Pallet Town | inicial com vantagem de tipo (Bulbasaur→Charmander; Charmander→Squirtle; Squirtle→Bulbasaur), nível não listado | — |
| 2 | rival-2 | rival | Gary (Blue) | Route 22 (oeste de Viridian) | Pidgey 9, inicial do rival 12 | 12 |
| 3 | brock-1 | gym | Brock (Boulder Badge) | Pewter Gym | Geodude 12, Onix 14 | 14 |
| 4 | super-nerd-mtmoon | other | Super Nerd (fósseis) | Mt. Moon, final da caverna | time não listado | — |
| 5 | rival-3 | rival | Gary (Blue) | Nugget Bridge (norte de Cerulean) | Pidgeotto 18, Abra 15, Rattata 15, inicial do rival 17 | 18 |
| 6 | misty-1 | gym | Misty (Cascade Badge) | Cerulean Gym | Staryu 18, Starmie 21 | 21 |
| 7 | rocket-cerulean | evil | Rocket (casa assaltada) | Cerulean City, atrás da casa com a policial | time não listado | — |
| 8 | rival-4 | rival | Gary (Blue) | S.S. Anne, corredor antes da cabine do capitão | Pidgeotto 19, Raticate 16, Kadabra 18, inicial do rival evoluído 20 | 20 |
| 9 | lt-surge-1 | gym | Lt. Surge (Thunder Badge) | Vermilion Gym (2 interruptores nas lixeiras) | Voltorb 21, Pikachu 18, Raichu 24 | 24 |
| 10 | erika-1 | gym | Erika (Rainbow Badge) | Celadon Gym (precisa de Cut) | Victreebel 29, Tangela 24, Vileplume 29 | 29 |
| 11 | giovanni-1 | evil | Giovanni (Team Rocket) | Rocket Hideout B4F (Game Corner) | Onix 25, Rhyhorn 24, Kangaskhan 29 | 29 |
| 12 | rival-5 | rival | Gary (Blue) | Pokémon Tower, Lavender Town | Pidgeotto 25, Kadabra 20, mais 3 por inicial (Bulbasaur: Exeggcute 23, Gyarados 22, Charmeleon 25; Charmander: Growlithe 23, Exeggcute 22, Wartortle 25; Squirtle: Gyarados 23, Growlithe 22, Ivysaur 25) | 25 |
| 13 | ghost-marowak | other | Fantasma (Marowak, não capturável) | Pokémon Tower, andar antes do topo (precisa do Silph Scope) | Marowak, nível não listado | — |
| 14 | rival-6 | rival | Gary (Blue) | Silph Co., sala alcançada pelo 1º teletransporte do 3F | Pidgeot 37, Alakazam 35, mais 3 por inicial (Bulbasaur: Exeggcute 38, Gyarados 35, Charizard 40; Charmander: Growlithe 38, Exeggcute 35, Blastoise 40; Squirtle: Gyarados 38, Growlithe 35, Venusaur 40) | 40 |
| 15 | giovanni-2 | evil | Giovanni (Team Rocket) | Silph Co., sala do presidente | Nidorino 37, Kangaskhan 35, Rhyhorn 37, Nidoqueen 41 | 41 |
| 16 | karate-master | other | Karate Master (Fighting Dojo) | Saffron City, prédio à esquerda do ginásio | time não listado (prêmio: Hitmonlee ou Hitmonchan) | — |
| 17 | sabrina-1 | gym | Sabrina (Marsh Badge) | Saffron Gym (teletransportes) | Kadabra 38, Mr. Mime 37, Venomoth 38, Alakazam 43 | 43 |
| 18 | koga-1 | gym | Koga (Soul Badge) | Fuchsia Gym | Koffing 37, Muk 39, Koffing 37, Weezing 43 | 43 |
| 19 | blaine-1 | gym | Blaine (Volcano Badge) | Cinnabar Gym (precisa da Secret Key) | Growlithe 42, Rapidash 42, Ponyta 40, Arcanine 47 | 47 |
| 20 | giovanni-3 | gym | Giovanni (Earth Badge) | Viridian Gym | Rhyhorn 45, Dugtrio 42, Nidoqueen 44, Nidoking 45, Rhydon 50 | 50 |
| 21 | rival-7 | rival | Gary (Blue) | Route 22/23, antes da Victory Road | Pidgeot 47, Alakazam 50, Rhyhorn 45, mais 3 por inicial (Bulbasaur: Exeggcute 45, Gyarados 47, Charizard 53; Charmander: Growlithe 45, Exeggcute 47, Blastoise 53; Squirtle: Gyarados 45, Growlithe 47, Venusaur 53) | 53 |
| 22 | lorelei | elite | Lorelei (gelo) | Indigo Plateau | Dewgong 54, Cloyster 53, Slowbro 54, Jynx 56, Lapras 56 | 56 |
| 23 | bruno | elite | Bruno (lutador/pedra) | Indigo Plateau | Onix 53, Hitmonchan 55, Hitmonlee 55, Onix 56, Machamp 58 | 58 |
| 24 | agatha | elite | Agatha (fantasma/veneno) | Indigo Plateau | Gengar 56, Golbat 56, Haunter 55, Arbok 58, Gengar 60 | 60 |
| 25 | lance | elite | Lance (dragão) | Indigo Plateau | Gyarados 58, Dragonair 56, Dragonair 56, Aerodactyl 60, Dragonite 62 | 62 |
| 26 | rival-champion | champion | Gary (Blue), campeão | Indigo Plateau | Pidgeot 61, Alakazam 59, Rhydon 61, mais 3 por inicial (Bulbasaur: Exeggutor 61, Gyarados 63, Charizard 65; Charmander: Arcanine 61, Exeggutor 63, Blastoise 65; Squirtle: Gyarados 61, Arcanine 63, Venusaur 65) | 65 |

Ordem 16–18 (Saffron e Fuchsia) é a do detonado; o jogo deixa escolher (Fuchsia pela Cycling Road ou pela Route 12).
Nível recomendado para o campeão segundo o detonado: 65.

## Áreas (para `areas/rb.json`, kind e ordem de liberação)

| Id | Nome | Kind | Libera |
| --- | --- | --- | --- |
| pallet-town | Pallet Town | city | início |
| route-1 | Route 1 | route | início |
| viridian-city | Viridian City | city | início (ginásio só no fim) |
| route-22 | Route 22 | route | após o Pokédex |
| route-2 | Route 2 | route | após o Pokédex |
| viridian-forest | Viridian Forest | dungeon | após o Pokédex |
| pewter-city | Pewter City | city | após Viridian Forest |
| pewter-museum | Pewter Museum of Science | building | após Brock; anexo do Old Amber só com Cut |
| route-3 | Route 3 | route | após Brock |
| mt-moon | Mt. Moon | dungeon | após Brock |
| route-4 | Route 4 | route | após Mt. Moon |
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
| rock-tunnel | Rock Tunnel | dungeon | com Cut (Flash recomendado) |
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
| saffron-city | Saffron City | city | dar uma bebida ao guarda (Fresh Water) |
| silph-co | Silph Co. | building | após a Poké Flute / entrada em Saffron |
| fighting-dojo | Fighting Dojo | building | após entrar em Saffron |
| cycling-road | Cycling Road (Route 17) | route | com Bicycle e Snorlax da Route 16 acordado |
| route-18 | Route 18 | route | após a Cycling Road |
| route-12 | Route 12 | route | Snorlax com a Poké Flute |
| route-13 | Route 13 | route | após a Route 12 |
| route-14 | Route 14 | route | após a Route 12 |
| route-15 | Route 15 | route | após a Route 12 |
| fuchsia-city | Fuchsia City | city | pela Cycling Road ou pela Route 12 |
| safari-zone | Safari Zone | zone | após Fuchsia |
| power-plant | Power Plant | dungeon | com Surf (rio perto do Rock Tunnel) |
| route-19 | Route 19 | sea | com Surf |
| route-20 | Route 20 | sea | com Surf |
| seafoam-islands | Seafoam Islands | dungeon | com Surf e Strength |
| cinnabar-island | Cinnabar Island | city | com Surf |
| pokemon-mansion | Pokémon Mansion | building | após chegar em Cinnabar |
| route-21 | Route 21 | sea | com Surf (Cinnabar → Pallet) |
| route-23 | Route 23 | route | com as 8 insígnias |
| victory-road | Victory Road | dungeon | com as 8 insígnias, Surf e Strength |
| indigo-plateau | Indigo Plateau | building | após Victory Road |
| unknown-dungeon | Unknown Dungeon (Cerulean Cave) | dungeon | pós-jogo (após os créditos), com Surf pela Route 24 |

## Encontros (para `encounters/rb.json`; method `walk` salvo indicação)

- Route 1 / Route 2 / Route 22 (o detonado agrupa como "rotas" perto de Viridian): Pidgey, Rattata, Spearow, Nidoran (♂/♀ não separado).
- Viridian Forest: Caterpie, Metapod, Weedle, Kakuna; Pikachu (raro).
- Route 3: Jigglypuff.
- Mt. Moon (`cave`): Zubat, Paras, Geodude, Clefairy. Moon Stone no chão.
- Rotas ao norte de Cerulean (Nugget Bridge / Route 25): Oddish, Bellsprout.
- Rota depois da casa assaltada (Route 5): Mankey, Meowth.
- Rota depois do Underground Path (Route 6): Ekans, Sandshrew.
- Diglett's Cave (`cave`): Diglett (o detonado cita como opção para Lt. Surge).
- Route 10: Voltorb (fim da rota).
- Rock Tunnel (`cave`): Graveler, Onix, Machop "e outros".
- Rotas a oeste de Lavender (Route 8): Vulpix, Growlithe.
- Pokémon Tower: Gastly, Haunter, Cubone.
- Cycling Road: Raticate, Doduo; Fearow (raro).
- Routes 12–15: Venonat; Ditto (raro).
- Safari Zone: Exeggcute, família Nidoran, Venonat, Paras, Doduo, Rhyhorn; raros: Tauros, Chansey, Scyther, Pinsir, Kangaskhan, Venomoth, Parasect. `fish` com Super Rod: Dratini; senão Psyduck, Slowpoke, Krabby.
- Power Plant: Pikachu, Magnemite, Magneton; Voltorb e Electrode disfarçados de item (`fixed`); Electabuzz [Red], Raichu [Blue].
- Seafoam Islands (`cave`/`surf`): Seel, Psyduck, Golduck, Horsea, Krabby, Zubat, Golbat, Shellder, Staryu, Kingler, Dewgong, Seadra, Slowbro.
- Victory Road (`cave`): Machop, Machoke, Geodude, Graveler, Zubat, Golbat, Onix, Marowak, Venomoth.
- Unknown Dungeon (`cave`): Dodrio, Venomoth, Kadabra, Rhydon, Marowak, Electrode, Chansey, Wigglytuff, Ditto, Parasect, Raichu, Sandslash/Arbok, Golbat, Hypno, Magneton.
- Snorlax (`fixed`, 2 no jogo): Route 16 (oeste de Celadon) e Route 12 (sul de Lavender); acordar com a Poké Flute. Nível não listado.
- Lendários (`method: fixed`, nível não listado no detonado): Zapdos (fim da Power Plant); Articuno (Seafoam Islands, andar de baixo, após o puzzle das 4 pedras); Moltres (Victory Road, escada do último mapa); Mewtwo (Unknown Dungeon, andar mais baixo, canto inferior esquerdo, pós-jogo; o detonado recomenda a Master Ball).
- Mew: o detonado remete a um guia separado ("como capturar o Mew") e avisa para não lutar com o treinador virado para o norte na Route 25 e com o último treinador da Route 8.

## Presentes, trocas in-game e fósseis (`method: gift` / `trade`)

- Inicial: Bulbasaur, Charmander ou Squirtle (Laboratório do Oak, Pallet Town; nível não listado).
- Eevee lv 25 (Celadon Mansion, entrada dos fundos, último andar; precisa de vaga na equipe).
- Lapras (Silph Co., funcionário perto da sala do rival; nível não listado).
- Hitmonlee ou Hitmonchan (Fighting Dojo, Saffron, após vencer o mestre; escolha de um).
- Fósseis: Helix Fossil → Omanyte ou Dome Fossil → Kabuto (escolha de um, Mt. Moon, após o Super Nerd); Old Amber → Aerodactyl (anexo do Pewter Museum, acesso com Cut). Revividos no Cinnabar Lab (nível não listado).
- Trocas in-game: o detonado não cita nenhuma.

## TMs/HMs e itens-chave (localização)

| Item | Local |
| --- | --- |
| Potion | PC do quarto, Pallet Town |
| Oak's Parcel | Poké Mart de Viridian (entregar ao Oak) |
| Pokédex + Poké Balls | Prof. Oak, após entregar o Oak's Parcel |
| Town Map | Irmã do rival, Pallet Town |
| TM34 Bide | Brock |
| Moon Stone | Mt. Moon |
| S.S. Ticket | Bill, Route 25 |
| TM28 Dig | Rocket da casa assaltada, Cerulean |
| HM01 Cut | Capitão do S.S. Anne |
| Old Rod | Fishing Guru, Vermilion |
| Bike Voucher → Bicycle | Presidente do Pokémon Fan Club, Vermilion → Bike Shop de Cerulean |
| TM Thunderbolt (número não citado) | Lt. Surge |
| HM05 Flash | Assistente do Oak, Route 2 (saída sul do Diglett's Cave) |
| Old Amber | Anexo do Pewter Museum (Cut) |
| HM02 Fly | Casa na Route 16 (Cut) |
| TMs (números não citados) | Menina do terraço do Celadon Dept. Store, em troca de Fresh Water, Soda Pop e Lemonade |
| Fresh Water / Soda Pop / Lemonade | Máquinas do terraço do Celadon Dept. Store |
| Lift Key | Rocket do Rocket Hideout B4F |
| Silph Scope | Giovanni, Rocket Hideout |
| Poké Flute | Mr. Fuji, Lavender Town |
| Card Key | Silph Co. 5F |
| Master Ball | Presidente da Silph Co. |
| Super Rod | Casa na Route 12 (depois do Snorlax) |
| Good Rod | Parente do Fishing Guru, Fuchsia City |
| HM03 Surf + Gold Teeth | Casa secreta da Safari Zone |
| HM04 Strength | Safari Zone Warden, Fuchsia (entregar o Gold Teeth) |
| Rare Candy | Atrás da pedra do Warden (Strength) |
| TM22 Solar Beam | Pokémon Mansion |
| Secret Key | Pokémon Mansion (abre o Cinnabar Gym) |
| TM27 Fissure | Giovanni, Viridian Gym |

Lojas: Celadon Dept. Store (5 andares, máquinas de bebida no terraço); Poké Mart do Indigo Plateau.

## Mecânicas (para o guia de mecânicas, texto nosso)

- Exclusivos de versão citados: Electabuzz [Red] e Raichu [Blue] na Power Plant.
- Rival escolhe o inicial com vantagem de tipo; o time dele muda conforme o inicial do jogador.
- Rocket Hideout e Viridian Gym: pisos de setas que empurram o jogador; Saffron Gym e Silph Co.: teletransportes.
- Silph Co.: Card Key abre os portões; cura no 9F.
- Pokémon Tower: Silph Scope revela os fantasmas; piso de cura no meio de um andar.
- Cycling Road: só com Bicycle; descida automática para o sul.
- Safari Zone: área de captura especial ao norte de Fuchsia.
- Seafoam Islands: correnteza bloqueia o Surf até as pedras serem empurradas nos buracos com Strength.
- Cinnabar Gym: quiz; respostas na ordem yes, no, no, no, yes, no pulam os treinadores.
- Victory Road: pedras nos botões redondos abrem passagem (Strength).
- Elite Four: 5 lutas seguidas, sem Pokémon Center nem PC entre elas.
- Mew: guia separado do site (truque com treinadores das Routes 25 e 8).

Pós-jogo (ordem): créditos → Continue → recomeça em Pallet Town → Unknown Dungeon (Route 24, Surf) → Mewtwo.

## Dúvidas

- Rival na Route 22: o detonado diz inicial lv 12 junto com Pidgey 9; conferir (outras fontes dão nível menor).
- Rival no laboratório: time e nível não listados.
- Blaine: o detonado lista Growlithe 42, Rapidash 42, Ponyta 40, Arcanine 47 nessa ordem; conferir a ordem do jogo.
- Níveis dos lendários (Zapdos, Articuno, Moltres, Mewtwo), dos Snorlax, do Lapras e dos fósseis revividos não constam.
- Números das TMs do Lt. Surge e da menina do terraço não constam; números de rota que o detonado não cita (Routes 5, 6, 8, 11, 12–23) vêm do mapa padrão, conferir.
- Vulpix/Growlithe, Ekans/Sandshrew e Mankey/Meowth não estão marcados como exclusivos de versão no detonado; conferir.
- Times do Super Nerd, do Rocket de Cerulean e do Karate Master não constam.
