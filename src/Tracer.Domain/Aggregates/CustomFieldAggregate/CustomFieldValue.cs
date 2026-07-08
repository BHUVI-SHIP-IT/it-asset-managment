using Tracer.Domain.Common;

namespace Tracer.Domain.Aggregates.CustomFieldAggregate;

/// <summary>
/// A value assigned to a <see cref="CustomField"/> for a specific owning entity instance
/// (M6, Doc 4 §3.21). <see cref="EntityId"/> is the Guid of the extended record (e.g. an Asset).
/// </summary>
public sealed class CustomFieldValue : AuditableEntity<Guid>
{
    private CustomFieldValue() { }

    private CustomFieldValue(Guid id, Guid customFieldId, Guid entityId, string? value)
        : base(id)
    {
        CustomFieldId = customFieldId;
        EntityId = entityId;
        Value = value;
    }

    public Guid CustomFieldId { get; private set; }
    public Guid EntityId { get; private set; }
    public string? Value { get; private set; }

    public CustomField? CustomField { get; private set; }

    public static CustomFieldValue Create(Guid customFieldId, Guid entityId, string? value)
    {
        if (customFieldId == Guid.Empty)
            throw new ArgumentException("CustomFieldId is required.", nameof(customFieldId));
        if (entityId == Guid.Empty)
            throw new ArgumentException("EntityId is required.", nameof(entityId));

        return new CustomFieldValue(Guid.NewGuid(), customFieldId, entityId, value);
    }

    public void SetValue(string? value) => Value = value;
}
