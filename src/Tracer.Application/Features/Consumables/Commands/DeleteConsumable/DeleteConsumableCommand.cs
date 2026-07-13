using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Exceptions;
using Tracer.Application.Common.Interfaces;
using Tracer.Domain.Aggregates.InventoryAggregate;

namespace Tracer.Application.Features.Consumables.Commands.DeleteConsumable;

public record DeleteConsumableCommand(int Id) : IRequest;

public sealed class DeleteConsumableCommandHandler : IRequestHandler<DeleteConsumableCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DeleteConsumableCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task Handle(DeleteConsumableCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUserService.CompanyId
            ?? throw new UnauthorizedAccessException("User is not associated with a company.");

        var consumable = await _context.Consumables
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.CompanyId == companyId, cancellationToken)
            ?? throw new NotFoundException(nameof(Consumable), request.Id);

        consumable.SoftDelete();
        await _context.SaveChangesAsync(cancellationToken);
    }
}
