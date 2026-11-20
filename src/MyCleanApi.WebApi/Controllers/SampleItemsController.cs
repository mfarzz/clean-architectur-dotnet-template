using Microsoft.AspNetCore.Mvc;
using MyCleanApi.Application.Common.Models;
using MyCleanApi.Application.Features.SampleItems.Commands.CreateSampleItem;
using MyCleanApi.Application.Features.SampleItems.Commands.DeleteSampleItem;
using MyCleanApi.Application.Features.SampleItems.Commands.UpdateSampleItem;
using MyCleanApi.Application.Features.SampleItems.DTOs;
using MyCleanApi.Application.Features.SampleItems.Queries.GetSampleItemById;
using MyCleanApi.Application.Features.SampleItems.Queries.GetSampleItems;
using MyCleanApi.WebApi.Common;

namespace MyCleanApi.WebApi.Controllers;

public class SampleItemsController : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<SampleItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetItems([FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetSampleItemsQuery(pagination), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SampleItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetSampleItemByIdQuery(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(SampleItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Create([FromBody] CreateSampleItemCommand command, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
        }
        return HandleResult(result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(SampleItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateSampleItemCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Bad Request",
                Detail = "Route id must match command id."
            });
        }

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new DeleteSampleItemCommand(id), cancellationToken);
        return HandleResult(result);
    }
}
