using eShop.Domain.Common.Api;

namespace eShop.Application.Utilities;

public static class ErrorHandler
{
    public static ActionResult<Response> Handle(Error error)
    {
        return new ObjectResult(
            new ResponseBuilder()
                .Failed()
                .WithResult(error)
                .WithMessage(error.Message)
                .Build())
        {
            StatusCode = Convert.ToInt32(error.Code),
        };
    }
}