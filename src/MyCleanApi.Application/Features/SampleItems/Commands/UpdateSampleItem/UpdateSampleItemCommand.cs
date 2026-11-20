using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyCleanApi.Application.Common.Interfaces;
using MyCleanApi.Application.Features.SampleItems.DTOs;
using MyCleanApi.Domain.Common;

namespace MyCleanApi.Application.Features.SampleItems.Commands.UpdateSampleItem;

public record UpdateSampleItemCommand(
    Guid Id,
    string Title,
    string? Description,
    bool IsCompleted) : IRequest<Result<SampleItemDto>>;

public class UpdateSampleItemCommandValidator : AbstractValidator<UpdateSampleItemCommand>
{
    public UpdateSampleItemCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");
    }
}

public class UpdateSampleItemCommandHandler : IRequestHandler<UpdateSampleItemCommand, Result<SampleItemDto>>
{
    private readonly IApplicationDbContext _context;

    public UpdateSampleItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<SampleItemDto>> Handle(
        UpdateSampleItemCommand request,
        CancellationToken cancellationToken)
    {
        var entity = await _context.SampleItems
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            return Result.Failure<SampleItemDto>(
                Error.NotFound("SampleItem.NotFound", $"Sample item with ID '{request.Id}' was not found."));
        }

        entity.Update(request.Title, request.Description, request.IsCompleted);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = new SampleItemDto(
            entity.Id,
            entity.Title,
            entity.Description,
            entity.IsCompleted,
            entity.CreatedAt,
            entity.UpdatedAt);

        return Result.Success(dto);
    }
}
