#!/usr/bin/env bash
# Monta o RockAssistent para macOS (Apple Silicon, M1 ou mais novo; só ESP32, D35) em build/macos/RockAssistent.app
# e o disco de instalação dist/RockAssistent-<versão>-macOS-arm64.dmg.
#
#   scripts/package-macos.sh [versão]
#
# Roda só num Mac com Apple Silicon (o CI usa o macos-15 do GitHub): o .dmg (hdiutil) e a assinatura (codesign) só
# existem no macOS, e o pip do Python embutido instala as rodas daqui mesmo.
#
# Assinatura "ad-hoc" (sem conta de desenvolvedor da Apple, decisão do autor): o Mac com chip Apple só roda código
# assinado, e ad-hoc basta para isso. Sem a notarização, o Gatekeeper pede, na primeira vez, o "Abrir mesmo assim"
# em Ajustes do Sistema → Privacidade e Segurança (packaging/release-notes.md).
#
# Conteúdo do RockAssistent.app:
#   Contents/MacOS/traderock + .NET (self-contained, osx-arm64) + frontend embutido
#   Contents/Resources/traderock.icns
#   Contents/Resources/esp32-runtime/python   CPython 3.12 do python-build-standalone (aarch64-apple-darwin,
#                                             checksum conferido) + esptool 5.4.0 e as dependências do pokeldn
#                                             (worker/requirements-esp32-macos.lock, hash fixo)
#   Contents/Resources/esp32-runtime/worker   só o necessário para "run.py --esp32" e "run.py --pokeldn"
#   Contents/Resources/esp32-runtime/pokeldn  o pokeldn com os patches (scripts/stage-pokeldn.sh, D21/D23)
#   Contents/Resources/LICENSE, THIRD-PARTY-NOTICES.md
#
# Nenhuma chave, ROM ou dado do usuário entra aqui.
set -euo pipefail
cd "$(dirname "$0")/.."

[[ $(uname -s) == Darwin && $(uname -m) == arm64 ]] || { echo "rode num Mac com Apple Silicon (o CI usa o macos-15)" >&2; exit 2; }

