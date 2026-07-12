using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracer.Domain.Entities;

namespace Tracer.Persistence.Configurations;

public sealed class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("Locations");

        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(l => l.Name).HasMaxLength(255).IsRequired();
        builder.Property(l => l.RowVersion).IsRowVersion();

        builder.HasQueryFilter(l => !l.IsDeleted);

        builder.HasIndex(l => l.Name).IsUnique().HasFilter("[IsDeleted] = 0");

        builder.HasOne(l => l.Company)
            .WithMany()
            .HasForeignKey(l => l.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
