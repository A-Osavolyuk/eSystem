using eSystem.Auth.Api.Features.Passkeys.Commands;
using eSystem.Auth.Api.Features.Passkeys.Queries;
using eSystem.Core.Common.Errors;
using eSystem.Core.Common.Http;
using eSystem.Core.Requests.Auth;

namespace eSystem.Auth.Api.Controllers.v1;

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
    [HttpPost("options/attestation")]
    [Authorize]
    public async ValueTask<IActionResult> GenerateCreationOptionsAsync([FromBody] GenerateCreationOptionsRequest request)
    {
        var result = await sender.Send(new GenerateCreationOptionsCommand(request));

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
            ErrorHandler.Handle);
    }
    
    [EndpointSummary("Sign in with passkey")]
    [EndpointDescription("Sign in with passkey")]
    [ProducesResponseType(200)]
    [HttpPost("options/assertion")]
    [AllowAnonymous]
    public async ValueTask<IActionResult> GenerateRequestOptionsAsync([FromBody] GenerateRequestOptionsRequest requestOptionsRequest)
    {
        var result = await sender.Send(new GenerateRequestOptionsCommand(requestOptionsRequest));

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Verify passkey")]
    [EndpointDescription("Verify passkey")]
    [ProducesResponseType(200)]
    [HttpPost("create")]
    [Authorize]
    public async ValueTask<IActionResult> CreatePasskeyAsync([FromBody] CreatePasskeyRequest request)
    {
        var result = await sender.Send(new CreatePasskeyCommand(request));

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
    public async ValueTask<IActionResult> RemovePasskeyAsync([FromBody] RemovePasskeyRequest request)
    {
        var result = await sender.Send(new RemovePasskeyCommand(request));

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
            ErrorHandler.Handle);
    }

    [EndpointSummary("Set passkey name")]
    [EndpointDescription("Set passkey name")]
    [ProducesResponseType(200)]
    [HttpPost("set-name")]
    [Authorize]
    public async ValueTask<IActionResult> SetPasskeyNameAsync([FromBody] ChangePasskeyNameRequest request)
    {
        var result = await sender.Send(new ChangePasskeyNameCommand(request));

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
            ErrorHandler.Handle);
    }
}