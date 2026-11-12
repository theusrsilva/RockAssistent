# RockAssistent

Assistente Pokémon para o computador, que funciona **sem internet**, com todos os jogos da série principal (22
jogos, de Red/Blue a Legends: Z-A). Ele também **troca Pokémon com os jogos do Nintendo Switch** pelo wireless local
(LDN): o PC, ou uma plaquinha ESP32, finge ser um segundo Switch.

Antes ele se chamava **TradeRock**. O repositório, o pacote `.deb`, o comando `traderock` e a pasta de dados
continuam com o nome antigo.

## O que tem no app

| Área | O que faz |
| --- | --- |
| **Pokédex** | Espécies, formas, golpes, habilidades, itens e onde achar cada Pokémon em cada jogo (selvagem, presente, troca, ovo, evolução). Também avisa quando o shiny é bloqueado e quando precisa do Pokéwalker ou de um item de evento |
| **Guias** | Detonado dos 22 jogos com os chefes e os times de cada um, TMs, trocas, lendários, pós-jogo, mecânicas e VGC. Tem busca por palavra dentro do guia, e clicar num Pokémon abre a Pokédex no jogo do guia |
| **Boxes** | O **cofre do app**, com caixas de 30, caixa de entrada, tags, busca e lixeira. Aceita Pokémon de todas as gerações (Gen 1 a 9), e o "Adaptar para…" leva um Pokémon antigo para um jogo mais novo, como o Pokémon HOME. Também mostra os **saves** das pastas que você escolher, com backup antes de qualquer mudança |
| **Construtor** | Aberto pelo Boxes ("+ Novo" e "Editar"): cria Pokémon legais com o PKHeX e o AutoMod, no modo automático ou no avançado |
| **Contador Hunt** | Contador de shiny com as chances de cada método e um atalho de teclado opcional, que só funciona com um contador aberto |
| **Nuzlocke** | Runs com as regras, o mapa da região em estilo metrô e o limite de nível do próximo chefe. O time do rival segue o inicial escolhido, o inicial já entra capturado, e dá para evoluir os Pokémon e andar de área em área com setas |
| **VGC** | Times do formato atual e análise do time. As estatísticas do Smogon só aparecem com a internet opcional ligada |
| **Troca e atividades** | Troca com FireRed/LeafGreen, Let's Go, Sword/Shield, BDSP, Legends: Arceus, Scarlet/Violet e Legends: Z-A. Também tem Mystery Gift, raids (Max Raid, Dynamax Adventure, Tera Raid) e Union Circle |
| **Histórico** | Trocas, Mystery Gift, raids e Union Circle numa lista só |
| **Modo servidor** | Celulares e outros PCs da casa usam o app pelo navegador, com contas e uma fila de trocas |

Tudo funciona offline. A internet é opcional e vem desligada. Quando ligada, ela só busca as estatísticas do
Smogon, os links do pokepast.es e as atualizações do app no GitHub. Nada sai do seu PC, e não há telemetria.

## Como instalar

Baixe o arquivo da versão mais recente na página de
[Releases](https://github.com/theusrsilva/RockAssistent/releases):

| Seu computador | Arquivo | Como instalar |
| --- | --- | --- |
| **Windows 10 (1809+) ou 11**, 64 bits | `RockAssistent-Setup-<versão>.exe` | Abra e siga os passos |
| **Ubuntu 22.04+, Debian 12+** e derivados | `traderock_<versão>_amd64.deb` | `sudo apt install ./traderock_<versão>_amd64.deb` |
| **Outras distribuições Linux** | `RockAssistent-<versão>-x86_64.AppImage` | `chmod +x` no arquivo e abra |
| **Mac com chip Apple** (M1+, macOS 14+) | `RockAssistent-<versão>-macOS-arm64.dmg` | Abra e arraste para Aplicativos |

- **Windows:** o instalador ainda não tem assinatura digital, então o Windows mostra "O Windows protegeu o
  computador". Clique em **Mais informações → Executar assim mesmo**. O instalador pede administrador só para
  instalar e liberar o firewall em redes privadas; o app roda sempre como o seu usuário.
- **Mac:** o app ainda não tem a assinatura da Apple. Na primeira vez, o macOS bloqueia; vá em **Ajustes do
  Sistema → Privacidade e Segurança → Abrir Mesmo Assim**. Só é preciso uma vez.
- **AppImage:** precisa do WebKitGTK 4.1, do libnotify e do `pkexec`. Se faltar alguma coisa, o app mostra o
  comando para instalar. Para trocar pela placa Wi-Fi, use **Configurações → Conexão → Componente de rede**.
- **Atualizar:** o app procura versões novas sozinho uma vez por dia (dá para desligar) e se atualiza pela tela
  **Sobre**. Também dá para instalar a versão nova por cima. Os seus dados continuam.
- **Já tinha o TradeRock?** Baixe o RockAssistent uma vez e instale por cima. Depois disso, ele volta a se
  atualizar sozinho.
- **Conferir o download (opcional):** no Linux, `sha256sum -c SHA256SUMS --ignore-missing`. No Windows,
  `Get-FileHash <arquivo>` no PowerShell. No Mac, `grep macOS SHA256SUMS | shasum -a 256 -c`.

### Para trocar com o Switch

| Sistema | O que faz o papel do segundo Switch |
| --- | --- |
| Linux | A placa Wi-Fi do próprio PC ou um ESP32 no USB |
| Windows | Só um ESP32 no USB (o Windows não deixa usar o Wi-Fi do jeito que o LDN precisa) |
| Mac | Só um ESP32 no USB, pelo mesmo motivo |

**Configurações → Conexão** confere tudo e mostra o que falta: a placa ou o ESP32 (o app grava o firmware sozinho)
e o `prod.keys`. O RockAssistent **nunca** baixa, sugere fontes nem distribui `prod.keys` ou ROMs. Use só as chaves
extraídas do seu próprio console. O Sword/Shield também troca pela rede de casa (modo LAN), sem placa.

### Onde ficam os seus dados

| Sistema | Pasta |
| --- | --- |
| Linux | `~/.local/share/traderock` |
| Windows | `%LOCALAPPDATA%\TradeRock` (só o seu usuário tem acesso) |
| Mac | `~/Library/Application Support/TradeRock` (só o seu usuário tem acesso) |

O cofre, as hunts, os Nuzlockes, o histórico e as configurações ficam num banco SQLite nessa pasta e continuam lá
quando você fecha o app ou atualiza.

## Licença e créditos

O RockAssistent é software livre sob a **GNU AGPL-3.0**. Veja [LICENSE](LICENSE),
[THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md) e a tela **Sobre** do app. O código-fonte completo também vai em
cada Release.

Agradecimentos: [PokeAPI](https://github.com/PokeAPI/pokeapi) (dados, BSD-3, e sprites), [PKHeX](https://github.com/kwsch/PKHeX)
e PKHeX-Plugins (Pokémon, saves e legalidade), [pret](https://github.com/pret) (decompilações usadas para conferir os
dados), frlg-ldn-trade e pokeldn (troca com o Switch). Os guias foram escritos por nós; nenhum texto foi copiado de
site.

Pokémon e os nomes relacionados são marcas da Nintendo, da Creatures e da GAME FREAK. O RockAssistent é um projeto de
fã, sem ligação com elas.
