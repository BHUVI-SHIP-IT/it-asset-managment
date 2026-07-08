using FluentAssertions;
using Moq;
using Tracer.Application.Common.Interfaces;
using Tracer.Application.Features.Assets.Queries;
using Tracer.Domain.Aggregates.AssetAggregate;
using Xunit;

namespace Tracer.Application.UnitTests.Features.Assets.Queries;

public class GetAssetByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnAssetDetailDto_WhenAssetExists()
    {
        // Arrange
        var assetId = Guid.NewGuid();
        var repositoryMock = new Mock<IAssetRepository>();
        var asset = Asset.Create(
            "TAG-001",
            "Test Asset",
            Guid.NewGuid(),
            Guid.NewGuid(),
            1,
            100.00m,
            null,
            "SN123",
            DateTime.UtcNow
        );

        // Use reflection to set Id
        var idProperty = typeof(Tracer.Domain.Common.Entity<Guid>).GetProperty("Id");
        idProperty?.SetValue(asset, assetId);

        repositoryMock.Setup(r => r.GetByIdAsync(assetId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(asset);

        var handler = new GetAssetByIdQueryHandler(repositoryMock.Object);
        var query = new GetAssetByIdQuery(assetId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(assetId);
        result.AssetTag.Should().Be("TAG-001");
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenAssetDoesNotExist()
    {
        // Arrange
        var repositoryMock = new Mock<IAssetRepository>();
        repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Asset?)null);

        var handler = new GetAssetByIdQueryHandler(repositoryMock.Object);
        var query = new GetAssetByIdQuery(Guid.NewGuid());

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}
