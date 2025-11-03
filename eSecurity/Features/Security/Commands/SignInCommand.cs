using eSecurity.Security.Authentication.SignIn;
using eSecurity.Security.Authentication.SignIn.Strategies;
using eSystem.Core.Requests.Auth;
using eSystem.Core.Security.Authentication.SignIn;
using eSystem.Core.Security.Authentication.SignIn.Payloads;
using eSystem.Core.Security.Credentials.PublicKey;

namespace eSecurity.Features.Security.Commands;

public record SignInCommand(SignInRequest Request) : IRequest<Result>;

public class SignInCommandHandler(ISignInResolver signInResolver) : IRequestHandler<SignInCommand, Result>
{
    private readonly ISignInResolver signInResolver = signInResolver;

    public async Task<Result> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        var type = request.Request.Payload switch
        {
            PasswordSignInPayload => SignInType.Password,
            PasskeySignInPayload => SignInType.Passkey,
            AuthenticatorSignInPayload => SignInType.AuthenticatorApp,
            OAuthSignInPayload => SignInType.OAuth,
            _ => throw new NotSupportedException("Unknown payload")
        };
        if (type == SignInType.OAuth) return Results.BadRequest("Unsupported for manual call");
        
        var strategy = signInResolver.Resolve(type);
        return await strategy.SignInAsync(request.Request.Payload, cancellationToken);
    }
}