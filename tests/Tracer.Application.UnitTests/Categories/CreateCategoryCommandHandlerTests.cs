using Microsoft.EntityFrameworkCore;
using Moq;
using Tracer.Application.Common.Interfaces;
using Tracer.Application.Features.Categories.Commands;
using Tracer.Domain.Entities;
using Xunit;

namespace Tracer.Application.UnitTests.Categories;

public class CreateCategoryCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_CreateCategory_And_ReturnId()
    {
        // Arrange
        var contextMock = new Mock<IApplicationDbContext>();
        var dbSetMock = new Mock<DbSet<Category>>();
        var currentUserMock = new Mock<ICurrentUserService>();
        var companyId = Guid.NewGuid();

        contextMock.Setup(c => c.Categories).Returns(dbSetMock.Object);
        currentUserMock.Setup(u => u.CompanyId).Returns(companyId);

        var handler = new CreateCategoryCommandHandler(contextMock.Object, currentUserMock.Object);
        var command = new CreateCategoryCommand("Laptops");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        dbSetMock.Verify(x => x.Add(It.IsAny<Category>()), Times.Once);
        contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
