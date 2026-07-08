using MediatR;
using Tracer.Shared.Results;

namespace Tracer.Application.Features.Assets.Commands.CreateAsset;

/// <summary>Registers a new asset (Doc 5 POST /api/v1/assets, Doc 8 §2.1).</summary>
public sealed record CreateAssetCommand(
    string AssetTag,
    string Name,
    Guid AssetModelId,
    int StatusLabelId,
    decimal PurchaseCost,
    Guid? LocationId,
    string? SerialNumber,
    DateTime? PurchaseDate,
    Guid? DepreciationId) : IRequest<Result<Guid>>;
