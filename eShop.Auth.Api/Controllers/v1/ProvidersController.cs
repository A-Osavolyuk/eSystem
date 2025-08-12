using eShop.Auth.Api.Features.Providers.Commands;
using eShop.Auth.Api.Features.Providers.Queries;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[Authorize]
public class ProvidersController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;
    
    [EndpointSummary("Get two-factor providers")]
    [EndpointDescription("Get two-factor providers")]
    [ProducesResponseType(200)]
    [HttpGet]
    [AllowAnonymous]
    public async ValueTask<IActionResult> GetTwoFactorProvidersState()
    {
        var result = await sender.Send(new GetProvidersQuery());

        return result.Match(
            s => Ok(new ResponseBuilder()
                .Succeeded()
                .WithResult(s.Value!)
                .Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Subscribe provider")]
    [EndpointDescription("Subscribe provider")]
    [ProducesResponseType(200)]
    [HttpPost("subscribe")]
    public async ValueTask<IActionResult> SubscribeProviderAsync(
        [FromBody] SubscribeProviderRequest request)
    {
        var result = await sender.Send(new SubscribeProviderCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Verify provider")]
    [EndpointDescription("Verify provider")]
    [ProducesResponseType(200)]
    [HttpPost("verify")]
    public async ValueTask<IActionResult> VerifyProviderAsync(
        [FromBody] VerifyProviderRequest request)
    {
        var result = await sender.Send(new VerifyProviderCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Unsubscribe provider")]
    [EndpointDescription("Unsubscribe provider")]
    [ProducesResponseType(200)]
    [HttpPost("unsubscribe")]
    public async ValueTask<IActionResult> UnsubscribeProviderAsync(
        [FromBody] UnsubscribeProviderRequest request)
    {
        var result = await sender.Send(new UnsubscribeProviderCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
}