#!/usr/bin/env bash
# Cópias com o nome de antes do rebranding, para as instalações antigas se atualizarem sozinhas.
# O app antigo procura TradeRock-Setup-<versão>.exe e TradeRock-<versão>-x86_64.AppImage no Release e confere o
# hash pelo mesmo nome no SHA256SUMS; o .deb sempre se chamou traderock_*. Roda no job "release" do CI, depois do
# SHA256SUMS completo:
#   scripts/release-legacy-names.sh dist
set -euo pipefail
cd "${1:?uso: $0 <pasta dist>}"
shopt -s nullglob
files=(RockAssistent-Setup-*.exe RockAssistent-*-x86_64.AppImage)
(( ${#files[@]} )) || { echo "nenhum RockAssistent-* em $PWD" >&2; exit 1; }
for f in "${files[@]}"; do
  old="TradeRock-${f#RockAssistent-}"
  cp -- "$f" "$old"
  sha256sum -- "$old" >> SHA256SUMS
  echo "    $old"
done
sha256sum -c --quiet SHA256SUMS
