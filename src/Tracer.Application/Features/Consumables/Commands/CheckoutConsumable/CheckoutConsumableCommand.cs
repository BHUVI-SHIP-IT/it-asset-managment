using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Exceptions;
using Tracer.Application.Common.Interfaces;
using Tracer.Domain.Aggregates.InventoryAggregate;

namespace Tracer.Application.Features.Consumables.Commands.CheckoutConsumable;

public record CheckoutConsumableCommand(
    int ConsumableId,
    Guid AssignedToUserId,
    int Quantity) : IRequest<Unit>;

public sealed class CheckoutConsumableCommandValidator : AbstractValidator<CheckoutConsumableCommand>
{
    public CheckoutConsumableCommandValidator()
    {
        RuleFor(v => v.ConsumableId).GreaterThan(0);
        RuleFor(v => v.AssignedToUserId).NotEmpty();
        RuleFor(v => v.Quantity).GreaterThan(0);
    }
}

public sealed class CheckoutConsumableCommandHandler : IRequestHandler<CheckoutConsumableCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CheckoutConsumableCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(CheckoutConsumableCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUserService.CompanyId;
        
        var consumable = await _context.Consumables
            .FirstOrDefaultAsync(c => c.Id == request.ConsumableId && c.CompanyId == companyId, cancellationToken);

        if (consumable == null)
            throw new NotFoundException(nameof(Consumable), request.ConsumableId);

        // Persist assignee and deduct stock
        consumable.AssignTo(request.AssignedToUserId, request.Quantity);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
