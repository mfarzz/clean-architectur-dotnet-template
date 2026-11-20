namespace MyCleanApi.Application.Features.SampleItems.DTOs;

public record SampleItemDto(
    Guid Id,
    string Title,
    string? Description,
    bool IsCompleted,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);
