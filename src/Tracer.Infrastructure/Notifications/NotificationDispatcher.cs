using Microsoft.Extensions.Logging;
using Tracer.Application.Common.Interfaces;
using Tracer.Application.Common.Models;
using Tracer.Domain.Aggregates.NotificationAggregate;

namespace Tracer.Infrastructure.Notifications;

/// <summary>
/// Fans a message out to every configured notification channel and records a Notification row per
/// attempt for the notification center (M6, Doc 5 §3.21). One channel failing never aborts the others.
/// </summary>
public sealed class NotificationDispatcher : INotificationDispatcher
{
    private readonly IEnumerable<INotificationChannel> _channels;
    private readonly IApplicationDbContext _dbContext;
    private readonly ILogger<NotificationDispatcher> _logger;

    public NotificationDispatcher(
        IEnumerable<INotificationChannel> channels,
        IApplicationDbContext dbContext,
        ILogger<NotificationDispatcher> logger)
    {
        _channels = channels;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task DispatchAsync(NotificationMessage message, CancellationToken cancellationToken)
    {
        var configured = _channels.Where(c => c.IsConfigured).ToList();

        if (configured.Count == 0)
        {
            _logger.LogWarning(
                "No notification channels configured; message '{Title}' was not delivered.", message.Title);
            return;
        }

        foreach (var channel in configured)
        {
            var record = Notification.Create(
                message.Title, message.Body, message.Severity, channel.ChannelType, message.CompanyId, message.Recipient);

            try
            {
                await channel.SendAsync(message, cancellationToken);
                record.MarkSent(DateTime.UtcNow);
                _logger.LogInformation(
                    "Notification '{Title}' delivered via {Channel}.", message.Title, channel.ChannelType);
            }
            catch (Exception ex)
            {
                record.MarkFailed(ex.Message);
                _logger.LogError(ex,
                    "Notification '{Title}' failed on channel {Channel}.", message.Title, channel.ChannelType);
            }

            _dbContext.Notifications.Add(record);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
