using eSystem.Core.Common.Errors;
using eSystem.Core.Common.Http;
using eSystem.Core.Requests.Product;
using eSystem.Product.Api.Features.Products.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eSystem.Product.Api.Controllers.v1;

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
    public async ValueTask<IActionResult> CreateAsync([FromBody] CreateProductRequest request)
    {
        var result = await sender.Send(new CreateProductCommand(request));
        
        return result.Match(s => Ok(HttpResponseBuilder.Create().Succeeded().WithMessage(s.Message!).Build()),
            ErrorHandler.Handle);
    }
}