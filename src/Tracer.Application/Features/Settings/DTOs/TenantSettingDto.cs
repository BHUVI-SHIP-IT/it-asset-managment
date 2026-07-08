namespace Tracer.Application.Features.Settings.DTOs;

public class TenantSettingDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string? Value { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}
