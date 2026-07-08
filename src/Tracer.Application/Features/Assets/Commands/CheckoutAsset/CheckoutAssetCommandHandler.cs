using MediatR;
using Tracer.Application.Common.Exceptions;
using Tracer.Application.Common.Interfaces;
using Tracer.Domain.Aggregates.AssetAggregate;
using Tracer.Shared.Results;

namespace Tracer.Application.Features.Assets.Commands.CheckoutAsset;

/// <summary>
/// Orchestrates checkout (Doc 3 §4.2). The "must be Deployable" invariant lives in the aggregate
/// (Asset.Checkout), which raises AssetCheckedOutEvent → Outbox → EULA email (Doc 10 §4.2).
/// </summary>
public sealed class CheckoutAssetCommandHandler : IRequestHandler<CheckoutAssetCommand, Result>
{
    private readonly IAssetRepository _assetRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CheckoutAssetCommandHandler(IAssetRepository assetRepository, IUnitOfWork unitOfWork)
    {
        _assetRepository = assetRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CheckoutAssetCommand request, CancellationToken cancellationToken)
    {
        var asset = await _assetRepository.GetByIdAsync(request.AssetId, cancellationToken)
            ?? throw new NotFoundException(nameof(Asset), request.AssetId);

        // Throws AssetNotDeployableException (→ 422) if the asset is not in a checkout-able state.
        asset.Checkout(request.UserId);

        _assetRepository.Update(asset);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
