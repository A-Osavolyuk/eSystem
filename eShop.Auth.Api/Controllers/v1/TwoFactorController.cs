using eShop.Auth.Api.Features.TwoFactor.Commands;
using eShop.Auth.Api.Features.TwoFactor.Queries;
using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Requests.API.TwoFactor;

namespace eShop.Auth.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[Authorize]
public class TwoFactorController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;
    
    [EndpointSummary("Get two-factor providers")]
    [EndpointDescription("Get two-factor providers")]
    [ProducesResponseType(200)]
    [HttpGet("get-providers")]
    [Authorize]
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
    [Authorize]
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
    [HttpGet("get-state/{email}")]
    [Authorize]
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
    [HttpPost("two-factor-login")]
    [AllowAnonymous]
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
    [HttpPost("change-state")]
    [Authorize(Policy = "UpdateUsersPolicy")]
    public async ValueTask<ActionResult<Response>> ChangeTwoFactorAuthentication(
        [FromBody] ChangeTwoFactorStateRequest request)
    {
        var result = await sender.Send(new ChangeTwoFactorStateCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Send two-factor token")]
    [EndpointDescription("Send two-factor token")]
    [ProducesResponseType(200)]
    [HttpPost("send-token")]
    [Authorize]
    public async ValueTask<ActionResult<Response>> SendTokenAsync(
        [FromBody] SendTwoFactorTokenRequest request)
    {
        var result = await sender.Send(new SendTwoFactorTokenCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Subscribe provider")]
    [EndpointDescription("Subscribe provider")]
    [ProducesResponseType(200)]
    [HttpPost("subscribe-provider")]
    [Authorize(Policy = "UpdateUsersPolicy")]
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
    [Authorize(Policy = "UpdateUsersPolicy")]
    public async ValueTask<ActionResult<Response>> UnsubscribeProviderAsync(
        [FromBody] UnsubscribeProviderRequest request)
    {
        var result = await sender.Send(new UnsubscribeProviderCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
}