# ── Build Stage ──
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

# Copy project files first for layer caching.
COPY src/Tracer.Shared/Tracer.Shared.csproj Tracer.Shared/
COPY src/Tracer.Domain/Tracer.Domain.csproj Tracer.Domain/
COPY src/Tracer.Application/Tracer.Application.csproj Tracer.Application/
COPY src/Tracer.Persistence/Tracer.Persistence.csproj Tracer.Persistence/
COPY src/Tracer.Infrastructure/Tracer.Infrastructure.csproj Tracer.Infrastructure/
COPY src/Tracer.Api/Tracer.Api.csproj Tracer.Api/
COPY Directory.Build.props .

RUN dotnet restore Tracer.Api/Tracer.Api.csproj

# Copy all source and build.
COPY src/ .
WORKDIR /src/Tracer.Api
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# ── Watch Stage (Compose hot-reload overlay) ──
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS watch
WORKDIR /src
RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV DOTNET_USE_POLLING_FILE_WATCHER=1
EXPOSE 8080
# Source is bind-mounted at runtime from docker-compose.dev.yml
CMD ["dotnet", "watch", "run", "--project", "Tracer.Api/Tracer.Api.csproj", "--urls", "http://0.0.0.0:8080", "--no-launch-profile"]

# ── Runtime Stage ──
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
WORKDIR /app
EXPOSE 8080

RUN apk add --no-cache icu-libs wget
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Non-root user for security (Doc 11 §8.2).
USER $APP_UID

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Tracer.Api.dll"]
