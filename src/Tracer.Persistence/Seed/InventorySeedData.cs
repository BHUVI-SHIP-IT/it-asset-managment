using Microsoft.EntityFrameworkCore;
using Tracer.Domain.Aggregates.DepreciationAggregate;
using Tracer.Domain.Aggregates.InventoryAggregate;
using Tracer.Domain.Aggregates.LicenseAggregate;
using Tracer.Persistence.Contexts;

namespace Tracer.Persistence.Seed;

/// <summary>
/// Idempotent demo inventory: consumables, components, accessories, licenses, seats, depreciation.
/// </summary>
public static class InventorySeedData
{
    public static readonly Guid DepreciationLaptopId = Guid.Parse("f0000000-0000-0000-0000-000000000001");
    public static readonly Guid DepreciationPhoneId = Guid.Parse("f0000000-0000-0000-0000-000000000002");
    public static readonly Guid DepreciationNetworkId = Guid.Parse("f0000000-0000-0000-0000-000000000003");

    public static readonly Guid LicenseOfficeId = Guid.Parse("e1000000-0000-0000-0000-000000000001");
    public static readonly Guid LicenseAdobeId = Guid.Parse("e1000000-0000-0000-0000-000000000002");
    public static readonly Guid LicenseWindowsId = Guid.Parse("e1000000-0000-0000-0000-000000000003");
    public static readonly Guid LicenseSlackId = Guid.Parse("e1000000-0000-0000-0000-000000000004");
    public static readonly Guid LicenseZoomId = Guid.Parse("e1000000-0000-0000-0000-000000000005");
    public static readonly Guid LicenseJetBrainsId = Guid.Parse("e1000000-0000-0000-0000-000000000006");

