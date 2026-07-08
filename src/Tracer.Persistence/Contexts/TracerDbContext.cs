using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Tracer.Domain.Aggregates.AssetAggregate;
using Tracer.Domain.Entities;
using Tracer.Persistence.Outbox;

namespace Tracer.Persistence.Contexts;

/// <summary>
/// Central EF Core DbContext for Tracer (Doc 10 §3.3). Applies all Fluent API
/// configurations from the assembly and enforces soft-delete via global query filters.
/// </summary>
public sealed class TracerDbContext : DbContext, Tracer.Application.Common.Interfaces.IApplicationDbContext
{
    public TracerDbContext(DbContextOptions<TracerDbContext> options) : base(options) { }

    // ── Core Aggregates ──
    public DbSet<Asset> Assets => Set<Asset>();

    // ── IAM / Auth (M1) ──
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<ApplicationUser> Users => Set<ApplicationUser>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    // ── Master Data (M2) ──
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Manufacturer> Manufacturers => Set<Manufacturer>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<AssetModel> AssetModels => Set<AssetModel>();
    public DbSet<StatusLabel> StatusLabels => Set<StatusLabel>();

    // ── Inventory (M4) ──
    public DbSet<Tracer.Domain.Aggregates.LicenseAggregate.SoftwareLicense> SoftwareLicenses => Set<Tracer.Domain.Aggregates.LicenseAggregate.SoftwareLicense>();
    public DbSet<Tracer.Domain.Aggregates.LicenseAggregate.LicenseSeat> LicenseSeats => Set<Tracer.Domain.Aggregates.LicenseAggregate.LicenseSeat>();
    public DbSet<Tracer.Domain.Aggregates.InventoryAggregate.Accessory> Accessories => Set<Tracer.Domain.Aggregates.InventoryAggregate.Accessory>();
    public DbSet<Tracer.Domain.Aggregates.InventoryAggregate.Component> Components => Set<Tracer.Domain.Aggregates.InventoryAggregate.Component>();
    public DbSet<Tracer.Domain.Aggregates.InventoryAggregate.Consumable> Consumables => Set<Tracer.Domain.Aggregates.InventoryAggregate.Consumable>();

    // ── Financial (M5) ──
    public DbSet<Tracer.Domain.Aggregates.DepreciationAggregate.Depreciation> Depreciations => Set<Tracer.Domain.Aggregates.DepreciationAggregate.Depreciation>();
    public DbSet<ReportExport> ReportExports => Set<ReportExport>();

    // ── Notifications & Tenant Config (M6) ──
    public DbSet<Tracer.Domain.Aggregates.NotificationAggregate.Notification> Notifications => Set<Tracer.Domain.Aggregates.NotificationAggregate.Notification>();
    public DbSet<Tracer.Domain.Aggregates.SettingAggregate.TenantSetting> TenantSettings => Set<Tracer.Domain.Aggregates.SettingAggregate.TenantSetting>();
    public DbSet<Tracer.Domain.Aggregates.CustomFieldAggregate.CustomField> CustomFields => Set<Tracer.Domain.Aggregates.CustomFieldAggregate.CustomField>();
    public DbSet<Tracer.Domain.Aggregates.CustomFieldAggregate.CustomFieldValue> CustomFieldValues => Set<Tracer.Domain.Aggregates.CustomFieldAggregate.CustomFieldValue>();

    // ── Outbox (Doc 10 §4.2) ──
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply all IEntityTypeConfiguration<T> from this assembly.
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.Ignore<Tracer.Domain.Common.DomainEvent>();

        // Apply seed data
        Seed.RolePermissionSeedData.SeedRolesAndPermissions(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }
}
