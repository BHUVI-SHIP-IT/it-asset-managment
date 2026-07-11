# Project Tracer - Current State & Handoff

Welcome! This document is the updated handoff for the current workspace state.

## Overall Status
**Current Active Phase:** `M7 Hardening / Authorization & Inventory Stabilization`
**Last Fully Completed Phase:** `M6 Notifications`
**Current Date Context:** `2026-07-11`

We are building an Enterprise IT Asset Management system using:
- **Backend:** ASP.NET Core 9, Clean Architecture, CQRS (MediatR), FluentValidation, EF Core 9
- **Database:** SQL Server 2022 with Temporal Tables
- **Frontend:** Angular
- **Background Jobs:** Hangfire

## Completed So Far
1. **Foundation & Auth (M0 & M1):** Core solution structure, identity schema, JWT auth, and Clean Architecture layers are in place.
2. **Master Data (M2):** CRUD support for reference entities including companies, departments, locations, manufacturers, suppliers, asset models, and status labels.
3. **Asset Core (M3):** Asset aggregate, checkout/check-in flows, and outbox integration are implemented.
4. **Inventory & Licenses (M4):**
   - Domain aggregates exist for `Consumable`, `Accessory`, `Component`, and `SoftwareLicense`.
   - FIFO logic for consumables is implemented.
   - Temporal-table migration issues were previously resolved.
5. **Financial (M5):**
   - Depreciation schedules, current value tracking, valuation job, and async financial report exports are implemented.
6. **Notifications & Settings (M6):**
   - Notifications, tenant settings, and custom fields CQRS flows are implemented.
   - `NotificationsController`, `SettingsController`, and `CustomFieldsController` exist.

## Current In-Progress Work
The latest active work is a **permanent authorization and permissions refactor** plus **inventory endpoint stabilization**.

### Authorization Hardening In Progress
- Shared backend authorization constants were introduced under `Tracer.Shared.Authorization`.
- A shared `PermissionChecker` has been added to centralize permission matching behavior.
- `PermissionAuthorizationHandler` has been updated toward a proper `SuperAdmin` bypass path.
- Role/permission seed data is being reworked to use the shared permission model.
- Controllers and Angular permission checks are being migrated to shared permission names.

### Inventory Surface Expansion In Progress
- New API controllers now exist in the working tree for:
  - `AccessoriesController`
  - `ComponentsController`
  - `CompaniesController`
  - `LicensesController`
- Angular navigation and inventory pages for accessories, components, and licenses also exist in the working tree.

## Last Observed Runtime Status
The most recent browser/API verification indicates the frontend inventory pages were being tested locally.

### Verified Working During Last Test
- `GET /inventory/licenses` loaded in the Angular app
- `GET /api/v1/notifications?page=1&pageSize=10&unreadOnly=false` returned `200 OK`
- `GET /api/v1/consumables` returned `200 OK`

### Verified Failing During Last Test
- `GET /api/v1/licenses` returned `404 Not Found`
- `GET /api/v1/accessories?pageNumber=1&pageSize=10` returned `404 Not Found`

This means the UI routes were loading, but at least some backend inventory endpoints were not yet reachable in the running app at the time of the last test.

## Current Risks / Likely Gaps
- The handoff had previously claimed the project was ready to move into pure `M7 Hardening`, but the working tree shows core feature work is still underway.
- Some inventory controllers exist in source but may not yet be fully wired, built, migrated, or running in the active API process.
- Permission seeding and authorization behavior are mid-refactor and should be treated as not yet finalized.
- There are uncommitted changes across API, application, persistence, infrastructure, and Angular frontend layers.

## Immediate Next Steps
1. Build and run the API with the current working tree to verify whether `licenses` and `accessories` endpoints are now live.
2. Finish the authorization refactor end-to-end:
   - permission constants
   - controller usage
   - frontend permission sync
   - seed data alignment
3. Apply pending EF Core migrations and reseed permissions if required.
4. Re-test inventory pages in the browser after the API is rebuilt.
5. Resume `M7 Hardening` only after feature stabilization is complete.

## Important Files
- `CURRENT_STATE.md` - this handoff
- `last task.md` - notes from the latest active authorization task
- `ROADMAP.md` - roadmap / module dependency reference
- `src/Tracer.Shared/Authorization/` - shared roles and permission constants
- `src/Tracer.Api/Controllers/v1/` - current API surface including new inventory controllers

## Practical Summary
The project is **not just in M7 hardening** right now. The real current state is:
- M6 is complete
- authorization refactoring is actively in progress
- inventory endpoints/pages were being expanded and tested
- the last observed blocker was `404` responses from `/api/v1/licenses` and `/api/v1/accessories`

Use this document as the current truth unless a newer verification run supersedes it.
