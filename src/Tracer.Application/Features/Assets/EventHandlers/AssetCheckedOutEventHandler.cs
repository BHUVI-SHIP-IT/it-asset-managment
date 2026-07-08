using MediatR;
using Microsoft.Extensions.Logging;
using Tracer.Application.Common.Interfaces;
using Tracer.Application.Common.Models;
using Tracer.Domain.Aggregates.NotificationAggregate;
using Tracer.Domain.Events;

namespace Tracer.Application.Features.Assets.EventHandlers;

/// <summary>
/// Reacts to the outbox-published <see cref="AssetCheckedOutEvent"/> by dispatching the checkout/EULA
/// notification through the multi-channel pipeline (M6; Doc 2 §2.1.9, Doc 3 §4.2).
/// </summary>
public class AssetCheckedOutEventHandler : INotificationHandler<DomainEventNotification<AssetCheckedOutEvent>>
{
    private readonly INotificationDispatcher _dispatcher;
    private readonly ILogger<AssetCheckedOutEventHandler> _logger;

    public AssetCheckedOutEventHandler(
        INotificationDispatcher dispatcher,
        ILogger<AssetCheckedOutEventHandler> logger)
    {
        _dispatcher = dispatcher;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<AssetCheckedOutEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        var message = new NotificationMessage(
            Title: "Asset Checked Out",
            Body: $"Asset {domainEvent.AssetId} was checked out to user {domainEvent.AssignedUserId} at {domainEvent.CheckedOutAtUtc:u}. Please review and accept the EULA.",
            Severity: NotificationSeverity.Info,
            Metadata: new Dictionary<string, string>
            {
                ["assetId"] = domainEvent.AssetId.ToString(),
                ["userId"] = domainEvent.AssignedUserId.ToString()
            });

        _logger.LogInformation(
            "Dispatching checkout notification for Asset {AssetId} to User {UserId}.",
            domainEvent.AssetId, domainEvent.AssignedUserId);

        await _dispatcher.DispatchAsync(message, cancellationToken);
    }
}
