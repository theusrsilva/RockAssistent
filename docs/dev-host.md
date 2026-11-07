# Testar no host Linux (placa Wi-Fi real)

O motor com a placa Wi-Fi (`PythonLdnEngine`) **não roda em container nem no WSL**. Ele precisa de:

- **Acesso ao phy da placa.** O motor usa `/sys/class/ieee80211/phy*`, modo monitor e transmissão de
  quadros de ação via nl80211. Um container não enxerga o phy do host, e o WSL não tem placa Wi-Fi.
- **NetworkManager fora do caminho.** Ele não pode controlar as interfaces que a biblioteca LDN cria
  (`ldn*`).
- **root.** Criar interfaces e trocar de modo exige privilégio. No TradeRock isso fica isolado no
  worker, iniciado via `pkexec` com uma política polkit própria. O app principal nunca roda como
  root.

## Requisitos

- Linux instalado de verdade (dual boot serve).
- .NET 10 SDK e Node 24.
- `libwebkit2gtk-4.1-0` e `libnotify4`, para a janela.
- `pkexec`. No Ubuntu 24.04: `sudo apt install pkexec`. `iw` e `ip` não precisam estar instalados:
  o worker traz os seus, compilados por `scripts/build-nettools.sh`.
- Um agente do polkit na sessão, que é o diálogo de senha. GNOME e KDE já trazem um.

## Rodar

```bash
scripts/dev-host.sh
```

O script:

1. compila o frontend e o app;
2. monta o worker com `scripts/build-worker.sh`:
   - o Python 3.12 do python-build-standalone, com o checksum conferido;
   - as dependências do motor, com hash fixo;
   - o frlg-ldn-trade com os patches de `engines/patches`;
   - `iw` e `ip` estáticos, compilados na primeira vez com `scripts/build-nettools.sh` (uns 2 minutos)
     e guardados em `~/.cache/traderock`;
3. instala o worker com `sudo`, se ele estiver ausente ou desatualizado:
   - em `/usr/local/lib/traderock-dev`, com dono root e sem escrita para outros usuários;
   - junto com a política `/usr/share/polkit-1/actions/io.github.traderock.worker.dev.policy`;
4. abre o app em modo desenvolvimento.

Opções:
- `--no-worker` pula os passos 2 e 3.
- `--uninstall-worker` remove o worker e a política.

## Roteiro de teste

1. **Configurações → Conexão.**
   - No item Chaves, envie o `prod.keys`.
   - No item Rádio, escolha a placa Wi-Fi.
2. **Componente de rede → Testar a placa.**
   - O sistema pede a senha de administrador, uma vez por sessão do app.
   - Durante uns 10 s, o Wi-Fi da placa cai e volta sozinho.
   - Anote o resultado por canal (enviados e recebidos).
3. **Componente de rede → Configurar o NetworkManager** (recomendado).
4. **Teste com o Switch → Procurar a sala**, com o Switch no Direct Corner como Leader, na tela de espera:
   - o teste escuta por até 30 s e para ao achar a sala; o Wi-Fi da placa cai e volta sozinho;
   - anote o resultado e abra **Detalhes** (canal, protocolo LDN, anúncios que não decifraram,
     canais que a placa não sintonizou);
   - repita com a sala fechada (deve dar "nenhuma sala") e, se tiver, com um prod.keys de outra
     versão (deve dar "encontrada, mas não decifrada");
   - confira que o Switch **não** recebe pedido de entrada durante o teste.
5. **Usar nas trocas de verdade** (no topo da lista, quando tudo passa) seleciona o motor da placa.
   Também dá em **Configurações → Avançado → Motor de troca → Placa Wi-Fi do PC**.
6. **Trocar**, com o Switch no Direct Corner como Leader:
   - aprove o pedido de entrada ("EMU", ou o nome que você escolheu);
   - sente na cadeira da esquerda;
   - troque;
   - cancele no menu de troca;
   - saia da sala.
7. Se algo falhar:
   - **Resumo → Exportar relatório.**
   - Log de diagnóstico: rode com `--Logging:LogLevel:TradeRock=Debug` para ver as linhas `worker:`,
     que são o log verboso do motor. Nenhum desses logs tem chaves.

