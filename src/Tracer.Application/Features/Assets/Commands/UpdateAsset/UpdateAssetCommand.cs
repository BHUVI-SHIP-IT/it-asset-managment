using MediatR;
using Tracer.Shared.Results;

namespace Tracer.Application.Features.Assets.Commands.UpdateAsset;

/// <summary>Updates descriptive fields of an existing asset (Doc 5 PUT /api/v1/assets/{id}).</summary>
public sealed record UpdateAssetCommand(
    Guid Id,
    string Name,
    Guid AssetModelId,
    int StatusLabelId,
    decimal PurchaseCost,
    Guid? LocationId,
    string? SerialNumber,
    DateTime? PurchaseDate,
    Guid? DepreciationId,
    string? Notes) : IRequest<Result>;
