namespace Tracer.Domain.Aggregates.NotificationAggregate;

/// <summary>Delivery lifecycle of a notification record (M6 notification center).</summary>
public enum NotificationStatus
{
    Pending = 0,
    Sent = 1,
    Failed = 2
}
