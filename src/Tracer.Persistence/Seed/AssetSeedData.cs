using Microsoft.EntityFrameworkCore;
using Tracer.Domain.Aggregates.AssetAggregate;
using Tracer.Persistence.Contexts;

namespace Tracer.Persistence.Seed;

/// <summary>
/// Idempotent demo assets (30) with a realistic checkout mix and temporal history churn.
/// </summary>
public static class AssetSeedData
{
    private const int StatusDeployable = 1;
    private const int StatusDeployed = 2;
    private const int StatusArchived = 3;
    private const int StatusBroken = 4;

    private static readonly Guid[] AssigneeIds =
    [
        UserSeedData.ItAdminId,
        UserSeedData.AssetManagerId,
        UserSeedData.HelpDeskId,
        UserSeedData.EmployeeId,
        UserSeedData.DeptManagerId,
        UserSeedData.FinanceOfficerId,
        UserSeedData.InventoryManagerId,
        UserSeedData.SalesRepId,
    ];

    private static readonly Guid[] ModelIds =
    [
        MasterDataSeedData.ModelMacBookProId,
        MasterDataSeedData.ModelLatitudeId,
        MasterDataSeedData.ModelEliteBookId,
        MasterDataSeedData.ModelThinkPadId,
        MasterDataSeedData.ModelOptiPlexId,
        MasterDataSeedData.ModelUltraSharpId,
        MasterDataSeedData.ModelIPhoneId,
        MasterDataSeedData.ModelIPadId,
        MasterDataSeedData.ModelCatalystId,
        MasterDataSeedData.ModelMagicKeyboardId,
    ];

    private static readonly Guid[] LocationIds =
    [
        MasterDataSeedData.LocationHqId,
        MasterDataSeedData.LocationAustinId,
        MasterDataSeedData.LocationChicagoId,
        MasterDataSeedData.LocationSeattleId,
        MasterDataSeedData.LocationWarehouseId,
    ];

    private static readonly (string Tag, string Name, decimal Cost)[] Catalog =
    [
        ("TRC-1001", "MacBook Pro — Design Studio", 2499.00m),
        ("TRC-1002", "Latitude 5540 — Field Engineering", 1349.00m),
        ("TRC-1003", "EliteBook 860 — Finance Analyst", 1520.00m),
        ("TRC-1004", "ThinkPad X1 — Platform Lead", 1899.00m),
        ("TRC-1005", "OptiPlex Micro — Reception Desk", 749.00m),
        ("TRC-1006", "UltraSharp 27\" — Dual Monitor A", 629.00m),
        ("TRC-1007", "iPhone 15 Pro — On-Call Rotation", 999.00m),
        ("TRC-1008", "iPad Pro — Floor Sales Kit", 899.00m),
        ("TRC-1009", "Catalyst 9300 — Core Switch Floor 2", 4899.00m),
        ("TRC-1010", "Magic Keyboard — Conference Room B", 129.00m),
        ("TRC-1011", "MacBook Pro — Backend Pair Station", 2499.00m),
        ("TRC-1012", "Latitude 5540 — QA Automation", 1349.00m),
        ("TRC-1013", "EliteBook 860 — HR Partner", 1520.00m),
        ("TRC-1014", "ThinkPad X1 — Security Review", 1899.00m),
        ("TRC-1015", "OptiPlex Micro — Lab Bench 3", 749.00m),
        ("TRC-1016", "UltraSharp 27\" — Dual Monitor B", 629.00m),
        ("TRC-1017", "iPhone 15 Pro — Facilities Manager", 999.00m),
        ("TRC-1018", "iPad Pro — Visitor Check-in", 899.00m),
        ("TRC-1019", "Catalyst 9300 — Warehouse Edge", 4899.00m),
        ("TRC-1020", "Magic Keyboard — Hot Desk Pool", 129.00m),
        ("TRC-1021", "MacBook Pro — Executive Briefing", 2499.00m),
        ("TRC-1022", "Latitude 5540 — Spare Fleet A", 1349.00m),
        ("TRC-1023", "EliteBook 860 — Spare Fleet B", 1520.00m),
        ("TRC-1024", "ThinkPad X1 — Spare Fleet C", 1899.00m),
        ("TRC-1025", "OptiPlex Micro — Spare Fleet D", 749.00m),
        ("TRC-1026", "UltraSharp 27\" — Spare Monitor Rack", 629.00m),
        ("TRC-1027", "iPhone 15 Pro — Loaner Pool", 999.00m),
        ("TRC-1028", "iPad Pro — Broken Display Unit", 899.00m),
        ("TRC-1029", "Catalyst 9300 — Decommissioned Core", 4899.00m),
        ("TRC-1030", "Magic Keyboard — RMA Pending", 129.00m),
    ];