## Testar os pacotes

Gere com `scripts/package-linux.sh` (ou baixe o artefato `traderock-linux` do CI).

- **.deb:** remova a instalação de desenvolvimento (`scripts/dev-host.sh --uninstall-worker`) e rode
  `sudo apt install ./dist/traderock_*.deb`. Abra pelo menu de aplicativos e siga o roteiro acima. O
  diálogo de senha deve citar "Rodar o componente de rede do TradeRock".
- **AppImage:**
  1. `chmod +x dist/TradeRock-*.AppImage` e abra com dois cliques ou pelo terminal (precisa de FUSE).
  2. Etapa 4 → **Instalar o componente de rede**. O diálogo de senha cita `/bin/sh`: é o script de
     instalação. Confira `/usr/local/lib/traderock` (dono root) e
     `/usr/share/polkit-1/actions/io.github.traderock.worker.appimage.policy`.
  3. Teste a placa: o diálogo agora cita o componente de rede.
  4. **Configurações → Remover o componente de rede** apaga os dois.
- `sudo scripts/smoke-linux.sh deb|appimage <pacote>` faz a parte automática sem Switch.

## Como as peças se encaixam

```
app (usuário) ──pkexec──▶ /usr/local/lib/traderock-dev/traderock-worker   (root)
     ▲   stdin/stdout JSON        │
     └────────────────────────────┤  python3 -I worker/run.py
                                  ├─ --child card   teste da placa (ldn.wlan)
                                  ├─ --child beacon teste com o Switch (ldn.Scanner, sem entrar na sala)
                                  └─ --child trade  uma tentativa de troca (frlgtrade.run_live)
```

- **Uma operação de rádio por vez.** Cada troca e cada teste roda num processo filho. O motor instala
  o próprio tratador de Ctrl+C, que só funciona na thread principal.
- **Cancelar:**
  1. primeiro SIGINT: o motor sai da sala e espera o Switch fechar (até 30 s);
  2. segundo SIGINT: encerra na hora (até 10 s);
  3. SIGKILL.

  Em todos os casos, o worker apaga as interfaces `ldn*` e devolve cada interface da placa ao estado
  anterior (ligada e gerenciada pelo NetworkManager, se era assim).
- **App fechado ou travado:** o stdin do worker fecha, e ele cancela, restaura o rádio e sai.
- **Arquivos:** `.pk3` e `prod.keys` chegam como bytes e ficam numa pasta temporária do root
  (permissão 700), apagada no fim. O Pokémon recebido volta como bytes no instante do commit.

## Testes sem hardware

- `python3 -m unittest discover -s worker/tests -t worker` cobre:
  - validação, rádio (com `/sys` falso) e mapeamento de eventos;
  - o wrapper contra um motor falso e o laço de protocolo;
  - o teste com o Switch (`beacon_child.run_scan`) com dublês de `trio` e da biblioteca LDN
    (`worker/tests/fake_ldn`): sala achada, chaves erradas, outro jogo, placa sem 5 GHz;
  - um teste que confere se cada frase do motor usada no mapeamento ainda existe no submódulo.
- `WorkerTests.cs` cobre a API de ponta a ponta com um worker falso: assistente, teste da placa,
  teste com o Switch (resultados, validade e cancelamento), troca, cancelamento e retry.

## Roteiro de teste com um ESP32 de verdade

Não precisa de root nem do worker instalado: o ESP32 conversa com o app pela serial. Basta o grupo
`dialout` (no Arch, `uucp`) e o runtime Python com o esptool (`scripts/build-worker.sh`).

1. Ligue a placa com um **cabo USB de dados**. No S3, use a porta do conversor USB-serial (UART); no
   C3, a USB nativa.
2. **Etapa 3:** escolha "ESP32 via USB". **Etapa 4:** marque o modelo da placa e a porta
   (`/dev/ttyUSB0` ou `/dev/ttyACM0`). O app faz o `LDN_HELLO` sozinho.
