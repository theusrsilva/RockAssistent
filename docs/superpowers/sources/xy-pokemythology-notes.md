# X / Y — fatos extraídos do detonado da Pokémon Mythology

Fonte: https://www.pokemythology.net/detonados/detonado-x-y/ (PDF salvo em 05/11/2026, 94 páginas).
Só **fatos de jogo** (chefes, níveis, encontros, itens) no nosso formato. Nenhuma prosa foi copiada. Golpes dos
chefes **não** constam no detonado: deixar `moves: []` e `verified: false` para golpes; espécies e níveis são
`verified: true` (uma fonte) até o analista conferir numa segunda. Exclusivos marcados `[X]` / `[Y]`.

Regra de iniciais (do detonado): Shauna fica com o inicial que perde para o seu; Calem/Serena com o que ganha do seu.
Shauna: Chespin→Froakie, Fennekin→Chespin, Froakie→Fennekin. Calem/Serena: Chespin→Fennekin, Fennekin→Froakie,
Froakie→Chespin (linhas confirmadas pelas tabelas por inicial das batalhas 9 e 27).

## Ordem da história e chefes (para `bosses/xy.json`)

| Order | Id | Kind | Nome | Local | Time (espécie lv) | Cap |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | shauna-1 | rival | Shauna | Aquacorde Town | inicial (fraco ao seu) 5 | 5 |
| 2 | viola | gym | Viola (Bug Badge) | Santalune City | Surskit 10, Vivillon 12 | 12 |
| 3 | sycamore-1 | other | Prof. Sycamore | Lab, Lumiose City (3º andar) | Bulbasaur 10, Charmander 10, Squirtle 10 | 10 |
| 4 | tierno-1 | rival | Tierno | Route 5 | Corphish 12 | 12 |
| 5 | tierno-trevor-1 | rival | Tierno + Trevor (dupla, ao lado de Calem/Serena) | Route 7 | Tierno: Corphish 18; Trevor: Pikachu 14, Flabébé 14 | 18 |
| 6 | snorlax | other | Snorlax (acordado com a Poké Flute) | ponte da Route 7 | Snorlax 15 | — (capturável) |
| 7 | grant | gym | Grant (Cliff Badge) | Cyllage City | Tyrunt 25, Amaura 25 | 25 |
| 8 | korrina-geosenge | other | Korrina | saída leste de Geosenge Town | Lucario ×2 (sem nível listado) | — |
| 9 | calem-1 | rival | Calem/Serena (disputa do Mega Ring) | Tower of Mastery, Shalour City | Meowstic 28, Absol 28, inicial evoluído 30 (Chespin→Braixen, Fennekin→Frogadier, Froakie→Quilladin; espécie do 2º estágio não escrita no detonado) | 30 |
| 10 | korrina | gym | Korrina (Rumble Badge) | Shalour City | Mienfoo 29, Machoke 28, Hawlucha 32 | 32 |
| 11 | korrina-lucario | other | Korrina (Lucario vs Lucario, Mega) | topo da Tower of Mastery | Mega Lucario (sem nível listado) | — |
| 12 | calem-2 | rival | Calem/Serena | entrada do ginásio, Coumarine City | Meowstic 31, Absol 31, inicial evoluído 33 | 33 |
| 13 | ramos | gym | Ramos (Plant Badge) | Coumarine City | Jumpluff 30, Weepinbell 31, Gogoat 34 | 34 |
| 14 | flare-admin-power-plant | evil | Team Flare Admin | Kalos Power Plant | Houndoom 36 | 36 |
| 15 | aliana-1 | evil | Aliana (Team Flare) | Kalos Power Plant | Mightyena 38 | 38 |
| 16 | clemont | gym | Clemont (Voltage Badge) | Prism Tower, Lumiose City | Emolga 35, Magneton 35, Heliolisk 37 | 37 |
| 17 | calem-3 | rival | Calem/Serena | Route 14 | Meowstic 35, Absol 35, inicial no último estágio 37 | 37 |
| 18 | valerie | gym | Valerie (Fairy Badge) | Laverre City | Mawile 38, Mr. Mime 39, Sylveon 42 | 42 |
| 19 | flare-admin-factory | evil | Team Flare Admin | Poké Ball Factory | Scraggy 37, Houndoom 38 | 38 |
| 20 | celosia-bryony-1 | evil | Celosia + Bryony (dupla, com Calem/Serena) | Poké Ball Factory | Celosia: Manectric 41; Bryony: Liepard 41 | 41 |
| 21 | mable-1 | evil | Mable (Team Flare) | Frost Cavern | Houndoom 48 | 48 |
| 22 | calem-4 | rival | Calem/Serena | entrada do ginásio, Anistar City | Meowstic 44, Absol 44 + por inicial: Chespin→Jolteon 44, Delphox 46; Fennekin→Flareon 44, Greninja 46; Froakie→Vaporeon 44, Chesnaught 46 | 46 |
| 23 | olympia | gym | Olympia (Psychic Badge) | Anistar City | Sigilyph 44, Slowking 45, Meowstic 48 | 48 |
| 24 | lysandre-1 | evil | Lysandre | Lysandre Labs (Lysandre Café) | Mienfoo 45, Murkrow 45, Pyroar 47, Gyarados 49 | 49 |
| 25 | aliana-2 | evil | Aliana | Lysandre Labs | Mightyena 46, Druddigon 48 | 48 |
| 26 | celosia-2 | evil | Celosia | Lysandre Labs | Manectric 46, Drapion 48 | 48 |
| 27 | bryony-2 | evil | Bryony | Lysandre Labs | Liepard 46, Bisharp 48 | 48 |
| 28 | mable-2 | evil | Mable (dá a Elevator Key) | Lysandre Labs | Houndoom 46, Weavile 48 | 48 |
| 29 | xerosic-1 | evil | Xerosic | Lysandre Labs | Crobat 46, Malamar 48 | 48 |
| 30 | lysandre-2 | evil | Lysandre | Team Flare Secret HQ (Geosenge Town) | Mienshao 47, Honchkrow 47, Pyroar 49, Gyarados 51 | 51 |
| 31 | box-legendary | other | Xerneas [X] / Yveltal [Y] | Team Flare Secret HQ | 50 | — (captura) |
| 32 | lysandre-3 | evil | Boss Lysandre (Mega) | Team Flare Secret HQ | Mienshao 49, Honchkrow 49, Pyroar 51, Mega Gyarados 51 | 51 |
| 33 | sycamore-2 | other | Prof. Sycamore | Couriway Town (sul) | Venusaur 50, Charizard 50, Blastoise 50 | 50 |
| 34 | shauna-2 | rival | Shauna | ponte da Route 19 | Delcatty 49, Goodra 49 + por inicial: Chespin→Greninja 51; Fennekin→Chesnaught 51; Froakie→Delphox 51 | 51 |
| 35 | tierno-2 | rival | Tierno | Route 19 | Talonflame 48, Roserade 49, Crawdaunt 52 | 52 |
| 36 | trevor-2 | rival | Trevor (Shauna dá HM05 depois) | Route 19 | Raichu 49, Aerodactyl 49, Florges 51 | 51 |
| 37 | wulfric | gym | Wulfric (Iceberg Badge) | Snowbelle City | Cryogonal 55, Abomasnow 56, Avalugg 59 | 59 |
| 38 | calem-5 | rival | Calem/Serena | Victory Road | Meowstic 57, Altaria 57, Absol 59 + por inicial: Chespin→Jolteon 57, Delphox 61; Fennekin→Flareon 57, Greninja 61; Froakie→Vaporeon 57, Chesnaught 61 | 61 |
| 39 | drasna | elite | Drasna (Dragonmark Chamber) | Pokémon League | Dragalge 63, Altaria 63, Druddigon 63, Noivern 65 | 65 |
| 40 | wikstrom | elite | Wikstrom (Ironworks Chamber) | Pokémon League | Klefki 63, Probopass 63, Scizor 63, Aegislash 65 | 65 |
| 41 | malva | elite | Malva (Blazing Chamber) | Pokémon League | Pyroar 63, Torkoal 63, Chandelure 63, Talonflame 65 | 65 |
| 42 | siebold | elite | Siebold (Flood Chamber) | Pokémon League | Clawitzer 63, Starmie 63, Gyarados 63, Barbaracle 65 | 65 |
| 43 | diantha | champion | Diantha (Radiant Chamber) | Pokémon League | Hawlucha 64, Tyrantrum 65, Aurorus 65, Gourgeist 65, Goodra 66, Mega Gardevoir 68 | 68 |
| 44 | az | other | AZ (depois do desfile) | Lumiose City | Torkoal 60, Golurk 60, Sigilyph 60 | 60 |
| 45 | calem-kiloude | rival | Calem/Serena (pós-jogo; precisa de 1 batalha na Battle Maison; depois diário) | norte de Kiloude City | Meowstic 66, Altaria 68, Clefable 68, Mega Absol 68 + por inicial: Chespin→Jolteon 68, Delphox 70; Fennekin→Flareon 68, Greninja 70; Froakie→Vaporeon 68, Chesnaught 70 | 70 |
| 46 | essentia-1 | other | Essentia (Emma) ×4, Looker cap. 5 | andar secreto do Lysandre Labs | sem time listado (depois usa os Pokémon de Xerosic: Crobat, Malamar) | — |
| 47 | malva-looker | other | Malva (Looker cap. 5) | Royal Suite, 5º andar, Hotel Richissime | Pyroar (sem nível listado) | — |
| 48 | articuno-zapdos-moltres | other | ave lendária pelo inicial (Chespin→Articuno, Fennekin→Zapdos, Froakie→Moltres) | Sea Spirit's Den, Azure Bay | 70 | — |
| 49 | mewtwo | other | Mewtwo | Unknown Dungeon, Pokémon Village | 70 | — |
| 50 | zygarde | other | Zygarde | fundo da Terminus Cave, Route 18 | 70 | — |

