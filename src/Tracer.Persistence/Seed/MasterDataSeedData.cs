using Microsoft.EntityFrameworkCore;
using Tracer.Domain.Entities;
using Tracer.Persistence.Contexts;

namespace Tracer.Persistence.Seed;

/// <summary>
/// Idempotent demo master data for the default company (locations, departments,
/// categories, manufacturers, asset models). Safe to re-run.
/// </summary>
public static class MasterDataSeedData
{
    public static readonly Guid DefaultCompanyId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    // Manufacturers (5)
    public static readonly Guid ManufacturerAppleId = Guid.Parse("a0000000-0000-0000-0000-000000000001");
    public static readonly Guid ManufacturerDellId = Guid.Parse("a0000000-0000-0000-0000-000000000002");
    public static readonly Guid ManufacturerHpId = Guid.Parse("a0000000-0000-0000-0000-000000000003");
    public static readonly Guid ManufacturerLenovoId = Guid.Parse("a0000000-0000-0000-0000-000000000004");
    public static readonly Guid ManufacturerCiscoId = Guid.Parse("a0000000-0000-0000-0000-000000000005");

    // Categories (6) — inventory class names used throughout the app
    public static readonly Guid CategoryLaptopsId = Guid.Parse("b0000000-0000-0000-0000-000000000001");
    public static readonly Guid CategoryDesktopsId = Guid.Parse("b0000000-0000-0000-0000-000000000002");
    public static readonly Guid CategoryMonitorsId = Guid.Parse("b0000000-0000-0000-0000-000000000003");
    public static readonly Guid CategoryMobileDevicesId = Guid.Parse("b0000000-0000-0000-0000-000000000004");
    public static readonly Guid CategoryNetworkEquipmentId = Guid.Parse("b0000000-0000-0000-0000-000000000005");
    public static readonly Guid CategoryAccessoriesId = Guid.Parse("b0000000-0000-0000-0000-000000000006");

    // Asset models (10)
    public static readonly Guid ModelMacBookProId = Guid.Parse("c0000000-0000-0000-0000-000000000001");
    public static readonly Guid ModelLatitudeId = Guid.Parse("c0000000-0000-0000-0000-000000000002");
    public static readonly Guid ModelEliteBookId = Guid.Parse("c0000000-0000-0000-0000-000000000003");
    public static readonly Guid ModelThinkPadId = Guid.Parse("c0000000-0000-0000-0000-000000000004");
    public static readonly Guid ModelOptiPlexId = Guid.Parse("c0000000-0000-0000-0000-000000000005");
    public static readonly Guid ModelUltraSharpId = Guid.Parse("c0000000-0000-0000-0000-000000000006");
    public static readonly Guid ModelIPhoneId = Guid.Parse("c0000000-0000-0000-0000-000000000007");
    public static readonly Guid ModelIPadId = Guid.Parse("c0000000-0000-0000-0000-000000000008");
    public static readonly Guid ModelCatalystId = Guid.Parse("c0000000-0000-0000-0000-000000000009");
    public static readonly Guid ModelMagicKeyboardId = Guid.Parse("c0000000-0000-0000-0000-00000000000a");

    // Locations (5)
    public static readonly Guid LocationHqId = Guid.Parse("d0000000-0000-0000-0000-000000000001");
    public static readonly Guid LocationAustinId = Guid.Parse("d0000000-0000-0000-0000-000000000002");
    public static readonly Guid LocationChicagoId = Guid.Parse("d0000000-0000-0000-0000-000000000003");
    public static readonly Guid LocationSeattleId = Guid.Parse("d0000000-0000-0000-0000-000000000004");
    public static readonly Guid LocationWarehouseId = Guid.Parse("d0000000-0000-0000-0000-000000000005");

