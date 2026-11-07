# RockAssistent: design do rebranding e das novas áreas

Data: 05/11/2026. Autor da decisão: Matheus Rocha. Escrito a partir da conversa de brainstorming.

## 1. Intenção e porquê

O TradeRock nasceu para trocar Pokémon entre o PC e o Switch. O autor quer que a troca vire **só mais uma
ferramenta** de um assistente Pokémon de todos os jogos da série principal, que ele usa no dia a dia: começar um
save novo, jogar um Nuzlocke, consultar um Pokémon, trocar consigo mesmo para evoluir, Mystery Gift, contar uma
shiny hunt. O nome visível passa a ser **RockAssistent**.

Referências que o autor gosta:

- **Serebii**, pela página de detalhes de cada Pokémon, filtrada por jogo.
- **allgenwiki.com**, pela cobertura (Pokédex, guias de jogo, líderes, TMs, guias práticos, mitologia), mas
  achou confuso (página única cheia de âncoras, seções que se sobrepõem).
- **PKVault**, pelas Boxes: saves achados por pastas configuradas, cofre fora dos saves, mover entre eles.

Público: o próprio autor e amigos, leigos. Tudo em português e inglês, explicado de forma simples, intuitivo.

## 2. Metas, não metas e antimetas

**Metas**

- A troca com o Switch continua funcionando exatamente como hoje.
- Tudo funciona **offline**. Os dados (Pokédex, guias, times de chefes, métodos de shiny, mapas) vão embutidos
  no instalador. Nada é buscado em API em tempo de uso.
- Entregar tudo de uma vez: menu novo, Pokédex, Guias de todos os jogos, Boxes com saves por pasta, Nuzlocke,
  Contador Hunt, Histórico unificado, rebranding visível.
- Visual e tema atuais (visual A claro/escuro, tokens de `src/index.css`, componentes de `ui.tsx`) se mantêm.
  Os mockups do brainstorming eram só esqueleto.
- Sprites dos Pokémon em todo lugar onde um Pokémon aparece.

**Não metas**

- Renomear namespaces, pastas, scripts ou pacotes NuGet internos (`TradeRock.*` fica). O rebranding é só na
  parte visível.
- Buscar dados de API em tempo de uso.
- Reproduzir texto ou tabelas do Serebii, do allgenwiki ou de qualquer site sem licença.
- Gravar saves do Switch (o app não extrai nem injeta save no console).

**Antimetas** (seria um fracasso mesmo funcionando)

- Quebrar a troca ou o modo servidor que já funcionam.
- Ficar confuso como o allgenwiki: menus que se sobrepõem, página única com âncoras, sem ponto de entrada claro.
- Exigir internet para o uso normal.
- Vazar prod.keys ou dados do PC (regras de segurança do CLAUDE.md continuam valendo).

## 3. Restrições

- Licenças de dados (verificado em 05/11/2026):
  - **PokeAPI**: BSD-3. Os CSVs de `data/v2/csv` podem ser embutidos. É a fonte de Pokédex, golpes, habilidades,
    itens, encontros por versão e método, evoluções, textos por jogo, grupos de ovo, tipos.
  - **Serebii**: "© Serebii.net", sem permissão de reuso. Só serve de referência de **estrutura**.
  - **allgenwiki**: sem licença publicada (todos os direitos reservados por padrão). Os números dele são da
    PokeAPI; o texto não pode ser copiado.
  - **nuzlocke.app**: BSD-3 (código). **nuzlocke.data**: sem licença. Serve para conferir cobertura, não para copiar.
  - **pret** (decompilações, FRLG/Emerald/HGSS/Platinum/BW...): sem licença declarada. Os times de treinadores
    são fatos do jogo; o app embute uma tabela **própria** montada e conferida a partir deles, em formato nosso.
  - **Smogon stats**: sem licença escrita, arquivos públicos mensais. Uso só com opção ligada pelo usuário.
  - **Showdown (servidor)**: MIT. O formato de time (`sim/TEAMS.md`) é o que o construtor já lê.
  - Pikalytics, Victory Road, VR Pastes: todos os direitos reservados. Não usar.
