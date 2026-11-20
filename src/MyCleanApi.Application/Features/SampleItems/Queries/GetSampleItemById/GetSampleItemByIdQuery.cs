using MediatR;
using Microsoft.EntityFrameworkCore;
using MyCleanApi.Application.Common.Interfaces;
using MyCleanApi.Application.Features.SampleItems.DTOs;
using MyCleanApi.Domain.Common;

namespace MyCleanApi.Application.Features.SampleItems.Queries.GetSampleItemById;

public record GetSampleItemByIdQuery(Guid Id) : IRequest<Result<SampleItemDto>>;

public class GetSampleItemByIdQueryHandler : IRequestHandler<GetSampleItemByIdQuery, Result<SampleItemDto>>
{
    private readonly IApplicationDbContext _context;

    public GetSampleItemByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<SampleItemDto>> Handle(
        GetSampleItemByIdQuery request,
        CancellationToken cancellationToken)
    {
        var item = await _context.SampleItems
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new SampleItemDto(
                x.Id,
                x.Title,
                x.Description,
                x.IsCompleted,
                x.CreatedAt,
                x.UpdatedAt))
            .FirstOrDefaultAsync(cancellationToken);

        if (item is null)
        {
            return Result.Failure<SampleItemDto>(
                Error.NotFound("SampleItem.NotFound", $"Sample item with ID '{request.Id}' was not found."));
        }

        return Result.Success(item);
    }
}
