using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Tracer.Application.Common.Interfaces;
using Tracer.Application.Common.Models;
using Tracer.Domain.Aggregates.NotificationAggregate;

namespace Tracer.Infrastructure.Notifications;

/// <summary>
/// Delivers notifications to a Microsoft Teams Incoming Webhook using the MessageCard format (M6, real channel).
/// </summary>
public sealed class TeamsWebhookChannel : INotificationChannel
{
    private readonly HttpClient _httpClient;
    private readonly TeamsSettings _settings;

    public TeamsWebhookChannel(HttpClient httpClient, IOptions<NotificationSettings> options)
    {
        _httpClient = httpClient;
        _settings = options.Value.Teams;
    }

    public string ChannelType => "Teams";

    public bool IsConfigured => _settings.Enabled && !string.IsNullOrWhiteSpace(_settings.WebhookUrl);

    public async Task SendAsync(NotificationMessage message, CancellationToken cancellationToken)
    {
        if (!IsConfigured)
            throw new InvalidOperationException("Teams channel is not configured.");

        var themeColor = message.Severity switch
        {
            NotificationSeverity.Critical => "D93025",
            NotificationSeverity.Warning => "F9AB00",
            _ => "1A73E8"
        };

        var payload = new Dictionary<string, object?>
        {
            ["@type"] = "MessageCard",
            ["@context"] = "http://schema.org/extensions",
            ["themeColor"] = themeColor,
            ["summary"] = message.Title,
            ["title"] = message.Title,
            ["text"] = message.Body
        };

        using var response = await _httpClient.PostAsJsonAsync(_settings.WebhookUrl, payload, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