- O app principal nunca roda como root; o worker continua isolado (D15).
- A única consulta à internet continua sendo a procura de atualização (D29), mais as duas opções **desligadas
  por padrão** descritas em 4.10 (estatísticas do Smogon e abrir link do pokepast.es) e, se vier a ser preciso,
  o download de conteúdo extra (4.11). Nenhuma delas manda nada do PC.
- Tamanho do pacote: hoje os sprites ocupam 4,2 MB. Os CSVs da PokeAPI necessários ficam na casa das dezenas de
  MB; o construtor decide o formato embutido (ver 6).

## 4. O que foi decidido

### 4.1 Menu principal

Ordem fixa, no mesmo layout de hoje (lateral no PC, inferior no celular):

1. **Início**
2. **Pokédex**
3. **Guias** (sub-seções: Jogos, Mecânicas, VGC Champions, Mitologia)
4. **Boxes** (abas: Saves, Cofre do app)
5. **Trocas** (troca, raids, Union Circle)
6. **Mystery Gift**
7. **Nuzlocke**
8. **Contador Hunt**
9. **Histórico**
10. **Configurações**
11. **Sobre**

Mapeamento do que existe hoje:

| Hoje | Vai para |
| --- | --- |
| `/trocar` | Trocas |
| `/pokemon` (cofre) | Boxes → aba Cofre do app |
| `/criar` (construtor) | Boxes: botão "Editar" no painel do Pokémon e "+ Novo" na caixa |
| `/atividades` (gifts, raids, Union Circle) | Mystery Gift (gifts) e Trocas (raids e Union Circle) |
| `/historico` | Histórico (agora com tudo) |
| Configurações → Conexão | Fica igual; ganha "Pastas de saves", "Atalhos" e "Internet opcional" |

Rotas antigas redirecionam. No modo servidor (celular), valem as regras de hoje: o que é "só na janela do PC"
continua assim (raids, Union Circle, Mystery Gift, configuração de conexão). Pokédex, Guias, Contador Hunt,
Nuzlocke e Boxes (cofre da conta) ficam disponíveis pelo celular. Boxes → Saves é só no PC, porque lê o disco.

### 4.2 Pokédex

Lista com busca, filtro por tipo, geração, jogo e "tenho nos meus saves/cofre". Página do Pokémon em **abas**
(escolha do autor entre dois mockups), com estrutura inspirada no Serebii:

- Cabeçalho: sprite normal e shiny do jogo escolhido, número nacional e regional, nome em outras línguas
  (JP, FR, DE, KR, ES, IT), classificação, tipos, altura, peso, proporção de sexo, taxa de captura, passos do ovo.
- **Seletor de jogo** no topo (RSE, FRLG, DPPt, HGSS, BW, B2W2, XY, ORAS, SM, USUM, LGPE, SwSh, BDSP, PLA, SV, Z-A,
  Champions). Trocar o jogo troca todos os dados da página. Jogos em que o Pokémon não existe aparecem apagados.
- Formas (Mega, Gigantamax, regionais, Alpha) como sub-abas do sprite.
- Abas: **Geral** (habilidades com descrição, cadeia evolutiva com condição, dano recebido 18 tipos, texto da
  Pokédex do jogo, item segurado, cor, grupos de ovo), **Onde encontrar** (por jogo do grupo escolhido: local,
  método, chance, níveis, condições; "evoluir", "troca", "evento" quando não há encontro), **Golpes** (nível, TM/HM,
  tutor, ovo, com tipo, categoria, força, precisão, PP, efeito), **Stats** (base, total, barras, faixas Nv 50 e
  100 por natureza, EVs que dá, experiência, felicidade), **Ovo** (grupos, passos, golpes de ovo), **Nos seus
  saves** (onde ele está: save, caixa, nível, shiny; link para o Boxes).
