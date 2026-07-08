using System;
using FluentAssertions;
using Tracer.Domain.Aggregates.InventoryAggregate;
using Xunit;

namespace Tracer.Domain.UnitTests.Aggregates.InventoryAggregate;

public class ConsumableTests
{
    [Fact]
    public void Checkout_Should_Decrease_TotalQuantity_When_Sufficient_Stock()
    {
        // Arrange
        var consumable = Consumable.Create("Printer Toner", Guid.NewGuid(), 10, 150.00m);

        // Act
        consumable.Checkout(4);

        // Assert
        consumable.TotalQuantity.Should().Be(6);
    }

    [Fact]
    public void Checkout_Should_Throw_InvalidOperationException_When_Insufficient_Stock()
    {
        // Arrange
        var consumable = Consumable.Create("Printer Toner", Guid.NewGuid(), 5, 150.00m);

        // Act
        var action = () => consumable.Checkout(10);

        // Assert
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Insufficient stock*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Checkout_Should_Throw_ArgumentException_When_Quantity_Is_Invalid(int invalidQuantity)
    {
        // Arrange
        var consumable = Consumable.Create("Printer Toner", Guid.NewGuid(), 5, 150.00m);

        // Act
        var action = () => consumable.Checkout(invalidQuantity);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("Checkout quantity must be greater than zero.*");
    }
}