Battle Maison (Kiloude City, pós-jogo, todos lv 50; castelã na 20ª e na 50ª vitória seguida):
- Single — Nita: 20ª Wigglytuff 50 (só um listado); 50ª Landorus, Purugly, Grumpig, Tornadus, Thundurus 50.
- Double — Evelyn: 20ª Primeape, Persian, Pachirisu, Lumineon 50; 50ª Suicune, Entei, Raikou, Latios 50.
- Triple — Dana: 20ª Dragalge, Whimsicott, Magcargo 50 (só três listados); 50ª Articuno, Zapdos, Moltres, Magneton, Piloswine, Girafarig, Regirock, Regice, Registeel 50 (nove nomes; ver Dúvidas).
- Rotation — Morgan: 20ª Sawsbuck ("awsbuck" no texto), Mantine, Klefki, Swalot 50; 50ª Cobalion, Terrakion, Virizion, Latias 50.
- Multi: parceiro local, ou Shauna/Tierno/Trevor/amigo.

Battle Chateau (Route 7; treinadores mudam com as horas):
- Marquês/Marquesa → líderes lv 40: Viola (Masquerain, Vivillon); Grant (Tyrantrum, Aurorus); Korrina (Hawlucha, Machamp); Ramos (Victreebel, Gogoat); Clemont (Magneton, Heliolisk); Valerie (Mawile, Sylveon); Olympia (Sigilyph, Meowstic); Wulfric (Cryogonal, Avalugg).
- Duque/Duquesa → líderes lv 50: Viola (Masquerain, Vivillon); Grant (Tyrantrum, Aurorus); Korrina (Hawlucha, Lucario); Ramos (Victreebel, Gogoat); Clemont (Magnezone, Heliolisk); Valerie (Mawile, Sylveon); Olympia (Slowking, Meowstic); Wulfric (Abomasnow, Avalugg). E Elite Four lv 55: Wikstrom (Scizor, Probopass, Aegislash); Malva (Pyroar, Talonflame, Chandelure); Drasna (Dragalge, Noivern, Altaria); Siebold (Clawitzer, Barbaracle, Starmie).
- Grão-duque/Grã-duquesa → Diantha lv 60: Hawlucha, Goodra, Gourgeist, Mega Gardevoir.
- Títulos: barão → visconde → conde → marquês → duque → grão-duque. Writ of Invitation (mais treinadores até a meia-noite), Writ of Challenge (níveis maiores até a meia-noite).

