## Qual arquivo baixar

O RockAssistent (antes chamado TradeRock) é um assistente Pokémon para o computador, que funciona sem internet, de
todos os jogos da série principal, e continua trocando Pokémon com todos os jogos do Nintendo Switch que têm troca
local.

- **Pokédex offline** com espécies, formas, golpes, habilidades, itens e onde encontrar cada Pokémon.
- **Guias** dos 22 jogos: detonado, chefes com os times, TMs, trocas, lendários, pós-jogo, mecânicas e VGC.
- **Boxes**: o cofre e os saves das pastas que você escolher, com backup automático antes de mudar um save.
- **Contador Hunt** de shiny com as chances de cada método e um atalho de teclado opcional.
- **Nuzlocke** com as regras, o mapa da região e o limite de nível do próximo chefe.
- **VGC**: times do formato atual, análise do time e estatísticas do Smogon (só se você ligar a internet opcional).
- **Histórico** de trocas, Mystery Gift, raids e Union Circle numa lista só.
- **Internet opcional**, desligada por padrão: estatísticas do Smogon e links do pokepast.es.

**Ainda não testado de verdade:** as telas novas num navegador, o atalho de teclado com uma tecla apertada de verdade
(Windows e Linux com X11) e saves reais de emulador nas Boxes. Faça uma cópia dos seus saves antes de mexer neles.

| Seu computador | Arquivo | Como instalar |
| --- | --- | --- |
| **Windows 10 ou 11** (64 bits) | `RockAssistent-Setup-{{VERSION}}.exe` | Abra e siga os passos. Veja o aviso abaixo. |
| **Ubuntu, Debian** e derivados | `traderock_{{DEB_VERSION}}_amd64.deb` | `sudo apt install ./traderock_{{DEB_VERSION}}_amd64.deb` |
| **Outras distribuições Linux** | `RockAssistent-{{VERSION}}-x86_64.AppImage` | `chmod +x` no arquivo e abra |
| **Mac com chip Apple** (M1 ou mais novo, macOS 14 ou mais novo) | `RockAssistent-{{VERSION}}-macOS-arm64.dmg` | Abra e arraste o RockAssistent para Aplicativos. Veja o aviso abaixo. |

- **Windows:** o instalador ainda não tem assinatura digital, então o Windows mostra "O Windows
  protegeu o computador". Clique em **Mais informações → Executar assim mesmo**. No Windows, o rádio é
  sempre um **ESP32** ligado no USB.
- **Linux:** o rádio pode ser a placa Wi-Fi do próprio PC ou um ESP32.
- **Mac:** o app ainda não tem a assinatura da Apple, então o macOS bloqueia a primeira abertura ("A Apple não pôde
  verificar…"). Clique em **OK**, abra **Ajustes do Sistema → Privacidade e Segurança**, desça até o aviso do
  RockAssistent e clique em **Abrir Mesmo Assim** (o Mac pede a sua senha). Isso só é preciso uma vez. Abra o app
  pela pasta **Aplicativos**, não de dentro do disco, para ele conseguir se atualizar sozinho. No Mac, o rádio é
  sempre um **ESP32** ligado no USB, e o atalho global do Contador Hunt ainda não funciona (use o atalho da janela).
  O arquivo do Mac entra na lista alguns minutos depois dos outros: se ele ainda não apareceu, volte daqui a pouco.
- **Tem o TradeRock instalado?** As versões antigas, de antes da troca de nome, não acham mais a atualização sozinhas.
  Baixe o RockAssistent uma vez por esta página e instale por cima: seus Pokémon, contas e chaves continuam. Depois
  disso, ele volta a se atualizar sozinho.
- **Para atualizar,** use a tela **Sobre** do app (ele também avisa no Início quando há versão nova), ou baixe a versão nova e instale por cima. Seus Pokémon, contas e chaves continuam.
- **Conferir o download (opcional):** compare com o `SHA256SUMS`. No Linux, rode
  `sha256sum -c SHA256SUMS --ignore-missing`. No Windows, rode
  `Get-FileHash RockAssistent-Setup-{{VERSION}}.exe` no PowerShell. No Mac, rode
  `grep macOS SHA256SUMS | shasum -a 256 -c` no Terminal, na pasta dos downloads.

Nunca distribuímos prod.keys nem ROMs: use só as chaves extraídas do seu próprio Switch. O
código-fonte completo (AGPL-3.0) está nos arquivos `.tar.gz` deste Release.
