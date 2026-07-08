using FluentAssertions;
using Tracer.Domain.Aggregates.SettingAggregate;
using Xunit;

namespace Tracer.Domain.UnitTests;

/// <summary>
/// Tests for the TenantSetting aggregate invariants (M6, Doc 5 §settings).
/// </summary>
public sealed class TenantSettingTests
{
    private static readonly Guid _companyId = Guid.NewGuid();

    // ── Create ──────────────────────────────────────────────────────────────

    [Fact]
    public void Create_WithValidData_SetsProperties()
    {
        var setting = TenantSetting.Create(_companyId, "Notifications.Slack.Enabled", "true");

        setting.CompanyId.Should().Be(_companyId);
        setting.Key.Should().Be("Notifications.Slack.Enabled");
        setting.Value.Should().Be("true");
    }

    [Fact]
    public void Create_TrimsKey()
    {
        var setting = TenantSetting.Create(_companyId, "  my.key  ", null);

        setting.Key.Should().Be("my.key");
    }

    [Fact]
    public void Create_WithNullValue_IsAllowed()
    {
        var setting = TenantSetting.Create(_companyId, "some.key", null);

        setting.Value.Should().BeNull();
    }

    [Fact]
    public void Create_WithEmptyCompanyId_ThrowsArgumentException()
    {
        var act = () => TenantSetting.Create(Guid.Empty, "key", "value");

        act.Should().Throw<ArgumentException>().WithParameterName("companyId");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyKey_ThrowsArgumentException(string? key)
    {
        var act = () => TenantSetting.Create(_companyId, key!, "value");

        act.Should().Throw<ArgumentException>().WithParameterName("key");
    }

    // ── UpdateValue ─────────────────────────────────────────────────────────

    [Fact]
    public void UpdateValue_ChangesValue()
    {
        var setting = TenantSetting.Create(_companyId, "some.key", "old");

        setting.UpdateValue("new");

        setting.Value.Should().Be("new");
    }

    [Fact]
    public void UpdateValue_ToNull_ClearsValue()
    {
        var setting = TenantSetting.Create(_companyId, "some.key", "has-value");

        setting.UpdateValue(null);

        setting.Value.Should().BeNull();
    }
}
