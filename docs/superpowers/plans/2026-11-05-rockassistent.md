# RockAssistent Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Transformar o TradeRock no RockAssistent: assistente Pokémon offline de todos os jogos (Pokédex, Guias, Boxes com saves por pasta, Nuzlocke, Contador Hunt, Histórico unificado), mantendo a troca com o Switch intacta.

**Architecture:** Dados da PokeAPI compilados por script num SQLite somente leitura (`assets/data/dex.sqlite`, gerado, não versionado) e servidos pelo backend em `/api/dex`. Dados nossos (chefes, treinadores, encontros dos jogos de Switch, métodos de shiny, áreas e mapas) em JSON embutido no `TradeRock.Core` e servidos em `/api/data`. Runs de Nuzlocke, hunts e anexos de save em novas tabelas do SQLite do app (EF Core, com escopo por conta). Frontend React segue a estrutura atual (páginas em `frontend/src/pages/<área>/`, `ui.tsx`, tokens de `index.css`); guias em markdown em `content/{pt-BR,en}/guides/**` carregados sob demanda.

**Tech Stack:** .NET 10, ASP.NET Core minimal API, EF Core + SQLite, Microsoft.Data.Sqlite (já vem com o EF), PKHeX.Core 26.8.26, React 19 + react-router 8 + i18next + Tailwind 4 + Vite 8 + Vitest, Python 3.12 (scripts), xUnit v3.

**Spec:** `docs/superpowers/specs/2026-11-05-rockassistent-design.md`

## Global Constraints

- Conversar e escrever commits/código/comentários em **português**; textos de interface em `frontend/src/locales/{pt-BR,en}.json`; textos explicativos em `content/{pt-BR,en}/**/*.md` (mesmos nomes nas duas línguas).
- **Não inventar APIs** do PKHeX.Core, do Photino, do EF Core ou de qualquer biblioteca: consultar o pacote NuGet (`~/.nuget/packages/pkhex.core/26.8.26/`) ou o código-fonte. Se algo não existir, reportar.
- A troca com o Switch, as raids, o Union Circle, o Mystery Gift e o modo servidor **não mudam de comportamento**; só mudam de lugar no menu.
- **Offline:** nenhuma chamada de rede em tempo de uso, exceto a procura de atualização (D29) e as opções de internet desligadas por padrão (spec 4.10). Nada do PC é enviado.
- **Licenças:** PokeAPI (BSD-3) é a fonte embutida. Fatos de jogo podem ser pesquisados em qualquer site e escritos no nosso formato. Texto corrido (guias, lore, walkthrough) é sempre escrito do zero; nunca copiar prosa de Serebii, Bulbapedia, allgenwiki ou outro site. Dado sem fonte conferida leva `"verified": false`.
- Namespaces, pastas, scripts, nomes de pacote NuGet, nome do binário (`traderock`), AppId do Inno Setup, nome do pacote `.deb` (`traderock`) e pasta de dados **ficam como estão**. Só o nome visível vira **RockAssistent** (grafia exata).
- JSON dos enums em snake_case (`ServiceSetup.EnumNaming`). Novas entidades com dados do usuário implementam `IOwned` e entram nos filtros de dono do `TradeRockDb`.
- Visual e tema atuais: usar `Button`, `Card`, `PageTitle`, `Badge`, `Modal`, `Segmented`, `Icon` etc. de `src/components/ui.tsx` e as cores `bg-surface`, `text-muted`, `bg-accent`...; sprites por `spriteUrl(species)` em todo lugar onde um Pokémon aparece.
- Segurança: prod.keys nunca lidos fora de `Setup/`; o app nunca roda como root; sem telemetria.
- Rodar `scripts/ci.sh` antes de commitar a última tarefa de cada entrega (ele leva ~10 min por causa do pokeldn; nas tarefas intermediárias, rodar `cd frontend && npm run typecheck && npm test` e `dotnet test --project app/TradeRock.Tests`).
- Trailer de commit: `Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>`.
- Nomes de golpes, itens e habilidades em inglês; o resto traduzido.

## Review Focus

1. **`.sav` aberto pelo emulador ou corrompido:** abrir uma pasta com um `.sav` truncado, de outro jogo (NDS ROM save com footer) ou em uso não derruba o app; a lista mostra o arquivo com erro e o resto continua (teste na Task 16).
2. **Pokémon sem dados no jogo escolhido** (ex.: Gengar no LGPE não existe; Sprigatito no FRLG): a página da Pokédex mostra "não existe neste jogo" e as abas ficam vazias sem erro (teste na Task 5).
3. **Atalho do contador disparando dentro de campos de texto:** digitar "+" num input não soma; o atalho só vale fora de inputs e com a hunt aberta (teste na Task 20).
4. **Banco existente de um usuário antigo:** as migrações novas rodam sobre um banco com cofre, histórico e contas sem perder linhas; `OwnerId` das tabelas novas é estampado (teste na Task 19).
5. **Run de Nuzlocke em jogo sem treinadores comuns (Switch):** a tela mostra o aviso e deixa registrar tudo manualmente; nenhuma consulta falha por lista vazia (teste na Task 22).

---

## Entrega A: base e rebranding

### Task 1: Catálogo de todos os jogos (`GameVersions`)

**Files:**
- Create: `app/TradeRock.Core/Games/GameVersions.cs`
- Create: `frontend/src/lib/gameVersions.ts`
- Test: `app/TradeRock.Tests/GameVersionsTests.cs`, `frontend/src/lib/gameVersions.test.ts`

