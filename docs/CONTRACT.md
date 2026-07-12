# Tracer API Contract Reference

Living contract between Angular (`Tracer.Web`), .NET API (`Tracer.Api`), and SQL Server schema.
Update this file when routes, DTOs, or tables change.

**Base URL:** `/api/v1` (dev proxy → `http://localhost:5001`)  
**Auth:** `Authorization: Bearer <accessToken>` on all endpoints except login/refresh  
**JSON enums (request bodies):** camelCase via `JsonStringEnumConverter` (e.g. `"text"`, `"deployable"`)  
**Note:** Some list DTO string fields use `.ToString()` and return PascalCase (e.g. asset `status`: `"Deployable"`).

**Pagination envelope (lists):**
```json
{ "items": [], "totalCount": 0, "pageNumber": 1, "pageSize": 10 }
```

**Create responses:** bare id (`Guid` string or `int`), not `{ "id": "..." }`.

---

## Auth

| Method | Path | Request | Response |
|--------|------|---------|----------|
| POST | `/auth/login` | `{ email, password }` | `{ accessToken, expiresIn }` |
| POST | `/auth/refresh` | `{ token }` | `{ accessToken, expiresIn }` |
| GET | `/auth/me` | — | `{ id, fullName, email, role, permissions[], companyId }` |

**DB:** `Users`, `Roles`, `Permissions`, `RolePermissions`

---

## Dashboard

| Method | Path | Request | Response |
|--------|------|---------|----------|
| GET | `/dashboard/metrics` | — | `{ totalAssets, activeAssets, pendingCheckouts, overdueCheckins }` |

- `activeAssets` = `Status == Deployed`
- `pendingCheckouts` = `Status == Pending`
- `overdueCheckins` = Deployed with `CheckedOutAtUtc` older than 90 days  
**Auth:** `Assets.View`  
**DB:** `Assets`

---

## Assets

| Method | Path | Request | Response |
|--------|------|---------|----------|
| GET | `/assets` | Query: `pageNumber`, `pageSize`, `searchTerm?`, `status?`, `statusLabelId?`, `locationId?`, `sortBy?`, `sortDescending?` | Paginated `AssetDto` |
| POST | `/assets` | `CreateAssetCommand` | `Guid` |
| GET | `/assets/{id}` | — | `AssetDetailDto` |
| PUT | `/assets/{id}` | `UpdateAssetCommand` | 204 |
| DELETE | `/assets/{id}` | — | 204 |
| POST | `/assets/{id}/checkout` | `{ assetId, userId }` (route id must match `assetId`) | 204 |
| POST | `/assets/{id}/checkin` | (route id only; body ignored) | 204 |
| GET | `/assets/{id}/history` | — | `AssetHistoryDto[]` |

**CreateAssetCommand:** `assetTag`, `name`, `assetModelId`, `statusLabelId`, `purchaseCost`, `locationId?`, `serialNumber?`, `purchaseDate?`, `depreciationId?`  
**UpdateAssetCommand:** `id`, `name`, `assetModelId`, `statusLabelId`, `purchaseCost`, `locationId?`, `serialNumber?`, `purchaseDate?`, `depreciationId?`, `notes?`  
**AssetStatus filter values:** `Pending`, `Deployable`, `Deployed`, `Maintenance`, `Archived`  
**DB:** `Assets` (+ temporal `AssetsHistory`). Checkout is columns on `Assets` (`AssignedUserId`, `CheckedOutAtUtc`, `LastCheckinAtUtc`, `Status`) — no checkout table.

---

## Users

| Method | Path | Request | Response |
|--------|------|---------|----------|
| GET | `/users` | `pageNumber`, `pageSize` | Paginated `UserDto` |
| GET | `/users/roles` | — | `RoleDto[]` |
| GET | `/users/{id}` | — | `UserDto` |
| POST | `/users` | `{ fullName, email, password, roleId }` | `Guid` |

**DB:** `Users`, `Roles`

---

## Master data (Name-only CRUD)

Shared shape for Categories, Locations, Departments, Manufacturers, Suppliers, Asset Models:

| Method | Path pattern | Body | Response |
|--------|--------------|------|----------|
| GET | `/{resource}` | `pageNumber`, `pageSize` | Paginated `{ id, name }` |
| GET | `/{resource}/{id}` | — | `{ id, name }` |
| POST | `/{resource}` | `{ name }` | bare `Guid` |
| PUT | `/{resource}/{id}` | `{ id, name }` | 204 |
| DELETE | `/{resource}/{id}` | — | 204 |

| Resource | Path | DB table |
|----------|------|----------|
| Categories | `/categories` | `Categories` |
| Locations | `/locations` | `Locations` |
| Departments | `/departments` | `Departments` |
| Manufacturers | `/manufacturers` | `Manufacturers` |
| Suppliers | `/suppliers` | `Suppliers` |
| Asset models | `/asset-models` | `AssetModels` (also requires `ManufacturerId`, `CategoryId` — create auto-picks first of each) |
| Companies | `/companies` | `Companies` (Settings permissions; create returns bare Guid via `Ok`) |

### Status labels

Path: `/status-labels` — **Id is `int`**

| Method | Body / notes |
|--------|----------------|
| POST | `{ name, isDeployable, isPending, isArchived }` → bare `int` |
| PUT | `{ id, name, isDeployable, isPending, isArchived }` |
| GET | DTO includes the three flags |

