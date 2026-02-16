using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Authentication.SignIn;
using eSecurity.Server.Security.Authentication.SignIn;
using eSystem.Core.Mediator;

namespace eSecurity.Server.Features.Account.Commands;

public record SignInCommand(SignInRequest Request) : IRequest<Result>;

public class SignInCommandHandler(ISignInResolver signInResolver) : IRequestHandler<SignInCommand, Result>
{
    private readonly ISignInResolver _signInResolver = signInResolver;

    public async Task<Result> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        var type = request.Request.Payload switch
        {
            PasswordSignInPayload => SignInType.Password,
            PasskeySignInPayload => SignInType.Passkey,
            OAuthSignInPayload => SignInType.OAuth,
            TwoFactorSignInPayload => SignInType.TwoFactor,
            _ => throw new NotSupportedException("Unknown payload")
        };
        if (type == SignInType.OAuth) return Results.BadRequest("Unsupported for manual call");
        
        var strategy = _signInResolver.Resolve(type);
        return await strategy.ExecuteAsync(request.Request.Payload, cancellationToken);
    }
}