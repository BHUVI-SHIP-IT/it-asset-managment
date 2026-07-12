using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracer.Domain.Aggregates.CustomFieldAggregate;

namespace Tracer.Persistence.Configurations;

public sealed class CustomFieldConfiguration : IEntityTypeConfiguration<CustomField>
{
    public void Configure(EntityTypeBuilder<CustomField> builder)
    {
        builder.ToTable("CustomFields");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.CompanyId).IsRequired();

        builder.Property(f => f.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(f => f.FieldType)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(f => f.Options)
            .HasMaxLength(4000);

        builder.Property(f => f.RowVersion).IsRowVersion();

        builder.HasIndex(f => new { f.CompanyId, f.Name })
            .IsUnique()
            .HasDatabaseName("UX_CustomFields_CompanyId_Name")
            .HasFilter("[IsDeleted] = 0");

        builder.HasOne<Domain.Entities.Company>()
            .WithMany()
            .HasForeignKey(f => f.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(f => !f.IsDeleted);
    }
}
