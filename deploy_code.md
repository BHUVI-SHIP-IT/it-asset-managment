cd /home/sakthi/projects/new

# Build + start (host-network workaround)
docker compose -f docker-compose.yml -f docker-compose.host.yml up -d --build

# Apply DB migrations
export PATH="$HOME/.dotnet:$PATH:$HOME/.dotnet/tools"
dotnet ef database update \
  --project src/Tracer.Persistence/Tracer.Persistence.csproj \
  --startup-project src/Tracer.Api/Tracer.Api.csproj


to stop: 
  docker compose -f docker-compose.yml -f docker-compose.host.yml down
