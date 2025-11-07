using eSecurity.Features.Odic.Commands;
using eSystem.Core.Common.Errors;
using eSystem.Core.Common.Http;

namespace eSecurity.Controllers.v1;

[Route("v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class ConnectController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [EndpointSummary("Token")]
    [EndpointDescription("Token")]
    [ProducesResponseType(200)]
    [HttpPost("token")]
    public async ValueTask<IActionResult> TokenAsync(
        [FromQuery(Name = "grant_type")] string grantType, 
        [FromQuery(Name = "client_id")] string clientId,
        [FromQuery(Name = "redirect_uri")] string? redirectUri,
        [FromQuery(Name = "refresh_token")] string? refreshToken,
        [FromQuery(Name = "code")] string? code,
        [FromQuery(Name = "client_secret")] string? clientSecret,
        [FromQuery(Name = "code_verifier")] string? codeVerifier)
    {
        var command = new TokenCommand()
        {
            GrantType = grantType,
            ClientId = clientId,
            RedirectUri = redirectUri,
            ClientSecret = clientSecret,
            Code = code,
            CodeVerifier = codeVerifier,
            RefreshToken = refreshToken
        };

        var result = await sender.Send(command);

        return result.Match(
            s => Ok(HttpResponseBuilder.Create()
                .Succeeded()
                .WithMessage(s.Message)
                .WithResult(s.Value)
                .Build()),
            ErrorHandler.Handle);
    }
}