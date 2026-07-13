using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Exceptions;
using Tracer.Application.Common.Interfaces;
using Tracer.Domain.Aggregates.InventoryAggregate;

namespace Tracer.Application.Features.Consumables.Commands.UpdateConsumable;

public record UpdateConsumableCommand(
    int Id,
    string Name,
    int TotalQuantity,
    decimal PurchaseCost) : IRequest;

public sealed class UpdateConsumableCommandValidator : AbstractValidator<UpdateConsumableCommand>
{
    public UpdateConsumableCommandValidator()
    {
        RuleFor(v => v.Id).GreaterThan(0);
        RuleFor(v => v.Name).NotEmpty().MaximumLength(255);
        RuleFor(v => v.TotalQuantity).GreaterThanOrEqualTo(0);
        RuleFor(v => v.PurchaseCost).GreaterThanOrEqualTo(0);
    }
}

public sealed class UpdateConsumableCommandHandler : IRequestHandler<UpdateConsumableCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateConsumableCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task Handle(UpdateConsumableCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUserService.CompanyId
            ?? throw new UnauthorizedAccessException("User is not associated with a company.");

        var consumable = await _context.Consumables
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.CompanyId == companyId, cancellationToken)
            ?? throw new NotFoundException(nameof(Consumable), request.Id);

        consumable.Update(request.Name, request.TotalQuantity, request.PurchaseCost);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
