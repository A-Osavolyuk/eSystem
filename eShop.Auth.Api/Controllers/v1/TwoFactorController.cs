using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class TwoFactorController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;
    
    [EndpointSummary("Get two-factor state")]
    [EndpointDescription("Get two-factor state")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "ManageAccountPolicy")]
    [HttpGet("get-2fa-state/{email}")]
    public async ValueTask<ActionResult<Response>> GetTwoFactorAuthenticationState(string email)
    {
        var result = await sender.Send(new Get2FaStateQuery(email));

        return result.Match(
            s => Ok(new ResponseBuilder()
                .Succeeded()
                .WithResult(s.Value!)
                .Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Get two-factor providers")]
    [EndpointDescription("Get two-factor providers")]
    [ProducesResponseType(200)]
    [HttpGet("get-providers/{email}")]
    public async ValueTask<ActionResult<Response>> GetTwoFactorProvidersState(string email)
    {
        var result = await sender.Send(new Get2FaStateQuery(email));

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
    [AllowAnonymous]
    [HttpPost("2fa-login")]
    public async ValueTask<ActionResult<Response>> LoginWithTwoFactorAuthenticationCode(
        [FromBody] TwoFactorLoginRequest request)
    {
        var result =
            await sender.Send(new LoginWith2FaCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Change two-factor state")]
    [EndpointDescription("Changes two-factor state")]
    [ProducesResponseType(200)]
    [Authorize(Policy = "ManageAccountPolicy")]
    [HttpPost("change-2fa-state")]
    public async ValueTask<ActionResult<Response>> ChangeTwoFactorAuthentication(
        [FromBody] Change2FaStateRequest request)
    {
        var result = await sender.Send(new Change2FaStateCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithResult(s.Value!).WithMessage(s.Message).Build()),
            ErrorHandler.Handle);
    }
}