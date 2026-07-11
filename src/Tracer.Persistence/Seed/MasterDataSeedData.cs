using Microsoft.EntityFrameworkCore;
using Tracer.Domain.Entities;
using Tracer.Persistence.Contexts;

namespace Tracer.Persistence.Seed;

/// <summary>
/// Seeds demo master data for the default company so asset create dropdowns work out of the box.
/// Used by Development runtime ensure-seed (migrations are not applied on API startup).
/// </summary>
public static class MasterDataSeedData
{
    public static readonly Guid DefaultCompanyId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public static readonly Guid ManufacturerAppleId = Guid.Parse("a0000000-0000-0000-0000-000000000001");
    public static readonly Guid ManufacturerDellId = Guid.Parse("a0000000-0000-0000-0000-000000000002");

    public static readonly Guid CategoryLaptopsId = Guid.Parse("b0000000-0000-0000-0000-000000000001");
    public static readonly Guid CategoryPhonesId = Guid.Parse("b0000000-0000-0000-0000-000000000002");
    public static readonly Guid CategoryMonitorsId = Guid.Parse("b0000000-0000-0000-0000-000000000003");

    public static readonly Guid ModelMacBookId = Guid.Parse("c0000000-0000-0000-0000-000000000001");
    public static readonly Guid ModelLatitudeId = Guid.Parse("c0000000-0000-0000-0000-000000000002");
    public static readonly Guid ModelIPhoneId = Guid.Parse("c0000000-0000-0000-0000-000000000003");
    public static readonly Guid ModelUltraSharpId = Guid.Parse("c0000000-0000-0000-0000-000000000004");

    public static readonly Guid LocationHqId = Guid.Parse("d0000000-0000-0000-0000-000000000001");
    public static readonly Guid LocationBranchId = Guid.Parse("d0000000-0000-0000-0000-000000000002");
    public static readonly Guid LocationRemoteId = Guid.Parse("d0000000-0000-0000-0000-000000000003");

    /// <summary>Idempotent insert of lookup rows when the tenant has none yet.</summary>
    public static async Task EnsureSeededAsync(TracerDbContext db, CancellationToken cancellationToken = default)
    {
        var seededAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        if (!await db.Manufacturers.IgnoreQueryFilters().AnyAsync(m => m.Id == ManufacturerAppleId, cancellationToken))
        {
            db.Manufacturers.AddRange(
                new Manufacturer(ManufacturerAppleId)
                {
                    Name = "Apple",
                    CompanyId = DefaultCompanyId,
                    CreatedAtUtc = seededAt
                },
                new Manufacturer(ManufacturerDellId)
                {
                    Name = "Dell",
                    CompanyId = DefaultCompanyId,
                    CreatedAtUtc = seededAt
                });
        }

        if (!await db.Categories.IgnoreQueryFilters().AnyAsync(c => c.Id == CategoryLaptopsId, cancellationToken))
        {
            db.Categories.AddRange(
                new Category(CategoryLaptopsId)
                {
                    Name = "Laptops",
                    CompanyId = DefaultCompanyId,
                    CreatedAtUtc = seededAt
                },
                new Category(CategoryPhonesId)
                {
                    Name = "Phones",
                    CompanyId = DefaultCompanyId,
                    CreatedAtUtc = seededAt
                },
                new Category(CategoryMonitorsId)
                {
                    Name = "Monitors",
                    CompanyId = DefaultCompanyId,
                    CreatedAtUtc = seededAt
                });
        }

        if (!await db.AssetModels.IgnoreQueryFilters().AnyAsync(a => a.Id == ModelMacBookId, cancellationToken))
        {
            db.AssetModels.AddRange(
                new AssetModel(ModelMacBookId)
                {
                    Name = "MacBook Pro 14\"",
                    CompanyId = DefaultCompanyId,
                    ManufacturerId = ManufacturerAppleId,
                    CategoryId = CategoryLaptopsId,
                    CreatedAtUtc = seededAt
                },
                new AssetModel(ModelLatitudeId)
                {
                    Name = "Dell Latitude 5540",
                    CompanyId = DefaultCompanyId,
                    ManufacturerId = ManufacturerDellId,
                    CategoryId = CategoryLaptopsId,
                    CreatedAtUtc = seededAt
                },
                new AssetModel(ModelIPhoneId)
                {
                    Name = "iPhone 15",
                    CompanyId = DefaultCompanyId,
                    ManufacturerId = ManufacturerAppleId,
                    CategoryId = CategoryPhonesId,
                    CreatedAtUtc = seededAt
                },
                new AssetModel(ModelUltraSharpId)
                {
                    Name = "Dell UltraSharp 27\"",
                    CompanyId = DefaultCompanyId,
                    ManufacturerId = ManufacturerDellId,
                    CategoryId = CategoryMonitorsId,
                    CreatedAtUtc = seededAt
                });
        }

        if (!await db.Locations.IgnoreQueryFilters().AnyAsync(l => l.Id == LocationHqId, cancellationToken))
        {
            db.Locations.AddRange(
                new Location(LocationHqId)
                {
                    Name = "Headquarters",
                    CompanyId = DefaultCompanyId,
                    CreatedAtUtc = seededAt
                },
                new Location(LocationBranchId)
                {
                    Name = "Branch Office",
                    CompanyId = DefaultCompanyId,
                    CreatedAtUtc = seededAt
                },
                new Location(LocationRemoteId)
                {
                    Name = "Remote",
                    CompanyId = DefaultCompanyId,
                    CreatedAtUtc = seededAt
                });
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}
