using System.Text.Json.Serialization;
using eSecurity.Core.Security.Authentication.SignIn;
using eSecurity.Idp.Security.Authentication.SignIn;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using FluentValidation;

namespace eSecurity.Idp.Features.Account;

public sealed class SignInCommand : IRequest<Result>
{
    [JsonPropertyName("payload")]
    public SignInPayload? Payload { get; set; } = null!;
}

public class SignInCommandHandler(ISignInResolver signInResolver) : IRequestHandler<SignInCommand, Result>
{
    private readonly ISignInResolver _signInResolver = signInResolver;

    public async Task<Result> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        var type = request.Payload switch
        {
            PasswordSignInPayload => SignInType.Password,
            PasskeySignInPayload => SignInType.Passkey,
            OAuthSignInPayload => SignInType.OAuth,
            TwoFactorSignInPayload => SignInType.TwoFactor,
            _ => throw new NotSupportedException("Unknown payload")
        };
        
        if (type == SignInType.OAuth) return Results.ClientError(ClientErrorCode.BadRequest, new Error
        {
            Code = ErrorCode.BadRequest,
            Description = "Unsupported for manual call"
        });
        
        var strategy = _signInResolver.Resolve(type);
        return await strategy.ExecuteAsync(request.Payload, cancellationToken);
    }
}

public sealed class SignInCommandValidator(IServiceProvider serviceProvider) : IRequestValidator<SignInCommand>
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public async ValueTask<Result> Validate(SignInCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.Payload is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'payload' is required"
            });
        }

        var payloadType = request.Payload.GetType();
        var validatorType = typeof(IValidator<>).MakeGenericType(payloadType);
        var validator = (IValidator<SignInPayload>) _serviceProvider.GetRequiredService(validatorType);
        var validationResult = await validator.ValidateAsync(request.Payload, cancellationToken);
        if (!validationResult.IsValid)
        {
            var error = validationResult.Errors.FirstOrDefault()?.ErrorMessage;
            if (!string.IsNullOrWhiteSpace(error))
            {
                return Results.ClientError(ClientErrorCode.BadRequest, new Error()
                {
                    Code = ErrorCode.InvalidRequest,
                    Description = error
                });
            }
        }
        
        return Results.Success(SuccessCodes.Ok);
    }
}