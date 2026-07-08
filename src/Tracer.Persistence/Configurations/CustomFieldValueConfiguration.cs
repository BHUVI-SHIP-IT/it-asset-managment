using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracer.Domain.Aggregates.CustomFieldAggregate;

namespace Tracer.Persistence.Configurations;

public sealed class CustomFieldValueConfiguration : IEntityTypeConfiguration<CustomFieldValue>
{
    public void Configure(EntityTypeBuilder<CustomFieldValue> builder)
    {
        builder.ToTable("CustomFieldValues");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.CustomFieldId).IsRequired();
        builder.Property(v => v.EntityId).IsRequired();

        builder.Property(v => v.Value)
            .HasMaxLength(4000);

        builder.Property(v => v.RowVersion).IsRowVersion();

        builder.HasOne(v => v.CustomField)
            .WithMany()
            .HasForeignKey(v => v.CustomFieldId)
            .OnDelete(DeleteBehavior.Cascade);

        // One value per field per owning entity instance.
        builder.HasIndex(v => new { v.CustomFieldId, v.EntityId })
            .IsUnique()
            .HasDatabaseName("UX_CustomFieldValues_FieldId_EntityId");

        builder.HasQueryFilter(v => !v.IsDeleted);
    }
}
