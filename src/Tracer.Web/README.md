# TracerWeb

Angular frontend for Tracer. **Do not use `ng serve` on the host** — run the app through Docker Compose so the UI, API, and database share the same network.

## Run (recommended)

From the repository root:

```bash
docker compose up -d --build
```

Open http://localhost:4200/

Hot-reload (bind-mounted sources):

```bash
docker compose -f docker-compose.yml -f docker-compose.dev.yml up --build
```

The production image serves static files with nginx and proxies `/api` → `http://tracer-api:8080`. The hot-reload image uses `proxy.docker.json` for the same Compose DNS name.

## Code scaffolding

```bash
# Inside a Node container or after npm ci in Tracer.Web (for generating files only):
npx ng generate component component-name
```

## Building (used by Docker)

```bash
npm run build
```

Artifacts land in `dist/`. The `Dockerfile` runs this step during image build.

## Tests

```bash
npm test
```

## Additional Resources

[Angular CLI Overview](https://angular.dev/tools/cli)
