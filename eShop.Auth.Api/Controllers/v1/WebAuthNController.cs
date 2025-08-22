using eShop.Auth.Api.Features.Credentials;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[Authorize]
public class WebAuthNController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [EndpointSummary("Create public key credential options")]
    [EndpointDescription("Create public key credential options")]
    [ProducesResponseType(200)]
    [HttpPost("credential/options")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> CreatePublicKeyCredentialAsync(
        [FromBody] CreatePublicKeyCredentialRequest request)
    {
        var result = await sender.Send(new CreatePublicKeyCredentialCommand(request, HttpContext));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Verify public key credential")]
    [EndpointDescription("Verify public key credential")]
    [ProducesResponseType(200)]
    [HttpPost("credential/verification")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> VerifyPublicKeyCredentialAsync(
        [FromBody] VerifyPublicKeyCredentialRequest request)
    {
        var result = await sender.Send(new VerifyPublicKeyCredentialCommand(request, HttpContext));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Create credential request options")]
    [EndpointDescription("Create credential request options")]
    [ProducesResponseType(200)]
    [HttpPost("assertion/options")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> CreateCredentialRequestOptionsAsync(
        [FromBody] CreatePublicKeyCredentialRequestOptionsRequest request)
    {
        var result = await sender.Send(new CreatePublicKeyCredentialRequestOptionsCommand(request, HttpContext));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Verify credential request options")]
    [EndpointDescription("Verify credential request options")]
    [ProducesResponseType(200)]
    [HttpPost("assertion/verification")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> CreateCredentialRequestOptionsAsync(
        [FromBody] VerifyPublicKeyCredentialRequestOptionsRequest request)
    {
        var result = await sender.Send(new VerifyPublicKeyCredentialRequestOptionsCommand(request, HttpContext));

        return result.Match(
            s => Ok(new ResponseBuilder().Succeeded().WithMessage(s.Message).WithResult(s.Value).Build()),
            ErrorHandler.Handle);
    }
}