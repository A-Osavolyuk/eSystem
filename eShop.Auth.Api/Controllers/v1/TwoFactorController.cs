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
    
    [EndpointSummary("Get two-factor state")]
    [EndpointDescription("Get two-factor state")]
    [ProducesResponseType(200)]
    [HttpGet("state/{id:guid}")]
    [Authorize]
    public async ValueTask<ActionResult<Response>> GetTwoFactorAuthenticationState(Guid id)
    {
        var result = await sender.Send(new GetTwoFactorStateQuery(id));

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
}