3. **Identificar a placa:** confira se o chip e a flash batem com o que você comprou.
4. **Gravar o firmware:** leva de 30 s a 2 min. Se a placa não entrar no modo de gravação, siga as
   instruções do botão BOOT. No fim, "O firmware do TradeRock respondeu" deve aparecer como OK.
5. **Etapa 6:** com o Switch no Direct Corner como Leader, faça o teste com o Switch.
6. Troque, com o motor "ESP32 via USB" selecionado em Configurações.
7. Se algo falhar, mande:
   - o relatório (Resumo → Exportar relatório);
   - o `native.log` da sessão, em `~/.local/share/traderock/logs/esp32/<sessão>/`, que não tem chaves.

## Roteiro de teste do RockAssistent (Entrega 20)

Nada disto precisa de root, placa ou Switch: dá para testar no WSL com `scripts/dev-host.sh --no-worker` ou com
`dotnet run --project app/TradeRock.App`. Antes, gere a Pokédex (`python3 -I scripts/build-dex.py`; o build falha sem
ela). As telas novas ainda não foram vistas num navegador: anote tudo o que parecer quebrado.

### Boxes com um save de emulador

1. Copie saves para uma pasta de teste (nunca use o único save que você tem). Formatos que o PKHeX.Core reconhece,
   de todas as gerações: `.sav` (mGBA, VBA, melonDS, Gen 1–3 e DS), `.dsv` (DeSmuME, com o rodapé), `.sa1`, `.bin` e o
   `main` dos jogos do Switch copiado pelo usuário. O app nunca leva o save de volta ao console.
2. **Configurações → Preferências → Pastas de saves** (só na janela do PC): adicione a pasta (caminho absoluto). O padrão
   vazio vale `**/*.sav;**/*.dsv;**/*.sa1;**/main;**/*.bin`; o app desce até 6 níveis e ignora arquivos acima de 16 MB.
3. **Boxes → Saves:** cada save aparece com jogo, treinador e caixas. Arquivo que não abre aparece na lista com o erro
   (`sav_unknown`, `sav_too_big`...) e o resto continua. Teste com um arquivo truncado e um de outro programa.
4. **Mover, copiar, anexar:** arraste do cofre para uma caixa do save (ou use "Mover para…"). Confira:
   - o backup em `backups/` na pasta de dados antes de cada gravação;
   - o save gravado abre de novo no emulador com o Pokémon no lugar;
   - de outra geração, o app converte (como o HOME) e avisa da legalidade; quando o PKHeX não converte, recusa com a
     explicação;
   - com o emulador aberto e salvando depois que o app leu o arquivo, a gravação dá "o save mudou" (`save_changed`) e
     nada é escrito;
   - numa pasta sem permissão de escrita, `save_unwritable` e o original intacto.
5. **Gen 1 e 2:** as caixas são compactadas ao gravar. Só dá para gravar até a primeira posição vazia, e tirar um
   Pokémon faz os seguintes subirem. Depois de gravar, a tela relê o arquivo.
6. **Anexar:** anexe um Pokémon do save ao cofre, jogue no emulador (suba um nível) e salve. A interface ainda não tem
   botão de sincronizar: chame `POST /api/boxes/sync-attachments` (pela janela local) e confira `updated` e o cofre
   atualizado.
7. Painel do Pokémon: Editar (construtor), Evoluir por troca (edição local, sem Switch), Clonar (só em saves),
   Exportar, Ver na Pokédex.

### Contador Hunt

1. **Contador Hunt → Nova hunt:** escolha um Pokémon, o jogo (só os jogos em que ele é obtível), o método e os
   modificadores (Amuleto, sanduíche, nível de pesquisa...). Confira a odd: Masuda no SV com Amuleto = 1/512,44.
2. Na hunt aberta: + e − pelos botões e pelos atalhos da janela (Configurações → Preferências → Atalhos; padrão Espaço
   para somar). Digitar num campo de texto não soma. Tab, Enter e Escape não podem ser atalho.
3. "Achei!" fecha a hunt e ela vai para o **Histórico**.

### Atalho global do Contador Hunt

