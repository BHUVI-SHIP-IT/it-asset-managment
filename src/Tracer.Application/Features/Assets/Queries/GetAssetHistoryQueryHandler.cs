using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;
using Tracer.Application.Features.Assets.DTOs;

namespace Tracer.Application.Features.Assets.Queries;

public sealed class GetAssetHistoryQueryHandler
    : IRequestHandler<GetAssetHistoryQuery, IReadOnlyList<AssetHistoryDto>?>
{
    private readonly IAssetRepository _repository;

    public GetAssetHistoryQueryHandler(IAssetRepository repository) => _repository = repository;

    public async Task<IReadOnlyList<AssetHistoryDto>?> Handle(
        GetAssetHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var exists = await _repository.GetByIdAsync(request.AssetId, cancellationToken);

        if (exists is null)
            return null;

        return await _repository.GetHistoryAsync(request.AssetId, cancellationToken);
    }
}
