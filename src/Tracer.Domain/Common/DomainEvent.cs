namespace Tracer.Domain.Common;

/// <summary>
/// Base type for all domain events (Doc 10 §3.1). Raised by aggregates, extracted by the
/// EF Core OutboxInterceptor, and published reliably via the Outbox pattern (Doc 10 §4.2).
/// </summary>
public abstract record DomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
