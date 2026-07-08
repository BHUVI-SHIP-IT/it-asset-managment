using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using Tracer.Application.Common.Interfaces;
using Tracer.Application.Common.Models;

namespace Tracer.Infrastructure.Notifications;

/// <summary>
/// Delivers notifications as email over SMTP (M6, real channel — Doc 10 §Services/SmtpEmailService).
/// </summary>
public sealed class SmtpEmailChannel : INotificationChannel
{
    private readonly EmailSettings _settings;

    public SmtpEmailChannel(IOptions<NotificationSettings> options)
    {
        _settings = options.Value.Email;
    }

    public string ChannelType => "Email";

    public bool IsConfigured =>
        _settings.Enabled &&
        !string.IsNullOrWhiteSpace(_settings.Host) &&
        !string.IsNullOrWhiteSpace(_settings.FromAddress);

    public async Task SendAsync(NotificationMessage message, CancellationToken cancellationToken)
    {
        if (!IsConfigured)
            throw new InvalidOperationException("Email channel is not configured.");

        var recipient = message.Recipient;
        if (string.IsNullOrWhiteSpace(recipient))
            throw new InvalidOperationException("Email channel requires a recipient address.");

        using var client = new SmtpClient(_settings.Host, _settings.Port)
        {
            EnableSsl = _settings.UseSsl,
            Credentials = string.IsNullOrWhiteSpace(_settings.Username)
                ? CredentialCache.DefaultNetworkCredentials
                : new NetworkCredential(_settings.Username, _settings.Password)
        };

        using var mail = new MailMessage
        {
            From = new MailAddress(_settings.FromAddress!, _settings.FromName ?? "Tracer"),
            Subject = message.Title,
            Body = message.Body,
            IsBodyHtml = false
        };
        mail.To.Add(recipient);

        await client.SendMailAsync(mail, cancellationToken);
    }
}
