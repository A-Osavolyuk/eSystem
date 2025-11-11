using eSecurity.Core.Common.Requests;
using eSecurity.Server.Security.Identity.SignUp;
using eSecurity.Server.Security.Identity.SignUp.Strategies;

namespace eSecurity.Server.Features.Account.Commands;

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
        
        return await strategy.ExecuteAsync(payload, cancellationToken);
    }
}