- Páginas secundárias: golpes, habilidades e itens (a tabela de golpes linka para elas), tabela de tipos.
- Nomes de golpes e itens em inglês; o resto traduzido, como hoje. Textos da Pokédex em pt-BR quando a PokeAPI
  tem (há poucos), senão em inglês com aviso discreto.

### 4.3 Guias

Quatro sub-seções, cada uma com um índice claro (nada de página única com âncoras):

- **Jogos**: um guia por jogo ou dupla (RB/Y, GSC, RSE, FRLG, DPPt, HGSS, BW, B2W2, XY, ORAS, SM, USUM, LGPE, SwSh,
  BDSP, PLA, SV, Z-A). Cada um com: visão geral e diferenças de versão, walkthrough por trecho (cidade/rota, o
  que fazer, itens importantes, treinadores notáveis), líderes de ginásio e Elite Four (times com níveis e
  golpes), rivais e equipe vilã, TMs/HMs e onde achar, tutores, exclusivos de versão e inobtíveis, trocas
  in-game, eventos e presentes, pós-jogo (Ilhas Sevii, Battle Frontier, etc.), lendários.
- **Mecânicas**: natures, EVs e IVs, breeding, shiny, tabela de tipos, evoluções especiais, felicidade,
  habilidades, itens segurados, concursos, Safari, bases secretas, Dynamax/Tera/Mega/Z, e o que mais couber.
- **VGC Champions**: "Começando no VGC", "Regulamentação atual" (com data), "Times sugeridos" (6 a 10 arquétipos
  escritos por nós, cada um com os seis sets completos em formato Showdown, ideia do time, como jogar,
  perigos; botão copiar o paste e importar pro Boxes), "Minha equipe" (monta do cofre ou do zero; fraquezas,
  cobertura, velocidade), "Abrir um paste" (cola o texto ou um link do pokepast.es, ver 4.10).
- **Mitologia**: lore por região e por lendário, escrita por nós.

Fontes: as partes estruturadas (times de chefes, encontros, TMs, trocas in-game) vêm dos dados embutidos; o texto
corrido é markdown nosso em `content/{pt-BR,en}/guides/**`, como os textos de hoje. Nada copiado de site.

**Estilo (05/11/2026):** rápido de ler, como o allgenwiki, só fatos importantes: listas curtas e tabelas por
trecho, sem narrativa. O autor pode enviar detonados em PDF; os fatos deles vão para notas em
`docs/superpowers/sources/*.md` (nunca a prosa) e servem de fonte para os dados e os guias.

### 4.4 Boxes

Duas abas:

- **Saves**: lista de saves achados nas pastas configuradas (Configurações → Pastas de saves, com padrões tipo
  `~/saves/**/*.sav`), lidos pelo PKHeX.Core (todas as gerações que ele lê, inclusive os formatos de Switch,
  como já faz o `Saves/`). Cada save mostra as caixas (e o time) com 30 posições e sprites. **Mover** um Pokémon
  para uma caixa de save grava o `.sav` com backup automático (a injeção com backup já existe). Saves do Switch
  copiados pelo usuário aparecem e podem ser gravados do mesmo jeito; o app não os leva de volta ao console.
- **Cofre do app**: o cofre de hoje (caixas de 30, Caixa de entrada, Lixeira, tags, filtro por jogo).

Entre as abas: arrastar ou "Mover para…" com três ações: **Mover**, **Copiar**, **Anexar** (cópia no cofre que
acompanha o save: ao reler o save, a cópia é atualizada). Painel do Pokémon à direita com legalidade, stats,
IVs/EVs, golpes, tags e ações: Editar (abre o construtor de hoje), Evoluir por troca, Clonar, Exportar, Ver na
Pokédex, Trocar, Mover. "+ Novo" na caixa abre o construtor em branco. Sem seção de "arquivos soltos" (o autor
não quis); importar `.pk*` continua pelo arrastar global de hoje.

### 4.5 Trocas, Mystery Gift

