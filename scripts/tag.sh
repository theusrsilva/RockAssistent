#!/usr/bin/env bash
# make tag: publica uma versão nova do RockAssistent.
#
# 1. Busca a main e as tags do GitHub. A tag sai sempre da origin/main, sem trocar a sua branch nem mexer nos arquivos.
# 2. Mostra os commits desde a última tag (vXX.XX.XX). Sem nenhum, não há o que publicar.
# 3. Pergunta o tipo: Atualização (+1 na 1ª casa), Feature (2ª), Fix (3ª) ou outra versão digitada. As casas à direita
#    zeram.
# 4. Pergunta se é pré-lançamento (a marca [pré-lançamento] vai na mensagem da tag; o CI tira a marca do texto).
# 5. Monta as Novidades (a mensagem da tag vira o texto do Release) com os commits, mostra e deixa editar.
# 6. Confirma, cria a tag anotada na origin/main e envia. O CI (.github/workflows/ci.yml) testa, gera os pacotes e
#    publica o Release.
{
set -euo pipefail

export GIT_PAGER=cat PAGER=cat
TAG_PATTERN='v[0-9][0-9].[0-9][0-9].[0-9][0-9]'
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT"

die() { echo "✗ $*" >&2; exit 1; }

# ---- 1. main do GitHub ------------------------------------------------------------------------------------------------
echo "→ Buscando a main e as tags do GitHub"
git fetch --quiet origin main || die "O git fetch da main falhou."
git fetch --quiet --tags --force origin || die "O git fetch das tags falhou."
target="$(git rev-parse origin/main)"

# ---- 2. o que mudou desde a última tag --------------------------------------------------------------------------------
last="$(git tag --list "$TAG_PATTERN" --sort=-v:refname | head -n 1)"
mapfile -t news < <(git log --no-merges --reverse --format='%s' ${last:+"$last"..}"$target")
if [ "${#news[@]}" -eq 0 ]; then
    echo "Nada novo na main desde ${last}: não há o que publicar."
    exit 0
fi

echo
echo "Última versão: ${last:-nenhuma}  ·  ${#news[@]} commit(s) novo(s) na main que vão subir:"
echo
git --no-pager log --no-merges --reverse --format='  • %h  %ad  %s' --date=format:'%d/%m %H:%M' \
    ${last:+"$last"..}"$target"
echo

# ---- 3. tipo da versão --------------------------------------------------------------------------------------------------
current="${last:-v00.00.00}"
IFS=. read -r major minor patch <<< "${current#v}"
major=$((10#$major)); minor=$((10#$minor)); patch=$((10#$patch))
fmt() { printf 'v%02d.%02d.%02d' "$1" "$2" "$3"; }
echo "  1) Atualização   $(fmt $((major + 1)) 0 0)"
echo "  2) Feature       $(fmt "$major" $((minor + 1)) 0)"
echo "  3) Fix           $(fmt "$major" "$minor" $((patch + 1)))"
echo "  4) Outra (digitar)"
echo
read -rp "Tipo [1-4]: " choice
case "$choice" in
    1) major=$((major + 1)); minor=0; patch=0 ;;
    2) minor=$((minor + 1)); patch=0 ;;
    3) patch=$((patch + 1)) ;;
    4) read -rp "Versão (ex.: v02.01.00 ou v02.01.00-rc.1): " typed
       [[ "$typed" =~ ^v[0-9]{2}\.[0-9]{2}\.[0-9]{2}(-[0-9A-Za-z.]+)?$ ]] || die "Formato inválido: $typed" ;;
    *) die "Opção inválida: $choice" ;;
esac
if [ "$choice" = 4 ]; then
    tag="$typed"
else
    [ "$major" -lt 100 ] && [ "$minor" -lt 100 ] && [ "$patch" -lt 100 ] || die "Cada casa vai até 99."
    tag="$(fmt "$major" "$minor" "$patch")"
fi
git rev-parse -q --verify "refs/tags/$tag" >/dev/null && die "A tag $tag já existe."

# ---- 4. pré-lançamento --------------------------------------------------------------------------------------------------
# Versão com sufixo (-rc.1) já sai como pré-lançamento no CI.
mark=""
if [[ "$tag" != *-* ]]; then
    read -rp "Marcar como pré-lançamento? (y/N) " answer
    [ "$answer" = "y" ] && mark=" [pré-lançamento]"
fi

# ---- 5. Novidades (mensagem da tag) -------------------------------------------------------------------------------------
v="${tag#v}"; base="${v%%-*}"; suffix="${v#"$base"}"
IFS=. read -r a b c <<< "$base"
title="RockAssistent $((10#$a)).$((10#$b)).$((10#$c))${suffix}${mark}"
msg="$(mktemp)"
trap 'rm -f "$msg"' EXIT
{
    echo "$title"
    echo
    # Só o que quem usa o app vê: sem chore/docs/test/ci/build, sem o prefixo feat:/fix: e com maiúscula no começo.
    shown=0
    for line in "${news[@]}"; do
        [[ "$line" =~ ^(chore|docs|test|ci|build|style|refactor)(\(.*\))?!?: ]] && continue
        line="$(sed -E 's/^[a-z]+(\([^)]*\))?!?: *//' <<< "$line")"
        echo "- ${line^}"
        shown=$((shown + 1))
    done
    [ "$shown" -gt 0 ] || echo "- Ajustes e correções."
} > "$msg"

echo
echo "Novidades do Release (mensagem da tag):"
echo "------------------------------------------------------------"
cat "$msg"
echo "------------------------------------------------------------"
read -rp "Editar o texto antes de publicar? (y/N) " answer
if [ "$answer" = "y" ]; then
    editor="$(git var GIT_EDITOR)"
    $editor "$msg" || die "O editor saiu com erro; nada foi publicado."
    echo
    cat "$msg"
fi
[ -n "$(grep -v '^[[:space:]]*$' "$msg" || true)" ] || die "Mensagem vazia; nada foi publicado."

# ---- 6. tag e push ------------------------------------------------------------------------------------------------------
echo
read -rp "Publicar $tag na origin/main ($(git rev-parse --short "$target"))? (y/n) " answer
[ "$answer" = "y" ] || { echo "Nada foi feito."; exit 0; }

git tag -a "$tag" -F "$msg" --cleanup=verbatim "$target"
if ! git push --quiet origin "refs/tags/$tag"; then
    git tag -d "$tag" >/dev/null
    die "O push da tag falhou; a tag local foi apagada. Nada foi publicado."
fi
repo="$(gh repo view --json nameWithOwner -q .nameWithOwner 2>/dev/null || echo theusrsilva/RockAssistent)"
echo
echo "✓ $tag enviada. O CI testa, gera os pacotes e publica o Release:"
echo "  https://github.com/$repo/actions"
exit 0
}
