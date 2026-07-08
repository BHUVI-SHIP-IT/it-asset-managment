# Project Tracer - Current State & Handoff

Welcome! If you are another AI model, IDE, or developer taking over this project, this document serves as the single source of truth for where we left off.

## 📍 Overall Status
**Current Active Phase:** `M7 Hardening`
**Last Completed Phase:** `M6 Notifications`

We are building an Enterprise IT Asset Management system using:
- **Backend:** ASP.NET Core 9, Clean Architecture, CQRS (MediatR), FluentValidation, EF Core 9
- **Database:** SQL Server 2022 (with Temporal Tables)
- **Background Jobs:** Hangfire

## 🏁 Completed So Far (M0 - M6)
1. **Foundation & Auth (M0 & M1):** The Domain, Application, Infrastructure, and Api layers are set up. Identity schema and JWT authorization are in place.
2. **Master Data (M2):** CRUD endpoints and Domain aggregates for reference data (Companies, Departments, Locations, Manufacturers, StatusLabels, etc.).
3. **Asset Core (M3):** The main `Asset` aggregate. Implemented checkout/check-in flows and outbox pattern (Domain Events -> Outbox Messages).
4. **Inventory & Licenses (M4):**
   - Added `Consumable`, `Accessory`, `Component`, and `SoftwareLicense` aggregates.
   - Implemented FIFO logic for consumables.
   - Successfully resolved database migration conflicts (`[IsDeleted]` column mismatch with Temporal Tables).
   - Generated and successfully applied EF Core migrations (`AddMasterData`, `EnableTemporalAssets`, `AddInventoryAggregates`).
   - The API is fully functional and responds correctly to routing (`ConsumablesController`).
5. **Financial (M5):**
   - Created `Depreciation` schedule aggregate.
   - Added `DepreciationId` and `CurrentValue` to the `Asset` aggregate.
   - Built a monthly Hangfire `CalculateAssetValuationsJob` to process straight-line depreciation automatically.
   - Built an asynchronous reporting engine (`FinancialReportJob`) to generate financial CSV reports on demand and track status via `ReportExport`.
   - Applied the `AddFinancialFeatures` EF Core migration.
6. **Notifications (M6):**
   - Settings CQRS: `GetAllSettingsQuery`, `UpsertSettingCommand`, `DeleteSettingCommand`.
   - CustomFields CQRS: Full CRUD commands + queries + `SetCustomFieldValueCommand` for per-entity upsert.
   - Three new API controllers: `NotificationsController`, `SettingsController`, `CustomFieldsController`.
   - EF Core migration `20260708000000_AddNotificationsAndTenantConfig` (hand-crafted, ready to apply).
   - Unit tests: `NotificationTests`, `TenantSettingTests`, `CustomFieldTests`.

## 🚀 Next Steps (M7 Hardening — In Progress)

**M7 Goals:**
1. OWASP ZAP DAST scan — run against the API and resolve findings.
2. k6 load test — 5,000 concurrent users at < 100ms p95 on `/api/v1/assets` and `/api/v1/notifications`.
3. WCAG 2.1 AA audit on the Angular frontend.
4. Disaster Recovery drill — validate RPO/RTO targets pass.

## ⚠️ Pending Action Before M7

The M6 EF Core migration `20260708000000_AddNotificationsAndTenantConfig` has been written but **not yet applied to the database**. Run this first:
```bash
dotnet ef database update -p src/Tracer.Persistence -s src/Tracer.Api
```

## 📂 Important Files & Context
- **Roadmap:** Check `ROADMAP.md` (copied from my internal state) for the full module dependency graph.
- **EF Core Migrations:** We are using EF Core Code-First. The latest migration is `20260708000000_AddNotificationsAndTenantConfig`. If you need to update the database, ensure you run `dotnet ef database update -p src/Tracer.Persistence -s src/Tracer.Api`.
- **Docker:** `docker-compose.yml` spins up SQL Server and Redis. The DB is named `TracerDb`.
- **Namespaces:** The project strictly enforces `file-scoped namespaces`. If you encounter `IDE0161` errors during build, ensure any generated files (like migrations) are converted to file-scoped.

---
*Good luck with M7! You have a solid, fully-implemented M6 notification system with three new API controllers and a complete CQRS stack to build upon.*
