using Tracer.Domain.Common;

namespace Tracer.Domain.Aggregates.CustomFieldAggregate;

/// <summary>
/// Tenant-defined custom field definition (M6, Doc 2 §2.15, Doc 4 §3.20).
/// Extends core entities (e.g. Assets) with enterprise-specific attributes without schema changes.
/// </summary>
public sealed class CustomField : AuditableEntity<Guid>
{
    private CustomField() { }

    private CustomField(Guid id, Guid companyId, string name, CustomFieldType fieldType, bool isRequired, string? options)
        : base(id)
    {
        CompanyId = companyId;
        Name = name;
        FieldType = fieldType;
        IsRequired = isRequired;
        Options = options;
    }

    public Guid CompanyId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public CustomFieldType FieldType { get; private set; }
    public bool IsRequired { get; private set; }

    /// <summary>JSON array of allowed values; only meaningful for <see cref="CustomFieldType.Dropdown"/>.</summary>
    public string? Options { get; private set; }

    public static CustomField Create(Guid companyId, string name, CustomFieldType fieldType, bool isRequired, string? options = null)
    {
        if (companyId == Guid.Empty)
            throw new ArgumentException("CompanyId is required.", nameof(companyId));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));
        if (fieldType == CustomFieldType.Dropdown && string.IsNullOrWhiteSpace(options))
            throw new ArgumentException("Dropdown fields require options.", nameof(options));

        return new CustomField(Guid.NewGuid(), companyId, name.Trim(), fieldType, isRequired, options);
    }

    public void Update(string name, CustomFieldType fieldType, bool isRequired, string? options)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));
        if (fieldType == CustomFieldType.Dropdown && string.IsNullOrWhiteSpace(options))
            throw new ArgumentException("Dropdown fields require options.", nameof(options));

        Name = name.Trim();
        FieldType = fieldType;
        IsRequired = isRequired;
        Options = options;
    }
}
