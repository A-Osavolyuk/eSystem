using eShop.Auth.Api.Features.Passkeys.Commands;
using eShop.Auth.Api.Features.Passkeys.Queries;
using eShop.Domain.Common.Http;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[Authorize]
public class PasskeyController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [EndpointSummary("Get passkey")]
    [EndpointDescription("Get passkey")]
    [ProducesResponseType(200)]
    [HttpGet("{id:guid}")]
    [Authorize]
    public async ValueTask<IActionResult> GetPasskeyAsync(Guid id)
    {
        var result = await sender.Send(new GetPasskeyQuery(id));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Create passkey")]
    [EndpointDescription("Create passkey")]
    [ProducesResponseType(200)]
    [HttpPost("create")]
    [Authorize]
    public async ValueTask<IActionResult> CreatePasskeyAsync([FromBody] CreatePasskeyRequest request)
    {
        var result = await sender.Send(new CreatePasskeyCommand(request, HttpContext));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Verify passkey")]
    [EndpointDescription("Verify passkey")]
    [ProducesResponseType(200)]
    [HttpPost("verify")]
    [Authorize]
    public async ValueTask<IActionResult> VerifyPasskeyAsync([FromBody] VerifyPasskeyRequest request)
    {
        var result = await sender.Send(new VerifyPasskeyCommand(request, HttpContext));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Sign in with passkey")]
    [EndpointDescription("Sign in with passkey")]
    [ProducesResponseType(200)]
    [HttpPost("sign-in/options")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> PasskeySignInAsync([FromBody] PasskeySignInRequest request)
    {
        var result = await sender.Send(new PasskeySignInCommand(request, HttpContext));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Verify sign in with passkey")]
    [EndpointDescription("Verify sign in with passkey")]
    [ProducesResponseType(200)]
    [HttpPost("sign-in/verify")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> VerifyPasskeySignInAsync([FromBody] VerifyPasskeySignInRequest request)
    {
        var result = await sender.Send(new VerifyPasskeySignInCommand(request, HttpContext));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Remove passkey")]
    [EndpointDescription("Remove passkey")]
    [ProducesResponseType(200)]
    [HttpPost("remove")]
    [Authorize]
    public async ValueTask<IActionResult> RemovePasskeyAsync([FromBody] RemovePasskeyRequest request)
    {
        var result = await sender.Send(new RemovePasskeyCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Set passkey name")]
    [EndpointDescription("Set passkey name")]
    [ProducesResponseType(200)]
    [HttpPost("set-name")]
    [Authorize]
    public async ValueTask<IActionResult> SetPasskeyNameAsync([FromBody] SetPasskeyNameRequest request)
    {
        var result = await sender.Send(new SetPasskeyNameCommand(request));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }
}