    public static async Task EnsureSeededAsync(TracerDbContext db, CancellationToken cancellationToken = default)
    {
        var companyId = MasterDataSeedData.DefaultCompanyId;
        var purchaseBase = new DateTime(2024, 3, 15, 0, 0, 0, DateTimeKind.Utc);

        // Create missing assets first (all start Deployable).
        // Soft-deleted rows still occupy the tag (IgnoreQueryFilters), so revive them
        // instead of skipping — otherwise later FirstAsync (filtered) throws.
        for (var i = 0; i < Catalog.Length; i++)
        {
            var (tag, name, cost) = Catalog[i];
            var existing = await db.Assets.IgnoreQueryFilters()
                .FirstOrDefaultAsync(a => a.AssetTag == tag && a.CompanyId == companyId, cancellationToken);

            if (existing is not null)
            {
                if (existing.IsDeleted)
                {
                    existing.IsDeleted = false;
                    existing.DeletedAtUtc = null;
                }
                continue;
            }

            var asset = Asset.Create(
                assetTag: tag,
                name: name,
                companyId: companyId,
                assetModelId: ModelIds[i % ModelIds.Length],
                statusLabelId: StatusDeployable,
                purchaseCost: cost,
                locationId: LocationIds[i % LocationIds.Length],
                serialNumber: $"SN-{tag.Replace("-", "")}",
                purchaseDate: purchaseBase.AddDays(i * 7));

            asset.ClearDomainEvents();
            db.Assets.Add(asset);
        }

        await db.SaveChangesAsync(cancellationToken);

        // Apply lifecycle states: ~40% checked out, a few broken/archived.
        // Indices 0–11 = checked out (12/30), 27–28 broken, 28 archived wait — 28 and 29 broken, 28 archived?
        // Plan: 0-11 checkout, 27+29 Broken, 28 Archived.
        var checkoutCount = 12;
        for (var i = 0; i < checkoutCount; i++)
        {
            var tag = Catalog[i].Tag;
            var asset = await db.Assets.FirstOrDefaultAsync(a => a.AssetTag == tag && a.CompanyId == companyId, cancellationToken);
            if (asset is null)
                continue;

            if (asset.Status == AssetStatus.Deployable)
            {
                asset.Checkout(AssigneeIds[i % AssigneeIds.Length]);
                asset.UpdateDetails(
                    asset.Name,
                    asset.AssetModelId,
                    StatusDeployed,
                    asset.PurchaseCost,
                    asset.LocationId,
                    asset.SerialNumber,
                    asset.PurchaseDate,
                    asset.DepreciationId,
                    asset.Notes);
                asset.ClearDomainEvents();
            }
        }

        await MarkBrokenAsync(db, companyId, "TRC-1028", "Display cracked — awaiting depot repair", cancellationToken);
        await MarkBrokenAsync(db, companyId, "TRC-1030", "Keycaps failing — RMA filed with Apple", cancellationToken);
        await MarkArchivedAsync(db, companyId, "TRC-1029", "Replaced during core network refresh", cancellationToken);

        await db.SaveChangesAsync(cancellationToken);

        // Temporal history: churn ~10 assets (notes / checkout-checkin) so AssetsHistory has rows.
        await GenerateHistoryAsync(db, companyId, cancellationToken);
    }

