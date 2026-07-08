using Tracer.Application.Common.Models;

namespace Tracer.Application.Common.Interfaces;

/// <summary>
/// Fans a <see cref="NotificationMessage"/> out to every configured, tenant-enabled channel and
/// persists a Notification record per attempt for the notification center (M6, Doc 5 §3.21).
/// A failure on one channel never aborts the others.
/// </summary>
public interface INotificationDispatcher
{
    Task DispatchAsync(NotificationMessage message, CancellationToken cancellationToken);
}
