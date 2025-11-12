using eSecurity.Core.Common.Requests;
using eSecurity.Server.Features.Passkeys.Commands;
using eSystem.Core.Common.Errors;
using eSystem.Core.Common.Http;

namespace eSecurity.Server.Controllers.v1;

[Route("v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class PasskeyController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;
    
    [EndpointSummary("Generate public key credential creation options")]
    [EndpointDescription("Generate public key credential creation options")]
    [ProducesResponseType(200)]
    [HttpPost("options/creation")]
    [Authorize]
    public async ValueTask<IActionResult> GenerateCreationOptionsAsync(
        [FromBody] GenerateCreationOptionsRequest request)
    {
        var result = await _sender.Send(new GenerateCreationOptionsCommand(request));

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Generate public key credential request options")]
    [EndpointDescription("Generate public key credential request options")]
    [ProducesResponseType(200)]
    [HttpPost("options/request")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> GenerateRequestOptionsAsync([FromBody] GenerateRequestOptionsRequest request)
    {
        var result = await _sender.Send(new GenerateRequestOptionsCommand(request));

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Create passkey")]
    [EndpointDescription("Create passkey")]
    [ProducesResponseType(200)]
    [HttpPost("create")]
    [Authorize]
    public async ValueTask<IActionResult> CreateAsync([FromBody] CreatePasskeyRequest request)
    {
        var result = await _sender.Send(new CreatePasskeyCommand(request));

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Remove passkey")]
    [EndpointDescription("Remove passkey")]
    [ProducesResponseType(200)]
    [HttpPost("remove")]
    [Authorize]
    public async ValueTask<IActionResult> RemoveAsync([FromBody] RemovePasskeyRequest request)
    {
        var result = await _sender.Send(new RemovePasskeyCommand(request));

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Change passkey display name")]
    [EndpointDescription("Change passkey display name")]
    [ProducesResponseType(200)]
    [HttpPost("change-name")]
    [Authorize]
    public async ValueTask<IActionResult> ChangeNameAsync([FromBody] ChangePasskeyNameRequest request)
    {
        var result = await _sender.Send(new ChangePasskeyNameCommand(request));

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
            ErrorHandler.Handle);
    }
}