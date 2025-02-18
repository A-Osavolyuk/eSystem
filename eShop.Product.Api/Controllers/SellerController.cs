using eShop.Product.Api.Features.Sellers.Commands;
using Response = eShop.Domain.Common.Api.Response;

namespace eShop.Product.Api.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class SellerController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [EndpointSummary("Register seller")]
    [EndpointDescription("Registers new seller")]
    [ProducesResponseType(200)]
    [HttpPost("register-seller")]
    public async ValueTask<ActionResult<Response>> RegisterSeller([FromBody] RegisterSellerRequest request)
    {
        var response = await sender.Send(new RegisterSellerCommand(request));

        return response.Match(
            s => StatusCode(201, new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ExceptionHandler.HandleException);
    }
}