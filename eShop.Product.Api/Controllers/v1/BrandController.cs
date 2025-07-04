using eShop.Product.Api.Features.Brands.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eShop.Product.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[AllowAnonymous]
public class BrandController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;
    
    [EndpointSummary("Get brands")]
    [EndpointDescription("Get brands")]
    [ProducesResponseType(200)]
    [HttpGet]
    public async ValueTask<ActionResult<Response>> GetBrandsAsync()
    {
        var result = await sender.Send(new GetBrandsQuery());
        
        return result.Match(s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message!).WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }
}