namespace Tracer.Domain.Aggregates.CustomFieldAggregate;

/// <summary>Data type of a tenant-defined custom field (M6, Doc 2 §2.15).</summary>
public enum CustomFieldType
{
    Text = 0,
    Number = 1,
    Date = 2,
    Boolean = 3,
    Dropdown = 4
}
