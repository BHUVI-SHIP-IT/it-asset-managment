using FluentAssertions;
using Tracer.Domain.Aggregates.DepreciationAggregate;
using Xunit;

namespace Tracer.Domain.UnitTests;

public class DepreciationTests
{
    [Fact]
    public void Create_WithValidData_ReturnsDepreciation()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        
        // Act
        var depreciation = Depreciation.Create("Straight Line 3 Years", 36, companyId, 100m);

        // Assert
        depreciation.Should().NotBeNull();
        depreciation.Name.Should().Be("Straight Line 3 Years");
        depreciation.Months.Should().Be(36);
        depreciation.CompanyId.Should().Be(companyId);
        depreciation.MinimumValue.Should().Be(100m);
    }

    [Theory]
    [InlineData("", 36, 100)]
    [InlineData("   ", 36, 100)]
    [InlineData(null, 36, 100)]
    public void Create_WithInvalidName_ThrowsArgumentException(string? name, int months, decimal minVal)
    {
        // Act
        Action act = () => Depreciation.Create(name!, months, Guid.NewGuid(), minVal);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("*Name is required*");
    }

    [Fact]
    public void Create_WithInvalidMonths_ThrowsArgumentException()
    {
        Action act = () => Depreciation.Create("Test", 0, Guid.NewGuid(), 100m);
        act.Should().Throw<ArgumentException>().WithMessage("*Months must be greater than zero*");
    }

    [Fact]
    public void Create_WithNegativeMinimumValue_ThrowsArgumentException()
    {
        Action act = () => Depreciation.Create("Test", 36, Guid.NewGuid(), -1m);
        act.Should().Throw<ArgumentException>().WithMessage("*Minimum value cannot be negative*");
    }
}
