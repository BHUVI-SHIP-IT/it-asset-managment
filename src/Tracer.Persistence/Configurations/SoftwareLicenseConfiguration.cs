using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracer.Domain.Aggregates.LicenseAggregate;

namespace Tracer.Persistence.Configurations;

public sealed class SoftwareLicenseConfiguration : IEntityTypeConfiguration<SoftwareLicense>
{
    public void Configure(EntityTypeBuilder<SoftwareLicense> builder)
    {
        builder.ToTable("SoftwareLicenses", b => b.IsTemporal());
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id).HasDefaultValueSql("NEWSEQUENTIALID()");
        builder.Property(l => l.Name).HasMaxLength(255).IsRequired();
        builder.Property(l => l.PurchaseCost).HasColumnType("decimal(18,2)");
        builder.Property(l => l.RowVersion).IsRowVersion();

        builder.HasQueryFilter(l => !l.IsDeleted);

        builder.HasIndex(l => new { l.CompanyId, l.Name })
            .IsUnique()
            .HasDatabaseName("UX_SoftwareLicenses_CompanyId_Name")
            .HasFilter("[IsDeleted] = 0");
    }
}
