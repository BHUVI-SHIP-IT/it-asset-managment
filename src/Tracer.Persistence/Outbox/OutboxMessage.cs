namespace Tracer.Persistence.Outbox;

/// <summary>
/// Outbox message entity (Doc 10 §4.2). Domain events are serialized into this table
/// within the same DB transaction as the aggregate change, then published asynchronously
/// by a Hangfire background job (built in M3).
/// </summary>
public sealed class OutboxMessage
{
    public Guid Id { get; set; }

    /// <summary>Assembly-qualified type name of the domain event.</summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>JSON-serialized domain event payload.</summary>
    public string Content { get; set; } = string.Empty;

    public DateTime OccurredOnUtc { get; set; }

    public DateTime? ProcessedOnUtc { get; set; }

    public string? Error { get; set; }
}
