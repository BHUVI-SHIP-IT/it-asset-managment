using FluentAssertions;
using Tracer.Domain.Aggregates.AssetAggregate;
using Tracer.Domain.Events;
using Tracer.Domain.Exceptions;
using Xunit;

namespace Tracer.Domain.UnitTests;

/// <summary>
/// Tests for the Asset aggregate root invariants (Doc 3 §4.2, Doc 10 §7.4).
/// </summary>
public sealed class AssetTests
{
    private static Asset CreateDeployableAsset() =>
        Asset.Create(
            assetTag: "ASSET-001",
            name: "Dell Latitude 5540",
            companyId: Guid.NewGuid(),
            assetModelId: Guid.NewGuid(),
            statusLabelId: 1,
            purchaseCost: 1299.99m);

    // ── Create ──

    [Fact]
    public void Create_WithValidData_ReturnsDeployableAsset()
    {
        var asset = CreateDeployableAsset();

        asset.Status.Should().Be(AssetStatus.Deployable);
        asset.AssetTag.Should().Be("ASSET-001");
        asset.AssignedUserId.Should().BeNull();
    }

    [Fact]
    public void Create_RaisesAssetCreatedDomainEvent()
    {
        var asset = CreateDeployableAsset();

        asset.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<AssetCreatedDomainEvent>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyAssetTag_ThrowsArgumentException(string? tag)
    {
        var act = () => Asset.Create(tag!, "Name", Guid.NewGuid(), Guid.NewGuid(), 1, 100m);

        act.Should().Throw<ArgumentException>().WithParameterName("assetTag");
    }

    [Fact]
    public void Create_WithNegativeCost_ThrowsArgumentException()
    {
        var act = () => Asset.Create("TAG", "Name", Guid.NewGuid(), Guid.NewGuid(), 1, -1m);

        act.Should().Throw<ArgumentException>().WithParameterName("purchaseCost");
    }

    // ── Checkout ──

    [Fact]
    public void Checkout_WhenDeployable_SetsStatusToDeployedAndAssignsUser()
    {
        var asset = CreateDeployableAsset();
        var userId = Guid.NewGuid();

        asset.Checkout(userId);

        asset.Status.Should().Be(AssetStatus.Deployed);
        asset.AssignedUserId.Should().Be(userId);
        asset.CheckedOutAtUtc.Should().NotBeNull();
    }

    [Fact]
    public void Checkout_RaisesAssetCheckedOutEvent()
    {
        var asset = CreateDeployableAsset();
        asset.ClearDomainEvents(); // Clear the Created event.

        asset.Checkout(Guid.NewGuid());

        asset.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<AssetCheckedOutEvent>();
    }

    [Fact]
    public void Checkout_WhenAlreadyDeployed_ThrowsAssetNotDeployableException()
    {
        var asset = CreateDeployableAsset();
        asset.Checkout(Guid.NewGuid());

        var act = () => asset.Checkout(Guid.NewGuid());

        act.Should().Throw<AssetNotDeployableException>();
    }

    // ── Checkin ──

    [Fact]
    public void Checkin_WhenDeployed_SetsStatusToDeployableAndClearsUser()
    {
        var asset = CreateDeployableAsset();
        asset.Checkout(Guid.NewGuid());

        asset.Checkin();

        asset.Status.Should().Be(AssetStatus.Deployable);
        asset.AssignedUserId.Should().BeNull();
        asset.LastCheckinAtUtc.Should().NotBeNull();
    }

    [Fact]
    public void Checkin_WhenNotDeployed_ThrowsAssetAlreadyCheckedInException()
    {
        var asset = CreateDeployableAsset();

        var act = () => asset.Checkin();

        act.Should().Throw<AssetAlreadyCheckedInException>();
    }

    // ── Transfer ──

    [Fact]
    public void Transfer_WhenDeployed_ChangesAssignedUser()
    {
        var asset = CreateDeployableAsset();
        var user1 = Guid.NewGuid();
        var user2 = Guid.NewGuid();
        asset.Checkout(user1);

        asset.Transfer(user2);

        asset.AssignedUserId.Should().Be(user2);
        asset.Status.Should().Be(AssetStatus.Deployed);
    }

    // ── Retire ──

    [Fact]
    public void Retire_WhenDeployable_SetsStatusToArchived()
    {
        var asset = CreateDeployableAsset();

        asset.Retire();

        asset.Status.Should().Be(AssetStatus.Archived);
    }

    [Fact]
    public void Retire_WhenDeployed_Throws()
    {
        var asset = CreateDeployableAsset();
        asset.Checkout(Guid.NewGuid());

        var act = () => asset.Retire();

        act.Should().Throw<AssetAlreadyCheckedInException>();
    }
}
