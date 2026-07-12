using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracer.Domain.Aggregates.InventoryAggregate;

namespace Tracer.Persistence.Configurations;

public sealed class ConsumableConfiguration : IEntityTypeConfiguration<Consumable>
{
    public void Configure(EntityTypeBuilder<Consumable> builder)
    {
        builder.ToTable("Consumables", b => b.IsTemporal());
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).HasMaxLength(255).IsRequired();
        builder.Property(c => c.PurchaseCost).HasColumnType("decimal(18,2)");
        builder.Property(c => c.RowVersion).IsRowVersion();

        builder.HasQueryFilter(c => !c.IsDeleted);

        builder.HasIndex(c => new { c.CompanyId, c.Name })
            .IsUnique()
            .HasDatabaseName("UX_Consumables_CompanyId_Name")
            .HasFilter("[IsDeleted] = 0");

        builder.HasOne<Domain.Entities.Company>()
            .WithMany()
            .HasForeignKey(c => c.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
