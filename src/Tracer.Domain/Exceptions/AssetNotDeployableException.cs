namespace Tracer.Domain.Exceptions;

/// <summary>
/// Thrown when a checkout is attempted on an asset whose status is not Deployable (Doc 3 §4.2).
/// </summary>
public sealed class AssetNotDeployableException : DomainException
{
    public override string Code => "Asset.NotDeployable";

    public AssetNotDeployableException(Guid assetId, string currentStatus)
        : base($"Asset '{assetId}' cannot be checked out because its status is '{currentStatus}'. It must be 'Deployable'.")
    {
    }
}