## Áreas (para `areas/xy.json`, kind e ordem de liberação)

| Ordem | Id | Nome | Kind | Libera |
| --- | --- | --- | --- | --- |
| 1 | vaniville-town | Vaniville Town | city | início |
| 2 | route-1 | Route 1 (sem selvagens) | route | início |
| 3 | aquacorde-town | Aquacorde Town | city | início (inicial) |
| 4 | route-2 | Route 2 | route | após inicial |
| 5 | santalune-forest | Santalune Forest | dungeon | após Route 2 |
| 6 | route-3 | Route 3 | route | após a floresta |
| 7 | santalune-city | Santalune City | city | após Route 3 |
| 8 | route-22 | Route 22 (trecho inicial; resto até a Victory Road) | route | Santalune; completa com as 8 insígnias |
| 9 | route-4 | Route 4 | route | após Viola |
| 10 | lumiose-city | Lumiose City (South/North Boulevard, Magenta/Rouge/Centrico Plaza, Vernal Avenue) | city | após Route 4; saídas fechadas até a Power Plant |
| 11 | route-5 | Route 5 | route | após Lumiose |
| 12 | camphrier-town | Camphrier Town (Shabboneau Castle) | city | após Route 5 |
| 13 | route-6 | Route 6 | route | após Camphrier |
| 14 | parfum-palace | Parfum Palace | building | Route 6 ($1000) |
| 15 | route-7 | Route 7 (Day Care, Battle Chateau; berries ao sul) | route | após Poké Flute (Snorlax) |
| 16 | battle-chateau | Battle Chateau | building | Route 7 |
| 17 | connecting-cave | Connecting Cave | dungeon | após Route 7 |
| 18 | route-8 | Route 8 | route | após Connecting Cave |
| 19 | ambrette-town | Ambrette Town (Fossil Lab, Ambrette Aquarium) | city | após Route 8 |
| 20 | route-9 | Route 9 (Rhyhorn) | route | após Ambrette |
| 21 | glittering-cave | Glittering Cave | dungeon | Route 9 |
| 22 | ambrette-beach | praia depois do Ambrette Aquarium (nº da rota não citado) | route | após Glittering Cave |
| 23 | cyllage-city | Cyllage City | city | após a praia |
| 24 | route-10 | Route 10 | route | após Grant |
| 25 | geosenge-town | Geosenge Town | city | após Route 10 |
| 26 | route-11 | Route 11 | route | após Geosenge |
| 27 | reflection-cave | Reflection Cave | dungeon | Route 11 |
| 28 | shalour-city | Shalour City | city | após Reflection Cave |
| 29 | tower-of-mastery | Tower of Mastery | building | Shalour |
| 30 | route-12 | Route 12 (rancho de Skiddo) | route | após Korrina (HM03) |
| 31 | azure-bay | Azure Bay | sea | Route 12 com Surf |
| 32 | coumarine-city | Coumarine City (monotrilho) | city | após Route 12 |
| 33 | route-13 | Route 13 (deserto) | route | após Ramos |
| 34 | kalos-power-plant | Kalos Power Plant | building | Route 13 (Power Plant Pass) |
| 35 | prism-tower | Prism Tower (ginásio) | building | após a Power Plant |
| 36 | route-14 | Route 14 (pântano, chuva) | route | após Clemont |
| 37 | scary-house | Scary House | building | Route 14 |
| 38 | laverre-city | Laverre City | city | após Route 14 |
| 39 | poke-ball-factory | Poké Ball Factory | building | após Valerie |
| 40 | dendemille-town | Dendemille Town | city | após a Factory |
| 41 | route-15 | Route 15 | route | Dendemille |
| 42 | route-16 | Route 16 (liga a Lumiose) | route | Route 15 com Strength |
| 43 | lost-hotel | Lost Hotel | building | Routes 15/16 |
| 44 | frost-cavern | Frost Cavern | dungeon | norte de Dendemille |
| 45 | route-17 | Route 17 (Mamoswine) | route | após Frost Cavern |
| 46 | anistar-city | Anistar City (sundial) | city | após Route 17 |
| 47 | lysandre-labs | Lysandre Labs | building | após Olympia |
| 48 | team-flare-secret-hq | Team Flare Secret HQ | dungeon | após Lysandre Labs |
| 49 | route-18 | Route 18 | route | após a HQ (sul de Anistar) |
| 50 | terminus-cave | Terminus Cave | dungeon | Route 18 (fundo só no pós-jogo) |
| 51 | couriway-town | Couriway Town | city | após Route 18 |
| 52 | route-19 | Route 19 | route | após Sycamore |
| 53 | snowbelle-city | Snowbelle City | city | após Route 19 |
| 54 | route-20 | Route 20 (floresta) | route | Snowbelle |
| 55 | pokemon-village | Pokémon Village | zone | fim da Route 20 |
| 56 | route-21 | Route 21 | route | após Wulfric |
| 57 | victory-road | Victory Road | dungeon | 8 insígnias |
| 58 | pokemon-league | Pokémon League | building | após Victory Road |
| 59 | kiloude-city | Kiloude City (Battle Maison, Friend Safari) | city | pós-jogo (TMV Pass, Lumiose Station) |
| 60 | battle-maison | Battle Maison | building | pós-jogo |
| 61 | friend-safari | Friend Safari | zone | pós-jogo |
| 62 | looker-bureau | Looker Bureau (Rouge Plaza, Lumiose) | building | pós-jogo, após upgrade do Mega Ring |
| 63 | sea-spirits-den | Sea Spirit's Den | dungeon | pós-jogo, após 10–15 encontros com a ave |
| 64 | unknown-dungeon | Unknown Dungeon | dungeon | pós-jogo |
| 65 | chamber-of-emptiness | Chamber of Emptiness | dungeon | Route 22 com Surf/Waterfall |

