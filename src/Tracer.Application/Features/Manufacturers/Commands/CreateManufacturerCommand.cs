using MediatR;
using Tracer.Application.Common.Interfaces;
using Tracer.Domain.Entities;

namespace Tracer.Application.Features.Manufacturers.Commands;

/// <summary>Creates a manufacturer. CompanyId comes from the authenticated tenant context.</summary>
public record CreateManufacturerCommand(string Name) : IRequest<Guid>;

public class CreateManufacturerCommandHandler : IRequestHandler<CreateManufacturerCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateManufacturerCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(CreateManufacturerCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId
            ?? throw new UnauthorizedAccessException("No tenant context is available for the current user.");

        var entity = new Manufacturer(Guid.NewGuid())
        {
            Name = request.Name.Trim(),
            CompanyId = companyId
        };

        _context.Manufacturers.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
