using MediatR;
using MyCleanApi.Application.Common.Models;
using MyCleanApi.Application.Features.SampleItems.Commands.CreateSampleItem;
using MyCleanApi.Application.Features.SampleItems.Commands.DeleteSampleItem;
using MyCleanApi.Application.Features.SampleItems.Commands.UpdateSampleItem;
using MyCleanApi.Application.Features.SampleItems.Queries.GetSampleItemById;
using MyCleanApi.Application.Features.SampleItems.Queries.GetSampleItems;
using MyCleanApi.WebApi.Common;

namespace MyCleanApi.WebApi.Endpoints;

public static class SampleItemEndpoints
{
    public static IEndpointRouteBuilder MapSampleItemEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/sampleitems")
            .WithTags("SampleItems");

        group.MapGet("/", async ([AsParameters] PaginationParams pagination, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetSampleItemsQuery(pagination), ct);
            return result.ToHttpResult();
        })
        .WithName("GetSampleItems")
        .WithSummary("Get paginated sample items");

        group.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetSampleItemByIdQuery(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetSampleItemById")
        .WithSummary("Get sample item by ID");

        group.MapPost("/", async (CreateSampleItemCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return result.ToHttpResult($"/api/sampleitems/{result.Value?.Id}");
        })
        .WithName("CreateSampleItem")
        .WithSummary("Create a new sample item");

        group.MapPut("/{id:guid}", async (Guid id, UpdateSampleItemCommand command, ISender sender, CancellationToken ct) =>
        {
            if (id != command.Id)
            {
                return Results.BadRequest(new { error = "Route id must match command id." });
            }

            var result = await sender.Send(command, ct);
            return result.ToHttpResult();
        })
        .WithName("UpdateSampleItem")
        .WithSummary("Update an existing sample item");

        group.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new DeleteSampleItemCommand(id), ct);
            return result.ToHttpResult();
        })
        .WithName("DeleteSampleItem")
        .WithSummary("Delete a sample item");

        return app;
    }
}
