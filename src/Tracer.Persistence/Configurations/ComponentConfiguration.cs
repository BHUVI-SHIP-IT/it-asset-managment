using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracer.Domain.Aggregates.InventoryAggregate;

namespace Tracer.Persistence.Configurations;

public sealed class ComponentConfiguration : IEntityTypeConfiguration<Component>
{
    public void Configure(EntityTypeBuilder<Component> builder)
    {
        builder.ToTable("Components", b => b.IsTemporal());
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).HasMaxLength(255).IsRequired();
        builder.Property(c => c.PurchaseCost).HasColumnType("decimal(18,2)");
        builder.Property(c => c.RowVersion).IsRowVersion();

        builder.HasIndex(c => c.AssignedUserId)
            .HasDatabaseName("IX_Components_AssignedUserId");

        builder.HasOne<Domain.Entities.ApplicationUser>()
            .WithMany()
            .HasForeignKey(c => c.AssignedUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne<Domain.Entities.AssetModel>()
            .WithMany()
            .HasForeignKey(c => c.CompatibleAssetModelId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(c => !c.IsDeleted);

        builder.HasIndex(c => new { c.CompanyId, c.Name })
            .IsUnique()
            .HasDatabaseName("UX_Components_CompanyId_Name")
            .HasFilter("[IsDeleted] = 0");

        builder.HasOne<Domain.Entities.Company>()
            .WithMany()
            .HasForeignKey(c => c.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
