using Tracer.Application.Common.Models;

namespace Tracer.Application.Common.Interfaces;

/// <summary>
/// A pluggable delivery channel (Email, Slack, Teams, ...) for notifications (M6, Doc 3 §4.2).
/// Implementations live in the Infrastructure layer and are fanned out by <see cref="INotificationDispatcher"/>.
/// </summary>
public interface INotificationChannel
{
    /// <summary>Stable identifier used to enable/disable this channel per tenant (e.g. "Slack", "Teams", "Email").</summary>
    string ChannelType { get; }

    /// <summary>True when the channel has the configuration it needs to deliver (webhook URL, SMTP host, ...).</summary>
    bool IsConfigured { get; }

    /// <summary>Deliver a single message. Implementations should throw on transport failure so the dispatcher can record it.</summary>
    Task SendAsync(NotificationMessage message, CancellationToken cancellationToken);
}
