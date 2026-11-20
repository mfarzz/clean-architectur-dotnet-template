using FluentAssertions;
using MyCleanApi.Domain.Common;
using MyCleanApi.Domain.Entities;

namespace MyCleanApi.Domain.UnitTests.Entities;

public class SampleItemTests
{
    [Fact]
    public void Constructor_ShouldCreateSampleItem_WithDefaultProperties()
    {
        // Arrange & Act
        var item = new SampleItem("Test Title", "Test Description");

        // Assert
        item.Id.Should().NotBeEmpty();
        item.Title.Should().Be("Test Title");
        item.Description.Should().Be("Test Description");
        item.IsCompleted.Should().BeFalse();
    }

    [Fact]
    public void Update_ShouldUpdateProperties()
    {
        // Arrange
        var item = new SampleItem("Old Title", "Old Description");

        // Act
        item.Update("New Title", "New Description", true);

        // Assert
        item.Title.Should().Be("New Title");
        item.Description.Should().Be("New Description");
        item.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public void MarkCompleted_ShouldSetIsCompletedToTrue()
    {
        // Arrange
        var item = new SampleItem("Title", "Description");

        // Act
        item.MarkCompleted();

        // Assert
        item.IsCompleted.Should().BeTrue();
    }
}
