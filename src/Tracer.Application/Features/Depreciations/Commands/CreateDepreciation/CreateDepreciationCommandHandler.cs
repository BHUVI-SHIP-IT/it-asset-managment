using MediatR;
using Tracer.Application.Common.Interfaces;
using Tracer.Domain.Aggregates.DepreciationAggregate;
using Tracer.Shared.Results;

namespace Tracer.Application.Features.Depreciations.Commands.CreateDepreciation;

public sealed class CreateDepreciationCommandHandler : IRequestHandler<CreateDepreciationCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateDepreciationCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<Guid>> Handle(CreateDepreciationCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId
            ?? throw new UnauthorizedAccessException("No tenant context is available for the current user.");

        var depreciation = Depreciation.Create(
            request.Name,
            request.Months,
            companyId,
            request.MinimumValue);

        _context.Depreciations.Add(depreciation);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(depreciation.Id);
    }
}
