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

        builder.Property(d => d.Method)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Ignore(d => d.UsefulLifeYears);

        builder.Property(d => d.RowVersion).IsRowVersion();

        // Enforce uniqueness of Depreciation Name per Tenant (CompanyId), ignoring soft-deleted rows
        builder.HasIndex(d => new { d.CompanyId, d.Name })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.HasOne<Domain.Entities.Company>()
            .WithMany()
            .HasForeignKey(d => d.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        // Soft-delete query filter
        builder.HasQueryFilter(d => !d.IsDeleted);
    }
}
