#!/usr/bin/env bash
# Fumaça do .dmg do macOS (D35): monta o disco, copia o RockAssistent.app para uma pasta "Aplicativos" temporária
# (sem mexer no /Applications de quem roda), confere a assinatura, roda o app sem janela e fala com a API.
#
#   scripts/smoke-macos.sh dist/RockAssistent-<versão>-macOS-arm64.dmg <versão>
#
# Os dados vão para uma pasta pessoal temporária (HOME), apagada no fim.
set -euo pipefail
cd "$(dirname "$0")/.."

DMG=${1:?arquivo .dmg}
VERSION=${2:?versão}
WORK=$(mktemp -d)
MOUNT=$WORK/mount
APPS=$WORK/Applications
PID=""

fail() { echo "FALHOU: $*" >&2; exit 1; }
cleanup() {
  [[ -n $PID ]] && kill "$PID" 2>/dev/null || true
  hdiutil detach -quiet "$MOUNT" 2>/dev/null || true
  rm -rf "$WORK"
}
trap cleanup EXIT

json() { python3 -c "import json,sys; d=json.load(sys.stdin); print($1)"; }

mkdir -p "$MOUNT" "$APPS" "$WORK/home"
hdiutil attach -quiet -nobrowse -readonly -mountpoint "$MOUNT" "$DMG"
[[ -L "$MOUNT/Aplicativos" ]] || fail "o disco não tem o atalho para Aplicativos"
ditto "$MOUNT/RockAssistent.app" "$APPS/RockAssistent.app"
hdiutil detach -quiet "$MOUNT"
APP=$APPS/RockAssistent.app

codesign --verify --strict "$APP" || fail "assinatura inválida"
[[ $(plutil -extract CFBundleExecutable raw "$APP/Contents/Info.plist") == traderock ]] || fail "Info.plist errado"
out=$("$APP/Contents/MacOS/traderock" --version)
[[ $out == "RockAssistent $VERSION" ]] || fail "versão: $out"
echo "==> $out"

HOME="$WORK/home" "$APP/Contents/MacOS/traderock" --no-window > "$WORK/app.log" 2>&1 &
PID=$!
for _ in $(seq 60); do
  grep -q "http://127.0.0.1" "$WORK/app.log" 2>/dev/null && break
  sleep 1
done
url=$(grep -o 'http://127.0.0.1:[0-9]*/?t=[A-Za-z0-9_-]*' "$WORK/app.log" | head -1) || { cat "$WORK/app.log" >&2; fail "o app não subiu"; }
base=${url%%/?t=*}
token=${url##*t=}
api() { curl -fsS -H "X-TradeRock-Token: $token" "$base$1"; }

api /api/health | json 'd["os"]' | grep -qx macos || fail "o app não se viu no macOS"
api /sobre | grep -q '<div id="root">' || fail "o frontend não foi servido"
setup=$(api /api/setup | json 'd["system"]["os"] + " " + d["advice"]["situation"]')
[[ $setup == "macos macos" ]] || fail "leitura do sistema: $setup"
# O pokeldn embutido (Contents/Resources/esp32-runtime) foi achado: nenhum motor reclama dele.
missing=$(api /api/engines | json '",".join(e["id"] for e in d if e.get("reasonCode") == "pokeldn_missing")')
[[ -z $missing ]] || fail "o app não achou o pokeldn embutido: $missing"
[[ -f "$WORK/home/Library/Application Support/TradeRock/traderock.db" ]] || fail "o banco não está no Application Support"
[[ $(stat -f %Lp "$WORK/home/Library/Application Support/TradeRock") == 700 ]] || fail "a pasta de dados não é só do usuário"
echo "==> fumaça ok"
