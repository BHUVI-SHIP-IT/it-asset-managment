using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracer.Domain.Entities;

namespace Tracer.Persistence.Configurations;

public sealed class StatusLabelConfiguration : IEntityTypeConfiguration<StatusLabel>
{
    public void Configure(EntityTypeBuilder<StatusLabel> builder)
    {
        builder.ToTable("StatusLabels");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedOnAdd(); // Because it's INT, let it be identity or explicit if seeded

        builder.Property(s => s.Name).HasMaxLength(255).IsRequired();
        builder.Property(s => s.RowVersion).IsRowVersion();

        builder.HasQueryFilter(s => !s.IsDeleted);
        builder.HasIndex(s => s.Name).IsUnique().HasFilter("[IsDeleted] = 0");

        // Seed default statuses as defined in Doc 4 / Doc 7
        builder.HasData(
            new StatusLabel(1) { Name = "Deployable", IsDeployable = true, IsPending = false, IsArchived = false },
            new StatusLabel(2) { Name = "Deployed", IsDeployable = false, IsPending = false, IsArchived = false },
            new StatusLabel(3) { Name = "Archived", IsDeployable = false, IsPending = false, IsArchived = true },
            new StatusLabel(4) { Name = "Broken", IsDeployable = false, IsPending = false, IsArchived = false },
            new StatusLabel(5) { Name = "Pending", IsDeployable = false, IsPending = true, IsArchived = false }
        );
    }
}
