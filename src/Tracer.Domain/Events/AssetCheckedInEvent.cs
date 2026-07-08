using Tracer.Domain.Common;

namespace Tracer.Domain.Events;

/// <summary>Raised when an asset is returned to inventory (Doc 3 §4.2, Doc 8).</summary>
public sealed record AssetCheckedInEvent(Guid AssetId, Guid PreviousUserId, DateTime CheckedInAtUtc) : DomainEvent;
