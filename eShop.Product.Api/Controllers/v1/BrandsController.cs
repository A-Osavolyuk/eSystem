using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Product;
using eShop.Product.Api.Features.Brands.Commands;
using eShop.Product.Api.Features.Brands.Queries;
using Response = eShop.Domain.Common.API.Response;

namespace eShop.Product.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class BrandsController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [EndpointSummary("Get brands")]
    [EndpointDescription("Gets all brands")]
    [ProducesResponseType(200)]
    [HttpGet("get-brands")]
    public async ValueTask<ActionResult<Response>> GetBrandAsync()
    {
        var response = await sender.Send(new GetBrandsQuery());

        return response.Match(
            s =>
            {
                var response1 = new ResponseBuilder().Succeeded().WithResult(s).Build();
                return Ok(response1);
            },
            ErrorHandler.Handle);
    }

    [EndpointSummary("Create brand")]
    [EndpointDescription("Creates a brand")]
    [ProducesResponseType(200)]
    [HttpPost("create-brand")]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> CreateBrandAsync([FromBody] CreateBrandRequest request)
    {
        var response = await sender.Send(new CreateBrandCommand(request));

        return response.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Update brand")]
    [EndpointDescription("Updates the brand")]
    [ProducesResponseType(200)]
    [HttpPut("update-brand")]
    [ValidationFilter]
    public async ValueTask<ActionResult<Response>> UpdateBrandAsync([FromBody] UpdateBrandRequest request)
    {
        var response = await sender.Send(new UpdateBrandCommand(request));

        return response.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Delete brand")]
    [EndpointDescription("Deletes the brand")]
    [ProducesResponseType(200)]
    [HttpDelete("delete-brand")]
    public async ValueTask<ActionResult<Response>> DeleteBrandAsync([FromBody] DeleteBrandRequest request)
    {
        var response = await sender.Send(new DeleteBrandCommand(request));

        return response.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
}