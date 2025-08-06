using eShop.Domain.Common.API;

namespace eShop.Application.Utilities;

public static class ErrorHandler
{
    public static ActionResult<Response> Handle(Result result)
    {
        var error = result.GetError();
        var value = result.Value;

        return error.Code switch
        {
            ErrorCode.Found => new RedirectResult(value?.ToString() ?? throw new ArgumentException("Redirect URL was not provided")),
            _ => new ObjectResult(new ResponseBuilder()
                    .Failed()
                    .WithResult(value)
                    .WithMessage(error.Details!)
                    .Build()) { StatusCode = Convert.ToInt32(error.Code) }
        };
    }
}