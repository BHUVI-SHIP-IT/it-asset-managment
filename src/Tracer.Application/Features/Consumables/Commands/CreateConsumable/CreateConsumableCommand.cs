using FluentValidation;
using MediatR;
using Tracer.Application.Common.Interfaces;
using Tracer.Domain.Aggregates.InventoryAggregate;

namespace Tracer.Application.Features.Consumables.Commands.CreateConsumable;

public record CreateConsumableCommand(
    string Name,
    int TotalQuantity,
    decimal PurchaseCost) : IRequest<int>;

public sealed class CreateConsumableCommandValidator : AbstractValidator<CreateConsumableCommand>
{
    public CreateConsumableCommandValidator()
    {
        RuleFor(v => v.Name).NotEmpty().MaximumLength(255);
        RuleFor(v => v.TotalQuantity).GreaterThanOrEqualTo(0);
        RuleFor(v => v.PurchaseCost).GreaterThanOrEqualTo(0);
    }
}

public sealed class CreateConsumableCommandHandler : IRequestHandler<CreateConsumableCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateConsumableCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<int> Handle(CreateConsumableCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUserService.CompanyId;
        if (!companyId.HasValue || companyId.Value == Guid.Empty)
        {
            throw new UnauthorizedAccessException("User is not associated with a company.");
        }

        var consumable = Consumable.Create(
            request.Name,
            companyId.Value,
            request.TotalQuantity,
            request.PurchaseCost);

        _context.Consumables.Add(consumable);
        await _context.SaveChangesAsync(cancellationToken);

        return consumable.Id;
    }
}
