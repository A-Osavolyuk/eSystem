using eSecurity.Security.Authentication.SignIn;
using eSecurity.Security.Authentication.SignIn.Strategies;

namespace eSecurity.Features.Security.Commands;

public record SignInCommand() : IRequest<Result>
{
    public required SignInPayload Payload { get; set; }
}

public class SignInCommandHandler(ISignInResolver signInResolver) : IRequestHandler<SignInCommand, Result>
{
    private readonly ISignInResolver signInResolver = signInResolver;

    public async Task<Result> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        var type = request.Payload switch
        {
            PasswordSignInPayload => SignInType.Password,
            PasskeySignInPayload => SignInType.Passkey,
            AuthenticatorSignInPayload => SignInType.AuthenticatorApp,
            OAuthSignInPayload => SignInType.OAuth,
            RecoveryCodeSignInPayload => SignInType.RecoveryCode,
            _ => throw new NotSupportedException("Unknown payload")
        };
        if (type == SignInType.OAuth) return Results.BadRequest("Unsupported for manual call");
        
        var strategy = signInResolver.Resolve(type);
        return await strategy.ExecuteAsync(request.Payload, cancellationToken);
    }
}