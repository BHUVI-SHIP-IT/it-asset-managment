namespace Tracer.Application.Features.CustomFields.DTOs;

public class CustomFieldDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FieldType { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public string? Options { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}

public class CustomFieldValueDto
{
    public Guid Id { get; set; }
    public Guid CustomFieldId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string FieldType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public string? Value { get; set; }
}
