using MediatR;
using Microsoft.EntityFrameworkCore;
using MyCleanApi.Application.Common.Interfaces;
using MyCleanApi.Domain.Common;

namespace MyCleanApi.Application.Features.SampleItems.Commands.DeleteSampleItem;

public record DeleteSampleItemCommand(Guid Id) : IRequest<Result>;

public class DeleteSampleItemCommandHandler : IRequestHandler<DeleteSampleItemCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public DeleteSampleItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(
        DeleteSampleItemCommand request,
        CancellationToken cancellationToken)
    {
        var entity = await _context.SampleItems
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            return Result.Failure(
                Error.NotFound("SampleItem.NotFound", $"Sample item with ID '{request.Id}' was not found."));
        }

        _context.SampleItems.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
