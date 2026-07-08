using Tracer.Domain.Common;

namespace Tracer.Domain.Aggregates.LicenseAggregate;

/// <summary>
/// LicenseSeat aggregate root (Doc 4 §3.14).
/// Represents an allocated seat of a software license.
/// </summary>
public sealed class LicenseSeat : AuditableEntity<Guid>
{
    private LicenseSeat() { }

    private LicenseSeat(
        Guid id,
        Guid softwareLicenseId,
        Guid? assignedUserId,
        Guid? assignedAssetId) : base(id)
    {
        SoftwareLicenseId = softwareLicenseId;
        AssignedUserId = assignedUserId;
        AssignedAssetId = assignedAssetId;
    }

    public Guid SoftwareLicenseId { get; private set; }
    public Guid? AssignedUserId { get; private set; }
    public Guid? AssignedAssetId { get; private set; }

    public static LicenseSeat AllocateToUser(Guid softwareLicenseId, Guid userId)
    {
        return new LicenseSeat(Guid.NewGuid(), softwareLicenseId, userId, null);
    }

    public static LicenseSeat AllocateToAsset(Guid softwareLicenseId, Guid assetId)
    {
        return new LicenseSeat(Guid.NewGuid(), softwareLicenseId, null, assetId);
    }
}
