using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracer.Domain.Aggregates.DepreciationAggregate;

namespace Tracer.Persistence.Configurations;

public sealed class DepreciationConfiguration : IEntityTypeConfiguration<Depreciation>
{
    public void Configure(EntityTypeBuilder<Depreciation> builder)
    {
        builder.ToTable("Depreciations");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(d => d.Months)
            .IsRequired();

        builder.Property(d => d.CompanyId)
            .IsRequired();

        builder.Property(d => d.MinimumValue)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        // Enforce uniqueness of Depreciation Name per Tenant (CompanyId)
        builder.HasIndex(d => new { d.CompanyId, d.Name }).IsUnique();
        
        // Soft-delete query filter
        builder.HasQueryFilter(d => !d.IsDeleted);
    }
}