## Encontros (para `encounters/xy.json`; method `walk` salvo indicação)

- Route 2: Pidgey, Fletchling, Bunnelby, Zigzagoon, Scatterbug, Weedle [X], Caterpie [Y].
- Santalune Forest: Pikachu, Pansage, Pansear, Panpour, Weedle, Caterpie, Kakuna [X], Metapod [Y].
- Route 3: Dunsparce, Azurill, Bidoof, Burmy (+ espécies da Route 2).
- Route 22: Litleo, Psyduck, Riolu, Farfetch'd.
- Route 4: Ledyba, Ralts, Skitty, Budew, Combee, Flabébé (a espécie depende da cor da flor).
- Route 5: Furfrou, Pancham, Skiddo, Doduo, Abra, Gulpin, Plusle, Minun.
- Route 6: Kecleon, Sentret, Nincada, Oddish, Espurr, Honedge; Audino e Venipede saltam dos arbustos.
- Route 7: Roselia, Croagunk, Ducklett, Smeargle, Volbeat, Illumise, Swirlix [X], Spritzee [Y]; `horde`: Psyduck, Hoppip, Budew.
- Connecting Cave: Zubat, Whismur, Meditite, Axew; `horde`: Zubat, Whismur, Axew.
- Route 8: Inkay, Spoink, Drifloon, Mienfoo, Absol, Bagon, Zangoose [X], Seviper [Y]; `horde`: Taillow, Wingull, Zangoose, Seviper; arbustos: Lunatone, Solrock. Chuva em alguns dias.
- Ambrette Town: `rock_smash` Dwebble, Binacle. Praia depois do Aquarium: `fish` (Old Rod) Luvdisc; Old Rod nos outros lugares: Magikarp.
- Route 9 (andando de Rhyhorn): Helioptile, Hippopotas, Sandile.
- Glittering Cave (`cave`, sombras): Onix, Machop, Woobat, Rhyhorn, Cubone, Ferroseed, Mawile, Kangaskhan, Lunatone, Solrock.
- Route 10: Eevee, Hawlucha, Sigilyph, Emolga, Snubbull, Golett, Houndour [X], Electrike [Y]; `horde`: Nosepass, Yanma.
- Route 11: Nidorino, Nidorina, Dedenne, Staravia, Hariyama, Chingling, Stunky, Sawk [X], Throh [Y]; `horde`: Nidoran♂, Nidoran♀, Starly, Stunky.
- Reflection Cave: Roggenrola, Solosis, Wobbuffet, Mr. Mime, Sableye, Chingling, Carbink; sombras (`cave`): Ferroseed, Woobat; `horde`: Mime Jr., Roggenrola, Carbink.
- Route 12: Slowpoke, Exeggcute, Pachirisu, Chatot, Tauros, Miltank, Pinsir [X], Heracross [Y]; `horde`: Mareep, Tauros, Miltank, Wingull; `surf`: Tentacool, Mantyke, Lapras.
- Route 13 (montes de areia que andam): Dugtrio, Trapinch, Gible; `rock_smash`: Graveler, Slugma. Magneton e Nosepass evoluem ao subir de nível aqui.
- Route 14: Goomy, Quagsire, Carnivine, Weepinbell, Haunter, Skorupi, Karrablast, Shelmet; água rasa: Stunfisk; `surf`: Goomy, Quagsire, Stunfisk; `fish` Old Rod: Poliwag; Good Rod: Poliwhirl, Barboach; `horde`: Ekans, Bellsprout, Skorupi.
- Route 15: Foongus, Pawniard, Klefki, Skorupi, Watchog, Mightyena [X], Liepard [Y]; `horde`: Foongus, Klefki, Murkrow; `surf`: Lombre, Floatzel; `fish` Good Rod: Basculin.
- Route 16: Phantump, Pumpkaboo.
- Lost Hotel: Electrode, Litwick, Magneton; lixeiras: Rotom, Trubbish; `horde`: Magnemite, Voltorb.
- Frost Cavern: Haunter, Jynx, Piloswine, Beartic, Bergmite; `horde`: Cubchoo, Vanillite, Smoochum; sombras (`cave`): Cryogonal. Pedra de gelo (Eevee → Glaceon ao subir de nível).
- Route 17 (saltam da neve, andando de Mamoswine): Sneasel, Delibird, Snover, Abomasnow.
- Route 18: Heatmor, Durant, Sandslash, Torkoal, Graveler, Gurdurr, Lairon [X], Pupitar [Y]; arbustos: Skarmory; `rock_smash`: Shuckle; `horde`: Geodude, Heatmor, Durant.
- Terminus Cave (`cave`, sombras): Ariados, Noibat.
- Route 19: Weepinbell, Haunter, Sliggoo, Karrablast, Shelmet, Quagsire, Carnivine, Drapion; água rasa: Stunfisk; `horde`: Ekans, Weepinbell, Gligar, Skorupi.
- Route 20: Trevenant, Amoonguss, Noctowl, Zoroark, Gothita, Jigglypuff; `horde`: Foongus, Sudowoodo, Trevenant. Pedra com musgo (Eevee → Leafeon).
- Pokémon Village: Ditto, Amoonguss; lixeiras: Banette, Garbodor.
- Route 21: Scyther, Ursaring, Spinda, Altaria, Floatzel; `horde`: Scyther, Spinda, Swablu; `surf`: Lombre, Floatzel.
- Victory Road: Graveler, Haunter, Gurdurr, Lickitung, Druddigon, Zweilous; `horde`: Lickitung, Teddiursa, Floatzel, Geodude, Gurdurr; sombras (`cave`): Fearow, Skarmory, Hydreigon.
- Friend Safari (`zone`): tipo e espécies definidos pelo Friend Code de cada amigo; lv 30; Hidden Ability se o amigo estiver online; sem limite de passos; sem Safari Balls.
- Lendários e fixos (`fixed`): Snorlax lv 15 (Route 7); Xerneas [X] / Yveltal [Y] lv 50 (Team Flare Secret HQ, história); Articuno (Chespin) / Zapdos (Fennekin) / Moltres (Froakie) lv 70: vaga pelo mato e água de Kalos e foge na hora; depois de 10–15 encontros fica na Sea Spirit's Den (Azure Bay); Mewtwo lv 70 (Unknown Dungeon, Pokémon Village; dá Mewtwonite X [X] / Y [Y]); Zygarde lv 70 (fundo da Terminus Cave).

