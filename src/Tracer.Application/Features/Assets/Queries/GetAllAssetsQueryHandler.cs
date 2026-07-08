using MediatR;
using Tracer.Application.Common.Interfaces;
using Tracer.Application.Features.Assets.DTOs;
using Tracer.Application.Features.Assets.Mapping;
using Tracer.Application.Features.Assets.Specifications;
using Tracer.Shared.Models;

namespace Tracer.Application.Features.Assets.Queries;

public class GetAllAssetsQueryHandler : IRequestHandler<GetAllAssetsQuery, PagedList<AssetDto>>
{
    private readonly IAssetRepository _repository;

    public GetAllAssetsQueryHandler(IAssetRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedList<AssetDto>> Handle(GetAllAssetsQuery request, CancellationToken cancellationToken)
    {
        var spec = new AssetsByFilterSpec(
            request.CompanyId,
            request.SearchTerm,
            request.Status,
            request.StatusLabelId,
            request.LocationId,
            request.Page,
            request.PageSize,
            request.SortBy,
            request.SortDescending,
            forCount: false
        );

        var countSpec = new AssetsByFilterSpec(
            request.CompanyId,
            request.SearchTerm,
            request.Status,
            request.StatusLabelId,
            request.LocationId,
            request.Page,
            request.PageSize,
            request.SortBy,
            request.SortDescending,
            forCount: true
        );

        var count = await _repository.CountAsync(countSpec, cancellationToken);
        var items = await _repository.ListAsync(spec, cancellationToken);

        var dtos = items.Select(x => x.ToDto()).ToList();

        return new PagedList<AssetDto>(dtos, request.Page, request.PageSize, count);
    }
}
