using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracer.Domain.Entities;

namespace Tracer.Persistence.Configurations;

public sealed class ReportExportConfiguration : IEntityTypeConfiguration<ReportExport>
{
    public void Configure(EntityTypeBuilder<ReportExport> builder)
    {
        builder.ToTable("ReportExports");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.ReportName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(r => r.CompanyId)
            .IsRequired();

        builder.Property(r => r.RequestedBy)
            .IsRequired();

        builder.Property(r => r.Status)
            .IsRequired();

        builder.Property(r => r.CreatedAtUtc)
            .IsRequired();

        builder.Property(r => r.FileContent)
            .HasColumnType("varbinary(max)");
    }
}
