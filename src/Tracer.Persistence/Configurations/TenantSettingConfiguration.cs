using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracer.Domain.Aggregates.SettingAggregate;

namespace Tracer.Persistence.Configurations;

public sealed class TenantSettingConfiguration : IEntityTypeConfiguration<TenantSetting>
{
    public void Configure(EntityTypeBuilder<TenantSetting> builder)
    {
        builder.ToTable("TenantSettings");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.CompanyId).IsRequired();

        builder.Property(s => s.Key)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(s => s.Value)
            .HasMaxLength(4000);

        builder.Property(s => s.RowVersion).IsRowVersion();

        // One value per key per tenant.
        builder.HasIndex(s => new { s.CompanyId, s.Key })
            .IsUnique()
            .HasDatabaseName("UX_TenantSettings_CompanyId_Key");

        builder.HasQueryFilter(s => !s.IsDeleted);
    }
}