## Presentes, trocas in-game e fósseis (`method: gift` / `trade`)

- Inicial: Chespin / Fennekin / Froakie (Aquacorde Town; nível não listado). Inicial de Kanto: Bulbasaur / Charmander / Squirtle (Prof. Sycamore, Lumiose; nível não listado), com a Mega Stone correspondente.
- Lucario com Lucarionite (Korrina, topo da Tower of Mastery; nível não listado).
- Lapras lv 30 (primeira pessoa da Route 12).
- Fóssil (Glittering Cave, escolha): Jaw Fossil → Tyrunt ou Sail Fossil → Amaura; revive no Fossil Lab de Ambrette (nível não listado).
- Trocas: Bunnelby → Farfetch'd (Santalune City); qualquer → Ralts lv 5 com Gardevoirite (Diantha, Café Soleil, pós-jogo); qualquer → inicial fraco ao seu (Shauna, Vaniville, pós-jogo: Chespin/Fennekin/Froakie, nascido de ovo).
- Item por troca, diário: Poké Ball → Dive Ball (Ambrette Town).

## Mega Stones

- Da história: Venusaurite/Charizardite/Blastoisinite do inicial de Kanto (Sycamore, nome exato não escrito); Lucarionite (Korrina); Aerodactylite (pesquisador, Ambrette); Ampharosite (Azure Bay); Gengarite (menina de Laverre City); Abomasite (Abomasnow, Frost Cavern).
- Stone Emporium (Lumiose): vende as de Venusaur, Charizard e Blastoise.
- Pós-jogo: Absolite (Calem/Serena, Kiloude); Gardevoirite (Diantha, troca do Ralts); Mewtwonite X [X] / Y [Y] (Mewtwo).
- Sundial (Anistar; upgrade do Mega Ring; brilha das 20h às 21h, pedras só aparecem nessa hora): Medichamite (Laverre City, frente do poço); Banettite (Chamber of Emptiness, Route 22 via Surf e cachoeira); Garchompite (Victory Road, à esquerda do ponto da batalha com o rival, Rock Smash); Kangaskhanite (Glittering Cave, 1º corredor à direita); Gyaradosite (Couriway Town, perto das cachoeiras); Mawilite (Shabboneau Castle, Camphrier, andar de cima); Scizorite (Frost Cavern, atrás do Abomasnow); Alakazite (Reflection Cave, entrada vista pelo reflexo); Pinsirite [X] / Heracronite [Y] (Santalune Forest, gramado de cima); Manectite [X] / Houndoominite [Y] (Route 16, dois patinadores em volta da pedra); Tyranitarite [X] / Aggronite [Y] (ginásio de Grant, escalando a parede).

