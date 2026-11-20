using FluentValidation;
using MediatR;
using MyCleanApi.Application.Common.Interfaces;
using MyCleanApi.Application.Features.SampleItems.DTOs;
using MyCleanApi.Domain.Common;
using MyCleanApi.Domain.Entities;

namespace MyCleanApi.Application.Features.SampleItems.Commands.CreateSampleItem;

public record CreateSampleItemCommand(string Title, string? Description) : IRequest<Result<SampleItemDto>>;

public class CreateSampleItemCommandValidator : AbstractValidator<CreateSampleItemCommand>
{
    public CreateSampleItemCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");
    }
}

public class CreateSampleItemCommandHandler : IRequestHandler<CreateSampleItemCommand, Result<SampleItemDto>>
{
    private readonly IApplicationDbContext _context;

    public CreateSampleItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<SampleItemDto>> Handle(
        CreateSampleItemCommand request,
        CancellationToken cancellationToken)
    {
        var entity = new SampleItem(request.Title, request.Description);

        _context.SampleItems.Add(entity);
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