**Interfaces:**
- Produces (C#):
  ```csharp
  public enum Region { Kanto, Johto, Hoenn, Sinnoh, Unova, Kalos, Alola, Galar, Hisui, Paldea, Lumiose }
  public sealed record GameVersionGroup(
      string Key,                 // "rb","y","gs","c","rs","e","frlg","dp","pt","hgss","bw","b2w2","xy","oras","sm","usum","lgpe","swsh","bdsp","pla","sv","za"
      string Name,                // "Red / Blue", "FireRed / LeafGreen"...
      int Generation,             // 1..9
      Region Region,
      string PokeApiVersionGroup, // identifier de version_groups.csv ("red-blue", "firered-leafgreen"...)
      IReadOnlyList<string> PokeApiVersions, // identifiers de versions.csv
      GameFamily? Family,         // ligação com o GameCatalog quando é jogo de Switch
      bool HasShinyCharm,         // true de b2w2 em diante (inclui lgpe); false até bw
      bool HasMasuda);            // true de dp em diante (exceto lgpe, pla e za, que não têm breeding)
  public static class GameVersions { IReadOnlyList<GameVersionGroup> All; GameVersionGroup ByKey(string key); GameVersionGroup? ByFamily(GameFamily f); }
  ```
- Produces (TS): `GAME_VERSIONS: GameVersionDef[]` com os mesmos campos em camelCase e `gameVersionByKey(key)`, `gameVersionsByGeneration()`.
- Endpoint: `GET /api/dex/games` (feito na Task 5) devolve `GameVersions.All`; o espelho TS existe para não depender de rede antes do primeiro fetch.

- [ ] **Step 1: Teste C#** `GameVersionsTests`: `All` tem 22 grupos; chaves únicas; `ByKey("frlg").Family == GameFamily.Frlg`; `ByKey("rb").Generation == 1`; `ByKey("b2w2").HasShinyCharm && !ByKey("bw").HasShinyCharm`; `!ByKey("lgpe").HasMasuda`; cada `Family` do `GameCatalog.All` aparece em exatamente um grupo; `PokeApiVersionGroup` únicos.
- [ ] **Step 2: Rodar** `dotnet test --project app/TradeRock.Tests --filter GameVersions` → FAIL (tipo não existe).
- [ ] **Step 3: Implementar** `GameVersions.cs` com os 22 grupos (Sinnoh para dp/pt/bdsp, Kanto para rb/y/frlg/lgpe, Johto para gs/c/hgss, Hisui para pla, Lumiose para za, Kalos para xy, Hoenn para rs/e/oras, Unova para bw/b2w2, Alola para sm/usum, Galar para swsh, Paldea para sv).
- [ ] **Step 4: Rodar** o teste → PASS.
- [ ] **Step 5: Teste TS** `gameVersions.test.ts`: 22 entradas, chaves iguais às do C# (lista literal no teste), `gameVersionByKey('sv').generation === 9`.
- [ ] **Step 6: Implementar** `gameVersions.ts`; rodar `npm test` → PASS.
- [ ] **Step 7: Commit** `feat: catálogo de todos os jogos (GameVersions)`.

### Task 2: Menu novo, rotas e páginas-esqueleto

**Files:**
- Modify: `frontend/src/components/Layout.tsx` (`mainLinks`), `frontend/src/App.tsx` (rotas), `frontend/src/locales/pt-BR.json`, `frontend/src/locales/en.json` (`nav.*`)
- Create: `frontend/src/pages/dex/DexPage.tsx`, `frontend/src/pages/guides/GuidesPage.tsx`, `frontend/src/pages/boxes/BoxesPage.tsx`, `frontend/src/pages/trades/TradesHubPage.tsx`, `frontend/src/pages/gifts/GiftsPage.tsx`, `frontend/src/pages/nuzlocke/NuzlockePage.tsx`, `frontend/src/pages/hunt/HuntPage.tsx` (esqueletos com `PageTitle` e um `Callout` "em construção" que as tarefas seguintes substituem)
- Modify: `frontend/src/pages/ActivitiesPage.tsx` (dividir: gifts → `GiftsPage`; raids e Union Circle → cartões dentro de `TradesHubPage`)
- Test: `frontend/src/components/layout.test.ts` (novo)

**Interfaces:**
- Rotas novas: `/` Início, `/pokedex`, `/pokedex/:species`, `/guias/*`, `/boxes`, `/trocas` (hub: cartão "Trocar" → `/trocas/trocar` com o `TradePage` atual; cartões de raids e Union Circle com os formulários de `pages/activities/`), `/mystery-gift`, `/nuzlocke`, `/hunt`, `/historico`, `/configuracoes`, `/sobre`.
- Redirects: `/trocar` → `/trocas/trocar`; `/pokemon` → `/boxes`; `/criar` e `/criar/:id` → continuam servindo o `BuilderPage` (o Boxes abre essa rota em "Editar"/"+ Novo"); `/atividades` → `/trocas`.
- `mainLinks` exportado de `Layout.tsx` para teste, na ordem: home, dex, guides, boxes, trades, gifts (localOnly), nuzlocke, hunt, history. Configurações e Sobre continuam no rodapé/"Mais" como hoje.
- `localOnly`: gifts; dentro de `/trocas` os cartões de raids e Union Circle só aparecem quando `!remote`.

- [ ] **Step 1: Teste** `layout.test.ts`: `mainLinks.map(l => l.to)` é exatamente `['/', '/pokedex', '/guias', '/boxes', '/trocas', '/mystery-gift', '/nuzlocke', '/hunt', '/historico']`; `nav.*` existe em pt-BR e en para cada `key`.
- [ ] **Step 2: Rodar** `npm test` → FAIL.
- [ ] **Step 3: Implementar** menu, rotas, redirects e esqueletos. Ícones Material Symbols: `home`, `menu_book`, `map`, `inventory_2`, `swap_horiz`, `redeem`, `skull`, `auto_awesome`, `history`.
- [ ] **Step 4: Rodar** `npm run typecheck && npm test` → PASS. Abrir `npm run dev` e conferir que `/trocar` redireciona e a troca abre igual.
- [ ] **Step 5: Commit** `feat: menu do RockAssistent com as novas áreas`.

### Task 3: Rebranding visível

**Files:**
- Modify: `frontend/src/locales/{pt-BR,en}.json` (`app.name`), `frontend/src/components/ui.tsx` (`Logo`), `frontend/index.html` (title), `app/TradeRock.App/Program.cs` (título da janela, `--version` imprime `RockAssistent {versão}`), `app/TradeRock.App/TradeRockServer.cs` (`MissingFrontendPage`), `app/TradeRock.App/Api/AppEndpoints.cs` (`app = "RockAssistent"`), `app/TradeRock.Core/Updates/Updates.cs` (User-Agent e nomes dos assets: aceitar `RockAssistent-Setup-*.exe`, `RockAssistent-*-x86_64.AppImage`, `traderock_*_amd64.deb` **e** os nomes antigos), `packaging/windows/traderock.iss` (`AppName`, `AppVerName`, `UninstallDisplayName`, `DefaultGroupName`, `OutputBaseFilename=RockAssistent-Setup-{#AppVersion}`, nomes das regras de firewall; **AppId e DefaultDirName não mudam**), `packaging/linux/io.github.traderock.TradeRock.desktop` (`Name=RockAssistent`, Comment), `scripts/package-linux.sh` (AppImage `RockAssistent-<ver>-x86_64.AppImage`; checagem de `--version`), `scripts/package-windows.sh`, `scripts/smoke-linux.sh`, `scripts/smoke-windows.ps1`, `packaging/release-notes.md`, `content/{pt-BR,en}/sobre.md`, `README.md` (título e primeiro parágrafo), `.github/workflows/ci.yml` (nomes dos artefatos)
- Create: `packaging/linux/io.github.traderock.TradeRock.svg` (substituir o desenho: ícone novo em SVG no estilo das ilustrações, uma Poké Ball estilizada com um "R"), `packaging/windows/traderock.ico` regenerado por `make-icon.py` a partir do SVG novo
- Test: `app/TradeRock.Tests/UpdatesTests.cs` (existente ou novo caso): o parser de assets aceita os dois conjuntos de nomes; `frontend/src/pages/about.test.ts` ajustado.

- [ ] **Step 1: Teste** no `UpdatesTests`: dado um Release com `RockAssistent-Setup-1.2.3.exe`, `traderock_1.2.3_amd64.deb`, `RockAssistent-1.2.3-x86_64.AppImage` e `SHA256SUMS`, o seletor de asset escolhe o certo por plataforma; com os nomes antigos também.
- [ ] **Step 2: Rodar** → FAIL.
- [ ] **Step 3: Implementar** todas as trocas de nome listadas. Grep final: `grep -rn "TradeRock" frontend/src/locales packaging scripts/package-*.sh README.md content/*/sobre.md` só pode restar em URLs do GitHub, nomes de arquivo/pacote (`traderock`) e no texto "antes chamado TradeRock" do Sobre.
- [ ] **Step 4: Rodar** `dotnet test --project app/TradeRock.Tests` e `npm test` → PASS. `python3 packaging/windows/make-icon.py` gera o `.ico`.
- [ ] **Step 5: Commit** `feat: nome visível RockAssistent`.

## Entrega B: Pokédex (dados da PokeAPI)

### Task 4: `scripts/build-dex.py` gera `assets/data/dex.sqlite`

**Files:**
- Create: `scripts/build-dex.py`, `scripts/tests/test_build_dex.py`
- Modify: `scripts/ci.sh` (passo 0: `python3 scripts/build-dex.py`), `scripts/package-linux.sh`, `scripts/package-windows.sh` (chamam o build-dex antes do `dotnet publish`), `.gitignore` (`assets/data/dex.sqlite`), `app/TradeRock.Core/TradeRock.Core.csproj` (EmbeddedResource `..\..\assets\data\dex.sqlite` LogicalName `data/dex.sqlite`, com `<Error Condition="!Exists(...)" Text="Rode scripts/build-dex.py" />`), `.github/workflows/ci.yml` (python + cache de `~/.cache/traderock/pokeapi`)

**Interfaces:**
- `COMMIT = "6ea97b5641faa12e090866e8144ae8f57764fc64"` (PokeAPI/pokeapi, 2026-11-05). CSVs baixados para `~/.cache/traderock/pokeapi/<COMMIT>/` (reuso se existirem).
- Tabelas do SQLite (nomes e colunas fixos, usadas pela Task 5): `languages(id, identifier)`; `species(id, identifier, generation_id, color, capture_rate, base_happiness, hatch_counter, gender_rate, growth_rate, evolution_chain_id, evolves_from_species_id, genus_en, genus_ptbr)`; `species_names(species_id, language, name)`; `pokemon(id, species_id, identifier, height, weight, base_experience, is_default, form_order)`; `pokemon_forms(id, pokemon_id, identifier, form_name_en, is_mega, is_battle_only, introduced_in_version_group)`; `pokemon_types(pokemon_id, slot, type_id)`; `types(id, identifier, generation_id)`; `type_efficacy(damage_type_id, target_type_id, damage_factor)`; `pokemon_stats(pokemon_id, stat_id, base_stat, effort)`; `abilities(id, identifier, name_en, effect_en, short_effect_en, generation_id)`; `pokemon_abilities(pokemon_id, ability_id, is_hidden, slot)`; `pokemon_abilities_past(pokemon_id, generation_id, ability_id, is_hidden, slot)`; `moves(id, identifier, name_en, type_id, power, pp, accuracy, priority, damage_class, effect_chance, effect_en, generation_id)`; `move_changelog(move_id, changed_in_version_group_id, type_id, power, pp, accuracy)`; `pokemon_moves(pokemon_id, version_group_id, move_id, method, level, order)`; `machines(version_group_id, item_id, move_id, machine_number)`; `items(id, identifier, name_en, category, cost, effect_en, fling_power)`; `evolution(evolved_species_id, evolution_trigger, trigger_item_id, minimum_level, gender_id, location_id, held_item_id, time_of_day, known_move_id, known_move_type_id, minimum_happiness, minimum_beauty, minimum_affection, relative_physical_stats, party_species_id, party_type_id, trade_species_id, needs_overworld_rain, turn_upside_down)`; `flavor_text(species_id, version_id, language, text)`; `egg_groups(id, identifier, name_en)`; `pokemon_egg_groups(species_id, egg_group_id)`; `pokedexes(id, identifier, region, name_en)`; `pokemon_dex_numbers(species_id, pokedex_id, number)`; `version_groups(id, identifier, generation_id, order)`; `versions(id, version_group_id, identifier, name_en)`; `locations(id, region_id, identifier, name_en)`; `location_areas(id, location_id, identifier, name_en)`; `location_game_indices(location_id, generation_id, game_index)`; `encounters(id, version_id, location_area_id, pokemon_id, min_level, max_level, method, rarity, condition_values)`; `natures(id, identifier, name_en, decreased_stat_id, increased_stat_id)`; `stats(id, identifier, name_en)`; `experience(growth_rate_id, level, experience)`; `meta(key, value)` com `pokeapi_commit` e `built_at`.
- Línguas mantidas: names em `en, ja-Hrkt, ja, fr, de, es, it, ko, pt-BR (se existir no CSV)`; flavor_text em `en` e `pt-BR` (se existir); prose só `en`. Tudo o mais descartado. `VACUUM` no fim. Tamanho esperado: 10–25 MB.

- [ ] **Step 1: Teste** `test_build_dex.py` (unittest, roda com `python3 -m unittest discover -s scripts/tests -t scripts`): gera o banco a partir de um diretório de CSVs mínimos fixos no teste (Gengar + Gastly + Haunter, 2 golpes, 1 encontro) e verifica `species_names` tem `en` e `fr` para Gengar, `evolution` de Gengar tem trigger `trade`, `encounters` tem 1 linha com `method='walk'`, `meta.pokeapi_commit` = COMMIT.
- [ ] **Step 2: Rodar** → FAIL.
- [ ] **Step 3: Implementar** `build-dex.py` (stdlib: csv, sqlite3, urllib). Funções: `download(commit, cache_dir) -> Path`, `build(csv_dir: Path, out: Path, commit: str)`, `main()`. Os métodos de encontro e condições viram texto (`identifier` de `encounter_methods.csv`; `condition_values` como `"time-night,season-winter"`).
- [ ] **Step 4: Rodar** o teste → PASS; rodar o script de verdade; `ls -la assets/data/dex.sqlite` e `sqlite3 assets/data/dex.sqlite "select count(*) from species"` → 1025.
- [ ] **Step 5: Integrar** em `ci.sh`, `package-*.sh`, csproj, `.gitignore`, workflow. `dotnet build` passa.
- [ ] **Step 6: Commit** `feat: dex.sqlite gerado da PokeAPI (scripts/build-dex.py)`.

### Task 5: `DexService` e `/api/dex`

**Files:**
- Create: `app/TradeRock.Core/Dex/DexDatabase.cs` (copia o recurso embutido para `<DataDir>/dex/<sha8>.sqlite` se não existir, abre `SqliteConnection` somente leitura `Mode=ReadOnly`; apaga versões antigas), `app/TradeRock.Core/Dex/DexService.cs`, `app/TradeRock.Core/Dex/DexModels.cs`, `app/TradeRock.App/Api/DexEndpoints.cs`
- Modify: `app/TradeRock.App/ServiceSetup.cs` (singletons), `app/TradeRock.App/TradeRockServer.cs` (`MapDex`)
- Test: `app/TradeRock.Tests/DexServiceTests.cs` (usa o `dex.sqlite` real embutido; se não existir, `Skip`)

**Interfaces:**
- `DexService(DexDatabase db)`:
  ```csharp
  Task<IReadOnlyList<SpeciesSummary>> ListAsync(DexQuery q, CancellationToken ct);   // SpeciesSummary(int Id, string Name, int Generation, string[] Types, int? NationalNumber)
  Task<SpeciesDetail?> GetAsync(int speciesId, string gameKey, string lang, CancellationToken ct); // null se a espécie não existe; SpeciesDetail.ExistsInGame=false quando não há pokemon_moves nem dex number nem encounter para o grupo
  Task<MoveDetail?> GetMoveAsync(int id, string gameKey, CancellationToken ct);
  Task<AbilityDetail?> GetAbilityAsync(int id, CancellationToken ct);
  Task<ItemDetail?> GetItemAsync(int id, CancellationToken ct);
  Task<TypeChart> GetTypeChartAsync(int generation, CancellationToken ct);
  Task<IReadOnlyList<GameAvailability>> AvailabilityAsync(int speciesId, CancellationToken ct); // por grupo: Encounter | Evolution | Egg | Gift | None (gift vem dos nossos dados na Task 8; aqui None/Encounter/Evolution/Egg)
  ```
- `DexQuery(string? Text, string? Type, int? Generation, string? GameKey, int Offset = 0, int Limit = 100)`.
- `SpeciesDetail`: `Id, Names (dict lang→name), Genus, Types, Height, Weight, GenderRate, CaptureRate, HatchSteps, BaseHappiness, GrowthRate, Color, EggGroups, Abilities[] (Name, IsHidden, Effect), EvolutionChain (lista de etapas com condição legível), DamageTaken (18 multiplicadores da geração do jogo), FlavorTexts (por versão do grupo, texto e língua), Encounters[] (versão, local, área, método, níveis, chance, condições), Moves { LevelUp[], Machine[], Tutor[], Egg[] } (Move: Id, Name, Type, Class, Power, Accuracy, Pp, Level/MachineNumber), Stats (6 base + total + EVs cedidos), Forms[] (Id, Name, Types, Stats, IsMega), DexNumbers (dict dex→n), `ExistsInGame`.
- Endpoints: `GET /api/dex/games` (GameVersions), `GET /api/dex/species?text&type&generation&game&offset&limit`, `GET /api/dex/species/{id}?game=frlg&lang=pt-BR`, `GET /api/dex/species/{id}/availability`, `GET /api/dex/moves/{id}?game`, `GET /api/dex/abilities/{id}`, `GET /api/dex/items/{id}`, `GET /api/dex/types?generation=9`. Todos públicos no modo servidor (leitura).

- [ ] **Step 1: Testes** `DexServiceTests`: Gengar (94) em `frlg` tem tipos `ghost, poison`, habilidade Levitate, cadeia Gastly→Haunter(level 25)→Gengar(trade), `DamageTaken["ground"]==2`, `Moves.LevelUp` contém Shadow Punch no nível 25, `ExistsInGame` true; Gengar em `lgpe` tem `ExistsInGame` true; Sprigatito (906) em `frlg` → `ExistsInGame == false` e listas vazias; `ListAsync(new("geng", null, null, null))` devolve Gengar; `AvailabilityAsync(94)` marca `frlg` como Evolution.
- [ ] **Step 2: Rodar** → FAIL.
- [ ] **Step 3: Implementar** `DexDatabase`, `DexService` (SQL direto com `SqliteCommand`; sem EF), DTOs e endpoints; cache em memória dos `GameVersions` ↔ ids de version_group/versions.
- [ ] **Step 4: Rodar** → PASS.
- [ ] **Step 5: Commit** `feat: serviço e API da Pokédex`.

### Task 6: Frontend da Pokédex (lista e página em abas)

**Files:**
- Create: `frontend/src/lib/dex.ts` (tipos espelho dos DTOs + `useSpecies(id, game)`), `frontend/src/pages/dex/DexPage.tsx` (lista: busca, filtros tipo/geração/jogo, grade de cartões com sprite), `frontend/src/pages/dex/SpeciesPage.tsx` (cabeçalho + seletor de jogo + abas), `frontend/src/pages/dex/tabs/{GeneralTab,LocationsTab,MovesTab,StatsTab,EggTab,SavesTab}.tsx`, `frontend/src/pages/dex/GameSelector.tsx`, `frontend/src/pages/dex/TypeBadge.tsx`, `frontend/src/pages/dex/DamageTakenGrid.tsx`, `frontend/src/pages/dex/EvolutionChain.tsx`
- Modify: locales (`dex.*`), `App.tsx` (rotas `/pokedex`, `/pokedex/:id`)
- Test: `frontend/src/pages/dex/dex.test.ts` (funções puras: formatação de condição de evolução, cor do multiplicador, ordenação de golpes)

**Interfaces:**
- Seletor de jogo: lista `GAME_VERSIONS` em ordem; jogos onde `availability` é `None` aparecem apagados mas selecionáveis; jogo escolhido guardado em `localStorage traderock.dex.game` (padrão `frlg`).
- Aba "Nos seus saves" consome `GET /api/boxes/where/{species}` (Task 16); até lá mostra "em breve" e não quebra.
- Tipos traduzidos via `dex.types.<identifier>`; categorias de golpe `dex.class.<physical|special|status>`; textos de Pokédex em pt-BR quando `lang==='pt-BR'` e houver, senão en com `Badge` "em inglês".
- Sprites: `spriteUrl(species)`; shiny e por jogo ficam para a Task 7b (fallback para o sprite base).

- [ ] **Step 1: Testes** das funções puras em `dex.test.ts`.
- [ ] **Step 2: Rodar** → FAIL.
- [ ] **Step 3: Implementar** páginas e abas com os componentes de `ui.tsx`; grade de dano com 18 células coloridas (`bg-bad-soft` ×2/×4, `bg-ok-soft` ×½/×¼, `bg-tile` ×0).
- [ ] **Step 4: Rodar** `npm run typecheck && npm test` → PASS; conferir no `npm run dev` Gengar em FRLG e em SV e Sprigatito em FRLG.
- [ ] **Step 5: Commit** `feat: Pokédex com página por jogo em abas`.

### Task 7: Páginas de golpe, habilidade, item e tabela de tipos; sprites shiny

**Files:**
- Create: `frontend/src/pages/dex/{MovePage,AbilityPage,ItemPage,TypeChartPage}.tsx`, `scripts/fetch-sprites.sh` (ampliar: baixa também `sprites/pokemon/shiny/<n>.png` para `assets/sprites/shiny/<n>.png`, mesmo COMMIT), `app/TradeRock.App/Api/AppEndpoints.cs` (`GET /api/sprites/{species}?shiny=1`, fallback para o normal quando o arquivo não existe)
- Modify: `App.tsx` (rotas `/pokedex/golpe/:id`, `/pokedex/habilidade/:id`, `/pokedex/item/:id`, `/pokedex/tipos`), `TradeRock.App.csproj` (embutir `assets/sprites/shiny/*.png` como `sprites/shiny/<n>.png`), `assets/sprites/SHA256SUMS`
- Test: `app/TradeRock.Tests/SpritesTests.cs` (shiny existe para 94; `?shiny=1` de um n sem arquivo devolve o normal)

- [ ] **Step 1: Teste** → FAIL. **Step 2: Implementar.** **Step 3: Rodar** `scripts/fetch-sprites.sh --shiny` e testes → PASS. **Step 4: Commit** `feat: páginas de golpe, habilidade, item e tipos; sprites shiny`.

## Entrega C: dados nossos (JSON) e verificação

Todos os JSON desta entrega ficam em `assets/data/` e são embutidos no `TradeRock.Core` (`LogicalName data/<caminho>`). Cada arquivo tem `"source"` (lista de URLs/decompilações consultadas) e `"verified": true|false` por item. O **analista de dados (Fable 5.1)** revisa cada tarefa de dados com o prompt `docs/superpowers/plans/data-review-prompt.md` (Task 8 cria) e escreve as dúvidas em `docs/superpowers/data-doubts.md`.

### Task 8: Esquemas, validador, endpoints `/api/data` e `shiny-methods.json`

**Files:**
- Create: `app/TradeRock.Core/GameData/GameDataModels.cs`, `app/TradeRock.Core/GameData/GameDataStore.cs`, `app/TradeRock.App/Api/GameDataEndpoints.cs`, `assets/data/shiny-methods.json`, `docs/superpowers/plans/data-review-prompt.md`, `docs/superpowers/data-doubts.md` (cabeçalho vazio)
- Modify: `TradeRock.Core.csproj` (EmbeddedResource `..\..\assets\data\**\*.json` LogicalName `data/%(RecursiveDir)%(Filename)%(Extension)`), `ServiceSetup.cs`, `TradeRockServer.cs`
- Test: `app/TradeRock.Tests/GameDataTests.cs`

**Interfaces (modelos C#, JSON em snake_case):**
```csharp
public sealed record ShinyMethod(string Id, string Name, string Description, double BaseOdds /* 1/BaseOdds */, IReadOnlyList<ShinyModifier> Modifiers, IReadOnlyList<string> Games /* keys de GameVersions */, string? Notes, bool Verified);
public sealed record ShinyModifier(string Id, string Name, string Kind /* "toggle" | "steps" */, IReadOnlyList<ShinyStep>? Steps, double? Odds /* para toggle: odds resultante 1/Odds */);
public sealed record ShinyStep(string Label, int Min, int? Max, double Odds);
public sealed record BossBattle(string Id, string Kind /* gym|rival|evil|elite|champion|other */, string Name, string? Title, string? Location, int Order, string? Badge, IReadOnlyList<BossPokemon> Party, int LevelCap /* maior nível do time */, bool Verified, string? Notes);
public sealed record BossPokemon(int Species, int Form, int Level, IReadOnlyList<string> Moves /* identifiers PokeAPI */, string? Ability, string? Item, string? TeraType);
public sealed record TrainerBattle(string Id, string Area /* id de áreas */, string Class, string Name, IReadOnlyList<BossPokemon> Party, bool Verified);
public sealed record GameArea(string Id, string Name, string Kind /* route|city|dungeon|building|sea */, int Order, string? MapNode, int? PokeApiLocationId, IReadOnlyList<string> BossesBefore /* ids de chefes enfrentados ao chegar aqui */);
public sealed record OwnEncounter(string Area, int Species, int Form, string Method, int MinLevel, int MaxLevel, string? Chance /* "common|uncommon|rare|fixed" */, string? Conditions, IReadOnlyList<string> Versions, bool Alpha, bool Verified);
public sealed record MapGraph(string Region, IReadOnlyList<MapNode> Nodes, IReadOnlyList<MapEdge> Edges);
public sealed record MapNode(string Id, string Name, string Kind, double X, double Y, IReadOnlyList<string>? Games /* null = todos os jogos da região */);
public sealed record MapEdge(string From, string To);
```
- `GameDataStore` (singleton, carrega tudo no construtor): `IReadOnlyList<ShinyMethod> ShinyMethods`; `IReadOnlyList<BossBattle> Bosses(string gameKey)`; `IReadOnlyList<TrainerBattle> Trainers(string gameKey)`; `IReadOnlyList<GameArea> Areas(string gameKey)`; `IReadOnlyList<OwnEncounter> Encounters(string gameKey)`; `MapGraph? Map(Region)`; `IReadOnlyList<string> GamesWithTrainers`.
- Endpoints: `GET /api/data/shiny-methods?game=`, `GET /api/data/{game}/bosses`, `GET /api/data/{game}/trainers`, `GET /api/data/{game}/areas`, `GET /api/data/{game}/encounters`, `GET /api/data/maps/{region}`.
- Validador (no construtor e nos testes): toda espécie existe (1..1025), golpes existem no `moves` do dex (via `DexService`), `Area` de treinadores/encontros existe em `areas`, `MapNode` existe no mapa da região, `BossesBefore` existem, `Games` dos métodos existem em `GameVersions`.
- `shiny-methods.json`: cobertura mínima listada na spec 4.7, com odds: Gen 2–5 base 1/8192, Gen 6+ 1/4096; Charm ×3 rolls (1/2731 → 1/1365 na Gen 6+); Masuda 5 rolls (1/1639 Gen 4, 6 rolls 1/1366 Gen 5, 1/683 Gen 6+), Masuda+Charm 8 rolls (1/1024 Gen 5, 1/512 Gen 6+); Crystal Odd Egg 14% (JP 50%), breeding com shiny Gen 2 1/64; Poké Radar chain 40 = 1/200 (DPPt/XY) e 1/99 (BDSP); SOS chain 31+ 1/315 (1/273 com Charm); Chain fishing 20+ 1/100; DexNav níveis; LGPE catch combo 31+ com Lure e Charm 1/273; SwSh número de batalhas 500+ (1/1365; brilliant aura) e Dynamax Adventure 1/300 (1/100 com Charm); PLA nível de pesquisa 10 (1/2048,25), perfeito (1/1024,38), massive outbreak (+25 rolls), outbreak (+12), Charm (+3); SV sandwich níveis 1–3 (+1/+2/+3 rolls), outbreak 30+ (+1), 60+ (+2); BDSP Grand Underground sem bônus; Z-A: pesquisar. Esses números são o ponto de partida; o implementador **confere na web** e corrige, marcando `verified`.

- [ ] **Step 1: Testes** `GameDataTests`: store carrega sem exceção; `ShinyMethods` tem `masuda` com games incluindo `sv` e um modificador `shiny_charm`; cada arquivo em `data/` desserializa; o validador rejeita um JSON com espécie 2000 (teste com fixture inline).
- [ ] **Step 2: Rodar** → FAIL. **Step 3: Implementar** modelos, store, validador, endpoints e o JSON. Escrever `data-review-prompt.md` (revisor de dados: confere cada item contra pelo menos uma fonte na web, lista itens errados com correção e fonte, itens não confirmáveis para `data-doubts.md`; nunca copia texto).
- [ ] **Step 4: Rodar** → PASS. **Step 5: Commit** `feat: dados do jogo embutidos (esquemas, validador, métodos de shiny)`.

### Task 9: Chefes dos jogos Gen 1–3 (`assets/data/bosses/{rb,y,gs,c,rs,e,frlg}.json`)

**Files:** Create os 7 arquivos. Test: `GameDataTests` ganha um caso por jogo: `Bosses(key)` tem 8 líderes (`kind=gym`, `badge` preenchido), 4 `elite`, 1 `champion`, rivais em ordem, e `LevelCap` crescente por `Order` entre os ginásios.

- [ ] **Step 1: Teste** → FAIL. **Step 2: Pesquisar** (decompilações pret `pokered`, `pokecrystal`, `pokeruby`, `pokeemerald`, `pokefirered`: `data/trainers/parties.asm` ou `src/data/trainer_parties.h`; comparar com uma segunda fonte) e **escrever** os JSON com todos os chefes, inclusive rematches do pós-jogo quando existirem (`Order` maior). Golpes dos times "default moves" derivados do learnset no nível (anotar `notes: "golpes padrão do nível"`). **Step 3: Rodar** → PASS. **Step 4: Commit** `data: chefes da Gen 1 à 3`.

### Task 10: Chefes dos jogos Gen 4–5 (`dp, pt, hgss, bw, b2w2`)

Igual à Task 9 (fontes: pret `pokediamond`, `pokeplatinum`, `pokeheartgold`, Bulbapedia/Serebii para conferir). Commit `data: chefes da Gen 4 e 5`.

### Task 11: Chefes dos jogos Gen 6–9 (`xy, oras, sm, usum, lgpe, swsh, bdsp, pla, sv, za`)

Igual à Task 9. Para o Z-A, a fonte principal é `docs/superpowers/sources/za-pokemythology-notes.md` (times e níveis de todos os chefes; golpes ficam `verified: false`). Alola usa `kind=gym` para os Trials/Kahunas com `title` ("Captain"/"Kahuna"); PLA usa `kind=other` para Nobles e Wardens; SV tem três rotas (`title` "Victory Road"/"Path of Legends"/"Starfall Street") com `Order` seguindo o nível recomendado. Commit `data: chefes da Gen 6 à 9`.

### Task 12: Áreas e mapas, parte 1 (Kanto, Johto, Hoenn, Sinnoh)

**Files:** Create `assets/data/areas/{rb,y,gs,c,rs,e,frlg,dp,pt,hgss,lgpe,bdsp}.json` e `assets/data/maps/{kanto,johto,hoenn,sinnoh}.json`.

- Áreas em ordem de progressão normal do jogo, com `BossesBefore` (ids da Task 9–11), `PokeApiLocationId` (procurar em `locations` do dex por identifier, ex.: `kanto-route-1`), `MapNode`.
- Mapas: coordenadas `X,Y` em 0–100 num grid esquemático (não precisa ser fiel: só vizinhança e direção aproximada). Nós com `Games` quando só existem em alguns jogos (Sevii Islands só `frlg`; Safari Zone Johto só `hgss`).
- Test: para cada jogo, toda área com `PokeApiLocationId` aponta para um location existente no dex; todo `MapNode` existe; a primeira área é a cidade inicial; nenhum `BossesBefore` cita chefe inexistente.

- [ ] Commit `data: áreas e mapas de Kanto, Johto, Hoenn e Sinnoh`.

### Task 13: Áreas e mapas, parte 2 (Unova, Kalos, Alola, Galar, Hisui, Paldea, Lumiose) + encontros dos jogos de Switch

**Files:** Create `assets/data/areas/{bw,b2w2,xy,oras,sm,usum,swsh,pla,sv,za}.json`, `assets/data/maps/{unova,kalos,alola,galar,hisui,paldea,lumiose}.json`, `assets/data/encounters/{bdsp,pla,sv,za}.json` (os jogos sem encontros na PokeAPI; SwSh tem).

- Encontros: por área, espécie, método (`walk|surf|fish|tree|outbreak|raid|fixed|gift|trade|city`), faixa de nível, chance qualitativa, `Versions` (`scarlet`/`violet` etc.), `Conditions` (`day`/`night`), `Alpha` (bool). Pesquisar na web; marcar `verified`. Para o Z-A, as 20 Wild Zones, esgotos, Lysandre Labs, raros por setor e lendários estão em `docs/superpowers/sources/za-pokemythology-notes.md`.
- Test: igual à Task 12 mais: `Encounters("sv")` não vazio, toda área citada existe, toda espécie existe.

- [ ] Commit `data: áreas e mapas das outras regiões; encontros de BDSP, PLA, SV e Z-A`.

### Task 14: Treinadores comuns dos jogos de GBA e DS

**Files:** Create `assets/data/trainers/{rs,e,frlg,dp,pt,hgss,bw,b2w2}.json` (e `rb,y,gs,c` se as decompilações permitirem no mesmo esforço; senão deixar para `data-doubts.md`).

- Fonte principal: decompilações pret (`trainer_parties.h`, `trainers.h`, e os scripts de mapa para saber em que área cada treinador fica). Golpes `NoItemDefaultMoves` = learnset no nível (anotar). Áreas pelos ids da Task 12/13.
- Test: para cada jogo, >= 150 treinadores; toda `Area` existe; toda espécie existe; `GamesWithTrainers` lista esses jogos.

- [ ] Commit `data: treinadores comuns dos jogos de GBA e DS`.

## Entrega D: Boxes

### Task 15: Leitura e escrita de saves de todas as gerações (`SaveFileAccess`)

**Files:**
- Create: `app/TradeRock.Core/Saves/SaveFileAccess.cs`
- Modify: `app/TradeRock.Core/Saves/SaveInjection.cs` (passa a delegar o `Open` ao novo tipo; o comportamento Gen 3 atual não muda), `app/TradeRock.Core/Builder/SaveImport.cs` (lê qualquer geração que o PKHeX abrir)
- Test: `app/TradeRock.Tests/SaveFileAccessTests.cs`

**Interfaces:**
```csharp
public sealed record SaveInfo(string Path, string GameKey /* GameVersions key */, GameVersion Version, string TrainerName, uint TrainerId, int Generation, int BoxCount, int SlotsPerBox, int PartyCount, DateTimeOffset ModifiedAt, string Sha256, bool Writable /* sav.State.Exportable */);
public sealed record SaveSlotEntry(int Box /* -1 = time */, int Slot, byte[] Data /* formato guardado (decrypted) */, PokemonSummary Summary);
public static class SaveFileAccess {
    SaveFile Open(byte[] bytes);               // SaveUtil.GetVariantSAV; lança SaveInjectionException("sav_unknown") se null
    SaveInfo Describe(SaveFile sav, string path, DateTimeOffset modifiedAt, string sha);
    IReadOnlyList<SaveSlotEntry> ReadAll(SaveFile sav);  // caixas + time
    void WriteSlot(SaveFile sav, int box, int slot, PKM pk); // valida pk.Context == sav.Context (senão SaveInjectionException("sav_wrong_game")); box -1 = time (sav.SetPartySlotAtIndex)
    void ClearSlot(SaveFile sav, int box, int slot);
    byte[] Export(SaveFile sav);  // sav.Write() -> bytes no formato original (com footer/header quando o arquivo tinha)
    string GameKeyOf(SaveFile sav); // mapeia sav.Version para GameVersions (ex.: GameVersion.FR → "frlg", GameVersion.SW → "swsh")
}
```
- Consultar o PKHeX.Core: `SaveUtil.GetVariantSAV(Memory<byte>)`, `SaveFile.BoxCount`, `BoxSlotCount`, `GetBoxSlotAtIndex`/`SetBoxSlotAtIndex`, `GetPartySlotAtIndex`, `PartyCount`, `Write()`, `State.Exportable`, `Metadata`, `Version`, `OT`, `TID16`/`DisplayTID`. Não inventar nomes: abrir o `.dll`/fonte do pacote.

- [ ] **Step 1: Testes** com saves em branco do PKHeX (`SaveUtil.GetBlankSAV(GameVersion.FR, "ROCHA")`, `GameVersion.SW`, `GameVersion.C`): `Describe` dá `GameKey` esperado; `WriteSlot` de um `Blank()` de Gengar e `ReadAll` devolve na posição; `WriteSlot` de um pk8 num SAV3 lança `sav_wrong_game`; `Export` → `Open` → mesmo conteúdo; um array de 10 bytes lança `sav_unknown`.
- [ ] **Step 2: Rodar** → FAIL. **Step 3: Implementar.** **Step 4: Rodar** todos os testes (inclusive os antigos de SaveInjection) → PASS. **Step 5: Commit** `feat: leitura e escrita de saves de todas as gerações`.

### Task 16: Pastas de saves, scanner e API `/api/boxes`

**Files:**
- Create: `app/TradeRock.Core/Saves/SaveFolders.cs` (setting `saves.folders`: lista de `SaveFolder(string Path, string Pattern /* glob, padrão "**/*.sav;**/*.dsv;**/*.sa1;**/main;**/*.bin" */)`), `app/TradeRock.Core/Saves/SaveLibrary.cs` (varre as pastas; cache por caminho+mtime+tamanho; lê com `SaveFileAccess`; entradas com erro viram `SaveLibraryEntry.Error`), `app/TradeRock.Core/Storage/Entities.cs` (+ `SaveAttachment : IOwned { Guid Id; Guid OwnerId; Guid PokemonId; string SavePath; string SaveSha256; int Box; int Slot; DateTimeOffset AttachedAt; }`), migração `SaveAttachments`, `app/TradeRock.App/Api/BoxesEndpoints.cs`
- Modify: `TradeRockDb.cs` (DbSet + filtro de dono), `ServiceSetup.cs`, `TradeRockServer.cs`, `Server/Caller.cs` (`/api/boxes/saves` local-only), `SettingsEndpoints.cs` (GET/PUT `/api/settings/save-folders`)
- Test: `app/TradeRock.Tests/SaveLibraryTests.cs`, `BoxesEndpointsTests.cs`

**Interfaces:**
- `SaveLibrary(SaveFolders, SaveBackups, TimeProvider)`:
  ```csharp
  Task<IReadOnlyList<SaveLibraryEntry>> ListAsync(bool refresh, CancellationToken ct); // SaveLibraryEntry(string Id /* sha8 do caminho */, SaveInfo? Info, string? Error)
  Task<SaveBoxes> ReadAsync(string id, CancellationToken ct);  // SaveBoxes(SaveInfo Info, IReadOnlyList<SaveSlotEntry> Slots)
  Task<SaveWriteResult> ApplyAsync(string id, IReadOnlyList<SaveChange> changes, CancellationToken ct); // SaveChange(int Box, int Slot, byte[]? Data /* null = limpar */); faz backup (SaveBackups.Create) antes, grava com File.Replace, relê e confere; SaveWriteResult(string BackupPath, string NewSha256)
  ```
- Endpoints (local-only): `GET /api/boxes/saves?refresh=1`, `GET /api/boxes/saves/{id}`, `POST /api/boxes/saves/{id}/apply {changes}`, `GET /api/boxes/saves/{id}/slot/{box}/{slot}/file` (download .pk*), `POST /api/boxes/move { from: {kind: "save"|"vault", saveId?, box, slot?, pokemonId?}, to: {kind, saveId?, box?, slot?, boxId?}, mode: "move"|"copy"|"attach" }` (regras: vault→save grava o arquivo convertido pelo `HomeTransfer` quando a família difere, com aviso de legalidade; save→vault importa com `Origin=Imported`; `attach` = copy + `SaveAttachment`; move = copy + limpar a origem), `GET /api/boxes/where/{species}` (lista de `{saveId|vault, name, box, slot, level, shiny}`), `POST /api/boxes/sync-attachments` (relê os saves e substitui os anexados cujo slot mudou).
- Pokémon de save não existem no cofre: são identificados por `(saveId, box, slot)`.

- [ ] **Step 1: Testes**: pasta temporária com um SAV3 em branco gravado + um arquivo lixo `x.sav` de 10 bytes → `ListAsync` devolve 2 entradas, uma com `Error`; `ApplyAsync` grava um Gengar na caixa 0 slot 0, cria backup em `SaveBackups.Directory`, e `ReadAsync` o devolve; mover vault→save via endpoint com um `pk8` para um SAV3 falha com `save_wrong_game`.
- [ ] **Step 2–5:** FAIL → implementar (migração `dotnet ef migrations add SaveAttachments --project app/TradeRock.Core --output-dir Storage/Migrations`) → PASS → Commit `feat: saves por pasta e movimentação entre saves e cofre`.

### Task 17: Frontend Boxes

**Files:**
- Create: `frontend/src/pages/boxes/BoxesPage.tsx` (abas `Segmented` "Saves" | "Cofre do app"), `frontend/src/pages/boxes/SavesTab.tsx` (lista de saves à esquerda, caixas no meio, painel à direita; reaproveita `pages/vault/BoxGrid.tsx`, `Details.tsx`, `dnd.ts`), `frontend/src/pages/boxes/VaultTab.tsx` (o `VaultPage` atual movido/embrulhado), `frontend/src/pages/boxes/MoveDialog.tsx` (Mover/Copiar/Anexar com destino), `frontend/src/pages/settings/SaveFoldersCard.tsx` (em Configurações: adicionar pasta pelo diálogo nativo `POST /api/saves/open-folder` — criar esse endpoint local-only em `SaveEndpoints.cs` usando o diálogo de pasta do Photino se existir; se o Photino não tiver diálogo de pasta, campo de texto)
- Modify: `pages/vault/Details.tsx` (ações: Editar → `/criar/:id`, Evoluir por troca (Task 18), Clonar, Exportar, Ver na Pokédex → `/pokedex/:species`, Trocar → `/trocas/trocar?pokemon=id`, Mover), `BoxGrid.tsx` (botão "+ Novo" em slot vazio → `/criar?box=&slot=`), `BuilderPage.tsx` (ao salvar com `box`/`slot` na query, move para lá), locales `boxes.*`
- Test: `frontend/src/pages/boxes/boxes.test.ts` (redutor de seleção/drag: origem save → destino cofre monta o corpo certo do `/api/boxes/move`)

- [ ] Steps TDD como nas anteriores; conferir no `npm run dev` arrastar entre um save e o cofre. Commit `feat: tela Boxes com saves e cofre`.

### Task 18: Evoluir por troca (local) e Clonar

**Files:**
- Create: `app/TradeRock.Core/Builder/TradeEvolution.cs`
- Modify: `VaultEndpoints.cs` (`POST /api/vault/pokemon/{id}/evolve`, `POST /api/vault/pokemon/{id}/clone`), `BoxesEndpoints.cs` (`POST /api/boxes/saves/{id}/slot/{box}/{slot}/evolve`)
- Test: `app/TradeRock.Tests/TradeEvolutionTests.cs`

**Interfaces:**
```csharp
public static class TradeEvolution {
    IReadOnlyList<(ushort Species, byte Form, int? RequiredItem)> Options(PKM pk); // só evoluções por troca (EvolutionTree do PKHeX: EvolutionType.Trade, TradeHeldItem, TradeSpecies)
    PKM Apply(PKM pk, ushort species, byte form); // muda espécie/forma, consome o item quando a evolução exige, recalcula stats (pk.ResetPartyStats / RefreshChecksum), mantém OT/PID
}
```
- Consultar `EvolutionTree.Evolves(...)`/`Forward` no PKHeX.Core para listar; não inventar.

- [ ] Testes: Haunter pk3 → `Options` = Gengar; `Apply` dá Gengar legal (`LegalityReporter.Analyze` ≠ Illegal); Pikachu → `Options` vazio; Kadabra pk8 → Alakazam. Commit `feat: evoluir por troca e clonar no Boxes`.

## Entrega E: Contador Hunt

### Task 19: Backend das hunts

**Files:**
- Modify: `Entities.cs` (+ `ShinyHunt : IOwned { Guid Id; Guid OwnerId; int Species; int Form; string GameKey; string MethodId; string ModifiersJson; int Count; long ElapsedSeconds; DateTimeOffset StartedAt; DateTimeOffset? EndedAt; bool Found; string? Notes; }`), `TradeRockDb.cs`, migração `ShinyHunts`
- Create: `app/TradeRock.Core/Hunts/ShinyOdds.cs`, `app/TradeRock.Core/Hunts/HuntStore.cs`, `app/TradeRock.App/Api/HuntEndpoints.cs`
- Test: `ShinyOddsTests.cs`, `HuntStoreTests.cs`

**Interfaces:**
```csharp
public static class ShinyOdds {
    double Current(ShinyMethod m, IReadOnlyDictionary<string, string> modifiers, int count); // devolve N de "1/N"
    double Cumulative(double oddsN, int count); // 1 - (1 - 1/N)^count
}
```
- Endpoints: `GET /api/hunts?all=1`, `POST /api/hunts {species, form, gameKey, methodId, modifiers}`, `PUT /api/hunts/{id} {count|modifiers|notes|elapsedSeconds}`, `POST /api/hunts/{id}/increment {delta}` (delta ±1..±100), `POST /api/hunts/{id}/finish {found: bool}`, `DELETE /api/hunts/{id}`, `GET /api/hunts/species/{id}/games` (combina `DexService.AvailabilityAsync` com os encontros nossos e `gift` dos dados; devolve os jogos e os métodos válidos de cada um).

- [ ] Testes: `Current(masuda, {shiny_charm:true}, 0)` em `sv` = 512; `Cumulative(512, 355) ≈ 0.50`; incremento negativo não vai abaixo de 0; `finish` grava `EndedAt`; hunt de outro dono não aparece (OwnerScope). Migração sobre um banco com dados antigos mantém o cofre (teste cria o banco com a migração anterior, insere um StoredPokemon, migra, conta 1). Commit `feat: hunts de shiny (backend)`.

### Task 20: Frontend do Contador Hunt e atalhos na janela

**Files:**
- Create: `frontend/src/pages/hunt/HuntPage.tsx` (lista de hunts ativas/pausadas), `NewHuntWizard.tsx` (escolhe Pokémon com `SearchSelect` → jogos disponíveis → método → modificadores), `HuntCounter.tsx` (número grande, odd, chance acumulada, cronômetro, +/−, achei!), `frontend/src/lib/hotkeys.ts` (`useHotkey(combo, handler)`; ignora quando o alvo é input/textarea/contentEditable), `frontend/src/pages/settings/HotkeysCard.tsx` (grava `hotkeys.increment`/`hotkeys.decrement` via `GET/PUT /api/settings/hotkeys` — adicionar em `SettingsEndpoints.cs`, padrão `Space` e `Backspace`), locales `hunt.*`
- Test: `frontend/src/lib/hotkeys.test.ts` (combo parse; evento vindo de `<input>` é ignorado)

- [ ] TDD; commit `feat: Contador Hunt`.

### Task 21: Atalho global (Windows e X11)

**Files:**
- Create: `app/TradeRock.App/Hotkeys/GlobalHotkeys.cs` (interface `IGlobalHotkeys { bool Supported; string? Reason; void Register(string id, string combo); void Unregister(string id); event Action<string> Pressed; }`), `Hotkeys/WindowsHotkeys.cs` (thread própria com `RegisterHotKey`/`GetMessage` via P/Invoke `user32`), `Hotkeys/X11Hotkeys.cs` (`XOpenDisplay`, `XGrabKey`, `XNextEvent` em thread própria via P/Invoke `libX11.so.6`; se `DISPLAY` vazio ou `XDG_SESSION_TYPE=wayland`, `Supported=false` com `Reason="wayland"`), `Hotkeys/NoHotkeys.cs`
- Modify: `SettingsEndpoints.cs` (`hotkeys.global: bool`, `GET /api/settings/hotkeys` devolve `supported` e `reason`), `TradeEndpoints.cs`-like SSE: `GET /api/hunts/events` manda `{type:"hotkey", id}`; o `HuntCounter` escuta e aplica
- Test: `GlobalHotkeysTests.cs` (parse de combo "Ctrl+Shift+F7" para modificadores e vk; em ambiente sem X11 `Supported=false` e nada explode)

- [ ] TDD; commit `feat: atalho global do contador (Windows e X11)`.

## Entrega F: Nuzlocke

### Task 22: Backend das runs

**Files:**
- Modify: `Entities.cs` (+ `NuzlockeRun : IOwned { Guid Id; Guid OwnerId; string Name; string GameKey; string RulesJson; string Status /* active|paused|champion|wipe */; DateTimeOffset StartedAt; DateTimeOffset? EndedAt; string? CurrentAreaId; }`, `NuzlockeMon { Guid Id; Guid RunId; string AreaId; int Species; int Form; string? Nickname; int Level; string Status /* party|box|dead|missed|failed */; string? DeathCause; DateTimeOffset CaughtAt; DateTimeOffset? DiedAt; string MovesJson; }`, `NuzlockeEvent { Guid Id; Guid RunId; DateTimeOffset At; string Kind; string Json; }`), migração `Nuzlocke`
- Create: `app/TradeRock.Core/Nuzlocke/NuzlockeStore.cs`, `app/TradeRock.Core/Nuzlocke/NuzlockeRules.cs` (`NuzlockeRules(bool FirstEncounterOnly, bool NicknameRequired, bool FaintIsDeath, bool SpeciesClause, bool ShinyClause, bool DupesClause, bool LevelCapRequired, string? Custom)`), `app/TradeRock.Core/Nuzlocke/RunProgress.cs`, `app/TradeRock.App/Api/NuzlockeEndpoints.cs`
- Test: `NuzlockeStoreTests.cs`, `RunProgressTests.cs`

**Interfaces:**
```csharp
public sealed record RunView(NuzlockeRun Run, GameArea CurrentArea, BossBattle? NextBoss, int? LevelCap, IReadOnlyList<SpeciesEncounterOption> AreaEncounters /* da PokeAPI (versões do grupo) ou dos nossos encontros */, IReadOnlyList<TrainerBattle> AreaTrainers, bool TrainersAvailable, IReadOnlyList<NuzlockeMon> Party, IReadOnlyList<NuzlockeMon> Box, IReadOnlyList<NuzlockeMon> Graveyard, IReadOnlyList<NuzlockeMon> OverCap /* party acima do cap */);
```
- `RunProgress.Compute(run, GameDataStore, DexService)` → `RunView`. Próximo chefe = primeiro `BossBattle` cujo id não está em `BossesBefore` de nenhuma área já visitada... simplificar: chefe = o de menor `Order` ainda não marcado como vencido (evento `boss_defeated`). Level cap = `LevelCap` dele.
- Endpoints: `GET /api/nuzlocke`, `POST /api/nuzlocke {name, gameKey, rules}`, `GET /api/nuzlocke/{id}` (RunView), `PUT /api/nuzlocke/{id}/area {areaId}`, `POST /api/nuzlocke/{id}/encounter {areaId, species, form, nickname, level, status}`, `PUT /api/nuzlocke/{id}/mon/{monId} {status, level, nickname, deathCause, moves}`, `POST /api/nuzlocke/{id}/boss/{bossId}/defeated`, `POST /api/nuzlocke/{id}/finish {status: champion|wipe}`, `DELETE /api/nuzlocke/{id}`, `GET /api/nuzlocke/{id}/mon/{monId}/learnset` (próximos golpes por nível e na evolução, via `DexService`).
- Regra do primeiro encontro: segundo `encounter` na mesma área com `status=party|box` é recusado com `area_already_used` quando `FirstEncounterOnly`; `SpeciesClause`: se a espécie (ou linha, com `DupesClause`) já foi capturada, devolve `dupe` e deixa registrar como `missed`; `ShinyClause` ignora as duas.

- [ ] Testes: run em `frlg` começa em `pallet-town` com próximo chefe Brock (cap 14 — valor vem dos dados da Task 9; o teste lê `Bosses("frlg")[0].LevelCap` em vez de fixar); segundo encontro na mesma área → `area_already_used`; `party` com nível > cap aparece em `OverCap`; run em `sv` → `TrainersAvailable == false` e `AreaTrainers` vazio sem exceção; `finish` com `wipe` grava `EndedAt`. Commit `feat: runs de Nuzlocke (backend)`.

### Task 23: Frontend do Nuzlocke

**Files:**
- Create: `frontend/src/pages/nuzlocke/NuzlockePage.tsx` (lista), `NewRunDialog.tsx` (jogo + regras), `RunPage.tsx` (rota `/nuzlocke/:id`: colunas Chefe/Área/Equipe; aba Cemitério e Estatísticas), `BossCard.tsx`, `AreaCard.tsx` (encontros + "capturei"), `TeamCard.tsx` (sprites, nível, golpes, próximos golpes), `TrainersCard.tsx`, `locales nuzlocke.*`
- Test: `frontend/src/pages/nuzlocke/nuzlocke.test.ts` (função pura `overCap(party, cap)` e formatação de regra)

- [ ] TDD; commit `feat: tela do Nuzlocke`.

### Task 24: Mapa esquemático em SVG

**Files:**
- Create: `frontend/src/pages/nuzlocke/RegionMap.tsx` (`<RegionMap graph={MapGraph} game={key} states={Record<areaId, 'none'|'visited'|'caught'|'dead'|'missed'>} current={areaId} onSelect={(areaId)=>void} />`: renderiza `nodes` como círculos/retângulos (cidade = quadrado, rota = círculo, dungeon = losango), `edges` como linhas; cores por estado com os tokens do tema; nó atual com anel `accent`; filtra nós por `games`), `frontend/src/pages/nuzlocke/regionMap.test.ts` (filtragem por jogo; mapeamento estado→classe)
- Modify: `RunPage.tsx` (mapa no topo, clicar muda a área via `PUT /area`)

- [ ] TDD; commit `feat: mapa esquemático da run`.

## Entrega G: Guias

### Task 25: Infraestrutura dos guias

**Files:**
- Create: `frontend/src/lib/guides.ts` (`import.meta.glob('../../../content/*/guides/**/*.md', {query:'?raw'})` **lazy**; `guideIndex(lang)` lê `content/<lang>/guides/index.json` (estrutura: `{ games: [{key, title, sections:[{id,title,file}]}], mechanics:[...], vgc:[...], mythology:[...] }`), `loadGuide(lang, file)`), `frontend/src/pages/guides/GuidesPage.tsx` (índice com 4 seções), `GuidePage.tsx` (rota `/guias/:section/:slug/:page?`: sumário lateral + markdown), `frontend/src/pages/guides/blocks/{BossTable,EncounterTable,TmTable}.tsx` (blocos estruturados: no markdown, uma linha `:::bosses frlg` ou `:::encounters frlg kanto-route-1` ou `:::tms frlg` vira o componente que lê `/api/data` e `/api/dex`; implementar no `Markdown.tsx` via plugin de diretiva simples por regex antes do `react-markdown`), `scripts/check-guides.py` (valida: todo jogo de `GameVersions` tem guia com as seções obrigatórias `overview, walkthrough, bosses, tms, exclusives, trades, events, postgame, legendaries`; todo arquivo existe em pt-BR e en; toda diretiva `:::bosses <jogo>` cita jogo existente), `scripts/tests/test_check_guides.py`
- Modify: `scripts/ci.sh` (roda `check-guides.py`), locales `guides.*`
- Test: `frontend/src/lib/guides.test.ts` (parser das diretivas)

- [ ] TDD; commit `feat: infraestrutura dos guias`.

### Task 26: Guias dos jogos, Gen 1–3

**Files:** `content/{pt-BR,en}/guides/games/{rb,y,gs,c,rs,e,frlg}/{overview,walkthrough,bosses,tms,exclusives,trades,events,postgame,legendaries}.md` + entradas em `index.json`.

- **Estilo (pedido do autor em 05/11/2026): rápido de ler, na pegada do allgenwiki, só fatos importantes.** Nada de narrativa ou parágrafos longos. `walkthrough.md`: um `##` por trecho na ordem das áreas (`areas/<jogo>.json`), e dentro dele **listas curtas**: "Faça" (3 a 6 itens), "Itens" (lista), "Encontros" (`:::encounters <jogo> <area>`), "Chefe" (`:::boss <jogo> <id>` + 1 linha de dica). `bosses.md` usa `:::bosses <jogo>` + uma linha de dica por chefe. `tms.md` usa `:::tms <jogo>` + onde achar cada um (pesquisar). Textos em pt-BR e en. Fontes de fatos: dados embutidos, pesquisa na web e, quando houver, as notas em `docs/superpowers/sources/*.md` (fatos extraídos de detonados que o autor enviou; nunca copiar prosa).
- Test: `check-guides.py` passa para esses jogos.

