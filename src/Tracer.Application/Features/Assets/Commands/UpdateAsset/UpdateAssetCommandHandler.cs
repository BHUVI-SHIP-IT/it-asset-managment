using MediatR;
using Tracer.Application.Common.Exceptions;
using Tracer.Application.Common.Interfaces;
using Tracer.Domain.Aggregates.AssetAggregate;
using Tracer.Shared.Results;

namespace Tracer.Application.Features.Assets.Commands.UpdateAsset;

public sealed class UpdateAssetCommandHandler : IRequestHandler<UpdateAssetCommand, Result>
{
    private readonly IAssetRepository _assetRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAssetCommandHandler(IAssetRepository assetRepository, IUnitOfWork unitOfWork)
    {
        _assetRepository = assetRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateAssetCommand request, CancellationToken cancellationToken)
    {
        var asset = await _assetRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Asset), request.Id);

        asset.UpdateDetails(
            request.Name,
            request.AssetModelId,
            request.StatusLabelId,
            request.PurchaseCost,
            request.LocationId,
            request.SerialNumber,
            request.PurchaseDate,
            request.DepreciationId,
            request.Notes);

        _assetRepository.Update(asset);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
