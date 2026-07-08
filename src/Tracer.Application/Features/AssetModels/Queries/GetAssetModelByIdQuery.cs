using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;
using Tracer.Application.Features.AssetModels.DTOs;

namespace Tracer.Application.Features.AssetModels.Queries;

public record GetAssetModelByIdQuery(Guid Id) : IRequest<AssetModelDto?>;

public class GetAssetModelByIdQueryHandler : IRequestHandler<GetAssetModelByIdQuery, AssetModelDto?>
{
    private readonly IApplicationDbContext _context;
    public GetAssetModelByIdQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<AssetModelDto?> Handle(GetAssetModelByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.AssetModels.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity == null) return null;
        return new AssetModelDto { Id = entity.Id, Name = entity.Name };
    }
}
