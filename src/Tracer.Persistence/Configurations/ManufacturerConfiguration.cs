using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracer.Domain.Entities;

namespace Tracer.Persistence.Configurations;

public sealed class ManufacturerConfiguration : IEntityTypeConfiguration<Manufacturer>
{
    public void Configure(EntityTypeBuilder<Manufacturer> builder)
    {
        builder.ToTable("Manufacturers");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(m => m.Name).HasMaxLength(255).IsRequired();
        builder.Property(m => m.RowVersion).IsRowVersion();

        builder.HasQueryFilter(m => !m.IsDeleted);

        builder.HasIndex(m => m.Name).IsUnique().HasFilter("[IsDeleted] = 0");

        builder.HasOne(m => m.Company)
            .WithMany()
            .HasForeignKey(m => m.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
