using MediatR;
using Tracer.Domain.Common;

namespace Tracer.Application.Common.Models;

public record DomainEventNotification<TDomainEvent>(TDomainEvent DomainEvent) : INotification where TDomainEvent : DomainEvent;