Os testes automáticos cobrem o combo, a tabela de teclas, a API e o SSE com um atalho falso. No WSLg (XWayland) o
`XGrabKey` e o `BadAccess` de outra conexão foram verificados; a tecla apertada de verdade, não. O atalho global só fica
ativo (tecla pega no sistema) enquanto um contador de hunt está aberto, e teclas de texto sem Ctrl/Alt/Meta (letras,
dígitos, Espaço, Enter, Tab...) são recusadas no global. Para conferir à mão:

1. **Linux com X11** (sessão "Xorg"/X11, não Wayland): abra o app, Configurações → Preferências → ligue "Atalho global".
   Crie uma hunt, abra o contador e ponha outra janela na frente (um editor de texto, o emulador). Aperte o atalho de
   somar (escolha antes uma tecla aceita no global, como F8 ou Ctrl+Espaço): o contador sobe uma vez por aperto (segurar a tecla não repete). Com a janela do app na frente,
   conta uma vez só. Com NumLock e CapsLock ligados, continua funcionando.
2. **Tecla em uso:** com outro programa que pegue a mesma tecla global (ex.: `xbindkeys` com `F8`), escolha F8 → o app
   mostra "Outro programa já usa essa tecla" e o atalho anterior continua.
3. **Wayland** (sessão GNOME/KDE padrão): o interruptor fica desligado com o aviso "No Wayland o sistema não permite".
4. **Windows** (janela com WebView2 e também o modo "só navegador", sem WebView2): ligue o atalho, deixe o Bloco de
   Notas na frente e aperte a tecla: o contador sobe e o Bloco de Notas não recebe a tecla. Fechar o app solta a tecla
   (ela volta a funcionar no Bloco de Notas).
5. **Modo servidor:** pelo celular/navegador da rede, o interruptor não aparece; o PUT com `global=true` dá 403.

### Nuzlocke

1. **Nuzlocke → Nova run:** jogo, versão (obrigatória nos jogos com duas versões, como FireRed/LeafGreen ou
   Sword/Shield), nome e regras.
2. Tela da run: próximo chefe com o time e o level cap; área atual com os encontros (com a chance, nos jogos do
   PokeAPI) e os treinadores (só nos jogos de GBA e DS; nos do Switch aparece o aviso); "Capturei", "Fugiu/desmaiou",
   "Sem encontro". Teste a cláusula de espécie e o aviso de quem passou do cap.
3. **Mapa:** os nós mudam de cor (visitado, capturado, perdido); clicar num nó vai para a área. Áreas sem nó (prédios,
   Grand Underground) continuam na lista.
4. Equipe: morte com causa, caixa, evolução, golpes. Termine como "Campeão" ou "Wipe" e confira o Histórico.

### Guias

- `python3 -I scripts/check-guides.py --strict`: todos os 22 jogos com as 9 seções, nos dois idiomas, e todas as
  diretivas (`:::bosses`, `:::boss`, `:::encounters`, `:::tms`, ...) apontando para dados que existem. O `ci.sh` roda
  sem o `--strict`.
- No app, **Guias**: abra um jogo, navegue pelo sumário e por anterior/próxima, e confira que os blocos de chefes,
  encontros e TMs carregam.

### Internet opcional (D32)

1. **Configurações → Preferências → Internet opcional:** as duas chaves começam desligadas. Pela rede (modo
   servidor), o cartão só mostra o estado.
2. Com tudo desligado: **Guias → VGC → Times sugeridos → Mais usados** mostra só o aviso, e "Abrir link" em **Minha
   equipe** não aparece; nenhuma conexão sai do PC (confira com `ss -tp` ou o monitor de rede).
3. Ligue "Estatísticas do Smogon" e clique em **Atualizar**: baixa o arquivo do mês anterior (~15 MB) para
   `vgc/` na pasta de dados e mostra os 50 mais usados. Sem rede, aparece o erro e o arquivo anterior continua.
4. Ligue "pokepast.es" e cole um link `https://pokepast.es/<id>` em **Minha equipe → Abrir link**: o texto do time
   preenche o campo. Outro endereço é recusado sem conexão.
5. Os Logs registram só "download iniciado/concluído" e o código do erro, sem o endereço do paste.
