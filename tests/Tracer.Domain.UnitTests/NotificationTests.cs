using FluentAssertions;
using Tracer.Domain.Aggregates.NotificationAggregate;
using Xunit;

namespace Tracer.Domain.UnitTests;

/// <summary>
/// Tests for the Notification aggregate root invariants (M6, Doc 5 §3.21).
/// </summary>
public sealed class NotificationTests
{
    private static Notification CreateInfo() =>
        Notification.Create(
            title: "Asset Checked Out",
            body: "Asset ASSET-001 was checked out.",
            severity: NotificationSeverity.Info,
            channel: "Email",
            companyId: Guid.NewGuid(),
            recipient: "admin@tracer.io");

    // ── Create ──────────────────────────────────────────────────────────────

    [Fact]
    public void Create_WithValidData_StartsAsPending()
    {
        var n = CreateInfo();

        n.Status.Should().Be(NotificationStatus.Pending);
        n.IsRead.Should().BeFalse();
        n.SentAtUtc.Should().BeNull();
        n.FailureReason.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyTitle_ThrowsArgumentException(string? title)
    {
        var act = () => Notification.Create(title!, "body", NotificationSeverity.Info, "Email");

        act.Should().Throw<ArgumentException>().WithParameterName("title");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyChannel_ThrowsArgumentException(string? channel)
    {
        var act = () => Notification.Create("Title", "body", NotificationSeverity.Info, channel!);

        act.Should().Throw<ArgumentException>().WithParameterName("channel");
    }

    [Fact]
    public void Create_TrimsTitle()
    {
        var n = Notification.Create("  Alert  ", "body", NotificationSeverity.Warning, "Slack");

        n.Name.Should().Be("Alert");
    }

    // ── MarkSent ────────────────────────────────────────────────────────────

    [Fact]
    public void MarkSent_SetsStatusToSentAndRecordsSentTime()
    {
        var n = CreateInfo();
        var sentAt = DateTime.UtcNow;

        n.MarkSent(sentAt);

        n.Status.Should().Be(NotificationStatus.Sent);
        n.SentAtUtc.Should().Be(sentAt);
        n.FailureReason.Should().BeNull();
    }

    // ── MarkFailed ──────────────────────────────────────────────────────────

    [Fact]
    public void MarkFailed_SetsStatusToFailedAndRecordsReason()
    {
        var n = CreateInfo();

        n.MarkFailed("SMTP timeout");

        n.Status.Should().Be(NotificationStatus.Failed);
        n.FailureReason.Should().Be("SMTP timeout");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void MarkFailed_WithEmptyReason_UsesDefaultReason(string? reason)
    {
        var n = CreateInfo();

        n.MarkFailed(reason!);

        n.FailureReason.Should().Be("Unknown error");
    }

    // ── MarkRead ────────────────────────────────────────────────────────────

    [Fact]
    public void MarkRead_SetsIsReadToTrue()
    {
        var n = CreateInfo();

        n.MarkRead();

        n.IsRead.Should().BeTrue();
    }

    [Fact]
    public void MarkRead_IsIdempotent()
    {
        var n = CreateInfo();

        n.MarkRead();
        n.MarkRead(); // second call must not throw

        n.IsRead.Should().BeTrue();
    }
}
