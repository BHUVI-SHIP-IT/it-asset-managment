namespace Tracer.Domain.Aggregates.AssetAggregate;

/// <summary>
/// Asset meta-status state machine (Doc 2 §2, Doc 3 §4.2, Doc 7 lifecycle actions).
/// Governs which lifecycle transitions are legal:
///   Deployable --checkout--> Deployed --checkin--> Deployable
///   Deployable/Deployed --retire--> Archived (terminal)
/// </summary>
public enum AssetStatus
{
    /// <summary>Pending intake from procurement; not yet ready to deploy.</summary>
    Pending = 0,

    /// <summary>Ready to be checked out to a user.</summary>
    Deployable = 1,

    /// <summary>Currently assigned to a user.</summary>
    Deployed = 2,

    /// <summary>Undergoing maintenance/repair; not deployable.</summary>
    Maintenance = 3,

    /// <summary>Retired / end-of-life. Terminal state (Doc 2 §2.1.4 "deployed entities cannot be soft-deleted").</summary>
    Archived = 4
}
