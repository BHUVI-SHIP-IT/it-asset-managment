using Microsoft.EntityFrameworkCore;
using Tracer.Domain.Entities;

namespace Tracer.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Category> Categories { get; }
    DbSet<Location> Locations { get; }
    DbSet<Manufacturer> Manufacturers { get; }
    DbSet<Supplier> Suppliers { get; }
    DbSet<Department> Departments { get; }
    DbSet<AssetModel> AssetModels { get; }
    DbSet<StatusLabel> StatusLabels { get; }
    DbSet<Company> Companies { get; }
    DbSet<ApplicationUser> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<Tracer.Domain.Aggregates.AssetAggregate.Asset> Assets { get; }

    DbSet<Tracer.Domain.Aggregates.LicenseAggregate.SoftwareLicense> SoftwareLicenses { get; }
    DbSet<Tracer.Domain.Aggregates.LicenseAggregate.LicenseSeat> LicenseSeats { get; }
    DbSet<Tracer.Domain.Aggregates.InventoryAggregate.Accessory> Accessories { get; }
    DbSet<Tracer.Domain.Aggregates.InventoryAggregate.Component> Components { get; }
    DbSet<Tracer.Domain.Aggregates.InventoryAggregate.Consumable> Consumables { get; }

    DbSet<Tracer.Domain.Aggregates.DepreciationAggregate.Depreciation> Depreciations { get; }
    DbSet<ReportExport> ReportExports { get; }

    // ── Notifications & Tenant Config (M6) ──
    DbSet<Tracer.Domain.Aggregates.NotificationAggregate.Notification> Notifications { get; }
    DbSet<Tracer.Domain.Aggregates.SettingAggregate.TenantSetting> TenantSettings { get; }
    DbSet<Tracer.Domain.Aggregates.CustomFieldAggregate.CustomField> CustomFields { get; }
    DbSet<Tracer.Domain.Aggregates.CustomFieldAggregate.CustomFieldValue> CustomFieldValues { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
