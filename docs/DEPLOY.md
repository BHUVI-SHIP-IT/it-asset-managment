# Deploy Tracer (Docker Compose only)

The full stack (Angular UI, .NET API, SQL Server, Redis, Seq) runs exclusively via Docker Compose on the default bridge network. Services reach each other by **Compose service name** (`sql-server`, `redis`, `tracer-api`, `tracer-web`, `seq`) — not host `localhost` inside containers.

## Quick start

```bash
cd /home/sakthi/projects/new

# Stop any previous stack
docker compose down

# Build and start (API applies EF migrations on startup when RUN_MIGRATIONS=true)
docker compose up -d --build
```

## Hot-reload development

Bind-mounts source and watches for changes:

```bash
docker compose -f docker-compose.yml -f docker-compose.dev.yml up --build
```

## URLs (published on the host)

| Service | URL |
|---------|-----|
| Angular UI | http://localhost:4200 |
| API | http://localhost:5001 |
| API health | http://localhost:5001/health/live |
| Seq UI | http://localhost:8081 |

**Login:** `admin@tracer.io` / `Admin123!`

Inside the Compose network:

| From | To |
|------|----|
| `tracer-web` (nginx) | `http://tracer-api:8080` |
| `tracer-api` | `Server=sql-server;...` / `redis:6379` / `http://seq:5341` |

## Useful commands

```bash
docker compose ps -a
docker compose logs -f tracer-api
docker compose logs -f tracer-web
docker compose down
docker compose down -v   # wipe DB / Redis / Seq volumes
```

Or: `./scripts/deploy-docker.sh`

## Files

- [`docker-compose.yml`](../docker-compose.yml) — full stack
- [`docker-compose.dev.yml`](../docker-compose.dev.yml) — hot-reload overlay
- [`scripts/deploy-docker.sh`](../scripts/deploy-docker.sh) — build + start helper
