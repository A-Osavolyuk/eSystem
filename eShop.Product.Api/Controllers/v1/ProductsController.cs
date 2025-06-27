using eShop.Application.Utilities;
using eShop.Domain.Requests.API.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eShop.Product.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[AllowAnonymous]
public class ProductsController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;
    
    [EndpointSummary("Create product")]
    [EndpointDescription("Create product")]
    [ProducesResponseType(200)]
    [HttpPost]
    public async ValueTask<ActionResult<Response>> CreateAsync([FromBody] CreateProductRequest request)
    {
        var result = await sender.Send(new CreateProductCommand(request));
        
        return result.Match(s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message!).Build()),
            ErrorHandler.Handle);
    }
}