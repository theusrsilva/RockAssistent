# RockAssistent (repositório TradeRock): instruções para o Claude Code

App desktop (.NET 10 + React), com público leigo: assistente Pokémon offline de todos os jogos da série principal
(Pokédex, Guias, Boxes com saves por pasta, Nuzlocke, Contador Hunt, Histórico) que também troca Pokémon com os jogos
do Switch via wireless local (LDN). O nome visível é **RockAssistent**; namespaces, pastas, scripts, pacotes, binário
`traderock`, worker e pasta de dados continuam **TradeRock** (D33). Especificação original da troca:
[docs/spec.md](docs/spec.md) (o projeto se chamava "FRLG Trade Studio"); do RockAssistent:
[docs/superpowers/specs/2026-11-05-rockassistent-design.md](docs/superpowers/specs/2026-11-05-rockassistent-design.md).
Decisões técnicas: [docs/decisions.md](docs/decisions.md) (D1–D34).

## Regras de trabalho (pedidas pelo autor, valem sempre)

- **Antes de cada entrega, apresente o plano e as dúvidas** e espere a resposta. Só então implemente.
- **Não invente APIs** do PKHeX, da biblioteca LDN (kinnay/LDN), do pokeldn, do firmware ESP32 ou do
  Photino. Consulte o código-fonte, que está nos submódulos de `engines/` ou no
  pacote NuGet. Se algo não existir, diga.
- Converse em **português**. Os textos da interface ficam em `frontend/src/locales/{pt-BR,en}.json`
  e os textos explicativos em `content/{pt-BR,en}/**/*.md`. Explique tudo de forma simples.
- **Commits:** cada trabalho na própria branch, criada a partir da `main` e levada à `main` por PR, com o
  trailer `Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>`. Commite quando o autor pedir;
  o padrão combinado é commitar cada entrega ao terminá-la. Antes de commitar, confira a branch atual
  (o autor faz merge e troca de branch pelo VS Code).
- **Tags de versão:** sempre na `main` do GitHub (`git tag -a vX origin/main`), com a versão que o autor
  disser (nunca escolha nem sugira o número). O autor gera a tag com `make tag` (`scripts/tag.sh`): o script busca a
  `origin/main`, mostra os commits desde a última tag, pergunta o tipo (Atualização, Feature, Fix ou versão digitada)
  e o pré-lançamento, monta as Novidades com os commits (sem `chore`/`docs`/`test`) e envia a tag. A mensagem da tag vira as "Novidades" do Release; a marca
  `[pré-lançamento]` só quando o autor pedir (versões com sufixo, como `v1.0.0-rc.1`, também saem
  como pré-lançamento). O CI roda só nas tags `v*` e publica o Release com o texto de
  `packaging/release-notes.md` e a lista automática do GitHub.
- Rode `scripts/ci.sh` antes de commitar. Ele roda os testes dos scripts e o `build-dex.py`, o `check-guides.py`,
  typecheck, testes e build do frontend, os testes do worker (Python `unittest`), os testes do pokeldn
  (`scripts/test-pokeldn.sh`, ~5 min) e o build e os testes do .NET.
- **Dados e guias:** fatos do jogo podem ser pesquisados em qualquer site e escritos no nosso formato; texto corrido
  (guias, mitologia) é sempre escrito do zero, nunca copiado. Dado sem fonte conferida leva `"verified": false` e entra
  em `docs/superpowers/data-doubts.md`.

## Segurança (inegociável)

