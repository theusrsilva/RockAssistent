# Roteiro de teste no Mac

Este roteiro é para quem vai testar o RockAssistent num Mac pela primeira vez. Siga na ordem e anote o que
aconteceu em cada passo: "funcionou", ou o que apareceu na tela (um print ajuda muito).

**Precisa de:** um Mac com chip Apple (M1, M2, M3, M4…) e macOS 14 (Sonoma) ou mais novo. Para os passos da troca,
também de um ESP32 compatível, um cabo USB que passe dados e o `prod.keys` extraído do seu próprio Switch.

## 1. Instalar

1. Baixe o `RockAssistent-<versão>-macOS-arm64.dmg` da página de Releases.
2. Abra o `.dmg` e arraste o **RockAssistent** para o atalho **Aplicativos**.
3. Abra o RockAssistent pela pasta **Aplicativos**.
4. O macOS deve bloquear ("A Apple não pôde verificar…"). Clique em **OK**.
5. Abra **Ajustes do Sistema → Privacidade e Segurança**, desça até o aviso do RockAssistent e clique em
   **Abrir Mesmo Assim**. Confirme com a sua senha.

Anote: a mensagem do passo 4 foi essa ou "está danificado"? O passo 5 resolveu?

## 2. Janela e telas

1. A janela abre com o RockAssistent? O ícone aparece no Dock?
2. Passe por **Início, Pokédex, Guias, Boxes, Contador Hunt, Nuzlocke, Histórico, Configurações e Sobre**. Alguma
   tela ficou em branco, cortada ou com erro?
3. Em **Sobre**, aparece "macOS" e a versão certa?
4. Em **Boxes**, escolha uma pasta de saves (botão de abrir pasta). A janela do Finder para escolher abriu?
5. Em **Boxes**, exporte um Pokémon do cofre. A janela de salvar abriu e o arquivo apareceu onde você escolheu?
6. Feche e abra de novo: o que você fez continua lá?

## 3. Contador Hunt

1. Abra um contador e use o atalho da janela (Espaço, ou a tecla configurada). Conta?
2. Em **Configurações → Preferências**, o atalho global deve dizer que ainda não está disponível no Mac.

## 4. ESP32 e troca

1. Ligue o ESP32 no USB e abra **Configurações → Conexão**.
2. O item do sistema diz "Mac: as trocas usam um ESP32 via USB"?
3. A porta do ESP32 aparece na lista (`/dev/cu.usbmodem…` ou `/dev/cu.usbserial…`), com o nome da placa?
4. Grave o firmware pelo app. Funcionou? Quanto tempo levou?
5. Envie o `prod.keys` e rode o **teste com o Switch**.
6. Faça uma troca com um jogo do Switch. Qual jogo? Funcionou do começo ao fim?
7. Se tiver o Sword/Shield, teste a troca pela rede de casa (modo LAN). O Mac perguntou sobre a rede local? O que
   você respondeu?

## 5. Modo servidor (celular)

1. Em **Configurações → Avançado**, ligue o modo servidor e leia o QR code com o celular na mesma rede Wi-Fi.
2. O celular abriu o RockAssistent? Deu para entrar com uma conta e ver o cofre?
3. O Mac perguntou se o app pode aceitar conexões ou acessar a rede local? O que você respondeu?

## 6. Atualização

Só quando sair uma versão nova depois da que você instalou:

1. Em **Sobre**, clique em atualizar.
2. O app baixou, fechou e abriu de novo sozinho, já na versão nova?
3. O macOS bloqueou de novo na abertura?

## Se algo der errado

- Em **Configurações → Logs** (aba ao lado de Avançado) há o registro do dia. Mande o arquivo junto com o relato
  (ele não tem chaves nem dados dos seus Pokémon).
- Os dados ficam em `~/Library/Application Support/TradeRock`. Para começar do zero, feche o app e apague essa pasta.
- Para desinstalar, arraste o RockAssistent de Aplicativos para o Lixo.
