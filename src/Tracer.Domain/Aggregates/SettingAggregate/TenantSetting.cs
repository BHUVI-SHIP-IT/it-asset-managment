using Tracer.Domain.Common;

namespace Tracer.Domain.Aggregates.SettingAggregate;

/// <summary>
/// Tenant-scoped configuration key/value (M6, Doc 5 settings module).
/// Backs per-tenant notification rules and channel enablement (e.g. "Notifications.Slack.Enabled" = "true",
/// "Notifications.Slack.WebhookUrl" = "https://...").
/// </summary>
public sealed class TenantSetting : AuditableEntity<Guid>
{
    private TenantSetting() { }

    private TenantSetting(Guid id, Guid companyId, string key, string? value)
        : base(id)
    {
        CompanyId = companyId;
        Key = key;
        Value = value;
    }

    public Guid CompanyId { get; private set; }
    public string Key { get; private set; } = string.Empty;
    public string? Value { get; private set; }

    public static TenantSetting Create(Guid companyId, string key, string? value)
    {
        if (companyId == Guid.Empty)
            throw new ArgumentException("CompanyId is required.", nameof(companyId));
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key is required.", nameof(key));

        return new TenantSetting(Guid.NewGuid(), companyId, key.Trim(), value);
    }

    public void UpdateValue(string? value) => Value = value;
}
