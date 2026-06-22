using System.Text.Json.Serialization;
using eSecurity.Idp.Security.Authentication.TwoFactor.Authenticator;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.TwoFactor;

public record VerifyAuthenticatorCommand : IRequest<Result>
{
    [JsonPropertyName("code")]
    public required string Code { get; set; }
    
    [JsonPropertyName("secret")]
    public required string Secret { get; set; }
}

public class VerifyAuthenticatorCommandHandler(
    ICurrentUserAccessor currentUserAccessor) : IRequestHandler<VerifyAuthenticatorCommand, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;

    public async Task<Result> Handle(VerifyAuthenticatorCommand request, CancellationToken cancellationToken)
    {
        var userResult = await _currentUserAccessor.GetCurrentUserAsync(cancellationToken);
        if (!userResult.Succeeded)
        {
            var error = userResult.GetError();
            return Results.ClientError(ClientErrorCode.Unauthorized, error);
        }

        if (!userResult.TryGetValue(out var user))
        {
            return Results.ClientError(ClientErrorCode.Unauthorized, new Error()
            {
                Code = ErrorCode.Unauthorized,
                Description = "Unauthorized"
            });
        }

        var verified = AuthenticatorUtils.VerifyCode(request.Code, request.Secret);
        return verified
            ? Results.Success(SuccessCodes.Ok)
            : Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.BadRequest,
                Description = "Invalid code."
            });
    }
}