using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracer.Domain.Aggregates.AssetAggregate;

namespace Tracer.Persistence.Configurations;

/// <summary>
/// Fluent API configuration for the Asset aggregate root.
/// Maps to the "Assets" table with soft-delete filter, concurrency token, and indexes (Doc 4 §3).
/// </summary>
public sealed class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.ToTable("Assets", b => b.IsTemporal());
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(a => a.AssetTag)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(a => a.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(a => a.SerialNumber)
            .HasMaxLength(255);

        builder.Property(a => a.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(a => a.PurchaseCost)
            .HasColumnType("decimal(18,2)");
            
        builder.Property(a => a.CurrentValue)
            .HasColumnType("decimal(18,2)");

        builder.Property(a => a.Notes)
            .HasMaxLength(4000);

        // Optimistic concurrency via SQL Server ROWVERSION (Doc 4 §1.2).
        builder.Property(a => a.RowVersion)
            .IsRowVersion();

        // Soft-delete global query filter (Doc 4 §1.2).
        builder.HasQueryFilter(a => !a.IsDeleted);

        // ── Indexes ──

        // Unique asset tag per company (Doc 4 §1.1 naming + Doc 2 uniqueness).
        builder.HasIndex(a => new { a.CompanyId, a.AssetTag })
            .IsUnique()
            .HasDatabaseName("UX_Assets_CompanyId_AssetTag")
            .HasFilter("[IsDeleted] = 0");

        // Covering index for dashboard queries (Doc 4 §1.7).
        builder.HasIndex(a => new { a.CompanyId, a.StatusLabelId })
            .HasDatabaseName("IX_Assets_CompanyId_StatusLabelId");

        builder.HasIndex(a => a.AssignedUserId)
            .HasDatabaseName("IX_Assets_AssignedUserId");

        // ── Relationships ──

        builder.HasOne<Domain.Entities.Company>()
            .WithMany()
            .HasForeignKey(a => a.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Domain.Entities.AssetModel>()
            .WithMany()
            .HasForeignKey(a => a.AssetModelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Domain.Entities.StatusLabel>()
            .WithMany()
            .HasForeignKey(a => a.StatusLabelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Domain.Entities.Location>()
            .WithMany()
            .HasForeignKey(a => a.LocationId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne<Domain.Entities.ApplicationUser>()
            .WithMany()
            .HasForeignKey(a => a.AssignedUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne<Tracer.Domain.Aggregates.DepreciationAggregate.Depreciation>()
            .WithMany()
            .HasForeignKey(a => a.DepreciationId)
            .OnDelete(DeleteBehavior.SetNull);

        // Ignore the in-memory domain events collection.
        builder.Ignore(a => a.DomainEvents);
    }
}
