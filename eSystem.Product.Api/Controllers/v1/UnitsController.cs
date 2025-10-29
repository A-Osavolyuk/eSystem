using eSystem.Core.Common.Errors;
using eSystem.Core.Common.Http;
using eSystem.Product.Api.Features.Units.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eSystem.Product.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[AllowAnonymous]
public class UnitsController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;
    
    [EndpointSummary("Get units")]
    [EndpointDescription("Get units")]
    [ProducesResponseType(200)]
    [HttpGet]
    public async ValueTask<IActionResult> GetUnitsAsync()
    {
        var result = await sender.Send(new GetUnitsQuery());
        
        return result.Match(s => Ok(HttpResponseBuilder.Create().Succeeded().WithMessage(s.Message!).WithResult(s.Value!).Build()),
            ErrorHandler.Handle);
    }
}