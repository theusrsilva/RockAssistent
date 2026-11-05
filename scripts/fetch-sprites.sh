#!/usr/bin/env bash
# Baixa os sprites do PokeAPI/sprites num commit fixo para assets/sprites e confere com assets/sprites/SHA256SUMS:
#   <n>.png                espécies 1–1025 (os 1–386 vieram do easyworld e já estão no repositório; baixa 387–1025)
#   shiny/<n>.png          versão shiny das espécies 1–1025
#   forms/<id>.png         formas alternativas (Mega, Gigantamax, regionais...) pelo id do pokemon no PokeAPI (> 10000)
#   forms/shiny/<id>.png   a versão shiny delas
# Roda só quando o conjunto muda (o resultado fica no repositório); o app nunca baixa nada (D17).
#   scripts/fetch-sprites.sh                    baixa o que o SHA256SUMS lista e falta, e confere
#   scripts/fetch-sprites.sh --shiny --forms    também procura shiny e formas que ainda não estão aqui e regrava o SHA256SUMS
#   scripts/fetch-sprites.sh --update           baixa tudo de novo e regrava o SHA256SUMS (ao trocar o commit)
# As formas sem arquivo no PokeAPI (404) são puladas e listadas no fim; o app usa o sprite da espécie no lugar.
set -euo pipefail
cd "$(dirname "$0")/.."

COMMIT=1dd55c0436d2a5b6e324c0b780e1b1b416fbfb64      # PokeAPI/sprites, 2026-10-28
DATA_COMMIT=6ea97b5641faa12e090866e8144ae8f57764fc64 # PokeAPI/pokeapi (o mesmo de scripts/build-dex.py): ids das formas
FIRST=387
LAST=1025
DIR=assets/sprites
BASE="https://raw.githubusercontent.com/PokeAPI/sprites/$COMMIT/sprites/pokemon"

update=0 shiny=0 forms=0
for arg in "$@"; do
  case $arg in
    --update) update=1 shiny=1 forms=1 ;;
    --shiny) shiny=1 ;;
    --forms) forms=1 ;;
    *) echo "opção desconhecida: $arg" >&2; exit 2 ;;
  esac
done
mkdir -p "$DIR/shiny" "$DIR/forms/shiny"
missing=$(mktemp)
trap 'rm -f "$missing"' EXIT

# Caminho local → endereço no PokeAPI/sprites.
upstream() {
  case $1 in
    forms/shiny/*) echo "$BASE/shiny/${1#forms/shiny/}" ;;
    forms/*) echo "$BASE/${1#forms/}" ;;
    *) echo "$BASE/$1" ;;
  esac
}

# Baixa cada caminho da entrada (um por linha, relativo a $DIR); 404 vai para $missing, outro erro para tudo.
download() {
  while read -r path; do
    printf '%s %s\n' "$DIR/$path" "$(upstream "$path")"
  done | xargs -r -P 8 -n 2 sh -c '
    code=$(curl -sSL -w "%{http_code}" -o "$1.part" "$2") || { rm -f "$1.part"; echo "falhou: $2" >&2; exit 255; }
    if [ "$code" = 200 ]; then mv "$1.part" "$1"
    elif [ "$code" = 404 ]; then rm -f "$1.part"; echo "${1#'"$DIR"'/}" >> "$0"
    else rm -f "$1.part"; echo "HTTP $code: $2" >&2; exit 255; fi
  ' "$missing"
}

form_ids() {
  local csv=$HOME/.cache/traderock/pokeapi/$DATA_COMMIT/pokemon.csv
  if [[ ! -s $csv ]]; then
    mkdir -p "$(dirname "$csv")"
    curl -fsSL -o "$csv" "https://raw.githubusercontent.com/PokeAPI/pokeapi/$DATA_COMMIT/data/v2/csv/pokemon.csv"
  fi
  awk -F, 'NR > 1 && $1 > 10000 { print $1 }' "$csv"
}

candidates() {
  if ((shiny)); then seq -f 'shiny/%g.png' 1 "$LAST"; fi
  if ((forms)); then form_ids | while read -r id; do echo "forms/$id.png"; echo "forms/shiny/$id.png"; done; fi
  # Sem --update: o que o SHA256SUMS já lista (com os 387–1025 de sempre). Com --update, os 1–386 do easyworld ficam.
  if ((!update)) && [[ -s $DIR/SHA256SUMS ]]; then awk '{ print $2 }' "$DIR/SHA256SUMS"; fi
  seq -f '%g.png' "$FIRST" "$LAST"
}

# Antes de acrescentar arquivos, confere os que já estão aqui (um arquivo estragado não entra no novo SHA256SUMS).
if ((!update)) && [[ -s $DIR/SHA256SUMS ]]; then
  (cd "$DIR" && sha256sum -c --quiet --ignore-missing SHA256SUMS)
fi

candidates | sort -u | while read -r path; do
  if ((update)) || [[ ! -s $DIR/$path ]]; then echo "$path"; fi
done | download

if [[ -s $missing ]]; then
  echo "sem arquivo no PokeAPI/sprites (o app usa o sprite da espécie):"
  sort -V "$missing" | sed 's/^/  /'
fi

if ((update || shiny || forms)); then
  (cd "$DIR" && find . -name '*.png' -printf '%P\n' | sort -V | xargs sha256sum > SHA256SUMS)
fi
(cd "$DIR" && sha256sum -c --quiet SHA256SUMS)
echo "$(wc -l < "$DIR/SHA256SUMS") sprites conferidos em $DIR"
