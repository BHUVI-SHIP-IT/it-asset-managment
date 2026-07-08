namespace Tracer.Domain.Exceptions;

/// <summary>
/// Thrown when a check-in is attempted on an asset that is not currently deployed/assigned (Doc 3 §4.2).
/// </summary>
public sealed class AssetAlreadyCheckedInException : DomainException
{
    public override string Code => "Asset.AlreadyCheckedIn";

    public AssetAlreadyCheckedInException(Guid assetId)
        : base($"Asset '{assetId}' cannot be checked in because it is not currently assigned to a user.")
    {
    }
}
