using System.Text.Json.Serialization;
using eSecurity.Idp.Security.Authentication.TwoFactor.Authenticator;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Server.Exceptions;

namespace eSecurity.Idp.Features.TwoFactor;

public sealed class VerifyAuthenticatorCommand : IRequest<Result>
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }
    
    [JsonPropertyName("secret")]
    public string? Secret { get; set; }
}

public sealed class VerifyAuthenticatorCommandHandler(
    ICurrentUserAccessor currentUserAccessor) : IRequestHandler<VerifyAuthenticatorCommand, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;

    public async Task<Result> Handle(VerifyAuthenticatorCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Code))
            throw new ValidationException("Code is required");

        if (string.IsNullOrWhiteSpace(request.Secret))
            throw new ValidationException("Secret is required");
        
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

public sealed class VerifyAuthenticatorCommandValidator : IRequestValidator<VerifyAuthenticatorCommand>
{
    public async ValueTask<Result> Validate(VerifyAuthenticatorCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Code))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'code' is required"
            });
        }
        
        if (string.IsNullOrWhiteSpace(request.Secret))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'secret' is required"
            });
        }
        
        return Results.Success(SuccessCodes.Ok);
    }
}