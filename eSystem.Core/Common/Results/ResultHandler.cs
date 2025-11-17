using Microsoft.AspNetCore.Mvc;

namespace eSystem.Core.Common.Results;

public static class ResultHandler
{
    public static IActionResult Handle(Result result)
    {
        return result.StatusCode switch
        {
            StatusCode.Ok => new OkObjectResult(result.Value),
            StatusCode.Created => new ObjectResult(result.Value) { StatusCode = (int)result.StatusCode },
            StatusCode.Found => new ObjectResult(result.Value) { StatusCode = (int)result.StatusCode },
            StatusCode.SeeOther => new ObjectResult(result.Value) { StatusCode = (int)result.StatusCode },
            StatusCode.BadRequest => new BadRequestObjectResult(result.Value),
            StatusCode.Unauthorized => new UnauthorizedObjectResult(result.Value),
            StatusCode.Forbidden => new ObjectResult(result.Value) { StatusCode = (int)result.StatusCode },
            StatusCode.NotFound => new NotFoundObjectResult(result.Value),
            StatusCode.TooManyRequests => new ObjectResult(result.Value) { StatusCode = (int)result.StatusCode },
            StatusCode.InternalServerError => new ObjectResult(result.Value) { StatusCode = (int)result.StatusCode },
            _ => throw new NotSupportedException("Not supported status code.")
        };
    }
}