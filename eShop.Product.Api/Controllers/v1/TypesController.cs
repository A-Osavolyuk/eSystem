using eShop.Product.Api.Features.ProductTypes.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eShop.Product.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[AllowAnonymous]
public class TypesController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;
    
    [EndpointSummary("Get product types")]
    [EndpointDescription("Get product types")]
    [ProducesResponseType(200)]
    [HttpGet]
    public async ValueTask<IActionResult> GetTypesAsync()
    {
        var result = await sender.Send(new GetProductTypesQuery());
        
        return result.Match(s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message!).WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }
}