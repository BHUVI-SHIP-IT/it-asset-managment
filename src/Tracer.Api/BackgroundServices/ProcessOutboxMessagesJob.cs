using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Tracer.Domain.Common;
using Tracer.Persistence.Contexts;

namespace Tracer.Api.BackgroundServices;

public class ProcessOutboxMessagesJob
{
    private readonly TracerDbContext _context;
    private readonly IPublisher _publisher;
    private readonly ILogger<ProcessOutboxMessagesJob> _logger;

    public ProcessOutboxMessagesJob(
        TracerDbContext context, 
        IPublisher publisher,
        ILogger<ProcessOutboxMessagesJob> logger)
    {
        _context = context;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var messages = await _context.OutboxMessages
            .Where(m => m.ProcessedOnUtc == null)
            .OrderBy(m => m.OccurredOnUtc)
            .Take(20)
            .ToListAsync(cancellationToken);

        foreach (var message in messages)
        {
            try
            {
                var type = Type.GetType(message.Type);
                if (type == null)
                {
                    throw new InvalidOperationException($"Type {message.Type} not found");
                }
                
                var domainEvent = System.Text.Json.JsonSerializer.Deserialize(message.Content, type) as DomainEvent;

                if (domainEvent != null)
                {
                    var genericType = typeof(Tracer.Application.Common.Models.DomainEventNotification<>).MakeGenericType(type);
                    var notification = Activator.CreateInstance(genericType, domainEvent);
                    
                    if (notification != null)
                    {
                        await _publisher.Publish(notification, cancellationToken);
                    }
                }

                message.ProcessedOnUtc = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing outbox message {MessageId}", message.Id);
                message.Error = ex.Message;
            }
        }

        if (messages.Any())
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