## TMs, HMs e itens-chave (localização)

- HM01 Cut (Parfum Palace); HM04 Strength (Grant, Cyllage City; uso fora de batalha após a Cliff Badge); HM03 Surf (Calem/Serena, saída leste de Shalour); HM02 Fly (Sycamore, estação do monotrilho de Coumarine; uso após a Plant Badge); HM05 Waterfall (Shauna, Route 19).
- TM94 Rock Smash (menina, Ambrette Town); TM70 Flash (Tierno, Reflection Cave).
- Itens-chave: Town Map (mãe, Vaniville); patins (menina na porta do ginásio de Santalune); Exp. Share (Alexa, após Viola); Honey (Trevor, Route 5); Sprinklotad (Route 7); Poké Flute (dono do Furfrou, Parfum Palace); Old Rod (Ambrette Aquarium); Dowsing Machine (garota, praia após o Aquarium); bicicleta (Cyllage City, amarela ou verde); Intriguing Stone (Tierno, Shalour); Mega Ring (Korrina); Good Rod (pescador, Coumarine); Power Plant Pass (Route 13); Super Rod (casa da Route 16); Elevator Key (Mable, Lysandre Labs); Poké Radar (lab do Sycamore, 2º andar, pós-jogo); TMV Pass (Sycamore, Lumiose Station); VS. Recorder (Kiloude City).
- Outros: 5 Luxury Ball (lab, 2º andar); Quick Claw, 3 Timer Ball, 3 Quick Ball (Lumiose); Amulet Coin (Parfum Palace); Lucky Egg (hotel de Coumarine, com Pokémon de felicidade alta); King's Rock (Lysandre, Lysandre Café); Master Ball + Big Nugget (presidente da Poké Ball Factory); 10 Repeat Ball (assistente, Anistar); 2 Full Restore (Power Plant).
- Pedras de evolução: Leaf Stone (Route 8); Fire Stone (Route 9, escondida); Thunder Stone (Route 10); Moon Stone (Reflection Cave); Water Stone e Shiny Stone (Route 12); Sun Stone (Route 13).
- Lojas e serviços: tutor dos golpes Frenzy Plant / Blast Burn / Hydro Cannon (Snowbelle City); Stone Emporium e Lumiose Galette (Lumiose); restaurante com batalhas em dupla diárias (Lumiose); Moomoo Milk à venda (Dendemille); Soda Pop a preço de Potion (Cyllage); cabeleireiro e boutiques (Lumiose, Santalune).