- [ ] Commit `content: guias dos jogos da Gen 1 à 3`.

### Task 27: Guias dos jogos, Gen 4–6 (`dp, pt, hgss, bw, b2w2, xy, oras`)

Igual à Task 26. Commit `content: guias dos jogos da Gen 4 à 6`.

### Task 28: Guias dos jogos, Gen 7–9 (`sm, usum, lgpe, swsh, bdsp, pla, sv, za`)

Igual à Task 26. Para o Z-A, usar `docs/superpowers/sources/za-pokemythology-notes.md` (chefes, Wild Zones, trocas, Mega Stones, TMs). Commit `content: guias dos jogos da Gen 7 à 9`.

### Task 29: Mecânicas e Mitologia

**Files:** `content/{pt-BR,en}/guides/mechanics/{natures,evs-ivs,breeding,shiny,types,evolutions,happiness,abilities,held-items,contests,safari,secret-bases,battle-gimmicks,status}.md`; `content/{pt-BR,en}/guides/mythology/{kanto,johto,hoenn,sinnoh,unova,kalos,alola,galar,hisui,paldea,lumiose}.md` (lore da região e dos lendários dela) + `index.json`.

- `shiny.md` usa uma diretiva `:::shiny-methods` que lista os métodos de `shiny-methods.json` (bloco novo `ShinyMethodsTable.tsx`).
- Commit `content: guias de mecânicas e mitologia`.

