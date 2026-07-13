#!/usr/bin/env bash
# Deploy Tracer with Docker Compose (bridge network, service-name DNS).
set -euo pipefail
cd "$(dirname "$0")/.."

echo "==> Building and starting stack..."
docker compose up -d --build

echo "==> Waiting for API health..."
for i in $(seq 1 60); do
  if curl -sf http://127.0.0.1:5001/health/live >/dev/null 2>&1; then
    echo "API healthy (attempt $i)"
    break
  fi
  sleep 2
done

echo ""
echo "Deployed."
echo "  UI:      http://localhost:4200"
echo "  API:     http://localhost:5001"
echo "  Seq UI:  http://localhost:8081"
echo "  Login:   admin@tracer.io / Admin123!"
echo ""
echo "Logs:  docker compose logs -f"
echo "Stop:  docker compose down"
echo "Dev:   docker compose -f docker-compose.yml -f docker-compose.dev.yml up --build"
