using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracer.Domain.Aggregates.NotificationAggregate;

namespace Tracer.Persistence.Configurations;

public sealed class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(n => n.Body)
            .IsRequired();

        builder.Property(n => n.Channel)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(n => n.Recipient)
            .HasMaxLength(320);

        builder.Property(n => n.Severity)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(n => n.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(n => n.FailureReason)
            .HasMaxLength(1024);

        // Optimistic concurrency token (Doc 4 §1.2).
        builder.Property(n => n.RowVersion).IsRowVersion();

        // Notification-center query: newest-first per tenant, filterable by delivery status.
        builder.HasIndex(n => new { n.CompanyId, n.Status });

        builder.HasQueryFilter(n => !n.IsDeleted);
    }
}
