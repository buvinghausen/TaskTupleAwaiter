#!/usr/bin/env bash
# Runs the full test matrix on Linux/WSL2, where bare `dotnet test` can't work:
# its MTP orchestrator enumerates every TFM up front and aborts the whole run
# the instant it hits net472 (not launchable through the dotnet muxer on Linux).
# Modern TFMs go through `dotnet test -f <tfm>` one at a time; net472 projects
# are built with the SDK and the resulting .exe is run directly under Mono.
set -euo pipefail

cd "$(dirname "${BASH_SOURCE[0]}")"

MODERN_TFMS=(net11.0 net10.0 net9.0 net8.0)

# Test projects whose TargetFrameworks (tests/unit/Directory.Build.props) include net472.
NET472_PROJECTS=(
	tests/unit/TaskTupleAwaiter.Tests/TaskTupleAwaiter.Tests.csproj
)

for tfm in "${MODERN_TFMS[@]}"; do
	echo "=== dotnet test -f $tfm ==="
	dotnet test -f "$tfm"
done

for proj in "${NET472_PROJECTS[@]}"; do
	name=$(basename "$proj" .csproj)
	dir=$(dirname "$proj")
	echo "=== net472: $name (via Mono) ==="
	dotnet build "$proj" -f net472 -c Debug
	mono "$dir/bin/Debug/net472/$name.exe"
done
