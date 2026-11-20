using MediatR;
using Microsoft.EntityFrameworkCore;
using MyCleanApi.Application.Common.Interfaces;
using MyCleanApi.Application.Common.Models;
using MyCleanApi.Application.Features.SampleItems.DTOs;
using MyCleanApi.Domain.Common;

namespace MyCleanApi.Application.Features.SampleItems.Queries.GetSampleItems;

public record GetSampleItemsQuery(PaginationParams Pagination) : IRequest<Result<PaginatedList<SampleItemDto>>>;

public class GetSampleItemsQueryHandler : IRequestHandler<GetSampleItemsQuery, Result<PaginatedList<SampleItemDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetSampleItemsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<SampleItemDto>>> Handle(
        GetSampleItemsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.SampleItems
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new SampleItemDto(
                x.Id,
                x.Title,
                x.Description,
                x.IsCompleted,
                x.CreatedAt,
                x.UpdatedAt));

        var paginatedList = await PaginatedList<SampleItemDto>.CreateAsync(
            query,
            request.Pagination.PageNumber,
            request.Pagination.PageSize,
            cancellationToken);

        return Result.Success(paginatedList);
    }
}
