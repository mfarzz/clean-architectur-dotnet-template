using FluentAssertions;
using MyCleanApi.Domain.Common;

namespace MyCleanApi.Domain.UnitTests.Common;

public class ResultTests
{
    [Fact]
    public void Success_ShouldCreateSuccessfulResult()
    {
        // Act
        var result = Result.Success();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void Failure_ShouldCreateFailureResult()
    {
        // Arrange
        var error = Error.NotFound("Item.NotFound", "Item was not found");

        // Act
        var result = Result.Failure(error);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void SuccessWithValue_ShouldReturnValue()
    {
        // Act
        var result = Result.Success(42);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
    }

    [Fact]
    public void FailureWithValue_ShouldThrowWhenAccessingValue()
    {
        // Arrange
        var result = Result.Failure<int>(Error.Failure("Code", "Description"));

        // Act & Assert
        var act = () => _ = result.Value;
        act.Should().Throw<InvalidOperationException>();
    }
}
