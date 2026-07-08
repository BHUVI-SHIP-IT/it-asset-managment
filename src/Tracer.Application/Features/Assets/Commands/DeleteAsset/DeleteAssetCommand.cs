using MediatR;
using Tracer.Shared.Results;

namespace Tracer.Application.Features.Assets.Commands.DeleteAsset;

/// <summary>Soft-deletes an asset (Doc 4 §1.2 soft delete, Doc 5 DELETE /api/v1/assets/{id}).</summary>
public sealed record DeleteAssetCommand(Guid Id) : IRequest<Result>;
