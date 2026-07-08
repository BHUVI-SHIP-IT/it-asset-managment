# Project Tracer - Current State & Handoff

Welcome! If you are another AI model, IDE, or developer taking over this project, this document serves as the single source of truth for where we left off.

## 📍 Overall Status
**Current Active Phase:** `M6 Notifications`
**Last Completed Phase:** `M5 Financial`

We are building an Enterprise IT Asset Management system using:
- **Backend:** ASP.NET Core 9, Clean Architecture, CQRS (MediatR), FluentValidation, EF Core 9
- **Database:** SQL Server 2022 (with Temporal Tables)
- **Background Jobs:** Hangfire

## 🏁 Completed So Far (M0 - M4)
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

## 🚀 Next Steps (M6 Notifications)
The immediate next step is to begin Phase **M6 Notifications**. This involves:
1. **Notification Channels:** Setting up a notification center with webhooks (Slack/Teams/Email).
2. **Tenant Configuration:** Adding tenant-level settings for custom fields and notification rules.

## 📂 Important Files & Context
- **Roadmap:** Check `ROADMAP.md` (copied from my internal state) for the full module dependency graph.
- **EF Core Migrations:** We are using EF Core Code-First. The current migration is `20260707140456_AddInventoryAggregates`. If you need to update the database, ensure you run `dotnet ef database update -p src/Tracer.Persistence -s src/Tracer.Api`.
- **Docker:** `docker-compose.yml` spins up SQL Server and Redis. The DB is named `TracerDb`.
- **Namespaces:** The project strictly enforces `file-scoped namespaces`. If you encounter `IDE0161` errors during build, ensure any generated files (like migrations) are converted to file-scoped.

---
*Good luck with M5! You have a solid, fully-tested domain and a clean, compiling database schema to build upon.*