## Mecânicas (para o guia de mecânicas, texto nosso)

- Mega Evolução: Mega Ring + Mega Stone; o botão aparece ao lado dos golpes.
- Exp. Share dá experiência à equipe toda; capturar um Pokémon também dá experiência.
- Hordas: até 5 selvagens de uma vez (Honey ou ao acaso), nível menor, podem ter Hidden Ability; só se captura quando sobra um.
- Sky Battles: só Pokémon voadores ou com Levitate (Sky Trainers).
- Montarias: Rhyhorn (Route 9), Skiddo (rancho da Route 12), Mamoswine (Route 17) quebram pedras e atravessam terreno.
- Berries em árvores grandes; plantio com o Sprinklotad.
- Patins e trilhos; bicicleta e patins não funcionam em deserto, neve e Reflection Cave.
- Lapras é o único Pokémon que aparece no Surf.
- Pokédex de Kalos em três partes: Central, Coastal (Route 8), Mountain (Route 13); National no pós-jogo.
- Lumiose: táxi; Holo Caster para ligações.
- Battle Chateau com títulos de nobreza; Battle Maison sem limite de sequência e com castelãs (20ª e 50ª).
- Friend Safari depende da lista de amigos.
- Missões do Looker Bureau (pós-jogo, 6 capítulos: tickets, Mimi/Emma, ladrões na estação, Essentia, Malva/Xerosic, despedida).

