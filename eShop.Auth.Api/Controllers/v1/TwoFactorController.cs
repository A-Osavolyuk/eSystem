using eShop.Auth.Api.Features.TwoFactor.Commands;
using eShop.Auth.Api.Features.TwoFactor.Queries;
using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Requests.API.TwoFactor;

namespace eShop.Auth.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class TwoFactorController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;
    
    [EndpointSummary("Get two-factor providers")]
    [EndpointDescription("Get two-factor providers")]
    [ProducesResponseType(200)]
    [HttpGet("get-providers")]
    public async ValueTask<ActionResult<Response>> GetTwoFactorProvidersState()
    {
        var result = await sender.Send(new GetProvidersQuery());

        return result.Match(
            s => Ok(new ResponseBuilder()
                .Succeeded()
                .WithResult(s.Value!)
                .Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Get user two-factor providers")]
    [EndpointDescription("Get user two-factor providers")]
    [ProducesResponseType(200)]
    [HttpGet("get-user-providers/{email}")]
    public async ValueTask<ActionResult<Response>> GetUserTwoFactorProvidersState(string email)
    {
        var result = await sender.Send(new GetUserProvidersQuery(email));

        return result.Match(
            s => Ok(new ResponseBuilder()
                .Succeeded()
                .WithResult(s.Value!)
                .Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Get two-factor state")]
    [EndpointDescription("Get two-factor state")]
    [ProducesResponseType(200)]
    [HttpGet("get-2fa-state/{email}")]
    public async ValueTask<ActionResult<Response>> GetTwoFactorAuthenticationState(string email)
    {
        var result = await sender.Send(new GetTwoFactorStateQuery(email));

        return result.Match(
            s => Ok(new ResponseBuilder()
                .Succeeded()
                .WithResult(s.Value!)
                .Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Login two-factor")]
    [EndpointDescription("Login with two-factor")]
    [ProducesResponseType(200)]
    [HttpPost("2fa-login")]
    public async ValueTask<ActionResult<Response>> LoginWithTwoFactorAuthenticationCode(
        [FromBody] TwoFactorLoginRequest request)
    {
        var result =
            await sender.Send(new TwoFactorLoginCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Change two-factor state")]
    [EndpointDescription("Changes two-factor state")]
    [ProducesResponseType(200)]
    [HttpPost("change-2fa-state")]
    public async ValueTask<ActionResult<Response>> ChangeTwoFactorAuthentication(
        [FromBody] ChangeTwoFactorStateRequest request)
    {
        var result = await sender.Send(new ChangeTwoFactorStateCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Subscribe provider")]
    [EndpointDescription("Subscribe provider")]
    [ProducesResponseType(200)]
    [HttpPost("subscribe-provider")]
    public async ValueTask<ActionResult<Response>> SubscribeProviderAsync(
        [FromBody] SubscribeProviderRequest request)
    {
        var result = await sender.Send(new SubscribeProviderCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Unsubscribe provider")]
    [EndpointDescription("Unsubscribe provider")]
    [ProducesResponseType(200)]
    [HttpPost("unsubscribe-provider")]
    public async ValueTask<ActionResult<Response>> UnsubscribeProviderAsync(
        [FromBody] UnsubscribeProviderRequest request)
    {
        var result = await sender.Send(new UnsubscribeProviderCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
}