### Task 30: VGC Champions: textos, regulamentação, times sugeridos, Minha equipe e paste

**Files:**
- Create: `content/{pt-BR,en}/guides/vgc/{starting,regulation,teams,reading-a-team}.md`, `assets/data/vgc/teams.json` (`VgcTeam(string Id, string Name, string Archetype, string Regulation, string DateIso, string Paste /* formato Showdown */, string IdeaPt, string IdeaEn, string HowToPlayPt, string HowToPlayEn, string DangersPt, string DangersEn)`; 6–10 times da regulamentação vigente em 05/11/2026, pesquisada na web, escritos por nós), `assets/data/vgc/regulation.json` (`{ name, validFrom, validTo?, allowedDexes, bannedSpecies[], notes }`), `app/TradeRock.Core/Vgc/TeamAnalysis.cs` (`Analyze(IReadOnlyList<(ushort species, byte form, string[] moves)>, DexService) -> TeamReport(weaknesses por tipo, resistências, cobertura ofensiva por tipo, velocidades base)`), `app/TradeRock.App/Api/VgcEndpoints.cs` (`GET /api/vgc/teams`, `GET /api/vgc/regulation`, `POST /api/vgc/analyze {paste, game:"sv"}` usando `ShowdownImport.Parse`), `frontend/src/pages/guides/vgc/{TeamsPage,MyTeamPage,PasteDialog}.tsx` (copiar paste; importar pro Boxes via `POST /api/builder/showdown` + `/api/builder/generate` já existentes; analisar paste colado)
- Test: `TeamAnalysisTests.cs` (time Gengar+Blastoise: fraqueza a Ghost/Dark/Psychic/Ground/Electric/Grass; cobertura inclui ghost/water); `GameDataTests` carrega `teams.json` e cada paste parseia com 6 Pokémon.

