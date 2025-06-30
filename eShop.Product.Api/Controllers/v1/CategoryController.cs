using eShop.Product.Api.Features.Categories.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eShop.Product.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[AllowAnonymous]
public class CategoryController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;
    
    [EndpointSummary("Get categories")]
    [EndpointDescription("Get categories")]
    [ProducesResponseType(200)]
    [HttpGet]
    public async ValueTask<ActionResult<Response>> GetCategoriesAsync()
    {
        var result = await sender.Send(new GetCategoriesQuery());
        
        return result.Match(s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message!).WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }
}