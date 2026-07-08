using Tracer.Domain.Common;

namespace Tracer.Domain.Events;

/// <summary>
/// Raised when an asset is checked out. Triggers the EULA email side-effect via the Outbox
/// (Doc 3 §4.2, Doc 10 §7.3, Doc 2 §2.1.9 OnAssetAssigned).
/// </summary>
public sealed record AssetCheckedOutEvent(Guid AssetId, Guid AssignedUserId, DateTime CheckedOutAtUtc) : DomainEvent;