Pós-jogo, ordem de eventos: troca com a Shauna → Poké Radar → Lumiose Station (National Dex, TMV Pass) → Kiloude City (Battle Maison, Friend Safari) → rival em Kiloude (Absolite) → sundial em Anistar (Mega Stones das 20h–21h) → Looker Bureau (Diantha troca o Ralts) → ave lendária / Sea Spirit's Den → Mewtwo → Zygarde.

## Dúvidas

- Páginas 45–48 e 54–57 do PDF não têm texto (só imagens): podem esconder times dos admins da Team Flare HQ (duplas com Calem/Serena) e outras batalhas. O PDF não pôde ser renderizado aqui (sem poppler).
- Espécie do 2º estágio do inicial de Calem/Serena nas batalhas 9 e 12: o detonado só diz "inicial evoluído"; a tabela usa a regra das linhas.
- Níveis do inicial, do inicial de Kanto, do Lucario da Korrina, dos fósseis, dos Lucario da Korrina em Geosenge, do Pyroar da Malva e do time da Essentia: não listados.
- Battle Maison: Nita 20ª (1 Pokémon) e Dana 20ª (3) parecem incompletos; Dana 50ª lista 9 espécies (talvez dois times juntos).
- Erros de digitação no texto: "Houndooom", "Hounchkrow" (Honchkrow), "Meowtic", "awsbuck", "Wiksttrom", "Bannette", "Ferrossed".
- Nº da rota da praia entre o Ambrette Aquarium e Cyllage não citado.
- O Pikachu e a Flabébé do Trevor (lv 14) e o Corphish do Tierno (lv 18) na dupla da Route 7: o texto separa só por quebra de página.
