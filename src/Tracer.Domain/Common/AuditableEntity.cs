namespace Tracer.Domain.Common;

/// <summary>
/// Auditable aggregate root. Audit fields are auto-populated by the AuditableEntityInterceptor
/// (Doc 10 §3.3). Soft-delete + optimistic concurrency per Doc 4 §1.2.
/// </summary>
public abstract class AuditableEntity<TId> : Entity<TId>, IAggregateRoot
    where TId : notnull
{
    protected AuditableEntity(TId id) : base(id) { }
    protected AuditableEntity() { }

    public DateTime CreatedAtUtc { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public Guid? UpdatedBy { get; set; }

    // Soft-delete (Doc 4 §1.2) — excluded via EF Core global query filter.
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }

    // Optimistic concurrency (Doc 4 §1.2) — SQL Server rowversion; 409 Conflict on mismatch.
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
