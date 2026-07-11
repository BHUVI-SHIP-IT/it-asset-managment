using MediatR;
using Tracer.Application.Features.Assets.DTOs;

namespace Tracer.Application.Features.Assets.Queries;

/// <summary>
/// Returns temporal history rows for an asset (Doc 4 temporal AssetsHistory).
/// Null = asset not found; empty list = asset exists but has no prior versions yet.
/// </summary>
public record GetAssetHistoryQuery(Guid AssetId) : IRequest<IReadOnlyList<AssetHistoryDto>?>;