**DB:** `StatusLabels` (`IsDeployable`, `IsPending`, `IsArchived`)

---

## Inventory

### Consumables — `/consumables`

| Method | Path | Body | Response |
|--------|------|------|----------|
| GET | `/consumables` | — | `ConsumableDto[]` |
| POST | `/consumables` | `{ name, totalQuantity, purchaseCost }` | `int` |
| POST | `/consumables/{id}/checkout` | `{ consumableId, assignedToUserId, quantity }` | 204 |

### Accessories — `/accessories` / Components — `/components`

Paginated list; create/update/delete. Create body: `{ name, totalQuantity, purchaseCost }`. Create returns `int`.

### Licenses — `/licenses`

| Method | Path | Body | Response |
|--------|------|------|----------|
| GET | `/licenses` | — | `LicenseDto[]` |
| GET | `/licenses/{id}` | — | `LicenseDto` |
| POST | `/licenses` | `{ name, manufacturerId?, totalSeats, purchaseCost, expirationDate?, notes? }` | `Guid` |

**DB:** `Consumables`, `Accessories`, `Components`, `SoftwareLicenses`, `LicenseSeats`

---

## Custom fields — `/custom-fields`

| Method | Path | Body | Response |
|--------|------|------|----------|
| GET | `/custom-fields` | — | `CustomFieldDto[]` |
| POST | `/custom-fields` | `{ name, fieldType, isRequired, options? }` | `Guid` |
| PUT | `/custom-fields/{id}` | same | 204 |
| DELETE | `/custom-fields/{id}` | — | 204 |
| GET | `/custom-fields/values/{entityId}` | — | values |
| PUT | `/custom-fields/{id}/values/{entityId}` | `{ value }` | value id |

**`fieldType` (JSON):** `text` \| `number` \| `date` \| `boolean` \| `dropdown` (camelCase)  
**DB:** `CustomFields`, `CustomFieldValues`

---

## Notifications — `/notifications`

| Method | Path | Response |
|--------|------|----------|
| GET | `/notifications?page&pageSize&unreadOnly` | `NotificationDto[]` |
| POST | `/notifications/{id}/read` | 204 |
| DELETE | `/notifications/{id}` | 204 |

**NotificationDto:** `id`, `title`, `body`, `severity`, `channel`, `status`, `recipient?`, `failureReason?`, `isRead`, `sentAtUtc?`, `createdAtUtc`  
**DB:** `Notifications`

---

## Settings — `/settings`

| Method | Path | Body | Response |
|--------|------|------|----------|
| GET | `/settings` | — | `TenantSettingDto[]` |
| PUT | `/settings/{key}` | `{ value }` | `Guid` |
| DELETE | `/settings/{key}` | — | 204 |

**DB:** `TenantSettings`

---

## Reports & depreciation

| Method | Path | Body | Response |
|--------|------|------|----------|
| GET | `/reports` | — | report list |
| POST | `/reports` | `{ reportType? }` | 202 + id |
| GET | `/reports/{id}/download` | — | CSV blob |
| GET | `/depreciation` | — | depreciation list |
| POST | `/depreciation` | `{ name, months, minimumValue }` | `{ id }` |

**DB:** `ReportExports`, `Depreciations`

---

## Core schema (quick)

| Table | Key columns |
|-------|-------------|
| Companies | Id, Name |
| Users | Id, Email, PasswordHash, CompanyId, RoleId, RefreshToken* |
| Categories / Locations / … | Id, Name, CompanyId + audit/soft-delete |
| AssetModels | Id, Name, CompanyId, ManufacturerId, CategoryId |
| StatusLabels | Id (int), Name, IsDeployable, IsPending, IsArchived |
| Assets | AssetTag, Status, FKs, AssignedUserId, CheckedOutAtUtc, LastCheckinAtUtc, PurchaseCost, … |
| Accessories / Components / Consumables | Id (int), Name, CompanyId, TotalQuantity, PurchaseCost |
| SoftwareLicenses / LicenseSeats | seats assignment |
| CustomFields / CustomFieldValues | FieldType as string |
| Notifications | Name(=title), Body, Severity, Channel, Status, IsRead |
| TenantSettings | CompanyId, Key, Value |
| Depreciations / ReportExports | financial |
| OutboxMessages | messaging |

**Relationships:** Inventory (`Accessories`/`Components`/`Consumables`), `SoftwareLicenses`, `Depreciations`, `CustomFields`, `TenantSettings`, `Notifications`, and `ReportExports` have FK constraints to `Companies` (and related assignees where applicable). `LicenseSeats` FKs to `Users` / `Assets` for assignments.

**Concurrency:** Auditable tables use SQL Server `rowversion` for `RowVersion` (optimistic concurrency).

**Soft-delete:** `AuditableEntityInterceptor` soft-deletes both `AuditableEntity<Guid>` and `AuditableEntity<int>` (including StatusLabels) on `Remove()`.

---

## Drift-prevention checklist

1. Prefer one create-response shape (bare id) for all resources.  
2. Prefer one list envelope (`pageNumber`, not `page`).  
3. Keep Angular interfaces in `*.service.ts` aligned with Application DTOs.  
4. Run [`tests/Tracer.Api.http`](../tests/Tracer.Api.http) after contract changes.  
5. Do not add FE form fields without matching command + entity + migration.  
6. After Fluent API relationship changes, add an EF migration immediately (avoid shadow FKs like the former `DepreciationId1`).
