using MediatR;
using Tracer.Shared.Results;

namespace Tracer.Application.Features.Assets.Commands.CheckoutAsset;

/// <summary>Checks an asset out to a user (Doc 3 §4.2, Doc 5 POST /api/v1/assets/{id}/checkout).</summary>
public sealed record CheckoutAssetCommand(Guid AssetId, Guid UserId) : IRequest<Result>;
