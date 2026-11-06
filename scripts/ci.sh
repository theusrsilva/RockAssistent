#!/usr/bin/env bash
# Build + testes do TradeRock. Roda no CI dentro da imagem do .devcontainer e localmente.
set -euo pipefail
cd "$(dirname "$0")/.."

echo "::group::Pokédex offline (scripts/build-dex.py)"
python3 -m unittest discover -s scripts/tests -t scripts
python3 -I scripts/build-dex.py
echo "::endgroup::"

echo "::group::Guias (scripts/check-guides.py)"
python3 -I scripts/check-guides.py
echo "::endgroup::"

echo "::group::Frontend"
(cd frontend && npm ci && npm run typecheck && npm test && npm run build)
echo "::endgroup::"

echo "::group::Worker (Python)"
python3 -m unittest discover -s worker/tests -t worker
echo "::endgroup::"

echo "::group::pokeldn (testes offline com os patches, D21)"
scripts/test-pokeldn.sh
echo "::endgroup::"

echo "::group::.NET build"
dotnet build TradeRock.slnx -c Release
echo "::endgroup::"

echo "::group::.NET testes"
dotnet test --solution TradeRock.slnx -c Release --no-build
echo "::endgroup::"
