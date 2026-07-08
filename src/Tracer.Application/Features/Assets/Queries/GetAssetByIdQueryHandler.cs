using MediatR;
using Tracer.Application.Common.Interfaces;
using Tracer.Application.Features.Assets.DTOs;
using Tracer.Application.Features.Assets.Mapping;

namespace Tracer.Application.Features.Assets.Queries;

public class GetAssetByIdQueryHandler : IRequestHandler<GetAssetByIdQuery, AssetDetailDto?>
{
    private readonly IAssetRepository _repository;

    public GetAssetByIdQueryHandler(IAssetRepository repository)
    {
        _repository = repository;
    }

    public async Task<AssetDetailDto?> Handle(GetAssetByIdQuery request, CancellationToken cancellationToken)
    {
        var asset = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return asset?.ToDetailDto();
    }
}