Sem mudança de comportamento. Só reorganização de telas: Trocas junta o fluxo de hoje com as raids e o Union
Circle (cartões por jogo); Mystery Gift vira item próprio com os gifts do FRLG e do SwSh.

### 4.6 Nuzlocke

- **Criar run**: jogo, nome, regras. Padrão: primeiro encontro de cada área, apelido obrigatório, desmaiou
  morreu. Opções: cláusula de espécie (repetido não conta), cláusula de shiny (sempre pode pegar), cláusula de
  duplicata por linha evolutiva, level cap obrigatório, sem itens em batalha, sem Pokémon Center limitado (só
  registro). Regras customizadas em texto livre.
- **Tela da run** (trecho atual):
  - **Próximo chefe** com time (Pokémon, nível, golpes, habilidade, item) e **level cap** em destaque. Aviso
    quando um membro passa do cap.
  - **Área atual** com os encontros possíveis (da PokeAPI, por versão, método, chance) e o botão "capturei" que
    registra o Pokémon daquela área (ou "fugiu/desmaiou/sem encontro").
  - **Equipe** com sprites, nível, golpes atuais, próximos golpes por nível, evolução e golpes da evolução.
  - **Treinadores da área** com níveis e golpes nos jogos com fonte (GBA e DS); nos de Switch só os chefes,
    com aviso "treinadores comuns não disponíveis neste jogo".
  - **Mapa esquemático em SVG**: cidades e rotas como nós ligados por linhas, colorido por estado (não
    visitado, visitado, capturado, perdido). Clicar num nó vai para a área. Mesmo estilo das ilustrações SVG do
    app. Um mapa por região (Kanto, Johto, Hoenn, Sinnoh, Unova, Kalos, Alola, Galar, Hisui, Paldea, Lumiose).
  - Registro manual: morte (com causa), caixa, troca de membro, evolução, apelido.
  - Cemitério, caixa e estatísticas da run.
- **Fim**: "Campeão" ou "Wipe". Vai para o Histórico com resumo.
- Várias runs ao mesmo tempo, pausáveis.

### 4.7 Contador Hunt

- **Criar hunt**: escolhe o Pokémon → o app mostra os jogos em que ele é obtível (encontro, ovo, evolução,
  presente, com a origem) → escolhe o jogo → escolhe o **método** daquele jogo → marca modificadores (Shiny
  Charm, Oval Charm onde conta, combo/chain, nível de pesquisa, sanduíche, etc.).
- **Tabela de métodos e odds por jogo**, embutida e nossa (`assets/data/shiny-methods.json`). Cobertura mínima:
  Gen 2 (encontro 1/8192, Odd Egg do Crystal, breeding com shiny 1/64), Gen 3 e 4 (encontro, reset; Masuda no DPPt
  e HGSS; Poké Radar; Chain fishing não), Gen 5 (Masuda; Charm no B2W2), Gen 6 (encontro, Masuda, Poké Radar,
  Chain fishing, Friend Safari, horda, DexNav no ORAS), Gen 7 (SOS, Ultra Wormhole, Masuda), LGPE (catch combo,
  Lure, Charm), SwSh (encontro com número de batalhas, brilliant aura, Masuda, Dynamax Adventure), BDSP (Radar,
  Grand Underground, Masuda), PLA (outbreak, massive outbreak, nível de pesquisa 10 e perfeito, Charm), SV
  (encontro, sandwich níveis 1 a 3, outbreak, Masuda, Charm), Z-A (o que estiver documentado). Cada entrada:
  nome, descrição curta, odd base, odds por modificador, observações.
- **Contador**: número grande, odd atual, chance acumulada ("você já tinha X% de ter achado"), tempo
  decorrido, botões + e −, "achei!" (fecha a hunt, pede foto opcional não, só registra; vai para o Histórico;
  oferece marcar no Boxes se um shiny da espécie aparecer num save). Várias hunts, pausáveis.