    public static async Task EnsureSeededAsync(TracerDbContext db, CancellationToken cancellationToken = default)
    {
        var companyId = MasterDataSeedData.DefaultCompanyId;
        var now = DateTime.UtcNow;

        await EnsureDepreciationsAsync(db, companyId, cancellationToken);
        await EnsureConsumablesAsync(db, companyId, cancellationToken);
        await EnsureComponentsAsync(db, companyId, cancellationToken);
        await EnsureAccessoriesAsync(db, companyId, cancellationToken);
        await EnsureLicensesAsync(db, companyId, now, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        await AssignInventoryAsync(db, companyId, cancellationToken);
        await ApplyAssetDepreciationAsync(db, companyId, now, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task EnsureDepreciationsAsync(TracerDbContext db, Guid companyId, CancellationToken ct)
    {
        async Task Ensure(Guid id, string name, int months, decimal min)
        {
            if (await db.Depreciations.IgnoreQueryFilters().AnyAsync(d => d.Id == id, ct))
                return;

            var entity = Depreciation.Create(id, name, months, companyId, min);
            db.Depreciations.Add(entity);
        }

        await Ensure(DepreciationLaptopId, "Straight-line 3yr — Laptops", 36, 100m);
        await Ensure(DepreciationPhoneId, "Straight-line 2yr — Mobile", 24, 50m);
        await Ensure(DepreciationNetworkId, "Straight-line 5yr — Network", 60, 200m);
    }

    private static async Task EnsureConsumablesAsync(TracerDbContext db, Guid companyId, CancellationToken ct)
    {
        var rows = new (string Name, int Qty, decimal Cost, int Reorder)[]
        {
            ("HP LaserJet Toner 26A", 40, 89.99m, 10),
            ("USB-C Charge Cable 2m", 120, 12.50m, 25),
            ("Logitech Wireless Mouse", 35, 24.99m, 8),
            ("AA Alkaline Batteries (24pk)", 60, 14.99m, 15),
            ("Disinfectant Wipes", 80, 8.49m, 20),
            ("Cat6 Ethernet Patch 3m", 90, 6.99m, 20),
            ("Laptop Cleaning Kit", 25, 18.00m, 5),
            ("Thermal Label Rolls", 45, 22.50m, 10),
        };

        foreach (var (name, qty, cost, reorder) in rows)
        {
            if (await db.Consumables.IgnoreQueryFilters().AnyAsync(c => c.CompanyId == companyId && c.Name == name, ct))
                continue;
            db.Consumables.Add(Consumable.Create(name, companyId, qty, cost, reorder));
        }
    }

    private static async Task EnsureComponentsAsync(TracerDbContext db, Guid companyId, CancellationToken ct)
    {
        var rows = new (string Name, int Qty, decimal Cost, Guid? ModelId)[]
        {
            ("16GB DDR5 SODIMM", 28, 79.00m, MasterDataSeedData.ModelLatitudeId),
            ("32GB DDR5 SODIMM", 18, 149.00m, MasterDataSeedData.ModelMacBookProId),
            ("1TB NVMe SSD", 22, 99.00m, MasterDataSeedData.ModelThinkPadId),
            ("512GB NVMe SSD", 30, 59.00m, MasterDataSeedData.ModelEliteBookId),
            ("65W USB-C GaN Charger", 40, 45.00m, MasterDataSeedData.ModelMacBookProId),
            ("90W Dell Dock Charger", 15, 69.00m, MasterDataSeedData.ModelLatitudeId),
            ("Wi-Fi 6E AX210 Module", 12, 35.00m, MasterDataSeedData.ModelOptiPlexId),
            ("Magic Trackpad", 10, 129.00m, MasterDataSeedData.ModelMacBookProId),
        };

        foreach (var (name, qty, cost, modelId) in rows)
        {
            if (await db.Components.IgnoreQueryFilters().AnyAsync(c => c.CompanyId == companyId && c.Name == name, ct))
                continue;
            db.Components.Add(Component.Create(name, companyId, qty, cost, modelId));
        }
    }

    private static async Task EnsureAccessoriesAsync(TracerDbContext db, Guid companyId, CancellationToken ct)
    {
        var rows = new (string Name, int Qty, decimal Cost)[]
        {
            ("Targus 15\" Laptop Bag", 20, 49.99m),
            ("Dell WD19TBS Docking Station", 14, 279.00m),
            ("Logitech MX Keys Keyboard", 18, 119.00m),
            ("Apple Magic Keyboard", 12, 99.00m),
            ("Anker USB-C Hub 7-in-1", 25, 39.99m),
            ("Jabra Evolve2 65 Headset", 16, 189.00m),
        };

        foreach (var (name, qty, cost) in rows)
        {
            if (await db.Accessories.IgnoreQueryFilters().AnyAsync(a => a.CompanyId == companyId && a.Name == name, ct))
                continue;
            db.Accessories.Add(Accessory.Create(name, companyId, qty, cost));
        }
    }

    private static async Task EnsureLicensesAsync(TracerDbContext db, Guid companyId, DateTime now, CancellationToken ct)
    {
        var rows = new (Guid Id, string Name, Guid? Mfr, int Seats, decimal Cost, DateTime? Exp)[]
        {
            (LicenseOfficeId, "Microsoft 365 E3", MasterDataSeedData.ManufacturerDellId, 50, 36.00m * 50, now.AddMonths(8)),
            (LicenseAdobeId, "Adobe Creative Cloud", MasterDataSeedData.ManufacturerAppleId, 15, 79.99m * 15, now.AddDays(20)),
            (LicenseWindowsId, "Windows 11 Pro", MasterDataSeedData.ManufacturerDellId, 100, 199.00m, now.AddYears(2)),
            (LicenseSlackId, "Slack Business+", null, 40, 12.50m * 40, now.AddMonths(-1)), // expired
            (LicenseZoomId, "Zoom Workplace Pro", null, 30, 19.99m * 30, now.AddDays(45)),
            (LicenseJetBrainsId, "JetBrains All Products Pack", null, 12, 649.00m, now.AddMonths(4)),
        };

        foreach (var (id, name, mfr, seats, cost, exp) in rows)
        {
            if (await db.SoftwareLicenses.IgnoreQueryFilters().AnyAsync(l => l.Id == id, ct))
                continue;

            var license = SoftwareLicense.Create(id, name, companyId, mfr, seats, cost, exp);
            db.SoftwareLicenses.Add(license);
        }
    }

    private static async Task AssignInventoryAsync(TracerDbContext db, Guid companyId, CancellationToken ct)
    {
        // Assign consumables (last assignee marker) without wiping all stock.
        await AssignConsumableAsync(db, companyId, "Logitech Wireless Mouse", UserSeedData.EmployeeId, 1, ct);
        await AssignConsumableAsync(db, companyId, "USB-C Charge Cable 2m", UserSeedData.HelpDeskId, 2, ct);
        await AssignConsumableAsync(db, companyId, "HP LaserJet Toner 26A", UserSeedData.ItAdminId, 1, ct);

        await AssignComponentAsync(db, companyId, "16GB DDR5 SODIMM", UserSeedData.EmployeeId, ct);
        await AssignComponentAsync(db, companyId, "65W USB-C GaN Charger", UserSeedData.SalesRepId, ct);
        await AssignComponentAsync(db, companyId, "1TB NVMe SSD", UserSeedData.AssetManagerId, ct);

        await AssignAccessoryAsync(db, companyId, "Targus 15\" Laptop Bag", UserSeedData.EmployeeId, ct);
        await AssignAccessoryAsync(db, companyId, "Dell WD19TBS Docking Station", UserSeedData.DeptManagerId, ct);
        await AssignAccessoryAsync(db, companyId, "Jabra Evolve2 65 Headset", UserSeedData.HelpDeskId, ct);

        // License seats — some users multi-licensed, Noah/Finance none for seats beyond assets.
        await EnsureSeatAsync(db, LicenseOfficeId, UserSeedData.EmployeeId, ct);
        await EnsureSeatAsync(db, LicenseOfficeId, UserSeedData.ItAdminId, ct);
        await EnsureSeatAsync(db, LicenseOfficeId, UserSeedData.HelpDeskId, ct);
        await EnsureSeatAsync(db, LicenseAdobeId, UserSeedData.SalesRepId, ct);
        await EnsureSeatAsync(db, LicenseAdobeId, UserSeedData.DeptManagerId, ct);
        await EnsureSeatAsync(db, LicenseWindowsId, UserSeedData.EmployeeId, ct);
        await EnsureSeatAsync(db, LicenseJetBrainsId, UserSeedData.ItAdminId, ct);
        await EnsureSeatAsync(db, LicenseZoomId, UserSeedData.HelpDeskId, ct);
        // FinanceOfficer intentionally has no license seats (mix of none)
    }

    private static async Task AssignConsumableAsync(
        TracerDbContext db, Guid companyId, string name, Guid userId, int qty, CancellationToken ct)
    {
        var item = await db.Consumables.FirstOrDefaultAsync(c => c.CompanyId == companyId && c.Name == name, ct);
        if (item is null || item.AssignedUserId == userId)
            return;
        if (item.TotalQuantity < qty)
            return;
        item.AssignTo(userId, qty);
    }

    private static async Task AssignComponentAsync(
        TracerDbContext db, Guid companyId, string name, Guid userId, CancellationToken ct)
    {
        var item = await db.Components.FirstOrDefaultAsync(c => c.CompanyId == companyId && c.Name == name, ct);
        if (item is null || item.AssignedUserId == userId)
            return;
        if (item.TotalQuantity < 1)
            return;
        item.AssignTo(userId);
    }

    private static async Task AssignAccessoryAsync(
        TracerDbContext db, Guid companyId, string name, Guid userId, CancellationToken ct)
    {
        var item = await db.Accessories.FirstOrDefaultAsync(a => a.CompanyId == companyId && a.Name == name, ct);
        if (item is null || item.AssignedUserId == userId)
            return;
        if (item.TotalQuantity < 1)
            return;
        item.AssignTo(userId);
    }

    private static async Task EnsureSeatAsync(
        TracerDbContext db, Guid licenseId, Guid userId, CancellationToken ct)
    {
        if (await db.LicenseSeats.AnyAsync(s => s.SoftwareLicenseId == licenseId && s.AssignedUserId == userId, ct))
            return;
        db.LicenseSeats.Add(LicenseSeat.AllocateToUser(licenseId, userId));
    }

    private static async Task ApplyAssetDepreciationAsync(
        TracerDbContext db, Guid companyId, DateTime now, CancellationToken ct)
    {
        var deps = await db.Depreciations
            .Where(d => d.CompanyId == companyId)
            .ToDictionaryAsync(d => d.Id, ct);

        if (deps.Count == 0)
            return;

        var assets = await db.Assets.Where(a => a.CompanyId == companyId).ToListAsync(ct);
        foreach (var asset in assets)
        {
            var depId = asset.Name.Contains("iPhone", StringComparison.OrdinalIgnoreCase)
                || asset.Name.Contains("iPad", StringComparison.OrdinalIgnoreCase)
                ? DepreciationPhoneId
                : asset.Name.Contains("Catalyst", StringComparison.OrdinalIgnoreCase)
                    ? DepreciationNetworkId
                    : DepreciationLaptopId;

            if (!deps.TryGetValue(depId, out var schedule))
                continue;

            if (asset.DepreciationId != depId)
            {
                asset.UpdateDetails(
                    asset.Name,
                    asset.AssetModelId,
                    asset.StatusLabelId,
                    asset.PurchaseCost,
                    asset.LocationId,
                    asset.SerialNumber,
                    asset.PurchaseDate,
                    depId,
                    asset.Notes);
            }

            var current = schedule.ComputeCurrentValue(asset.PurchaseCost, asset.PurchaseDate, now);
            if (asset.CurrentValue != current)
                asset.UpdateCurrentValue(current);

            asset.ClearDomainEvents();
        }
    }
}
