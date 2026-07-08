namespace Tracer.Domain.Aggregates.NotificationAggregate;

/// <summary>Severity of a notification; channels use it to style/route messages (M6).</summary>
public enum NotificationSeverity
{
    Info = 0,
    Warning = 1,
    Critical = 2
}
