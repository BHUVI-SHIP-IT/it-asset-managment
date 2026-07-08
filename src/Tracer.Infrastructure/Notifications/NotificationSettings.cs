namespace Tracer.Infrastructure.Notifications;

/// <summary>
/// Default (app-level) notification channel configuration, bound from the "NotificationSettings"
/// configuration section (M6). Per-tenant overrides are read from TenantSettings at dispatch time.
/// Secrets (SMTP password, webhook URLs) must come from configuration/env, never hardcoded.
/// </summary>
public sealed class NotificationSettings
{
    public SlackSettings Slack { get; set; } = new();
    public TeamsSettings Teams { get; set; } = new();
    public EmailSettings Email { get; set; } = new();
}

public sealed class SlackSettings
{
    public bool Enabled { get; set; }
    public string? WebhookUrl { get; set; }
}

public sealed class TeamsSettings
{
    public bool Enabled { get; set; }
    public string? WebhookUrl { get; set; }
}

public sealed class EmailSettings
{
    public bool Enabled { get; set; }
    public string? Host { get; set; }
    public int Port { get; set; } = 587;
    public bool UseSsl { get; set; } = true;
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? FromAddress { get; set; }
    public string? FromName { get; set; }
}
