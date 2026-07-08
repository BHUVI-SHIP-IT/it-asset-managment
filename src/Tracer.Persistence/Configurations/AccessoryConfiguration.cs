using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracer.Domain.Aggregates.InventoryAggregate;

namespace Tracer.Persistence.Configurations;

public sealed class AccessoryConfiguration : IEntityTypeConfiguration<Accessory>
{
    public void Configure(EntityTypeBuilder<Accessory> builder)
    {
        builder.ToTable("Accessories", b => b.IsTemporal());
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Name).HasMaxLength(255).IsRequired();
        builder.Property(a => a.PurchaseCost).HasColumnType("decimal(18,2)");
        builder.Property(a => a.RowVersion).IsRowVersion();

        builder.HasQueryFilter(a => !a.IsDeleted);

        builder.HasIndex(a => new { a.CompanyId, a.Name })
            .IsUnique()
            .HasDatabaseName("UX_Accessories_CompanyId_Name")
            .HasFilter("[IsDeleted] = 0");
    }
}
