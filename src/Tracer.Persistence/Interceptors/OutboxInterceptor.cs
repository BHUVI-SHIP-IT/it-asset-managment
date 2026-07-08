using System.Text.Json;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Tracer.Domain.Common;
using Tracer.Persistence.Outbox;

namespace Tracer.Persistence.Interceptors;

/// <summary>
/// EF Core interceptor implementing the Outbox Pattern (Doc 10 §4.2).
/// Before SaveChanges commits, it extracts all domain events from tracked aggregate roots
/// and serializes them into the OutboxMessages table within the same DB transaction.
/// A background worker (Hangfire, built in M3) later polls and publishes these messages.
/// </summary>
public sealed class OutboxInterceptor : SaveChangesInterceptor
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null)
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        var context = eventData.Context;

        // Collect domain events from all tracked entities that implement Entity<T>.
        var outboxMessages = context.ChangeTracker
            .Entries<Entity<Guid>>()
            .SelectMany(entry =>
            {
                var events = entry.Entity.DomainEvents.ToList();
                entry.Entity.ClearDomainEvents();
                return events;
            })
            .Select(domainEvent => new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = domainEvent.GetType().AssemblyQualifiedName!,
                Content = JsonSerializer.Serialize(domainEvent, domainEvent.GetType(), JsonOptions),
                OccurredOnUtc = domainEvent.OccurredOnUtc,
                ProcessedOnUtc = null,
                Error = null
            })
            .ToList();

        if (outboxMessages.Count > 0)
        {
            context.Set<OutboxMessage>().AddRange(outboxMessages);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
