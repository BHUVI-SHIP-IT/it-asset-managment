using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracer.Persistence.Outbox;

namespace Tracer.Persistence.Configurations;

/// <summary>
/// Fluent API configuration for the OutboxMessages table (Doc 10 §4.2).
/// </summary>
public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Type)
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(o => o.Content)
            .IsRequired();

        builder.Property(o => o.Error)
            .HasMaxLength(4000);

        // Index for the Hangfire poller: fetch unprocessed messages ordered by occurrence.
        builder.HasIndex(o => o.ProcessedOnUtc)
            .HasDatabaseName("IX_OutboxMessages_ProcessedOnUtc")
            .HasFilter("[ProcessedOnUtc] IS NULL");
    }
}