    // Departments (5)
    public static readonly Guid DepartmentEngineeringId = Guid.Parse("e0000000-0000-0000-0000-000000000001");
    public static readonly Guid DepartmentFinanceId = Guid.Parse("e0000000-0000-0000-0000-000000000002");
    public static readonly Guid DepartmentHrId = Guid.Parse("e0000000-0000-0000-0000-000000000003");
    public static readonly Guid DepartmentSalesId = Guid.Parse("e0000000-0000-0000-0000-000000000004");
    public static readonly Guid DepartmentOperationsId = Guid.Parse("e0000000-0000-0000-0000-000000000005");

    /// <summary>Legacy aliases kept for any callers referencing old constants.</summary>
    public static readonly Guid CategoryPhonesId = CategoryMobileDevicesId;
    public static readonly Guid ModelMacBookId = ModelMacBookProId;
    public static readonly Guid ModelIPhoneLegacyId = ModelIPhoneId;
    public static readonly Guid LocationBranchId = LocationAustinId;
    public static readonly Guid LocationRemoteId = LocationSeattleId;

    public static async Task EnsureSeededAsync(TracerDbContext db, CancellationToken cancellationToken = default)
    {
        var seededAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        await EnsureManufacturersAsync(db, seededAt, cancellationToken);
        await EnsureCategoriesAsync(db, seededAt, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        await EnsureAssetModelsAsync(db, seededAt, cancellationToken);
        await EnsureLocationsAsync(db, seededAt, cancellationToken);
        await EnsureDepartmentsAsync(db, seededAt, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task EnsureManufacturersAsync(TracerDbContext db, DateTime seededAt, CancellationToken ct)
    {
        var rows = new (Guid Id, string Name)[]
        {
            (ManufacturerAppleId, "Apple"),
            (ManufacturerDellId, "Dell Technologies"),
            (ManufacturerHpId, "HP Inc."),
            (ManufacturerLenovoId, "Lenovo"),
            (ManufacturerCiscoId, "Cisco Systems"),
        };

        foreach (var (id, name) in rows)
        {
            if (await db.Manufacturers.IgnoreQueryFilters().AnyAsync(m => m.Id == id, ct))
                continue;

            db.Manufacturers.Add(new Manufacturer(id)
            {
                Name = name,
                CompanyId = DefaultCompanyId,
                CreatedAtUtc = seededAt
            });
        }
    }

    private static async Task EnsureCategoriesAsync(TracerDbContext db, DateTime seededAt, CancellationToken ct)
    {
        // Rename legacy rows in place so re-runs stay idempotent and names stay realistic.
        var renames = new Dictionary<Guid, string>
        {
            [CategoryLaptopsId] = "Laptops",
            [CategoryDesktopsId] = "Desktops",
            [CategoryMonitorsId] = "Monitors",
            [CategoryMobileDevicesId] = "Mobile Devices",
            [CategoryNetworkEquipmentId] = "Network Equipment",
            [CategoryAccessoriesId] = "Accessories",
        };

        foreach (var (id, name) in renames)
        {
            var existing = await db.Categories.IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Id == id, ct);

            if (existing is null)
            {
                db.Categories.Add(new Category(id)
                {
                    Name = name,
                    CompanyId = DefaultCompanyId,
                    CreatedAtUtc = seededAt
                });
            }
            else if (!string.Equals(existing.Name, name, StringComparison.Ordinal))
            {
                // Avoid unique-name collisions with leftover "Phones" etc.
                var clash = await db.Categories.IgnoreQueryFilters()
                    .AnyAsync(c => c.CompanyId == DefaultCompanyId && c.Name == name && c.Id != id && !c.IsDeleted, ct);
                if (!clash)
                    existing.Name = name;
            }
        }
    }

    private static async Task EnsureAssetModelsAsync(TracerDbContext db, DateTime seededAt, CancellationToken ct)
    {
        var rows = new (Guid Id, string Name, Guid ManufacturerId, Guid CategoryId)[]
        {
            (ModelMacBookProId, "MacBook Pro 14\" M3", ManufacturerAppleId, CategoryLaptopsId),
            (ModelLatitudeId, "Latitude 5540", ManufacturerDellId, CategoryLaptopsId),
            (ModelEliteBookId, "EliteBook 860 G10", ManufacturerHpId, CategoryLaptopsId),
            (ModelThinkPadId, "ThinkPad X1 Carbon Gen 12", ManufacturerLenovoId, CategoryLaptopsId),
            (ModelOptiPlexId, "OptiPlex 7010 Micro", ManufacturerDellId, CategoryDesktopsId),
            (ModelUltraSharpId, "UltraSharp U2723QE", ManufacturerDellId, CategoryMonitorsId),
            (ModelIPhoneId, "iPhone 15 Pro", ManufacturerAppleId, CategoryMobileDevicesId),
            (ModelIPadId, "iPad Pro 11\"", ManufacturerAppleId, CategoryMobileDevicesId),
            (ModelCatalystId, "Catalyst 9300-48P", ManufacturerCiscoId, CategoryNetworkEquipmentId),
            (ModelMagicKeyboardId, "Magic Keyboard", ManufacturerAppleId, CategoryAccessoriesId),
        };

        foreach (var (id, name, manufacturerId, categoryId) in rows)
        {
            var existing = await db.AssetModels.IgnoreQueryFilters()
                .FirstOrDefaultAsync(a => a.Id == id, ct);

            if (existing is null)
            {
                db.AssetModels.Add(new AssetModel(id)
                {
                    Name = name,
                    CompanyId = DefaultCompanyId,
                    ManufacturerId = manufacturerId,
                    CategoryId = categoryId,
                    CreatedAtUtc = seededAt
                });
                continue;
            }

            // Align previously seeded demo rows with the current catalog (same fixed IDs).
            existing.Name = name;
            existing.ManufacturerId = manufacturerId;
            existing.CategoryId = categoryId;
        }
    }

    private static async Task EnsureLocationsAsync(TracerDbContext db, DateTime seededAt, CancellationToken ct)
    {
        var rows = new (Guid Id, string Name)[]
        {
            (LocationHqId, "San Francisco HQ"),
            (LocationAustinId, "Austin Campus"),
            (LocationChicagoId, "Chicago Office"),
            (LocationSeattleId, "Seattle Hub"),
            (LocationWarehouseId, "Central Warehouse"),
        };

        foreach (var (id, name) in rows)
        {
            var existing = await db.Locations.IgnoreQueryFilters()
                .FirstOrDefaultAsync(l => l.Id == id, ct);

            if (existing is null)
            {
                db.Locations.Add(new Location(id)
                {
                    Name = name,
                    CompanyId = DefaultCompanyId,
                    CreatedAtUtc = seededAt
                });
            }
            else if (!string.Equals(existing.Name, name, StringComparison.Ordinal))
            {
                var clash = await db.Locations.IgnoreQueryFilters()
                    .AnyAsync(l => l.CompanyId == DefaultCompanyId && l.Name == name && l.Id != id && !l.IsDeleted, ct);
                if (!clash)
                    existing.Name = name;
            }
        }
    }

    private static async Task EnsureDepartmentsAsync(TracerDbContext db, DateTime seededAt, CancellationToken ct)
    {
        var rows = new (Guid Id, string Name)[]
        {
            (DepartmentEngineeringId, "Engineering"),
            (DepartmentFinanceId, "Finance"),
            (DepartmentHrId, "Human Resources"),
            (DepartmentSalesId, "Sales"),
            (DepartmentOperationsId, "Operations"),
        };

        foreach (var (id, name) in rows)
        {
            if (await db.Departments.IgnoreQueryFilters().AnyAsync(d => d.Id == id, ct))
                continue;

            db.Departments.Add(new Department(id)
            {
                Name = name,
                CompanyId = DefaultCompanyId,
                CreatedAtUtc = seededAt
            });
        }
    }
}
