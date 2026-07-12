#!/usr/bin/env bash
# Deploy Tracer with Docker Compose (host-network workaround when bridge/veth is broken).
set -euo pipefail
cd "$(dirname "$0")/.."

COMPOSE=(docker compose -f docker-compose.yml -f docker-compose.host.yml)

echo "==> Building and starting stack (host network)..."
"${COMPOSE[@]}" up -d --build

echo "==> Waiting for SQL Server on :1433..."
for i in $(seq 1 60); do
  if docker compose -f docker-compose.yml -f docker-compose.host.yml ps sql-server 2>/dev/null | grep -qi healthy \
     || (command -v nc >/dev/null && nc -z 127.0.0.1 1433); then
    echo "SQL port is open (attempt $i)"
    break
  fi
  sleep 2
done

echo "==> Applying EF migrations..."
export PATH="${HOME}/.dotnet:${HOME}/.dotnet/tools:${PATH}"
dotnet ef database update \
  --project src/Tracer.Persistence/Tracer.Persistence.csproj \
  --startup-project src/Tracer.Api/Tracer.Api.csproj

echo ""
echo "Deployed."
echo "  UI:      http://localhost:4200"
echo "  API:     http://localhost:5001"
echo "  Seq UI:  http://localhost:8081"
echo "  Login:   admin@tracer.io / Admin123!"
echo ""
echo "Logs:  ${COMPOSE[*]} logs -f"
echo "Stop:  ${COMPOSE[*]} down"
