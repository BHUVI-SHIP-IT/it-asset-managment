using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;
using Tracer.Domain.Entities;

namespace Tracer.Application.Features.AssetModels.Commands;

/// <summary>Creates an asset model. CompanyId comes from the authenticated tenant context.</summary>
public record CreateAssetModelCommand(string Name) : IRequest<Guid>;

public class CreateAssetModelCommandHandler : IRequestHandler<CreateAssetModelCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateAssetModelCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(CreateAssetModelCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId
            ?? throw new UnauthorizedAccessException("No tenant context is available for the current user.");

        // AssetModel requires Manufacturer + Category FKs; use tenant defaults when UI only sends a name.
        var manufacturerId = await _context.Manufacturers
            .Where(m => m.CompanyId == companyId)
            .Select(m => m.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (manufacturerId == Guid.Empty)
            throw new InvalidOperationException("Create a manufacturer before creating asset models.");

        var categoryId = await _context.Categories
            .Where(c => c.CompanyId == companyId)
            .Select(c => c.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (categoryId == Guid.Empty)
            throw new InvalidOperationException("Create a category before creating asset models.");

        var entity = new AssetModel(Guid.NewGuid())
        {
            Name = request.Name.Trim(),
            CompanyId = companyId,
            ManufacturerId = manufacturerId,
            CategoryId = categoryId
        };

        _context.AssetModels.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
