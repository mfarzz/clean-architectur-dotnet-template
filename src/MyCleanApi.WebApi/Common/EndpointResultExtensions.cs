using MyCleanApi.Domain.Common;

namespace MyCleanApi.WebApi.Common;

public static class EndpointResultExtensions
{
    public static IResult ToHttpResult<T>(this Result<T> result, string? uri = null)
    {
        if (result.IsSuccess)
        {
            if (!string.IsNullOrEmpty(uri))
            {
                return Results.Created(uri, result.Value);
            }
            return Results.Ok(result.Value);
        }

        return MapErrorToResult(result.Error);
    }

    public static IResult ToHttpResult(this Result result)
    {
        if (result.IsSuccess)
        {
            return Results.NoContent();
        }

        return MapErrorToResult(result.Error);
    }

    private static IResult MapErrorToResult(Error error)
    {
        return error.Type switch
        {
            ErrorType.NotFound => Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Not Found",
                detail: error.Description,
                extensions: new Dictionary<string, object?> { ["errorCode"] = error.Code }),

            ErrorType.Conflict => Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Conflict",
                detail: error.Description,
                extensions: new Dictionary<string, object?> { ["errorCode"] = error.Code }),

            ErrorType.Validation => Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Bad Request",
                detail: error.Description,
                extensions: new Dictionary<string, object?> { ["errorCode"] = error.Code }),

            ErrorType.Unauthorized => Results.Unauthorized(),
            ErrorType.Forbidden => Results.Forbid(),

            _ => Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Bad Request",
                detail: error.Description,
                extensions: new Dictionary<string, object?> { ["errorCode"] = error.Code })
        };
    }
}
