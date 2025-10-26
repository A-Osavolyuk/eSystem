using eSystem.Core.Common.Errors;
using eSystem.Core.Common.Http;
using eSystem.Product.Api.Features.PriceTypes.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eSystem.Product.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[AllowAnonymous]
public class PriceController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;
    
    [EndpointSummary("Get price types")]
    [EndpointDescription("Get price types")]
    [ProducesResponseType(200)]
    [HttpGet]
    public async ValueTask<IActionResult> GetPriceTypesAsync()
    {
        var result = await sender.Send(new GetPriceTypesQuery());
        
        return result.Match(s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message!).WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }
}