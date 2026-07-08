using MediatR;
using Tracer.Application.Features.Assets.DTOs;
using Tracer.Domain.Aggregates.AssetAggregate;
using Tracer.Shared.Models;

namespace Tracer.Application.Features.Assets.Queries;

public record GetAllAssetsQuery(
    Guid CompanyId,
    string? SearchTerm,
    AssetStatus? Status,
    int? StatusLabelId,
    Guid? LocationId,
    int Page = 1,
    int PageSize = 10,
    string? SortBy = null,
    bool SortDescending = false
) : IRequest<PagedList<AssetDto>>;
