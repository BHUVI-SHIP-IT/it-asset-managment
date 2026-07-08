# Project Tracer — Implementation Roadmap

This roadmap defines the implementation sequence for Project Tracer, ensuring the fastest enterprise-grade delivery based on the frozen architecture (ASP.NET Core 9, Angular 20+, SQL Server 2022).

## Part 1 — Architecture Freeze (Locked Decisions)

These are ratified from Docs 1–12 and should not change during build without a formal ADR:

| Layer | Frozen Decision | Source |
| :--- | :--- | :--- |
| **Frontend** | Angular 20 · Signals · Standalone Components · Material · SCSS | Docs 6, 9 |
| **Backend** | ASP.NET Core 9 · Clean Architecture · CQRS/MediatR · FluentValidation | Docs 3, 10 |
| **Persistence** | SQL Server 2022 · EF Core 9 · Temporal Tables · RLS · Soft-delete | Doc 4 |
| **Auth** | OIDC/OAuth2 (Azure AD) · SCIM 2.0 · JWT policy-based authz | Docs 3, 7 |
| **Async/Events** | Hangfire + Outbox pattern | Docs 10, 11 |
| **Cache** | Redis (L2 CQRS caching, rate limiting, Hangfire state) | Docs 3, 10 |
| **Platform** | Docker · Kubernetes · Nginx Ingress · GitHub Actions · Helm | Doc 11 |
| **Testing** | xUnit/Moq · Jest/ATL · Playwright · Testcontainers · k6 · OWASP ZAP | Doc 12 |

> [!NOTE]
> **Resolution of Discrepancies:**
> 1. Background jobs: Frozen on **Hangfire** (dropping Quartz) for built-in dashboard and Redis support.
> 2. SRS Artifact: The generic "Dashboard" module describing Asset behaviors will be corrected before sprint planning.

## Part 2 — Backend-first vs Frontend-first

> [!RECOMMENDATION]
> **Recommendation: Contract-first, then backend-led vertical slices with the frontend trailing by one sprint.**

**Rationale:**
- The system is API-contract-defined (OpenAPI 3.0, RFC 7807).
- CQRS aligns naturally with vertical slices (Command → Handler → EF write → API endpoint → Angular Signal store → Component).
- The Angular `*hasPermission` directive and the .NET `[Authorize(Policy)]` share the same permission strings. Defining those once, backend-first, prevents drift.

**Execution:**
1. **Sprint N:** Publish/lock the OpenAPI contract for a module → backend implements the slice.
2. **Sprint N+1:** Frontend consumes the real endpoint (having prototyped against the contract mock in Sprint N).

## Part 3 — Milestones

| Milestone | Goal | Exit Criteria |
| :--- | :--- | :--- |
| **M0 Foundation** ✅ | Solution skeleton, CI/CD, DB baseline | Tracer.sln 6-project layout builds; Docker Compose up; empty migration applies; health checks green |
| **M1 IAM & Auth** ✅ | Login → JWT → RBAC end-to-end | OIDC login works; permission claims in JWT; `[Authorize]` + `*hasPermission` enforced; one protected page renders |
| **M2 Master Data** ✅ | All lookup/reference CRUD | Companies, Categories, Locations, Manufacturers, Suppliers, StatusLabels, AssetModels, Depreciations CRUD + cached dropdowns |
| **M3 Asset Core** ✅ | Asset CRUD + Checkout/Checkin + Audit | Full asset aggregate; checkout workflow with EULA + temporal audit; Outbox → email fires |
| **M4 License & Inv** ✅ | Licenses, Components, Accessories, Consumables | Seat allocation, true-up job, FIFO consumables |
| **M5 Financial** ✅ | Depreciation + async exports | Depreciation jobs; report builder with Hangfire polling; CSV/PDF export |
| **M6 Notifications** ⭐ | Multi-channel alerts + tenant config | Email/Slack/Teams webhooks; settings; custom fields; notification center |
| **M7 Hardening** | Security, perf, a11y, DR | OWASP ZAP clean; k6 5k users @ <100ms; WCAG 2.1 AA; DR drill passes RPO/RTO |
| **M8 UAT & Go-Live** | Business sign-off + prod cutover | UAT approved; idempotent migration script; blue/green prod deploy |

## Part 4 — Module Dependency Graph & Critical Path

**Critical Path (Longest chain of strictly-dependent work):**

