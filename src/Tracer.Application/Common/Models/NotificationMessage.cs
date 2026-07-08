using Tracer.Domain.Aggregates.NotificationAggregate;

namespace Tracer.Application.Common.Models;

/// <summary>
/// Channel-agnostic payload dispatched to one or more <c>INotificationChannel</c> implementations
/// (M6 Notifications). Produced by domain-event handlers reacting to outbox messages.
/// </summary>
public record NotificationMessage(
    string Title,
    string Body,
    NotificationSeverity Severity,
    Guid? CompanyId = null,
    string? Recipient = null,
    IReadOnlyDictionary<string, string>? Metadata = null);
