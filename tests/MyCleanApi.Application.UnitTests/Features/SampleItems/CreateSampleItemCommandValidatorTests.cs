using FluentAssertions;
using MyCleanApi.Application.Features.SampleItems.Commands.CreateSampleItem;

namespace MyCleanApi.Application.UnitTests.Features.SampleItems;

public class CreateSampleItemCommandValidatorTests
{
    private readonly CreateSampleItemCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenTitleIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new CreateSampleItemCommand(string.Empty, "Description");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateSampleItemCommand.Title));
    }

    [Fact]
    public void Validate_WhenTitleExceedsMaxLength_ShouldHaveValidationError()
    {
        // Arrange
        var command = new CreateSampleItemCommand(new string('a', 201), "Description");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateSampleItemCommand.Title));
    }

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new CreateSampleItemCommand("Valid Title", "Valid Description");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
