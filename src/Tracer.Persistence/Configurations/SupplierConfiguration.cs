using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracer.Domain.Entities;

namespace Tracer.Persistence.Configurations;

public sealed class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("Suppliers");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(s => s.Name).HasMaxLength(255).IsRequired();
        builder.Property(s => s.RowVersion).IsRowVersion();

        builder.HasQueryFilter(s => !s.IsDeleted);

        builder.HasIndex(s => s.Name).IsUnique().HasFilter("[IsDeleted] = 0");

        builder.HasOne(s => s.Company)
            .WithMany()
            .HasForeignKey(s => s.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
