using eSecurity.Security.Identity.SignUp;
using eSecurity.Security.Identity.SignUp.Strategies;
using eSystem.Core.Requests.Auth;

namespace eSecurity.Features.Security.Commands;

public sealed record SignUpCommand(SignUpRequest Request) : IRequest<Result>;

public sealed class SignUpCommandHandler(ISignUpResolver resolver) : IRequestHandler<SignUpCommand, Result>
{
    private readonly ISignUpResolver resolver = resolver;

    public async Task<Result> Handle(SignUpCommand request,
        CancellationToken cancellationToken)
    {
        var strategy = resolver.Resolve(SignUpType.Manual);

        var payload = new ManualSignUpPayload()
        {
            Username = request.Request.Username,
            Email = request.Request.Email,
            Password = request.Request.Password
        };
        
        return await strategy.SignUpAsync(payload, cancellationToken);
    }
}