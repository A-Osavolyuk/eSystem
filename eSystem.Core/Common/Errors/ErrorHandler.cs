using eSystem.Core.Common.Http;
using eSystem.Core.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace eSystem.Core.Common.Errors;

public static class ErrorHandler
{
    public static IActionResult Handle(Result result)
    {
        var error = result.GetError();
        var value = result.Value;
        var response = HttpResponseBuilder.Create()
            .Failed()
            .WithResult(value)
            .WithMessage(error.Details!)
            .Build();
        
        return new ObjectResult(response){ StatusCode = Convert.ToInt32(error.Code) };
    }
}