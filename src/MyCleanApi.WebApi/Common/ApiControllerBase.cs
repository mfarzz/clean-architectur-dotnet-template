using MediatR;
using Microsoft.AspNetCore.Mvc;
using MyCleanApi.Domain.Common;

namespace MyCleanApi.WebApi.Common;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender? _mediator;

    protected ISender Mediator =>
        _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    protected ActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return result.Value is null ? NoContent() : Ok(result.Value);
        }

        return MapErrorToActionResult(result.Error);
    }

    protected ActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
        {
            return NoContent();
        }

        return MapErrorToActionResult(result.Error);
    }

    private ActionResult MapErrorToActionResult(Error error)
    {
        return error.Type switch
        {
            ErrorType.NotFound => NotFound(new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = "Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = error.Description,
                Extensions = { ["errorCode"] = error.Code }
            }),
            ErrorType.Conflict => Conflict(new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                Title = "Conflict",
                Status = StatusCodes.Status409Conflict,
                Detail = error.Description,
                Extensions = { ["errorCode"] = error.Code }
            }),
            ErrorType.Validation => BadRequest(new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = error.Description,
                Extensions = { ["errorCode"] = error.Code }
            }),
            ErrorType.Unauthorized => Unauthorized(),
            ErrorType.Forbidden => Forbid(),
            _ => BadRequest(new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = error.Description,
                Extensions = { ["errorCode"] = error.Code }
            })
        };
    }
}
