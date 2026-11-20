using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using MyCleanApi.Application.Common.Interfaces;
using MyCleanApi.Application.Features.SampleItems.Commands.CreateSampleItem;
using MyCleanApi.Domain.Entities;

namespace MyCleanApi.Application.UnitTests.Features.SampleItems;

public class CreateSampleItemCommandHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly DbSet<SampleItem> _dbSet = Substitute.For<DbSet<SampleItem>>();

    public CreateSampleItemCommandHandlerTests()
    {
        _context.SampleItems.Returns(_dbSet);
    }

    [Fact]
    public async Task Handle_WhenValidCommand_ShouldCreateItemAndReturnSuccessResult()
    {
        // Arrange
        _context.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(1));
        var handler = new CreateSampleItemCommandHandler(_context);
        var command = new CreateSampleItemCommand("Test Title", "Test Description");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Title.Should().Be("Test Title");
        result.Value.Description.Should().Be("Test Description");
        _dbSet.Received(1).Add(Arg.Is<SampleItem>(x => x.Title == "Test Title"));
        await _context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
