using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Tracer.Application.Common.Interfaces;
using Tracer.Application.Common.Models;
using Tracer.Domain.Aggregates.NotificationAggregate;

namespace Tracer.Infrastructure.Notifications;

/// <summary>
/// Delivers notifications to a Slack Incoming Webhook via HTTP POST (M6, real channel).
/// </summary>
public sealed class SlackWebhookChannel : INotificationChannel
{
    private readonly HttpClient _httpClient;
    private readonly SlackSettings _settings;

    public SlackWebhookChannel(HttpClient httpClient, IOptions<NotificationSettings> options)
    {
        _httpClient = httpClient;
        _settings = options.Value.Slack;
    }

    public string ChannelType => "Slack";

    public bool IsConfigured => _settings.Enabled && !string.IsNullOrWhiteSpace(_settings.WebhookUrl);

    public async Task SendAsync(NotificationMessage message, CancellationToken cancellationToken)
    {
        if (!IsConfigured)
            throw new InvalidOperationException("Slack channel is not configured.");

        var emoji = message.Severity switch
        {
            NotificationSeverity.Critical => ":rotating_light:",
            NotificationSeverity.Warning => ":warning:",
            _ => ":information_source:"
        };

        var payload = new
        {
            text = $"{emoji} *{message.Title}*\n{message.Body}"
        };

        using var response = await _httpClient.PostAsJsonAsync(_settings.WebhookUrl, payload, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