- **prod.keys:** nunca exibir valores, logar, enviar pela rede nem incluir em backups, exportações ou
  relatórios. O arquivo fica com permissão 600. O app nunca baixa chaves nem sugere fontes ("só use
  as extraídas do seu próprio console").
- Nunca distribuir prod.keys nem ROMs.
- O app principal **nunca** roda como root e nunca pede nem guarda senha. O root fica isolado no
  worker (pkexec + polkit, D15).
- Sem telemetria. As consultas à internet são a procura de atualizações no GitHub (D29) e, só quando a pessoa liga
  nas Configurações, as duas da internet opcional (D32: estatísticas do Smogon e pokepast.es); nenhuma envia nada do
  PC. Todo o resto funciona offline (dados embutidos). O pokeldn continua sem internet.

## Decisões do autor (resumo)

| Tema | Decisão |
| --- | --- |
| Runtime | .NET 10, não .NET 8 (o PKHeX.Core 26.8.26 só suporta net10.0) |
| Motores externos | Submódulos do upstream, sem fork. Ajustes via wrapper/patches nossos (`engines/patches`) |
| Placas sem teste | Entram no catálogo com o aviso "não testado" |
| Sprites | PokeAPI (distribuição só entre amigos por enquanto) |
| Ilustrações | SVG em vez de fotos |
| Trocas por sessão | Até 6 |
| Trocas pelo pokeldn | O recebido é guardado assim que o pokeldn o grava (o Z-A e o Arceus gravam depois do `[done]`: o worker espera o arquivo). Troca feita conta mesmo se o console encerrar, a conexão cair ou o processo sair com erro. A sala espera 240 s. Cancelar manda `stop` pela entrada (saída educada também no Windows). Tela ao vivo com o fluxo do anfitrião (D30) |
| Mini IA das raids | Dynamax Adventure e Max Raid em que o TradeRock entra (SwSh): escolhe o aluguel e o golpe de cada batalha "no papel" (tipo, força, STAB, precisão), com a tabela de golpes do PokeAPI. Sem decisão turno a turno nem Dynamax (D31) |
| Testes E2E | Só pela API |
| Construtor | Modo automático (AutoMod) por padrão e modo avançado. OT padrão = perfil "Meu treinador". Mensagens do PKHeX traduzidas só as da Gen 3. Nomes de golpes e itens em inglês |
| Cofre | Caixas de 30 posições com Caixa de entrada. Lixeira recuperável. Injeção em `.sav` sobrescreve, com backup. Exportação padrão: .pk3 de 100 bytes |
| Logs | Aba Logs nas Configurações (só na janela do PC). Arquivo por dia na pasta de dados, 7 dias, sem chaves nem dados (D28) |
| Assistente | RTL8821CE também recomendada. Seletor de simulação só em desenvolvimento. Desde a Entrega 12 é a lista de Configurações → Conexão (`/configurar` redireciona) |
| Worker | Em Python (runtime embutido). Canal por stdin/stdout em vez de socket Unix. Polkit `auth_admin_keep`. Patches do FRLG-LDN-Trade-Console aplicados. `iw` e `ip` estáticos embutidos (o autor não quer mandar a pessoa instalar pré-requisitos) |
| Pacotes | .deb e AppImage. Tag `v*` publica o Release direto. Tela Sobre com créditos e oferta do código-fonte |
| Atualização | Procura no GitHub ao abrir (1×/dia, desligável), sem pré-lançamentos. Baixa, confere o SHA256SUMS e instala: Windows pelo instalador silencioso (UAC), AppImage troca o arquivo, .deb pelo pkexec (D29) |
| Nomes no Release | Só `RockAssistent-*` (decisão do autor de 07/11/2026): sem cópias `TradeRock-*`. As instalações antigas baixam o RockAssistent uma vez pelo GitHub; o app continua aceitando os dois nomes (D33) |
| Teste com o Switch | Até 30 s. Sessão de outro jogo decifrada não aprova. Vale até mudar a placa ou o prod.keys (não expira). Opcional desde a D28: a troca libera sem ele |
| Modo servidor | HTTP (só rede de casa, com aviso). Argon2id (Konscious). Fila começa a troca sozinha. Conta nova com os Pokémon de demonstração. Porta 8337. Começa desligado a cada abertura |
| Windows | Só ESP32. Instalador Inno Setup com administrador (regra do firewall só em redes privadas). Sem assinatura digital por enquanto. Dados em `%LOCALAPPDATA%` com ACL só do usuário. Sem WebView2: abre no navegador |
| Jogos do Switch | Todos os jogos com troca local (FRLG, Let's Go, SwSh, BDSP, Arceus, SV, Z-A) pelo pokeldn (submódulo, D21). Escopo todo (troca, raids, Mystery Gift, Union Circle). ESP32 passa a usar o firmware do pokeldn. Python embutido no Windows. Modo LAN só no SwSh. Legalidade avisa e não bloqueia nos jogos novos. Cofre único com filtro por jogo. "Meu treinador" por jogo. Mover entre jogos como o HOME (D22) |
| Internet opcional | Estatísticas do Smogon (formato do Champions, só ao clicar em Atualizar) e links do pokepast.es. Desligadas por padrão, só GET, mudar só na janela do PC (D32) |
| RockAssistent | Nome só visível. Menu: Início, Pokédex, Guias, Boxes, Trocas, Mystery Gift, Nuzlocke, Contador Hunt, Histórico, Configurações, Sobre (celular: Início, Pokédex, Boxes, Trocas + "Mais"). Pokédex do PokeAPI (`dex.sqlite` gerado no build). Dados nossos em `assets/data` (JSON estrito, validador). Boxes com saves de pastas (backup automático; Gen 1/2 compactam caixas; mover entre gerações com aviso; clone só em saves). Hunt com odds por rolagens e atalho global opcional (Windows, X11). Nuzlocke com versão obrigatória nos jogos duplos e mapa SVG. Histórico unificado (`ActivityRecord`) (D33) |
| Cofre de todas as gerações | Formato próprio do cofre (pk1 a pa9), separado do catálogo de troca. Gen 1, 2 e 4 a 7 ficam no cofre, mas não trocam nem se editam. "Adaptar para…" segue os caminhos oficiais (VC, Pal Park, Poké Transfer, Poké Transporter, Bank, HOME) com as regras de cada etapa; o original fica e a cópia vai para a Caixa de entrada (D34) |

## Estado das entregas

| # | Entrega | Estado |
| --- | --- | --- |
| 1 | Monorepo, servidor local, janela Photino | feita |
| 2 | FakeEngine + assistente de troca de ponta a ponta | feita |
| 3 | Construtor + legalidade (PKHeX + AutoMod) | feita |
| 4 | Cofre (caixas, tags, lixeira, exportação, injeção em .sav) | feita |
| 5 | Catálogo de hardware + assistente de primeira execução (7 etapas) | feita |
| 6 | Worker privilegiado + polkit + PythonLdnEngine + fluxo de teste no host | feita, **sem teste com hardware real** |
| 7 | Teste do beacon com a placa Wi-Fi (etapa 6 do assistente) | feita, **sem teste com hardware real** |
| 8 | Empacotamento Linux (AppImage e .deb) e CI | feita; AppImage com FUSE, pkexec e janela **não testados** |
| 9 | Esp32SerialEngine + gravação de firmware + validações ESP32 + beacon com ESP32 | feita, **sem placa real** |
| 10 | Modo servidor na rede (contas, cofre por conta, fila) | feita, **sem celular real** |
| 11 | Build e instalador Windows (só ESP32) | feita, **sem ESP32 real no Windows** |
| 12 | Nova interface (protótipo "TradeRock v2" do Claude Design, visual A claro/escuro, celular). O assistente de 7 etapas virou a lista de Configurações → Conexão; edição no painel do cofre (`POST /api/vault/pokemon/{id}/replace`) | feita |
| 13 | Integração do pokeldn (submódulo, patch do modo TradeRock, trava de rede, testes no CI, D21) | feita |
| 14 | Base multijogo: catálogo, cofre e construtor por jogo, "Meu treinador" por jogo, mover entre jogos (HOME), sprites até a Gen 9 (D22) | feita; troca real com os jogos novos só na 15 |
| 15 | Motor pokeldn: ESP32 com o firmware do pokeldn (todos os jogos, Linux e Windows) e placa Wi-Fi (jogos novos pelo worker); o TradeRock é sempre o anfitrião (D23) | feita, **sem placa nem console reais** |
| 16 | Modo LAN do Sword/Shield pela rede de casa: motor `lan`, cartão em Conexão, regra UDP no firewall do Windows (D24) | feita, **sem console real** |
| 17 | Sem o motor e o submódulo do easyworld; catálogo de ESP32 com os dados do pokeldn; teste do modo AP da placa Wi-Fi (D25) | feita, **AP sem placa real** |
| 18 | Outras atividades (D26): 18a Mystery Gift do FRLG e do SwSh (tela Atividades); 18b raids do SwSh (Max Raid e Dynamax Adventure, também pela rede de casa) e Tera Raid do SV, com as raids de evento embutidas; 18c Union Circle do SV (missões e encontros) | feita, **sem console real** |
| 19 | Modo servidor com os jogos novos: troca de qualquer jogo pela fila, "Meu treinador" por jogo e por conta; outras atividades só na janela do PC (D27) | feita, **sem celular real** |
| 20 | RockAssistent: rebranding visível, menu novo, Pokédex offline, Guias dos 22 jogos, Boxes com saves por pasta, Contador Hunt, Nuzlocke, VGC Champions, Histórico unificado, Início, internet opcional (D32, D33) | feita (PR #14), **sem hardware real** |
| 21 | Rodadas de ajuste do RockAssistent (PRs #15–#18): Pokédex com PokeAPI ∪ JSON ∪ PKHeX e travas de shiny; time do rival por inicial; mapas do Nuzlocke em estilo metrô; fonte pixel só na logo; guia → Pokédex no jogo do guia e busca no guia; inicial já capturado no Nuzlocke (conta a área só em cidade), botão evoluir e setas de área; cofre com Gen 1–9 e "Adaptar para…" (D34) | feita, tag `v02.00.02` |

Todas as entregas estão na `main` (até o PR #18). O primeiro pré-lançamento foi a tag `v0.00.01`; a última tag é a
`v02.00.02` (09/11/2026). As branches das entregas foram apagadas depois do merge.

## Pendências já combinadas

- **Nunca verificado de verdade:**
  - janela nativa do Photino no Linux (WebKitGTK) e os diálogos de arquivo nos dois sistemas (no
    Windows, a janela abriu com o WebView2);
  - `.sav` real e o formato de save NSO do Switch;
  - troca com Switch real nos outros jogos (o Legends: Z-A pelo ESP32-S3 no Windows funcionou em 01/11/2026,
    v01.02.00, D30);
  - teste com o Switch (Entrega 7, D16);
  - ESP32 real no Linux (no Windows, o ESP32-S3 do autor gravou o firmware, passou no teste com o Switch e trocou
    com o Z-A);
  - teste da placa com root;
  - AppImage montado por FUSE, instalação do worker pelo pkexec (diálogo) e a janela a partir dos pacotes.
  - modo servidor com um celular de verdade na rede (Entrega 10, D19);
  - ESP32 com conversor CH340/CP210x no Windows (o ESP32-S3 usa a USB nativa). O app, a janela, o instalador e os
    testes foram verificados num Windows 11 real, pelo WSL.
  - Entrega 20: as telas novas num navegador (nenhuma foi vista); o atalho global com tecla apertada de verdade no
    Windows e no X11 (só o grab no WSLg); saves reais de emulador nas Boxes; Smogon e pokepast.es com rede real (só o
    arquivo do Smogon de 2026-10 foi baixado e lido).
- Dados e guias: as dúvidas dos JSON e o que foi decidido estão em
  [docs/superpowers/data-doubts.md](docs/superpowers/data-doubts.md) e
  [docs/superpowers/data-decisions.md](docs/superpowers/data-decisions.md). Regra do autor para dados: vale o que
  tiver mais fontes; o **Serebii** pesa mais e desempata.

  O roteiro de teste no host está em [docs/dev-host.md](docs/dev-host.md).

## Mapa rápido do código

| Pasta | Conteúdo |
| --- | --- |
| `app/TradeRock.App` | ASP.NET Core + Photino (API em `Api/`, DI em `ServiceSetup.cs`, modo servidor em `Server/`: contas, logins, `AccessPolicy`, liga/desliga; atalho global do contador em `Hotkeys/`: Windows, X11) |
| `app/TradeRock.Core` | Domínio: `Games/` (catálogo dos jogos do Switch, IDs de treinador, `GameVersions` com os 22 jogos), `Trading/`, `Engines/` (Fake, PythonLdn, `Pokeldn/` com ESP32 e LAN, `Worker/`), `Esp32/` (firmware, serial, esptool), `Pokemon/`, `Builder/`, `Storage/` (EF Core + SQLite, `HistoryFeed`), `Saves/` (`SaveFileAccess`, `SaveFolders`, `SaveLibrary` das Boxes), `Setup/` (catálogo, probe do Linux, verificações, prod.keys), `Dex/` (`DexService` sobre o `dex.sqlite`), `GameData/` (`GameDataStore` e validador dos JSON de `assets/data`), `Hunts/` (`ShinyOdds`, `HuntStore`), `Nuzlocke/` (runs, regras, `RunProgress`), `Vgc/` (regulamentação, times, análise, Smogon, pokepast.es), `Activities/`, `Updates/` |
| `app/TradeRock.Tests` | xUnit v3; worker falso em `FakeWorker.cs`, pokeldn falso em `FakePokeldn` (`Esp32Tests.cs`) |
| `worker/` | Worker Python: `traderock_worker/` (server, radio, trade_child, card_child, beacon_child, esp32_flash, events, protocol) e `tests/` (dublês de trio/ldn em `fake_ldn`) |
| `frontend/` | React + Vite + TS + Tailwind; o build vai para `app/TradeRock.App/wwwroot`. Cores do tema em `src/index.css` (tokens `bg-surface`, `text-muted`, `bg-accent`...); componentes base em `src/components/ui.tsx`; fontes e ícones embutidos (`src/assets/fonts`) |
| `catalog/hardware-catalog.json` | Catálogo de placas |
| `assets/` | Sprites 1–1025 do PokeAPI (`sprites/`, com `shiny/` e `forms/`; `scripts/fetch-sprites.sh`), Pokémon de demonstração (`demo/`), firmware do ESP32 (`firmware/pokeldn/`) e dados (`data/`: `dex.sqlite` gerado e git-ignorado; nossos `bosses/`, `trainers/`, `areas/`, `encounters/`, `maps/`, `vgc/` e `shiny-methods.json`) |
| `content/*/guides` | Guias em markdown (`index.json`, `games/<jogo>/`, `mechanics/`, `vgc/`, `mythology/`) com diretivas `:::bosses`, `:::boss`, `:::encounters`, `:::tms`, `:::shiny-methods`, `:::vgc-*` |
| `docs/superpowers/` | Spec e plano do RockAssistent, `data-doubts.md` e `sources/` (fatos tirados dos detonados do autor, nunca a prosa) |
| `engines/` | Submódulos (frlg-ldn-trade, pokeldn, PKHeX-Plugins) e `patches/` |
| `scripts/` | `ci.sh`, `build-dex.py` (Pokédex do PokeAPI), `check-guides.py` (`--strict`), `dev-host.sh`, `build-worker.sh`, `stage-pokeldn.sh`, `test-pokeldn.sh`, `build-nettools.sh`, `install-worker.sh`, `package-linux.sh`, `smoke-linux.sh`, `package-windows.sh`, `smoke-windows.ps1` |
| `packaging/linux/` | Atalho, ícone e `worker-setup.sh` (instalação do worker do AppImage como root) |
| `packaging/windows/` | `traderock.iss` (Inno Setup), `traderock.ico` e `make-icon.py` |

## Ambiente

- `git submodule update --init --recursive` antes de tudo.
- .NET 10 SDK, Node 24 e Python 3.12 (para os testes do worker).
- `python3 -I scripts/build-dex.py` gera `assets/data/dex.sqlite` (git-ignorado; o build do Core falha sem ele). Os
  CSVs do PokeAPI ficam em `~/.cache/traderock/pokeapi`; o `ci.sh` roda o script sozinho.
- `scripts/build-worker.sh` baixa o Python embutido e o Zig. A primeira vez leva alguns minutos; o
  resultado fica em `~/.cache/traderock`.
- JSON dos enums: snake_case. xUnit v3 com Microsoft.Testing.Platform:
  `dotnet test --project app/TradeRock.Tests`.
- Windows a partir do WSL: o WSL roda `.exe` do Windows. `scripts/package-windows.sh --installer` acha o
  Inno Setup 6 instalado no Windows (winget, escopo do usuário) e dá para rodar o app, os testes
  publicados para `win-x64` e `scripts/smoke-windows.ps1` (pede o UAC) pelo `powershell.exe`.
