# Ultra Sun / Ultra Moon — fatos extraídos do detonado da Pokémon Mythology

Fonte: https://pokemythology.net/conteudo/detonados/detonado_usum.htm (PDF salvo em 05/11/2026, 136 páginas; o texto
extraído tem os acentos corrompidos e termina na página 136 de 137).
Só **fatos de jogo** (chefes, níveis, encontros, itens) no nosso formato. Nenhuma prosa foi copiada. Golpes dos
chefes **não** constam no detonado: deixar `moves: []` e `verified: false` para golpes; espécies e níveis são
`verified: true` (uma fonte) até o analista conferir numa segunda. Exclusivos: `[US]` = Ultra Sun, `[UM]` = Ultra Moon.
"inicial" = Rowlet/Litten/Popplio; Hau escolhe o que tem fraqueza ao seu. Trials (Totem) e Grand Trials (kahunas)
marcados como `gym` por serem o equivalente dos ginásios. Números de rota que o detonado não cita vão marcados
"(inferida)".

## Ordem da história e chefes (para `bosses/usum.json`)

| Order | Id | Kind | Nome | Local | Time (espécie lv) | Cap |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | hau-1 | rival | Hau | Iki Town | inicial (fraco ao seu) 5 | 5 |
| 2 | hau-2 | rival | Hau (festival) | Iki Town | Pichu 6, inicial 7 | 7 |
| 3 | ilima-1 | other | Ilima (captain) | Hau'oli City | Yungoos 10, Smeargle 11 | 11 |
| 4 | totem-gumshoos | gym | Totem Gumshoos [US] / Totem Alolan Raticate [UM] (Normalium Z) | Verdant Cavern | Totem 12 + aliado Yungoos [US] / Alolan Rattata [UM] 11 | 12 |
| 5 | urs-1 | other | Dulse [US] / Soliera [UM] (Ultra Recon Squad) | Seaward Cave | Furfrou 13 | 13 |
| 6 | hau-3 | rival | Hau | Saída da Melemele Meadow | inicial 12, Pikachu 12, Noibat 11 | 12 |
| 7 | hala-1 | gym | Hala (Grand Trial Melemele, Fightinium Z) | Iki Town | Machop 15, Makuhita 15, Crabrawler 16 | 16 |
| 8 | dexio-sina-1 | other | Dexio [US] / Sina [UM] | Fonte de Heahea City | [US] Mime Jr. 15, Espeon 15 / [UM] Smoochum 15, Glaceon 15 | 15 |
| 9 | hau-4 | rival | Hau | Paniola Town | inicial evoluído 16, Pikachu 15, Noibat 14, Eevee 14 | 16 |
| 10 | gladion-1 | rival | Gladion | Paniola Ranch | Zorua 17, Zubat 17, Type: Null 18 | 18 |
| 11 | totem-araquanid | gym | Totem Araquanid (Waterium Z) | Brooklet Hill | Araquanid 20, Dewpider 18, Masquerain 18 | 20 |
| 12 | urs-2 | other | Dulse [US] / Soliera [UM] | Saída dos Sudowoodo, sul do Paniola Ranch | Poipole 20 | 20 |
| 13 | battle-royal-1 | other | Battle Royal (Royal, Hau, Gladion) | Battle Royal Dome, Royal Avenue | The Royal: Rockruff 19; Hau: inicial evoluído 19; Gladion: Type: Null 19 | 19 |
| 14 | totem-marowak | gym | Totem Alolan Marowak (Firium Z) | Wela Volcano Park | Marowak 22, Salazzle 20 | 22 |
| 15 | totem-lurantis | gym | Totem Lurantis (Grassium Z) | Lush Jungle | Lurantis 24, Comfey 22, Kecleon 22 | 24 |
| 16 | plumeria-1 | evil | Plumeria (Admin Team Skull) | Memorial Hill | Golbat 26, Salandit 27 | 27 |
| 17 | olivia-1 | gym | Olivia (Grand Trial Akala, Rockium Z) | Ruins of Life | Anorith 27, Lileep 27, Lycanroc 28 | 28 |
| 18 | ub-aether-1 | other | Ultra Beast (sem nome nem nível no detonado; só derrotar) | Aether Paradise | — | — |
| 19 | hau-5 | rival | Hau (dá Lycanium Z) | Malie City | Rowlet: Brionne 30, Alolan Raichu 28, Flareon 28, Noibat 28, Tauros 28; Litten: Dartrix 30 + Vaporeon 28 (resto igual); Popplio: Torracat 30 + Leafeon 28 (resto igual) | 30 |
| 20 | totem-togedemaru | gym | Totem Togedemaru (Electrium Z) | Hokulani Observatory | Togedemaru 33, Skarmory 32, Dedenne 31 | 33 |
| 21 | guzma-1 | evil | Guzma (chefe Team Skull) | Malie Garden | Golisopod 34, Masquerain 34 | 34 |
| 22 | totem-mimikyu | gym | Totem Mimikyu (Ghostium Z) | Abandoned Thrifty Megamart | Mimikyu 33, Banette 32, Jellicent 33 | 33 |
| 23 | plumeria-2 | evil | Plumeria | Aether House | Golbat 37, Salazzle 38 | 38 |
| 24 | guzma-2 | evil | Guzma | Shady House, Po Town | Golisopod 41, Masquerain 41, Pinsir 41 | 41 |
| 25 | gladion-2 | rival | Gladion | Aether House | Golbat 42, Zoroark 42, Type: Null 43 | 43 |
| 26 | nanu-1 | gym | Nanu (Grand Trial Ula'ula, Darkinium Z) | Ferry Terminal de Malie | Sableye 43, Krokorok 43, Alolan Persian 44 | 44 |
| 27 | faba-1 | evil | Faba (Aether) | Aether Paradise | Hypno 43 | 43 |
| 28 | guzma-3 | evil | Guzma | Aether Paradise | Golisopod 45, Vikavolt 45, Masquerain 45, Pinsir 45 | 45 |
| 29 | urs-3 | other | Dulse [US] / Soliera [UM] | Aether Paradise | Poipole 47 | 47 |
| 30 | lusamine-1 | evil | Lusamine (presidente da Aether) | Aether Paradise | Clefable 47, Lilligant 47, Lopunny 47, Milotic 47, Bewear 47 | 47 |
| 31 | urs-4 | other | Dulse [US] / Soliera [UM] | Vast Poni Canyon | Poipole 49 | 49 |
| 32 | totem-kommoo | gym | Totem Kommo-o (Dragonium Z) | Vast Poni Canyon | Kommo-o 47, Scizor 46, Noivern 48 | 48 |
| 33 | necrozma-1 | other | Necrozma (não capturável) | Altar of the Sunne [US] / Moone [UM] | Necrozma 50 | — |
| 34 | ultra-necrozma | other | Ultra Necrozma (não capturável) | Megalo Tower, Ultra Megalopolis | Ultra Necrozma 60 | — |
| 35 | mina-1 | other | Mina (captain) | Casa da Mina, Seafolk Village | Mawile 51, Granbull 51, Ribombee 51 | 51 |
| 36 | ilima-2 | other | Ilima (pétala laranja) | Hau'oli Cemetery | Gumshoos 51, Smeargle 51, Komala 51 | 51 |
| 37 | mallow-lana-2 | other | Mallow [US] / Lana [UM] (pétala verde/azul) | Lush Jungle | [US] Trevenant 51, Shiinotic 51, Tsareena 51 / [UM] Lanturn 51, Cloyster 51, Araquanid 51 | 51 |
| 38 | kiawe-2 | other | Kiawe (pétala vermelha; depois o hiker com Magmar, sem nível) | Wela Volcano Park | Arcanine 51, Talonflame 51, Alolan Marowak 51 | 51 |
| 39 | sophocles-2 | other | Sophocles [US] (pétala amarela; em [UM] sem batalha) | Hokulani Observatory | Togedemaru 51, Magnezone 51, Golem 51 | 51 |
| 40 | nanu-2 | other | Nanu [UM] (pétala roxa; em [US] sem batalha) | Aether House | Sableye 52, Absol 52, Alolan Persian 53 | 53 |
| 41 | totem-ribombee | gym | Totem Ribombee (Fairium Z) | Seafolk Village | Ribombee 55, Pelipper 52, Blissey 53 | 55 |
| 42 | hapu-1 | gym | Hapu (Grand Trial Poni, Groundium Z) | Exeggutor Island | Golurk 53, Gastrodon 53, Flygon 53, Mudsdale 54 | 54 |
| 43 | gladion-3 | rival | Gladion | Mount Lanakila | Crobat 53, Zoroark 53, Lucario 53, Silvally 55 (tipo forte contra o seu inicial: fogo/água/planta) | 55 |
| 44 | necrozma-capture | other | Necrozma (captura) | Mount Lanakila | Necrozma 65 | — |
| 45 | box-legend | other | Solgaleo [US] / Lunala [UM] (Nebby, captura) | Plank Bridge, Mahalo Trail | 60 | — |
| 46 | molayne-1 | elite | Molayne | Pokémon League (sala 1) | Klefki 56, Bisharp 56, Magnezone 56, Metagross 56, Alolan Dugtrio 57 | 57 |
| 47 | olivia-e4 | elite | Olivia | Pokémon League (sala 2) | Armaldo 56, Cradily 56, Gigalith 56, Probopass 56, Lycanroc 57 | 57 |
| 48 | acerola-1 | elite | Acerola | Pokémon League (sala 3) | Banette 56, Drifblim 56, Dhelmise 56, Froslass 56, Palossand 57 | 57 |
| 49 | kahili-1 | elite | Kahili | Pokémon League (sala 4) | Braviary 56, Hawlucha 56, Oricorio 56, Mandibuzz 56, Toucannon 57 | 57 |
| 50 | hau-champion | champion | Hau (desafiante pelo título) | Pokémon League | Alolan Raichu 56, Noivern 56, Tauros 56, Crabominable 56 + por inicial: Rowlet → Flareon 56, Primarina 57; Litten → Vaporeon 56, Decidueye 57; Popplio → Leafeon 56, Incineroar 57 | 57 |
| 51 | ub-poni-grove | other | Blacephalon [US] / Stakataka [UM] ×2 (5 Beast Balls; reaparecem até capturar) | Poni Grove | nível não listado | — |
| 52 | festival-plaza-rocket | other | Team Rocket no Festival Plaza (só com Pokémon emprestado da Battle Agency) | Festival Plaza | — | — |
| 53 | hala-2 | other | Hala (revanche; dá os Z dos outros iniciais) | Iki Town | Hariyama 63, Machamp 63, Bewear 63, Poliwrath 63, Crabominable 63 | 63 |
| 54 | ilima-3 | other | Ilima (dá Everstone) | Trainers' School | Gumshoos 60, Smeargle 60, Komala 60 | 60 |
| 55 | totem-gumshoos-2 | other | Totem Gumshoos [US] / Alolan Raticate [UM] (aliados infinitos) | Verdant Cavern | Totem 60 + Yungoos/Alolan Rattata 58 | 60 |
| 56 | lana-3 | other | Lana (após as irmãs; Deep Sea Tooth/Scale) | Casa ao norte de Konikoni City | Lanturn 60, Cloyster 60, Araquanid 60 | 60 |
| 57 | mallow-3 | other | Mallow [UM] (de dia) | Lush Jungle | Trevenant 60, Shiinotic 60, Tsareena 60 | 60 |
| 58 | kiawe-3 | other | Kiawe [US] (à noite) | Thrifty Megamart, Royal Avenue | Arcanine 60, Talonflame 60, Alolan Marowak 60 | 60 |
| 59 | morimoto-iwao | other | Morimoto + Iwao (dupla, diária; Oval Charm) | Prédio da Game Freak, Heahea City | Morimoto: Gengar 57, Machamp 57, Dragonite 57; Iwao: Kecleon 56, Ambipom 56, Blissey 56 (ver Dúvidas) | 57 |
| 60 | eevee-trainers | other | 8 treinadores das Eeveelutions + tio do Megamart (Eevium Z) | Vários (ver Mecânicas) | uma Eeveelution 55 cada; Eevee do tio sem nível | 55 |
| 61 | tapu-koko | other | Tapu Koko (dá Tapunium Z) | Ruins of Conflict | 60 | — |
| 62 | tapu-lele | other | Tapu Lele | Ruins of Life | 60 | — |
| 63 | tapu-bulu | other | Tapu Bulu | Ruins of Abundance | 60 | — |
| 64 | tapu-fini | other | Tapu Fini | Ruins of Hope | 60 | — |
| 65 | faba-rr | evil | Faba + membro da Aether (dupla, Lillie com Clefairy ao lado) | Aether Paradise (Episode RR) | time não listado | — |
| 66 | archie-rr | evil | Archie (Team Rainbow Rocket) | Castelo Rocket, Aether Paradise | Mightyena 64, Muk 64, Crobat 64, Sharpedo 64, Kyogre 66 | 66 |
| 67 | maxie-rr | evil | Maxie | Castelo Rocket | Mightyena 64, Weezing 64, Crobat 64, Camerupt 64, Groudon 66 | 66 |
| 68 | cyrus-rr | evil | Cyrus (dá Galactic Key) | Castelo Rocket | Houndoom 65, Honchkrow 65, Weavile 65, Crobat 65, Dialga [US] / Palkia [UM] 67 | 67 |
| 69 | lysandre-rr | evil | Lysandre | Castelo Rocket | Mienshao 65, Honchkrow 65, Mega Gyarados 65, Pyroar 65, Xerneas [US] / Yveltal [UM] 67 | 67 |
| 70 | ghetsis-rr | evil | Ghetsis | Castelo Rocket | Cofagrigus 66, Bouffalant 66, Bisharp 66, Hydreigon 66, Zekrom [US] / Reshiram [UM] 68 | 68 |
| 71 | giovanni-rr | evil | Giovanni (líder da Team Rainbow Rocket) | Castelo Rocket | Dugtrio 68, Nidoqueen 68, Nidoking 68, Rhyperior 68, Mega Mewtwo X [US] / Mega Mewtwo Y [UM] 70 | 70 |
| 72 | faba-2 | other | Faba (revanche; Dubious Disc) | Aether Paradise 1F | Claydol 63, Bruxish 63, Alolan Raichu 63, Alakazam 63, Hypno 63 | 63 |
| 73 | dexio-2 | other | Dexio (dá Key Stone + Alakazite) | Poni Meadow | Espeon 60, Alolan Raichu 60, Slowking 60, Metagross 60, Mega Alakazam 60 | 60 |
| 74 | zygarde-50 | other | Zygarde 50% (captura) | Resolution Cave | 60 | — |
| 75 | dexio-3 | other | Dexio (dá Zygarde 10% + Zygarde Cube) | Trailer, Route 16 | Espeon 63, Alolan Raichu 63, Slowking 63, Metagross 63, Alakazam 63, Zygarde 10% 63 | 63 |
| 76 | aristo-1 | other | Aristo (após vencer todos os treinadores-líderes de Alola) | Mount Lanakila | Carbink 65, Torkoal 65, Pelipper 65, Alolan Ninetales 65, Gigalith 65 | 65 |
| 77 | red-1 | other | Red (escolha Red ou Blue) | Entrada da Battle Tree | Pikachu 74, Lapras 69, Snorlax 69, Venusaur 70, Charizard 70, Blastoise 70 | 74 |
| 78 | blue-1 | other | Blue | Entrada da Battle Tree | Alakazam 69, Machamp 69, Aerodactyl 69, Exeggutor 69, Arcanine 69, Gyarados 69 | 69 |
| 79 | molayne-2 | elite | Molayne (revanche) | Pokémon League | Klefki 66, Bisharp 66, Magnezone 66, Metagross 66, Alolan Dugtrio 66 | 66 |
| 80 | olivia-e4-2 | elite | Olivia (revanche) | Pokémon League | Armaldo 66, Cradily 66, Gigalith 66, Probopass 66, Lycanroc 66 | 66 |
| 81 | acerola-2 | elite | Acerola (revanche) | Pokémon League | Banette 66, Drifblim 66, Dhelmise 66, Froslass 66, Palossand 66 | 66 |
| 82 | kahili-2 | elite | Kahili (revanche) | Pokémon League | Braviary 66, Hawlucha 66, Oricorio 66, Mandibuzz 66, Toucannon 66 | 66 |
| 83 | defense-hau | champion | Hau (defesa do título) | Pokémon League | Alolan Raichu 69, Noivern 68, Tauros 68, Crabominable 69 + Rowlet → Flareon 68, Primarina 70; Litten → Vaporeon 68, Decidueye 70; Popplio → Leafeon 68, Incineroar 70 | 70 |
| 84 | defense-gladion | champion | Gladion (defesa; só 1 mês após Necrozma) | Pokémon League | Golbat 68, Zoroark 68, Porygon-Z 68, Lucario 68 + Rowlet → Venusaur 68, Silvally (fogo) 70; Litten → Charizard 68, Silvally (água) 70; Popplio → Blastoise 68, Silvally (planta) 70 | 70 |
| 85 | defense-lusamine | champion | Lusamine (defesa; só no dia 1 do mês) | Pokémon League | Clefable 68, Lilligant 68, Lopunny 68, Milotic 68, Bewear 68 | 68 |
| 86 | defense-guzma | champion | Guzma (defesa) | Pokémon League | Golisopod 68, Vikavolt 68, Masquerain 68, Pinsir 68, Scizor 68 | 68 |
| 87 | defense-plumeria | champion | Plumeria (defesa) | Pokémon League | Gengar 67, Alolan Muk 67, Crobat 67, Toxapex 67, Salazzle 67 | 67 |
| 88 | defense-sophocles | champion | Sophocles (defesa) | Pokémon League | Togedemaru 67, Magnezone 67, Electivire 67, Vikavolt 67, Alolan Golem 67 | 67 |
| 89 | defense-hapu | champion | Hapu (defesa) | Pokémon League | Golurk 68, Gastrodon 68, Krookodile 68, Flygon 68, Mudsdale 69 | 69 |
| 90 | defense-ryuki | champion | Ryuki (defesa) | Pokémon League | Garchomp 67, Drampa 67, Turtonator 67, Dragonite 67, Kommo-o 67 | 67 |
| 91 | defense-tristan | champion | Tristan (defesa) | Pokémon League | Emolga 66, Sharpedo 66, Alolan Exeggutor 66, Magmortar 66, Tauros 66 | 66 |
| 92 | defense-kukui | champion | Professor Kukui (defesa) | Pokémon League | Lycanroc 69, Alolan Ninetales 68, Braviary 68, Magnezone 68, Snorlax 68 + inicial (Decidueye/Incineroar/Primarina) nível incerto (ver Dúvidas) | 69 |

Sem time listado: Spearow de Mahalo Trail (1ª luta); duplas com a Team Skull (Hau'oli City, Big Wave Beach, Diglett's
Tunnel); Meredith (Oricorio Pom-Pom na Melemele Meadow, Pa'u na Royal Avenue, Baile na Ula'ula Meadow, Sensu na Poni
Meadow); duplas no Aether Paradise; Ryuki no Kantonian Gym; treinadores-líderes de cada área.

Battle Tree (lv 50): Battle Legend Red (Single/Multi) e Blue (Double/Multi) após 20 vitórias — pool combinado no texto:
Venusaur, Charizard, Blastoise, Lapras, Snorlax, Pidgeot, Alakazam, Machamp, Aerodactyl, Exeggutor, Arcanine, Gyarados,
Tyranitar, Rhyperior (divisão por treinador em Dúvidas). Treinadores especiais (Super, a cada 10 lutas, lv 50):
Wally (Altaria, Garchomp, Magnezone, Gallade); Cynthia (Milotic, Spiritomb, Garchomp, Lucario, Togekiss); Anabel
(Alakazam, Snorlax, Raikou, Entei, Latios, Lucario); Colress (Alolan Muk, Electrode, Porygon2, Metagross, Magnezone,
Porygon-Z, Klinklang); Grimsley (Houndoom, Tyranitar, Sharpedo, Absol, Honchkrow, Drapion, Liepard, Scrafty, Bisharp);
Plumeria [US] (Gengar, Crobat, Toxapex, Lurantis, Salazzle); Guzma [UM] (Pinsir, Honchkrow, Toxicroak, Liepard, Bisharp,
Toxapex, Golisopod); Sina [US] (Alolan Sandslash, Abomasnow, Glaceon, Lilligant, Mandibuzz, Oranguru, Drampa); Dexio
[UM] (Alolan Ninetales, Slowbro, Espeon, Whimsicott, Braviary, Passimian, Turtonator); Kiawe [US] (Arcanine, Alolan
Marowak, Kangaskhan, Talonflame, Salazzle); Mallow [UM] (Sceptile, Talonflame, Trevenant, Toucannon, Lurantis, Tsareena,
Comfey). Parceira padrão do Multi: Rada (Barbaracle, Hawlucha); Lillie (Ribombee, Comfey) pela casa do Kukui.

## Áreas (para `areas/usum.json`, em ordem de progressão)

| id | nome | kind | libera |
| --- | --- | --- | --- |
| player-house | Casa do jogador (Route 1) | building | início |
| iki-town | Iki Town | city | início |
| mahalo-trail | Mahalo Trail | route | início |
| route-1 | Route 1 | route | início |
| kukui-lab | Laboratório do Kukui (Route 1) | building | após o festival |
| route-1-hauoli-outskirts | Route 1 (Hau'oli Outskirts, Pokémon Center) | route | após o laboratório |
| trainers-school | Trainers' School | building | após o laboratório |
| hauoli-city | Hau'oli City | city | após a escola (Tauros retirado) |
| route-2 | Route 2 | route | após Ilima |
| hauoli-cemetery | Hau'oli Cemetery | zone | após Ilima |
| big-wave-beach | Big Wave Beach | zone | após Ilima |
| berry-fields | Berry Fields | zone | Route 2 |
| verdant-cavern | Verdant Cavern | dungeon | Route 2 |
| route-3 | Route 3 | route | após a trial do Ilima |
| melemele-meadow | Melemele Meadow | zone | Route 3 |
| seaward-cave | Seaward Cave | dungeon | Melemele Meadow (resto com Lapras) |
| ten-carat-hill | Ten Carat Hill | dungeon | Tauros Charge (após Hala) |
| alola-photo-club | Alola Photo Club, Hau'oli City | building | após Hala |
| heahea-beach | Heahea Beach | zone | Mantine Surf |
| heahea-city | Heahea City | city | Mantine Surf |
| route-4 | Route 4 | route | Heahea City |
| pikachu-valley | Pikachu Valley | zone | Route 4 |
| paniola-town | Paniola Town | city | Route 4 |
| paniola-ranch | Paniola Ranch (Route 5, Pokémon Nursery) | route | Paniola Town |
| brooklet-hill | Brooklet Hill | zone | Paniola Ranch |
| route-6 | Route 6 (sul dos Sudowoodo) (inferida) | route | após Lana |
| royal-avenue | Royal Avenue | city | após Lana |
| battle-royal-dome | Battle Royal Dome | building | Royal Avenue |
| route-7 | Route 7 (inferida) | route | Royal Avenue |
| wela-volcano-park | Wela Volcano Park | zone | Route 7 |
| dividing-peak-tunnel | Dividing Peak Tunnel | dungeon | após Kiawe |
| route-8 | Route 8 | route | após Kiawe |
| lush-jungle | Lush Jungle | zone | Route 8 |
| dimensional-research-lab | Dimensional Research Lab, Heahea City | building | após Mallow |
| diglett-tunnel | Diglett's Tunnel | dungeon | após Mallow |
| route-9 | Route 9 (saída do Diglett's Tunnel) (inferida) | route | após Diglett's Tunnel |
| memorial-hill | Memorial Hill | zone | Route 9 |
| akala-outskirts | Akala Outskirts | route | Memorial Hill |
| konikoni-city | Konikoni City | city | Diglett's Tunnel |
| ruins-of-life | Ruins of Life | dungeon | após Plumeria (Tapu Lele no pós-jogo) |
| hano-grand-resort | Hano Grand Resort | building | após Olivia |
| hano-beach | Hano Beach | zone | após Olivia |
| aether-paradise | Aether Paradise | building | após Olivia (barco); Fly após Lusamine |
| malie-city | Malie City | city | Ula'ula Island |
| malie-garden | Malie Garden | zone | Malie City |
| malie-library | Malie Library | building | Malie City |
| malie-outer-cape | Malie City Outer Cape | route | Malie City |
| kantonian-gym | Kantonian Gym | building | Malie City |
| route-10 | Route 10 (ponto de ônibus) (inferida) | route | Malie City |
| mount-hokulani | Mount Hokulani | route | ônibus da Route 10 |
| hokulani-observatory | Hokulani Observatory | building | Mount Hokulani |
| route-11 | Route 11 (sul de Malie, Komala) (inferida) | route | Malie City |
| route-12 | Route 12 (Mudsdale) (inferida) | route | após Sophocles (barricada) |
| blush-mountain | Blush Mountain | zone | Route 12 |
| geothermal-power-plant | Geothermal Power Plant | building | Blush Mountain |
| ulaula-beach | Ula'ula Beach | zone | Route 12 |
| tapu-village | Tapu Village | city | após Ula'ula Beach |
| route-14-15 | Rotas ao sul/oeste de Tapu Village (inferidas: Route 14 e 15) | route | Tapu Village |
| aether-house | Aether House (Route 15) | building | Tapu Village |
| abandoned-thrifty-megamart | Abandoned Thrifty Megamart | building | Tapu Village (sul) |
| haina-desert | Haina Desert | zone | após Acerola |
| ruins-of-abundance | Ruins of Abundance | dungeon | fim do Haina Desert |
| melemele-sea | Melemele Sea | sea | Sharpedo Jet |
| sandy-cave | Sandy Cave (Big Wave Beach) | dungeon | Sharpedo Jet / Machamp Shove |
| route-16 | Route 16 (Pokémon Center, café, trailer) | route | Sharpedo Jet |
| ulaula-meadow | Ula'ula Meadow | zone | Route 16 |
| lake-of-the-sunne-moone | Lake of the Sunne [US] / Moone [UM] | zone | Ula'ula Meadow |
| route-17 | Arredores de Po Town (inferida: Route 17) | route | após Ula'ula Meadow |
| po-town | Po Town | city | após vencer os guardas |
| shady-house | Shady House | building | Po Town |
| seafolk-village | Seafolk Village | city | Poni Island (barco) |
| poni-wilds | Poni Wilds | route | Seafolk Village |
| poni-beach | Poni Beach | zone | Poni Wilds |
| ancient-poni-path | Ancient Poni Path | route | Poni Wilds |
| poni-breaker-coast | Poni Breaker Coast | sea | Ancient Poni Path |
| ruins-of-hope | Ruins of Hope | dungeon | Machamp Shove |
| exeggutor-island | Exeggutor Island | zone | barco da Mina |
| vast-poni-canyon | Vast Poni Canyon | dungeon | após a flauta |
| altar-sunne-moone | Altar of the Sunne [US] / Moone [UM] | building | fim do Vast Poni Canyon |
| ultra-megalopolis | Ultra Megalopolis / Megalo Tower | zone | Ultra Warp Ride |
| mount-lanakila | Mount Lanakila | dungeon | após Hapu |
| plank-bridge | Plank Bridge (Mahalo Trail) | route | após Necrozma |
| ruins-of-conflict | Ruins of Conflict | dungeon | Plank Bridge |
| pokemon-league | Pokémon League | building | topo do Mount Lanakila |
| poni-grove | Poni Grove | zone | pós-jogo |
| festival-plaza | Festival Plaza | building | Battle Agency após Sophocles; invasão no pós-jogo |
| ultra-space | Ultra Space / Ultra Space Wilds | zone | pós-jogo (portal no Altar) |
| reverse-world | Reverse World | zone | pós-jogo (Solgaleo à noite, Lunala de dia) |
| poni-plains | Poni Plains | route | pós-jogo |
| plains-grotto | Plains Grotto | dungeon | pós-jogo |
| poni-meadow | Poni Meadow | zone | pós-jogo |
| resolution-cave | Resolution Cave | dungeon | pós-jogo |
| poni-coast | Poni Coast | route | pós-jogo |
| poni-gauntlet | Poni Gauntlet | route | pós-jogo |
| battle-tree | Battle Tree | building | pós-jogo (após Red/Blue) |

## Encontros (para `encounters/usum.json`; `method: walk` salvo indicação; `sos` = aliado chamado; `conditions`: day/night)

- route-1: Pikipek, Buneary, Caterpie; Yungoos e Ledyba (dia); Alolan Rattata e Spinarak (noite); mais adiante Grubbin e Pichu (raros). Perto do laboratório: Slowpoke, Inkay, Wingull. Área do Tauros: Bonsly, Munchlax.
- trainers-school: Magnemite, Zorua, Alolan Meowth, Alolan Grimer.
- hauoli-city (grama atrás dos portões): Abra, Furfrou, Mime Jr.
- route-2: Smeargle, Makuhita, Drowzee, Ekans, Cutiefly, Growlithe, Dunsparce, Spearow (grama que se mexe avança).
- hauoli-cemetery: Gastly, Zubat; Drifloon (dia); Murkrow (noite).
- berry-fields: Crabrawler (pilha de berries, `fixed`).
- verdant-cavern (pós-trial): Zubat, Noibat, Alolan Diglett; nuvem de poeira: Yungoos [US] / Alolan Rattata [UM].
- route-3: Delibird, Hawlucha, Mankey; sombras no chão: Spearow, Rufflet [US] / Vullaby [UM].
- melemele-meadow: Cottonee [US] / Petilil [UM] (grama florida), Oricorio Pom-Pom Style.
- ten-carat-hill: Roggenrola, Mawile, Carbink; Sableye (`sos`). Área externa: Rockruff, Machop, Spinda.
- route-4: Lillipup, Mudbray, Igglybuff, Eevee; `sos`: Jigglypuff, Happiny (de Igglybuff); Espeon, Umbreon (de Eevee).
- paniola-ranch: Mareep, Tauros, Miltank; área de cima: Fomantis.
- brooklet-hill: Poliwag; Paras, Dewpider (manhã); Morelull, Surskit (noite); surf: Finneon, Tentacool; `fish`: Magikarp, Goldeen, Basculin, Feebas, Wishiwashi, Alomomola; Gyarados (`sos` de Magikarp); ondulação na água da trial aumenta Feebas/Alomomola.
- route-6: Oricorio Pa'u Style.
- route-7: surf: Pyukumuku; `fish`: Magikarp, Wishiwashi, Staryu.
- wela-volcano-park: Fletchling, Salandit, Magby, Cubone; Kangaskhan (raro).
- dividing-peak-tunnel: Wimpod (corre; `fixed`), Kecleon (sombra invisível).
- route-8: Stufful (raro), Wimpod fujão; `fish`: Chinchou (raro).
- lush-jungle: Fomantis, Comfey, Passimian [US] / Oranguru [UM]. Moss Rock (Leafeon).
- route-9: `fish`: Luvdisc, Corsola; Mareanie (`sos`).
- memorial-hill: Phantump. akala-outskirts: Natu, Nosepass.
- hano-beach: Staryu, Sandygast (sombras na areia).
- malie-outer-cape: Trubbish, Minccino.
- route-10: Fearow, Skarmory, Pancham.
- route-11: Komala.
- mount-hokulani (estrada do ônibus): Beldum, Minior, Ditto; Cleffa (noite).
- route-12: Alolan Graveler, Torkoal, Houndoom [US] / Manectric [UM].
- blush-mountain: Togedemaru, Dedenne, Turtonator [US]. Campo magnético: Magneton, Nosepass, Charjabug evoluem por nível.
- tapu-village: lago, `fish`: Bruxish (raro). Grama a oeste: Snorunt, Absol, Alolan Vulpix [US] / Alolan Sandshrew [UM]. Crabrawler evolui em Crabominable a partir daqui (entrada do Mount Lanakila).
- abandoned-thrifty-megamart (pós-trial): Shuppet, Klefki, Mimikyu.
- haina-desert: Krokorok, Trapinch; `sos` de Krokorok: Golett [US] / Baltoy [UM]. Dia = Sunny Day; noite = Sandstorm.
- route-16: Scraggy.
- ulaula-meadow: Oricorio Baile Style.
- seafolk-village: `fish`: Wailmer.
- poni-wilds: Granbull, Exeggcute, Gastrodon.
- poni-breaker-coast: ondulação: Clawitzer [US] / Dragalge [UM]; `fish`: Carvanha, Relicanth.
- exeggutor-island: Alolan Exeggutor; Pinsir (caem das árvores, `fixed`).
- vast-poni-canyon: Jangmo-o (raro, parte alta). Campo magnético (evoluções como no Blush Mountain).
- mount-lanakila: Sneasel; Drampa [UM] (caverna). Icy Rock (Glaceon).
- poni-grove: Riolu, Pinsir; Blacephalon [US] / Stakataka [UM] (`fixed`, Beast Ball).
- poni-meadow: Oricorio Sensu Style.
- resolution-cave: Druddigon; Zygarde 50% lv 60 (`fixed`).
- ultra-space, portais brancos (UB lv 60, `fixed`): Ultra Deep Sea — Nihilego (muito comum, ambas); Ultra Jungle — Buzzwole [US] (comum); Ultra Desert — Pheromosa [UM] (comum); Ultra Plant — Xurkitree (pouco comum, ambas); Ultra Forest — Kartana [US]; Ultra Crater — Celesteela [UM]; Ultra Ruin — Guzzlord (mais raro, ambas).
- ultra-space Cliff World (vermelho; comuns: Swellow, Altaria, Yanmega, Sigilyph, Swanna; lendários lv 60): Articuno, Zapdos, Moltres, Cresselia (ambas); Ho-Oh, Tornadus [US]; Thundurus, Yveltal [UM]; Landorus (com Tornadus e Thundurus no time); Rayquaza (com Kyogre e Groudon no time).
- ultra-space Water World (azul; comuns: Quagsire, Lombre, Floatzel, Stunfisk, Barbaracle): Uxie, Mesprit, Azelf (ambas); Latios [US]; Latias, Lugia, Kyogre [UM]; Suicune (com Raikou e Entei); Kyurem (com Reshiram e Zekrom).
- ultra-space Rocky World (verde; comuns: Nuzleaf, Grumpig, Drapion, Audino, Heliolisk): Mewtwo, Cobalion, Terrakion, Virizion (ambas); Raikou, Dialga, Reshiram, Xerneas [US]; Entei, Zekrom [UM].
- ultra-space Cave World (amarelo; comuns: Magcargo, Medicham, Hippowdon, Abomasnow, Crustle): Regirock, Regice, Registeel (ambas); Groudon, Heatran [US]; Palkia, Regigigas [UM]; Giratina (com Dialga e Palkia).
- Lendários da história (`fixed`): Necrozma lv 65 (Mount Lanakila); Solgaleo [US] / Lunala [UM] lv 60 (Plank Bridge); Tapu Koko, Tapu Lele, Tapu Bulu, Tapu Fini lv 60 (ruínas de cada ilha); Zygarde 50% lv 60.

## Presentes, trocas in-game e fósseis (`method: gift` / `trade`)

- Inicial: Rowlet, Litten ou Popplio (Route 1; nível não listado).
- Ovo de Eevee (Pokémon Nursery, Paniola Ranch).
- Pikachu de boné do 20º filme (Pikachu Valley, por QR Code).
- Aerodactyl (Seafolk Village; nível não listado).
- Type: Null + Memory Drives (Wicke, Poni Grove, pós-jogo; nível não listado).
- Poipole (Ultra Recon Squad, após Ultra Necrozma; nível não listado).
- Cosmog lv 5 (Lake of the Sunne/Moone no Reverse World, com Solgaleo/Lunala no time) + Z-Crystal do lendário oposto.
- Zygarde 10% lv 63 (Dexio, Route 16) + Zygarde Cube com 40 Zygarde Cells (Reassembly Unit no trailer).
- Porygon lv 30 (cientista da Aether, Aether House, após o Episode RR).
- Fósseis à venda na loja da Olivia, Konikoni City (alguns exclusivos de cada versão; espécies não listadas).
- Trocas in-game: nenhuma citada.

## Z-Crystals, TMs e itens

- Z-Crystals: Normalium Z (Verdant Cavern); Fightinium Z (Hala); Flyinium Z (Ten Carat Hill); Waterium Z (Lana); Firium Z (Kiawe); Grassium Z (Mallow); Rockium Z (Olivia); Lycanium Z (Hau, Malie); Steelium Z (Molayne, Hokulani); Electrium Z (Sophocles); Decidium/Incinium/Primarium Z do seu inicial (Kukui, Malie Garden; os outros dois com Hala no pós-jogo); Ghostium Z (Acerola); Psychium Z (Haina Desert); Buginium Z (Shady House); Darkinium Z (Nanu); Poisonium Z (Plumeria, Ancient Poni Path); Dragonium Z (Vast Poni Canyon); Fairium Z (Mina); Groundium Z (Hapu); Ultranecrozium Z e Icium Z (Mount Lanakila); Solganium Z [US] / Lunalium Z [UM] (Lillie, Plank Bridge); Mimikium Z (side quest noturna no Megamart); Tapunium Z (Tapu Koko); Eevium Z (tio do Thrifty Megamart); Kommonium Z (Plains Grotto).
- TMs citados: TM01 (Trainers' School, treinador do campinho); TM43 (Colress, saída do Dividing Peak Tunnel); TM67 (Kukui, Lush Jungle); TM nas pedras do Melemele Sea (Sharpedo); TM do Kukui no pós-jogo; TM da mãe do Guzma (Route 2); TM da Kahili (Hano Grand Resort); TMs de Surf e Waterfall (duas garotas, Poni Breaker Coast, após Tapu Fini).
- Ride Pager: Tauros Charge (Hala), Stoutland Search (Mallow), Lapras Paddle (Lana), Charizard Glide (Kiawe), Mudsdale Gallop (Hapu, Route 12), Sharpedo Jet (Grimsley), Machamp Shove (avó da Hapu); Mantine Surf (Big Wave Beach); Solgaleo/Lunala Ultra Warp Ride.
- Itens-chave: Z-Ring (Hala); Trainer Passport; Exp. Share (Kukui); Poké Finder (tourist bureau); Lens Case; Fishing Rod + 10 Dive Balls (Lana); Forage Bag (Mallow); Left Poké Ball (Heahea, dar à garotinha em Malie); Professor's Mask; Sun Flute / Moon Flute (Exeggutor Island); Master Ball (Gladion); Beast Balls (Lusamine 3; Ultra Recon Squad 5; à venda no Aether Paradise); Key Stone + Alakazite (Dexio); N-Solarizer + N-Lunarizer (Colress); Red Card (Looker); Galactic Key (Cyrus).
- Outros: Thunder Stone (Looker); Reveal Glass (Burnet); Grip Claw (Guzma); Max Elixir (Hau); Everstone (Ilima); Lucky Egg (Kukui, 50 capturas); Oval Charm (Morimoto); Shiny Charm (Iwao, Alola Dex completa sem Magearna/Marshadow/Zeraora); Mega Stones dos iniciais de Kanto (1ª vitória sobre um Battle Legend); Genesect Drives (Colress, Route 8); Protector (avô do Kiawe); Deep Sea Tooth + Deep Sea Scale (Lana); Dubious Disc (Faba); Up-Grade (Aether House); DNA Splicers, Prison Bottle, Soul Dew (Aether Paradise B2F); 55 Big Nuggets (Wicke, fim do Episode RR); Safety Goggles (hiker, Haina Desert à noite); Sun Stone [US] por Solrock / Moon Stone [UM] por Lunatone (Haina Desert); Plates de Arceus (antiquário do Hau'oli Mall e escondidas com Stoutland na Route 1, Hau'oli City e Cemetery); Poké Balls de Johto (Samson Oak: Malie Library com Alolan Persian, Outer Cape, Mount Hokulani, Geothermal Power Plant); Surge Badge réplica (Kantonian Gym).
- Lojas: loja da Olivia (Konikoni) vende Fire/Water/Thunder/Leaf Stone e fósseis; golpes por BP nas barracas da Ula'ula Beach; Battle Tree troca BP por itens; Mr. Hyper (Hyper Training) no Hau'oli Mall no pós-jogo; Move Reminder no Pokémon Center do Mount Lanakila (1 Heart Scale).

## Mecânicas (para o guia de mecânicas, texto nosso)

- Island Challenge: 7 trials com Totem Pokémon (chamam aliados) e 4 Grand Trials com os kahunas; carimbo no Trainer Passport.
- SOS: selvagens chamam aliados (alguns só aparecem assim); grama que se mexe, sombras no chão e nuvens de poeira avançam no jogador.
- Z-Moves com Z-Ring + Z-Crystal (coreografia por tipo); Mega Evolução só no pós-jogo (Key Stone do Dexio).
- Ride Pager substitui as HMs (ver lista acima).
- Mantine Surf entre ilhas rende BP (Beach Points). Battle Royal (4 treinadores, cada um por si).
- Poké Pelago (Pokémon Beans, berries, itens); Festival Plaza com Battle Agency (Pokémon emprestados); Roto Loto; Alola Photo Club.
- Treinadores-líderes: em cada área, um só luta depois de vencer todos os outros; Aristo (Mount Lanakila) após todos. Pokémon do Aristo ativam clima e seguram as pedras que o prolongam.
- Refeições diárias: Olivia (Konikoni, "special Z-set", 4 Heart Scales); Nanu (Malie, "Z-Kaiseki: Ronin Set", 8 Heart Scales); Hapu (Seafolk, "Zumongous Z-Noodles", 2 Heart Scales).
- Eeveelution trainers (lv 55): Vaporeon — professora da Trainers' School; Jolteon — Malie Community Center; Flareon — Tidal Song Hotel, Heahea; Espeon — Geothermal Power Plant; Umbreon — Hau'oli Cemetery; Leafeon — praia do Hano Grand Resort; Glaceon — casa em Iki Town; Sylveon — neta, barco Steelix de Seafolk.
- Ultra Warp Ride: giroscópio ou analógico (opção no prédio da Game Freak); 4 níveis de portal (mais enfeitado = mais raro e mais chance de shiny); distância também conta. No Ultra Space, Cubone/Exeggcute/Pikachu evoluem para a forma tradicional.
- Reverse World inverte o horário (Solgaleo só à noite, Lunala só de dia).
- Battle Tree: Single (3), Double (4), Multi (2); tudo lv 50; sem repetir espécie nem item; prêmios por sequência: 5 Moomoo Milk, 10 PP Up, 20 Rare Candy, 30 Bottle Cap, 40 PP Max, 50 Ability Capsule, 100 Lansat Berry, 200 Starf Berry; Battle Legend aos 20 e 50; Scout de parceiros por 10 BP.
- Defesa do título: a cada vitória na Elite 4 chega um desafiante (Hau, Gladion, Lusamine, Guzma, Plumeria, Sophocles, Hapu, Ryuki, Tristan, Kukui).
- Evoluções por local: Moss Rock (Lush Jungle), Icy Rock (Mount Lanakila), campo magnético (Blush Mountain, Vast Poni Canyon), Crabominable (Mount Lanakila).
- Pós-jogo (ordem do detonado): captura das UB no Poni Grove → invasão do Festival Plaza → Lucky Egg/TM do Kukui e revanche do Hala → revanches de captains e Totem → Morimoto/Iwao → Eeveelution trainers → Tapus → Ultra Space e Reverse World → Episode RR (Faba, Archie, Maxie, Cyrus, Lysandre, Ghetsis, Giovanni) → revanche do Faba e itens da Aether → Poni Grove/Plains/Meadow (Dexio, Meredith) → Resolution Cave (Zygarde) → Dexio no trailer → Aristo → Poni Coast/Gauntlet → Red ou Blue → Battle Tree → revanche da Elite 4 e defesa do título.

## Dúvidas

- Morimoto e Iwao: o texto põe os dois times em colunas misturadas; a divisão Gengar/Machamp/Dragonite (57) × Kecleon/Ambipom/Blissey (56) é a ordem da extração.
- Battle Legends Red e Blue (lv 50): o texto mistura as colunas; a divisão por treinador não é confiável.
- Kukui (defesa do título): o texto termina na página 136/137 sem o resto do time; numa página da Battle Tree aparece outro bloco com Lycanroc 69 duplicado, Snorlax 68 e Decidueye/Incineroar/Primarina "lv.50" (provável mistura de colunas).
- Sophocles [US] na pétala amarela: texto diz "Golem" (na defesa do título aparece "Alolan Golem").
- Números de Route 6, 7, 9, 10, 11, 12, 14, 15 e 17 não aparecem no detonado (inferidos); Route 1, 2, 3, 4, 5 (Paniola Ranch), 8 e 16 aparecem.
- Ultra Beast do primeiro encontro no Aether Paradise e níveis do inicial, Poipole, Type: Null, Aerodactyl e Blacephalon/Stakataka não constam.
- Dexio no Poni Meadow: texto lista "Slowking 60" (na Battle Tree o Dexio usa Slowbro).
- Drampa "em Moon" e Turtonator "em Ultra Sun": o detonado alterna Sun/Moon e Ultra Sun/Ultra Moon; assumido US/UM.
- Battle Tree: "Plumeria (em Sun)", "Guzma (em Moon)" etc.; assumido US/UM.