- **Atalhos de teclado**: configuráveis em Configurações → Atalhos (somar e diminuir). Padrão: funcionam com a
  janela na frente. Opção "atalho global" (com o app atrás, para quem joga no emulador): Windows e Linux com
  X11; no Wayland o app avisa que o sistema não permite. (Decisão do builder confirmada pelo autor como
  "opcional, interessante".)

### 4.8 Histórico

Linha do tempo única com filtros por tipo: trocas (como hoje), raids, Mystery Gift, Union Circle, hunts
terminadas (Pokémon, jogo, método, contagem, tempo), runs de Nuzlocke terminadas (jogo, resultado, equipe final,
mortes). Cada item abre o detalhe. Exportar continua como hoje, por tipo.

### 4.9 Início

Cartões: estado da conexão (como hoje), "Em andamento" (hunts e runs ativas, com atalho para continuar),
última troca, Mystery Gift ativos, "Guia do dia" (um guia aleatório), busca rápida da Pokédex.

### 4.10 Internet opcional (desligada por padrão)

Em Configurações → "Internet opcional", cada uma com explicação:

- **Estatísticas de uso do Smogon**: baixa o arquivo mensal do formato do Champions (`chaos/*championsvgc*`)
  e mostra na seção VGC os Pokémon, golpes, itens, habilidades e parceiros mais usados. Guarda o arquivo na
  pasta de dados com a data. Sem isso, a seção mostra só os times embutidos.
- **Abrir link do pokepast.es**: permite colar um link além do texto. Sem isso, só texto colado.

Nenhuma manda dados do PC. As duas entram em `docs/decisions.md` como D32.

### 4.11 Conteúdo extra (só se precisar)

Tudo vai embutido por padrão. Se o pacote crescer além do razoável (sprites shiny por jogo, formas, sons), o
builder pode separar esses extras em pacotes publicados no mesmo Release do GitHub, baixados por uma opção em
Configurações → "Conteúdo extra", com SHA256 conferido como na atualização (D29). O básico sempre funciona sem
baixar nada. Decisão nossa; o autor pediu só que o uso offline seja garantido.

### 4.12 Rebranding visível

- Nome na janela, título, Início, Sobre, instalador (Inno Setup), `.deb`, AppImage, atalho e ícone, nome do
  Release, `packaging/release-notes.md`, README.
- Ícone novo (SVG, mesmo estilo das ilustrações; o builder propõe e o autor aprova antes de fechar).
- Pasta de dados: o builder decide entre manter a pasta atual ou criar `RockAssistent` com **migração
  automática** dos dados, cofre, logs e prod.keys (permissão 600 mantida) na primeira abertura, sem perder nada.
- Repositório no GitHub: o autor renomeia quando quiser; o app procura atualização pela URL nova com
  redirecionamento da antiga (o GitHub redireciona renomeações).
- Namespaces, pastas, scripts, nomes de pacotes NuGet e o nome do worker ficam `TradeRock`.

## 5. Dados embutidos (resumo)

| Dado | Fonte | Formato no app |
| --- | --- | --- |
| Pokédex, golpes, habilidades, itens, encontros, evoluções, textos, tipos | PokeAPI CSV (commit fixo, BSD-3) | SQLite pré-gerado ou JSON por entidade, decisão do builder; gerado por script em `scripts/` |
| Sprites | PokeAPI (já embutidos) + shiny e formas | PNG em `assets/sprites` |
| Times de chefes de todos os jogos | Montado por nós, conferido com as decompilações e com a memória do builder | `assets/data/bosses/<jogo>.json` |
| Treinadores comuns (GBA/DS) | Montado por nós a partir das decompilações | `assets/data/trainers/<jogo>.json` |
| Métodos e odds de shiny | Nosso | `assets/data/shiny-methods.json` |
| Mapas esquemáticos | Nosso | SVG por região em `frontend/src/assets/maps` |
| Guias, mecânicas, mitologia, times VGC | Nosso | `content/{pt-BR,en}/guides/**` |
| Regulamentação VGC vigente | Nosso, com data | `content/.../guides/vgc/regulation.md` |