VERSION=${1:-$(sed -n 's|.*<Version>\(.*\)</Version>.*|\1|p' Directory.Build.props)}
VERSION=${VERSION#v}
if [[ ! $VERSION =~ ^[0-9]+\.[0-9]+\.[0-9]+(-[0-9A-Za-z.]+)?$ ]]; then
  echo "versão inválida: $VERSION" >&2; exit 2
fi

PY_RELEASE=20260929
PY_MAC="cpython-3.12.14+${PY_RELEASE}-aarch64-apple-darwin-install_only.tar.gz"
PY_MAC_SHA256=de6b8f94fa765639b423ea353ab340669704c7186f96ee3cab389dcfde770c3c
CACHE="${XDG_CACHE_HOME:-$HOME/.cache}/traderock"
BUILD=build/macos
APP=$BUILD/RockAssistent.app
RUNTIME=$APP/Contents/Resources/esp32-runtime
DIST=dist
DMG=$DIST/RockAssistent-$VERSION-macOS-arm64.dmg

# Os scripts usam o "sha256sum -c --quiet" do GNU. O macOS 15 traz um sha256sum próprio, sem o --quiet, e as versões
# antigas não trazem nenhum: aqui ele vira sempre o shasum (que tem o -c, e o -s no lugar do --quiet). O
# stage-pokeldn.sh também usa o sha256sum e herda o PATH.
SHIM=$BUILD/shim
mkdir -p "$SHIM"
cat > "$SHIM/sha256sum" <<'SH'
#!/bin/sh
for a; do
  shift
  [ "$a" = --quiet ] && a=-s
  set -- "$@" "$a"
done
exec shasum -a 256 "$@"
SH
chmod +x "$SHIM/sha256sum"
export PATH="$PWD/$SHIM:$PATH"

fetch() { # arquivo sha256
  if [[ ! -f "$CACHE/$1" ]]; then
    echo "==> baixando $1"
    curl -fsSL --retry 4 --retry-all-errors --retry-delay 5 -o "$CACHE/$1.part" "https://github.com/astral-sh/python-build-standalone/releases/download/$PY_RELEASE/${1//+/%2B}"
    mv "$CACHE/$1.part" "$CACHE/$1"
  fi
  echo "$2  $CACHE/$1" | sha256sum -c --quiet
}

rm -rf "$APP" "$BUILD/wheels" "$BUILD/dmg"
mkdir -p "$BUILD" "$DIST" "$CACHE"

echo "==> Pokédex offline (dex.sqlite, embutida no app)"
python3 -I scripts/build-dex.py

echo "==> app $VERSION (self-contained, osx-arm64)"
dotnet publish app/TradeRock.App -c Release -r osx-arm64 --self-contained \
  -p:BuildFrontend=true -p:Version="$VERSION" -p:DebugType=none -p:DebugSymbols=false \
  -o "$APP/Contents/MacOS" --nologo -v quiet
[[ -x "$APP/Contents/MacOS/traderock" && -f "$APP/Contents/MacOS/Photino.Native.dylib" ]] \
  || { echo "publicação do macOS incompleta" >&2; exit 1; }
# Só o Windows usa o .ico.
rm -f "$APP/Contents/MacOS/traderock.ico"
mkdir -p "$APP/Contents/Resources"
cp packaging/macos/traderock.icns LICENSE THIRD-PARTY-NOTICES.md "$APP/Contents/Resources/"

# A versão do pacote só aceita números (1.2.3): o sufixo de pré-lançamento fica de fora.
sed -e "s/{{VERSION}}/${VERSION%%-*}/g" -e "s/{{FULL_VERSION}}/$VERSION/g" packaging/macos/Info.plist > "$APP/Contents/Info.plist"
plutil -lint "$APP/Contents/Info.plist" >/dev/null

echo "==> Python do macOS para o esptool e o pokeldn"
fetch "$PY_MAC" "$PY_MAC_SHA256"
mkdir -p "$RUNTIME"
tar xzf "$CACHE/$PY_MAC" -C "$RUNTIME"
PY=$RUNTIME/python/bin/python3
[[ -x $PY ]] || { echo "python3 ausente no runtime" >&2; exit 1; }
SITE=$("$PY" -I -c 'import sysconfig; print(sysconfig.get_paths()["purelib"])')

echo "==> esptool e dependências (hash fixo)"
PIP=("$PY" -I -m pip --disable-pip-version-check --no-input)
# O esptool e o python-netlink só são publicados como código-fonte (puro Python): viram rodas py3-none-any aqui.
grep -E '^(esptool|python_netlink)==' worker/requirements.lock > "$BUILD/esptool.lock"
"${PIP[@]}" wheel --quiet --no-deps --require-hashes --cache-dir "$CACHE/pip" -w "$BUILD/wheels" -r "$BUILD/esptool.lock"
"${PIP[@]}" install --quiet --no-deps --require-hashes --only-binary=:all: --no-compile --cache-dir "$CACHE/pip" \
  --target "$SITE" -r worker/requirements-esp32-macos.lock
"${PIP[@]}" install --quiet --no-deps --no-index --only-binary=:all: --no-compile \
  --target "$SITE" "$BUILD"/wheels/esptool-*.whl "$BUILD"/wheels/python_netlink-*.whl
rm -rf "$SITE/bin"

# Sem interface gráfica, testes, cabeçalhos nem pip (só roda o esptool e o pokeldn).
LIB=$(dirname "$SITE")
rm -rf "$RUNTIME/python"/{include,share} \
       "$LIB"/{test,idlelib,tkinter,turtledemo,ensurepip,lib2to3,pydoc_data} \
       "$LIB"/lib-dynload/_tkinter* "$RUNTIME/python/lib"/{tcl*,tk*,itcl*,thread*,Tix*} \
       "$SITE"/pip* "$RUNTIME/python/bin"/{pip*,idle*,pydoc*,2to3*,python3*-config}
find "$RUNTIME/python" -name __pycache__ -prune -exec rm -rf {} +

echo "==> worker (gravação do ESP32 e pokeldn)"
mkdir -p "$RUNTIME/worker/traderock_worker/data"
cp worker/run.py "$RUNTIME/worker/"
cp worker/traderock_worker/{__init__.py,main.py,esp32_flash.py,netguard.py,pokeldn_env.py,pokeldn_board.py,pokeldn_trade.py,partner_ai.py} \
  "$RUNTIME/worker/traderock_worker/"
cp worker/traderock_worker/data/moves.json "$RUNTIME/worker/traderock_worker/data/"

echo "==> pokeldn + patches"
scripts/stage-pokeldn.sh "$RUNTIME/pokeldn"
find "$RUNTIME/pokeldn" -name __pycache__ -prune -exec rm -rf {} +
cat > "$RUNTIME/BUILD-INFO.json" <<JSON
{
  "python": "3.12.14+$PY_RELEASE (macOS arm64)",
  "esptool": "$(sed -n 's/^esptool==\([^ ]*\).*/\1/p' worker/requirements.lock)",
  "pokeldnCommit": "$(git -C engines/pokeldn rev-parse HEAD)"
}
JSON
rm -rf "$BUILD/wheels" "$BUILD/esptool.lock"

echo "==> conferência do runtime"
"$PY" -I -B -c 'import esptool, serial, trio, zstandard, Crypto, cryptography; print("    imports ok")'
"$APP/Contents/MacOS/traderock" --version

echo "==> assinatura ad-hoc"
# Primeiro cada binário de dentro (Mach-O), depois o pacote, que lacra o resto. Sem --deep: ele só assina pacotes
# aninhados, e aqui os binários estão soltos. Sem o "hardened runtime": ele exigiria permissões extras para o .NET.
find "$APP/Contents" -type f -print0 | while IFS= read -r -d '' f; do
  if file -b "$f" | grep -q '^Mach-O'; then
    codesign --force --sign - --timestamp=none "$f"
  fi
done
codesign --force --sign - --timestamp=none "$APP"
codesign --verify --strict --verbose=1 "$APP"
"$APP/Contents/MacOS/traderock" --version >/dev/null

echo "==> disco de instalação"
mkdir -p "$BUILD/dmg"
ditto "$APP" "$BUILD/dmg/RockAssistent.app"
ln -s /Applications "$BUILD/dmg/Aplicativos"
rm -f "$DMG"
hdiutil create -quiet -volname "RockAssistent" -srcfolder "$BUILD/dmg" -fs HFS+ -format UDZO -ov "$DMG"
rm -rf "$BUILD/dmg"
echo "    $(du -sh "$APP" | cut -f1) em $APP"
ls -la "$DMG"
