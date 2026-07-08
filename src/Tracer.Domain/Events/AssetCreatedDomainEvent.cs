using Tracer.Domain.Common;

namespace Tracer.Domain.Events;

/// <summary>Raised when a new asset is registered (Doc 8 §2.1, Doc 2 §2.1.9 OnAssetCreated webhook).</summary>
public sealed record AssetCreatedDomainEvent(Guid AssetId, string AssetTag) : DomainEvent;
