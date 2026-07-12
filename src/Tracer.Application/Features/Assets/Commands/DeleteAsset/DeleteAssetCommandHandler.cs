using MediatR;
using Tracer.Application.Common.Interfaces;
using Tracer.Shared.Results;

namespace Tracer.Application.Features.Assets.Commands.DeleteAsset;

/// <summary>
/// Soft-deletes an asset via <see cref="IAssetRepository.Remove"/>;
/// the auditable interceptor converts the hard delete into <c>IsDeleted = true</c>.
/// </summary>
public sealed class DeleteAssetCommandHandler : IRequestHandler<DeleteAssetCommand, Result>
{
    private readonly IAssetRepository _assetRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAssetCommandHandler(IAssetRepository assetRepository, IUnitOfWork unitOfWork)
    {
        _assetRepository = assetRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteAssetCommand request, CancellationToken cancellationToken)
    {
        var asset = await _assetRepository.GetByIdAsync(request.Id, cancellationToken);
        if (asset is null)
        {
            return Result.Failure(
                Error.NotFound("Assets.NotFound", $"Asset '{request.Id}' was not found."));
        }

        _assetRepository.Remove(asset);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
