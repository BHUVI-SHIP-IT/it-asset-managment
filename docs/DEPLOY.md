# Deploy Tracer with Docker

## Why host networking?

On this machine Docker **bridge/veth** networking fails:

```text
failed to add the host (veth...) <=> sandbox (veth...) pair interfaces: operation not supported
```

Root cause: running kernel modules don’t match (reboot after a `linux` package upgrade usually fixes bridge mode). Until then, use the **host-network** overlay.

Permanent fix for bridge networking later:

```bash
sudo reboot   # after pacman -Syu linux / linux-headers
# then: sudo modprobe veth && docker run --rm hello-world
```

---

## Deploy (commands used)

From the repo root:

```bash
cd /home/sakthi/projects/new

# Build images + start SQL, Redis, API, Web (host network)
docker compose -f docker-compose.yml -f docker-compose.host.yml up -d --build

# Apply EF migrations (from host; SQL on localhost:1433)
export PATH="$HOME/.dotnet:$PATH:$HOME/.dotnet/tools"
dotnet ef database update \
  --project src/Tracer.Persistence/Tracer.Persistence.csproj \
  --startup-project src/Tracer.Api/Tracer.Api.csproj
```

Or one script:

```bash
./scripts/deploy-docker.sh
```

---

## URLs

| Service | URL |
|---------|-----|
| Angular UI | http://localhost:4200 |
| API | http://localhost:5001 |
| API health | http://localhost:5001/health/live |
| SQL Server | localhost:1433 |
| Redis | localhost:6379 |

**Login:** `admin@tracer.io` / `Admin123!`

---

## Useful commands

```bash
# Status
docker compose -f docker-compose.yml -f docker-compose.host.yml ps -a

# Logs
docker compose -f docker-compose.yml -f docker-compose.host.yml logs -f tracer-api
docker compose -f docker-compose.yml -f docker-compose.host.yml logs -f tracer-web

# Stop
docker compose -f docker-compose.yml -f docker-compose.host.yml down

# Stop + wipe DB volume
docker compose -f docker-compose.yml -f docker-compose.host.yml down -v
```

---

## Files involved

- [`docker-compose.yml`](docker-compose.yml) — base stack
- [`docker-compose.host.yml`](docker-compose.host.yml) — host-network workaround + localhost connection strings
- [`src/Tracer.Web/nginx.host.conf`](src/Tracer.Web/nginx.host.conf) — UI on `:4200`, proxies `/api` → `127.0.0.1:5001`
- [`scripts/deploy-docker.sh`](scripts/deploy-docker.sh) — build + migrate helper

## Notes

- Seq may exit under host networking; it is optional for the app.
- Do **not** use plain `docker compose up --build` until `docker run --rm hello-world` works without `--network=host`.