- [ ] TDD; commit `feat: seção VGC do Champions`.

## Entrega H: Histórico, Início, internet opcional, documentação

### Task 31: Histórico unificado

**Files:**
- Create: `app/TradeRock.Core/Storage/HistoryFeed.cs` (`ListAsync(string? kind, int limit, ct) -> IReadOnlyList<HistoryItem(string Kind /* trade|raid|gift|circle|hunt|nuzlocke */, Guid Id, DateTimeOffset At, string Title, string Subtitle, string? GameKey, int? Species, bool Ok)>`; trades vêm do `TradeRecord` (kind por `Game`/atividade como o `ActivityService` grava hoje; conferir como raids/gifts são registrados e mapear), hunts terminadas do `ShinyHunt`, runs terminadas do `NuzlockeRun`), `GET /api/history?kind&limit` em `TradeEndpoints.cs` ou novo `HistoryEndpoints.cs`
- Modify: `frontend/src/pages/HistoryPage.tsx` (filtros `Segmented` por tipo; detalhe abre o existente para trocas, `/hunt/:id` ou `/nuzlocke/:id` para os outros)
- Test: `HistoryFeedTests.cs` (mistura 1 trade, 1 hunt, 1 run; ordena por data; filtro `kind=hunt`)

- [ ] TDD; commit `feat: Histórico unificado`.