## 6. O que fica com o builder

- Formato de embutir a PokeAPI (SQLite separado vs JSON) e como o backend serve (endpoints `/api/dex/...`).
- Modelo de dados das runs e hunts no SQLite do app (EF Core, migrações), com escopo por conta no modo servidor.
- Como o "anexar" acompanha o save (hash do save, identidade do Pokémon por PID/EC/OT).
- Como ler o `.sav` da pasta com segurança (nunca gravar sem backup; detectar save aberto pelo emulador).
- Implementação do atalho global por plataforma.
- Como são escritos e validados os guias (um script que confere que todo jogo tem todas as seções, que todo
  chefe citado existe nos dados, etc.).
- Ordem das entregas. Sugestão: (1) menu e rebranding, (2) dados embutidos e Pokédex, (3) Boxes, (4) Contador
  Hunt, (5) Nuzlocke, (6) Guias, (7) Histórico e Início, (8) VGC e internet opcional. Cada uma na própria
  branch, com PR, como manda o CLAUDE.md.

## 7. Decisões tomadas depois da revisão do builder (decisões minhas, o autor estava fora)

1. **Encontros nos jogos de Switch.** A PokeAPI cobre bem até a Gen 7 e pouco ou nada de SwSh, BDSP, PLA, SV
   e Z-A. Onde a PokeAPI tem, usa-se a PokeAPI. Para os jogos de Switch o app embute tabelas nossas
   (`assets/data/encounters/<jogo>.json`) no nível "área → espécies, método e faixa de nível", sem chance
   exata. Os agentes **pesquisam na internet** para montar e conferir (fatos do jogo são públicos e não
   pertencem a site nenhum), sempre escrevendo no nosso formato. Só o que não puder ser conferido em pelo
   menos uma fonte leva "não conferido" (como o catálogo faz com "não testado"). Assim o Nuzlocke e o Hunt
   funcionam em todos os jogos. (Orientação do autor em 05/11/2026.)
2. **Profundidade mínima dos guias.** Em todos os jogos: chefes com níveis, golpes e level cap completos;
   walkthrough por trecho (o que fazer, itens-chave, chefe do trecho) curto; TMs, trocas in-game,
   exclusivos, lendários e pós-jogo. Nos jogos com decompilação (GBA e DS) o walkthrough é mais detalhado. O
   validador exige todas as seções em todos os jogos, mas não um tamanho mínimo. Dados (níveis, times, locais)
   podem ser conferidos na internet; texto corrido nunca é copiado de site. O que não foi conferido leva a
   marca "não conferido" nos dados e na interface.
3. **Gen 1 e 2 entram** na Pokédex (seletor começa em RB/Y e GSC, a PokeAPI cobre), no Nuzlocke, no Hunt e nos
   Guias. Mapas: um SVG por região, com nós que podem estar ausentes conforme o jogo (Kanto de RBY, FRLG e
   HGSS compartilham a base). No Boxes, saves de Gen 1/2 aparecem e recebem Pokémon só quando o PKHeX.Core
   consegue converter; senão a ação é recusada com explicação.
4. **"Champions" sai do seletor da Pokédex.** A elegibilidade no formato atual aparece na seção VGC, sobre os
   dados de SV. O catálogo de jogos não ganha o Champions.
5. **Mover entre gerações no Boxes é livre, com aviso de legalidade**, igual à regra dos jogos novos (D22). Só é
   recusado quando o PKHeX.Core não converte o formato.
6. **"Evoluir por troca" é edição local** via PKHeX (cofre e saves de emulador). Não dispara troca com o
   Switch.
7. **Grafia: "RockAssistent"**, como o autor escreveu. Vai assim para o AppId, pacotes, pasta de dados e
   Release. Se ele quiser "RockAssistant", muda antes do primeiro release com o nome novo.

## 8. Aberto para o autor

O ícone, a grafia do nome e o texto dos guias passam pela aprovação dele no PR.
