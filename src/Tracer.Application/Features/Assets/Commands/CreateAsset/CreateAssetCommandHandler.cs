using MediatR;
using Tracer.Application.Common.Interfaces;
using Tracer.Domain.Aggregates.AssetAggregate;
using Tracer.Shared.Results;

namespace Tracer.Application.Features.Assets.Commands.CreateAsset;

public sealed class CreateAssetCommandHandler : IRequestHandler<CreateAssetCommand, Result<Guid>>
{
    private readonly IAssetRepository _assetRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public CreateAssetCommandHandler(
        IAssetRepository assetRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _assetRepository = assetRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<Guid>> Handle(CreateAssetCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId
            ?? throw new UnauthorizedAccessException("No tenant context is available for the current user.");

        // Uniqueness invariant (Doc 2 §2.1.7 → 409/422 on duplicate tag).
        var tagExists = await _assetRepository.AssetTagExistsAsync(
            request.AssetTag, companyId, excludeAssetId: null, cancellationToken);

        if (tagExists)
        {
            return Result.Failure<Guid>(
                Error.Conflict("Asset.DuplicateTag", $"The AssetTag '{request.AssetTag}' is already in use."));
        }

        var asset = Asset.Create(
            request.AssetTag,
            request.Name,
            companyId,
            request.AssetModelId,
            request.StatusLabelId,
            request.PurchaseCost,
            request.LocationId,
            request.SerialNumber,
            request.PurchaseDate,
            request.DepreciationId);

        await _assetRepository.AddAsync(asset, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(asset.Id);
    }
}
