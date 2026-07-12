using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracer.Domain.Entities;

namespace Tracer.Persistence.Configurations;

public sealed class AssetModelConfiguration : IEntityTypeConfiguration<AssetModel>
{
    public void Configure(EntityTypeBuilder<AssetModel> builder)
    {
        builder.ToTable("AssetModels");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(a => a.Name).HasMaxLength(255).IsRequired();
        builder.Property(a => a.RowVersion).IsRowVersion();

        builder.HasQueryFilter(a => !a.IsDeleted);

        builder.HasIndex(a => a.Name).IsUnique().HasFilter("[IsDeleted] = 0");

        builder.HasOne(a => a.Company)
            .WithMany()
            .HasForeignKey(a => a.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Manufacturer)
            .WithMany()
            .HasForeignKey(a => a.ManufacturerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Category)
            .WithMany()
            .HasForeignKey(a => a.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