    private static async Task MarkBrokenAsync(
        TracerDbContext db,
        Guid companyId,
        string tag,
        string notes,
        CancellationToken ct)
    {
        var asset = await db.Assets.FirstOrDefaultAsync(a => a.AssetTag == tag && a.CompanyId == companyId, ct);
        if (asset is null)
            return;

        if (asset.Status == AssetStatus.Deployed)
            asset.Checkin();

        if (asset.StatusLabelId == StatusBroken)
            return;

        asset.UpdateDetails(
            asset.Name,
            asset.AssetModelId,
            StatusBroken,
            asset.PurchaseCost,
            asset.LocationId,
            asset.SerialNumber,
            asset.PurchaseDate,
            asset.DepreciationId,
            notes);

        db.Entry(asset).Property(a => a.Status).CurrentValue = AssetStatus.Maintenance;
        asset.ClearDomainEvents();
    }

    private static async Task MarkArchivedAsync(
        TracerDbContext db,
        Guid companyId,
        string tag,
        string notes,
        CancellationToken ct)
    {
        var asset = await db.Assets.FirstOrDefaultAsync(a => a.AssetTag == tag && a.CompanyId == companyId, ct);
        if (asset is null)
            return;

        if (asset.Status == AssetStatus.Archived)
            return;

        if (asset.Status == AssetStatus.Deployed)
            asset.Checkin();

        asset.Retire();
        asset.UpdateDetails(
            asset.Name,
            asset.AssetModelId,
            StatusArchived,
            asset.PurchaseCost,
            asset.LocationId,
            asset.SerialNumber,
            asset.PurchaseDate,
            asset.DepreciationId,
            notes);
        asset.ClearDomainEvents();
    }

    private static async Task GenerateHistoryAsync(TracerDbContext db, Guid companyId, CancellationToken ct)
    {
        // Only run once: marker note on TRC-1001 after history pass.
        var marker = await db.Assets.FirstOrDefaultAsync(a => a.AssetTag == "TRC-1001" && a.CompanyId == companyId, ct);
        if (marker is null)
            return;
        if (marker.Notes is not null && marker.Notes.Contains("seed-history-v1", StringComparison.Ordinal))
            return;

        // Assets TRC-1001..TRC-1010: touch twice to create temporal versions.
        for (var i = 0; i < 10; i++)
        {
            var tag = Catalog[i].Tag;
            var asset = await db.Assets.FirstAsync(a => a.AssetTag == tag && a.CompanyId == companyId, ct);

            var wasDeployed = asset.Status == AssetStatus.Deployed;
            var assignee = asset.AssignedUserId;

            if (wasDeployed)
            {
                asset.Checkin();
                asset.ClearDomainEvents();
                await db.SaveChangesAsync(ct);

                asset.Checkout(assignee ?? AssigneeIds[i % AssigneeIds.Length]);
                asset.UpdateDetails(
                    asset.Name,
                    asset.AssetModelId,
                    StatusDeployed,
                    asset.PurchaseCost,
                    asset.LocationId,
                    asset.SerialNumber,
                    asset.PurchaseDate,
                    asset.DepreciationId,
                    i == 0 ? "seed-history-v1; returned and reissued" : "Reissued after inventory audit");
                asset.ClearDomainEvents();
                await db.SaveChangesAsync(ct);
            }
            else
            {
                asset.UpdateDetails(
                    asset.Name,
                    asset.AssetModelId,
                    asset.StatusLabelId,
                    asset.PurchaseCost,
                    asset.LocationId,
                    asset.SerialNumber,
                    asset.PurchaseDate,
                    asset.DepreciationId,
                    i == 0 ? "seed-history-v1; inventory verified" : "Inventory verified");
                asset.ClearDomainEvents();
                await db.SaveChangesAsync(ct);

                // Second version
                asset.Checkout(AssigneeIds[i % AssigneeIds.Length]);
                asset.Checkin();
                asset.UpdateDetails(
                    asset.Name,
                    asset.AssetModelId,
                    StatusDeployable,
                    asset.PurchaseCost,
                    asset.LocationId,
                    asset.SerialNumber,
                    asset.PurchaseDate,
                    asset.DepreciationId,
                    asset.Notes);
                asset.ClearDomainEvents();
                await db.SaveChangesAsync(ct);
            }
        }
    }
}
