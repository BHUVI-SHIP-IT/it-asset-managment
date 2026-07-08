using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;
using Tracer.Application.Features.AssetModels.DTOs;

namespace Tracer.Application.Features.AssetModels.Queries;

public record GetAllAssetModelsQuery : IRequest<List<AssetModelDto>>;

public class GetAllAssetModelsQueryHandler : IRequestHandler<GetAllAssetModelsQuery, List<AssetModelDto>>
{
    private readonly IApplicationDbContext _context;
    public GetAllAssetModelsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<AssetModelDto>> Handle(GetAllAssetModelsQuery request, CancellationToken cancellationToken)
    {
        return await _context.AssetModels
            .Select(x => new AssetModelDto { Id = x.Id, Name = x.Name })
            .ToListAsync(cancellationToken);
    }
}