### Task 32: Início

**Files:** Modify `frontend/src/pages/HomePage.tsx`: cartões "Em andamento" (hunts e runs ativas: `GET /api/hunts` + `GET /api/nuzlocke`), "Última troca" (já existe), "Mystery Gift" (só local), "Guia do dia" (escolhe determinístico por data entre os guias do índice), busca rápida da Pokédex (input → `/pokedex?text=`). Test: `home.test.ts` (guia do dia determinístico pela data).

- [ ] TDD; commit `feat: Início do RockAssistent`.

### Task 33: Internet opcional (Smogon e pokepast.es)

**Files:**
- Create: `app/TradeRock.Core/Vgc/SmogonStats.cs` (`DownloadAsync(HttpClient, string format /* "gen9championsvgc2026regmc-1760" */, DateOnly month, ct)` → salva em `<DataDir>/vgc/<format>-<yyyy-MM>.json`; `Parse(stream) -> UsageReport(top 50: species, usage %, moves, items, abilities, teammates)`; URL `https://www.smogon.com/stats/{yyyy-MM}/chaos/{format}.json`; sem cabeçalhos além de User-Agent; nada enviado), `app/TradeRock.Core/Vgc/PokePaste.cs` (`FetchAsync(HttpClient, Uri)`: aceita só host `pokepast.es`, baixa `<url>/raw`), settings `internet.smogon: bool` e `internet.pokepaste: bool` (padrão false) com `GET/PUT /api/settings/internet`, endpoints `POST /api/vgc/usage/refresh` (400 `internet_disabled` quando desligado) e `GET /api/vgc/usage`, `POST /api/vgc/paste/fetch {url}`
- Modify: `frontend/src/pages/SettingsPage.tsx` (cartão "Internet opcional" com as duas chaves e explicação), `TeamsPage.tsx` (aba "Mais usados" quando houver arquivo), `PasteDialog.tsx` (campo de link habilitado só com a opção ligada)
- Test: `SmogonStatsTests.cs` (parse de um JSON `chaos` reduzido em fixture; `FetchAsync` recusa `https://evil.example/x`; com a opção desligada o endpoint devolve `internet_disabled`)

