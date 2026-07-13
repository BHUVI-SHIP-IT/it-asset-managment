cd /home/sakthi/projects/new

# Stop any previous stack (avoids port conflicts)
docker compose down

# Build + start full stack (migrations run inside tracer-api)
docker compose up -d --build

# Hot-reload (optional):
# docker compose -f docker-compose.yml -f docker-compose.dev.yml up --build

# Stop:
# docker compose down
