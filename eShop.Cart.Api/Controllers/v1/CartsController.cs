using eShop.Cart.Api.Features.Cart.Commands;
using eShop.Cart.Api.Features.Cart.Queries;
using Response = eShop.Domain.Common.Api.Response;

namespace eShop.Cart.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class CartsController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [EndpointSummary("Update cart")]
    [EndpointDescription("Updates cart")]
    [ProducesResponseType(200)]
    [HttpPut("update-cart")]
    public async ValueTask<ActionResult<Response>> UpdateCartAsync([FromBody] UpdateCartRequest request)
    {
        var response = await sender.Send(new UpdatedCartCommand(request));

        return response.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Get cart")]
    [EndpointDescription("Gets cart")]
    [ProducesResponseType(200)]
    [HttpGet("get-cart/{userId:guid}")]
    public async ValueTask<ActionResult<Response>> GetCartAsync(Guid userId)
    {
        var response = await sender.Send(new GetCartQuery(userId));

        return response.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s).Build()),
            ErrorHandler.Handle);
    }
}