- [ ] TDD; commit `feat: internet opcional (estatísticas do Smogon e pokepast.es)`.

### Task 34: Documentação e CLAUDE.md

**Files:** Modify `docs/decisions.md` (+ **D32** Rebranding RockAssistent e novas áreas: resumo das decisões da spec, inclusive a seção 7; **D33** Internet opcional), `CLAUDE.md` (nome do app, novas pastas no mapa do código: `Dex/`, `GameData/`, `Hunts/`, `Nuzlocke/`, `Vgc/`, `Hotkeys/`, `assets/data/`, `content/*/guides`; tabela de decisões; linha "Entrega 20"; o `ci.sh` agora roda `build-dex.py` e `check-guides.py`), `README.md` (seção dos dados: PokeAPI BSD-3, como gerar o dex), `docs/dev-host.md` (como testar Boxes com um save de emulador), `docs/superpowers/data-doubts.md` (consolidado pelo analista de dados na última revisão).

- [ ] Commit `docs: D32 e D33, CLAUDE.md e README do RockAssistent`.

### Task 35: CI completo e release notes

- [ ] Rodar `scripts/ci.sh` inteiro → tudo verde (anotar a saída no relatório). Atualizar `packaging/release-notes.md` com os nomes novos. Commit `chore: CI verde do RockAssistent`.

---

## Processo de revisão (regras deste plano)

- Implementadores: modelo **opus** (Opus 5.5). Revisor de código por tarefa e revisão final: modelo **fable** (Fable 5.1). Nas tarefas de dados (8–14, 29, 30 `teams.json`), além do revisor de código, um **analista de dados** no modelo **fable** com acesso à web usa `docs/superpowers/plans/data-review-prompt.md`; cada item que ele não conseguir confirmar vai para `docs/superpowers/data-doubts.md` (uma linha: jogo, item, o que está em dúvida, fontes tentadas).
- Nenhuma tarefa é pulada por falta de dados: o que não se acha vira `verified: false` e entra em `data-doubts.md`.
