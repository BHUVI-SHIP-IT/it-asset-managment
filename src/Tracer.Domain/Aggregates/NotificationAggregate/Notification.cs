using Tracer.Domain.Common;

namespace Tracer.Domain.Aggregates.NotificationAggregate;

/// <summary>
/// Notification aggregate root (Doc 4 §3.23, Doc 5 §3.21).
/// One record per delivery attempt against a single channel; backs the notification center.
/// </summary>
public sealed class Notification : AuditableEntity<Guid>
{
    private Notification() { }

    private Notification(
        Guid id,
        string title,
        string body,
        NotificationSeverity severity,
        string channel,
        Guid? companyId,
        string? recipient)
        : base(id)
    {
        Name = title;
        Body = body;
        Severity = severity;
        Channel = channel;
        CompanyId = companyId;
        Recipient = recipient;
        Status = NotificationStatus.Pending;
    }

    /// <summary>Display title. Named <c>Name</c> to match the shared schema convention (Doc 4 §3.23).</summary>
    public string Name { get; private set; } = string.Empty;
    public string Body { get; private set; } = string.Empty;
    public NotificationSeverity Severity { get; private set; }
    public string Channel { get; private set; } = string.Empty;
    public Guid? CompanyId { get; private set; }
    public string? Recipient { get; private set; }
    public NotificationStatus Status { get; private set; }
    public DateTime? SentAtUtc { get; private set; }
    public string? FailureReason { get; private set; }
    public bool IsRead { get; private set; }

    public static Notification Create(
        string title,
        string body,
        NotificationSeverity severity,
        string channel,
        Guid? companyId = null,
        string? recipient = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));
        if (string.IsNullOrWhiteSpace(channel))
            throw new ArgumentException("Channel is required.", nameof(channel));

        return new Notification(Guid.NewGuid(), title.Trim(), body ?? string.Empty, severity, channel.Trim(), companyId, recipient);
    }

    public void MarkSent(DateTime sentAtUtc)
    {
        Status = NotificationStatus.Sent;
        SentAtUtc = sentAtUtc;
        FailureReason = null;
    }

    public void MarkFailed(string reason)
    {
        Status = NotificationStatus.Failed;
        FailureReason = string.IsNullOrWhiteSpace(reason) ? "Unknown error" : reason;
    }

    public void MarkRead() => IsRead = true;
}
