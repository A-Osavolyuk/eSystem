using eShop.Cart.Api.Features.Favorites.Commands;
using eShop.Cart.Api.Features.Favorites.Queries;
using eShop.Domain.Common.Api;
using eShop.Domain.Requests.Api.Favorites;
using Response = eShop.Domain.Common.Api.Response;

namespace eShop.Cart.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class FavoritesController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [EndpointSummary("Update favorites")]
    [EndpointDescription("Updates favorites")]
    [ProducesResponseType(200)]
    [HttpPut("update-favorites")]
    public async ValueTask<ActionResult<Response>> UpdateCartAsync([FromBody] UpdateFavoritesRequest request)
    {
        var response = await sender.Send(new UpdateFavoritesCommand(request));

        return response.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Get favorites")]
    [EndpointDescription("Gets favorites")]
    [ProducesResponseType(200)]
    [HttpGet("get-favorites/{userId:guid}")]
    public async ValueTask<ActionResult<Response>> GetCartAsync(Guid userId)
    {
        var response = await sender.Send(new GetFavoritesQuery(userId));

        return response.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s).Build()),
            ErrorHandler.Handle);
    }
}