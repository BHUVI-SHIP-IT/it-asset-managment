using MediatR;
using Tracer.Shared.Results;

namespace Tracer.Application.Features.Assets.Commands.CheckinAsset;

/// <summary>Returns an asset to inventory (Doc 3 §4.2, Doc 5 POST /api/v1/assets/{id}/checkin).</summary>
public sealed record CheckinAssetCommand(Guid AssetId) : IRequest<Result>;
