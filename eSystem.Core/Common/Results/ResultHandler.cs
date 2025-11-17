using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace eSystem.Core.Common.Results;

public static class ResultHandler
{
    public static IActionResult Handle(Result result)
    {
        return result.StatusCode switch
        {
            HttpStatusCode.OK => new OkObjectResult(result.Value),
            HttpStatusCode.Created => new ObjectResult(result.Value) { StatusCode = (int)result.StatusCode },
            HttpStatusCode.Found => new ObjectResult(result.Value) { StatusCode = (int)result.StatusCode },
            HttpStatusCode.SeeOther => new ObjectResult(result.Value) { StatusCode = (int)result.StatusCode },
            HttpStatusCode.BadRequest => new BadRequestObjectResult(result.GetError()),
            HttpStatusCode.Unauthorized => new UnauthorizedObjectResult(result.GetError()),
            HttpStatusCode.Forbidden => new ObjectResult(result.GetError()) { StatusCode = (int)result.StatusCode },
            HttpStatusCode.NotFound => new NotFoundObjectResult(result.GetError()),
            HttpStatusCode.TooManyRequests => new ObjectResult(result.GetError()) { StatusCode = (int)result.StatusCode },
            HttpStatusCode.InternalServerError => new ObjectResult(result.GetError()) { StatusCode = (int)result.StatusCode },
            _ => throw new NotSupportedException("Not supported status code.")
        };
    }
}