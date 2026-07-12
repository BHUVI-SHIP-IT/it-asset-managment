using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracer.Domain.Aggregates.RequestAggregate;

namespace Tracer.Persistence.Configurations;

public sealed class InventoryRequestConfiguration : IEntityTypeConfiguration<InventoryRequest>
{
    public void Configure(EntityTypeBuilder<InventoryRequest> builder)
    {
        builder.ToTable("InventoryRequests");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Type).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(r => r.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(r => r.ItemId).HasMaxLength(64);
        builder.Property(r => r.Notes).HasMaxLength(2000);
        builder.Property(r => r.ResolutionNotes).HasMaxLength(2000);
        builder.Property(r => r.RowVersion).IsRowVersion();

        builder.HasIndex(r => new { r.CompanyId, r.Status })
            .HasDatabaseName("IX_InventoryRequests_CompanyId_Status");
        builder.HasIndex(r => r.RequestedByUserId)
            .HasDatabaseName("IX_InventoryRequests_RequestedByUserId");

        builder.HasOne<Domain.Entities.Company>()
            .WithMany()
            .HasForeignKey(r => r.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Domain.Entities.ApplicationUser>()
            .WithMany()
            .HasForeignKey(r => r.RequestedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Domain.Entities.ApplicationUser>()
            .WithMany()
            .HasForeignKey(r => r.ResolvedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(r => !r.IsDeleted);
    }
}