M0 Foundation (Domain base + DbContext + CI/CD) ✅
→ IAM schema + JWT auth + PermissionHandler ✅
→ Master Data (StatusLabels, Categories, Locations, AssetModels) ✅
→ Asset aggregate (entity + invariants + repository) ✅
→ Checkout/Checkin workflow (state machine + temporal audit + Outbox email) ✅
→ M4 License & Inv ✅
→ M5 Financial ✅
→ M6 Notifications 🔄 (NEXT)
→ M8 UAT sign-off → Prod cutover

> [!IMPORTANT]
> **Auth sits on the critical path.** Because default-deny is mandated (`[Authorize]` on every endpoint), nothing can be tested end-to-end until JWT + policy handlers exist. Build it in M1.

## Part 5 — Database Migration Order

Migrations should be additive and follow dependency order:

1. **IAM:** Users, Roles, UserRoles, Permissions (Required for RLS/Auth)
2. **Settings:** Tenant Config, CustomFields
3. **Master Data:** Companies, Manufacturers, Suppliers, Categories, Locations, StatusLabels, Depreciations
4. **Assets:** AssetModels, Assets (Temporal enabled), AssetLogs
5. **Inventory:** Licenses, Components, Accessories, Consumables
6. **Relations:** AssetAssignments, LicenseSeats, ConsumableCheckouts
7. **Audit/Jobs:** OutboxMessages, Hangfire Schema

## Part 6 — API Implementation Order

Grouped by Sprint/Slice:

1. **Foundation:** `/health`, `/api/v1/auth/login`, `/api/v1/auth/me`
2. **Master Data (CRUD):** `/api/v1/categories`, `/api/v1/locations`, `/api/v1/status-labels`, `/api/v1/models`
3. **Asset Core:** `/api/v1/assets`, `/api/v1/assets/{id}/checkout`, `/api/v1/assets/{id}/checkin`
4. **Inventory:** `/api/v1/licenses`, `/api/v1/components`, `/api/v1/consumables`
5. **Reporting:** `/api/v1/reports/depreciation`, `/api/v1/exports (async)`

## Part 7 — Angular Implementation Order

Trailing backend by 1 sprint:

1. **Core:** `AppModule`, Routing, HTTP Interceptors (JWT + Error Handling), `AuthService`
2. **Shared:** `hasPermission` directive, Base Table Component, Base Form Modal
3. **Layout:** Sidebar, Header, Breadcrumbs
4. **Master Data:** Category List/Edit, Location List/Edit
5. **Asset Core:** Asset DataGrid, Checkout Flow (Wizard/Modal), Asset Details Page (Tabs for Audit/History)
6. **Inventory:** License Management, Consumables DataGrid
7. **Dashboards:** Main Dashboard (Charts, KPIs)

## Part 8 — Testing Order

> [!TIP]
> Use a Test Pyramid approach to avoid fragile end-to-end test bottlenecks.

1. **Unit Tests (Immediate):** Domain Entities (Invariants), MediatR Handlers (Logic), Angular Signals (State Transitions).
2. **Integration Tests (Post-API):** `Testcontainers` (SQL + Redis) testing CQRS commands through to DB and back.
3. **E2E Tests (Sprint N+1):** Playwright testing critical flows (Login → Checkout Asset → Accept EULA).
4. **Security & Perf (M7):** OWASP ZAP (DAST) in pipeline, k6 for load testing critical endpoints (Dashboards, Asset grids).

## Part 9 — Estimated Timeline (Sprint Planning)

Assuming 2-week sprints, 1 Full-Stack Squad (2 BE, 2 FE, 1 QA):

*   **Sprint 1:** M0 & M1 (Foundation, CI/CD, Auth)
*   **Sprint 2:** M2 (Master Data BE), M1 (Auth FE)
*   **Sprint 3:** M3 (Asset Core BE), M2 (Master Data FE)
*   **Sprint 4:** M4 (Inv BE), M3 (Asset Core FE)
*   **Sprint 5:** M5 (Fin BE), M4 (Inv FE)
*   **Sprint 6:** M6 (Notif BE), M5 (Fin FE)
*   **Sprint 7:** M7 (Hardening BE), M6 (Notif FE)
*   **Sprint 8:** M7 (Hardening FE, E2E), M8 (UAT)

**Total Estimated Duration:** 16 weeks (4 months) to Prod Go-Live.
