using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracer.Domain.Aggregates.LicenseAggregate;

namespace Tracer.Persistence.Configurations;

public sealed class LicenseSeatConfiguration : IEntityTypeConfiguration<LicenseSeat>
{
    public void Configure(EntityTypeBuilder<LicenseSeat> builder)
    {
        builder.ToTable("LicenseSeats", b => b.IsTemporal());
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id).HasDefaultValueSql("NEWSEQUENTIALID()");
        builder.Property(s => s.RowVersion).IsRowVersion();

        builder.HasQueryFilter(s => !s.IsDeleted);

        builder.HasOne<SoftwareLicense>()
            .WithMany()
            .HasForeignKey(s => s.SoftwareLicenseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
